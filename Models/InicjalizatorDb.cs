using System;
using System.Linq;
using UniversitySystem.Models;

namespace UniversitySystem.Data
{
    public static class InicjalizatorDb
    {
        public static void Inicjalizuj(UniversityContext kontekst)
        {
            // Upewnij się, że baza istnieje
            kontekst.Database.EnsureCreated();

            // Jeśli są już wydziały, nie rób nic (baza została już zainicjalizowana)
            if (kontekst.Wydzialy.Any()) return;

            Console.WriteLine("[SYSTEM] Inicjalizacja danych startowych...");

            // 1. Tworzenie słowników (StatusStudenta i Stanowisko)
            var statusAktywny = new StatusStudenta { Nazwa = "Aktywny" };
            kontekst.StatusyStudentow.Add(statusAktywny);
            
            var stanowiskoProfesor = new Stanowisko { Nazwa = "Profesor", StawkaGodzinowa = 150.00m };
            kontekst.Stanowiska.Add(stanowiskoProfesor);
            kontekst.SaveChanges();

            // 2. Tworzenie Wydziału i Kierunku
            var wydzial = new Wydzial { Nazwa = "Kolegium Informatyki Stosowanej", Dziekan = "Dr inż. Mariusz Wrzesień" };
            kontekst.Wydzialy.Add(wydzial);
            kontekst.SaveChanges();

            var kierunek = new Kierunek { Nazwa = "Informatyka", Stopien = 1, WydzialID = wydzial.WydzialID };
            kontekst.Kierunki.Add(kierunek);
            kontekst.SaveChanges();

            // 3. Tworzenie Grupy Dziekańskiej (z PDF: IN.ISP.009)
            var grupa = new GrupaDziekanska { KodGrupy = "IN.ISP.009", RokStudiow = 1, KierunekID = kierunek.KierunekID };
            kontekst.GrupyDziekanskie.Add(grupa);

            // 4. Tworzenie Przedmiotów i Sal
            var przedmiot = new Przedmiot { Nazwa = "Programowanie Obiektowe", ECTS = 5, Semestr = 1 };
            kontekst.Przedmioty.Add(przedmiot);

            var sala = new Sala { NumerSali = "A101", LiczbaMiejsc = 30 };
            kontekst.Sale.Add(sala);
            kontekst.SaveChanges();

            // 5. Tworzenie Pracownika i Planu Zajęć
            var pracownik = new Pracownik 
            { 
                Imie = "Jan", 
                Nazwisko = "Kowalski", 
                Email = "jan.kowalski@university.pl",
                DataZatrudnienia = DateTime.Now,
                StanowiskoID = stanowiskoProfesor.StanowiskoID
            };
            kontekst.Pracownicy.Add(pracownik);
            kontekst.SaveChanges();

            kontekst.PlanZajec.Add(new PlanZajec
            {
                DzienTygodnia = 1,
                Godzina = new TimeSpan(8, 0, 0),
                PracownikID = pracownik.PracownikID,
                PrzedmiotID = przedmiot.PrzedmiotID,
                GrupaID = grupa.GrupaID,
                SalaID = sala.SalaID
            });
            kontekst.SaveChanges();

            Console.WriteLine("[SYSTEM] Dane startowe zostały załadowane.");
        }
    }
}