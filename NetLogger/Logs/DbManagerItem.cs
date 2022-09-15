
namespace NetLogger.Logs
{
    public class DbManagerItem
    {
        [LiteDB.BsonId]
        public string HeadLine { get; set; } 
        public int LastTextIndex { get; set; }
        public int LastRemoteIndex { get; set; }
        public DateTime Date { get; set; }
    }
}
