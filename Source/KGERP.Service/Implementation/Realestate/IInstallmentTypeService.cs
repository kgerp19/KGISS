using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public interface IInstallmentTypeService
    {
        Task<bool> AddInstallmentType(InstallmentTypeInsertModel model);
        Task<List<BookingInstallmentType>> GetAllInstallmentTypesByCompanyId(int companyId);
        Task<BookingInstallmentType> GetInstallmentTypeById(int id);
        Task<bool> UpdateInstallmentType(InstallmentTypeEditModel model);
        Task<bool> DeleteInstallmentType(int id);
    }
}
