using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NetLogger.Logs
{
    internal class LogServer
    {
        #region Private parameter

        private string _server { get; set; }
        private int _port { get; set; }
        private string _protocol { get; set; }

        #endregion

        public bool Enabled { get; private set; }
        public string Uri { get { return $"{_protocol}://{_server}:{_port}"; } }
        public HttpClient Client { get; private set; }

        /// <summary>
        /// ログサーバーを1つだけ指定
        /// </summary>
        /// <param name="server"></param>
        /// <param name="defPort"></param>
        /// <param name="defProtocol"></param>
        public LogServer(string server, int defPort, string defProtocol)
        {
            ReadURI(server);
            if (_port == 0) _port = defPort;
            if (string.IsNullOrEmpty(_protocol)) _protocol = defProtocol;

            //  ★★★ここでTCP接続可否チェック★★★
        }

        /// <summary>
        /// ログサーバーを複数指定
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="defPort"></param>
        /// <param name="defProtocol"></param>
        public LogServer(string[] servers, int defPort, string defProtocol)
        {
            var random = new Random();
            string[] array = servers.OrderBy(x => random.Next()).ToArray();
            foreach (var server in array)
            {
                ReadURI(server);
                if (_port == 0) _port = defPort;
                if (string.IsNullOrEmpty(_protocol)) _protocol = defProtocol;

                //  ★★★ここでTCP接続可否チェック★★★

            }
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

    }
}
