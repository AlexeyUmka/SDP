using System;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T: class, new ();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}