using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    class Publisher
    {
        private static Publisher publisherInstance;
        private Dictionary<Topic, List<Subscriber>> subscriberLists;

        private Publisher()
        {
            this.subscriberLists = new Dictionary<Topic, List<Subscriber>>();
        }

        public static Publisher GetInstance()
        {
            if (publisherInstance == null)
                publisherInstance = new Publisher();

            return publisherInstance;
        }

        public void AddToPublisher(string message, Topic t)
        {
            foreach (Topic topic in subscriberLists.Keys)
            {
                if (topic.TopicString.Equals(t.TopicString))
                {
                    foreach(Subscriber subscriber in subscriberLists[topic])
                    {
                        subscriber.Tcp.Client.Send(Encoding.UTF8.GetBytes(message + "@"));
                    }
                }else
                {
                    foreach (Subscriber subscriber in subscriberLists[topic])
                    {
                        subscriber.Tcp.Client.Send(Encoding.UTF8.GetBytes("#non#" + "@"));
                    }
                }
            }
        }

        public void RegisterSubscriber(Subscriber s, Topic t)
        {
            if (!subscriberLists.ContainsKey(t))
                this.subscriberLists.Add(t, new List<Subscriber>());

            this.subscriberLists[t].Add(s);
        }

        public void UnregisterSubscriber(Subscriber s)
        {
            foreach (var item in subscriberLists)
                item.Value.Remove(s);
        }

    }
}
