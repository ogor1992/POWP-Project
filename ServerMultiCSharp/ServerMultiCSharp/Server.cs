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
            this.listenThread = new Thread(new ThreadStart(listenForClients));
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
                this.subscriberLists.Add(t, new List<Subscriber>());

            this.subscriberLists[t].Add(s);
        }

        public void unregisterSubscriber(Subscriber s)
        {
            foreach(var item in subscriberLists)
                item.Value.Remove(s);
        }

        private void listenForClients()
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
                Thread clientThread = new Thread(new ParameterizedThreadStart(handleClientCommunication));
                clientThread.Start(client);
            }
        }

        private void handleClientCommunication(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            String clientIP = "" + tcpClient.Client.RemoteEndPoint.AddressFamily;
            NetworkStream clientStream = tcpClient.GetStream();
            Topic receivedTopic = new Topic();
            Subscriber subscriber = new Subscriber(tcpClient.Client.RemoteEndPoint);
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] requestMessage = new byte[1024];

            //Subscribe topic
            receivedTopic.receiveTopic(clientStream);
            subscriber.registerSubscriber(receivedTopic);

            while (tcpClient.Connected)
            {
                try
                {
                    int byteRead = clientStream.Read(requestMessage, 0, 1024);
                    string requestLong = encoder.GetString(requestMessage, 0, 1024);
                    int found = requestLong.IndexOf("@", StringComparison.Ordinal);
                    string request = requestLong.Substring(0, found);

                    Console.WriteLine("request: " + request);

                    switch(request)
                    {
                        case "downloadFile":
                            sendFile(tcpClient, clientStream);
                            break;
                        case "sendFile":
                            receiveFile(tcpClient, clientStream);
                            break;
                        default:
                            break;
                    }
                }
                catch { break; }
            }
            tcpClient.Close();
            subscriber.unregisterSubscriber();
        }

        private void receiveFile(TcpClient tcpClient, NetworkStream stream)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] bByte = new byte[1024];
            byte[] messageByte = new byte[1024];

            //Receive fileName
            int byteRead = stream.Read(messageByte, 0, 1024);
            string fileNameLong = encoder.GetString(messageByte, 0, 1024);
            int found = fileNameLong.IndexOf("@", StringComparison.Ordinal);
            string fileName = fileNameLong.Substring(0, found);

            //Receive and save file
            tcpClient.Client.Receive(bByte);
            Console.WriteLine("Received from " + tcpClient.Client.RemoteEndPoint + "file: " + encoder.GetString(bByte, 0, 1024));
            string filePath = "C:\\Users\\eigdude\\Desktop\\Received\\";
            File.WriteAllBytes(filePath + fileName, bByte);
        }

        private void sendListOfFiles(TcpClient tcp)
        {
            List<string> files = new List<string>(Directory.EnumerateFiles("C:\\Users\\eigdude\\Desktop\\Received\\"));
            string fileToSend = "";
            foreach(string file in files)
            {
                fileToSend += file + "@";
                Console.WriteLine("filesToSenddd: " + fileToSend);
            }
            tcp.Client.Send(Encoding.UTF8.GetBytes(fileToSend));
        }

        private void sendFile(TcpClient tcpClient, NetworkStream stream)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] bByte = new byte[1024];
            byte[] messageByte = new byte[1024];

            //Send ListOfFiles
            sendListOfFiles(tcpClient);

            //Recieve FileName
            int byteRead = stream.Read(messageByte, 0, 1024);
            string fileNameLong = encoder.GetString(messageByte, 0, 1024);
            int found = fileNameLong.IndexOf("@", StringComparison.Ordinal);
            string fileName = fileNameLong.Substring(0, found);
            Console.WriteLine("FileName recived: " + fileName);

            //Send file to client
            tcpClient.Client.SendFile(fileName);
        }

    }
}
