﻿using LiteDB;
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

        public string LogDir = null;
        public string TableName = null;
        public string LogName = null;

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

        /// <summary>
        /// DBへ書き込み済み/未書き込み
        /// </summary>
        private bool _stored = false;

        #endregion

        public bool? IsToday
        {
            get
            {
                if (_manager == null) return null;
                return _manager.GetDate() == DateTime.Today;
            }
        }

        public LoggerBase(string logDir, string tableName, string logName)
        {
            _lock ??= new AsyncLock();

            this.LogDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this.TableName = tableName;
            this.LogName = logName;

            //  ログ出力先情報をセット
            SetTodayLog();
        }

        /// <summary>
        /// 本日の日付でログ出力先情報をセット
        /// </summary>
        private void SetTodayLog()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            this.LogFilePath = Path.Combine(LogDir, $"{LogName}_{today}.log");
            this.LogDbPath = Path.Combine(LogDir, $"{LogName}_{today}.db");

            _liteDB = new LiteDatabase($"Filename={LogDbPath};Connection=shared");
            _collection = _liteDB.GetCollection<T>(TableName);

            _manager = new DbManager(_liteDB);
        }

        /// <summary>
        /// ログをDBに追記
        /// </summary>
        /// <param name="logBody"></param>
        public async void Write(T logBody)
        {
            using (await _lock.LockAsync())
            {
                _collection.Upsert(logBody);
                _stored = true;
            }
        }

        /// <summary>
        /// 手動or他メソッドからの呼び出しで、DBからファイルへ書き込み
        /// </summary>
        /// <returns></returns>
        public async Task OutputTextAsync()
        {
            if (_stored)
            {
                using (await _lock.LockAsync())
                {
                    int lastIndex = _manager.GetLastIndex(reload: true);

                    var cols = _collection.Query().Skip(lastIndex).ToArray();
                    using (var sw = new StreamWriter(LogFilePath, true, Encoding.UTF8))
                    {
                        foreach (var item in cols)
                        {
                            sw.WriteLine(item.ToString());
                        }
                    }
                    _manager.SetLastIndex(cols.Length);
                    _manager.Upsert();
                    _stored = false;
                }
            }
        }

        #region Repeatable

        public async Task Work()
        {
            await OutputTextAsync();
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
