using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseModule.MongoDB;
using NLog;

namespace TcpCommunication.TcpClientDAA
{
    public class TcpClientSocket
    {
        public TcpClientSocket(string server, int port, string location, string database, string document)
        {
            Server = server;
            Port = port;
            Saver = MongoDbCall.GetSaverToMongoDb(location, database, document);
        }

        private string Server { get; }
        private int Port { get; }
        private MongoSaver Saver { get; }
        private static readonly Logger _logger = LogManager.GetLogger("Tcp Socket Client");
        private readonly string Pattern = @"<Robot>.*<\/Robot>";

        public void ConnectAndReceive()
        {
            var bytes = new byte[1024];
            while (true)
            {
                try
                {
                    var ipHostInfo = Dns.GetHostAddresses(Server);
                    var ipAddress = ipHostInfo[0];
                    var remoteEP = new IPEndPoint(ipAddress, Port);

                    var sender = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    try
                    {
                        sender.Connect(remoteEP);
                        _logger.Info("Socket connected to {0}",
                            sender.RemoteEndPoint);

                        var msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                        var bytesSent = sender.Send(msg);
                        while (sender.Connected)
                        {
                            var bytesRec = sender.Receive(bytes);
                            if (bytesRec > 0)
                            {
                                var mes = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                var packet = Regex.Matches(mes, Pattern, RegexOptions.IgnoreCase).FirstOrDefault()
                                    ?.Value;
                                Saver.SavePacket(packet);
                                _logger.Debug("Echoed test = {0}", packet);
                                bytes = new byte[1024];
                            }
                        }


                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }
                    catch (ArgumentNullException ane)
                    {
                        _logger.Error("ArgumentNullException : {0}", ane);
                    }
                    catch (SocketException se)
                    {
                        _logger.Error("SocketException : {0}", se);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Unexpected exception : {0}", e.ToString());
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.ToString());
                }
            }
        }
    }
}