
namespace NetLogger.Logs
{
    public class DbManagerItem
    {
        [LiteDB.BsonId]
        public string HeadLine { get; set; } 
        public int TextIndex { get; set; }
        public int RemoteIndex { get; set; }
        public DateTime Date { get; set; }
    }
}
