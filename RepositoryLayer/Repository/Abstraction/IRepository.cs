using System;
using System.Collections.Generic;
using System.Text;
using DomainLayer.Models;

namespace RepositoryLayer.Repository.Abstraction
{
    public interface IRepository<T, T2> where T : BaseEntity<T2>
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllQueryable();
        Task<T> CreateAsync(T entity);
        void SaveChangesAsync();
        void SaveChanges();
        Task<T> DeleteAsync(T entity);
        Task<T> DeleteById(T2 id);
        Task<T> UpdateAsync(T entity);
        Task<T?> Get(T2 Id);
    }
}
