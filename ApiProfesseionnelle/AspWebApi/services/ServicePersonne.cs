using AspWebApi.Data;
using AspWebApi.Data.Models;
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

        public async Task<List<Personne>> GetAllPersonnesAsync()
        {
            var peoples = await _context.Personnes.ToListAsync();
            return peoples;
        }

        public async Task<Personne> GetPersonByIdAsync(int id)
        {
            var person = await _context.Personnes.FirstOrDefaultAsync(p => p.Id == id);
            return person!;
        }

        public async Task<Personne> AddPersonne(Personne personne)
        {
            await _context.AddAsync(personne);
            await _context.SaveChangesAsync();
            return personne;
        }

        public async Task<bool> DeletePersonneAsync(int id)
        {
            var result = await _context.Personnes.Where(p => p.Id == id).ExecuteDeleteAsync();

            if (result > 0)
            {
                return true;
            }
            return false;

        }
        public async Task<bool> UpdatePersonne(int id, Personne personne)
        {
            var result = await _context.Personnes.Where(p => p.Id == id)
                  .ExecuteUpdateAsync(pe => pe
                  .SetProperty(pe => pe.Nom, personne.Nom)
                  .SetProperty(pe => pe.Prenom, personne.Prenom));

            if (result > 0) return true;
                return false;
        }



    }
}
