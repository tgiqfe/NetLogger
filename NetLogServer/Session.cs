
namespace NetLogServer
{
    public class Session
    {
        public string Table { get; set; }
        public NetLogger.Logs.Logger<LiteDB.BsonDocument> Logger { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
