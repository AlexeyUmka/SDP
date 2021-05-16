namespace DAL.Interfaces
{
    public interface IDbReaderMapperFactory
    {
        IDbReaderMapper<T> GetMapper<T>() where T:class, new();
    }
}