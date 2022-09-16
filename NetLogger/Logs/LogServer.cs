using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace NetLogger.Logs
{
    internal class LogServer
    {
        #region Private parameter

        private string _server = null;
        private int _port = 0;
        private string _protocol = null;
        private bool _reachable = false;
        private bool _connectable = false;

        #endregion

        public bool Enabled { get; private set; }
        public string Uri { get { return $"{_protocol}://{_server}:{_port}"; } }

        public LogServer(string server, int defPort, string defProtocol)
        {
            ReadURI(server);
            if (_port == 0) _port = defPort;
            if (string.IsNullOrEmpty(_protocol)) _protocol = defProtocol;
        }

        /// <summary>
        /// 指定のURIを一旦分解してサーバ等情報を取得
        /// </summary>
        /// <param name="uri"></param>
        private void ReadURI(string uri)
        {
            string tempServer = uri;
            string tempPort = "0";
            string tempProtocol = "";

            Match match;
            if ((match = Regex.Match(uri, "^.+(?=://)")).Success)
            {
                tempProtocol = match.Value;
                tempServer = tempServer.Substring(tempServer.IndexOf("://") + 3);
            }
            if ((match = Regex.Match(tempServer, @"(?<=:)\d+")).Success)
            {
                tempPort = match.Value;
                tempServer = tempServer.Substring(0, tempServer.IndexOf(":"));
            }

            _server = tempServer;
            _port = int.Parse(tempPort);
            _protocol = tempProtocol.ToLower();
        }

        /// <summary>
        /// サーバ接続可否チェック
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task TestConnect(int waitTime)
        {
            //  Pingチェック
            int interval = 1000;
            int timeout = 1000;
            Ping ping = new Ping();

            DateTime startTime = DateTime.Now;
            int maxTestCount = 10;
            int count = 0;
            do
            {
                PingReply reply = await ping.SendPingAsync(_server, timeout);
                if (reply.Status == IPStatus.Success)
                {
                    _reachable = true;
                    break;
                }
                await Task.Delay(interval);
                count++;
                if (count > maxTestCount) { break; }
            } while ((DateTime.Now - startTime).TotalMilliseconds > waitTime);
            if (!_reachable) { return; }

            //  TCP接続チェック
            using (var client = new TcpClient())
            {
                timeout = 3000;
                try
                {
                    Task task = client.ConnectAsync(_server, _port);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) != task)
                    {
                        throw new SocketException(10060);
                    }
                    _connectable = true;
                }
                catch { }
            }

            this.Enabled = _connectable;
        }
    }
}
