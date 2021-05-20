using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Transactions;
using DAL.Interfaces;
using DAL.Repositories;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;

        private bool _disposed = false;
        
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _dbTransaction?.Dispose();
                _dbConnection?.Dispose();
            }
            if (Transaction.Current is not null && Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
            {
                _dbTransaction?.Rollback();
            }
            if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
            }
            _disposed = true;
        }
    }
}