using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UniversitySystem.Models;

namespace UniversitySystem.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options) { }

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
    }
}