using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    public class DbManager
    {
        #region Private parameter

        const string HEAD_LINE = "manager";

        const string MANAGER_TABLE = "mngtable";

        private ILiteCollection<DbManagerItem> _collection = null;

        private DbManagerItem _cachedItem = null;

        #endregion

        public DbManager(LiteDatabase liteDB)
        {
            _collection = liteDB.GetCollection<DbManagerItem>(MANAGER_TABLE);
            _collection.EnsureIndex(x => x.HeadLine, true);
        }

        private void SetCachedItem(bool reload)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ?
                    record[0] :
                    new DbManagerItem()
                    {
                        HeadLine = HEAD_LINE,
                        TextIndex = 0,
                        RemoteIndex = 0,
                        Date = DateTime.Today
                    };
            }
        }

        public int GetTextIndex(bool reload = false)
        {
            SetCachedItem(reload);
            return _cachedItem.TextIndex;
        }

        public int GetRemoteIndex(bool reload = false)
        {
            SetCachedItem(reload);
            return _cachedItem.RemoteIndex;
        }

        public DateTime GetDate(bool reload = false)
        {
            SetCachedItem(reload);
            return _cachedItem.Date;
        }

        public void IncreaseTextIndex()
        {
            _cachedItem.TextIndex++;
        }

        public void IncreaseRemoteIndex()
        {
            _cachedItem.RemoteIndex++;
        }


        public void Upsert()
        {
            if (_cachedItem != null)
            {
                _collection.Upsert(_cachedItem);
            }
        }
    }
}
