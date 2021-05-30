using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DAL.Attributes;
using DAL.Extensions;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class AdoDisconnectedRepository<TEntity> : IRepository<TEntity> where TEntity:class, new()
    {
        private const string DbSchema = "delivery";
        private const string TableIndexName = "Table";
        private string TableName => $"{DbSchema}.{typeof(TEntity).Name}";

        private readonly IUnitOfWork _unitOfWork;
        
        public AdoDisconnectedRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return GetRows();
        }

        public TEntity GetByKey(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}=@{p.Name} "));
            return GetRows($"{equalsQuery}", parameters: key).FirstOrDefault();
        }

        public void Insert(TEntity entity)
        {
            void CreateRow(DataSet dataSet, SqlDataAdapter dataAdapter)
            {
                var row = dataSet.Tables[TableIndexName].NewRow();
                PrepareRow(entity, row);

                dataSet.Tables[TableIndexName].Rows.Add(row.ItemArray);
                var commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
            };

            ExecuteUpdate(CreateRow, count: 1, parameters: entity);
        }

        public void Update(TEntity entity)
        {
            var equalsQuery = string.Join("AND", entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes().Any(attr => attr is Identifier))
                .Select(p => $" {p.Name}=@{p.Name} "));

            void UpdateRow(DataSet dataSet, SqlDataAdapter dataAdapter)
            {
                var row = dataSet.Tables[TableIndexName].Rows[0];
                PrepareRow(entity, row);

                var commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            }

            ExecuteUpdate(UpdateRow, equalsQuery, 1, entity);
        }

        public void Delete(object key)
        {
            var equalsQuery = string.Join("AND",key.GetType().GetProperties().Select(p => $" {p.Name}=@{p.Name} "));
            void DeleteRow(DataSet dataSet, SqlDataAdapter dataAdapter)
            {
                if (dataSet.Tables[TableIndexName].Rows.Count == 0)
                {
                    return;
                }

                dataSet.Tables[TableIndexName].Rows[0].Delete();

                var commandBuilder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            }

            ExecuteUpdate(DeleteRow, equalsQuery, parameters: key);
        }

        private IEnumerable<TEntity> GetRows(string where = null, int? count = null, object parameters = null)
        {
            using var command = _unitOfWork.GetConnection().CreateCommand() as SqlCommand;

            var dataAdapter = new SqlDataAdapter();
            dataAdapter.TableMappings.Add(TableName, TableName);

            command.CommandText =
                $"SELECT {(count.HasValue ? "TOP " + count.Value : string.Empty)} * " +
                $"FROM {TableName} {(where != null ? "WHERE " + where : string.Empty)}";
            command.CommandType = CommandType.Text;
            dataAdapter.SelectCommand = command;
            command.Transaction = _unitOfWork.GetTransaction() as SqlTransaction;
            if(parameters != null){
                command.AddParameters(parameters);
            }

            var dataSet = new DataSet(TableName);
            dataAdapter.Fill(dataSet);

            var rows = dataSet.Tables[TableIndexName].Select();

            var resultEntities = new List<TEntity>();
            foreach (var row in rows)
            {
                var resultEntity = new TEntity();
                foreach (var column in GetColumns(resultEntity))
                {
                    var propertyValue = row[column];
                    resultEntity.GetType().GetProperty(column)?.SetValue(resultEntity, propertyValue);
                }

                resultEntities.Add(resultEntity);
            }

            return resultEntities;
        }

        private void ExecuteUpdate(Action<DataSet, SqlDataAdapter> updateAction, string where = null, int? count = null, object parameters = null)
        {
            using var command = _unitOfWork.GetConnection().CreateCommand() as DbCommand;

            var dataAdapter = new SqlDataAdapter();

            dataAdapter.TableMappings.Add(TableName, TableName);

            command.CommandText =
                $"SELECT {(count.HasValue ? "TOP " + count.Value : string.Empty)} * " +
                $"FROM {TableName} {(where != null ? "WHERE" + where : string.Empty)}";
            command.CommandType = CommandType.Text;
            command.Transaction = _unitOfWork.GetTransaction() as DbTransaction;
            if (parameters != null)
            {
                command.AddParameters(parameters);
            }

            dataAdapter.SelectCommand = command as SqlCommand;

            var dataSet = new DataSet(TableName);
            dataAdapter.Fill(dataSet);

            updateAction.Invoke(dataSet, dataAdapter);

            dataAdapter.Update(dataSet);
        }

        private void PrepareRow(TEntity entity, DataRow row)
        {
            var entityProperties = entity.GetType().GetProperties();
            foreach (var column in GetColumns(entity))
            {
                var propertyValue = entityProperties.FirstOrDefault(x => x.Name == column)
                    ?.GetValue(entity)
                    ?.ToString() ?? "";
                row[column] = propertyValue;
            }
        }

        private string[] GetColumns(TEntity entity)
        {
            return entity.GetType()
                .GetProperties()
                .Select(p => p.Name).ToArray();
        }
    }
}