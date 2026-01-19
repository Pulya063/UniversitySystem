using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;
using UniversitySystem.Models;

namespace UniversitySystem.Services
{
    public class ObslugaDydaktyczna
    {
        private readonly IRepository<Ocena> _repozytoriumOcen;
        private readonly UniversityContext _kontekst;

        public ObslugaDydaktyczna(IRepository<Ocena> repoOcen, UniversityContext kontekst)
        {
            _repozytoriumOcen = repoOcen;
            _kontekst = kontekst;
        }

        // Funkcja: Elektroniczny protokół ocen 
        public void WystawOcene(string nrIndeksu, string nazwaPrzedmiotu, double wartosc)
        {
            var student = _kontekst.Studenci.FirstOrDefault(s => s.NrIndeksu == nrIndeksu);
            var przedmiot = _kontekst.Przedmioty.FirstOrDefault(p => p.Nazwa == nazwaPrzedmiotu);

            if (student == null || przedmiot == null)
            {
                Console.WriteLine("[BŁĄD] Nie znaleziono studenta lub przedmiotu.");
                return;
            }

            // Znajdź zajęcia w planie
            var zajecia = _kontekst.PlanZajec.FirstOrDefault(p => p.PrzedmiotID == przedmiot.PrzedmiotID);

            if (zajecia == null)
            {
                Console.WriteLine("[BŁĄD] Ten przedmiot nie jest zaplanowany w bieżącym semestrze.");
                return;
            }

            var nowaOcena = new Ocena
            {
                Wartosc = wartosc,
                DataWystawienia = DateTime.UtcNow,
                TypOceny = "Egzamin",
                StudentID = student.StudentID,
                PlanZajecID = zajecia.PlanZajecID
            };

            _repozytoriumOcen.Dodaj(nowaOcena);
            Console.WriteLine($"[WYKŁADOWCA] Wystawiono ocenę {wartosc} dla studenta {student.Nazwisko} z przedmiotu '{nazwaPrzedmiotu}'.");
        }

        // Funkcja: Panel Studenta - Podgląd ocen 
        public void PokazOcenyStudenta(string nrIndeksu)
        {
            var student = _kontekst.Studenci
                .Include(s => s.Oceny)
                .ThenInclude(o => o.Zajecia)
                .ThenInclude(z => z.Przedmiot)
                .FirstOrDefault(s => s.NrIndeksu == nrIndeksu);

            if (student == null)
            {
                Console.WriteLine("[BŁĄD] Nie znaleziono studenta.");
                return;
            }

            Console.WriteLine($"\n--- KARTA OSIĄGNIĘĆ: {student.Imie} {student.Nazwisko} ---");
            if (!student.Oceny.Any())
            {
                Console.WriteLine("(Brak ocen)");
            }
            else
            {
                foreach (var ocena in student.Oceny)
                {
                    Console.WriteLine($"Przedmiot: {ocena.Zajecia.Przedmiot.Nazwa,-25} | Ocena: {ocena.Wartosc} | Data: {ocena.DataWystawienia:d}");
                }
            }
            Console.WriteLine("---------------------------------------------");
        }
    }
}