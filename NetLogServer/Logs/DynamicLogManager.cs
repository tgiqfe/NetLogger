using LiteDB;

namespace NetLogServer.Logs
{
    public class DynamicLogManager
    {
        private Dictionary<string, LoggerBase<BsonDocument>> _session { get; set; }
        private BackgroundWorker _worker = null;

        public DynamicLogManager()
        {
            this._session = new();
            this._worker = new();
        }

        public void Write(string table, Stream stream)
        {
            if (string.IsNullOrEmpty(table)) { return; }

            if (!this._session.ContainsKey(table))
            {
                var tempLogger = new LoggerBase<BsonDocument>(@"D:\Test\Loggggg", table, null);
                _worker.RepeatTargets.Add(tempLogger);
                this._session[table] = tempLogger;
            }

            try
            {
                var logger = _session[table];
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
