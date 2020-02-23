using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    class Proxy : IServerInterface
    {
        private IServerInterface fileServer;

        private void getInstanceOfServer()
        {
            fileServer = FileServer.GetInstance();
        }

        public int InitServer()
        {
            getInstanceOfServer();
            return fileServer.InitServer();
        }
    }
}
