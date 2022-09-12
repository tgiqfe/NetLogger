using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public class LoggerBase<T> :
        IDisposable, IRepeatable
        where T : LogbodyBase
    {
        private static AsyncLock _lock = null;

        public string LogDir = null;
        public string LogFilePath = null;
        public string LogDbPath = null;

        private LiteDatabase _liteDB = null;
        private ILiteCollection<T> _collection = null;

        private DbManager _manager = null;

        private long _serial = 0;
        private bool _stored = false;

        public bool? IsToday
        {
            get
            {
                if (_manager == null) return null;
                return _manager.GetDate() == DateTime.Today;
            }
        }

        public LoggerBase(string logDir)
        {
            _lock ??= new AsyncLock();

            this.LogDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            //  ログ出力先情報をセット
            SetTodayLog();
        }

        /// <summary>
        /// 本日の日付でログ出力先情報をセット
        /// </summary>
        private void SetTodayLog()
        {
            string preName = typeof(T).GetField("Name").GetValue(typeof(T)) as string;

            string today = DateTime.Now.ToString("yyyyMMdd");
            this.LogFilePath = Path.Combine(LogDir, $"{preName}_{today}.log");
            this.LogDbPath = Path.Combine(LogDir, $"{preName}_{today}.db");

            _liteDB = new LiteDatabase($"Filename={LogDbPath};Connection=shared");
            _collection = _liteDB.GetCollection<T>(preName);

            _manager = new DbManager(_liteDB);
            _serial = DateTime.Now.Ticks;
        }

        /// <summary>
        /// ログをDBに追記
        /// </summary>
        /// <param name="logBody"></param>
        public async void Write(T logBody)
        {
            using (await _lock.LockAsync())
            {

                logBody.Serial = _serial++;
                _collection.Upsert(logBody);
                _stored = true;
            }
        }

        public async Task Work()
        {
            await OutputAsync();
        }

        public void ResetDate()
        {
            _liteDB.Dispose();
            SetTodayLog();
        }

        private async Task OutputAsync()
        {
            if (_stored)
            {
                using (await _lock.LockAsync())
                {
                    long lastSerial = _manager.GetLastSerial(reload: true);
                    var result = _collection.Query().Where(x => x.Serial > lastSerial).ToList();

                    using (var sw = new StreamWriter(LogFilePath, true, Encoding.UTF8))
                    {
                        foreach (var item in result)
                        {
                            sw.WriteLine($"[{item.Date}][{item.Level}]{item.Title} {item.Message}");
                            _manager.SetLastSerial(item.Serial);
                        }
                    }
                    _manager.Upsert();

                    _stored = false;
                }
            }
        }


        #region Close method

        public virtual void Close()
        {
            OutputAsync().Wait();
            if (_liteDB != null) { _liteDB.Dispose(); _liteDB = null; }
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
