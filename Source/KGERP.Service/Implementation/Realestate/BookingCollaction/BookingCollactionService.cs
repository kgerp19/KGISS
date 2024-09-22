using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate.BookingCollaction
{
    public class BookingCollactionService : IBookingCollaction
    {
        ERPEntities context = new ERPEntities();

        private readonly AccountingService _accountingService;
        public BookingCollactionService(AccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        public async Task<CollactionBillViewModel> CustomerBookingView(CollactionBillViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {

                    int result = -1;
                    var bookinginfo = context.ProductBookingInfoes.FirstOrDefault(g => g.BookingId == model.BookingId);
                    var groupinfo = context.CustomerGroupInfoes.FirstOrDefault(g => g.CGId == model.CGId);

                    #region Payment ID
                    int paymentMastersCount = context.PaymentMasters.Where(x => x.CompanyId == model.CompanyId && x.CGID == model.CGId).Count();

                    if (paymentMastersCount == 0)
                    {
                        paymentMastersCount = 1;
                    }
                    else
                    {
                        paymentMastersCount++;
                    }

                    string PaymentNo = "C-" + model.BookingNo + "-" + paymentMastersCount.ToString().PadLeft(4, '0');
                    #endregion

                    PaymentMaster paymentMaster = new PaymentMaster();

                    paymentMaster.PaymentNo = PaymentNo;
                    paymentMaster.TransactionDate = Convert.ToDateTime(model.TransactionDateString);
                    paymentMaster.VendorId = Convert.ToInt32(groupinfo.PrimaryClientId);
                    paymentMaster.ReferenceNo = model.ReferenceNo;
                    paymentMaster.BankChargeHeadGLId = model.CompanyId == (int)CompanyName.KrishibidPropertiesLimited ? 43242 : model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited ? 50602580 : 0;
                    paymentMaster.BankCharge = model.BankCharge;
                    paymentMaster.CompanyId = Convert.ToInt32(model.CompanyId);
                    paymentMaster.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    paymentMaster.PaymentToHeadGLId = model.Accounting_BankOrCashId;
                    paymentMaster.CreatedDate = DateTime.Now;
                    paymentMaster.IsActive = true;
                    paymentMaster.VendorTypeId = 0;
                    paymentMaster.IsFinalized = false;
                    paymentMaster.CGID = model.CGId;
                    paymentMaster.MoneyReceiptNo = model.MoneyReceiptNo;

                    context.PaymentMasters.Add(paymentMaster);
                    context.SaveChanges();
                    if (model.CollactionType == 1)
                    {
                        List<Payment> payments = new List<Payment>();
                        payments = mappayments(model, paymentMaster.PaymentMasterId, (int)groupinfo.PrimaryClientId);
                        context.Payments.AddRange(payments);
                        context.SaveChanges();
                        foreach (var item in model.ScheduleVM)
                        {
                            BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.InstallmentId);
                            if (item.PaidAmount + schedule.PaidAmount == schedule.Amount)
                            {
                                schedule.PaidAmount = item.PaidAmount;
                                schedule.IsPaid = true;
                                schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                schedule.ModifiedDate = DateTime.Now;
                            }
                            else
                            {
                                schedule.PaidAmount = schedule.PaidAmount + item.PaidAmount;
                                schedule.IsPaid = false;
                                schedule.IsPartlyPaid = true;
                                schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                schedule.ModifiedDate = DateTime.Now;
                            }
                            context.Entry(schedule).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }

                    if (model.CollactionType == 3)
                    {
                        List<Payment> payments = new List<Payment>();
                        payments = mapFullpayments(model, paymentMaster.PaymentMasterId, (int)groupinfo.PrimaryClientId);
                        context.Payments.AddRange(payments);
                        context.SaveChanges();
                        var list = context.BookingInstallmentSchedules.Where(g => g.CGID == model.CGId && g.IsPaid == false);
                        foreach (var item in list)
                        {
                            BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.InstallmentId);
                            schedule.PaidAmount = item.Amount;
                            schedule.IsPaid = true;
                            schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            schedule.ModifiedDate = DateTime.Now;
                            context.Entry(schedule).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }

                    if (model.CollactionType == 2)
                    {
                        decimal amount = (decimal)model.AdjustmentAmount;
                        var list = context.BookingInstallmentSchedules.Where(h => h.IsPaid == false && h.IsActive && h.CGID == model.CGId).ToList();
                        foreach (var item in list)
                        {
                            decimal pay = 0;
                            decimal Installmentpay = 0;
                            if (amount != 0)
                            {
                                if (item.IsPartlyPaid == true)
                                {
                                    pay = item.Amount - item.PaidAmount;
                                    BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.InstallmentId);
                                    if (amount >= pay)
                                    {
                                        schedule.PaidAmount = schedule.PaidAmount + pay;
                                        schedule.IsPaid = true;
                                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        schedule.ModifiedDate = DateTime.Now;
                                        Installmentpay = pay;
                                    }
                                    else
                                    {
                                        schedule.PaidAmount = schedule.PaidAmount + pay;
                                        schedule.IsPaid = false;
                                        schedule.IsPartlyPaid = true;
                                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        schedule.ModifiedDate = DateTime.Now;
                                        Installmentpay = pay;
                                    }
                                    amount = amount - pay;
                                    context.Entry(schedule).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.InstallmentId);
                                    if (amount >= item.Amount)
                                    {
                                        schedule.PaidAmount = item.Amount;
                                        schedule.IsPaid = true;
                                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        schedule.ModifiedDate = DateTime.Now;
                                        amount = amount - item.Amount;
                                        Installmentpay = item.Amount;

                                    }
                                    else
                                    {
                                        schedule.PaidAmount = amount;
                                        schedule.IsPaid = false;
                                        schedule.IsPartlyPaid = true;
                                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                                        schedule.ModifiedDate = DateTime.Now;
                                        Installmentpay = amount;
                                        amount = 0;


                                    }

                                    context.Entry(schedule).State = EntityState.Modified;
                                    context.SaveChanges();
                                }

                                Payment payment = new Payment()
                                {
                                    PaymentMasterId = paymentMaster.PaymentMasterId,
                                    InAmount = Installmentpay,
                                    ProductType = model.CompanyId == 7 ? "F" : "P",
                                    ReferenceNo = model.ReferenceNo,
                                    TransactionDate = Convert.ToDateTime(model.TransactionDateString),
                                    VendorId = 3,
                                    CompanyId = Convert.ToInt32(model.CompanyId),
                                    PaymentFromHeadGLId = model.Accounting_BankOrCashParantId,
                                    //PaymentToHeadGLId = vmPayment.PaymentToHeadGLId,
                                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                    CreatedDate = DateTime.Now,
                                    IsActive = true,
                                    CGID = model.CGId,
                                    MoneyReceiptNo = model.MoneyReceiptNo,
                                    ChequeNo = model.ChequeNo,
                                    InstallmentId = item.InstallmentId,
                                };
                                context.Payments.Add(payment);
                                context.SaveChanges();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    scope.Commit();
                    model.PaymentMasterId = paymentMaster.PaymentMasterId;
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }

            }
        }

        private List<Payment> mappayments(CollactionBillViewModel model, int id, int clientId)
        {
            List<Payment> payments = new List<Payment>();
            foreach (var item in model.ScheduleVM)
            {
                Payment payment = new Payment()
                {
                    PaymentMasterId = id,
                    InAmount = item.PaidAmount,
                    ProductType = model.CompanyId == 7 ? "F" : "P",
                    ReferenceNo = model.ReferenceNo,
                    TransactionDate = Convert.ToDateTime(model.TransactionDateString),
                    VendorId = 3,
                    CompanyId = Convert.ToInt32(model.CompanyId),
                    PaymentFromHeadGLId = model.HeadGLId,
                    //PaymentToHeadGLId = vmPayment.PaymentToHeadGLId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    CGID = model.CGId,
                    MoneyReceiptNo = model.MoneyReceiptNo,
                    ChequeNo = model.ChequeNo,
                    InstallmentId = item.InstallmentId,
                };
                payments.Add(payment);

            }
            return payments;
        }

        private List<Payment> mapFullpayments(CollactionBillViewModel model, int id, int clientId)
        {
            var list = context.BookingInstallmentSchedules.Where(g => g.CGID == model.CGId && g.IsPaid == false);
            List<Payment> payments = new List<Payment>();
            foreach (var item in list)
            {
                Payment payment = new Payment()
                {
                    PaymentMasterId = id,
                    InAmount = item.IsPartlyPaid == true ? item.Amount - item.PaidAmount : item.Amount,
                    ProductType = model.CompanyId == 7 ? "F" : "P",
                    ReferenceNo = model.ReferenceNo,
                    TransactionDate = Convert.ToDateTime(model.TransactionDateString),
                    VendorId = 3,
                    CompanyId = Convert.ToInt32(model.CompanyId),
                    PaymentFromHeadGLId = model.Accounting_BankOrCashParantId,
                    //PaymentToHeadGLId = vmPayment.PaymentToHeadGLId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    CGID = model.CGId,
                    MoneyReceiptNo = model.MoneyReceiptNo,
                    ChequeNo = model.ChequeNo,
                    InstallmentId = item.InstallmentId,
                };
                payments.Add(payment);

            }
            return payments;
        }

        public async Task<CollactionBillViewModel> CollactionBookingView(int companyId, int paymentMasterId, long CGId)
        {
            CollactionBillViewModel model = new CollactionBillViewModel();
            model = await Task.Run(() => (from pm in context.PaymentMasters
                                          join PDetalis in context.Payments on pm.PaymentMasterId equals PDetalis.PaymentMasterId
                                          join ginfo in context.CustomerGroupInfoes on pm.CGID equals ginfo.CGId
                                          join PBooking in context.ProductBookingInfoes on pm.CGID equals PBooking.CGId
                                          join HGl in context.HeadGLs on pm.BankChargeHeadGLId equals HGl.Id
                                          join HG2 in context.HeadGLs on pm.PaymentToHeadGLId equals HG2.Id

                                          join t1 in context.Products on PBooking.ProductId equals t1.ProductId
                                          join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                          join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                          join t4 in context.VoucherMaps.Where(x => x.CompanyId == companyId && x.IntegratedFrom == "PaymentMaster" ) on pm.PaymentMasterId equals t4.IntegratedId into t4_Join
                                          from t4 in t4_Join.DefaultIfEmpty()
                                          where pm.PaymentMasterId == paymentMasterId && pm.CGID == CGId
                                          select new CollactionBillViewModel
                                          {
                                              IntegratedFrom = "PaymentMaster",
                                              ProductName = t3.Name + " " + t2.Name +" "+ t1.ProductName, 
                                              CGId = pm.CGID,
                                              VoucherId = t4 != null ? t4.VoucherId :0,
                                              PaymentMasterId = pm.PaymentMasterId,
                                              BookingId = PBooking.BookingId,
                                              BookingNo = PBooking.BookingNo,
                                              PaymentNo = pm.PaymentNo,
                                              CustomerGroupName = ginfo.GroupName,
                                              BankCharge = pm.BankCharge,
                                              ReferenceNo = pm.ReferenceNo,
                                              ChequeNo = PDetalis.ChequeNo,
                                              MoneyReceiptNo = pm.MoneyReceiptNo,
                                              TransactionDate = pm.TransactionDate,
                                              IsFinalized = pm.IsFinalized,
                                              CompanyId = pm.CompanyId,
                                              BankChargeName = HGl.AccCode + " - " + HGl.AccName,
                                              Accounting_BankOrCashId = pm.PaymentToHeadGLId,
                                              BankChargeHeadGLId = pm.BankChargeHeadGLId,
                                              BankCashHeadName = HG2.AccCode + " "+ HG2.AccName,
                                            
                                          }).FirstOrDefault());


            model.PaymentList = (from PDetalis in context.Payments
                                 join pm in context.PaymentMasters on PDetalis.PaymentMasterId equals pm.PaymentMasterId
                                 join ginfo in context.CustomerGroupInfoes on pm.CGID equals ginfo.CGId
                                 join Installment in context.BookingInstallmentSchedules on PDetalis.InstallmentId equals Installment.InstallmentId into join_Installment
                                 from t1 in join_Installment.DefaultIfEmpty()
                                 where PDetalis.PaymentMasterId == paymentMasterId && PDetalis.CGID == CGId
                                 select new InstallmentSchedulePayment
                                 {

                                     HeadGLId = ginfo.HeadGLId,
                                     CGId = pm.CGID,
                                     PaymentMasterId = PDetalis.PaymentMasterId,
                                     PaymentId = PDetalis.PaymentId,
                                     InAmount = PDetalis.InAmount,
                                     ChequeNo = PDetalis.ChequeNo,
                                     InstallmentId = PDetalis.InstallmentId,
                                     Title = t1.InstallmentTitle,
                                     InstallmentDate = t1.Date
                                 }).ToList();

            model.TotalInstallment = model.PaymentList.Select(d => d.InAmount).Sum();
            return model;
        }

        public async Task<CollactionBillViewModel> CollactionList(int companyId)
        {
            CollactionBillViewModel model = new CollactionBillViewModel();
            model.DataList = (from t1 in context.PaymentMasters.Where(x => x.CompanyId == companyId && x.IsActive)
                              join head in context.HeadGLs on t1.PaymentToHeadGLId equals head.Id into t4_join
                              from head in t4_join.DefaultIfEmpty()
                              join t2 in context.ProductBookingInfoes on t1.CGID equals t2.CGId
                              join t3 in context.CustomerGroupInfoes on t1.CGID equals t3.CGId
                              select new VMPaymentMaster
                              {
                                  CreatedBy = t1.CreatedBy,
                                  TransactionDate = t1.TransactionDate,
                                  PaymentNo = t1.PaymentNo,
                                  IsFinalized = t1.IsFinalized,
                                  CGId = t1.CGID,
                                  GroupName = t3.GroupName,
                                  PaymentMasterId = t1.PaymentMasterId,
                                  BankCharge = t1.BankCharge,
                                  MoneyReceiptNo = t1.MoneyReceiptNo,
                                  TotalAmount = 0,
                                  CompanyId = t1.CompanyId,
                                  BankChargeHeadGLId = t1.BankChargeHeadGLId,
                                  PaymentToHeadGLId = t1.PaymentToHeadGLId,
                                  PaymentFromHeadGLId=head.ParentId,
                              }).OrderByDescending(x => x.PaymentMasterId).ToList();

            foreach (var item in model.DataList)
            {
                var list = context.Payments.Where(f => f.PaymentMasterId == item.PaymentMasterId).ToList();
                var dataitem = list.FirstOrDefault(f => f.PaymentMasterId == item.PaymentMasterId);
                item.ChequeNo = dataitem.ChequeNo;
                item.ReferenceNo = dataitem.ReferenceNo;
                item.TotalAmount = list.Select(d => d.InAmount).Sum();

            }
            return model;
        }

        public async Task<CollactionBillViewModel> CollactionDelete(CollactionBillViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PaymentMaster master = await context.PaymentMasters.FirstOrDefaultAsync(d => d.PaymentMasterId == model.PaymentMasterId);
                    context.PaymentMasters.Remove(master);
                    context.SaveChanges();
                    List<Payment> payments = await context.Payments.Where(f => f.PaymentMasterId == model.PaymentMasterId).ToListAsync();
                    foreach (var item in payments)
                    {
                        BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.InstallmentId);
                        schedule.PaidAmount = schedule.PaidAmount - item.InAmount;
                        schedule.IsPaid = false;
                        context.Entry(schedule).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    context.Payments.RemoveRange(payments);
                    context.SaveChanges();
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<CollactionBillViewModel> NewCollaction(CollactionBillViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    decimal pay = 0;
                    var list = await context.BookingInstallmentSchedules.FirstOrDefaultAsync(g => g.InstallmentId == model.InstallmentId && g.IsPaid == false);
                    Payment payment = new Payment();
                    payment.PaymentMasterId = model.PaymentMasterId;
                    payment.InAmount = model.InAmount;
                    payment.ProductType = model.CompanyId == 7 ? "F" : "P";
                    payment.ReferenceNo = model.ReferenceNo;
                    payment.TransactionDate = (DateTime)model.TransactionDate;
                    payment.VendorId = 3;
                    payment.CompanyId = Convert.ToInt32(model.CompanyId);
                    payment.PaymentFromHeadGLId = model.Accounting_BankOrCashParantId;
                    //PaymentToHeadGLId = vmPayment.PaymentToHeadGLId,
                    payment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    payment.CreatedDate = DateTime.Now;
                    payment.IsActive = true;
                    payment.CGID = model.CGId;
                    payment.MoneyReceiptNo = model.MoneyReceiptNo;
                    payment.ChequeNo = model.ChequeNo;
                    payment.InstallmentId = list.InstallmentId;

                    context.Payments.Add(payment);
                    context.SaveChanges();

                    pay = list.Amount - list.PaidAmount;
                    BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == model.InstallmentId);

                    if (list.IsPartlyPaid == true)
                    {
                        if (model.InAmount >= pay)
                        {
                            schedule.PaidAmount = schedule.PaidAmount + pay;
                            schedule.IsPaid = true;
                            schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            schedule.ModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            schedule.PaidAmount = schedule.PaidAmount + model.InAmount;
                            schedule.IsPaid = false;
                            schedule.IsPartlyPaid = true;
                            schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            schedule.ModifiedDate = DateTime.Now;
                        }
                        context.Entry(schedule).State = EntityState.Modified;
                        context.SaveChanges();
                        scope.Commit();
                    }
                    else
                    {
                        if (model.InAmount >= pay)
                        {
                            schedule.PaidAmount = model.InAmount;
                            schedule.IsPaid = true;
                            schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            schedule.ModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            schedule.PaidAmount = model.InAmount;
                            schedule.IsPaid = false;
                            schedule.IsPartlyPaid = true;
                            schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            schedule.ModifiedDate = DateTime.Now;
                        }
                        context.Entry(schedule).State = EntityState.Modified;
                        context.SaveChanges();
                        scope.Commit();
                    }
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<CollactionBillViewModel> CollactionItemDelete(CollactionBillViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                 var payment = await context.Payments.FirstOrDefaultAsync(f => f.PaymentId == model.InstallmentId);
                 BookingInstallmentSchedule schedule = context.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == payment.InstallmentId);
                 schedule.PaidAmount = schedule.PaidAmount - payment.InAmount;
                 schedule.IsPaid = false;
                 context.Entry(schedule).State = EntityState.Modified;
                 context.SaveChanges();
                 context.Payments.Remove(payment);
                 context.SaveChanges();
                 scope.Commit();
                 return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<CollactionBillViewModel> CollactionUpdate(CollactionBillViewModel model)
        {
            try
            {
                PaymentMaster paymentMaster = await context.PaymentMasters.FirstOrDefaultAsync(d => d.PaymentMasterId == model.PaymentMasterId);
                paymentMaster.TransactionDate = Convert.ToDateTime(model.TransactionDateString);
                paymentMaster.ReferenceNo = model.ReferenceNo;
                paymentMaster.BankCharge = model.BankCharge;
                paymentMaster.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                paymentMaster.PaymentToHeadGLId = model.Accounting_BankOrCashId;
                paymentMaster.ModifiedDate = DateTime.Now;
                paymentMaster.MoneyReceiptNo = model.MoneyReceiptNo;
                paymentMaster.ReferenceNo = model.ReferenceNo;
                context.Entry(paymentMaster).State = EntityState.Modified;
                context.SaveChanges();
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        public async Task<long> CollactionStatusUpdate(long id)
        {
            try
            {
                if (id!=0)
                {
                    PaymentMaster paymentMaster = await context.PaymentMasters.SingleOrDefaultAsync(f => f.PaymentMasterId == id);
                    paymentMaster.IsFinalized = true;
                    context.Entry(paymentMaster).State = EntityState.Modified;
                    context.SaveChanges();
                    return id;
                }
                else
                {
                    return id;
                }
            }
            catch (Exception ex)
            {
                return id;
            }
          
        }

        public object GetBookingList(int companyId,string prefix)
        {

            return context.CustomerGroupInfoes.Where(x => x.CompanyId == companyId&&x.IsActive== true && x.GroupName.Contains(prefix)).Select(x => new
            {
                label = x.GroupName,
                val = x.CGId
            }).OrderBy(x => x.label).Take(100).ToList();
        }
    }
}
