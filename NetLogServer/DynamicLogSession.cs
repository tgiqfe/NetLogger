
namespace NetLogServer
{
    public class DynamicLogSession
    {
        public string Table { get; set; }
        public LoggerBase<LiteDB.BsonDocument> Logger { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
