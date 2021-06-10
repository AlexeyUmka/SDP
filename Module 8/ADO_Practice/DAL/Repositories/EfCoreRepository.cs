using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public async Task Insert(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
        
        public async Task<TEntity> GetByKey(object key)
        {
            var entity = await _entities.FindAsync(key);
            _dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public Task<IEnumerable<TEntity>> GetAll()
        {
            return Task.FromResult(_entities.AsNoTracking().AsEnumerable());
        }
        
        public async Task Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
        
        public async Task Delete(object key)
        {
            var entity = await GetByKey(key);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}