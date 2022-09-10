using NetLogger;
using NetLogger.Logs;

LogbodyBase body1 = new LogbodyBase()
{
    Date = DateTime.Now,
    HostName = "tqWin04",
    UserName = "User",
    Level = LogLevel.Info,
    Title = "sample1",
    Message = "これはテストです",
};
LogbodyBase body2 = new LogbodyBase()
{
    Date = DateTime.Now,
    HostName = "tqWin04",
    UserName = "User",
    Level = LogLevel.Info,
    Title = "sample2",
    Message = "これはテストです",
};
LogbodyBase body3 = new LogbodyBase()
{
    Date = DateTime.Now,
    HostName = "tqWin04",
    UserName = "User",
    Level = LogLevel.Info,
    Title = "sample3",
    Message = "これもテストです",
};


using (Logger<LogbodyBase> logger = new NetLogger.Logs.Logger<LogbodyBase>(@"D:\Test\Log"))
{
    OutputWorker worker = new OutputWorker();
    worker.RepeatTargets.Add(logger);



    logger.Write(body1);
    logger.Write(body2);
    logger.Write(body3);

    Console.ReadLine();

    logger.Write(body1);
    logger.Write(body2);
    logger.Write(body3);

    Console.ReadLine();

    logger.Write(body1);
    logger.Write(body2);
    logger.Write(body3);

    Console.ReadLine();
}


