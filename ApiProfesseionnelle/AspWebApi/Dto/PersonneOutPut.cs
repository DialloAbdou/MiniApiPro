namespace AspWebApi.Dto
{
    public record PersonneOutput
    (
     int id,
     string FullName,
     DateTime ? DateDeNaissance,
     string? Adresse
    );
    
}
