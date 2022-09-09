
namespace NetLogger.Logs
{
    internal class LogbodyBase
    {
        public virtual string Name { get { return ""; } }

        public long Serial { get; set; }
        public DateTime Date { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public LogLevel Level { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }



    }
}
