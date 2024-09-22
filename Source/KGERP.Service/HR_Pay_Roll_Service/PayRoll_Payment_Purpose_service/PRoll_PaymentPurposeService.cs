using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.ParollServices;
using KGERP.Service.HR_Pay_Roll_Service.PayRoll_Payment_Purpose_service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PayRoll_Payment_Purpose_service
{
    public class PRoll_PaymentPurposeService : IPaymentPurposeServices
    {
        ERPEntities context = new ERPEntities();

        public async Task<PRoll_PaymentPurposeVm> GetPurpose()
        {
            PRoll_PaymentPurposeVm result = new PRoll_PaymentPurposeVm();

            try
            {
                result.DataList = await context.PRoll_PaymentPurpose.Where(x=>x.IsActive==true)
                    .OrderByDescending(t1 => t1.PaymentPurposeId)
                    .Select(t1 => new PRoll_PaymentPurposeVm
                    {
                        Name = t1.Name,
                        PaymentPurposeId = t1.PaymentPurposeId,
                        CreatedDate = t1.CreatedDate,
                        CreatedBy = t1.CreatedBy
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"Error in GetPurpose: {ex.Message}");
            }

            return result;
        }
        public async Task<long> AddPurpose(PRoll_PaymentPurposeVm model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_PaymentPurpose PaymentPurpose = new PRoll_PaymentPurpose();
                    PaymentPurpose.NameInBangla = model.NameInBangla;
                    PaymentPurpose.Name = model.Name;
                    PaymentPurpose.IsActive = true;
                    PaymentPurpose.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    PaymentPurpose.CreatedDate = DateTime.Now;
                    context.PRoll_PaymentPurpose.Add(PaymentPurpose);
                    context.SaveChanges();

                    scope.Commit();
                    return PaymentPurpose.PaymentPurposeId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }
        public async Task<PRoll_PaymentPurposeVm> EditPayPurpose(int PayPurposeId)
        {
            PRoll_PaymentPurposeVm model = new PRoll_PaymentPurposeVm();
            try
            {
                var existingPupose = await context.PRoll_PaymentPurpose.FindAsync(PayPurposeId);

                if (existingPupose == null)
                {
                    return model;
                }
                model = new PRoll_PaymentPurposeVm
                {    PaymentPurposeId = existingPupose.PaymentPurposeId,
                    Name = existingPupose.Name,
                    NameInBangla = existingPupose.NameInBangla

                };
            }
            catch (Exception ex)
            {

            }
            return model;

        }
        public async Task<PRoll_PaymentPurpose> UpdatePayPurpose(PRoll_PaymentPurposeVm model)
        {
            PRoll_PaymentPurpose vm = new PRoll_PaymentPurpose();
            vm = await context.PRoll_PaymentPurpose.FindAsync(model.PaymentPurposeId);
            vm.Name = model.Name;
            vm.NameInBangla = model.NameInBangla;
            context.Entry(vm).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return vm;
        }
        public async Task<PRoll_PaymentPurpose> DeletePurpose(int PayPurposeId)
        {

            var obj = await context.PRoll_PaymentPurpose.Where(x => x.PaymentPurposeId == PayPurposeId).FirstOrDefaultAsync();
            if (obj != null)
            {
                obj.IsActive = false;
                context.SaveChangesAsync();

            }
            return obj;
        }

    }
}
