using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Worker
{
    internal class BackgroundWorker : IDisposable
    {
        public bool Running = false;
        public int Interval = 5000;

        public List<IBackgroundRepeat> RepeatTargets = new List<IBackgroundRepeat>();

        public BackgroundWorker()
        {
            this.RepeatTargets = new();

            this.Running = true;
            Run().ConfigureAwait(false);
        }

        public async Task Run()
        {
            while (this.Running)
            {
                foreach (var repeatTarget in this.RepeatTargets)
                {
                    await repeatTarget.Work();
                }
                await Task.Delay(this.Interval);
            }
        }



        #region Close method

        public virtual void Close()
        {
            this.Running = false;
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
