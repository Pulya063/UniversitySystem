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
    /// Service for managing schedules (PlanZajęć): CRUD operations for schedule entries.
    /// </summary>
    public class ScheduleService
    {
        private readonly UniversityContext _kontekst;
        private readonly IRepository<PlanZajec> _planRepo;

        public ScheduleService(UniversityContext kontekst, IRepository<PlanZajec> planRepo)
        {
            _kontekst = kontekst;
            _planRepo = planRepo;
        }

        /// <summary>
        /// Admin menu for managing schedule entries.
        /// </summary>
        public void CrudSchedule()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ZARZĄDZANIE PLANEM ZAJĘĆ ===");
                Console.WriteLine("1) Dodaj wpis planu");
                Console.WriteLine("2) Edytuj wpis planu");
                Console.WriteLine("3) Usuń wpis planu");
                Console.WriteLine("4) Lista wpisów planu");
                Console.WriteLine("0) Powrót");
                Console.Write("Wybór: ");
                var wybor = Console.ReadLine()?.Trim();
                switch (wybor)
                {
                    case "1": AddScheduleEntry(); break;
                    case "2": EditScheduleEntry(); break;
                    case "3": DeleteScheduleEntry(); break;
                    case "4": ShowAllScheduleEntries(); break;
                    case "0": return;
                    default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
                }
            }
        }

        private void AddScheduleEntry()
        {
            Console.Clear();
            Console.WriteLine("=== DODAJ WPIS PLANU ===");

            var grupy = _kontekst.GrupyDziekanskie.ToList();
            if (!grupy.Any())
            {
                Console.WriteLine("Brak grup.");
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

            var przedmioty = _kontekst.Przedmioty.ToList();
            if (!przedmioty.Any())
            {
                Console.WriteLine("Brak przedmiotów.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz przedmiot (numer):");
            for (int i = 0; i < przedmioty.Count; i++)
                Console.WriteLine($"{i + 1}) {przedmioty[i].Nazwa}");

            if (!int.TryParse(Console.ReadLine(), out int psel) || psel < 1 || psel > przedmioty.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var przedmiot = przedmioty[psel - 1];

            var sale = _kontekst.Sale.ToList();
            if (!sale.Any())
            {
                Console.WriteLine("Brak sal.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz salę (numer):");
            for (int i = 0; i < sale.Count; i++)
                Console.WriteLine($"{i + 1}) Sala {sale[i].NumerSali}");

            if (!int.TryParse(Console.ReadLine(), out int ssel) || ssel < 1 || ssel > sale.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var sala = sale[ssel - 1];

            var pracownicy = _kontekst.Pracownicy.ToList();
            if (!pracownicy.Any())
            {
                Console.WriteLine("Brak pracowników.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz pracownika (numer):");
            for (int i = 0; i < pracownicy.Count; i++)
                Console.WriteLine($"{i + 1}) {pracownicy[i].Imie} {pracownicy[i].Nazwisko}");

            if (!int.TryParse(Console.ReadLine(), out int wsel) || wsel < 1 || wsel > pracownicy.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var pracownik = pracownicy[wsel - 1];

            Console.Write("Dzień tygodnia (1=Pon, 2=Wt, 3=Śr, 4=Czw, 5=Pt, 6=Sob, 7=Nd): ");
            if (!int.TryParse(Console.ReadLine(), out int dzien) || dzien < 1 || dzien > 7)
            {
                Console.WriteLine("Nieprawidłowy dzień.");
                Console.ReadKey();
                return;
            }

            Console.Write("Godzina rozpoczęcia (HH:mm): ");
            if (!TimeSpan.TryParse(Console.ReadLine(), out var godzina))
            {
                Console.WriteLine("Nieprawidłowy format godziny.");
                Console.ReadKey();
                return;
            }

            Console.Write("Liczba godzin zajęć: ");
            if (!int.TryParse(Console.ReadLine(), out int liczbaGodzin) || liczbaGodzin <= 0)
            {
                Console.WriteLine("Nieprawidłowa liczba godzin.");
                Console.ReadKey();
                return;
            }

            var plan = new PlanZajec
            {
                GrupaID = grupa.GrupaID,
                PrzedmiotID = przedmiot.PrzedmiotID,
                SalaID = sala.SalaID,
                PracownikID = pracownik.PracownikID,
                DzienTygodnia = dzien,
                GodzinaRozpoczecia = godzina,
                GodzinaZakonczenia = godzina.Add(TimeSpan.FromMinutes(liczbaGodzin * 60))
            };

            _planRepo.Dodaj(plan);
            Console.WriteLine("Dodano wpis planu.");
            Console.ReadKey();
        }

        private void EditScheduleEntry()
        {
            var plan = _kontekst.PlanZajec
                .Include(p => p.Grupa)
                .Include(p => p.Przedmiot)
                .Include(p => p.Sala)
                .Include(p => p.Pracownik)
                .ToList();

            if (!plan.Any())
            {
                Console.WriteLine("Brak wpisów planu.");
                Console.ReadKey();
                return;
            }

            ShowScheduleEntries(plan);

            Console.Write("Wybierz numer wpisu do edycji: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > plan.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var wpis = plan[sel - 1];

            Console.Write($"Dzień tygodnia ({wpis.DzienTygodnia}): ");
            var dzienStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dzienStr) && int.TryParse(dzienStr, out int dzien))
                wpis.DzienTygodnia = dzien;

            Console.Write($"Godzina rozpoczęcia ({wpis.GodzinaRozpoczecia:hh\\:mm}): ");
            var godzinaStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(godzinaStr) && TimeSpan.TryParse(godzinaStr, out var godzina))
                wpis.GodzinaRozpoczecia = godzina;

            _planRepo.Aktualizuj(wpis);
            Console.WriteLine("Zapisano.");
            Console.ReadKey();
        }

        private void DeleteScheduleEntry()
        {
            var plan = _kontekst.PlanZajec
                .Include(p => p.Grupa)
                .Include(p => p.Przedmiot)
                .Include(p => p.Sala)
                .Include(p => p.Pracownik)
                .ToList();

            if (!plan.Any())
            {
                Console.WriteLine("Brak wpisów planu.");
                Console.ReadKey();
                return;
            }

            ShowScheduleEntries(plan);

            Console.Write("Wybierz numer wpisu do usunięcia: ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > plan.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            var wpis = plan[sel - 1];
            _planRepo.Usun(wpis.PlanZajecID);
            Console.WriteLine("Usunięto (jeśli istniał).");
            Console.ReadKey();
        }

        private void ShowAllScheduleEntries()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA WPISÓW PLANU ===");
            var plan = _kontekst.PlanZajec
                .Include(p => p.Grupa)
                .Include(p => p.Przedmiot)
                .Include(p => p.Sala)
                .Include(p => p.Pracownik)
                .ToList();

            if (!plan.Any())
            {
                Console.WriteLine("Brak wpisów planu.");
                Console.ReadKey();
                return;
            }

            ShowScheduleEntries(plan);

            Console.WriteLine("Wybierz numer, aby zobaczyć szczegóły (0 powrót): ");
            if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 0 || sel > plan.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór.");
                Console.ReadKey();
                return;
            }

            if (sel == 0) return;

            var wpis = plan[sel - 1];
            Console.Clear();
            string[] dni = { "", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
            var dzien = (wpis.DzienTygodnia >= 1 && wpis.DzienTygodnia <= 7) ? dni[wpis.DzienTygodnia] : $"Dzien {wpis.DzienTygodnia}";
            Console.WriteLine($"Szczegóły: {dzien} {wpis.GodzinaRozpoczecia:hh\\:mm}");
            Console.WriteLine($"Przedmiot: {wpis.Przedmiot?.Nazwa}");
            Console.WriteLine($"Sala: {wpis.Sala?.NumerSali}");
            Console.WriteLine($"Prowadzący: {wpis.Pracownik?.Imie} {wpis.Pracownik?.Nazwisko}");
            Console.WriteLine($"Grupa: {wpis.Grupa?.KodGrupy}");
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        private void ShowScheduleEntries(List<PlanZajec> plan)
        {
            string[] dni = { "", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
            int i = 1;
            foreach (var p in plan)
            {
                var dzien = (p.DzienTygodnia >= 1 && p.DzienTygodnia <= 7) ? dni[p.DzienTygodnia] : $"Dzien {p.DzienTygodnia}";
                Console.WriteLine($"{i++}. {dzien} {p.GodzinaRozpoczecia:hh\\:mm} | {p.Przedmiot?.Nazwa} | Sala: {p.Sala?.NumerSali} | Prowadzący: {p.Pracownik?.Imie} {p.Pracownik?.Nazwisko}");
            }
        }
    }
}
