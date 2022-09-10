
namespace NetLogger.Logs
{
    internal class LogPath
    {
        private string _today = null;
        public string LogDir { get; private set; }
        public string FilePath { get; private set; }
        public string DbPath { get; private set; }

        public LogPath(string logDir, string preName)
        {
            this._today = DateTime.Now.ToString("yyyyMMdd");
            this.LogDir = logDir;
            this.FilePath = Path.Combine(LogDir, $"{preName}_{_today}.log");
            this.DbPath = Path.Combine(LogDir, $"{preName}_{_today}.db");
        }
    }
}
