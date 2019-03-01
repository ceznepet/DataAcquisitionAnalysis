using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        private MongoSaver Saver { get; set; }

        public void ConnectAndReceive()
        {
            try
            {

                var client = new System.Net.Sockets.TcpClient(Server, Port);
                const string message = @"<Start>Ok</Start>";
                var data = Encoding.ASCII.GetBytes(message);


                var stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent connection request to Server.");

                while (client.Connected)
                {
                    data = new byte[512];

                    var bytes = stream.Read(data, 0, data.Length);
                    var reader = new StreamReader(stream);

                    

                    var responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    Saver.SavePacket(responseData);
                    stream.FlushAsync();
                }
                
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}