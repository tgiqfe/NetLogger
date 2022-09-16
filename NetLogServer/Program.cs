using NetLogServer;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dlogManager = new DynamicLog();

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
    dlogManager.Write(table, context.Request.Body);
});

#endregion

app.Run();
