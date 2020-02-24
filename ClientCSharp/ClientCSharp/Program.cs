using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//string fileName = "C:\\Users\\eigdude\\Desktop\\test.txt";

namespace ClientCSharp
{
    class Program
    {
        private static readonly string filePath = "C:\\Users\\eigdude\\Desktop\\Received_client\\";
        static void Main(string[] args)
        {
            try
            {
                bool power = true;
                byte[] buffor = new byte[1024];

                Socket socketClient = ConnectToServer();
                string topic = "";
                while(!((topic.Equals("1")) || (topic.Equals("2"))))
                {
                    Console.WriteLine("Podaj temat subskrybcji (1 - subskrybcja pobierania, 2 - subskrybcja wysyłania:");
                    topic = Console.ReadLine();
                    Console.WriteLine("Wybor:" + topic + "::");
                }
                socketClient.Send(Encoding.UTF8.GetBytes(topic + "@"));

                while (power)
                {
                    string operation = "";
                    while (!((operation.Equals("1")) || (operation.Equals("2"))))
                    {
                        Console.WriteLine("Aby wysłac plik wpisz 1, aby pobrać 2");
                        operation = Console.ReadLine();
                    }

                    switch (operation)
                    {
                        case "1":
                            socketClient.Send(Encoding.UTF8.GetBytes("sendFile@"));
                            SendFile(socketClient);
                            break;
                        case "2":
                            socketClient.Send(Encoding.UTF8.GetBytes("downloadFile@"));
                            DownloadFile(socketClient);
                            break;
                        default:
                            break;
                    }

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] messageByte = new byte[1024];

                    socketClient.Receive(messageByte);
                    string infoLong = encoder.GetString(messageByte);
                    int found = infoLong.IndexOf("@", StringComparison.Ordinal);
                    string info = infoLong.Substring(0, found);
                    if(!info.Contains("#non#"))
                    {
                        Console.WriteLine("Information from server:: " + info);
                    }

                    if (topic == "exit")
                        power = false;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Exception from program", ex.ErrorCode);
            }
            Console.ReadKey();
        }

        private static Socket ConnectToServer()
        {
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Podaj numer portu servera:");
            string port = Console.ReadLine();
            socketClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), System.Convert.ToInt32(port)));
            return socketClient;
        }

        private static void SendFile(Socket socket)
        {
            Console.WriteLine("Podaj pełną sciezke do pliku, który chcesz wysłać:");
            string fileName = Console.ReadLine();
            string filePath = "";
            while (fileName.IndexOf("\\") > -1)
            {
                filePath += fileName.Substring(0, fileName.IndexOf("\\") + 1);
                fileName = fileName.Substring(fileName.IndexOf("\\") + 1);
            }
            
            //Send FileName
            socket.Send(Encoding.UTF8.GetBytes(fileName+"@"));

            //Send File
            socket.SendFile(filePath+fileName);
            Console.WriteLine("File send completed");
        }


        private static void DownloadFile(Socket socket)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] bByte = new byte[1024];
            byte[] messageByte = new byte[1024];

            //Receive ListOfFiles
            socket.Receive(messageByte);
            string files = encoder.GetString(messageByte);
            string[] listOfFiles = files.Split('@');

            Console.WriteLine("Lista plików do pobrania: ");
            foreach(string file in listOfFiles)
                Console.WriteLine(file);

            //Send FileName
            Console.WriteLine("Podaj pełną sciezke do pliku, który chcesz pobrać:");
            string fileName = Console.ReadLine();
            socket.Send(Encoding.UTF8.GetBytes(fileName + "@"));

            //Receive and save file
            while (fileName.IndexOf("\\") > -1)
                fileName = fileName.Substring(fileName.IndexOf("\\") + 1);

            socket.Receive(bByte);
            File.WriteAllBytes(filePath + fileName, bByte);
            Console.WriteLine("File download completed");
        }
    }
}