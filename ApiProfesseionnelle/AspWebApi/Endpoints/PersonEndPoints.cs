using AspWebApi.Dto;
using AspWebApi.services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AspWebApi.Endpoints
{
    public static class PersonEndPoints
    {
        public static RouteGroupBuilder MapPersonEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("", GetAll/*async ([FromServices] PersonneDbContext context,*/

               ).WithTags("PersonneManagement");
            /*.CacheOutput();*/


            group.MapGet("/{id:int}", GetById
                   
                     
                   ).WithTags("PersonneManagement");
            /*.CacheOutput();*/

            //------------------Update-----------------------------------------
            group.MapPut("/{id:int}",UpdatePersonne        
            ).Produces(204)
             .Produces(404)
             .Produces<PersonneOutput>(contentType: "application/json")
             .WithTags("PersonneManagement");

            //------------------ Delete -----------------------------------------

            group.MapDelete("/{id:int}", DeletePersonne
                
               ).WithTags("PersonneManagement");

            //---------------- CreateValidation------------------------------------

            group.MapPost("", AddPersonne
            ).WithTags("PersonneManagement");

            return group;
        }

        private static async Task<IResult> GetAll([FromServices] IPersonneService service)
        {
            var peoples = await service.GetAllPersonnesAsync();
            return Results.Ok(peoples);
        }

        private static async Task<IResult> GetById([FromRoute] int id,
                      //[FromServices] PersonneDbContext context,
                      [FromServices] IPersonneService service)
                        
        {
            #region CacheMemoireDistribuee
            //var person = await cache.GetAsync<Personne>($"personne_{id}");
            //if (person is null)
            //{
            //    person = await context.Personnes.FirstOrDefaultAsync(p => p.Id == id); // on la recupere depuis la base de donnée
            //    if (person is null) return Results.NotFound("cette personne n'existe");
            //    await cache.SetAsync($"personne_{id}", person);
            //    return Results.Ok(person);
            //}
            var person = await service.GetPersonByIdAsync(id);
            if (person is null) return Results.NotFound("cette personne n'existe");
            return Results.Ok(person);
            #endregion
        }

        private static async Task<IResult>UpdatePersonne([FromRoute] int id,
                //[FromServices] PersonneDbContext context,
                [FromServices] IPersonneService service,
                [FromBody] PersonneInput personne)
        {
            var result = await service.UpdatePersonne(id, personne);
            if (result) return Results.Content("Validé");
            return Results.NotFound();
        }

        private static async Task<IResult>DeletePersonne( [FromRoute]int id, [FromServices] IPersonneService service)
        {
            //---Autre Version Avec EF7----------------------------
            var result = await service.DeletePersonneAsync(id);
            if (result) return Results.NoContent();
            return Results.NotFound("Cet Objet n'existe pas");

            #region Ancien VersionDelete
            //Personne? people = context.Personnes.FirstOrDefault(p => p.Id == id);
            //if (people is null) return Results.NotFound("cet objet n'existe pas");
            //context.Personnes.Remove(people);
            //context.SaveChanges();
            //return Results.NoContent();
            #endregion
        }

        private static async Task<IResult>AddPersonne(
            [FromBody] PersonneInput personne,
                [FromServices] IValidator<PersonneInput> validator,
                //[FromServices] PersonneDbContext context,
                [FromServices] IPersonneService service)
        {
            var resultat = validator.Validate(personne);
            if (!resultat.IsValid) return Results.BadRequest(resultat.Errors.Select(e => new
            {
                e.ErrorMessage,
                e.PropertyName
            }));
            await service.AddPersonne(personne);
            return Results.Ok(personne);
        }

    }
}
