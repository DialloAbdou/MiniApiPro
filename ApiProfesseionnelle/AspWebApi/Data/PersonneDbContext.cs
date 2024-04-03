using AspWebApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AspWebApi.Data
{
    public class PersonneDbContext : DbContext
    {
        public DbSet<Personne> Personnes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personne>(c =>
            {
                c.ToTable("Personnes");
                c.Property(p => p.Nom).HasMaxLength(250);
                c.Property(p => p.Prenom).HasMaxLength(250);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=api.db");
        }
    }
}
