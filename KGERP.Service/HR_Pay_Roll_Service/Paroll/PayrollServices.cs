using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.ParollServices
{
    public class PayrollServices : IPayrollServices
    {
        ERPEntities context = new ERPEntities();

        public async Task<VmPRollPayRoll> PayRollByCompanyId(int companyId)
        {
            VmPRollPayRoll mobile = new VmPRollPayRoll();
            mobile.CompanyId = companyId;
            mobile.DataList = await Task.Run(() => (from t1 in context.PRoll_PayRoll
                                                    join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                    where t1.IsActive == true && t1.CompanyId == companyId
                                                    select new VmPRollPayRoll
                                                    {
                                                        PRollApprovalId = context.PRollApprovals.Where(x => x.Month == t1.Month && x.Year == t1.Year && x.IsSubmitted  && x.CompanyId == t1.CompanyId && x.IsFestivalBonus == t1.IsFestivalBonus && x.FinalStatus != 4).Select(x => x.PRollApprovalId ).DefaultIfEmpty(0).FirstOrDefault(),
                                                        PayRollId = t1.PayRollId,
                                                        IsActive = t1.IsActive,
                                                        IsClose = t1.IsClose,
                                                        IsTest = t1.IsTest,
                                                        Note = t1.Note ,
                                                        FestivalDate = t1.FestivalDate,
                                                        IsFestivalBonus = t1.IsFestivalBonus,
                                                        CompanyId = t1.CompanyId,
                                                        Year = t1.Year,
                                                        Month = t1.Month,
                                                        CreatedDate = t1.CreatedDate,
                                                        CreatedBy = t1.CreatedBy
                                                       
                                                    }).OrderByDescending(f => f.PayRollId).AsQueryable());
            return mobile;
        }


        public async Task<long> AddPayRoll(VmPRollPayRollDetail model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_PayRoll prollPayRoll = new PRoll_PayRoll();
                    prollPayRoll.CompanyId = model.CompanyId;
                    prollPayRoll.Month = model.Month;
                    prollPayRoll.Year = model.Year;
                    prollPayRoll.IsActive = true;
                    prollPayRoll.Note = model.Note;
                    if (model.IsFestivalBonus)
                    {
                        prollPayRoll.IsFestivalBonus = model.IsFestivalBonus;
                        prollPayRoll.FestivalDate = Convert.ToDateTime(model.FestivalDateStr);
                    }

                    prollPayRoll.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    prollPayRoll.CreatedDate = DateTime.Now;
                    context.PRoll_PayRoll.Add(prollPayRoll);
                    context.SaveChanges();


                    scope.Commit();
                    return prollPayRoll.PayRollId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }
        public int ProcessPayroll(int CompanyId, long PayRollId, int Month, int Year)
        {
            PRoll_PayRoll payRoll = context.PRoll_PayRoll.Find(PayRollId);
            try
            {
                if (!payRoll.IsFestivalBonus)
                {
                    int result = context.Database.ExecuteSqlCommand("exec SalaryGenerateByPayRollId {0},{1},{2},{3}", CompanyId, PayRollId, Month, Year);

                }
                else
                {
                    int result = context.Database.ExecuteSqlCommand("exec BonusGenerateByPayRollId {0},{1},{2},{3}", CompanyId, PayRollId, Month, Year);

                }

                //if (!payRoll.IsClose)
                //{
                //}
            }
            catch (DbEntityValidationException e)
            {
                throw;
            }

            return 0;
        }


        public async Task<VmSalaryDetails> CompanyWiseSalaryDetails(int companyId, long PayRollId)
        {
            VmSalaryDetails vmSalaryDetails = new VmSalaryDetails();
            int index = 0;
            vmSalaryDetails.CompanyId = companyId;
            vmSalaryDetails.PayRollId = PayRollId;
            vmSalaryDetails.SalaryList = await Task.Run(() => context.Database.SqlQuery<VmSalaryDetails>("Exec GetCompanyWiseSalarySheet {0}, {1}", companyId, PayRollId).OrderByDescending(x => x.GrossSalary).ToList());
            foreach (VmSalaryDetails item in vmSalaryDetails.SalaryList)
            {
                item.Index = index++;
            }
            return vmSalaryDetails;
        }

        public async Task<VmSalaryDetails> CompanyWiseBonusDetails(int companyId, long PayRollId)
        {
            VmSalaryDetails vmSalaryDetails = new VmSalaryDetails();
            int index = 0;
            vmSalaryDetails.CompanyId = companyId;
            vmSalaryDetails.PayRollId = PayRollId;
            vmSalaryDetails.SalaryList = await Task.Run(() => context.Database.SqlQuery<VmSalaryDetails>("Exec GetCompanyWiseBonusSheet {0}, {1}", companyId, PayRollId).OrderByDescending(x => x.GrossSalary).ToList());
            foreach (VmSalaryDetails item in vmSalaryDetails.SalaryList)
            {
                item.Index = index++;
            }
            return vmSalaryDetails;
        }

        public async Task<long> ProcessBankSheetAndPayment(VmSalaryDetails model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var bankSheetData = model.SalaryList.Where(x => x.BankSheetFlag == true).ToList();
                    var EmpPaymentData = model.SalaryList.Where(x => x.EmpPaymentFlag == true).ToList();

                   
                    List<PRoll_PayRollDetail> PayRollDetailsList = new List<PRoll_PayRollDetail>();
                    foreach (var item in bankSheetData)
                    {
                        PRoll_PayRollDetail rollDetail = new PRoll_PayRollDetail
                        {
                            EmployeeId = item.Id,
                            PayRollId = item.PayRollId,
                            PaymentPurposeId = 18,
                            CalculationType = 4,
                            Amount = 0,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };
                        PayRollDetailsList.Add(rollDetail);

                    }

                    foreach (var item in EmpPaymentData)
                    {
                        PRoll_PayRollDetail rollDetail = new PRoll_PayRollDetail
                        {
                            EmployeeId = item.Id,
                            PayRollId = item.PayRollId,
                            PaymentPurposeId = 18,
                            CalculationType = 4,
                            Amount = 0,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };
                        PayRollDetailsList.Add(rollDetail);

                    }                     
                    context.PRoll_PayRollDetail.AddRange(PayRollDetailsList);
                   await context.SaveChangesAsync();

                    scope.Commit();
                    return model.PayRollId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }


        public async Task<PRoll_BonusVm> PayRollBonus(int companyId)
        {
            PRoll_BonusVm mobile = new PRoll_BonusVm();
            mobile.CompanyId = companyId;
            mobile.DataListDetail = await Task.Run(() => (from t1 in context.PRoll_Bonus
                                                    join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                    where t1.IsActive == true && t1.CompanyId == companyId
                                                    select new PRoll_BonusVm
                                                    {
                                                        BonusId = t1.BonusId,
                                                        IsActive = t1.IsActive,
                                                        IsClose = t1.IsClose,
                                                        Status = t1.Status,
                                                        BonusTitle = t1.BonusTitle,
                                                        CompanyId = t1.CompanyId,
                                                        Year = t1.BonusYear,
                                                        Month = t1.BonusMonth,
                                                        CreatedDate = t1.CreatedDate,
                                                        CreatedBy = t1.CreatedBy
                                                    }).OrderByDescending(f => f.BonusId).AsQueryable());
            return mobile;
        }



        public async Task<PRoll_AdvanceTypeVm> PayRollAdvanceType(int companyId)
        {
            PRoll_AdvanceTypeVm model = new PRoll_AdvanceTypeVm();
            model.CompanyId = companyId;
            model.DataList = await Task.Run(() => (from t1 in context.Proll_AdvanceeType
                                                    where t1.IsActive == true && t1.CompanyId == companyId
                                                    select new PRoll_AdvanceTypeVm
                                                    {
                                                        AdvanceTypeId = t1.AdvanceTypeId,
                                                        AdvanceName = t1.AdvanceName,
                                                        CreatedBy = t1.CreatedBy,
                                                        CompanyId=t1.CompanyId
                                                    }).OrderByDescending(f => f.AdvanceTypeId).ToListAsync());
            return model;
        }

        public async Task<PRoll_AdvanceTypeVm> Edit(int? Advanceid)
        {
            PRoll_AdvanceTypeVm model = new PRoll_AdvanceTypeVm();
            model = await Task.Run(() => (from t1 in context.Proll_AdvanceeType
                                                   where t1.IsActive == true && t1.AdvanceTypeId == Advanceid
                                                   select new PRoll_AdvanceTypeVm
                                                   {
                                                       AdvanceTypeId = t1.AdvanceTypeId,
                                                       AdvanceName = t1.AdvanceName,
                                                       CreatedBy = t1.CreatedBy,
                                                       CompanyId=t1.CompanyId
                                                   }).OrderByDescending(f => f.AdvanceTypeId).FirstOrDefaultAsync());
            return model;
        }

        public async Task<int> AddPayRollAdvanceType(PRoll_AdvanceTypeVm model)
        {

            var exit = context.Proll_AdvanceeType.Where(x=>x.AdvanceTypeId == model.AdvanceTypeId).FirstOrDefault();

            if (exit == null)
            {
                using (var scope = context.Database.BeginTransaction())
                {
                    try
                    {
                        Proll_AdvanceeType prollPayRollAdvanceType = new Proll_AdvanceeType();
                        prollPayRollAdvanceType.CompanyId = model.CompanyId;
                        prollPayRollAdvanceType.AdvanceName = model.AdvanceName;
                        prollPayRollAdvanceType.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        prollPayRollAdvanceType.IsActive = true;
                        prollPayRollAdvanceType.CreatedDate = DateTime.Now;
                        context.Proll_AdvanceeType.Add(prollPayRollAdvanceType);
                        context.SaveChanges();

                        scope.Commit();
                        return prollPayRollAdvanceType.AdvanceTypeId;

                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return 0;
                    }
                }
            }
            else
            {
                using (var scope = context.Database.BeginTransaction())
                {
                    try
                    {
                      
                     
                        exit.AdvanceName = model.AdvanceName;
                        exit.ModifyBy = System.Web.HttpContext.Current.User.Identity.Name;
                        exit.ModifyDate = DateTime.Now;
                        context.SaveChanges();

                        scope.Commit();
                        return exit.AdvanceTypeId;

                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return 0;
                    }
                }
            }


          
        }






        public async Task<long> AddPayRollBonus(PRoll_BonusVm model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PRoll_Bonus prollPayRoll = new PRoll_Bonus();
                    prollPayRoll.CompanyId = model.CompanyId;
                    prollPayRoll.BonusMonth = model.Month;
                    prollPayRoll.BonusYear = model.Year;
                    prollPayRoll.IsActive = true;
                    prollPayRoll.BonusTitle = model.BonusTitle;
                    prollPayRoll.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    prollPayRoll.CreatedDate = DateTime.Now;
                    context.PRoll_Bonus.Add(prollPayRoll);
                    context.SaveChanges();

                    scope.Commit();
                    return prollPayRoll.BonusId;

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }

        public async Task<PRoll_Bonus> DeleteBonus(long bonusid)
        {

            var obj = await context.PRoll_Bonus.Where(x => x.BonusId==bonusid).FirstOrDefaultAsync();
            if (obj != null)
            {
                obj.IsActive = false;
                context.SaveChangesAsync();

            }
            return obj;
        }
        public async Task<PRoll_BonusVm> EditPayRollBonus(long bonusid)
        {
            PRoll_BonusVm model= new PRoll_BonusVm();
            try
            {
                var existingBonus = await context.PRoll_Bonus.FindAsync(bonusid);

                if (existingBonus == null)
                {
                    return model;
                }
                 model = new PRoll_BonusVm
                {
                    BonusId = existingBonus.BonusId,
                    CompanyId = existingBonus.CompanyId,
                    Month = existingBonus.BonusMonth,
                    Year = existingBonus.BonusYear,
                    BonusTitle = existingBonus.BonusTitle
         
                };
            }
            catch (Exception ex)
            {
                
            }
            return model;

        }


        public async Task<PRoll_Bonus> UpdatePayRollBonus(PRoll_BonusVm model)
        {
            PRoll_Bonus vm = new PRoll_Bonus();
                vm = await context.PRoll_Bonus.FindAsync(model.BonusId);
                vm.CompanyId = model.CompanyId;
                vm.BonusMonth = model.Month;
                vm.BonusYear = model.Year;
                vm.BonusTitle = model.BonusTitle;
                  context.Entry(vm).State = EntityState.Modified;
               await context.SaveChangesAsync();
                return vm;


      
          
        }




        public async Task<bool> DeleteAdvanceType (int AdvanceTypeId)
        {

            var obj= await context.Proll_AdvanceeType.Where(x=>x.AdvanceTypeId==AdvanceTypeId).FirstOrDefaultAsync();

            if(obj != null)
            {
                obj.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }


            return false;

        }










    }
}
