using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public class BackgroundWorker : IDisposable
    {
        public bool Running { get; set; }
        public int Interval { get; set; } = 5000;

        public List<IRepeatable> RepeatList = null;

        public BackgroundWorker()
        {
            RepeatList = new();

            Running = true;
            Run().ConfigureAwait(false);
        }

        public async Task Run()
        {
            while (Running)
            {
                foreach (var repeatTarget in RepeatList)
                {
                    //  通常Work
                    await repeatTarget.Work();

                    //  日付リセット用処理
                    if (repeatTarget.IsToday == false)
                    {
                        repeatTarget.ResetDate();
                    }
                }
                await Task.Delay(Interval);
            }
        }



        #region Close method

        public virtual void Close()
        {
            Running = false;
        }

        #endregion
        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
