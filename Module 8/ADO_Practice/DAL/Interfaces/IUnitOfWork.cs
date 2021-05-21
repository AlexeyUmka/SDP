using System;
using System.Data;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection GetConnection();
        IDbTransaction GetTransaction();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}