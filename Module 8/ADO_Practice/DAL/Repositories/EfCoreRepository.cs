using System.Collections.Generic;
using System.Linq;
using DAL.Contexts;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;


namespace DAL.Repositories
{
    public class EfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly DbSet<TEntity> _entities;

        private readonly DbContext _dbContext;

        public EfCoreRepository(IConfigurationRoot configurationRoot)
        {
            _dbContext = new DatabaseContext(configurationRoot.GetConnectionString("SqlDeliveryDB"));
            _entities = _dbContext.Set<TEntity>();
        }
        
        public void Insert(TEntity entity)
        {
            _entities.Add(entity);
            _dbContext.SaveChanges();

            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.State = EntityState.Detached);
        }
        
        public IEnumerable<TEntity> GetByKey(object key)
        {
            return new List<TEntity>(){_entities.Find(key)};
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
            _dbContext.RemoveRange(entity);
            _dbContext.SaveChanges();
        }
    }
}