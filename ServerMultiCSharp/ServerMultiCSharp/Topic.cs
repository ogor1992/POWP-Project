using System;
using System.Net.Sockets;
using System.Text;

namespace ServerMultiCSharp
{
    public class Topic
    {
        ASCIIEncoding encoder = new ASCIIEncoding();
        private readonly int size = 1024;
        private readonly byte[] messageByte = new byte[1024];

        private int byteRead;
        private string topicString;

        public string TopicString { get => topicString; set => topicString = value; }

        public void ReceiveTopic(TcpClient tcp)
        {
            byteRead = tcp.Client.Receive(messageByte);
            string topicLong = encoder.GetString(messageByte, 0, size);
            int found = topicLong.IndexOf("@", StringComparison.Ordinal);
            TopicString = topicLong.Substring(0, found);
        }
    }
}