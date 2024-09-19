using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment
{
    public class PRoll_CashPaymentService : IPRoll_CashPayment
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddCashPayment(PRoll_CashPaymentViewModel model)
        {
            PRoll_CashPayment pRoll_CashPayment = new PRoll_CashPayment();
            pRoll_CashPayment.CompanyId = model.CompanyId;
            pRoll_CashPayment.EmployeeId = model.EmployeeId;
            pRoll_CashPayment.Month = model.Month;
            pRoll_CashPayment.Year = model.Year;
            pRoll_CashPayment.Amount = model.Amount;
            pRoll_CashPayment.IsActive = true;
            pRoll_CashPayment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pRoll_CashPayment.CreatedDate = DateTime.Now;
            context.PRoll_CashPayment.Add(pRoll_CashPayment);
            var res = await context.SaveChangesAsync();
            if (res > 0)
            {
                return pRoll_CashPayment.CashPaymentId;
            }
            return 0;
        }

        public async Task<PRoll_CashPaymentViewModel> CashPaymentList(int companyId)
        {
            PRoll_CashPaymentViewModel pRoll = new PRoll_CashPaymentViewModel();
            pRoll.CompanyId = companyId;
            pRoll.CashPaymentList = await Task.Run(() => (from t1 in context.PRoll_CashPayment
                                                          join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                          join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                                          where t1.IsActive == true && t1.CompanyId == companyId
                                                          select new PRoll_CashPaymentViewModel
                                                          {
                                                              CashPaymentId = t1.CashPaymentId,
                                                              CompanyId = t2.CompanyId,
                                                              CompanyName = t2.Name,
                                                              EmployeeId = t1.EmployeeId,
                                                              EmployeeIdKG = t3.EmployeeId,
                                                              EmployeeName = t3.Name,
                                                              Year = t1.Year,
                                                              Month = t1.Month,
                                                              Amount = t1.Amount,
                                                              CreatedDate = t1.CreatedDate,
                                                              CreatedBy = t1.CreatedBy
                                                          }).OrderByDescending(f => f.CashPaymentId).AsQueryable());
            return pRoll;
        }

        public async Task<long> Delete(long id)
        {
            var result = await context.PRoll_CashPayment.FirstOrDefaultAsync(f => f.CashPaymentId == id);
            result.IsActive = false;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return 1;
        }

        public Task<PRoll_CashPaymentViewModel> Detalis(long id)
        {
            throw new NotImplementedException();
        }
    }
}
