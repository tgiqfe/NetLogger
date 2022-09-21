using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Logs
{
    public interface IRepeatable
    {
        Task Work();
    }
}
