using System;
using System.IO;
using System.Security.Authentication;

namespace Betfair.ESAClient.Auth
{
    public interface IAppKeyAndSessionProvider
    {
        /// <summary>
        /// AppKey being used
        /// </summary>
        string Appkey { get; }

        /// <summary>
        /// Session expire time (default 3hrs)
        /// </summary>
        TimeSpan SessionExpireTime { get; set; }

        /// <summary>
        /// Specifies the timeout
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Constructs a new session token via identity SSO.
        /// Note: These are not cached.
        /// </summary>
        /// <exception cref="InvalidCredentialException">Thrown if authentication response is fail</exception>
        /// <exception cref="IOException">Thrown if authentication call fails</exception>
        /// <returns></returns>
        AppKeyAndSession GetOrCreateNewSession();

        /// <summary>
        /// Expires cached token
        /// </summary>
        void ExpireTokenNow();
    }
}