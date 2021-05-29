using System.Data;
using System.Data.SqlClient;

namespace DAL.Extensions
{
    public static class CommandExtensions
    {
        public static void AddParameters<T>(this IDbCommand command, T entity)
        {
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                command.Parameters.Add(new SqlParameter(property.Name, property.GetValue(entity)));
            }
        }
    }
}