using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Authentication;

namespace Betfair.ESAClient.Auth;

/// <summary>
/// Utility class to provide a session & token via identity SSO
/// </summary>
public class AppKeyAndSessionProvider : IAppKeyAndSessionProvider
{
    /// <summary>
    /// Proof-of-build marker for the Interactive Login contract (form body, not query string).
    /// </summary>
    public const string SsoLoginImplementationId = "form-urlencoded-post-body-v2";

    private string _appkey;
    private string _host;
    private string _password;
    private string _username;

    private AppKeyAndSession _session;

    public const string SSO_HOST_COM = "identitysso.betfair.com";
    public const string SSO_HOST_IT = "identitysso.betfair.it";
    public const string SSO_HOST_ES = "identitysso.betfair.es";

    public AppKeyAndSessionProvider(string ssoHost, string appkey, string username, string password) {
        _host = ssoHost;
        _appkey = appkey;
        _username = username;
        _password = password;
        Timeout = TimeSpan.FromSeconds(30);
        //4hrs is normal expire time
        SessionExpireTime = TimeSpan.FromHours(3);
    }

    /// <summary>
    /// AppKey being used
    /// </summary>
    public string Appkey {
        get { return _appkey; }
    }

    /// <summary>
    /// Session expire time (default 3hrs)
    /// </summary>
    public TimeSpan SessionExpireTime { get; set; }

    /// <summary>
    /// Specifies the timeout
    /// </summary>
    public TimeSpan Timeout { get; set; }


    /// <summary>
    /// Constructs a new session token via identity SSO.
    /// Note: These are not cached.
    /// </summary>
    /// <exception cref="InvalidCredentialException">Thrown if authentication response is fail</exception>
    /// <exception cref="IOException">Thrown if authentication call fails</exception>
    /// <returns></returns>
    public AppKeyAndSession GetOrCreateNewSession() {
        if (_session != null) {
            //have a cached session - is it expired
            if ((_session.CreateTime + SessionExpireTime) > DateTime.UtcNow) {
                Trace.TraceInformation("SSO Login - session not expired - re-using");
                return _session;
            }
            else {
                Trace.TraceInformation("SSO Login - session expired");
            }
        }

        Trace.TraceInformation("SSO Login host={0}, appkey={1}, username={2}",
            _host,
            _appkey,
            _username);
        SessionDetails sessionDetails;
        string rawResponse = null;
        try {
            var asmPath = typeof(AppKeyAndSessionProvider).Assembly.Location;
            Console.WriteLine(
                "[Betfair ESA] SSO login implementation=" + SsoLoginImplementationId +
                "; assembly=" + asmPath);

            // Betfair docs: POST application/x-www-form-urlencoded body (not query string); required for special chars in passwords.
            // https://betfair-developer-docs.atlassian.net/wiki/spaces/1smk3cen4v3lu3yomq5qye0ni/pages/2687772/Interactive+Login+-+API+Endpoint
            var uri = $"https://{_host}/api/login";
            Console.WriteLine("[Betfair ESA] POST " + uri + " (Content-Type: application/x-www-form-urlencoded body)");

            using var httpClient = new HttpClient { Timeout = Timeout };
            using var loginRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            loginRequest.Headers.Add("X-Application", _appkey);
            loginRequest.Headers.Accept.ParseAdd("application/json");
            loginRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = _username,
                ["password"] = _password
            });

            using var loginResponse = httpClient.Send(loginRequest);
            loginResponse.EnsureSuccessStatusCode();
            rawResponse = loginResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Trace.TraceInformation("{0}: Response: {1}", _host, rawResponse);
            sessionDetails = JsonConvert.DeserializeObject<SessionDetails>(rawResponse);
        }
        catch (Exception e) {
            throw new IOException("SSO Authentication - call failed:", e);
        }

        var status = sessionDetails?.Status ?? string.Empty;
        var token = sessionDetails?.Token;
        var loginOk = sessionDetails is not null &&
                      !string.IsNullOrEmpty(token) &&
                      ("SUCCESS".Equals(status, StringComparison.OrdinalIgnoreCase) ||
                       "LIMITED_ACCESS".Equals(status, StringComparison.OrdinalIgnoreCase));

        if (loginOk) {
            _session = new AppKeyAndSession(_appkey, token);
        }
        else {
            var err = sessionDetails?.Error ?? "(null sessionDetails)";
            Console.WriteLine("[Betfair ESA] SSO JSON response: " + (rawResponse ?? "(no body)"));
            throw new InvalidCredentialException(
                "SSO Authentication - response is fail: " + err + " (status=" + status + ")");
        }

        return _session;
    }

    /// <summary>
    /// Expires cached token
    /// </summary>
    public void ExpireTokenNow() {
        Trace.TraceInformation("SSO Login - expiring session token now");
        _session = null;
    }
}

internal sealed class SessionDetails
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("product")]
    public string Product { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("error")]
    public string Error { get; set; }
}