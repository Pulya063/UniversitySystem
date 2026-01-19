using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversitySystem.Data;
using UniversitySystem.Interfaces;

namespace UniversitySystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly UniversityContext _kontekst;
        private readonly DbSet<T> _zestawDanych;

        public Repository(UniversityContext kontekst)
        {
            _kontekst = kontekst;
            _zestawDanych = kontekst.Set<T>();
        }

        public IEnumerable<T> PobierzWszystkie()
        {
            return _zestawDanych.ToList();
        }

        public T PobierzPoId(int id)            
        {
            return _zestawDanych.Find(id);
        }

        public void Dodaj(T encja)
        {
            try
            {
                _zestawDanych.Add(encja);
                _kontekst.SaveChanges(); // Zapis synchroniczny (natychmiastowy)
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Give clearer guidance for schema/column mismatches which are common with Postgres casing
                var inner = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException($"Błąd zapisu do bazy danych: {inner}.\nJeśli widzisz błąd o brakującej kolumnie (np. 'KierunekID'), uruchom aplikację z env var FORCE_DB_RECREATE=1 aby odtworzyć schemat (tylko w trakcie developmentu).", ex);
            }
        }

        public void Aktualizuj(T encja)
        {
            _zestawDanych.Update(encja);
            _kontekst.SaveChanges();
        }

        public void Usun(int id)
        {
            var encja = _zestawDanych.Find(id);
            if (encja != null)
            {
                _zestawDanych.Remove(encja);
                _kontekst.SaveChanges();
            }
        }
    }
}
