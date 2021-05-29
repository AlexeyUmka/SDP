using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using DAL.Interfaces;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private readonly string _connectionString;

        private bool _disposed;
        
        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            if (_dbConnection is null)
            {
                var dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                _dbConnection = dbProviderFactory.CreateConnection();
                _dbConnection.ConnectionString = _connectionString;
                _dbConnection.Open();
            }
            else if ((new[] {ConnectionState.Broken, ConnectionState.Closed}).Any(state => _dbConnection.State == state))
            {
                _dbConnection.Open();
            }

            return _dbConnection;
        }

        public IDbTransaction GetTransaction()
        {
            return _dbTransaction ??= GetConnection().BeginTransaction();
        }

        public void Save()
        {
            var transaction = GetTransaction();
            transaction.Commit();
            transaction.Dispose();
            _dbTransaction = GetConnection().BeginTransaction();
        }

        public void Rollback()
        {
            var transaction = GetTransaction();
            transaction.Rollback();
            transaction.Dispose();
            _dbTransaction = GetConnection().BeginTransaction();
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
        }
    }
}