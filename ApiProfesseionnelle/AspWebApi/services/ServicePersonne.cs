using AspWebApi.Data;
using AspWebApi.Data.Models;
using AspWebApi.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AspWebApi.services
{
    public class ServicePersonne : IPersonneService
    {
        private readonly PersonneDbContext _context;
        private IMapper _mapper;
        public ServicePersonne(PersonneDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Obsolete
        private PersonneOutput GetOutPutPersonne(Personne personne)
        {
            return new PersonneOutput
            (
               Id: personne.Id,
               FullName: $"{personne.Nom}, {personne.Prenom}",
               DateDeNaissance: personne.DateDeNaissance == DateTime.MinValue ? null : personne.DateDeNaissance



            ); 
        }
        #endregion
        public async Task<List<PersonneOutput>> GetAllPersonnesAsync()
        {
            var peoples = (await _context.Personnes.ToListAsync()).ConvertAll(GetOutPutPersonne);
            return peoples;

        }

        public async Task<PersonneOutput> GetPersonByIdAsync(int id)
        {
            var person = await _context.Personnes.FirstOrDefaultAsync(p => p.Id == id);
            if (person is not null)
            {
                //var personOut = _mapper.Map<PersonneOutput>(person);
                var personOut = GetOutPutPersonne(person);
                return personOut;
            }
            return null!;
        }

        public async Task<PersonneOutput> AddPersonne(PersonneInput personneIn)
        {
            var personne = new Personne()
            {
                Nom = personneIn.Nom,
                Prenom = personneIn.Prenom,
                DateDeNaissance = personneIn.DateNaissance.GetValueOrDefault()
            };
            //var personne = _mapper.Map<Personne>(personneIn);
            await _context.AddAsync(personne);
            await _context.SaveChangesAsync();
            //var _personneOut = _mapper.Map<PersonneOutput>(personne);
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
