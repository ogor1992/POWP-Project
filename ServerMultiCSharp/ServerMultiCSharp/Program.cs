using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Proxy proxy = new Proxy();
            proxy.Server();
        }
    }

    interface ServerInterface
    {
        int Server();
    }

    class FileServer : ServerInterface
    {

        private TcpListener tcpListener;
        private Thread listenThread;

        public int Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 7);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            this.listenThread.Join();
            return 0;
        }

        private void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();
                //run listening
            }
            catch (Exception e)
            {
                Console.WriteLine("Server could not start. Error of binding");
                return;
            }
            Console.WriteLine("Server run on port 7");
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Console.WriteLine("New client connected from: " + client.Client.RemoteEndPoint);
                //create a thread to handle communication with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            String clientIP = "" + tcpClient.Client.RemoteEndPoint.AddressFamily;
            NetworkStream clientStream = tcpClient.GetStream();

            int size = 1024;
            byte[] message = new byte[1024];
            int byteRead;
            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                byteRead = 0;
                try
                {
                    byteRead = clientStream.Read(message, 0, size);
                    clientStream.Write(message, 0, byteRead);
                }
                catch { break; }

                if (byteRead == 0) break;

                Console.WriteLine("Message from " + tcpClient.Client.RemoteEndPoint + ": " + encoder.GetString(message, 0, byteRead));
            }
            tcpClient.Close();
        }

    }

    class Proxy : ServerInterface
    {
        private ServerInterface fileServer;

        private void CreateInstanceOfServer()
        {
            fileServer = new FileServer();
        }

        public int Server()
        {
            CreateInstanceOfServer();
            return fileServer.Server();
        }
    }
}
