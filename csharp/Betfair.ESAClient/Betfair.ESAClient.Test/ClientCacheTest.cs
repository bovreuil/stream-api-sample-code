using System;
using System.Threading;
using Betfair.ESAClient.Auth;
using Betfair.ESAClient.Test.Network;
using NUnit.Framework;

namespace Betfair.ESAClient.Test {
    [TestFixture]
    public class ClientCacheTest : BaseTest {
        private TcpConnectionServer _tcpConnectionServer;

        [Test]
        public void TestUserStory() {
            //0: Start a local tcp server to handle SSO
            _tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            _tcpConnectionServer.Start();
            
            //1: Create a session provider
            // var sessionProvider = new AppKeyAndSessionProvider(AppKeyAndSessionProvider.SSO_HOST_COM,
            //     AppKey,
            //     UserName,
            //     Password);
            var sessionProvider = new DummyAppKeyAndSessionProvider(AppKeyAndSessionProvider.SSO_HOST_COM,
                AppKey,
                UserName,
                Password);

            //2: Create a client
            //var client = new Client("stream-api-integration.betfair.com", 443, sessionProvider);
            var client = new Client("127.0.0.1", 1234, sessionProvider);

            //3: Create a cache
            var cache = new ClientCache(client);

            //4: Setup order subscription
            //Register for change events
            cache.OrderCache.OrderMarketChanged += (sender, arg) => Console.WriteLine("Order:" + arg.Snap);
            //Subscribe to orders    
            cache.SubscribeOrders();

            //5: Setup market subscription
            //Register for change events
            cache.MarketCache.MarketChanged += (sender, arg) => Console.WriteLine("Market:" + arg.Snap);
            //Subscribe to markets (use a valid market id or filter)
            cache.SubscribeMarkets("1.125037533");

            Thread.Sleep(5000); //pause for a bit

            client.Stop();
            
            _tcpConnectionServer.Stop();
        }
    }
}
