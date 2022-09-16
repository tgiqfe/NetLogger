using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public class LogServerSession
    {
        private LogServer _logServer = null;

        public LogServerSession(string server, int defPort, string defProtocol, int waitTime) :
            this(new string[1] { server }, defPort, defProtocol, waitTime)
        {
        }

        public LogServerSession(string[] uris, int defPort, string defProtocol, int waitTime)
        {
            Random random = new();
            string[] array = uris.OrderBy(x => random.Next()).ToArray();
            foreach (var uri in array)
            {
                var _logServer = new LogServer(uri, defPort, defProtocol);
                _logServer.TestConnect(waitTime).Wait();
                if (_logServer.Enabled) { break; }
            }
        }




        public async Task<bool> SendAsync(string json)
        {
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var reponse = await _logServer.Client.PostAsync(_logServer.ApiUri, content))
            {
                return reponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }
    }
}
