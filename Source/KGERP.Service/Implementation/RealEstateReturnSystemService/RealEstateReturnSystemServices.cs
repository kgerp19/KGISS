using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.Implementation.RealEstateReturnSystemService
{
    public class RealEstateReturnSystemServices : IRealEstateReturnSystem
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AccConfirmReturn(RealEstateReturnListVM vM, RealEstateReturnSystemVM bookingVM)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    Voucher voucher = new Voucher();
                    voucher.VoucherTypeId = context.VoucherTypes.SingleOrDefault(x => x.CompanyId == vM.CompanyId && x.Code == "SRV".Trim()).VoucherTypeId;
                    var ginfo = context.CustomerGroupInfoes.FirstOrDefault(f => f.CGId == vM.CGId);
                    var returninfo = context.RealEstateReturns.FirstOrDefault(d => d.ReturnId == vM.ReturnId);
                    var vmJournalSlave = context.VoucherDetails.Where(f => f.AccountHeadId == ginfo.HeadGLId && f.IsActive == true).ToList();
                    var CollectionValue = vmJournalSlave.Sum(d => d.CreditAmount);
                    var saleValue = vmJournalSlave.Sum(d => d.DebitAmount);
                    string voucherNo = GetNewVoucherNo((int)voucher.VoucherTypeId, (int)vM.CompanyId, DateTime.Now);
                    var FileStatus = Enum.GetName(typeof(RealStateReturnsStatus), bookingVM.BookingViewModel.FileStatus);
                    var stringone = FileStatus + " - " + "File No: " + bookingVM.BookingViewModel.FileNo + " " + bookingVM.BookingViewModel.BookingNo + " Date: " + bookingVM.BookingViewModel.BookingDate.ToString();
                    var stringtwo = "Project: " + bookingVM.BookingViewModel.ProjectName + " " + bookingVM.BookingViewModel.BlockName + " " + bookingVM.BookingViewModel.PlotName;
                    voucher.CompanyId = vM.CompanyId;
                    voucher.VoucherDate = DateTime.Now;
                    voucher.VoucherNo = voucherNo;
                    voucher.IsSubmit = true;
                    voucher.IsActive = true;
                    voucher.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    voucher.CreateDate = DateTime.Now;
                    voucher.Narration = stringone + " ," + stringtwo;
                    voucher.Accounting_CostCenterFk = bookingVM.BookingViewModel.AcCostCenterId;
                    context.Vouchers.Add(voucher);
                    context.SaveChanges();
                    List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
                    voucherDetailList = vmJournalSlave.Select(x => new VoucherDetail
                    {
                        Particular = FileStatus + " - " + x.Particular,
                        DebitAmount = x.CreditAmount,
                        CreditAmount = x.DebitAmount,
                        AccountHeadId = x.AccountHeadId,
                        IsActive = true,
                        VoucherId = voucher.VoucherId,
                        TransactionDate = voucher.VoucherDate,
                        IsVirtual = x.IsVirtual
                    }).ToList();

                    VoucherDetail detail = new VoucherDetail();
                    detail.Particular = voucher.Narration;
                    detail.DebitAmount = 0;
                    detail.CreditAmount = CollectionValue - (double)returninfo.ReturnFee;
                    detail.AccountHeadId = vM.Accounting_BankOrCashId;
                    detail.IsActive = true;
                    detail.VoucherId = voucher.VoucherId;
                    detail.TransactionDate = voucher.VoucherDate;
                    detail.IsVirtual = false;
                    voucherDetailList.Add(detail);

                    VoucherDetail detail2 = new VoucherDetail();
                    detail2.Particular = voucher.Narration;
                    detail2.DebitAmount = saleValue;
                    detail2.CreditAmount = 0;
                    detail2.AccountHeadId = bookingVM.BookingViewModel.AccountingIncomeHeadId;
                    detail2.IsActive = true;
                    detail2.VoucherId = voucher.VoucherId;
                    detail2.TransactionDate = voucher.VoucherDate;
                    detail2.IsVirtual = false;
                    voucherDetailList.Add(detail2);
                    if (returninfo.ReturnFee > 0)
                    {
                        HeadGL headGL = new HeadGL();
                        if (vM.CompanyId == 7)
                        {
                            headGL = context.HeadGLs.FirstOrDefault(d => d.AccCode == "3101001002078" && d.CompanyId == 7);
                        }
                        else
                        {
                            headGL = context.HeadGLs.FirstOrDefault(d => d.AccCode == "3201001007001" && d.CompanyId == 9);
                        }
                        VoucherDetail detail3 = new VoucherDetail();
                        detail3.Particular = FileStatus + returninfo.ReturnFeePercentage + "% " + "fee";
                        detail3.DebitAmount = 0;
                        detail3.CreditAmount = (double)returninfo.ReturnFee;
                        detail3.AccountHeadId = headGL.Id;
                        detail3.IsActive = true;
                        detail3.VoucherId = voucher.VoucherId;
                        detail3.TransactionDate = voucher.VoucherDate;
                        detail3.IsVirtual = false;
                        voucherDetailList.Add(detail3);
                    }
                    context.VoucherDetails.AddRange(voucherDetailList);
                    returninfo.AccAprovalDate = DateTime.Now;
                    returninfo.IsAccAproval = 1;
                    context.Entry(returninfo).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    context.SaveChanges();
                    return voucher.VoucherId;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
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
            voucherNo = voucherType.Code + "-" + vouchersCount.ToString().PadLeft(6, '0');
            return voucherNo;
        }
        public async Task<int> AddReturn(RealEstateReturnSystemVM model)
        {
            int returnResult = 0;
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {

                    RealEstateReturn realEstateReturn = new RealEstateReturn();
                    realEstateReturn.CGId = model.CGId;
                    realEstateReturn.Particular = model.Purpose;
                    realEstateReturn.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    realEstateReturn.CreatedDate = DateTime.Now;
                    realEstateReturn.Status = (int)model.StatusId;
                    realEstateReturn.DocId = model.DocId;
                    realEstateReturn.ReturnFee = model.ReturnFee;
                    realEstateReturn.ReturnFeePercentage = model.ReturnFeePercentage;
                    realEstateReturn.IsActive=true; 
                    realEstateReturn.CompanyId = model.CompanyId;   
                    realEstateReturn.CancelDate= Convert.ToDateTime(model.strCancelDate); 
                    context.RealEstateReturns.Add(realEstateReturn);
                    context.SaveChanges();
                    ProductBookingInfo info = await context.ProductBookingInfoes.FirstOrDefaultAsync(d => d.CGId == model.CGId);
                    info.FileStatus = (int)model.StatusId;
                    context.Entry(info).State = EntityState.Modified;
                    context.SaveChanges();

                    if (model.StatusId==2)
                    {
                        Product product = await context.Products.FirstAsync(x => x.ProductId == info.ProductId);
                        product.Status = (int)ProductStatusEnumGLDL.UnSold;
                        context.Entry(product).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                    scope.Commit();
                    returnResult = 1;


                    return returnResult;
                    
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    throw ex;
                }
            }

        }

        public async Task<RealEstateReturnSystemVM> GetFileInformation(long CGId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model = await Task.Run(() => (from t1 in context.CustomerGroupInfoes
                                          join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                          join t3 in context.ProductBookingInfoes on t1.CGId equals t3.CGId
                                          join t4 in context.Products on t3.ProductId equals t4.ProductId into t4_join
                                          from t4 in t4_join.DefaultIfEmpty()
                                          join t9 in context.Units on t4.UnitId equals t9.UnitId into t9_join
                                          from t9 in t9_join.DefaultIfEmpty()
                                          join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                                          from t5 in t5_join.DefaultIfEmpty()
                                          join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                                          from t6 in t6_join.DefaultIfEmpty()
                                          join t7 in context.Employees on t3.SoldBy equals t7.Id into t7_join
                                          from t7 in t7_join.DefaultIfEmpty()
                                          join t8 in context.Employees on t3.TeamLeadId equals t8.Id into t8_join
                                          from t8 in t8_join.DefaultIfEmpty()
                                          where t1.CGId == CGId
                                          select new GLDLBookingViewModel
                                          {
                                              ProductStatus = t4.Status,
                                              IntegratedFrom = "ProductBookingInfo",
                                              HeadGLId = t1.HeadGLId,
                                              CGId = t1.CGId,
                                              CustomerGroupName = t1.GroupName,
                                              KGRECompanyName = t2.Name,
                                              PrimaryContactNo = t1.PrimaryContactNo,
                                              PrimaryEmail = t1.PrimaryEmail,
                                              PrimaryContactAddr = t1.PrimaryContactAddr,
                                              ProjectName = t6.Name,
                                              ProductCategoryId = t6.ProductCategoryId,
                                              BlockName = t5.Name,
                                              ProductSubCategoryId = t5.ProductSubCategoryId,
                                              PlotName = t4.ProductName,
                                              ProductId = t3.ProductId,
                                              BookingNo = t3.BookingNo,
                                              BookingId = t3.BookingId,
                                              PlotNo = t4.ProductCode,
                                              PlotSize = t4.PackSize,
                                              RatePerKatha = t3.RatePerKatha,
                                              BookingMoney = t3.BookingAmt,
                                              Discount = t3.DiscountPercentage,
                                              LandValue = t3.LandValue,
                                              RestofAmount = t3.RestofAmount,
                                              CompanyId = t2.CompanyId,
                                              UnitName = t9.Name,
                                              SalesPerson = t7.Name,
                                              SalesPersonPhone = t7.MobileNo,
                                              SalesPersonEmail = t7.Email,
                                              SalesPersonAddress = t7.PermanentAddress,
                                              Status = t3.Status,
                                              Step = t3.Step,
                                              PaidDownPaymentAmt = t3.PaidDownPaymentAmt,
                                              ApplicationDate = t3.ApplicationDate,
                                              BookingDate = t3.BookingDate,
                                              SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                              TeamLeadName = t8.Name,
                                              TeamLeadPhone = t8.MobileNo,
                                              TeamLeadEmail = t8.Email,
                                              TeamLeadAddress = t8.PermanentAddress,
                                              EmployeeId = (int)t7.Id,
                                              IsSubmited = t3.IsSubmitted,
                                              FileNo = t3.FileNo,
                                              InstallmentAmount = t3.InstallmentAmount,
                                              AcCostCenterId = t6.CostCenterId,
                                              FileStatus = t3.FileStatus,
                                              AccountingIncomeHeadId = t5.AccountingIncomeHeadId,
                                              PaidBookingMoney = t3.PaidBookingAmt
                                          }).FirstOrDefaultAsync());

            model.LstPurchaseCostHeads = await Task.Run(() => (from t1 in context.ProductBookingInfoes.Where(x => x.CGId == CGId)
                                                               join t2 in context.BookingCostMappings on t1.BookingId equals t2.BookingId
                                                               join t3 in context.BookingCostHeads on t2.CostId equals t3.CostId
                                                               select new BookingHeadServiceModel
                                                               {
                                                                   CostId = t2.CostId,
                                                                   Amount = t2.Amount,
                                                                   BookingId = t2.BookingId,
                                                                   CostName = t3.CostName,
                                                                   PaidAmount = t2.PaidAmount
                                                               }).ToListAsync());
            decimal totalcost = model.LstPurchaseCostHeads.Select(d => d.Amount).Sum();
            decimal totalcost2 = model.LstPurchaseCostHeads.Select(d => d.PaidAmount).Sum();
            model.TotalCost = totalcost;
            model.TotalAmount = model.LandValue + totalcost;
            decimal TotalDiscount = ((model.LandValue / 100m) * Convert.ToDecimal(model.Discount));
            model.TotalDiscount = (decimal)(TotalDiscount + model.SpecialDiscountAmt);
            model.GrandTotalAmount = model.TotalAmount - model.TotalDiscount;



            model.Cutomers = await Task.Run(() => (from t1 in context.CustomerGroupMappings
                                                   join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                   where t1.CGId == CGId
                                                   select new CutomerListForBooking
                                                   {
                                                       MapId = t1.MapId,
                                                       VendorId = t1.CustomerId,
                                                       VendorName = t2.Name,
                                                       VendorMobile = t2.Phone,
                                                       SharePercentage = t1.SharePercentage
                                                   }).ToListAsync());


            model.Attachments = await context.CustomerBookingFileMappings.Where(t2 => t2.CGId == CGId && t2.IsActive).
                                                         Select(o => new GLDLBookingAttachment
                                                         {
                                                             DocId = o.DocId,
                                                             Title = o.FileTitel
                                                         }).ToListAsync();

            model.Schedule = await context.BookingInstallmentSchedules.Where(f => f.CGID == CGId && f.IsActive).
                Select(o => new InstallmentScheduleShortModel
                {
                    InstallmentId = o.InstallmentId,
                    Title = o.InstallmentTitle,
                    PayableAmount = o.Amount,
                    InstallmentDate = o.Date,
                    PaidAmount = o.PaidAmount,
                    IsPaid = o.IsPaid,

                }).ToListAsync();
            model.InstallmentSumOfAmount = model.Schedule.Sum(g => g.PayableAmount);
            decimal Schedule = model.Schedule.Sum(g => g.PaidAmount);
            model.TolalCollaction = model.PaidBookingMoney +model.PaidDownPaymentAmt + Schedule + totalcost2;
            RealEstateReturnSystemVM systemVM = new RealEstateReturnSystemVM();
            systemVM.BookingViewModel = model;
            return systemVM;
        }

        public async Task<RealEstateReturnListVM> ReturnListn(int companyid)
        {
            RealEstateReturnListVM model = new RealEstateReturnListVM();
            model.CompanyId = companyid;
            model.datalist = await Task.Run(() => (from t1 in context.RealEstateReturns
                                                   join t2 in context.CustomerGroupInfoes on t1.CGId equals t2.CGId
                                                   join t3 in context.ProductBookingInfoes on t1.CGId equals t3.CGId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId into t4_join
                                                   from t4 in t4_join.DefaultIfEmpty()
                                                   join t9 in context.Units on t4.UnitId equals t9.UnitId into t9_join
                                                   from t9 in t9_join.DefaultIfEmpty()
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                                                   from t5 in t5_join.DefaultIfEmpty()
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                                                   from t6 in t6_join.DefaultIfEmpty()
                                                   join t7 in context.Employees on t3.SoldBy equals t7.Id into t7_join
                                                   from t7 in t7_join.DefaultIfEmpty()
                                                   where t2.CompanyId== companyid   
                                                   select new RealEstateReturnListVM
                                                   {
                                                       ReturnId = t1.ReturnId,
                                                       CGId = t1.CGId,
                                                       FileNo = t3.FileNo,
                                                       Particular = t1.Particular,
                                                       ClientName = t2.GroupName,
                                                       ProjectName = t6.Name,
                                                       BlockName = t5.Name,
                                                       PlotName = t4.ProductName,
                                                       StatusId = t3.FileStatus,
                                                       Status = t1.Status,
                                                       FileValue = t3.RestofAmount + t3.BookingAmt,
                                                       CompanyId = t2.CompanyId,
                                                       AccHeadId = t2.HeadGLId,
                                                       DocId=t1.DocId,
                                                        IsAccAproval = t1.IsAccAproval
                                                   }).ToListAsync());
            return model;
        }

        public async Task<long> AccTransfarFileConfirmReturn(RealEstateReturnListVM vM, RealEstateReturnSystemVM bookingVM)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    Voucher voucher = new Voucher();
                    voucher.VoucherTypeId = context.VoucherTypes.SingleOrDefault(x => x.CompanyId == vM.CompanyId && x.Code == "SRV".Trim()).VoucherTypeId;
                    var ginfo = context.CustomerGroupInfoes.FirstOrDefault(f => f.CGId == vM.CGId);
                    var returninfo = context.RealEstateReturns.FirstOrDefault(d => d.ReturnId == vM.ReturnId);
                    string voucherNo = GetNewVoucherNo((int)voucher.VoucherTypeId, (int)vM.CompanyId, DateTime.Now);
                    var FileStatus = Enum.GetName(typeof(RealStateReturnsStatus), bookingVM.BookingViewModel.FileStatus);
                    var stringone = FileStatus + " - " + "File No: " + bookingVM.BookingViewModel.FileNo + " " + bookingVM.BookingViewModel.BookingNo + " Date: " + bookingVM.BookingViewModel.BookingDate.ToString();
                    var stringtwo = "Project: " + bookingVM.BookingViewModel.ProjectName + " " + bookingVM.BookingViewModel.BlockName + " " + bookingVM.BookingViewModel.PlotName;
                    voucher.CompanyId = vM.CompanyId;
                    voucher.VoucherDate = DateTime.Now;
                    voucher.VoucherNo = voucherNo;
                    voucher.IsSubmit = true;
                    voucher.IsActive = true;
                    voucher.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    voucher.CreateDate = DateTime.Now;
                    voucher.Narration = stringone + " ," + stringtwo;
                    voucher.Accounting_CostCenterFk = bookingVM.BookingViewModel.AcCostCenterId;
                    context.Vouchers.Add(voucher);
                    context.SaveChanges();
                    List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
                    VoucherDetail detail2 = new VoucherDetail();
                    detail2.Particular = voucher.Narration;
                    detail2.DebitAmount = (double)(bookingVM.BookingViewModel.RestofAmount+ bookingVM.BookingViewModel.BookingMoney);
                    detail2.CreditAmount = 0;
                    detail2.AccountHeadId = bookingVM.BookingViewModel.AccountingIncomeHeadId;
                    detail2.IsActive = true;
                    detail2.VoucherId = voucher.VoucherId;
                    detail2.TransactionDate = voucher.VoucherDate;
                    detail2.IsVirtual = false;
                    voucherDetailList.Add(detail2);

                    VoucherDetail detail = new VoucherDetail();
                    detail.Particular = voucher.Narration;
                    detail.DebitAmount = 0;
                    detail.CreditAmount = (double)(bookingVM.BookingViewModel.RestofAmount + bookingVM.BookingViewModel.BookingMoney); ;
                    detail.AccountHeadId = bookingVM.BookingViewModel.HeadGLId;
                    detail.IsActive = true;
                    detail.VoucherId = voucher.VoucherId;
                    detail.TransactionDate = voucher.VoucherDate;
                    detail.IsVirtual = false;
                    voucherDetailList.Add(detail);

                    if (returninfo.ReturnFee > 0)
                    {
                        HeadGL headGL = new HeadGL();
                        if (vM.CompanyId == 7)
                        {
                            headGL = context.HeadGLs.FirstOrDefault(d => d.AccCode == "3101001002078" && d.CompanyId == 7);
                        }
                        else
                        {
                            headGL = context.HeadGLs.FirstOrDefault(d => d.AccCode == "3201001007001" && d.CompanyId == 9);
                        }
                        VoucherDetail detail3 = new VoucherDetail();
                        detail3.Particular = FileStatus + returninfo.ReturnFeePercentage + "% " + "fee";
                        detail3.DebitAmount = 0;
                        detail3.CreditAmount = (double)returninfo.ReturnFee;
                        detail3.AccountHeadId = headGL.Id;
                        detail3.IsActive = true;
                        detail3.VoucherId = voucher.VoucherId;
                        detail3.TransactionDate = voucher.VoucherDate;
                        detail3.IsVirtual = false;
                        voucherDetailList.Add(detail3);
                    }
                    context.VoucherDetails.AddRange(voucherDetailList);
                    returninfo.AccAprovalDate = DateTime.Now;
                    returninfo.IsAccAproval = 1;
                    context.Entry(returninfo).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
    
                    return voucher.VoucherId;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return 0;
                }
            }
        }

        public async Task<RealEstateReturnListVM> KGEREReturnListn(int companyid)
        {
            RealEstateReturnListVM model = new RealEstateReturnListVM();
            model.CompanyId = companyid;
            model.datalist = await Task.Run(() => (from t1 in context.RealEstateReturns
                                                   join t2 in context.CustomerGroupInfoes on t1.CGId equals t2.CGId
                                                   join t3 in context.ProductBookingInfoes on t1.CGId equals t3.CGId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId into t4_join
                                                   from t4 in t4_join.DefaultIfEmpty()
                                                   join t9 in context.Units on t4.UnitId equals t9.UnitId into t9_join
                                                   from t9 in t9_join.DefaultIfEmpty()
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                                                   from t5 in t5_join.DefaultIfEmpty()
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                                                   from t6 in t6_join.DefaultIfEmpty()
                                                   join t7 in context.Employees on t3.SoldBy equals t7.Id into t7_join
                                                   from t7 in t7_join.DefaultIfEmpty()
                                                   where t2.CompanyId == companyid
                                                   select new RealEstateReturnListVM
                                                   {
                                                       ReturnId = t1.ReturnId,
                                                       CGId = t1.CGId,
                                                       FileNo = t3.FileNo,
                                                       Particular = t1.Particular,
                                                       ClientName = t2.GroupName,
                                                       ProjectName = t6.Name,
                                                       BlockName = t5.Name,
                                                       PlotName = t4.ProductName,
                                                       StatusId = t3.FileStatus,
                                                       Status = t1.Status,
                                                       FileValue = t3.RestofAmount + t3.BookingAmt,
                                                       CompanyId = t2.CompanyId,
                                                       AccHeadId = t2.HeadGLId,
                                                       DocId = t1.DocId,
                                                       IsAccAproval=t1.IsAccAproval

                                                   }).OrderBy(d=>d.Status).ToListAsync());
            return model;
        }

        public async Task<int> RemoveReturn(long Id)
        {
            var res = await context.RealEstateReturns.FirstOrDefaultAsync(g => g.ReturnId == Id);
            var booking = await context.ProductBookingInfoes.FirstOrDefaultAsync(f => f.CGId == res.CGId);
            booking.FileStatus = 0;
            context.Entry(booking).State = EntityState.Modified;
            context.SaveChanges();
            res.IsActive = false;
            context.Entry(res).State = EntityState.Modified;
            context.SaveChanges();
            return 1;
        }
    }
}
