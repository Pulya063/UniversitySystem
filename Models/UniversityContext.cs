using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UniversitySystem.Models;

namespace UniversitySystem.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options) { }

        // Helper to configure DbContextOptions from connection string (keeps connection logic out of Program)
        public static void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            // Configure provider here (PostgreSQL via Npgsql)
            optionsBuilder.UseNpgsql(connectionString);
        }

        // Rejestracja tabel w bazie danych
        public DbSet<Wydzial> Wydzialy { get; set; }
        public DbSet<Kierunek> Kierunki { get; set; }
        public DbSet<GrupaDziekanska> GrupyDziekanskie { get; set; }
        public DbSet<Student> Studenci { get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Przedmiot> Przedmioty { get; set; }
        public DbSet<PlanZajec> PlanZajec { get; set; }
        public DbSet<Sala> Sale { get; set; }
        public DbSet<Ocena> Oceny { get; set; }
        public DbSet<StatusStudenta> StatusyStudentow { get; set; }
        public DbSet<Stanowisko> Stanowiska { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Normalize table and column names to lower-case to avoid Postgres quoted identifier
            // casing issues (e.g. "KierunekID" vs kierunekid). This keeps the DB identifiers
            // consistent when EF creates the schema via EnsureCreated.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // set table name to lower-case
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName)) entity.SetTableName(tableName.ToLowerInvariant());

                // set column names to lower-case
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToLowerInvariant());
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}