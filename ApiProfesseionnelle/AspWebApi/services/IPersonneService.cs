using AspWebApi.Data.Models;
using AspWebApi.Dto;

namespace AspWebApi.services
{
    public interface IPersonneService
    {
        Task<List<PersonneOutput>> GetAllPersonnesAsync();
        Task<PersonneOutput> GetPersonByIdAsync(int id);
        Task<PersonneOutput> AddPersonne(PersonneInput personne);
        Task<bool> UpdatePersonne(int id, PersonneInput personne);
        Task<bool> DeletePersonneAsync(int id);
    }
}
