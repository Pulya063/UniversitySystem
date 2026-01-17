using System.Collections.Generic;

namespace UniversitySystem.Interfaces
{
    // Generyczny interfejs - działa dla każdego typu T (Student, Ocena, itd.)
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> PobierzWszystkie();
        T PobierzPoId(int id);
        void Dodaj(T encja);
        void Aktualizuj(T encja);
        void Usun(int id);
    }
}