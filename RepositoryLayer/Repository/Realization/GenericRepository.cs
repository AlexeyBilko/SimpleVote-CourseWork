using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using DomainLayer.Models;
using RepositoryLayer.Repository.Abstraction;

namespace RepositoryLayer.Repository.Realization
{
    public abstract class GenericRepository<T, TKey> : IRepository<T, TKey>  where T : BaseEntity<TKey>
    {
        public ApplicationDbContext context;
        private DbSet<T> table;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            table = context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await table.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            table.Remove(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteById(TKey id)
        {
            var entity = table.Find(id);
            table.Remove(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> Get(TKey Id)
        {
            return await table.FindAsync(Id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }

        //TODO : read about specifications and IQueryable
        public IQueryable<T> GetAllQueryable()
        {
            return table.AsQueryable();
        }

        public async void SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
