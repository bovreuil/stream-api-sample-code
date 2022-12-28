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
            return new AppKeyAndSession(_appkey, "abc");
        }

        public void ExpireTokenNow()
        {
        }
    }
}