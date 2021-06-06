using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        
        public async Task<IEnumerable<TEntity>> GetAll()
        {
            var connection = _unitOfWork.GetConnection();
            var command = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name}";
            return await connection.QueryAsync<TEntity>(command, transaction: _unitOfWork.GetTransaction());
        }

        public async Task<TEntity> GetByKey(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}=@{p.Name} "));
            var command = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            return await connection.QueryFirstAsync<TEntity>(command, key, _unitOfWork.GetTransaction());
        }

        public async Task Insert(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"@{p.Name}"));
            var command = $"INSERT INTO {DbSchema}.{typeof(TEntity).Name} VALUES ({insertQuery})";
            await connection.ExecuteAsync(command, entity, _unitOfWork.GetTransaction());
        }

        public async Task Update(TEntity entity)
        {
            var connection = _unitOfWork.GetConnection();
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name}"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}=@{p.Name} "));
            var command = $"UPDATE {DbSchema}.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            await connection.ExecuteAsync(command, entity, _unitOfWork.GetTransaction());
        }

        public async Task Delete(object key)
        {
            var connection = _unitOfWork.GetConnection();
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name} "));
            var command = $"DELETE FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            await connection.ExecuteAsync(command, key, transaction: _unitOfWork.GetTransaction());
        }
    }
}