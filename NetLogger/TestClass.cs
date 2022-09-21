using NetLogger;

namespace NetLogger
{
    internal class TestClass
    {
        public static void Run()
        {
            string logDir = @"D:\Test\Log_Client";
            string tableName = "TestLogA";

            using (var logger = new Logger<TestLogbody>(logDir, tableName))
            {
                logger.SetLogServer(new string[] { "http://localhost:5000" }, 5000, "http", 10000);

                var worker = new BackgroundWorker();
                worker.RepeatList.Add(logger);

                TestLogbody.Logger = logger;
                TestLogbody.MinLevel = LogLevel.Debug;

                int count = 1;

                Console.WriteLine("ログ 1グループ");
                TestLogbody.Write(LogLevel.Info, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Info, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Info, $"{count++}_Test", "これはテストですよ");

                /*
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Info, $"{count++}_Test", "これはテストですよ"));
                */

                Console.ReadLine();

                Console.WriteLine("ログ 2グループ");
                TestLogbody.Write(LogLevel.Warn, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Warn, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Warn, $"{count++}_Test", "これはテストですよ");
                /*
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Warn, $"{count++}_Test", "これはテストですよ"));
                */

                Console.ReadLine();

                Console.WriteLine("ログ 3グループ");
                TestLogbody.Write(LogLevel.Error, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Error, $"{count++}_Test", "これはテストですよ");
                TestLogbody.Write(LogLevel.Error, $"{count++}_Test", "これはテストですよ");
                /*
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));
                logger.Write(CreateLog(LogLevel.Error, $"{count++}_Test", "これはテストですよ"));
                */

                Console.ReadLine();
            }
        }
    }
}
