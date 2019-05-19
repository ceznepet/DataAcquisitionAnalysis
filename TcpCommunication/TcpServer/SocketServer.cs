using NLog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace SocketModule.TcpServer
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder StringBuilder = new StringBuilder();
        public Socket WorkSocket;
    }

    public class SocketServer
    {
        public TcpListener Listener;
        public TcpClient Client = new TcpClient();
        private bool Started = false;
        private static readonly Logger _logger = LogManager.GetLogger("Socket Server");

        private ManualResetEvent tcpClientConnected =
            new ManualResetEvent(false);

        public int SendData(string message)
        {
            if (Client.Connected)
            {
                Started = false;
                Send(message);
                return 0;
            }
            else if (!Client.Connected && !Started)
            {
                _logger.Warn("Client {0} is no longer connected.", Client.Client.LocalEndPoint);
                DoBeginAcceptTcpClient(Listener);
                Started = true;
                return -1;
            }
            return -1;
        }

        private void DoBeginAcceptTcpClient(TcpListener listener)
        {
            tcpClientConnected.Reset();

            _logger.Info("Waiting for a connection...");

            listener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                listener);

            tcpClientConnected.WaitOne();
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            Client = listener.EndAcceptTcpClient(ar);

            _logger.Info("Client {0} is connected.", Client.Client.LocalEndPoint);
            tcpClientConnected.Set();

        }

        private void Send(string message)
        {
            var nwStream = Client.GetStream();

            var sendBytes = Encoding.ASCII.GetBytes(message);

            nwStream.WriteAsync(sendBytes, 0, sendBytes.Length);
        }
    }
}