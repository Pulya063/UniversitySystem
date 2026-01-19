using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystem.Models
{
    // =============================================
    // 1. СЛОВНИКИ ТА РЕСУРСИ
    // =============================================

    /// <summary>
    /// Факультет університету.
    /// </summary>
    public class Wydzial
    {
        [Key]
        public int WydzialID { get; set; }

        [Required, StringLength(100)]
        public string Nazwa { get; set; } = null!; // Назва факультету

        public string Dziekan { get; set; } = null!; // ПІБ декана

        // Зв'язок: Один факультет — багато напрямів
        public virtual ICollection<Kierunek> Kierunki { get; set; } = new List<Kierunek>();
    }

    /// <summary>
    /// Напрям навчання (спеціальність).
    /// </summary>
    public class Kierunek
    {
        [Key]
        public int KierunekID { get; set; }

        [Required]
        public string Nazwa { get; set; } = null!;

        public int Stopien { get; set; } // 1 = Бакалавр, 2 = Магістр

        // Зв'язок з факультетом
        public int WydzialID { get; set; }
        public virtual Wydzial Wydzial { get; set; } = null!;

        public virtual ICollection<GrupaDziekanska> Grupy { get; set; } = new List<GrupaDziekanska>();
        public virtual ICollection<Przedmiot> Przedmioty { get; set; } = new List<Przedmiot>();
    }

    /// <summary>
    /// Посади працівників та їх ставки.
    /// </summary>
    public class Stanowisko
    {
        [Key]
        public int StanowiskoID { get; set; }

        public string Nazwa { get; set; } = null!;
        public decimal StawkaGodzinowa { get; set; } // Оплата за годину

        public virtual ICollection<Pracownik> Pracownicy { get; set; } = new List<Pracownik>();
    }

    /// <summary>
    /// Стутус студента (Активний, Академвідпустка тощо).
    /// </summary>
    public class StatusStudenta
    {
        [Key]
        public int StatusStudentaID { get; set; }

        public string Nazwa { get; set; } = null!;

        public virtual ICollection<Student> Studenci { get; set; } = new List<Student>();
    }

    /// <summary>
    /// Аудиторія для занять.
    /// </summary>
    public class Sala
    {
        [Key]
        public int SalaID { get; set; }

        public string NumerSali { get; set; } = null!;
        public int LiczbaMiejsc { get; set; }
        public bool CzyKomputery { get; set; } // Наявність ПК

        public virtual ICollection<PlanZajec> PlanZajec { get; set; } = new List<PlanZajec>();
    }

    // =============================================
    // 2. ЛЮДИ ТА ОРГАНІЗАЦІЯ
    // =============================================

    /// <summary>
    /// Студентська група.
    /// </summary>
    public class GrupaDziekanska
    {
        [Key]
        public int GrupaID { get; set; }

        public string KodGrupy { get; set; } = null!; // Напр. "IN-11"
        public int RokStudiow { get; set; }

        // Зв'язок з напрямом навчання
        public int KierunekID { get; set; }
        public virtual Kierunek Kierunek { get; set; } = null!;

        public virtual ICollection<Student> Studenci { get; set; } = new List<Student>();
    }

    /// <summary>
    /// Дані студента.
    /// </summary>
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        public string NrIndeksu { get; set; } = null!; // Номер залікової книжки
        public string Pesel { get; set; } = null!;
        public string Imie { get; set; } = null!;
        public string Nazwisko { get; set; } = null!;

        // Група студента
        public int GrupaID { get; set; }
        public virtual GrupaDziekanska Grupa { get; set; } = null!;

        // Статус навчання
        public int StatusStudentaID { get; set; }
        public virtual StatusStudenta Status { get; set; } = null!;

        public virtual ICollection<Ocena> Oceny { get; set; } = new List<Ocena>();
    }

    /// <summary>
    /// Викладач або адміністратор.
    /// </summary>
    public class Pracownik
    {
        [Key]
        public int PracownikID { get; set; }

        public string Imie { get; set; } = null!;
        public string Nazwisko { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DataZatrudnienia { get; set; }

        // Посада
        public int StanowiskoID { get; set; }
        public virtual Stanowisko Stanowisko { get; set; } = null!;

        public virtual ICollection<PlanZajec> Zajecia { get; set; } = new List<PlanZajec>();
    }

    // =============================================
    // 3. НАВЧАЛЬНИЙ ПРОЦЕС
    // =============================================

    /// <summary>
    /// Навчальна дисципліна.
    /// </summary>
    public class Przedmiot
    {
        [Key]
        public int PrzedmiotID { get; set; }

        public string Nazwa { get; set; } = null!;
        public int ECTS { get; set; } // Кредити ECTS
        public int Semestr { get; set; }

        // ВИПРАВЛЕНО: Зв'язок з напрямом навчання зроблено public
        public int KierunekID { get; set; }
        public virtual Kierunek Kierunek { get; set; } = null!;

        public virtual ICollection<PlanZajec> PlanZajec { get; set; } = new List<PlanZajec>();
    }

    /// <summary>
    /// Розклад занять (ланка між усіма сутностями).
    /// </summary>
    public class PlanZajec
    {
        [Key]
        public int PlanZajecID { get; set; }

        public int DzienTygodnia { get; set; } // 1-7

        // ВИПРАВЛЕНО: Залишено лише уніфіковані поля часу
        public TimeSpan GodzinaRozpoczecia { get; set; }
        public TimeSpan GodzinaZakonczenia { get; set; }

        // Викладач
        public int PracownikID { get; set; }
        public virtual Pracownik Pracownik { get; set; } = null!;

        // Дисципліна
        public int PrzedmiotID { get; set; }
        public virtual Przedmiot Przedmiot { get; set; } = null!;

        // Група
        public int GrupaID { get; set; }
        public virtual GrupaDziekanska Grupa { get; set; } = null!;

        // Аудиторія
        public int SalaID { get; set; }
        public virtual Sala Sala { get; set; } = null!;

        public virtual ICollection<Ocena> Oceny { get; set; } = new List<Ocena>();
    }

    /// <summary>
    /// Оцінка студента.
    /// </summary>
    public class Ocena
    {
        [Key]
        public int OcenaID { get; set; }

        public double Wartosc { get; set; } // 2.0 - 5.0
        public DateTime DataWystawienia { get; set; }
        public string TypOceny { get; set; } = null!; // "Екзамен", "Проєкт" тощо

        // Власник оцінки
        public int StudentID { get; set; }
        public virtual Student Student { get; set; } = null!;

        // Посилання на конкретне заняття з розкладу
        public int PlanZajecID { get; set; }
        public virtual PlanZajec Zajecia { get; set; } = null!;
    }
}