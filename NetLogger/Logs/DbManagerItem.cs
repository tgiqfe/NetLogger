
namespace NetLogger.Logs
{
    public class DbManagerItem
    {
        [LiteDB.BsonId]
        public string HeadLine { get; set; } 
        public int LastIndex { get; set; }
        public DateTime Date { get; set; }
    }
}
