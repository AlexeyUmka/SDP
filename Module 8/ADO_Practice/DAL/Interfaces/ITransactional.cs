using System.Data;

namespace DAL.Interfaces
{
    public interface ITransactional
    {
        IDbTransaction GetCurrentTransaction();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}