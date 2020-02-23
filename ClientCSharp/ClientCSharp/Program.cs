using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                bool power = true;
                byte[] buffor = new byte[1024];

                Socket socketClient = ConnectToServer();
                Console.WriteLine("Client - exit, aby wyjsc");
                Console.WriteLine("Client - Podaj temat subskrybcji:");
                string topic = Console.ReadLine();
                socketClient.Send(Encoding.UTF8.GetBytes(topic));

                while (power)
                {
                    Console.WriteLine("Podłączony do servera plików");
                    Console.WriteLine("Aby wysłac plik wpisz 1, aby pobrać 2, wyswietlic liste plików 3:");
                    string operation = Console.ReadLine();

                    //string fileName = "C:\\Users\\eigdude\\Desktop\\test.txt";
                    switch (operation)
                    {
                        case "1":
                            //Send request
                            socketClient.Send(Encoding.UTF8.GetBytes("sendFile@"));
                            SendFile(socketClient);
                            break;
                        case "2":
                            //Send request
                            socketClient.Send(Encoding.UTF8.GetBytes("downloadFile@"));
                            DownloadFile(socketClient);
                            break;
                        case "3":
                            break;
                        default:
                            break;
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
            Console.WriteLine("Client - Podaj numer portu servera:");
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
            int size = 1024;

            //Receive ListOfFiles
            int byteRead = socket.Receive(messageByte, 0, size, 0);
            string files = encoder.GetString(messageByte, 0, size);
            string[] listOfFiles = files.Split('@');

            Console.WriteLine("Lista plików do pobrania: ");
            foreach(string file in listOfFiles)
                Console.WriteLine(file);

            //Send FileName
            Console.WriteLine("Podaj pełną sciezke do pliku, który chcesz wysłać:");
            string fileName = Console.ReadLine();
            socket.Send(Encoding.UTF8.GetBytes(fileName + "@"));

            //Receive and save file
            while (fileName.IndexOf("\\") > -1)
                fileName = fileName.Substring(fileName.IndexOf("\\") + 1);

            socket.Receive(bByte);
            string filePath = "C:\\Users\\eigdude\\Desktop\\Received_client\\";
            File.WriteAllBytes(filePath + fileName, bByte);
            Console.WriteLine("File download completed");
        }
    }
}