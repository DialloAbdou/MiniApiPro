namespace AspWebApi.Dto
{
    public class PersonneInput
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime ? DateNaissance { get; set; }
        public string ? Adresse { get; set; }

    }
}
