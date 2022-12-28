using System;
using Betfair.ESAClient.Protocol;
using Betfair.ESASwagger.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Betfair.ESAClient.Test.Impl {
    [TestFixture]
    public class RequestResponseProcessorTest {
        private RequestResponseProcessor Processor { get; set; }
        public string LastLine { get; private set; }

        [SetUp]
        public void Init() {
            Processor = new RequestResponseProcessor(line => LastLine = line);
        }

        [Test]
        public void TestJsonNoOp() {
            Processor.ReceiveLine("{}");
        }

        [Test]
        public void TestJsonUnknownOp() {
            Processor.ReceiveLine("{\"op\": \"rubbish\"}");
        }

        [Test]
        public void TestOpNotFirst() {
            var msg = (ConnectionMessage) Processor.ReceiveLine("{\"connectionId\":\"aconnid\", \"op\":\"connection\"}");
            Assert.AreEqual("aconnid", msg.ConnectionId);
        }

        [Test]
        public void TestExtraJsonField() {
            var msg = (ConnectionMessage) Processor.ReceiveLine("{\"op\":\"connection\", \"connectionId\":\"aconnid\", \"extraField\":\"extraValue\"}");
            Assert.AreEqual("aconnid", msg.ConnectionId);
        }

        [Test]
        public void TestJsonMissingField() {
            var msg = (ConnectionMessage) Processor.ReceiveLine("{\"op\":\"connection\"}");
            Assert.IsNotNull(msg);
        }

        [Test]
        public void TestInvalidJson() {
            ;
            Assert.Catch<JsonException>(() =>
                Processor.ReceiveLine("rubbish")
            );
        }

        [Test]
        public void TestConnectionMessageUnwind() {
            //wait and get timeout
            Assert.IsFalse(Processor.ConnectionMessage()
                .Wait(10));

            //process
            var msg = (ConnectionMessage) Processor.ReceiveLine("{\"op\":\"connection\", \"connectionId\":\"aconnid\"}");

            //now unwound
            Assert.IsTrue(Processor.ConnectionMessage()
                .Wait(10));
            Assert.AreEqual("aconnid",
                Processor.ConnectionMessage()
                    .Result.ConnectionId);
            Assert.AreEqual(ConnectionStatus.CONNECTED, Processor.Status);
        }

        [Test]
        public void TestAuthentication() {
            var authTask = Processor.Authenticate(new AuthenticationMessage {Session = "asession", AppKey = "aappkey"});
            Console.WriteLine(LastLine);

            //wait and get timeout
            Assert.IsFalse(authTask.Wait(10));

            Processor.ReceiveLine("{\"op\":\"status\",\"id\":1,\"statusCode\":\"SUCCESS\"}");


            //wait and pass
            Assert.IsTrue(authTask.Wait(10));
            Assert.AreEqual(StatusMessage.StatusCodeEnum.Success, authTask.Result.StatusCode);
            Assert.AreEqual(ConnectionStatus.AUTHENTICATED, Processor.Status);
        }

        [Test]
        public void TestAuthenticationFailed() {
            var authTask = Processor.Authenticate(new AuthenticationMessage {Session = "asession", AppKey = "aappkey"});

            //wait and get timeout
            Assert.IsFalse(authTask.Wait(10));

            Processor.ReceiveLine("{\"op\":\"status\",\"id\":1,\"statusCode\":\"FAILURE\", \"errorCode\":\"NO_SESSION\"}");

            //wait and pass
            Assert.IsTrue(authTask.Wait(10));

            Assert.AreEqual(StatusMessage.StatusCodeEnum.Failure, authTask.Result.StatusCode);
            Assert.AreEqual(StatusMessage.ErrorCodeEnum.NoSession, authTask.Result.ErrorCode);
            Assert.AreEqual(ConnectionStatus.STOPPED, Processor.Status);
        }

        [Test]
        public void TestHeartbeat() {
            var authTask = Processor.Heartbeat(new HeartbeatMessage());
            Console.WriteLine(LastLine);

            //wait and get timeout
            Assert.IsFalse(authTask.Wait(10));

            Processor.ReceiveLine("{\"op\":\"status\",\"id\":1,\"statusCode\":\"SUCCESS\"}");


            //wait and pass
            Assert.IsTrue(authTask.Wait(10));
            Assert.AreEqual(StatusMessage.StatusCodeEnum.Success, authTask.Result.StatusCode);
        }
    }
}
