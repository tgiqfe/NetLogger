using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var options = new System.Text.Json.JsonSerializerOptions()
{
    //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    IgnoreReadOnlyProperties = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    //WriteIndented = true,
    //Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
};


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



});

#endregion

app.Run();
