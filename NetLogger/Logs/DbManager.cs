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

        private ILiteCollection<DbManagerItem> _collection = null;

        private DbManagerItem _cachedItem = null;

        public DbManager(LiteDatabase liteDB)
        {
            _collection = liteDB.GetCollection<DbManagerItem>(HEAD_LINE);
            _collection.EnsureIndex(x => x.HeadLine, true);
        }

        public long GetLastSerial(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ? record[0] : new DbManagerItem();
            }
            return _cachedItem.LastSerial;
        }


        public int GetLastIndex(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ? record[0] : new DbManagerItem();
            }
            return _cachedItem.LastIndex;
        }



        public DateTime GetDate(bool reload = false)
        {
            if (_cachedItem == null || reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedItem = record.Length > 0 ? record[0] : new DbManagerItem();
            }
            return _cachedItem.Date;
        }

        public void SetLastSerial(long serial)
        {
            _cachedItem.LastSerial = serial;
        }

        public void SetLastIndex(int index)
        {
            _cachedItem.LastIndex = index;
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
