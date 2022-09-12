using NetLogger.Logs;
using System.IO;

namespace NetLogServer.Logs
{
    public class DynamicLogger : LoggerBase<DynamicLogBody>
    {
        public DynamicLogger(string logDir) : base(logDir)
        {
        }


    }
}
