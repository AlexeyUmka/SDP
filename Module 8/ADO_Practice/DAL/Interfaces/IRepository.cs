using System;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where
        T: class
    {
        IEnumerable<T> GetAll();    
        IEnumerable<T> GetByKey(object key);    
        void Insert(T entity);    
        void Update(T entity);    
        void Delete(object key);
    }
}