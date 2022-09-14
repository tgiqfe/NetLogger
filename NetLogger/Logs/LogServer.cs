using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    internal class LogServer
    {
        public string Uri { get; private set; }
        public HttpClient HttpClient { get; private set; }

    }
}
