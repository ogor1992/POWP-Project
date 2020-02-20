﻿using System;
using System.Collections.Generic;
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
                Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Client - Podaj numer portu servera:");
                string port = Console.ReadLine();
                socketClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), System.Convert.ToInt32(port)));
                bool power = true;
                string message = null;
                byte[] buffor = new byte[1024];
                int result = 0;
                String time = null;
                Console.WriteLine("Client - exit, aby wyjsc");
                Console.WriteLine("Client - Podaj topic:");
                message = Console.ReadLine();
                string sendingMessage = "topic;" + message;
                socketClient.Send(Encoding.UTF8.GetBytes(sendingMessage));

                string fileName = "C:\\Users\\eigdude\\Desktop\\test.txt";
                Console.WriteLine("Sending {0} to the host.", fileName);
                socketClient.SendFile(fileName);

                while (power)
                {
                    //buffor = new byte[1024];
                    //result = socketClient.Receive(buffor);
                    //time = Encoding.ASCII.GetString(buffor, 0, result);
                    //Console.WriteLine("Client - Info z servera: ");
                    //Console.WriteLine(time);


                    if (message == "exit")
                    {
                        power = false;

                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Exception from program", ex.ErrorCode);
            }
            Console.ReadKey();
        }
    }
}