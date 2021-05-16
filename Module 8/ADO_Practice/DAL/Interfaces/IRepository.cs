using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IRepository<T> where
        T: class
    {
        IEnumerable<T> GetAll();    
        T GetByKey(object key);    
        void Insert(T entity);    
        void Update(T entity);    
        void Delete(T entity);
    }
}