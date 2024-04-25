using AspWebApi.Data.Models;
using AspWebApi.Dto;

namespace AspWebApi.services
{
    public interface IPersonneService
    {
        Task<List<PersonneOutPut>> GetAllPersonnesAsync();
        Task<PersonneOutPut> GetPersonByIdAsync(int id);
        Task<PersonneOutPut> AddPersonne(PersonneInput personne);
        Task<bool> UpdatePersonne(int id, PersonneInput personne);
        Task<bool> DeletePersonneAsync(int id);
    }
}
