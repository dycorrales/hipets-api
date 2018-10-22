using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiPets.Infra.Data.Repositories
{
    public abstract class Repository<T> where T : Entity
    {
        protected DbSet<T> DbSet;
        protected DbContext Context { get; }
        protected IUser User { get; }

        public Repository(DbContext context, IUser user)
        {
            Context = context;
            DbSet = Context.Set<T>();
            User = user;
        }

        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void InsertRange(IEnumerable<T> entites)
        {
            DbSet.AddRangeAsync(entites);
        }

        public virtual void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Update<U>(U entity) where U : class
        {
            Context.Set<U>().Update(entity);
        }

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(entity => entity.Status != Status.Deleted).Any(predicate);
        }

        public virtual IEnumerable<T> FindAll()
        {
            return DbSet.AsNoTracking().Where(entity => entity.Status != Status.Deleted).OrderByDescending(entity => entity.CreatedAt);
        }

        public virtual T FindById(Guid id)
        {
            return DbSet.AsNoTracking().Where(entity => entity.Id.Equals(id) && entity.Status != Status.Deleted).FirstOrDefault();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate);
        }
    }
}
