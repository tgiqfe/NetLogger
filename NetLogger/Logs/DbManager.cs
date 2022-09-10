using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    internal class DbManager
    {
        const string HEAD_LINE = "manager";

        private ILiteCollection<DbManagerItem> _collection = null;

        private DbManagerItem _cachedDbManager = null;

        public DbManager(LiteDatabase liteDB)
        {
            _collection = liteDB.GetCollection<DbManagerItem>(HEAD_LINE);
            _collection.EnsureIndex(x => x.HeadLine, true);
        }

        public long GetLastSerial(bool reload = false)
        {
            if (reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedDbManager = record.Length > 0 ? record[0] : new DbManagerItem();
            }
            return _cachedDbManager.LastSerial;
        }

        public DateTime GetDate(bool reload = false)
        {
            if (reload)
            {
                var record = _collection.FindAll().ToArray();
                this._cachedDbManager = record.Length > 0 ? record[0] : new DbManagerItem();
            }
            return _cachedDbManager.Date;
        }

        public void SetLastSerial(long serial)
        {
            _cachedDbManager.LastSerial = serial;
        }

        public void Upsert()
        {
            if (_cachedDbManager != null)
            {
                _collection.Upsert(_cachedDbManager);
            }
        }
    }
}
