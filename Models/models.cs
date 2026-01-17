using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystem.Models
{
    // =============================================
    // 1. SŁOWNIKI I ZASOBY (Tabele referencyjne)
    // =============================================

    /// <summary>
    /// Reprezentuje wydział uczelni (np. Kolegium Informatyki Stosowanej).
    /// </summary>
    public class Wydzial
    {
        [Key] // Klucz główny
        public int WydzialID { get; set; }

        public string Nazwa { get; set; } // np. "Wydział Informatyki"
        public string Dziekan { get; set; } // Imię i nazwisko dziekana

        // Relacja: Jeden wydział ma wiele kierunków
        public ICollection<Kierunek> Kierunki { get; set; }
    }

    /// <summary>
    /// Reprezentuje kierunek studiów (np. Informatyka, Zarządzanie).
    /// </summary>
    public class Kierunek
    {
        [Key]
        public int KierunekID { get; set; }

        public string Nazwa { get; set; }
        public int Stopien { get; set; } // 1 = I stopnia (Licencjat/Inżynier), 2 = II stopnia (Magister)

        // Klucz obcy do Wydziału
        public int WydzialID { get; set; }
        public Wydzial Wydzial { get; set; }

        public ICollection<GrupaDziekanska> Grupy { get; set; }
    }

    /// <summary>
    /// Słownik stanowisk pracowników (np. Profesor, Asystent).
    /// </summary>
    public class Stanowisko
    {
        [Key]
        public int StanowiskoID { get; set; }

        public string Nazwa { get; set; }
        public decimal StawkaGodzinowa { get; set; } // Do obliczania wynagrodzeń

        public ICollection<Pracownik> Pracownicy { get; set; }
    }

    /// <summary>
    /// Status studenta w cyklu życia akademickiego (np. Aktywny, Skreślony, Urlop).
    /// </summary>
    public class StatusStudenta
    {
        [Key]
        public int StatusStudentaID { get; set; }

        public string Nazwa { get; set; }

        public ICollection<Student> Studenci { get; set; }
    }

    /// <summary>
    /// Sala dydaktyczna.
    /// </summary>
    public class Sala
    {
        [Key]
        public int SalaID { get; set; }

        public string NumerSali { get; set; } // np. "A101"
        public int LiczbaMiejsc { get; set; }
        public bool CzyKomputery { get; set; } // Czy sala posiada sprzęt PC?

        public ICollection<PlanZajec> PlanZajec { get; set; }
    }

    // =============================================
    // 2. LUDZIE I ORGANIZACJA (Studenci, Pracownicy)
    // =============================================

    /// <summary>
    /// Grupa studencka przypisana do konkretnego kierunku i roku.
    /// </summary>
    public class GrupaDziekanska
    {
        [Key]
        public int GrupaID { get; set; }

        public string KodGrupy { get; set; } // np. "IN.ISP.009"
        public int RokStudiow { get; set; }

        // Powiązanie z kierunkiem
        public int KierunekID { get; set; }
        public Kierunek Kierunek { get; set; }

        public ICollection<Student> Studenci { get; set; }
    }

    /// <summary>
    /// Centralna tabela przechowująca dane studentów.
    /// </summary>
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        public string NrIndeksu { get; set; } // Unikalny numer albumu (np. "w12345")
        public string Pesel { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        // Przypisanie do grupy
        public int GrupaID { get; set; }
        public GrupaDziekanska Grupa { get; set; }

        // Przypisanie statusu (np. czy jest aktywny)
        public int StatusStudentaID { get; set; }
        public StatusStudenta Status { get; set; }

        // Lista ocen studenta
        public ICollection<Ocena> Oceny { get; set; }
    }

    /// <summary>
    /// Pracownik uczelni (Wykładowca lub Administracja).
    /// </summary>
    public class Pracownik
    {
        [Key]
        public int PracownikID { get; set; }

        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }
        public DateTime DataZatrudnienia { get; set; }

        // Stanowisko pracownika
        public int StanowiskoID { get; set; }
        public Stanowisko Stanowisko { get; set; }

        // Lista prowadzonych zajęć
        public ICollection<PlanZajec> Zajecia { get; set; }
    }

    // =============================================
    // 3. PROCES DYDAKTYCZNY (Przedmioty, Oceny, Plan)
    // =============================================

    /// <summary>
    /// Przedmiot realizowany w ramach toku studiów (Sylabus).
    /// </summary>
    public class Przedmiot
    {
        [Key]
        public int PrzedmiotID { get; set; }

        public string Nazwa { get; set; } // np. "Programowanie Obiektowe"
        public int ECTS { get; set; } // Punkty ECTS
        public int Semestr { get; set; }

        public ICollection<PlanZajec> PlanZajec { get; set; }
    }

    /// <summary>
    /// Tabela łącząca Wykładowcę, Przedmiot, Grupę i Salę w czasie (Harmonogram).
    /// </summary>
    public class PlanZajec
    {
        [Key]
        public int PlanZajecID { get; set; }

        public int DzienTygodnia { get; set; } // 1 = Poniedziałek, 7 = Niedziela
        public TimeSpan Godzina { get; set; } // Godzina rozpoczęcia zajęć

        // KTO uczy?
        public int PracownikID { get; set; }
        public Pracownik Pracownik { get; set; }

        // CZEGO uczy?
        public int PrzedmiotID { get; set; }
        public Przedmiot Przedmiot { get; set; }

        // KOGO uczy?
        public int GrupaID { get; set; }
        public GrupaDziekanska Grupa { get; set; }

        // GDZIE uczy?
        public int SalaID { get; set; }
        public Sala Sala { get; set; }

        // Oceny wystawione w ramach tych zajęć
        public ICollection<Ocena> Oceny { get; set; }
    }

    /// <summary>
    /// Elektroniczny dziennik ocen.
    /// </summary>
    public class Ocena
    {
        [Key]
        public int OcenaID { get; set; }

        public double Wartosc { get; set; } // np. 2.0, 3.0, 3.5, 4.0, 4.5, 5.0
        public DateTime DataWystawienia { get; set; }
        public string TypOceny { get; set; } // np. "Egzamin", "Kolokwium", "Projekt"

        // Czyja to ocena?
        public int StudentID { get; set; }
        public Student Student { get; set; }

        // Z jakich zajęć?
        public int PlanZajecID { get; set; }
        public PlanZajec Zajecia { get; set; }
    }
}