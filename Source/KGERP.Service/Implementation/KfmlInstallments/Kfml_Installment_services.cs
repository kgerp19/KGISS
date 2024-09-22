using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace KGERP.Service.Implementation.KfmlInstallments
{
    public class Kfml_Installment_services : IKfml_Installment
    {
        private bool disposed = false;
        ERPEntities context = new ERPEntities();

        public async Task<IKfml_InstallmentViewModel> AddInstallment(IKfml_InstallmentViewModel model)
        {

            using (var scope = context.Database.BeginTransaction())
            {
                try
                {

                    Vendor vendor = context.Vendors.FirstOrDefault(f => f.VendorId == model.ClientId);
                    //CommonBookingInfo
                    CommonBookingInfo BookingInfo = MapProductBooking(model);
                    context.CommonBookingInfoes.Add(BookingInfo);
                    context.SaveChanges();

                    //Booking Cost Mapping
                    List<CommonCostMapping> costMapping = GetCostMapping(model, BookingInfo.BookingId);
                    context.CommonCostMappings.AddRange(costMapping);
                    context.SaveChanges();

                    //Booking Installment Schedule ....... jishan......
                    List<CommonInstallmentSchedule> schedules = ConvertShortToDbModel(BookingInfo.BookingId, model.Schedule);
                    context.CommonInstallmentSchedules.AddRange(schedules);
                    context.SaveChanges();
                    scope.Commit();
                    model.BookingId = BookingInfo.BookingId;
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }

            }
        }

        private CommonBookingInfo MapProductBooking(IKfml_InstallmentViewModel model)
        {
            CommonBookingInfo pBooking = new CommonBookingInfo();
            pBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pBooking.CreatedDate = DateTime.Now;
            pBooking.ProductId = (int)model.ProductId;
            pBooking.VendorId = model.ClientId;
            pBooking.CompanyId = model.CompanyId;
            pBooking.BookingAmt = model.BookingMoney;
            pBooking.DiscountPercentage = model.Discount;
            pBooking.DeliveredQuantity = model.DeliveredQuantity;
            pBooking.InstallmentTypeId = model.BookingInstallmentTypeId == 0 ? model.BookingInstallmentTypeManualId : model.BookingInstallmentTypeId;
            pBooking.Status = 1;
            pBooking.EntryBy = model.EntryBy;
            pBooking.RestofAmount = model.RestofAmount;
            pBooking.ApplicationDate = Convert.ToDateTime(model.ApplicationDateString);
            pBooking.IsActive = true;
            //pBooking.BookingNo = model.BookingNo;
            pBooking.SpecialDiscountAmt = model.SpecialDiscountAmt;
            pBooking.BookingDate = Convert.ToDateTime(model.BookingDateString);
            pBooking.FileNo = model.FileNo;
            pBooking.ProductValue = model.ProductValue;
            pBooking.InstallmentAmount = model.InstallmentAmount;
            pBooking.Parcentage = model.InterestPercentage;
            pBooking.ParcentageAmount = model.InterestAmount;
            pBooking.COGSPrice = model.COGSPrice;   
            pBooking.SoldBy=model.EmployeeId;
            return pBooking;
        }
        private List<CommonInstallmentSchedule> ConvertShortToDbModel(long bookingId, List<SceduleInstallment> schedule)
        {
            var cout = schedule.Count();
            List<CommonInstallmentSchedule> model = new List<CommonInstallmentSchedule>();
            foreach (var sm in schedule)
            {
                sm.Date = Convert.ToDateTime(sm.StringDate);
                CommonInstallmentSchedule m = new CommonInstallmentSchedule()
                {
                    Amount = sm.Amount,
                    CommonBookingId = bookingId,
                    CreatedBy = "",
                    CreatedDate = DateTime.Now,
                    Date = sm.Date,
                    InstallmentId = 0,
                    IsActive = true,
                    IsLate = false,
                    IsPaid = false,
                    IsPartlyPaid = false,
                    PaidAmount = 0,
                    Remarks = "",
                    InstallmentTitle = sm.Title,
                    InstallmentTypeId = sm.Value
                };
                model.Add(m);
            }
            return model;
        }

        private List<CommonCostMapping> GetCostMapping(IKfml_InstallmentViewModel model, long bookingId)
        {
            List<CommonCostMapping> models = new List<CommonCostMapping>();
            foreach (var item in model.LstPurchaseCostHeads)
            {
                CommonCostMapping costMapping = new CommonCostMapping();
                costMapping.CommonBookingId = bookingId;
                costMapping.Amount = item.Amount;
                costMapping.CostId = item.CostId;
                costMapping.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                costMapping.CreatedDate = DateTime.Now;
                costMapping.IsActive = true;
                costMapping.IsSnstallmentInclude = item.IsSnstallmentInclude;
                costMapping.Percentage = item.Percentage;
                models.Add(costMapping);
            }
            return models;
        }

        public async Task<IKfml_InstallmentViewModel> InstallmentList(int CompanyId)
        {
            IKfml_InstallmentViewModel model = new IKfml_InstallmentViewModel();
            model.kfmldatalist = await Task.Run(() => (from t1 in context.CommonBookingInfoes
                                                       join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                       join t4 in context.Products.Where(f => f.IsActive == true) on t1.ProductId equals t4.ProductId
                                                       join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                       join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                                       where t1.CompanyId == CompanyId
                                                       select new IKfml_InstallmentViewModel
                                                       {
                                                           ClientId = t1.VendorId,
                                                           ClientName = t2.Name,
                                                           ClientPhone = t2.Phone,
                                                           ClientEmail = t2.Email,
                                                           ClientAddress = t2.Address,
                                                           FileNo = t1.FileNo,
                                                           InstallmentAmount = t1.InstallmentAmount,
                                                           BookingMoney = t1.BookingAmt,
                                                           RestofAmount = t1.RestofAmount,
                                                           ProductValue = t1.ProductValue,
                                                           ProductId = t1.ProductId,
                                                           SpecialDiscountAmt = t1.SpecialDiscountAmt,
                                                           IsSubmitted = t1.IsSubmitted,
                                                           InterestAmount = t1.InstallmentAmount,
                                                           InterestPercentage = t1.Parcentage,
                                                           ProductName = t4.ProductName,
                                                           ProductCategoryName = t5.Name,
                                                           ProductSubCategoryName = t6.Name,
                                                           BookingDate = t1.BookingDate,
                                                           BookingId = t1.BookingId,
                                                           CompanyId = t1.CompanyId,

                                                       }).ToListAsync());
            return model;

        }

        public async Task<IKfml_InstallmentViewModel> CustomerBookingView(int companyId, long bookingId)
        {
            IKfml_InstallmentViewModel model = new IKfml_InstallmentViewModel();
            model = await Task.Run(() => (from t1 in context.CommonBookingInfoes
                                          join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                          join t4 in context.Products.Where(f => f.IsActive == true) on t1.ProductId equals t4.ProductId
                                          join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                          join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                          where t1.CompanyId == companyId&&t1.BookingId == bookingId
                                          select new IKfml_InstallmentViewModel
                                          {
                                              CompanyId = t1.CompanyId,
                                              HeadGLId = t2.HeadGLId,
                                              ClientId = t1.VendorId,
                                              IntegratedFrom = "CommonBookingInfoes", 
                                              ClientName = t2.Name,
                                              ClientPhone = t2.Phone,
                                              ClientEmail = t2.Email,
                                              ClientAddress = t2.Address,
                                              FileNo = t1.FileNo,
                                              InstallmentAmount = t1.InstallmentAmount,
                                              BookingMoney = t1.BookingAmt,
                                              RestofAmount = t1.RestofAmount,
                                              ProductValue = t1.ProductValue,
                                              ProductId = t1.ProductId,
                                              Discount=t1.DiscountPercentage,
                                              SpecialDiscountAmt = t1.SpecialDiscountAmt,
                                              IsSubmitted = t1.IsSubmitted,
                                              InterestAmount = t1.ParcentageAmount,
                                              InterestPercentage = t1.Parcentage,
                                              ProductName = t4.ProductName,
                                              ProductCategoryName = t5.Name,
                                              ProductSubCategoryName = t6.Name,
                                              BookingDate = t1.BookingDate,
                                              BookingId = t1.BookingId,
                                              AccountingIncomeHeadId = t4.AccountingIncomeHeadId,
                                              AccountingStockHeadId = t4.AccountingHeadId,
                                              COGSPrice = t1.COGSPrice??0
                                          }).FirstOrDefaultAsync());
            model.LstPurchaseCostHeads = await Task.Run(() => (from t1 in context.CommonBookingInfoes.Where(x => x.BookingId == bookingId)
                                                               join t2 in context.BookingCostMappings on t1.BookingId equals t2.BookingId
                                                               join t3 in context.BookingCostHeads on t2.CostId equals t3.CostId
                                                               select new BookingHeadServiceModel
                                                               {
                                                                   CostId = t2.CostId,
                                                                   Amount = t2.Amount,
                                                                   BookingId = t2.BookingId,
                                                                   CostName = t3.CostName,
                                                               }).ToListAsync());
            decimal totalcost = model.LstPurchaseCostHeads.Select(d => d.Amount).Sum();
            model.TotalCost = totalcost;
            model.TotalAmount = model.ProductValue + totalcost;
            decimal TotalDiscount = ((model.ProductValue / 100m) * Convert.ToDecimal(model.Discount));
            model.TotalDiscount = (decimal)(TotalDiscount + model.SpecialDiscountAmt);
            model.GrandTotalAmount = model.TotalAmount - model.TotalDiscount;
            model.ScheduleVM = await context.CommonInstallmentSchedules.Where(f => f.CommonBookingId == bookingId).Select(o => new InstallmentScheduleShortModel
            {
                InstallmentId = o.InstallmentId,
                Title = o.InstallmentTitle,
                PayableAmount = o.Amount,
                InstallmentDate = o.Date,
                PaidAmount = o.PaidAmount,
                IsPaid = o.IsPaid,

            }).ToListAsync();
            model.InstallmentSumOfAmount = model.ScheduleVM.Sum(g => g.PayableAmount);
            return model;
        }
        public async Task<long> AccountingSalesPushKFML(int CompanyFK, IKfml_InstallmentViewModel bookingVM, int journalType)
        {
            
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "File No: " + bookingVM.FileNo + " " + bookingVM.BookingNo + " Date: " + bookingVM.BookingDate.ToString(),
                Narration = "Project: " + bookingVM.ProductCategoryName + " " + bookingVM.ProductSubCategoryName + " " + bookingVM.ProductName,
                CompanyFK = bookingVM.CompanyId,
                Date = bookingVM.BookingDate,
                IsSubmit = true,
                Accounting_CostCenterFK = bookingVM.AcCostCenterId
            };

            string perticular = bookingVM.FileNo + " " + bookingVM.ProductCategoryName + " " + bookingVM.ProductSubCategoryName + " " + bookingVM.ProductName ;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit =Convert.ToDouble( (bookingVM.RestofAmount + bookingVM.BookingMoney + bookingVM.InterestAmount)),
                Credit = 0,
                Accounting_HeadFK = bookingVM.HeadGLId.Value //Customer/ LC
            });
            
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Total Discount: " + Convert.ToDouble(bookingVM.TotalDiscount),
                Debit = Convert.ToDouble(bookingVM.TotalDiscount),
                Credit = 0,
                Accounting_HeadFK = 50604937 //Sale Discount 
            });



            //Sales
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = Convert.ToDouble((bookingVM.RestofAmount + bookingVM.BookingMoney  + bookingVM.TotalDiscount)),
                Accounting_HeadFK = (int)bookingVM.AccountingIncomeHeadId
            });
            //Provision
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = Convert.ToDouble(( bookingVM.InterestAmount)),
                Accounting_HeadFK = 50616638

            });
            //Stock
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = 1 * Convert.ToDouble(bookingVM.COGSPrice),
                Accounting_HeadFK = bookingVM.AccountingStockHeadId.Value,
                IsVirtual = true
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 1 * Convert.ToDouble(bookingVM.COGSPrice),
                Credit = 0,
                Accounting_HeadFK = 50616077, //KFMAL Stock Adjust With Erp Dr
                IsVirtual = true
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            var voucherMap = VoucherMapping(resultData.VoucherId, bookingVM.CompanyId, bookingVM.BookingId, bookingVM.IntegratedFrom);
            var result = context.CommonBookingInfoes.FirstOrDefault(d => d.BookingId == bookingVM.BookingId);
            result.IsSubmitted = true;
            result.Status = 3;
            result.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            result.ModifiedDate = DateTime.Now;
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
            return resultData.VoucherId;
        }
        public async Task<Voucher> AccountingJournalMasterPush(VMJournalSlave vmJournalSlave)
        {
            long result = -1;
            Accounting_CostCenter _CostCenter = new Accounting_CostCenter();
            if (vmJournalSlave.CompanyFK == 7 || vmJournalSlave.CompanyFK == 9)
            {
                if (vmJournalSlave.Accounting_CostCenterFK != null)
                {
                    _CostCenter.CostCenterId = (int)vmJournalSlave.Accounting_CostCenterFK;
                }
                else
                {
                    _CostCenter = context.Accounting_CostCenter.Where(x => x.CompanyId == vmJournalSlave.CompanyFK).FirstOrDefault();
                }
            }
            else
            {
                _CostCenter = context.Accounting_CostCenter.Where(x => x.CompanyId == vmJournalSlave.CompanyFK).FirstOrDefault();
            }
            string voucherNo = GetNewVoucherNo(vmJournalSlave.JournalType, vmJournalSlave.CompanyFK.Value, vmJournalSlave.Date.Value);
            Voucher voucher = new Voucher
            {
                VoucherTypeId = vmJournalSlave.JournalType,
                VoucherNo = voucherNo,
                Accounting_CostCenterFk = _CostCenter.CostCenterId,
                VoucherStatus = "A",
                VoucherDate = vmJournalSlave.Date,
                Narration = vmJournalSlave.Title + " " + vmJournalSlave.Narration,
                ChqDate = vmJournalSlave.ChqDate,
                ChqName = vmJournalSlave.ChqName,
                ChqNo = vmJournalSlave.ChqNo,
                CompanyId = vmJournalSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true,
                IsSubmit = vmJournalSlave.IsSubmit,
                IsIntegrated = true
            };

            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    context.Vouchers.Add(voucher);
                    context.SaveChanges();
                    List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
                    voucherDetailList = vmJournalSlave.DataListSlave.Select(x => new VoucherDetail
                    {
                        Particular = x.Particular,
                        DebitAmount = Convert.ToDouble(x.Debit),
                        CreditAmount = Convert.ToDouble(x.Credit),
                        AccountHeadId = x.Accounting_HeadFK,
                        IsActive = true,
                        VoucherId = voucher.VoucherId,
                        TransactionDate = voucher.VoucherDate,
                        IsVirtual = x.IsVirtual
                    }).ToList();

                    context.VoucherDetails.AddRange(voucherDetailList);
                    scope.Commit();



                    if (context.SaveChanges() > 0)
                    {
                        result = voucher.VoucherId;
                    }

                    return voucher;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return voucher;
                }
            }
        }


        private bool VoucherMapping(long voucherId, int companyId, long integratedId, string integratedFrom)
        {
            var objectToSave = context.VoucherMaps
               .SingleOrDefault(q => q.VoucherId == voucherId
               && q.IntegratedId == integratedId
               && q.CompanyId == companyId
               && q.IntegratedFrom == integratedFrom);


            if (objectToSave != null)
            {
                return false;
            }
            else
            {
                VoucherMap voucherMap = new VoucherMap();
                voucherMap.VoucherId = voucherId;
                voucherMap.IntegratedId = integratedId;
                voucherMap.CompanyId = companyId;
                voucherMap.IntegratedFrom = integratedFrom;

                context.VoucherMaps.Add(voucherMap);

                return context.SaveChanges() > 0;

            }

        }
        public string GetNewVoucherNo(int voucherTypeId, int companyId, DateTime voucherDate)
        {
            VoucherType voucherType = context.VoucherTypes.Where(x => x.VoucherTypeId == voucherTypeId).FirstOrDefault();
            string voucherNo = string.Empty;
            int vouchersCount = context.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId
            && x.VoucherDate.Value.Month == voucherDate.Month
            && x.VoucherDate.Value.Year == voucherDate.Year).Count();

            vouchersCount++;
            voucherNo = voucherType.Code + "-" + vouchersCount.ToString().PadLeft(4, '0');
          
            return voucherNo;
        }

    }
}
