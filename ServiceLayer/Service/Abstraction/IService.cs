using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace ServiceLayer.Service.Abstraction
{
    public interface IService<T1, T2, TKey>
        where T1 : BaseEntity<TKey>
        where T2 : class
    {
        Task<T2> AddAsync(T2 entity);
        Task<T2> DeleteAsync(T2 entity);
        Task<T2> UpdateAsync(T2 entity);
        Task<IEnumerable<T2>> GetAllAsync();
        Task<T2> GetAsync(int id);
    }
}
