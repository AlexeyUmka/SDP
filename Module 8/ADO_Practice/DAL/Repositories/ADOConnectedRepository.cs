using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using DAL.Attributes;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class AdoConnectedRepository<TEntity> : IRepository<TEntity> where TEntity:class, new()
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly DbConnection _connection;
        private readonly IDbReaderMapper<TEntity> _mapper;
        public DbTransaction _dbTransaction;

        public AdoConnectedRepository(IDbReaderMapperFactory mapperFactory, IDbConnection connection, DbProviderFactory dbProviderFactory)
        {
            _dbProviderFactory = dbProviderFactory;
            _mapper = mapperFactory.GetMapper<TEntity>();
            _connection = connection as DbConnection;
        }
        
        public IEnumerable<TEntity> GetAll()
        {
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM delivery.{typeof(TEntity).Name}";
            command.Connection = _connection;
            command.Transaction = _dbTransaction;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return _mapper.Map(reader);
            }
        }

        public IEnumerable<TEntity> GetByKey(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}='{p.GetValue(key)}' "));
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _connection;
            command.Transaction = _dbTransaction;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return _mapper.Map(reader);
            }
        }

        public void Insert(TEntity entity)
        {
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"'{p.GetValue(entity)?.ToString()}'"));
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"INSERT INTO delivery.{typeof(TEntity).Name} VALUES ({insertQuery})";
            command.Connection = _connection;
            command.Transaction = _dbTransaction;
            command.ExecuteNonQuery();
        }

        public void Update(TEntity entity)
        {
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(entity)}'"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}='{p.GetValue(entity)}' "));
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"UPDATE delivery.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            command.Connection = _connection;
            command.Transaction = _dbTransaction;
            command.ExecuteNonQuery();
        }

        public void Delete(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(key)}' "));
            var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"DELETE FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _connection;
            command.Transaction = _dbTransaction;
            command.ExecuteNonQuery();
        }

    }
}