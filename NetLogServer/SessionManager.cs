namespace NetLogServer
{
    public class SessionManager : IRepeatable
    {
        private int _timeout = 3000;

        public List<Session> Sessions { get; set; }

        public SessionManager()
        {
            this.Sessions = new();
        }

        public Task Work()
        {
            //  タイムアウト時間以上経過したセッションを破棄
            for (int i = this.Sessions.Count - 1; i >= 0; i--)
            {
                var diff = Sessions[i].LastWriteTime - DateTime.Now;
                if (diff.TotalMilliseconds > _timeout)
                {
                    this.Sessions[i].Logger.Dispose();
                    this.Sessions.RemoveAt(i);
                }
            }

            return Task.CompletedTask;
        }
    }
}
