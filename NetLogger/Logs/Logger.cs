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
    public class Logger<T> : IDisposable, IRepeatable
    {
        protected static AsyncLock _lock = null;

        public string LogDir { get; set; }
        public string TableName { get; set; }
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
        /// ログ転送先サーバ
        /// </summary>
        private LogServer _logServer = null;

        /// <summary>
        /// ログ転送用HttpClient
        /// </summary>
        private HttpClient _client = null;

        #endregion

        public Logger(string logDir, string tableName)
        {
            _lock = new AsyncLock();
            this.LogDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this.TableName = tableName;

            //  ログ出力先情報をセット
            Init();
        }

        public void SetLogServer(string[] uris, int defPort, string defProtocol, int waitTime)
        {
            Random random = new();
            string[] array = uris.OrderBy(x => random.Next()).ToArray();
            foreach (var uri in array)
            {
                _logServer = new LogServer(uri, defPort, defProtocol);
                _logServer.TestConnect(waitTime).Wait();
                if (_logServer.Enabled)
                {
                    _client = new HttpClient();
                    break;
                }
            }
        }

        /// <summary>
        /// 本日の日付でログ出力先情報をセット
        /// </summary>
        private void Init()
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

                        using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                        using (var response = await _client.PostAsync($"{_logServer.Uri}/api/logger/{TableName}", content))
                        {
                            bool ret = response.StatusCode == System.Net.HttpStatusCode.OK;
                            if (ret)
                            {
                                _manager.IncreaseRemoteIndex();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    _manager.Upsert();
                }
            }
        }

        #region Repeatable

        public async Task Work()
        {
            //  ローカルログ出力
            await OutputTextAsync();

            //  リモートログ転送
            if (_client != null)
            {
                await OutputRemoteAsync();
            }

            //  日付リセット
            if (_manager.GetDate() != DateTime.Today)
            {
                ResetDate();
            }
        }

        public void ResetDate()
        {
            _liteDB.Dispose();
            Init();
        }

        #endregion
        #region Close method

        public virtual void Close()
        {
            OutputTextAsync().Wait();
            if (_liteDB != null) { _liteDB.Dispose(); _liteDB = null; }
            if (_client != null) { _client.Dispose(); _client = null; }
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
