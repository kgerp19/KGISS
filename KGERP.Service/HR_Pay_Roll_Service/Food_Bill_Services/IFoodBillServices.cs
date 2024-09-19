using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services
{
    public interface IFoodBillServices
    {
        Task<long> AddBill(FoodBillViewModel model);
        Task<FoodBillViewModel> BillList(int companyId);
        Task<long> AddBillDetalis(FoodBillDetaliesViewModel item);
        Task<FoodBillViewModel> Detalis(long id);
        Task<long> Delete(long id);
        Task<long> UpdateBillDetalis(FoodBillDetaliesViewModel model);
        Task<List<EmployeeInfoVM>> employees(int companyId);
        Task<List<EmployeeInfoVM>> AllEmployees(int companyId);
        Task<List<EmployeeInfoVMForFine>> AllEmployeesForFine(int companyId);
        Task<List<EmployeeInfoVM>> AllEmployeesfoConfiguration(int companyId);
    }
}
