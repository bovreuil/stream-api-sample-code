using System.Diagnostics;
using Betfair.ESAClient.Auth;
using NUnit.Framework;

namespace Betfair.ESAClient.Test {
    public abstract class BaseTest {
        static BaseTest() {
            //setup logging
            var traceListener = new ConsoleTraceListener();
            traceListener.TraceOutputOptions = TraceOptions.DateTime;
            Trace.Listeners.Add(traceListener);
        }

        public TestContext TestContext { get; set; }

        public string SsoHost => (string) TestContext.Parameters["SsoHost"];
        public string AppKey => (string) TestContext.Parameters["AppKey"];
        public string UserName => (string) TestContext.Parameters["UserName"];
        public string Password => (string) TestContext.Parameters["Password"];

        public IAppKeyAndSessionProvider DummySessionProvider => new DummyAppKeyAndSessionProvider(SsoHost,
            AppKey,
            UserName,
            Password);

        public AppKeyAndSessionProvider ValidSessionProvider => new AppKeyAndSessionProvider(SsoHost,
            AppKey,
            UserName,
            Password);

        public AppKeyAndSessionProvider InvalidHostSessionProvider => new AppKeyAndSessionProvider("www.betfair.com",
            "a",
            "b",
            "c");

        public AppKeyAndSessionProvider InvalidLoginSessionProvider => new AppKeyAndSessionProvider(AppKeyAndSessionProvider.SSO_HOST_COM,
            "appkey",
            "username",
            "password");
    }
}
