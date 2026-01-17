using Microsoft.Extensions.DependencyInjection;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;
using UniversitySystem.Repositories;
using UniversitySystem.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Linq;

class Program
{
    private static ObslugaDziekanatu? dziekanat;
    private static ObslugaDydaktyczna? dydaktyka;
    private static UniversityContext? kontekst;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var kolekcjaSerwisow = new ServiceCollection();

        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                      ?? "Host=localhost;Port=5432;Database=WirtualnyDziekanat;Username=postgres;Password=SuperStrongPassword123!;";

        // Use Npgsql provider for PostgreSQL
        IServiceCollection serviceCollection = kolekcjaSerwisow.AddDbContext<UniversityContext>(opcje =>
            opcje.UseNpgsql(connStr)); 

        kolekcjaSerwisow.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        kolekcjaSerwisow.AddScoped<ObslugaDziekanatu>();
        kolekcjaSerwisow.AddScoped<ObslugaDydaktyczna>();

        var dostawcaUslug = kolekcjaSerwisow.BuildServiceProvider();

        using (var zakres = dostawcaUslug.CreateScope())
        {
            kontekst = zakres.ServiceProvider.GetRequiredService<UniversityContext>();

            try
            {
                InicjalizatorDb.Inicjalizuj(kontekst);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KRYTYCZNY BŁĄD] Nie można połączyć się z bazą danych: {ex.Message}");
                Console.WriteLine("Upewnij się, że kontener Dockera z PostgreSQL jest uruchomiony.");
                return;
            }

            dziekanat = zakres.ServiceProvider.GetRequiredService<ObslugaDziekanatu>();
            dydaktyka = zakres.ServiceProvider.GetRequiredService<ObslugaDydaktyczna>();

            // Uruchom główne menu
            UruchomMenuGlowne();
        }
    }

    static void UruchomMenuGlowne()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      SYSTEM ZARZĄDZANIA UNIWERSYTETEM - MENU GŁÓWNE     ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Panel Administracyjny (Dziekanat)                  ║");
            Console.WriteLine("║  2. Panel Wykładowcy                                   ║");
            Console.WriteLine("║  3. Panel Studenta (Wirtualny Dziekanat)               ║");
            Console.WriteLine("║  4. Przeglądaj dane (Przedmioty, Grupy, itp.)         ║");
            Console.WriteLine("║  0. Wyjście                                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nWybierz opcję: ");

            var wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    MenuDziekanatu();
                    break;
                case "2":
                    MenuWykladowcy();
                    break;
                case "3":
                    MenuStudenta();
                    break;
                case "4":
                    MenuPrzegladania();
                    break;
                case "0":
                    Console.WriteLine("\nDo widzenia!");
                    return;
                default:
                    Console.WriteLine("\n[BŁĄD] Nieprawidłowa opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void MenuDziekanatu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              PANEL ADMINISTRACYJNY (DZIEKANAT)          ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Zarejestruj nowego studenta                        ║");
            Console.WriteLine("║  2. Pokaż wszystkich studentów                         ║");
            Console.WriteLine("║  0. Powrót do menu głównego                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nWybierz opcję: ");

            var wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    ZarejestrujNowegoStudenta();
                    break;
                case "2":
                    Console.Clear();
                    dziekanat?.PokazWszystkichStudentow();
                    Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
                    Console.ReadKey();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\n[BŁĄD] Nieprawidłowa opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ZarejestrujNowegoStudenta()
    {
        Console.Clear();
        Console.WriteLine("=== REJESTRACJA NOWEGO STUDENTA ===\n");

        Console.Write("Imię: ");
        var imie = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(imie))
        {
            Console.WriteLine("[BŁĄD] Imię nie może być puste.");
            Console.ReadKey();
            return;
        }

        Console.Write("Nazwisko: ");
        var nazwisko = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(nazwisko))
        {
            Console.WriteLine("[BŁĄD] Nazwisko nie może być puste.");
            Console.ReadKey();
            return;
        }

        Console.Write("PESEL: ");
        var pesel = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(pesel))
        {
            Console.WriteLine("[BŁĄD] PESEL nie może być pusty.");
            Console.ReadKey();
            return;
        }

        // Pokaż dostępne grupy
        if (kontekst != null)
        {
            var grupy = kontekst.GrupyDziekanskie.Include(g => g.Kierunek).ToList();
            if (!grupy.Any())
            {
                Console.WriteLine("\n[BŁĄD] Brak dostępnych grup dziekańskich w systemie.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nDostępne grupy dziekańskie:");
            foreach (var grupa in grupy)
            {
                Console.WriteLine($"  - {grupa.KodGrupy} (Rok: {grupa.RokStudiow})");
            }
        }

        Console.Write("\nKod grupy dziekańskiej: ");
        var kodGrupy = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(kodGrupy))
        {
            Console.WriteLine("[BŁĄD] Kod grupy nie może być pusty.");
            Console.ReadKey();
            return;
        }

        dziekanat?.ZarejestrujStudenta(imie, nazwisko, pesel, kodGrupy);
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void MenuWykladowcy()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  PANEL WYKŁADOWCY                      ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Wystaw ocenę studentowi                            ║");
            Console.WriteLine("║  0. Powrót do menu głównego                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nWybierz opcję: ");

            var wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    WystawOcene();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\n[BŁĄD] Nieprawidłowa opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void WystawOcene()
    {
        Console.Clear();
        Console.WriteLine("=== WYSTAWIANIE OCENY ===\n");

        // Pokaż studentów
        if (kontekst != null)
        {
            var studenci = kontekst.Studenci.Include(s => s.Grupa).ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("[BŁĄD] Brak studentów w systemie.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Lista studentów:");
            foreach (var s in studenci)
            {
                Console.WriteLine($"  - {s.NrIndeksu}: {s.Imie} {s.Nazwisko} (Grupa: {s.Grupa?.KodGrupy})");
            }
        }

        Console.Write("\nNumer indeksu studenta: ");
        var nrIndeksu = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(nrIndeksu))
        {
            Console.WriteLine("[BŁĄD] Numer indeksu nie może być pusty.");
            Console.ReadKey();
            return;
        }

        // Pokaż przedmioty
        if (kontekst != null)
        {
            var przedmioty = kontekst.Przedmioty.ToList();
            if (!przedmioty.Any())
            {
                Console.WriteLine("[BŁĄD] Brak przedmiotów w systemie.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nDostępne przedmioty:");
            foreach (var p in przedmioty)
            {
                Console.WriteLine($"  - {p.Nazwa} (ECTS: {p.ECTS}, Semestr: {p.Semestr})");
            }
        }

        Console.Write("\nNazwa przedmiotu: ");
        var nazwaPrzedmiotu = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(nazwaPrzedmiotu))
        {
            Console.WriteLine("[BŁĄD] Nazwa przedmiotu nie może być pusta.");
            Console.ReadKey();
            return;
        }

        Console.Write("Ocena (2.0 - 5.0): ");
        if (!double.TryParse(Console.ReadLine(), out double wartosc) || wartosc < 2.0 || wartosc > 5.0)
        {
            Console.WriteLine("[BŁĄD] Nieprawidłowa ocena. Wprowadź wartość od 2.0 do 5.0.");
            Console.ReadKey();
            return;
        }

        dydaktyka?.WystawOcene(nrIndeksu, nazwaPrzedmiotu, wartosc);
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void MenuStudenta()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            PANEL STUDENTA (WIRTUALNY DZIEKANAT)        ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Pokaż moje oceny                                   ║");
            Console.WriteLine("║  0. Powrót do menu głównego                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nWybierz opcję: ");

            var wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    PokazOcenyStudentaInteraktywnie();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\n[BŁĄD] Nieprawidłowa opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void PokazOcenyStudentaInteraktywnie()
    {
        Console.Clear();
        Console.WriteLine("=== PRZEGLĄD OCEN STUDENTA ===\n");

        // Pokaż studentów
        if (kontekst != null)
        {
            var studenci = kontekst.Studenci.Include(s => s.Grupa).ToList();
            if (!studenci.Any())
            {
                Console.WriteLine("[BŁĄD] Brak studentów w systemie.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Lista studentów:");
            foreach (var s in studenci)
            {
                Console.WriteLine($"  - {s.NrIndeksu}: {s.Imie} {s.Nazwisko} (Grupa: {s.Grupa?.KodGrupy})");
            }
        }

        Console.Write("\nNumer indeksu studenta: ");
        var nrIndeksu = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(nrIndeksu))
        {
            Console.WriteLine("[BŁĄD] Numer indeksu nie może być pusty.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        dydaktyka?.PokazOcenyStudenta(nrIndeksu);
        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void MenuPrzegladania()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 PRZEGLĄDANIE DANYCH                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Pokaż wszystkie przedmioty                         ║");
            Console.WriteLine("║  2. Pokaż wszystkie grupy dziekańskie                  ║");
            Console.WriteLine("║  3. Pokaż plan zajęć                                   ║");
            Console.WriteLine("║  4. Pokaż pracowników                                  ║");
            Console.WriteLine("║  0. Powrót do menu głównego                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nWybierz opcję: ");

            var wybor = Console.ReadLine();

            switch (wybor)
            {
                case "1":
                    PokazPrzedmioty();
                    break;
                case "2":
                    PokazGrupy();
                    break;
                case "3":
                    PokazPlanZajec();
                    break;
                case "4":
                    PokazPracownikow();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\n[BŁĄD] Nieprawidłowa opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void PokazPrzedmioty()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA PRZEDMIOTÓW ===\n");

        if (kontekst == null) return;

        var przedmioty = kontekst.Przedmioty.ToList();
        if (!przedmioty.Any())
        {
            Console.WriteLine("Brak przedmiotów w systemie.");
        }
        else
        {
            foreach (var p in przedmioty)
            {
                Console.WriteLine($"- {p.Nazwa} | ECTS: {p.ECTS} | Semestr: {p.Semestr}");
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void PokazGrupy()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA GRUP DZIEKAŃSKICH ===\n");

        if (kontekst == null) return;

        var grupy = kontekst.GrupyDziekanskie.Include(g => g.Kierunek).ToList();
        if (!grupy.Any())
        {
            Console.WriteLine("Brak grup w systemie.");
        }
        else
        {
            foreach (var g in grupy)
            {
                var liczbaStudentow = kontekst.Studenci.Count(s => s.GrupaID == g.GrupaID);
                Console.WriteLine($"- {g.KodGrupy} | Kierunek: {g.Kierunek?.Nazwa} | Rok: {g.RokStudiow} | Studentów: {liczbaStudentow}");
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void PokazPlanZajec()
    {
        Console.Clear();
        Console.WriteLine("=== PLAN ZAJĘĆ ===\n");

        if (kontekst == null) return;

        var plan = kontekst.PlanZajec
            .Include(p => p.Przedmiot)
            .Include(p => p.Pracownik)
            .Include(p => p.Grupa)
            .Include(p => p.Sala)
            .ToList();

        if (!plan.Any())
        {
            Console.WriteLine("Brak zajęć w planie.");
        }
        else
        {
            string[] dniTygodnia = { "", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };

            foreach (var z in plan)
            {
                var dzien = z.DzienTygodnia >= 1 && z.DzienTygodnia <= 7 ? dniTygodnia[z.DzienTygodnia] : $"Dzień {z.DzienTygodnia}";
                Console.WriteLine($"{dzien} {z.Godzina:hh\\:mm} | {z.Przedmiot?.Nazwa} | Grupa: {z.Grupa?.KodGrupy} | Sala: {z.Sala?.NumerSali} | Wykładowca: {z.Pracownik?.Imie} {z.Pracownik?.Nazwisko}");
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void PokazPracownikow()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA PRACOWNIKÓW ===\n");

        if (kontekst == null) return;

        var pracownicy = kontekst.Pracownicy.Include(p => p.Stanowisko).ToList();
        if (!pracownicy.Any())
        {
            Console.WriteLine("Brak pracowników w systemie.");
        }
        else
        {
            foreach (var p in pracownicy)
            {
                Console.WriteLine($"- {p.Imie} {p.Nazwisko} | {p.Email} | {p.Stanowisko?.Nazwa} | Zatrudniony: {p.DataZatrudnienia:dd.MM.yyyy}");
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}