using LiteDB;
using NetLogServer;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dlogManager = new DynamicLog();


var sessions = new List<DynamicLogSession>();


#region Routing

app.MapGet("/", () => "");
app.MapPost("/", () => "");

app.MapPost("/api/logger/{table}", (HttpContext context) =>
{
    var syncIOFeature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpBodyControlFeature>();
    if (syncIOFeature != null)
    {
        syncIOFeature.AllowSynchronousIO = true;
    }
    var table = context.Request.RouteValues["table"]?.ToString();

    if (string.IsNullOrEmpty(table)) { return; }
    if (!sessions.Any(x => x.Table == table))
    {
        var tempSession = new DynamicLogSession()
        {
            Table = table,
            Logger = new LoggerBase<BsonDocument>(@"D:\Test\Loggggg", table, null),
        };
    }
    try
    {
        var session = sessions.First(x => x.Table == table);
        using (var sr = new StreamReader(context.Request.Body))
        {
            var doc = JsonSerializer.Deserialize(sr) as BsonDocument;
            session.Logger.Write(doc);
            session.LastWriteTime = DateTime.Now;
        }
    }
    catch { }


    //dlogManager.Write(table, context.Request.Body);
});

#endregion

app.Run();
