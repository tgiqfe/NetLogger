using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    internal class OutputWorker : IDisposable
    {
        public bool Running = false;
        public int Interval = 5000;

        public List<IRepeatable> RepeatTargets = null;

        public OutputWorker()
        {
            RepeatTargets = new();

            Running = true;
            Run().ConfigureAwait(false);
        }

        public async Task Run()
        {
            while (Running)
            {
                foreach (var repeatTarget in RepeatTargets)
                {
                    await repeatTarget.Work();
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
