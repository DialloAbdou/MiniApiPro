using Microsoft.AspNetCore.Mvc;
using Serilog;
using FluentValidation;
using AspWebApi.validations;
using AspWebApi.services;
using AspWebApi.Data.Models;
using AspWebApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

//------------Configuration Logger Serilog----------------

builder.Logging.ClearProviders();
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);

var logger = loggerConfiguration.CreateLogger();
builder.Services.AddSerilog(logger);

builder.Services.AddDbContext<PersonneDbContext>();
//---- Configuration Validation ------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
//--------Création Base de donnée -----------------------
app.Services
    .CreateScope().ServiceProvider.
    GetRequiredService<PersonneDbContext>().Database
    .EnsureCreated();
//---------lecture des Données--------------------------
app.MapGet("/personnes", ([FromServices] PersonneDbContext context) =>
{
    var peoples = context.Personnes.ToList();
    return Results.Ok(peoples);
});

app.MapGet("/personnes/{id:int}", ([FromRoute] int id, [FromServices] PersonneDbContext context) =>
{
    var peoples = context.Personnes.FirstOrDefault(p => p.Id == id);
    if (peoples is null) return Results.NotFound("cette personne n'existe");
    return Results.Ok(peoples);
});

//------------------Update-----------------------------------------
app.MapPut("/personnes/{id:int}", (
    [FromRoute] int id, 
    [FromServices] PersonneDbContext context,
    [FromBody] Personne personne) =>
{
    //-----------Version EF7-----------
    var result = context.Personnes.Where(p => p.Id == id)
    .ExecuteUpdate(pe => pe
    .SetProperty(pe => pe.Nom, personne.Nom)
    .SetProperty(pe => pe.Prenom, personne.Prenom));
   // var peoples = context.Personnes.FirstOrDefault(p => p.Id == id);
   //if( peoples is not null)
   //{

   //     peoples.Nom = personne.Nom;
   //     peoples.Prenom = personne.Prenom;
   //     context.Personnes.Update(peoples);
   //     context.SaveChanges();

   //     return Results.NoContent();
   //}
   //return Results.NotFound("Cet Objet n'existe pas");
});
//----------------------Delete-------------------------------------------
app.MapDelete("/personnes/{id:int}", ([FromRoute] int id, [FromServices] PersonneDbContext context) =>
{
    //---Autre Version Avec EF7----------------------------
    var result = context.Personnes.Where(p => p.Id == id).ExecuteDelete();
    if (result > 0) return Results.NoContent();
    return Results.NotFound("Cet Objet n'existe pas");
    //Personne? people = context.Personnes.FirstOrDefault(p => p.Id == id);

    //if (people is null) return Results.NotFound("cet objet n'existe pas");

    //context.Personnes.Remove(people);
    //context.SaveChanges();
    //return Results.NoContent();
});
//---------------- CreateValidation------------------------------------

app.MapPost("/personne", (
    [FromBody] Personne personne,
    [FromServices] IValidator<Personne> validator,
    [FromServices] PersonneDbContext context) =>
{
    var resultat = validator.Validate(personne);
    if (!resultat.IsValid) return Results.BadRequest(resultat.Errors.Select(e => new
    {
        e.ErrorMessage,
        e.PropertyName
    }));
    context.Add(personne);
    context.SaveChanges();
    return Results.Ok(personne);
});
app.Run();