using ServerMultiCSharp;
using System;
using System.Net.Sockets;
using System.Text;

namespace ServerMultiCSharp
{
    public abstract class Topic
    {
        public string TopicName { get; set; }
    }



    public class DownloadTopic : Topic
    {
        public DownloadTopic()
        {
            TopicName = "download";
        }
    }
    public class UploadTopic : Topic
    {
        public UploadTopic()
        {
            TopicName = "upload";
        }
    }


    public class DownloadFactory
    {
        public Topic CreateTopic()
        {
            return new DownloadTopic();
        }
    }

    public class UploadFactory
    {
        public Topic CreateTopic()
        {
            return new UploadTopic();

        }
    }

    public class TopicCreator
    {
        public Topic CreateTopic(string type)
        {
            Topic topic = null;
            switch (type)
            {
                case "download":
                    topic = new DownloadTopic();
                    break;
                case "upload":
                    topic = new UploadTopic();
                    break;
            }
            return topic;
        }
    }
}

