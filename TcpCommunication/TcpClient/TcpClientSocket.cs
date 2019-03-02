using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DatabaseModule.MongoDB;

namespace TcpCommunication.TcpClient
{
    public class TcpClientSocket
    {
        public TcpClientSocket(string server, int port, string database, string document)
        {
            Server = server;
            Port = port;
            Saver = MongoDbCall.GetSaverToMongoDb(database, document);
        }

        private string Server { get; }
        private int Port { get; }
        private MongoSaver Saver { get; }

        public void ConnectAndReceive()
        {
            var bytes = new byte[1024];

            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                var ipHostInfo = Dns.GetHostAddresses(Server);
                var ipAddress = ipHostInfo[0];
                var remoteEP = new IPEndPoint(ipAddress, Port);

                // Create a TCP/IP  socket.  
                var sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint);

                    // Encode the data string into a byte array.  
                    var msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.  
                    var bytesSent = sender.Send(msg);
                    while (sender.Connected)
                    {
                        var bytesRec = sender.Receive(bytes);
                        var mes = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Saver.SavePacket(mes);
                        Console.WriteLine("Echoed test = {0}", mes);
                    }
                    // Receive the response from the remote device.  


                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}