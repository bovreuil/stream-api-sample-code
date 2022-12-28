using System;

namespace Betfair.ESAClient.Auth
{
    public class DummyAppKeyAndSessionProvider : IAppKeyAndSessionProvider
    {
        private string _appkey;
        
        public DummyAppKeyAndSessionProvider(string ssoHost, string appkey, string username, string password) {
            _appkey = appkey;
        }
        
        public string Appkey { get; }
        public TimeSpan SessionExpireTime { get; set; }
        public TimeSpan Timeout { get; set; }        
        
        public AppKeyAndSession GetOrCreateNewSession()
        {
            return new AppKeyAndSession(_appkey, RandomString());
        }

        private static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);            
        }


        public void ExpireTokenNow()
        {
        }
    }
}