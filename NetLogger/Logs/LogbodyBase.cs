
namespace NetLogger.Logs
{
    public class LogbodyBase
    {
        public DateTime Date { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public LogLevel Level { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{this.Date.ToString("yyyy/MM/dd HH:mm:ss")}][{this.Level}] {this.Title} {this.Message}";
        }
    }
}
