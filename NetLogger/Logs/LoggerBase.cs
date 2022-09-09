using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    internal class LoggerBase<T> :
        IDisposable
        where T : LogbodyBase
    {
        private static AsyncLock _lock = null;
        private string _logDir = null;
        private string _logPreName = null;
        private string _logFilePath = null;
        private string _logDbPath = null;

        private LiteDatabase _liteDB = null;
        private ILiteCollection<T> _collection = null;

        private long _serial = 0;
        private bool _during = true;
        private bool _stored = false;

        public LoggerBase(string logDir, string logPreName, int outputInterval)
        {
            _lock ??= new AsyncLock();

            this._logDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this._logPreName = logPreName;

            //  ログ出力先情報をセット
            SetTodayLog();

            //  非同期でログファイルへ出力
            Outputter(outputInterval).ConfigureAwait(false);
        }

        /// <summary>
        /// 本日の日付でログ出力先情報をセット
        /// </summary>
        private void SetTodayLog()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            _logFilePath = Path.Combine(_logDir, $"{_logPreName}_{today}.log");
            _logDbPath = Path.Combine(_logDir, $"{_logPreName}_{today}.db");

            _liteDB = new LiteDatabase($"Filename={_logDbPath};Connection=shared");
            _collection = _liteDB.GetCollection<T>(_logPreName);
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

        /// <summary>
        /// 非同期でファイルに出力
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        private async Task Outputter(int interval)
        {
            //  テーブル名とHeadLineの名前用の文字列
            string managerText = "manager";

            var mngCol = _liteDB.GetCollection<DbManager>(managerText);
            mngCol.EnsureIndex(x => x.HeadLine, true);
            while (_during)
            {
                await Task.Delay(interval);
                if (_stored)
                {
                    using (await _lock.LockAsync())
                    {
                        var mngRec = mngCol.FindOne(x => x.HeadLine == managerText) ?? new DbManager(managerText);
                        var result = _collection.Query().Where(x => x.Serial > mngRec.LastSerial).ToList();

                        using (var sw = new StreamWriter(_logFilePath, true, Encoding.UTF8))
                        {
                            foreach (var item in result)
                            {
                                sw.WriteLine($"[{item.Date}][{item.Level}]{item.Title} {item.Message}");
                                mngRec.LastSerial = item.Serial;
                            }
                        }
                        mngCol.Upsert(mngRec);

                        //  日にちをまたいだ場合
                        if (mngRec.Date != DateTime.Today)
                        {
                            _liteDB.Dispose();
                            SetTodayLog();
                            mngCol = _liteDB.GetCollection<DbManager>(managerText);
                            mngCol.EnsureIndex(x => x.HeadLine, true);
                        }

                        _stored = false;
                    }
                }
            }
        }


        #region Close method

        public virtual async Task CloseAsync()
        {
            using (await _lock.LockAsync())
            {
                Close();
            }
        }

        public virtual void Close()
        {
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
