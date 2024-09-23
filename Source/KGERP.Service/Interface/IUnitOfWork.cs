using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGeneralRepository<T> GeneralRepository<T>() where T : class;
        int Complete();

        Task<int> CompleteAsync();

    }
}
