using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DAL.Interfaces;
using DAL.Repositories;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbReaderMapperFactory _mapperFactory;
        private readonly IDbConnection _dbConnection;
        private readonly DbProviderFactory _dbProviderFactory;
        private IDbTransaction _dbTransaction;

        private bool _disposed = false;
        
        public UnitOfWork(IDbReaderMapperFactory mapperFactory, IDbConnection connection, DbProviderFactory dbProviderFactory)
        {
            _mapperFactory = mapperFactory;
            _dbConnection = connection;
            _dbProviderFactory = dbProviderFactory;
            _dbConnection.Open();
        }

        private readonly Dictionary<Type, object> _repositories = new();
        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default :
                    object repository;
                    if (!_repositories.TryGetValue(typeof(T), out repository))
                    {
                        repository = 
                            Activator.CreateInstance(
                                typeof(AdoConnectedRepository<T>),
                                _mapperFactory, _dbConnection, _dbProviderFactory) as IRepository<T>;
                        _repositories.Add(typeof(T), repository);
                    }
                    return (IRepository<T>) repository;;
            }
        }

        public void BeginTransaction()
        {
            _dbTransaction = _dbConnection.BeginTransaction();
            foreach (var (key, value) in _repositories)
            {
                value.GetType().GetField("_dbTransaction")?.SetValue(value, _dbTransaction);
            }
        }

        public void CommitTransaction()
        {
            _dbTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            _dbTransaction.Rollback();
        }
        
        public void Dispose() => Dispose(true);
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _dbTransaction?.Rollback();
                if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }
                _dbTransaction?.Dispose();
                _dbConnection?.Dispose();
            }
            _disposed = true;
        }
    }
}