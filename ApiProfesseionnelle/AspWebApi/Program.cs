using AspWebApi;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using FluentValidation;
using AspWebApi.validations;
using AspWebApi.services;

var builder = WebApplication.CreateBuilder();

//======Configuration Logger Serilog=====

builder.Logging.ClearProviders();
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt",rollingInterval:RollingInterval.Day);

var logger = loggerConfiguration.CreateLogger();
builder.Services.AddSerilog(logger);

//---- Configuration Validation ------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

 app.MapGet("/bonjour", (ILogger<Program> logger) =>
 {
     logger.LogInformation("Bonjour à vous");
     return Results.Ok("Bonjour le Monde !");

 });

app.MapGet("/bonjour/{nom}", (string nom, ILogger<Program> logger) =>
{
    logger.LogInformation("Bonjour à vous {nom}", nom);
    return Results.Ok($"Bonjour {nom}!");

});

//======Validation=====================================
app.MapPost("/personne", ([FromBody]Personne personne, [FromServices] IValidator<Personne> validator ) =>
{
    var resultat = validator.Validate(personne);
    if (!resultat.IsValid) return Results.BadRequest(resultat.Errors.Select(e => new
    {
        e.ErrorMessage,
        e.PropertyName
    }));
    return Results.Ok(personne);
});
app.Run();