using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAL.Attributes;
using DAL.Interfaces;
using Dapper;

namespace DAL.Repositories
{
    public class DapperRepository<TEntity> : IRepository<TEntity> where TEntity: class, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public DapperRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IEnumerable<TEntity> GetAll()
        {
            var connection = _unitOfWork.GetConnection();
            var command = $"SELECT * FROM delivery.{typeof(TEntity).Name}";
            return connection.Query<TEntity>(command);
        }

        public IEnumerable<TEntity> GetByKey(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}='{p.GetValue(key)}' "));
            var command = $"SELECT * FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            return connection.Query<TEntity>(command);
        }

        public void Insert(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"'{p.GetValue(entity)?.ToString()}'"));
            var command = $"INSERT INTO delivery.{typeof(TEntity).Name} VALUES ({insertQuery})";
            connection.Execute(command);
        }

        public void Update(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(entity)}'"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}='{p.GetValue(entity)}' "));
            var command = $"UPDATE delivery.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            connection.Execute(command);
        }

        public void Delete(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}='{p.GetValue(key)}' "));
            var command = $"DELETE FROM delivery.{typeof(TEntity).Name} WHERE {equalsQuery}";
            connection.Execute(command);
        }
    }
}