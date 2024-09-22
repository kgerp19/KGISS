using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KGERP.Service.ServiceModel;

namespace KGERP.Service.Interface
{
    public interface IRentProductionService
    {
        Task<RentProductionModel> GetRentProductions(int companyId, DateTime? fromDate, DateTime? toDate);
        RentProductionModel GetRentProduction(int rentProductionId);
        bool SaveRentProduction(int rentProductionId, RentProductionModel model, out string message);
    }
}
