using LiteDB;
using NetLogger.Logs;
using System.Collections.Generic;
using System.IO;

namespace NetLogServer.Logs
{
    public class DynamicLogManager
    {
        public Dictionary<string, LoggerBase<BsonDocument>> Session { get; set; }

        private OutputWorker outputWorker = null;

        public DynamicLogManager()
        {
            this.Session = new();
            this.outputWorker = new();
        }

        public void Write(string table, Stream stream)
        {
            if (string.IsNullOrEmpty(table)) { return; }

            if (!this.Session.ContainsKey(table))
            {
                //  ログ名もテーブル名に合わせる
                var tempLogger = new LoggerBase<BsonDocument>(@"D:\Test\Loggggg", table, table);
                outputWorker.RepeatTargets.Add(tempLogger);
                this.Session[table] = tempLogger;
            }

            try
            {
                var logger = Session[table];
                using (var sr = new StreamReader(stream))
                {
                    var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                    logger.Write(doc);
                }
            }
            catch { }
        }
    }
}
