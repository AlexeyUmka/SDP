using System.Collections.Generic;
using System.Linq;
using DAL.Contexts;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DAL.Repositories
{
    public class EfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly DbSet<TEntity> _entities;

        private readonly DatabaseContext _dbContext;

        public EfCoreRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _entities = _dbContext.Set<TEntity>();
        }
        
        public void Insert(TEntity entity)
        {
            _entities.Add(entity);
            _dbContext.SaveChanges();

            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Detached);
        }
        
        public TEntity GetByKey(object key)
        {
            return _entities.Find(key);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entities.AsNoTracking().ToList();
        }
        
        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        
        public void Delete(object key)
        {
            var entity = GetByKey(key);
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}