namespace NetLogger
{
    public class TestLogbody
    {
        public DateTime Date { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public LogLevel Level { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{Date.ToString("yyyy/MM/dd HH:mm:ss")}][{Level}] {Title} {Message}";
        }
    }
}
