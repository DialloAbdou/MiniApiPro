using AspWebApi.Data;
using AspWebApi.Data.Models;
using AspWebApi.Dto;
using Microsoft.EntityFrameworkCore;

namespace AspWebApi.services
{
    public class ServicePersonne : IPersonneService
    {
        private readonly PersonneDbContext _context;
        public ServicePersonne(PersonneDbContext context)
        {
            _context = context;
        }

        private PersonneOutPut GetOutPutPersonne(Personne personne)
        {
            return new PersonneOutPut
            (
              Id: personne.Id,
              FullName: $"{personne.Nom}, {personne.Prenom}",
              DateNaissance: personne.DateDeNaissance

            );
        }
        public async Task<List<PersonneOutPut>> GetAllPersonnesAsync()
        {
            var peoples = (await _context.Personnes.ToListAsync()).ConvertAll(GetOutPutPersonne);
            return peoples;

        }

        public async Task<PersonneOutPut> GetPersonByIdAsync(int id)
        {
            var person = await _context.Personnes.FirstOrDefaultAsync(p => p.Id == id);
            if (person is not null)
            {
                var personOut = GetOutPutPersonne(person);
                return personOut;
            }
            return null!;
        }

        public async Task<PersonneOutPut> AddPersonne(PersonneInput personnein)
        {
            var personne = new Personne()
            {
                Nom = personnein.Nom,
                Prenom = personnein.Prenom,
                DateDeNaissance = personnein.DateNaissance.GetValueOrDefault()
            };
            await _context.AddAsync(personne);
            await _context.SaveChangesAsync();
           var _personneOut = GetOutPutPersonne(personne);
            return _personneOut;
        }

        public async Task<bool> DeletePersonneAsync(int id)
        {
            var result = await _context.Personnes.Where(p => p.Id == id).ExecuteDeleteAsync();
            return result > 0;

        }
        public async Task<bool> UpdatePersonne(int id, PersonneInput personne)
        {
            var result = await _context.Personnes.Where(p => p.Id == id)
                  .ExecuteUpdateAsync(pe => pe
                  .SetProperty(pe => pe.Nom, personne.Nom)
                  .SetProperty(pe => pe.Prenom, personne.Prenom)
                  .SetProperty(pe=>pe.DateDeNaissance,personne.DateNaissance)
                  );
            return result > 0;

          
        }



    }
}
