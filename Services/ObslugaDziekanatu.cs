using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;
using UniversitySystem.Models;

namespace UniversitySystem.Services
{
    public class ObslugaDziekanatu
    {
        private readonly IRepository<Student> _repozytoriumStudentow;
        private readonly UniversityContext _kontekst; // Potrzebny do sprawdzania grup i statusów

        public ObslugaDziekanatu(IRepository<Student> repoStudentow, UniversityContext kontekst)
        {
            _repozytoriumStudentow = repoStudentow;
            _kontekst = kontekst;
        }

        // Funkcja: Immatrykulacja (Rejestracja) studenta 
        public void ZarejestrujStudenta(string imie, string nazwisko, string pesel, string kodGrupy)
        {
            // Sprawdź, czy grupa istnieje
            var grupa = _kontekst.GrupyDziekanskie.FirstOrDefault(g => g.KodGrupy == kodGrupy);
            if (grupa == null)
            {
                Console.WriteLine($"[BŁĄD] Grupa dziekańska '{kodGrupy}' nie istnieje.");
                return;
            }

            // Upewnij się, że istnieje status "Aktywny"
            var status = _kontekst.StatusyStudentow.FirstOrDefault(s => s.Nazwa == "Aktywny");
            if (status == null)
            {
                status = new StatusStudenta { Nazwa = "Aktywny" };
                _kontekst.StatusyStudentow.Add(status);
                _kontekst.SaveChanges();
            }

            // Generuj unikalny numer indeksu
            string nrIndeksu;
            do
            {
                nrIndeksu = GenerujNumerIndeksu();
            } while (_kontekst.Studenci.Any(s => s.NrIndeksu == nrIndeksu));

            // Utwórz nowy obiekt studenta
            var nowyStudent = new Student
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Pesel = pesel,
                NrIndeksu = nrIndeksu,
                GrupaID = grupa.GrupaID,
                StatusStudentaID = status.StatusStudentaID
            };

            _repozytoriumStudentow.Dodaj(nowyStudent);
            Console.WriteLine($"[DZIEKANAT] Zarejestrowano studenta: {imie} {nazwisko} (Indeks: {nowyStudent.NrIndeksu})");
        }

        // Raport: Lista studentów
        public void PokazWszystkichStudentow()
        {
            // Używamy Include, aby pobrać też nazwę grupy (JOIN)
            var studenci = _kontekst.Studenci.Include(s => s.Grupa).ToList();

            Console.WriteLine("\n--- LISTA STUDENTÓW W SYSTEMIE ---");
            foreach (var s in studenci)
            {
                Console.WriteLine($"Indeks: {s.NrIndeksu} | {s.Imie} {s.Nazwisko} | Grupa: {s.Grupa?.KodGrupy}");
            }
            Console.WriteLine("----------------------------------");
        }

        private string GenerujNumerIndeksu()
        {
            return new Random().Next(10000, 99999).ToString();
        }
    }
}