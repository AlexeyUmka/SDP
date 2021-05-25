using System;
using System.Data.Common;

namespace DAL.Interfaces
{
    public interface IDbReaderMapper<T> where T:class
    {
        T Map(DbDataReader reader);
    }
}