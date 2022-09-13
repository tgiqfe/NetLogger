using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    internal interface IRepeatable
    {
        bool? IsToday { get; }
        Task Work();
        void ResetDate();
    }
}
