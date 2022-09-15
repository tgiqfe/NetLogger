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
        const string HEAD_LINE = "manager";
        const string MANAGER_TABLE = "mngtable";

        private ILiteCollection<DbManagerItem> _collection = null;

        private DbManagerItem _cachedItem = null;

        public DbManager(LiteDatabase liteDB)
        {
            _collection = liteDB.GetCollection<DbManagerItem>(MANAGER_TABLE);
            _collection.EnsureIndex(x => x.HeadLine, true);
        }

        public int GetLastTextIndex(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ? 
                    record[0] : 
                    new DbManagerItem() { Date = DateTime.Today, HeadLine = HEAD_LINE};
            }
            return _cachedItem.LastTextIndex;
        }

        public int GetLastRemoteIndex(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ?
                    record[0] :
                    new DbManagerItem() { Date = DateTime.Today, HeadLine = HEAD_LINE };
            }
            return _cachedItem.LastRemoteIndex;
        }

        public DateTime GetDate(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ?
                    record[0] :
                    new DbManagerItem() { Date = DateTime.Today, HeadLine = HEAD_LINE };
            }
            return _cachedItem.Date;
        }

        public void SetLastTextIndex(int index)
        {
            _cachedItem.LastTextIndex = index;
        }

        public void SetLastRemoteIndex(int index)
        {
            _cachedItem.LastRemoteIndex = index;
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
