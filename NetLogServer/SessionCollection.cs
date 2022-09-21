namespace NetLogServer
{
    public class SessionCollection : IRepeatable
    {
        private int _timeout = 3000;

        public List<Session> Sessions { get; set; }

        public bool? IsToday => throw new NotImplementedException();

        public void ResetDate()
        {
            throw new NotImplementedException();
        }

        public Task Work()
        {
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
