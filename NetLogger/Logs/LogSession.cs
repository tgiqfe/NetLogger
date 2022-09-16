﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public class LogSession
    {
        private LogServer _logServer = null;

        public LogSession()
        {

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