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
            _zestawDanych.Add(encja);
            _kontekst.SaveChanges(); // Zapis synchroniczny (natychmiastowy)
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
