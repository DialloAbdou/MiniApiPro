using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder();

// Consifiguration  de Serilog
 builder.Logging.ClearProviders(); // permet de supprimer tout les fichier logs par defaut installer par .net core
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt",rollingInterval:RollingInterval.Day);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);
var app = builder.Build();
app.MapGet("/hello", ( [FromServices]ILogger<Program> logger) =>
{
    logger.LogInformation(" log depuis l'endpoint Hello");
    return Results.Ok("Hello World !");
});

app.MapGet("/hello/{nom}", ( [FromRoute]string nom, ILogger<Program> logger)=>
{
    logger.LogInformation("j'ai dis hello à {nom}", nom);// cette chaine ne sera créer une seule fois en memoire
    Results.Ok(nom);
});

app.Run();