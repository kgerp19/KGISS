using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service
{
    public interface IMobileBillServices
    {
        Task<long> AddBill(MoobileBillViewModel model);
        Task<MoobileBillViewModel> BillList(int companyId);
        Task<long> AddBillDetalis(MobileBillDetaliesViewModel item);
        Task<MoobileBillViewModel> Detalis(long id);
        Task<long> Delete(long id);
        Task<long> UpdateBillDetalis(MobileBillDetaliesViewModel model);
        Task<List<EmployeeInfoVM>> employees(int companyId);
        List<EmployeeInfoVM> GetMobileBills(int companyId, int? year, int? month);
    }
}
