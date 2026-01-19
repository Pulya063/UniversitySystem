using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;
using UniversitySystem.Models;

namespace UniversitySystem.Services
{
    /// <summary>
    /// Service for managing grades (Oceny): issuing and viewing grades.
    /// </summary>
    public class GradeService
    {
        private readonly UniversityContext _kontekst;
        private readonly IRepository<Ocena> _ocenaRepo;

        public GradeService(UniversityContext kontekst, IRepository<Ocena> ocenaRepo)
        {
            _kontekst = kontekst;
            _ocenaRepo = ocenaRepo;
        }

        /// <summary>
        /// Teacher menu for issuing grades interactively.
        /// </summary>
        public void IssueGradeInteractively()
        {
            Console.Clear();
            Console.WriteLine("=== WYSTAWIANIE OCEN ===");

            var studenci = _kontekst.Studenci.ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("Brak studentów.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz studenta (numer):");
            int i = 1;
            foreach (var s in studenci)
                Console.WriteLine($"{i++}. {s.NrIndeksu} | {s.Imie} {s.Nazwisko}");

            if (!int.TryParse(Console.ReadLine(), out int ssel) || ssel < 1 || ssel > studenci.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var student = studenci[ssel - 1];

            var zajecia = _kontekst.PlanZajec
                .Include(z => z.Przedmiot)
                .Where(z => z.GrupaID == student.GrupaID)
                .ToList();

            if (!zajecia.Any())
            {
                Console.WriteLine("Brak zajęć dla grupy tego studenta.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz zajęcia (numer):");
            i = 1;
            foreach (var z in zajecia)
                Console.WriteLine($"{i++}. {z.Przedmiot?.Nazwa}");

            if (!int.TryParse(Console.ReadLine(), out int zsel) || zsel < 1 || zsel > zajecia.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var zajecie = zajecia[zsel - 1];

            Console.WriteLine("Wartość oceny (1-6):");
            if (!double.TryParse(Console.ReadLine(), out double wartosc) || wartosc < 1 || wartosc > 6)
            {
                Console.WriteLine("Nieprawidłowa wartość oceny.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Typ oceny (np. Zaliczenie, Egzamin, Kolokwium): ");
            var typ = Console.ReadLine()?.Trim() ?? "Ocena";

            var ocena = new Ocena
            {
                StudentID = student.StudentID,
                PlanZajecID = zajecie.PlanZajecID,
                Wartosc = wartosc,
                TypOceny = typ,
                DataWystawienia = DateTime.UtcNow
            };

            _ocenaRepo.Dodaj(ocena);
            Console.WriteLine("Ocena wystawiona.");
            Console.ReadKey();
        }

        /// <summary>
        /// Display all grades for a given student (used by student login menu).
        /// </summary>
        public void ShowStudentGrades(Student student)
        {
            Console.Clear();
            Console.WriteLine("=== MOJE OCENY ===");
            var oceny = _kontekst.Oceny
                .Include(o => o.Zajecia).ThenInclude(z => z.Przedmiot)
                .Where(o => o.StudentID == student.StudentID).ToList();
            if (!oceny.Any())
                Console.WriteLine("Brak ocen.");
            else
                foreach (var o in oceny)
                    Console.WriteLine($"- {o.Wartosc} | {o.TypOceny} | {o.Zajecia?.Przedmiot?.Nazwa} | {o.DataWystawienia:d}");
            Console.WriteLine("Naciśnij dowolny klawisz..."); 
            Console.ReadKey();
        }
    }
}
