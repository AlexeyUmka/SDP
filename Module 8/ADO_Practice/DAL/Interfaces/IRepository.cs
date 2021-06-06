using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where
        T: class
    {
        Task<IEnumerable<T>> GetAll();    
        Task<T> GetByKey(object key);    
        Task Insert(T entity);    
        Task Update(T entity);    
        Task Delete(object key);
    }
}