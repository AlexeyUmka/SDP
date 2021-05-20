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
        private readonly IDbReaderMapper<TEntity> _mapper;
        private readonly IUnitOfWork _unitOfWork;
        

        public AdoConnectedRepository(IDbReaderMapperFactory mapperFactory, DbProviderFactory dbProviderFactory, IUnitOfWork unitOfWork)
        {
            _dbProviderFactory = dbProviderFactory;
            _mapper = mapperFactory.GetMapper<TEntity>();
            _unitOfWork = unitOfWork;
        }
        
        public IEnumerable<TEntity> GetAll()
        {
            using var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM delivery.{typeof(TEntity).Name}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return _mapper.Map(reader);
            }
        }

        public IEnumerable<TEntity> GetByKey(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}='{p.GetValue(key)}' "));
            using var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return _mapper.Map(reader);
            }
        }

        public void Insert(TEntity entity)
        {
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"'{p.GetValue(entity)?.ToString()}'"));
            using var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"INSERT INTO delivery.{typeof(TEntity).Name} VALUES ({insertQuery})";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.ExecuteNonQuery();
        }

        public void Update(TEntity entity)
        {
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(entity)}'"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}='{p.GetValue(entity)}' "));
            using var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"UPDATE delivery.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.ExecuteNonQuery();
        }

        public void Delete(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(key)}' "));
            using var command = _dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"DELETE FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.ExecuteNonQuery();
        }

    }
}