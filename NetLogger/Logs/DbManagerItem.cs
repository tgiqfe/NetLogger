using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    public class DbManagerItem
    {
        [BsonId]
        public string HeadLine { get; set; } 
        public long LastSerial { get; set; }

        public int LastIndex { get; set; }

        public DateTime Date { get; set; }

        public DbManagerItem()
        {
            this.HeadLine = "manager";
            this.Date = DateTime.Today;
        }
    }
}
