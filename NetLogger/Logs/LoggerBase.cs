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
    public class LoggerBase<T> : IDisposable, IRepeatable
    {
        protected static AsyncLock _lock = null;

        public string LogDir { get; set; }
        public string TableName { get; set; }
        public LogServerSession Session { get; set; }

        public string LogFilePath = null;
        public string LogDbPath = null;

        #region Private,Protected

        /// <summary>
        /// LiteDBへのコネクション
        /// </summary>
        protected LiteDatabase _liteDB = null;

        /// <summary>
        /// 本クラスで使用するテーブル
        /// </summary>
        protected ILiteCollection<T> _collection = null;

        /// <summary>
        /// DB情報管理
        /// </summary>
        private DbManager _manager = null;

        #endregion

        public bool? IsToday
        {
            get
            {
                if (_manager == null) return null;
                return _manager.GetDate() == DateTime.Today;
            }
        }

        public LoggerBase(string logDir, string tableName, LogServerSession session)
        {
            _lock ??= new AsyncLock();

            this.LogDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this.TableName = tableName;
            this.Session = session;

            //  ログ出力先情報をセット
            SetTodayLog();
        }

        /// <summary>
        /// 本日の日付でログ出力先情報をセット
        /// </summary>
        private void SetTodayLog()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            this.LogFilePath = Path.Combine(LogDir, $"{TableName}_{today}.log");
            this.LogDbPath = Path.Combine(LogDir, $"{TableName}_{today}.db");

            _liteDB = new LiteDatabase($"Filename={LogDbPath};Connection=shared");
            _collection = _liteDB.GetCollection<T>(TableName);

            _manager = new DbManager(_liteDB);
        }

        #region Write log

        /// <summary>
        /// ログをDBに追記
        /// </summary>
        /// <param name="logBody"></param>
        public async void Write(T logBody)
        {
            using (await _lock.LockAsync())
            {
                _collection.Upsert(logBody);
            }
        }

        #endregion

        public async Task OutputTextAsync()
        {
            int index = _manager.GetTextIndex(reload: true);
            int count = _collection.FindAll().Count() - 1;
            if (index < count)
            {
                using (await _lock.LockAsync())
                {
                    var items = _collection.Query().Skip(index).ToArray();
                    using (var sw = new StreamWriter(LogFilePath, true, Encoding.UTF8))
                    {
                        foreach (var item in items)
                        {
                            sw.WriteLine(item.ToString());
                            _manager.IncreaseTextIndex();
                        }
                    }
                    _manager.Upsert();
                }
            }
        }



        public async Task OutputRemoteAsync()
        {
            int index = _manager.GetRemoteIndex(reload: true);
            int count = _collection.FindAll().Count() - 1;
            if (index < count)
            {
                using (await _lock.LockAsync())
                {
                    var items = _collection.Query().Skip(index).ToArray();
                    foreach (var item in items)
                    {
                        string json = System.Text.Json.JsonSerializer.Serialize(item);
                        bool ret = await Session.SendAsync(TableName, json);
                        if (ret)
                        {
                            _manager.IncreaseRemoteIndex();
                        }
                        else
                        {
                            break;
                        }
                    }
                    _manager.Upsert();
                }
            }
        }



        #region Repeatable

        public async Task Work()
        {
            await OutputTextAsync();

            if(this.Session != null)
            {
                await OutputRemoteAsync();
            }
        }

        public void ResetDate()
        {
            _liteDB.Dispose();
            SetTodayLog();
        }

        #endregion
        #region Close method

        public virtual void Close()
        {
            OutputTextAsync().Wait();
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
