using AspWebApi.Data.Models;
using AspWebApi.Dto;
using FluentValidation;

namespace AspWebApi.validations
{
    public class PersonneValidator : AbstractValidator<PersonneInput>
    {
        public PersonneValidator()
        {
            RuleFor(p => p.Nom).NotEmpty();
            RuleFor(p => p.Prenom).NotEmpty();
            RuleFor(p=>p.DateNaissance).LessThanOrEqualTo(DateTime.Now);

        }

    }
}
