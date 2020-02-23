using System.Net;

namespace ServerMultiCSharp
{
    internal class Subscriber
    {
        private readonly EndPoint endPoint;

        public Subscriber(EndPoint eP)
        {
            this.endPoint = eP;
        }
        public void RegisterSubscriber(Topic t)
        {
            FileServer.GetInstance().RegisterSubscriber(this, t);
        }

        public void UnregisterSubscriber()
        {
            FileServer.GetInstance().UnregisterSubscriber(this);
        }
    }
}