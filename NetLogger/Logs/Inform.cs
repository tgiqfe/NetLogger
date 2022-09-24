using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    //  ステータス情報出力用
    internal class Inform<T> : Storing<T>
    {


        public Inform(string logDir, string tableName)
        {

        }

        public override async Task OutputTextAsync()
        {
            using (await _lock.LockAsync())
            {
                var items = _collection.FindAll();
                using (var sw = new StreamWriter("<output dir>", false, Encoding.UTF8))
                {
                    foreach(var item in items)
                    {
                        sw.WriteLine(item.ToString());
                    }
                }
            }
        }
    }
}
