using FluentValidation;

namespace AspWebApi.validations
{
    public class PersonneValidator : AbstractValidator<Personne>
    {
        public PersonneValidator()
        {
            RuleFor(p => p.Nom).NotEmpty();
            RuleFor(p => p.Prenom).NotEmpty();

        }

    }
}
