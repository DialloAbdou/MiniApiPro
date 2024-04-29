namespace AspWebApi.Dto
{
    public record PersonneOutput
    (
     int Id,
     string FullName,
     DateTime ? DateDeNaissance
    );
    
}
