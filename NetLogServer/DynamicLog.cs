using LiteDB;

namespace NetLogServer
{
    public class DynamicLog
    {
        private List<DynamicLogSession> _sessionList { get; set; }

        private BackgroundWorker _worker = null;

        public DynamicLog()
        {
            _sessionList = new List<DynamicLogSession>();
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

            try
            {
                var session = _sessionList.First(x => x.Table == table);
                using (var sr = new StreamReader(stream))
                {
                    var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                    session.Logger.Write(doc);
                    session.LastWriteTime = DateTime.Now;
                }
            }
            catch { }
        }
    }
}
