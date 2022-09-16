using LiteDB;

namespace NetLogServer
{
    public class DynamicLog
    {
        private List<DynamicLogSession> _sessionList { get; set; }

        //private Dictionary<string, LoggerBase<BsonDocument>> _session { get; set; }

        private BackgroundWorker _worker = null;

        public DynamicLog()
        {
            _sessionList = new List<DynamicLogSession>();

            //_session = new();
            _worker = new();
        }

        public void Write(string table, Stream stream)
        {
            if (string.IsNullOrEmpty(table)) { return; }

            if (!_sessionList.Any(x => x.Table == table))
            {
                var tempSession = new DynamicLogSession()
                {
                    Table = table,
                    Logger = new LoggerBase<BsonDocument>(@"D:\Test\Loggggg", table, null),
                };
                _worker.RepeatList.Add(tempSession.Logger);
                _sessionList.Add(tempSession);
            }


            /*
            if (!_session.ContainsKey(table))
            {
                var tempLogger = new LoggerBase<BsonDocument>(@"D:\Test\Loggggg", table, null);
                _worker.RepeatList.Add(tempLogger);
                _session[table] = tempLogger;
            }
            */

            try
            {
                var session = _sessionList.First(x => x.Table == table);
                using (var sr = new StreamReader(stream))
                {
                    var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                    session.Logger.Write(doc);
                    session.LastWriteTime = DateTime.Now;
                }

                /*
                var logger = _session[table];
                using (var sr = new StreamReader(stream))
                {
                    var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                    logger.Write(doc);
                }
                */
            }
            catch { }
        }
    }
}
