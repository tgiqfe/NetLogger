
namespace NetLogger
{
    internal class TestClass
    {
        public static void Run()
        {
            string logDir = @"D:\Test\Loggggg";
            string tableName = "TestLog";
            string logName = "Temp";

            using (var logger = new LoggerBase<LogbodyBase>(logDir, tableName))
            {
                var worker = new BackgroundWorker();
                worker.RepeatTargets.Add(logger);

                int count = 1;

                Console.WriteLine("ログ 1グループ");
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));

                Console.ReadLine();

                Console.WriteLine("ログ 2グループ");
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));

                Console.ReadLine();

                Console.WriteLine("ログ 3グループ");
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));

                Console.ReadLine();
            }
        }

        private static LogbodyBase CreateLog(LogLevel level, string title, string message)
        {
            return new LogbodyBase()
            {
                Date = DateTime.Now,
                HostName = Environment.MachineName,
                UserName = Environment.UserName,
                Level = level,
                Title = title,
                Message = message,
            };
        }
    }
}
