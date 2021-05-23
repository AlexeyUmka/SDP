using System;
using System.Data.Common;
using System.Linq;
using DAL.Interfaces;

namespace DAL.Mappers
{
    public class DefaultDbReaderMapper<T> : IDbReaderMapper<T> where T:class, new()
    {
        public T Map(DbDataReader reader)
        {
            var result = new T();
            var properties = typeof(T).GetProperties().Select((p, index) => new {value = p, index = index}).ToList();
            if (properties.Count != reader.FieldCount)
                throw new NotSupportedException();
            foreach (var property in properties)
            {
                var genericMethod = typeof(DbDataReader).GetMethod("GetFieldValue").MakeGenericMethod(property.value.PropertyType);
                property.value.SetValue(result, genericMethod.Invoke(reader, new object[] {property.index}));
            }
            return result;
        }
    }
}