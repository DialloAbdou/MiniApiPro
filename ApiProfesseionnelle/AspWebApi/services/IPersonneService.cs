using AspWebApi.Data.Models;

namespace AspWebApi.services
{
    public interface IPersonneService
    {
        Task<List<Personne>> GetAllPersonnesAsync();
        Task<Personne> GetPersonByIdAsync(int id);
        Task<Personne>AddPersonne(Personne personne);
        Task<bool> UpdatePersonne(int id, Personne personne);
        Task<bool> DeletePersonneAsync(int id);
    }
}
