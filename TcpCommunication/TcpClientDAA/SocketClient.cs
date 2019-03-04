using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using DatabaseModule;
using DatabaseModule.MongoDB;

namespace TcpCommunication.TcpClientDAA
{
    public class StateObject
    {
        public Socket WorkSocket;
        public const int BufferSize = 360;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Sb = new StringBuilder();
    }

    public class SocketClient
    {
        private int Port { get; }
        private string Ip { get; }
        private MongoSaver Saver { get; set; }
        private readonly ManualResetEvent _connectDone =
            new ManualResetEvent(false);
        private readonly ManualResetEvent _sendDone =
            new ManualResetEvent(false);
        private readonly ManualResetEvent _receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private string _response = string.Empty;

        public SocketClient(int port, string iP, string location, string database, string document)
        {
            Port = port;
            Ip = iP;
            Saver = MongoDbCall.GetSaverToMongoDb(location, database, document);
        }

        public void StartClient()
        {
            try
            {
                var ipHostInfo = Dns.GetHostAddresses(Ip);
                var ipAddress = ipHostInfo[0];
                var remoteEP = new IPEndPoint(ipAddress, Port);

                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                _connectDone.WaitOne();

                Send(client, @"<Start>Ok</Start>");
                _sendDone.WaitOne();
                while (client.Connected)
                {
                    Receive(client);
                    _receiveDone.WaitOne();
                    //Saver.SavePacket(_response);
                    Console.WriteLine("Response received : {0}", _response);
                }

                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                _connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                var state = new StateObject { WorkSocket = client };

                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var client = state.WorkSocket;

                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.Sb.Length > 1)
                    {
                        _response = state.Sb.ToString();
                    }
                    _receiveDone.Set();
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            var byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                _sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
