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
    /// Service for managing subjects (Przedmioty): CRUD operations for subject management.
    /// </summary>
    public class SubjectService
    {
        private readonly UniversityContext _kontekst;
        private readonly IRepository<Przedmiot> _przedmiotRepo;

        public SubjectService(UniversityContext kontekst, IRepository<Przedmiot> przedmiotRepo)
        {
            _kontekst = kontekst;
            _przedmiotRepo = przedmiotRepo;
        }

        /// <summary>
        /// Admin menu for managing subjects.
        /// </summary>
        public void CrudSubjects()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ZARZĄDZANIE PRZEDMIOTAMI ===");
                Console.WriteLine("1) Dodaj przedmiot");
                Console.WriteLine("2) Edytuj przedmiot");
                Console.WriteLine("3) Usuń przedmiot");
                Console.WriteLine("4) Lista przedmiotów");
                Console.WriteLine("0) Powrót");
                Console.Write("Wybór: ");
                var wybor = Console.ReadLine()?.Trim();
                switch (wybor)
                {
                    case "1": AddSubject(); break;
                    case "2": EditSubject(); break;
                    case "3": DeleteSubject(); break;
                    case "4": ShowAllSubjects(); break;
                    case "0": return;
                    default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
                }
            }
        }

        private void AddSubject()
        {
            Console.Clear();
            Console.WriteLine("=== DODAJ PRZEDMIOT ===");
            
            var kierunki = _kontekst.Kierunki.ToList();
            if (!kierunki.Any())
            {
                Console.WriteLine("Brak kierunków. Dodaj kierunki przed dodaniem przedmiotu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz kierunek (numer):");
            for (int i = 0; i < kierunki.Count; i++)
                Console.WriteLine($"{i + 1}) {kierunki[i].Nazwa}");

            if (!int.TryParse(Console.ReadLine(), out int ksel) || ksel < 1 || ksel > kierunki.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór kierunku.");
                Console.ReadKey();
                return;
            }

            var kierunek = kierunki[ksel - 1];

            Console.Write("Nazwa przedmiotu: ");
            var nazwa = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(nazwa))
            {
                Console.WriteLine("Nazwa nie może być pusta.");
                Console.ReadKey();
                return;
            }

            Console.Write("ECTS: ");
            if (!int.TryParse(Console.ReadLine(), out int ects) || ects <= 0)
            {
                Console.WriteLine("Nieprawidłowe ECTS.");
                Console.ReadKey();
                return;
            }

            Console.Write("Semestr: ");
            if (!int.TryParse(Console.ReadLine(), out int semestr) || semestr <= 0)
            {
                Console.WriteLine("Nieprawidłowy semestr.");
                Console.ReadKey();
                return;
            }

            var przedmiot = new Przedmiot
            {
                Nazwa = nazwa,
                ECTS = ects,
                Semestr = semestr,
                KierunekID = kierunek.KierunekID
            };

            _przedmiotRepo.Dodaj(przedmiot);
            Console.WriteLine("Dodano przedmiot.");
            Console.ReadKey();
        }

        private void EditSubject()
        {
            var przedmioty = _kontekst.Przedmioty.Include(p => p.Kierunek).ToList();
            if (!przedmioty.Any())
            {
                Console.WriteLine("Brak przedmiotów.");
                Console.ReadKey();
                return;
            }

            ShowPaginatedSubjects(przedmioty);

            Console.Write("Wybierz numer przedmiotu do edycji: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > przedmioty.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var przedmiot = przedmioty[sel - 1];
            Console.Write($"Nazwa ({przedmiot.Nazwa}): ");
            var nazwa = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nazwa))
                przedmiot.Nazwa = nazwa.Trim();

            Console.Write($"ECTS ({przedmiot.ECTS}): ");
            var ectsStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ectsStr) && int.TryParse(ectsStr, out int ects))
                przedmiot.ECTS = ects;

            Console.Write($"Semestr ({przedmiot.Semestr}): ");
            var semesterStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(semesterStr) && int.TryParse(semesterStr, out int semester))
                przedmiot.Semestr = semester;

            _przedmiotRepo.Aktualizuj(przedmiot);
            Console.WriteLine("Zapisano.");
            Console.ReadKey();
        }

        private void DeleteSubject()
        {
            var przedmioty = _kontekst.Przedmioty.Include(p => p.Kierunek).ToList();
            if (!przedmioty.Any())
            {
                Console.WriteLine("Brak przedmiotów.");
                Console.ReadKey();
                return;
            }

            ShowPaginatedSubjects(przedmioty);

            Console.Write("Wybierz numer przedmiotu do usunięcia: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > przedmioty.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var przedmiot = przedmioty[sel - 1];
            _przedmiotRepo.Usun(przedmiot.PrzedmiotID);
            Console.WriteLine("Usunięto (jeśli istniał).");
            Console.ReadKey();
        }

        private void ShowAllSubjects()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA PRZEDMIOTÓW ===");
            var przedmioty = _kontekst.Przedmioty.Include(p => p.Kierunek).ToList();
            if (!przedmioty.Any())
            {
                Console.WriteLine("Brak przedmiotów.");
                Console.ReadKey();
                return;
            }

            ShowPaginatedSubjects(przedmioty);

            Console.WriteLine("Wybierz numer, aby zobaczyć szczegóły (0 powrót): ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 0 || sel > przedmioty.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            if (sel == 0) return;

            var przedmiot = przedmioty[sel - 1];
            Console.Clear();
            Console.WriteLine($"Szczegóły: {przedmiot.Nazwa}");
            Console.WriteLine($"Kierunek: {przedmiot.Kierunek?.Nazwa}");
            Console.WriteLine($"ECTS: {przedmiot.ECTS} | Semestr: {przedmiot.Semestr}");
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        private void ShowPaginatedSubjects(List<Przedmiot> przedmioty)
        {
            int i = 1;
            foreach (var p in przedmioty)
                Console.WriteLine($"{i++}. {p.Nazwa} (ECTS: {p.ECTS}, Semestr: {p.Semestr}) | Kierunek: {p.Kierunek?.Nazwa}");
        }
    }
}
