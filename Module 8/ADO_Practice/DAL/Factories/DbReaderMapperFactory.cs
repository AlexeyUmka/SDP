using DAL.Interfaces;
using DAL.Mappers;

namespace DAL.Factories
{
    public class DbReaderMapperFactory : IDbReaderMapperFactory
    {
        public IDbReaderMapper<T> GetMapper<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return new DefaultDbReaderMapper<T>();
            }
        }
    }
}