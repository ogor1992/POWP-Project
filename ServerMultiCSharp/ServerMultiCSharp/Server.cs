using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    interface ServerInterface
    {
        int initServer();
    }

    class FileServer : ServerInterface
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private Dictionary<Topic, List<Subscriber>> subscriberLists;
        public static FileServer serverInstance;

        private FileServer()
        {
            this.subscriberLists = new Dictionary<Topic, List<Subscriber>>();
            this.tcpListener = new TcpListener(IPAddress.Any, 7);
        }

        public int initServer()
        {
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            this.listenThread.Join();
            return 0;
        }

        public static FileServer getInstance()
        {
            if(serverInstance == null)
                serverInstance = new FileServer();

            return serverInstance;
        }

        public void registerSubscriber(Subscriber s, Topic t)
        {
            if(!subscriberLists.ContainsKey(t))
            {
                this.subscriberLists.Add(t, new List<Subscriber>());
            }
            this.subscriberLists[t].Add(s);
        }

        public void unregisterSubscriber(Subscriber s)
        {
            foreach(var item in subscriberLists)
            {
                item.Value.Remove(s);
            }
        }

        private void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Server could not start. Error of binding");
                return;
            }

            Console.WriteLine("Server run on port 7");
            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Console.WriteLine("New client connected from: " + client.Client.RemoteEndPoint);
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommunication));
                clientThread.Start(client);
            }
        }

        private void HandleClientCommunication(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            String clientIP = "" + tcpClient.Client.RemoteEndPoint.AddressFamily;
            NetworkStream clientStream = tcpClient.GetStream();
            Topic receivedTopic = new Topic();
            Subscriber subscriber = new Subscriber(tcpClient.Client.RemoteEndPoint);

            //Subscribe topic
            receivedTopic.receiveTopic(clientStream);
            subscriber.registerSubscriber(receivedTopic);

            while (tcpClient.Connected)
            {
                try
                {
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] bByte = new byte[1024];
                    tcpClient.Client.Receive(bByte);
                    Console.WriteLine("Received from " + tcpClient.Client.RemoteEndPoint + "file: " + encoder.GetString(bByte, 0, 1024));
                    string filePath = "C:\\Users\\eigdude\\Desktop\\test_received.txt";
                    File.WriteAllBytes(filePath, bByte);

                }
                catch { break; }

                Console.WriteLine("Topic from " + tcpClient.Client.RemoteEndPoint + ": " + receivedTopic.TopicString);
            }
            tcpClient.Close();
            subscriber.unregisterSubscriber();
        }

    }
}
