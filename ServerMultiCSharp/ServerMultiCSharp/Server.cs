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
    interface IServerInterface
    {
        int InitServer();
    }

    class FileServer : IServerInterface
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private TcpListener tcpListener;
#pragma warning restore IDE0044 // Add readonly modifier
        private Thread listenThread;
        private static FileServer serverInstance;
        private readonly string filePath = "C:\\Users\\eigdude\\Desktop\\Received\\";

        private FileServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 7);
        }

        public int InitServer()
        {
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            this.listenThread.Join();
            return 0;
        }

        public static FileServer GetInstance()
        {
            if(serverInstance == null)
                serverInstance = new FileServer();

            return serverInstance;
        }

        private void ListenForClients()
        {
            try
            {
                this.tcpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Server could not start. Error of binding: " + e);
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
            Subscriber subscriber = new Subscriber(tcpClient);
            ASCIIEncoding encoder = new ASCIIEncoding();
            Publisher publisher = Publisher.GetInstance();
            var topicCreator = new TopicCreator();
            Topic topic;

            //Subscribe topic
            string requestTopic = GetMessage(tcpClient, encoder);
            switch (requestTopic)
            {
                case "1":
                    topic = topicCreator.CreateTopic(1);
                    break;
                case "2":
                    topic = topicCreator.CreateTopic(2);
                    break;
                default:
                    topic = topicCreator.CreateTopic(1);
                    break;
            }
            subscriber.RegisterSubscriber(topic);

            while (tcpClient.Connected)
            {
                try
                {
                    string request = GetMessage(tcpClient, encoder);
                    switch(request)
                    {
                        case "downloadFile":
                            SendFile(tcpClient, encoder);
                            topic = topicCreator.CreateTopic(1);
                            publisher.AddToPublisher("file dowloaded from serwer", topic);
                            break;
                        case "sendFile":
                            ReceiveFile(tcpClient, encoder);
                            topic = topicCreator.CreateTopic(2);
                            publisher.AddToPublisher("file uploaded to serwer", topic);
                            break;
                        default:
                            break;
                    }
                }
                catch { break; }
            }
            tcpClient.Close();
            subscriber.UnregisterSubscriber();
        }

        private void ReceiveFile(TcpClient tcp, ASCIIEncoding encoder)
        {
            byte[] bByte = new byte[1024];
            //Receive fileName
            string fileName = GetMessage(tcp, encoder);
            //Receive and save file
            tcp.Client.Receive(bByte);
            File.WriteAllBytes(filePath + fileName, bByte);
        }

        private void SendListOfFiles(TcpClient tcp)
        {
            List<string> files = new List<string>(Directory.EnumerateFiles(filePath));
            string fileToSend = "";
            foreach(string file in files)
                fileToSend += file + "@";

            tcp.Client.Send(Encoding.UTF8.GetBytes(fileToSend));
        }

        private void SendFile(TcpClient tcp, ASCIIEncoding encoder)
        {
            //Send ListOfFiles
            SendListOfFiles(tcp);
            //Receieve FileName
            string fileName = GetMessage(tcp, encoder);
            //Send file to client
            tcp.Client.SendFile(fileName);
        }

        private string GetMessage(TcpClient tcp, ASCIIEncoding encoder)
        {
            byte[] requestMessage = new byte[1024];

            int byteRead = tcp.Client.Receive(requestMessage);
            string requestLong = encoder.GetString(requestMessage);
            int found = requestLong.IndexOf("@", StringComparison.Ordinal);
            return requestLong.Substring(0, found);
        }
    }
}
