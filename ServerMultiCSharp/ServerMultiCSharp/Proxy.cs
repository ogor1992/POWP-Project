using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMultiCSharp
{
    class Proxy : ServerInterface
    {
        private ServerInterface fileServer;

        private void getInstanceOfServer()
        {
            fileServer = FileServer.getInstance();
        }

        public int initServer()
        {
            getInstanceOfServer();
            return fileServer.initServer();
        }
    }
}
