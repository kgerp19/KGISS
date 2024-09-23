using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public int Complete() => _context.SaveChanges();

        public async Task<int> CompleteAsync()
        {
            try
            {
                
                // Await the asynchronous save operation
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

        }

        public void Dispose() => _context.Dispose();

        public IGeneralRepository<T> GeneralRepository<T>() where T : class
        {
            return new GeneralRepository<T>(_context);
        }


    }
}
