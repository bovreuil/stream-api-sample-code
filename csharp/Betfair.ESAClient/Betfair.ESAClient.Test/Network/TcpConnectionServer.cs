using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Betfair.ESASwagger.Model;
using Newtonsoft.Json;

namespace Betfair.ESAClient.Test.Network
{
    public class TcpConnectionServer
    {
        private Thread _server;
        private StreamReader _reader;
        private StreamWriter _writer;
        private int _savedMarketSubscriptionId;
        private int _savedOrderSubscriptionId;
        private readonly string _host;
        private readonly int _port;
        private bool _stopping;
        private bool _stopped;

        public TcpConnectionServer(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Start()
        {
            _server = new Thread(StartTcpServer);
            _server.Start();
        }

        public void Stop()
        {
            _stopping = true;
        }

        public void SendMarketChange(MarketChangeMessage marketChangeMessage)
        {
            // make sure we are responding with the right subscription id
            marketChangeMessage.Id = _savedMarketSubscriptionId;
            var outMessage = JsonConvert.SerializeObject(marketChangeMessage, Formatting.None);

            try
            {
                Console.WriteLine("ESA server sending mcm: {0}", outMessage);

                _writer.WriteLine(outMessage);
                _writer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void StartTcpServer()
        {
            IPAddress localAddr = IPAddress.Parse(_host);
            var listener = new TcpListener(localAddr, _port);
            listener.Start();
            while (!_stopping)
            {
                if (!listener.Pending()) {
                    Thread.Sleep(100);
                    continue;
                }
                var client = listener.AcceptTcpClient();
                new Thread(() => ManageTcpConnection(client)).Start();
            }
            listener.Server.Close();
            listener.Stop();
        }

        private void ManageTcpConnection(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1024 * 1024];
                    _reader = new StreamReader(stream, Encoding.UTF8, false, buffer.Length);
                    _writer = new StreamWriter(stream, Encoding.UTF8);
                    var outMessage = "{\"op\":\"connection\",\"connectionId\":\"abc\"}";
                    Console.WriteLine("ESA server sending: {0}", outMessage);
                    _writer.WriteLine(outMessage);
                    _writer.Flush();

                    while (!_stopping)
                    {
                        var inMessage = _reader.ReadLine();
                        if (inMessage != null)
                        {
                            ProcessMessage(inMessage);
                        }
                    }

                    if (_stopping)
                    {
                        _stopped = true;
                        _stopping = false;
                    }
                }
                client.Close();
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception);
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception);
            }
            catch (ThreadAbortException exception)
            {
                Console.WriteLine("ESA server aborting");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        private void ProcessMessage(string message)
        {
            try
            {
                Console.WriteLine("ESA server received message: {0}", message);
                var requestMessage = JsonConvert.DeserializeObject<RequestMessage>(message);
                var id = requestMessage.Id ?? 0;
                if (requestMessage.Op == "marketSubscription") _savedMarketSubscriptionId = id;
                if (requestMessage.Op == "orderSubscription") _savedMarketSubscriptionId = id;
                var response = new StatusMessage
                {
                    Op = "status",
                    Id = id,
                    StatusCode = StatusMessage.StatusCodeEnum.Success
                };
                var responseMessage = JsonConvert.SerializeObject(response, Formatting.None);
                Console.WriteLine("ESA server sending response: {0}", responseMessage);
                _writer.WriteLine(responseMessage);
                _writer.Flush();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception processing received TCP message");
                Console.WriteLine(exception);
            }
        }
    }
}