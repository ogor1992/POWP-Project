using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    class Publisher
    {
        Topic topic;

        public Publisher(Topic t)
        {
            this.topic = t;
        }
        //public void publish(Message m)
        //{
        //    sendMessage(this.topic, m);
        //}
    }
}
