using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;
using UniversitySystem.Repositories;
using UniversitySystem.Services;
using UniversitySystem.Models;

class Program
{
    private static UniversityContext? kontekst;
    private static StudentService? studentService;
    private static SubjectService? subjectService;
    private static ScheduleService? scheduleService;
    private static GradeService? gradeService;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var kolekcjaSerwisow = new ServiceCollection();

        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                      ?? "Host=localhost;Port=5432;Database=WirtualnyDziekanat;Username=postgres;Password=SuperStrongPassword123!;";

        kolekcjaSerwisow.AddDbContext<UniversityContext>(opcje => UniversityContext.Configure(opcje, connStr));

        kolekcjaSerwisow.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        kolekcjaSerwisow.AddScoped<StudentService>();
        kolekcjaSerwisow.AddScoped<SubjectService>();
        kolekcjaSerwisow.AddScoped<ScheduleService>();
        kolekcjaSerwisow.AddScoped<GradeService>();

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
                // Print full exception (including inner exception) to help debugging DB initialization errors
                Console.WriteLine($"[KRYTYCZNY BŁĄD] Nie można połączyć się z bazą danych: {ex}\n");
                Console.WriteLine("Upewnij się, że kontener Dockera z PostgreSQL jest uruchomiony.");
                return;
            }

            studentService = zakres.ServiceProvider.GetRequiredService<StudentService>();
            subjectService = zakres.ServiceProvider.GetRequiredService<SubjectService>();
            scheduleService = zakres.ServiceProvider.GetRequiredService<ScheduleService>();
            gradeService = zakres.ServiceProvider.GetRequiredService<GradeService>();

            RunServiceLoop();
        }
    }

    // Top-level role selection and dispatch
    static void RunServiceLoop()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== WITAMY W SYSTEMIE UNIWERSYTECKIM ===");
            Console.WriteLine("Wybierz rolę, pod którą chcesz pracować:");
            Console.WriteLine("1) Student");
            Console.WriteLine("2) Dziekanat");
            Console.WriteLine("3) Wykładowca");
            Console.WriteLine("0) Wyjście");
            Console.Write("Wybór: ");
            var wybor = Console.ReadLine()?.Trim();

            switch (wybor)
            {
                case "1": studentService?.StudentLogin(); break;
                case "2": DziekanatMenu(); break;
                case "3": WykladowcaMenu(); break;
                case "0": Console.WriteLine("Do widzenia!"); return;
                default: Console.WriteLine("Nieprawidłowy wybór. Naciśnij dowolny klawisz..."); Console.ReadKey(); break;
            }
        }
    }

    // Dziekanat (admin) menu - full access
    static void DziekanatMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PANEL DZIEKANATU ===");
            Console.WriteLine("1) Zarządzaj studentami (CRUD)");
            Console.WriteLine("2) Zarządzaj przedmiotami (CRUD)");
            Console.WriteLine("3) Zarządzaj planem zajęć");
            Console.WriteLine("0) Powrót");
            Console.Write("Wybór: ");
            var wybor = Console.ReadLine()?.Trim();
            switch (wybor)
            {
                case "1": studentService?.CrudStudents(true); break;
                case "2": subjectService?.CrudSubjects(); break;
                case "3": scheduleService?.CrudSchedule(); break;
                case "0": return;
                default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
            }
        }
    }

    // Wykładowca menu - can manage grades and view students and plan
    static void WykladowcaMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PANEL WYKŁADOWCY ===");
            Console.WriteLine("1) Wystaw ocenę");
            Console.WriteLine("2) Zarządzaj planem zajęć");
            Console.WriteLine("0) Powrót");
            Console.Write("Wybór: ");
            var wybor = Console.ReadLine()?.Trim();
            switch (wybor)
            {
                case "1": gradeService?.IssueGradeInteractively(); break;
                case "2": scheduleService?.CrudSchedule(); break;
                case "0": return;
                default: Console.WriteLine("Nieprawidłowy wybór."); Console.ReadKey(); break;
            }
        }
    }
}