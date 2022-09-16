using LiteDB;

namespace NetLogServer.Logs
{
    public class DynamicLogger : LoggerBase<BsonDocument>
    {
        public DynamicLogger(string logDir, string tableName, LogServerSession session) :
            base(logDir, tableName, session)
        {
        }

        public async Task Write(string table, Stream stream)
        {
            if (string.IsNullOrEmpty(table)) { return; }

            try
            {
                using (await _lock.LockAsync())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                        _collection.Upsert(doc);
                    }
                }
            }
            catch { }
        }
    }
}
