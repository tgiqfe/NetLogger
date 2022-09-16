using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public class LogServerSession : IDisposable
    {
        #region Private parameter

        private LogServer _logServer = null;

        private HttpClient _client = null;

        const string API_URI = "api/logger";

        #endregion 

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
                if (_logServer.Enabled)
                {
                    _client = new HttpClient();
                    break;
                }
            }
        }




        public async Task<bool> SendAsync(string table, string json)
        {
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var reponse = await _client.PostAsync($"{_logServer.Uri}/{API_URI}/{table}", content))
            {
                return reponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }



        #region Close method

        public virtual void Close()
        {
            if (_client != null) { _client.Dispose(); _client = null; }
        }

        #endregion
        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
