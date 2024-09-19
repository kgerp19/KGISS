using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public interface ICostHeadsService
    {
        Task<List<BookingHeadServiceModel>> GetCostHeadsByCompanyId(int companyId);
        Task<BookingCostHead> GetCostHeadsById(int id);
        Task<bool> AddCostHeads(BookingHeadInsertModel model);
        Task<bool> UpdateCostHeads(BookingHeadEditModel model);
        Task<bool> DeleteCostHeads(int id);
        List<SelectDDLModel> CostHeadsList(int companyId);
        Task<List<SelectModelInstallmentType>> GetBookingInstallmentType();
        Task<List<SelectModelInstallmentType>> GetCompanyBookingInstallmentType(int companyId);
    }
}
