using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    internal class DbManagerItem
    {
        const string HEAD_LINE = "manager";

        [BsonId]
        public string HeadLine { get; set; } = HEAD_LINE;
        public long LastSerial { get; set; }
        public DateTime Date { get; set; }
    }
}
