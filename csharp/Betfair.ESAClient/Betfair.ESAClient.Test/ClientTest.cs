using System;
using System.IO;
using System.Threading;
using Betfair.ESAClient.Protocol;
using Betfair.ESAClient.Test.Network;
using NUnit.Framework;

namespace Betfair.ESAClient.Test {
    [TestFixture]
    public class ClientTest : BaseTest {
        [Test]
        public void TestInvalidHost() {
            var invalidClient = new Client("www.betfair.com", 443, ValidSessionProvider);
            invalidClient.Timeout = TimeSpan.FromMilliseconds(100);
            Assert.Catch<IOException>(() =>
                invalidClient.Start()
            );
        }

        [Test]
        public void TestStartStop() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status);
            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Stop();
            Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status);
            
            tcpConnectionServer.Stop();
            Thread.Sleep(5000);
        }

        [Test]
        public void TestStartHearbeatStop() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Client.Start();
            Client.Heartbeat();
            Client.Stop();

            tcpConnectionServer.Stop();
            Thread.Sleep(5000);
        }

        [Test]
        public void TestReentrantStartStop() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Heartbeat();
            Client.Stop();
            Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status);

            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Heartbeat();
            Client.Stop();
            Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status);
            
            tcpConnectionServer.Stop();
            Thread.Sleep(5000);
        }

        [Test]
        public void TestDoubleStartStop() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Client.Start();
            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Heartbeat();
            Client.Stop();
            Client.Stop();
            Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status);
            
            tcpConnectionServer.Stop();
            Thread.Sleep(5000);
        }

        [Test]
        public void TestDisconnectWithNoAutoReconnect() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Client.AutoReconnect = false;
            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Heartbeat();

            //socket disconnect
            Client.Disconnect();
            Thread.Sleep(500);

            //wait for status to go disconnected
            Retry.Action(() => Assert.AreEqual(ConnectionStatus.STOPPED, Client.Status));
            
            Client.Stop();
            tcpConnectionServer.Stop();
            Thread.Sleep(5000);            
        }

        [Test]
        public void TestDisconnectWithAutoReconnect() {
            var tcpConnectionServer = new TcpConnectionServer("127.0.0.1", 1234);
            tcpConnectionServer.Start();
            var Client = new Client("127.0.0.1", 1234, DummySessionProvider);

            Client.ReconnectBackOff = TimeSpan.FromMilliseconds(100);
            Client.Start();
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Client.Heartbeat();

            //socket disconnect
            Assert.AreEqual(0, Client.DisconnectCounter);
            Client.Disconnect();
            Assert.AreEqual(0, Client.DisconnectCounter);

            //retry until connected
            Retry.Action(() => Client.Heartbeat());
            Thread.Sleep(500);
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Client.Status);
            Assert.AreEqual(1, Client.DisconnectCounter);
            
            Client.Stop();
            tcpConnectionServer.Stop();
            Thread.Sleep(5000);
        }
    }
}
