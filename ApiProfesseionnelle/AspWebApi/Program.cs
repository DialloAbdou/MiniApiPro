using Microsoft.AspNetCore.Mvc;
using Serilog;
using FluentValidation;
using AspWebApi.validations;
using AspWebApi.services;
using AspWebApi.Data.Models;
using AspWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using AspWebApi;

var builder = WebApplication.CreateBuilder(args);

//------------Configuration----------------

//---------Utilisation des logs-------------------------
builder.Logging.ClearProviders();
var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);

var logger = loggerConfiguration.CreateLogger();
builder.Services.AddSerilog(logger);

//-----Configuration de la base de  données ---------------------
builder.Services.AddDbContext<PersonneDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
//---- Configuration Validation ------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//----------Configuration de la cache mémoire ------------
//builder.Services.AddMemoryCache();

//builder.Services.AddOutputCache(op =>
//{
//    //ajoute moi la prise en charge du cache de la sotie voici la strategie dont j'ai choisie 

//    op.AddBasePolicy(b => b.Expire(TimeSpan.FromMinutes(1)));
//    op.AddPolicy("Expire2mn", b => b.Expire(TimeSpan.FromMinutes(2)));
//    op.AddPolicy("Expire10secds", b => b.Expire(TimeSpan.FromSeconds(10)).Tag("personnes"));
//    op.AddPolicy("ById", b => b.SetVaryByRouteValue("ById"));
//});


// utlisation de cache partagé ou distribuée
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = "localhost:6379";
});
var app = builder.Build();
//app.UseOutputCache();
//--------Création Base de donnée -----------------------
app.Services
    .CreateScope().ServiceProvider.
    GetRequiredService<PersonneDbContext>().Database
    .EnsureCreated();
//---------lecture des Données--------------------------
app.MapGet("/personnes", async ([FromServices] PersonneDbContext context) =>
{
    var peoples = await context.Personnes.ToListAsync();
    return Results.Ok(peoples);
}).CacheOutput();

app.MapGet("/personnes/{id:int}", async (
    [FromRoute] int id,
    [FromServices] PersonneDbContext context,
     /*[FromServices] IMemoryCache cache*/
     [FromServices] IDistributedCache cache) =>
   {
       #region CacheMemoireDistribuee
       var person = await cache.GetAsync<Personne>($"personne_{id}");
       if (person is null)
       {
           person = await context.Personnes.FirstOrDefaultAsync(p => p.Id == id); // on la recupere depuis la base de donnée
           if (person is null) return Results.NotFound("cette personne n'existe");
           await cache.SetAsync($"personne_{id}", person);
           return Results.Ok(person);
       }
       #endregion
       return Results.Ok(person);
   }).CacheOutput();


//------------------Update-----------------------------------------
app.MapPut("/personnes/{id:int}", async (
    [FromRoute] int id,
    [FromServices] PersonneDbContext context,
    [FromBody] Personne personne,
      /* [FromServices] IMemoryCache cache*/
      [FromServices] IDistributedCache cache) =>
{
    //-----------Version EF7-----------
    var result = await context.Personnes.Where(p => p.Id == id)
    .ExecuteUpdateAsync(pe => pe
    .SetProperty(pe => pe.Nom, personne.Nom)
    .SetProperty(pe => pe.Prenom, personne.Prenom));
    if (result > 0)
    {
        await cache.RemoveAsync($"personne_{id}");
        return Results.NoContent();
    }
    if (result > 0) return Results.Content("Validé");
    return Results.NotFound();

    #region Ancien Version Update
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
    #endregion
});
//----------------------Delete-------------------------------------------
app.MapDelete("/personnes/{id:int}", async (
    [FromRoute] int id,
    [FromServices] PersonneDbContext context) =>
{
    //---Autre Version Avec EF7----------------------------
    var result = await context.Personnes.Where(p => p.Id == id).ExecuteDeleteAsync();
    if (result > 0) return Results.NoContent();
    return Results.NotFound("Cet Objet n'existe pas");

    #region Ancien VersionDelete
    //Personne? people = context.Personnes.FirstOrDefault(p => p.Id == id);
    //if (people is null) return Results.NotFound("cet objet n'existe pas");
    //context.Personnes.Remove(people);
    //context.SaveChanges();
    //return Results.NoContent();
    #endregion
});
//---------------- CreateValidation------------------------------------

app.MapPost("/personne", async (
    [FromBody] Personne personne,
    [FromServices] IValidator<Personne> validator,
    [FromServices] PersonneDbContext context,
    CancellationToken token) =>
{
    var resultat = validator.Validate(personne);

    if (!resultat.IsValid) return Results.BadRequest(resultat.Errors.Select(e => new
    {
        e.ErrorMessage,
        e.PropertyName
    }));
    context.Add(personne);
    await context.SaveChangesAsync(token);
    return Results.Ok(personne);
});
app.Run();