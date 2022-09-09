﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    internal class DbManager
    {
        public const string HEAD_LINE = "manager";

        [BsonId]
        public string HeadLine { get; set; } = HEAD_LINE;
        public long LastSerial { get; set; }
        public DateTime Date { get; set; }

        public DbManager()
        {
            this.Date = DateTime.Today;
        }

        public static ILiteCollection<DbManager> GetCollection(LiteDatabase liteDB)
        {
            var collection = liteDB.GetCollection<DbManager>(DbManager.HEAD_LINE);
            collection.EnsureIndex(x => x.HeadLine, true);
            return collection;
        }
    }
}
