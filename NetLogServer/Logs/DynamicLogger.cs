using NetLogger.Logs;
using System.IO;
using LiteDB;
//using System.Text.Json;

namespace NetLogServer.Logs
{
    public class DynamicLogger : LoggerBase<DynamicLogBody>
    {
        public DynamicLogger(string logDir) : base(logDir)
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

                        //_collection.Insert(doc);

                    }
                }
            }
            catch { }
        }
    }
}
