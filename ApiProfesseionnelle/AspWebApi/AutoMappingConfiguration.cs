using AspWebApi.Data.Models;
using AspWebApi.Dto;
using AutoMapper;

namespace AspWebApi
{
    public class AutoMappingConfiguration : Profile
    {
        public AutoMappingConfiguration()
        {
            CreateMap<Personne, PersonneOutput>()
             .ConstructUsing(p => new PersonneOutput(
                  p.Id,
                  $"{p.Nom}, {p.Prenom}",
                  p.DateDeNaissance== DateTime.MinValue? null:p.DateDeNaissance,
                  p.Adresse!
                  
                 ));
            CreateMap<PersonneInput, Personne>();
                //.ForMember(p => p.Id, op => op.Ignore());
            
        }
    }
}
