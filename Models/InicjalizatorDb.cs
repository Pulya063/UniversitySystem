using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;

namespace UniversitySystem.Data
{
    public static class InicjalizatorDb
    {
        public static void Inicjalizuj(UniversityContext kontekst)
        {
            // Optional: allow force-recreate during development by setting env var FORCE_DB_RECREATE=1
            // WARNING: force-recreate will delete existing data
            var forceRecreate = Environment.GetEnvironmentVariable("FORCE_DB_RECREATE");
            if (forceRecreate == "1")
            {
                Console.WriteLine("[SYSTEM] FORCE_DB_RECREATE=1 detected - usuwam i odtwarzam bazę danych (DESTRUKCYJNE).\n");
                kontekst.Database.EnsureDeleted();
                // Apply migrations (fresh)
                kontekst.Database.Migrate();
            }
            else
            {
                // Prefer migrations in regular execution; Apply any pending migrations
                kontekst.Database.Migrate();
            }

            // Jeśli są już wydziały, nie rób nic (baza została już zainicjalizowana)
            if (kontekst.Wydzialy.Any()) return;

            Console.WriteLine("[SYSTEM] Inicjalizacja danych startowych...");

            // 1. Tworzenie słowników (StatusStudenta i Stanowisko)
            var statusAktywny = new StatusStudenta { Nazwa = "Aktywny" };
            kontekst.StatusyStudentow.Add(statusAktywny);
            
            var stanowiskoProfesor = new Stanowisko { Nazwa = "Profesor", StawkaGodzinowa = 150.00m };
            kontekst.Stanowiska.Add(stanowiskoProfesor);
            kontekst.SaveChanges();

            // 2. Tworzenie Wydziału i Kierunków
            var wydzial = new Wydzial { Nazwa = "Kolegium Informatyki Stosowanej", Dziekan = "Dr inż. Mariusz Wrzesień" };
            kontekst.Wydzialy.Add(wydzial);
            kontekst.SaveChanges();

            // Dodajemy dwa kierunki pod wydział: Programowanie i Marketing
            var kierunekProgramowanie = new Kierunek { Nazwa = "Programowanie", Stopien = 1, WydzialID = wydzial.WydzialID };
            var kierunekMarketing = new Kierunek { Nazwa = "Marketing", Stopien = 1, WydzialID = wydzial.WydzialID };
            kontekst.Kierunki.Add(kierunekProgramowanie);
            kontekst.Kierunki.Add(kierunekMarketing);
            kontekst.SaveChanges();

            // 3. Tworzenie Grupy Dziekańskiej (przykładowa grupa przypisana do kierunku Programowanie)
            var grupa = new GrupaDziekanska { KodGrupy = "IN.ISP.009", RokStudiow = 1, KierunekID = kierunekProgramowanie.KierunekID };
            kontekst.GrupyDziekanskie.Add(grupa);

            // 4. Tworzenie Przedmiotów przypisanych do kierunków
            var przedmiotPO = new Przedmiot { Nazwa = "Programowanie Obiektowe", ECTS = 5, Semestr = 1, KierunekID = kierunekProgramowanie.KierunekID };
            var przedmiotAlg = new Przedmiot { Nazwa = "Algorytmy i Struktury Danych", ECTS = 6, Semestr = 1, KierunekID = kierunekProgramowanie.KierunekID };
            var przedmiotMK1 = new Przedmiot { Nazwa = "Marketing Cyfrowy", ECTS = 4, Semestr = 1, KierunekID = kierunekMarketing.KierunekID };
            var przedmiotMK2 = new Przedmiot { Nazwa = "Strategie Marketingowe", ECTS = 5, Semestr = 1, KierunekID = kierunekMarketing.KierunekID };

            kontekst.Przedmioty.Add(przedmiotPO);
            kontekst.Przedmioty.Add(przedmiotAlg);
            kontekst.Przedmioty.Add(przedmiotMK1);
            kontekst.Przedmioty.Add(przedmiotMK2);

            // 5. Tworzenie Sal
            var sala = new Sala { NumerSali = "A101", LiczbaMiejsc = 30 };
            kontekst.Sale.Add(sala);
            kontekst.SaveChanges();

            // 6. Tworzenie Pracownika i Planu Zajęć
            var pracownik = new Pracownik 
            { 
                Imie = "Jan", 
                Nazwisko = "Kowalski", 
                Email = "jan.kowalski@university.pl",
                DataZatrudnienia = DateTime.UtcNow,
                StanowiskoID = stanowiskoProfesor.StanowiskoID
            };
            kontekst.Pracownicy.Add(pracownik);
            kontekst.SaveChanges();

            // Dodajemy wpisy do planu z godziną rozpoczęcia i zakończenia
            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 1,
                GodzinaRozpoczecia = new TimeSpan(8, 0, 0),
                GodzinaZakonczenia = new TimeSpan(9, 30, 0),
                PracownikID = pracownik.PracownikID,
                PrzedmiotID = przedmiotPO.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala.SalaID
            });

            // add a couple more schedule entries to make sample data richer
            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 2,
                GodzinaRozpoczecia = new TimeSpan(10, 0, 0),
                GodzinaZakonczenia = new TimeSpan(11, 30, 0),
                PracownikID = pracownik.PracownikID,
                PrzedmiotID = przedmiotAlg.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala.SalaID
            });

            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 3,
                GodzinaRozpoczecia = new TimeSpan(12, 0, 0),
                GodzinaZakonczenia = new TimeSpan(13, 30, 0),
                PracownikID = pracownik.PracownikID,
                PrzedmiotID = przedmiotMK1.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala.SalaID
            });

            kontekst.SaveChanges();

            // 7. Dodajemy kilka dodatkowych stanowisk, sal i pracowników
            var stanowiskoAsystent = new Stanowisko { Nazwa = "Asystent", StawkaGodzinowa = 80.00m };
            var stanowiskoAdiunkt = new Stanowisko { Nazwa = "Adiunkt", StawkaGodzinowa = 120.00m };
            kontekst.Stanowiska.AddRange(stanowiskoAsystent, stanowiskoAdiunkt);
            kontekst.SaveChanges();

            var sala2 = new Sala { NumerSali = "B202", LiczbaMiejsc = 25 };
            var sala3 = new Sala { NumerSali = "C303", LiczbaMiejsc = 40, CzyKomputery = true };
            kontekst.Sale.AddRange(sala2, sala3);
            kontekst.SaveChanges();

            var pracownik2 = new Pracownik
            {
                Imie = "Anna",
                Nazwisko = "Nowak",
                Email = "anna.nowak@university.pl",
                DataZatrudnienia = DateTime.UtcNow.AddYears(-2),
                StanowiskoID = stanowiskoAsystent.StanowiskoID
            };
            var pracownik3 = new Pracownik
            {
                Imie = "Piotr",
                Nazwisko = "Zieliński",
                Email = "piotr.zielinski@university.pl",
                DataZatrudnienia = DateTime.UtcNow.AddYears(-5),
                StanowiskoID = stanowiskoAdiunkt.StanowiskoID
            };
            kontekst.Pracownicy.AddRange(pracownik2, pracownik3);
            kontekst.SaveChanges();

            // 8. Dodajemy więcej wpisów do planu korzystając z nowych sal i pracowników
            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 4,
                GodzinaRozpoczecia = new TimeSpan(9, 0, 0),
                GodzinaZakonczenia = new TimeSpan(10, 30, 0),
                PracownikID = pracownik2.PracownikID,
                PrzedmiotID = przedmiotAlg.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala2.SalaID
            });

            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 5,
                GodzinaRozpoczecia = new TimeSpan(14, 0, 0),
                GodzinaZakonczenia = new TimeSpan(15, 30, 0),
                PracownikID = pracownik3.PracownikID,
                PrzedmiotID = przedmiotMK2.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala3.SalaID
            });

            kontekst.SaveChanges();

            // 9. Dodajemy studentów przypisanych do grupy
            var stud1 = new Student { Imie = "Marek", Nazwisko = "Kowalczyk", Pesel = "89010112345", NrIndeksu = "S2021001", GrupaID = grupa.GrupaID, StatusStudentaID = statusAktywny.StatusStudentaID };
            var stud2 = new Student { Imie = "Ewa", Nazwisko = "Wiśniewska", Pesel = "90020254321", NrIndeksu = "S2021002", GrupaID = grupa.GrupaID, StatusStudentaID = statusAktywny.StatusStudentaID };
            var stud3 = new Student { Imie = "Tomasz", Nazwisko = "Lewandowski", Pesel = "91030398765", NrIndeksu = "S2021003", GrupaID = grupa.GrupaID, StatusStudentaID = statusAktywny.StatusStudentaID };
            kontekst.Studenci.AddRange(stud1, stud2, stud3);
            kontekst.SaveChanges();

            // 10. Dodajemy oceny dla studentów powiązane z wpisami w planie
            var wszystkieZajecia = kontekst.PlanZajec.ToList();
            if (wszystkieZajecia.Any())
            {
                var pierwsze = wszystkieZajecia[0];
                var drugie = wszystkieZajecia.Count > 1 ? wszystkieZajecia[1] : pierwsze;
                kontekst.Oceny.Add(new Ocena { Wartosc = 4.5, DataWystawienia = DateTime.UtcNow.AddDays(-10), TypOceny = "Kolokwium", StudentID = stud1.StudentID, PlanZajecID = pierwsze.PlanZajecID });
                kontekst.Oceny.Add(new Ocena { Wartosc = 3.0, DataWystawienia = DateTime.UtcNow.AddDays(-5), TypOceny = "Projekt", StudentID = stud2.StudentID, PlanZajecID = drugie.PlanZajecID });
                kontekst.Oceny.Add(new Ocena { Wartosc = 5.0, DataWystawienia = DateTime.UtcNow.AddDays(-1), TypOceny = "Egzamin", StudentID = stud3.StudentID, PlanZajecID = pierwsze.PlanZajecID });
                kontekst.SaveChanges();
            }

            Console.WriteLine("[SYSTEM] Dane startowe zostały załadowane.");
        }
    }
}