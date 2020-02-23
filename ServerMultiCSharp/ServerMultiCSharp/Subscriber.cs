using System.Net;
using System.Net.Sockets;

namespace ServerMultiCSharp
{
    internal class Subscriber
    {
        private readonly TcpClient tcp;
        public TcpClient Tcp { get => tcp;}

        public Subscriber(TcpClient client)
        {
            this.tcp = client;
        }

        public void RegisterSubscriber(Topic t)
        {
            Publisher.GetInstance().RegisterSubscriber(this, t);
        }

        public void UnregisterSubscriber()
        {
            Publisher.GetInstance().UnregisterSubscriber(this);
        }
    }
}