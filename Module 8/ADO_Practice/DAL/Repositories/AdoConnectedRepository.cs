using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DAL.Attributes;
using DAL.Extensions;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class AdoConnectedRepository<TEntity> : IRepository<TEntity> where TEntity:class, new()
    {
        private const string DbSchema = "delivery";
        private readonly IDbReaderMapper<TEntity> _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AdoConnectedRepository(IDbReaderMapperFactory mapperFactory, IUnitOfWork unitOfWork)
        {
            _mapper = mapperFactory.GetMapper<TEntity>();
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<TEntity>> GetAll()
        {
            await using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            await using var reader = await command.ExecuteReaderAsync();
            var result = new List<TEntity>();
            while (await reader.ReadAsync())
            {
                result.Add(_mapper.Map(reader));
            }
            return result;
        }

        public async Task<TEntity> GetByKey(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}=@{p.Name} "));
            await using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.AddParameters(key);
            await using var reader = command.ExecuteReader();
            await reader.ReadAsync();
            return _mapper.Map(reader);
        }

        public async Task Insert(TEntity entity)
        {
            var insertQuery = string.Join(',',entity.GetType().GetProperties().Select(p => $"@{p.Name}"));
            await using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;
            command.CommandType = CommandType.Text;
            command.CommandText = $"INSERT INTO {DbSchema}.{typeof(TEntity).Name} VALUES ({insertQuery})";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.AddParameters(entity);
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(TEntity entity)
        {
            var updateQuery = string.Join(",",entity.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name}"));
            var equalsQuery = string.Join("AND",entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $"{p.Name}=@{p.Name} "));
            await using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;
            command.CommandType = CommandType.Text;
            command.CommandText = $"UPDATE {DbSchema}.{typeof(TEntity).Name} SET {updateQuery} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.AddParameters(entity);
            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $"{p.Name}=@{p.Name} "));
            await using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;
            command.CommandType = CommandType.Text;
            command.CommandText = $"DELETE FROM {DbSchema}.{typeof(TEntity).Name} WHERE {equalsQuery}";
            command.Connection = _unitOfWork.GetConnection() as DbConnection;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            command.AddParameters(key);
            await command.ExecuteNonQueryAsync();
        }
    }
}