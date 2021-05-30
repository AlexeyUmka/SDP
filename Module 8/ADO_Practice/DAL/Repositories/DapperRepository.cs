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
        private const string DbSchema = "delivery";
        private readonly IUnitOfWork _unitOfWork;
        
        public DapperRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IEnumerable<TEntity> GetAll()
        {
            var connection = _unitOfWork.GetConnection();
            var command = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name}";
            return connection.Query<TEntity>(command, transaction: _unitOfWork.GetTransaction());
        }

        public TEntity GetByKey(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}=@{p.Name} "));
            var command = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            return connection.QueryFirst<TEntity>(command, key, _unitOfWork.GetTransaction());
        }

        public void Insert(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"@{p.Name}"));
            var command = $"INSERT INTO {DbSchema}.{typeof(TEntity).Name} VALUES ({insertQuery})";
            connection.Execute(command, entity, _unitOfWork.GetTransaction());
        }

        public void Update(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name}"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}=@{p.Name} "));
            var command = $"UPDATE {DbSchema}.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            connection.Execute(command, entity, _unitOfWork.GetTransaction());
        }

        public void Delete(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name} "));
            var command = $"DELETE FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            connection.Execute(command, key, transaction: _unitOfWork.GetTransaction());
        }
    }
}