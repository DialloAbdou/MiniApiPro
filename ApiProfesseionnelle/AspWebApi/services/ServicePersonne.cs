namespace AspWebApi.services
{
    public class ServicePersonne
    {
        private List<Personne> personneList;
        public ServicePersonne()
        {
            personneList = new List<Personne>
            {
                new Personne {  Nom = "DIALLO",Prenom = "Abdou"},
                new Personne {  Nom = "Camara",Prenom = "Moussa"},
                new Personne {  Nom = "SYlla",Prenom = "Mamadi"},
                new Personne { Nom = "Doumbouya",Prenom = "Mamadi"},
                new Personne { Nom = "Bah",Prenom = "Oury"}

            };
        }

        public List<Personne> GetPersonnes()
        {
            return personneList;
        }
    }
}
