using System;
using System.Data;
using DAL.Interfaces;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;

        private bool _disposed;
        
        public UnitOfWork(IDbConnection connection)
        {
            _dbConnection = connection;
            _dbConnection.Open();
        }

        public IDbConnection GetConnection()
        {
            return _dbConnection;
        }

        public IDbTransaction GetTransaction()
        {
            return _dbTransaction;
        }

        public void BeginTransaction()
        {
            _dbTransaction = _dbConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            _dbTransaction.Rollback();
        }
        
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _dbTransaction?.Dispose();
            _dbConnection?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}