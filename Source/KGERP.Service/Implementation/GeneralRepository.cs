using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class GeneralRepository<T>: IGeneralRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T Get(int id) => _dbSet.Find(id);

        public IEnumerable<T> GetAll() => _dbSet.ToList();

        public void Add(T entity) => _dbSet.Add(entity);

        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null) _dbSet.Remove(entity);
        }


        public Task<T> GetAsync(int id) => Task.FromResult(_dbSet.Find(id));

        public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult(_dbSet.ToList() as IEnumerable<T>);

        public Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            return Task.CompletedTask; // Marking task as completed
        }

        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask; // Marking task as completed
        }

        public Task DeleteAsync(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
                _dbSet.Remove(entity);
            return Task.CompletedTask; // Marking task as completed
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities); // Synchronous call
            return Task.CompletedTask; // Mark as completed
        }

        public Task AddRangeListAsync(List<T> entities) // Accepting List<T>
        {
            _dbSet.AddRange(entities); // Synchronous call
            return Task.CompletedTask; // Mark as completed
        }

    }
}
