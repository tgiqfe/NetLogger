using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NetLogger.Logs
{
    internal class OutputerText<T>
    {
        public bool Stored { get; set; }

        private ILiteCollection<T> _collection = null;

        public OutputerText(ILiteCollection<T> collection)
        {
            _collection = collection;
        }



    }
}
