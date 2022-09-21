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

        #region Log write

        public static LogLevel MinLevel = LogLevel.Info;

        public static Logger<TestLogbody> Logger = null;

        public static void Write(LogLevel level, string title, string message)
        {
            if(level >= MinLevel)
            {
                var logBody = new TestLogbody()
                {
                    Date = DateTime.Now,
                    HostName = Environment.MachineName,
                    UserName = Environment.UserName,
                    Level = level,
                    Title = title,
                    Message = message,
                };
                Logger.Write(logBody);
            }
        }

        public static void Write(string title, string message)
        {
            Write(LogLevel.Info, title, message);
        }

        public static void Write(string message)
        {
            Write(LogLevel.Info, "", message);
        }

        #endregion
    }
}
