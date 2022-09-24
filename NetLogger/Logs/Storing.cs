using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    internal class Storing<T> : IDisposable, IRepeatable
    {
        protected static AsyncLock _lock = null;

        public string OutputDir { get; set; }
        public string OutputFilePath { get; set; }
        public string OutputDbPath { get; set; }
        public string TableNAme { get; set; }

        #region Protected parameter

        protected LiteDatabase _liteDB = null;
        protected ILiteCollection<T> _collection = null;
        protected DbManager _manager = null;
        protected LogServer _logServer = null;
        protected HttpClient _client = null;

        #endregion




        public virtual Task OutputTextAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OutputRemoteAsync()
        {
            return Task.CompletedTask;
        }




        #region Repeatable

        public virtual Task Work()
        {
            return Task.CompletedTask;
        }

        #endregion
        #region Close method

        public virtual void Close()
        {
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
