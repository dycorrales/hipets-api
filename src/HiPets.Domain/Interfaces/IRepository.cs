using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HiPets.Domain.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        void Insert(T entity);
        void InsertRange(IEnumerable<T> entites);
        void Update(T entity);
        void Update<U>(U entity) where U : class;
        void Delete(T entity);

        bool Any(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll();
        T FindById(Guid id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }
}
