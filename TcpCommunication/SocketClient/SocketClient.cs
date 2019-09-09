using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseModule.MongoDB;
using NLog;

namespace SocketModule.SocketClient
{
    public class SocketClient
    {
        public SocketClient(string server, int port, int protocol, string location, string database, string document)
        {
            Server = server;
            Port = port;
            TypeOfProtocol = protocol;
            Protocol = protocol == 0 ? ProtocolType.Tcp : ProtocolType.Udp;
            TypeOfSocket = protocol == 0 ? SocketType.Stream : SocketType.Dgram;
            Saver = MongoDbCall.GetSaverToMongoDb(location, database, document);
        }

        private string Server { get; }
        private int Port { get; }

        private ProtocolType Protocol { get; }

        private int TypeOfProtocol { get; }

        private SocketType TypeOfSocket { get; }
        private MongoSaver Saver { get; }
        private static readonly Logger _logger = LogManager.GetLogger("Socket Client");
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
                    
                    var sender = new Socket(ipAddress.AddressFamily, TypeOfSocket, Protocol);

                    try
                    {
                        sender.Connect(remoteEP);
                        var msg = Encoding.UTF8.GetBytes("This is a test");
                        int i = sender.Send(msg);
                        _logger.Info("Sent {0} bytes.", i);
                        _logger.Info("Socket connected to {0}",
                            sender.RemoteEndPoint);

                        while (sender.Connected)
                        {
                            ReadStream(sender, bytes);
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


       private void ReadStream(Socket sender, byte[] buffer)
        {
            int bytesRec = 0;

            bytesRec = sender.Receive(buffer);

            if (bytesRec > 0)
            {
                var mes = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                var packet = Regex.Matches(mes, Pattern, RegexOptions.IgnoreCase).FirstOrDefault()
                    ?.Value;
                Saver.SavePacket(packet);
                _logger.Debug("Echoed test = {0}", packet);
                buffer = new byte[1024];
            }
        }
    }
}