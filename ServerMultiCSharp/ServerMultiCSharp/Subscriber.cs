using System.Net;

namespace ServerMultiCSharp
{
    internal class Subscriber
    {
        private EndPoint endPoint;
        private Topic topic;

        public Subscriber(EndPoint eP)
        {
            this.endPoint = eP;
        }
        public void registerSubscriber(Topic t)
        {
            FileServer.getInstance().registerSubscriber(this, t);
        }

        public void unregisterSubscriber()
        {
            FileServer.getInstance().unregisterSubscriber(this);
        }
    }
}