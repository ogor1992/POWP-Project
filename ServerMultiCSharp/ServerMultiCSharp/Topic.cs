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

        public void receiveTopic(NetworkStream clientStream)
        {
            byteRead = clientStream.Read(messageByte, 0, size);
            TopicString = encoder.GetString(messageByte, 0, size);
        }
    }
}