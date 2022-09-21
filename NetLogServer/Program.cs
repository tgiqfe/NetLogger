using LiteDB;
using NetLogServer;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

using (var worker = new BackgroundWorker())
{
    var manager = new SessionManager();
    worker.RepeatList.Add(manager);

    #region Routing

    //  �g�b�v�T�C�g�ւ�GET�ʐM => �󔒂�Ԃ�
    app.MapGet("/", () => "");

    //  �g�b�v�T�C�g�ւ�POST�ʐM => �󔒂�Ԃ�
    app.MapPost("/", () => "");

    //  ���O�]���̎�M
    app.MapPost("/api/logger/{table}", (HttpContext context) =>
    {
        //  �f�o�b�O
        //  Console.WriteLine($"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}");


        var syncIOFeature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpBodyControlFeature>();
        if (syncIOFeature != null)
        {
            syncIOFeature.AllowSynchronousIO = true;
        }
        var table = context.Request.RouteValues["table"]?.ToString();

        if (string.IsNullOrEmpty(table)) { return; }
        if (!manager.Sessions.Any(x => x.Table == table))
        {
            var tempSession = new Session()
            {
                Table = table,
                Logger = new NetLogger.Logs.Logger<BsonDocument>(@"D:\Test\Log_Server", table),
            };
            manager.Sessions.Add(tempSession);
        }

        try
        {
            var session = manager.Sessions.First(x => x.Table == table);
            using (var sr = new StreamReader(context.Request.Body))
            {
                var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
                session.Logger.Write(doc);
                session.LastWriteTime = DateTime.Now;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    });

    #endregion

    app.Run();
}
