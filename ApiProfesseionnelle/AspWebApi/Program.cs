using AspWebApi;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using FluentValidation;
using AspWebApi.validations;

var builder = WebApplication.CreateBuilder();

// Consifiguration  de Serilog
builder.Logging.ClearProviders(); // permet de supprimer tout les fichier logs par defaut installer par .net core
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

// validation
builder.Services.AddValidatorsFromAssemblyContaining<PersonneValidator>();
var app = builder.Build();
app.MapGet("/hello", ([FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation(" log depuis l'endpoint Hello");
    return Results.Ok("Hello World !");
});

app.MapGet("/hello/{nom}", ([FromRoute] string nom, ILogger<Program> logger) =>
{
    logger.LogInformation("j'ai dis hello à {nom}", nom);// cette chaine ne sera créer une seule fois en memoire
    Results.Ok(nom);
});

app.MapPost("/personne", ([FromBody] Personne p,
    [FromServices] IValidator<Personne> validator) =>
{
    var result = validator.Validate(p);
    if (!result.IsValid)  return Results.BadRequest(result.Errors); 
    return Results.Ok(result);

});




app.Run();