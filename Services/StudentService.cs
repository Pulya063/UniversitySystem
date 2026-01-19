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
    /// Service for managing student operations: login, CRUD, and viewing details.
    /// </summary>
    public class StudentService
    {
        private readonly UniversityContext _kontekst;
        private readonly IRepository<Student> _studentRepo;

        public StudentService(UniversityContext kontekst, IRepository<Student> studentRepo)
        {
            _kontekst = kontekst;
            _studentRepo = studentRepo;
        }

        /// <summary>
        /// Login student by index number and display limited menu.
        /// </summary>
        public void StudentLogin()
        {
            Console.Clear();
            Console.Write("Podaj numer indeksu: ");
            var nrIndeksu = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nrIndeksu))
            {
                Console.WriteLine("Numer indeksu nie może być pusty.");
                Console.ReadKey();
                return;
            }

            var student = _kontekst.Studenci
                .Include(s => s.Oceny).ThenInclude(o => o.Zajecia).ThenInclude(z => z.Przedmiot)
                .FirstOrDefault(s => s.NrIndeksu == nrIndeksu);
            if (student == null)
            {
                Console.WriteLine("Student nie znaleziony.");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Zalogowano jako: {student.Imie} {student.Nazwisko} ({student.NrIndeksu})");
                Console.WriteLine("1) Pokaż moje oceny");
                Console.WriteLine("2) Pokaż mój Plan Zajęć");
                Console.WriteLine("0) Wyloguj");
                Console.Write("Wybór: ");
                var wybor = Console.ReadLine()?.Trim();
                switch (wybor)
                {
                    case "1": ShowMyGrades(student); break;
                    case "2": ShowStudentSchedule(student); break;
                    case "0": return;
                    default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
                }
            }
        }

        private void ShowMyGrades(Student student)
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

        private void ShowStudentSchedule(Student student)
        {
            Console.Clear();
            Console.WriteLine("=== MÓJ PLAN ZAJĘĆ ===");
            var plan = _kontekst.PlanZajec
                .Include(p => p.Przedmiot)
                .Include(p => p.Pracownik)
                .Include(p => p.Sala)
                .Where(p => p.GrupaID == student.GrupaID).ToList();
            if (!plan.Any())
            {
                Console.WriteLine("Brak zajęć dla twojej grupy.");
                Console.ReadKey();
                return;
            }
            foreach (var z in plan)
            {
                string[] dni = { "", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
                var dzien = (z.DzienTygodnia >= 1 && z.DzienTygodnia <= 7) ? dni[z.DzienTygodnia] : $"Dzien {z.DzienTygodnia}";
                Console.WriteLine($"- {dzien} {z.GodzinaRozpoczecia:hh\\:mm} | {z.Przedmiot?.Nazwa} | Sala: {z.Sala?.NumerSali} | Prowadzący: {z.Pracownik?.Imie} {z.Pracownik?.Nazwisko}");
            }
            Console.WriteLine("Naciśnij dowolny klawisz..."); 
            Console.ReadKey();
        }

        /// <summary>
        /// Admin/Teacher menu for managing students.
        /// </summary>
        public void CrudStudents(bool isAdminOrTeacher)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ZARZĄDZANIE STUDENTAMI ===");
                Console.WriteLine("1) Dodaj studenta");
                Console.WriteLine("2) Edytuj studenta");
                Console.WriteLine("3) Usuń studenta");
                Console.WriteLine("4) Lista studentów");
                Console.WriteLine("0) Powrót");
                Console.Write("Wybór: ");
                var wybor = Console.ReadLine()?.Trim();
                switch (wybor)
                {
                    case "1": AddStudent(); break;
                    case "2": EditStudent(); break;
                    case "3": DeleteStudent(); break;
                    case "4": ShowAllStudents(); break;
                    case "0": return;
                    default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
                }
            }
        }

        private void AddStudent()
        {
            Console.Clear();
            Console.WriteLine("=== DODAJ STUDENTA ===");
            Console.Write("Imię: ");
            var imie = Console.ReadLine()?.Trim();
            Console.Write("Nazwisko: ");
            var nazwisko = Console.ReadLine()?.Trim();
            Console.Write("PESEL: ");
            var pesel = Console.ReadLine()?.Trim();
            Console.Write("Numer indeksu (opcjonalnie): ");
            var nrIndeksu = Console.ReadLine()?.Trim();

            var grupy = _kontekst.GrupyDziekanskie.ToList();
            if (!grupy.Any())
            {
                Console.WriteLine("Brak grup. Dodaj grupy przed dodaniem studenta.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz grupę (numer):");
            for (int i = 0; i < grupy.Count; i++)
                Console.WriteLine($"{i + 1}) {grupy[i].KodGrupy}");

            if (!int.TryParse(Console.ReadLine(), out int gsel) || gsel < 1 || gsel > grupy.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var grupa = grupy[gsel - 1];
            var status = _kontekst.StatusyStudentow.FirstOrDefault() ?? new StatusStudenta { Nazwa = "Aktywny" };
            if (status.StatusStudentaID == 0)
            {
                _kontekst.StatusyStudentow.Add(status);
                _kontekst.SaveChanges();
            }

            var student = new Student
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Pesel = pesel,
                NrIndeksu = string.IsNullOrWhiteSpace(nrIndeksu) ? new Random().Next(10000, 99999).ToString() : nrIndeksu,
                GrupaID = grupa.GrupaID,
                StatusStudentaID = status.StatusStudentaID
            };

            _studentRepo.Dodaj(student);
            Console.WriteLine("Dodano studenta.");
            Console.ReadKey();
        }

        private void EditStudent()
        {
            var studenci = _kontekst.Studenci.ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("Brak studentów.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < studenci.Count; i++)
                Console.WriteLine($"{i + 1}) {studenci[i].NrIndeksu} | {studenci[i].Imie} {studenci[i].Nazwisko}");

            Console.Write("Wybierz numer studenta do edycji: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > studenci.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var student = studenci[sel - 1];
            Console.Write($"Imię ({student.Imie}): ");
            var imie = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(imie))
                student.Imie = imie.Trim();

            Console.Write($"Nazwisko ({student.Nazwisko}): ");
            var nazw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nazw))
                student.Nazwisko = nazw.Trim();

            _studentRepo.Aktualizuj(student);
            Console.WriteLine("Zapisano.");
            Console.ReadKey();
        }

        private void DeleteStudent()
        {
            var studenci = _kontekst.Studenci.ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("Brak studentów.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < studenci.Count; i++)
                Console.WriteLine($"{i + 1}) {studenci[i].NrIndeksu} | {studenci[i].Imie} {studenci[i].Nazwisko}");

            Console.Write("Wybierz numer studenta do usunięcia: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > studenci.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var student = studenci[sel - 1];
            _studentRepo.Usun(student.StudentID);
            Console.WriteLine("Usunięto (jeśli istniał).");
            Console.ReadKey();
        }

        private void ShowAllStudents()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA STUDENTÓW ===");
            var studenci = _kontekst.Studenci.Include(s => s.Grupa).ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("Brak studentów.");
                Console.ReadKey();
                return;
            }

            int i = 1;
            foreach (var s in studenci)
                Console.WriteLine($"{i++}. {s.NrIndeksu} | {s.Imie} {s.Nazwisko} | Grupa: {s.Grupa?.KodGrupy}");

            Console.WriteLine("Wybierz numer, aby zobaczyć szczegóły (0 powrót): ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 0 || sel > studenci.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            if (sel == 0) return;

            var student = studenci[sel - 1];
            Console.Clear();
            Console.WriteLine($"Szczegóły: {student.Imie} {student.Nazwisko} ({student.NrIndeksu})");
            Console.WriteLine($"PESEL: {student.Pesel}");
            Console.WriteLine($"Grupa: {student.Grupa?.KodGrupy}");
            Console.WriteLine("Oceny:");

            var oceny = _kontekst.Oceny
                .Include(o => o.Zajecia).ThenInclude(z => z.Przedmiot)
                .Where(o => o.StudentID == student.StudentID).ToList();

            foreach (var o in oceny)
                Console.WriteLine($"- {o.Wartosc} ({o.TypOceny}) z {o.Zajecia?.Przedmiot?.Nazwa} wystawiona: {o.DataWystawienia:d}");

            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}
