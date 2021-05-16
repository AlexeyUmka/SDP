using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories
{
    public class AdoConnectedRepository<TEntity> : IRepository<TEntity>, ITransactional where TEntity:class, new()
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly DbConnection _connection;
        private readonly IDbReaderMapper<TEntity> _mapper;
        private bool _disposed = false;

        public AdoConnectedRepository(IConfigurationRoot configurationRoot, IDbReaderMapperFactory mapperFactory)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            _dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            _mapper = mapperFactory.GetMapper<TEntity>();
            _connection = _dbProviderFactory.CreateConnection();
            _connection.ConnectionString = configurationRoot.GetConnectionString("SqlDeliveryDB");
            _connection.Open();
        }
        
        public IEnumerable<TEntity> GetAll()
        {
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM delivery.{typeof(TEntity).Name}";
            command.Connection = _connection;
            command.Transaction = GetCurrentTransaction() as DbTransaction;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return _mapper.Map(reader);
            }
        }

        public IEnumerable<TEntity> GetByKey(object key)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(object key)
        {
            throw new System.NotImplementedException();
        }
        
        public IDbTransaction GetCurrentTransaction()
        {
            return null;
        }

        public void BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new System.NotImplementedException();
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
                _connection.Close();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}