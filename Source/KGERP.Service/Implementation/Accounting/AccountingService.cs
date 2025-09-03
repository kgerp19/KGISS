using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Implementation.LcInfoServices;
using KGERP.Service.Implementation.Production;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Implementation.Short__CreditService;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.RealState;
using KGERP.Service.ServiceModel.SeedProcessingModel;
using KGERP.Service.ServiceModel.Vendor;
using KGERP.Services.Procurement;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;


namespace KGERP.Service.Implementation

{
    public class AccountingService
    {
        private readonly ERPEntities _db;
        string _urlInfo = "";
        public AccountingService(ERPEntities db)
        {
            _db = db;
            _urlInfo = GetErpUrlInfo();
        }

        public string GetErpUrlInfo()
        {
            int pi;


            return _db.UrlInfoes.Where(x => x.UrlType == 1).Select(x => x.Url).FirstOrDefault();
        }
        public async Task<long> AccountingJournalPush(DateTime journalDate, int CompanyFK, int drHeadID, long? crHeadID, decimal amount, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = Convert.ToDouble(amount),
                Credit = 0,
                Accounting_HeadFK = drHeadID
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = Convert.ToDouble(amount),
                Accounting_HeadFK = Convert.ToInt32(crHeadID)
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }

        public async Task<long> AccountingInventoryPush(DateTime journalDate, int CompanyFK, int adjustHeadDr, int adjustHeadCr, decimal adjustValue, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,

                IsSubmit = true
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = Convert.ToDouble(adjustValue),
                Accounting_HeadFK = adjustHeadCr
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = Convert.ToDouble(adjustValue),
                Credit = 0,
                Accounting_HeadFK = adjustHeadDr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }

        public async Task<Voucher> AccountingJournalMasterPush(VMJournalSlave vmJournalSlave)
        {
            long result = -1;
            string VoucherBy = GlobalValues.UserId;
            var C = Common.GetUserId();
            Accounting_CostCenter _CostCenter = new Accounting_CostCenter();

            if (vmJournalSlave.CompanyFK == 2)
            {
                if (vmJournalSlave.Accounting_CostCenterFK != null)
                {
                    _CostCenter.CostCenterId = (int)vmJournalSlave.Accounting_CostCenterFK; //for Sales Voucher
                }
                else
                {
                    _CostCenter = _db.Accounting_CostCenter.Where(x => x.CompanyId == vmJournalSlave.CompanyFK).FirstOrDefault();
                }
            }
            else
            {
                _CostCenter = _db.Accounting_CostCenter.Where(x => x.CompanyId == vmJournalSlave.CompanyFK).FirstOrDefault();
            }
            string voucherNo = GetNewVoucherNo(vmJournalSlave.JournalType, vmJournalSlave.CompanyFK.Value, vmJournalSlave.Date.Value);

            var data = (from t1 in _db.ReportApprovalDetails
                        join t2 in _db.ReportApprovals on t1.ReportApprovalId equals t2.ReportApprovalId
                        where t2.CompanyId == vmJournalSlave.CompanyFK &&
                        t2.Month == vmJournalSlave.Date.Value.Month &&
                        t2.Year == vmJournalSlave.Date.Value.Year &&
                        t1.ApprovalStatus == 3
                        select new
                        {
                            ReportApprovalId = t2.ReportApprovalId,
                            Month = t2.Month,
                            Year = t2.Year,
                            ReportApprovalDetailId = t1.ReportApprovalDetail1
                        }).OrderByDescending(x => x.ReportApprovalDetailId).FirstOrDefault();



            Voucher voucher = new Voucher
            {
                VoucherTypeId = vmJournalSlave.JournalType,
                VoucherNo = voucherNo,
                Accounting_CostCenterFk = _CostCenter.CostCenterId,
                VoucherStatus = (data == null ? "A" : null),

                VoucherDate = vmJournalSlave.Date,
                Narration = vmJournalSlave.Title + " " + vmJournalSlave.Narration,
                ChqDate = vmJournalSlave.ChqDate,
                ChqName = vmJournalSlave.ChqName,
                ChqNo = vmJournalSlave.ChqNo,
                CompanyId = vmJournalSlave.CompanyFK,
                CreatedBy = VoucherBy != null ? VoucherBy : System.Web.HttpContext.Current.User.Identity.Name,// vmJournalSlave.CreatedBy, // 
                CreateDate = DateTime.Now,
                IsActive = true,
                IsSubmit = (data == null ? vmJournalSlave.IsSubmit : false),
                IsIntegrated = true
            };



            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Vouchers.Add(voucher);
                    _db.SaveChanges();
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

                    _db.VoucherDetails.AddRange(voucherDetailList);
                    scope.Commit();
                    if (_db.SaveChanges() > 0)
                    {
                        result = voucher.VoucherId;
                    }
                    if (result > 0)
                    {
                        var v = _db.Vouchers.Find(voucher.VoucherId);
                        v.TotalAmount = voucherDetailList
                            .Where(x => x.IsActive == true && x.IsVirtual == false)
                            .Select(x => x.CreditAmount).DefaultIfEmpty(0).Sum();
                        _db.SaveChanges();
                    }
                    return voucher;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return voucher;
                }
            }
            return voucher;


        }

        public string GetNewVoucherNo(int voucherTypeId, int companyId, DateTime voucherDate)
        {
            VoucherType voucherType = _db.VoucherTypes.Where(x => x.VoucherTypeId == voucherTypeId).FirstOrDefault();
            string voucherNo = string.Empty;
            int vouchersCount = _db.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId
            && x.VoucherDate.Value.Month == voucherDate.Month
            && x.VoucherDate.Value.Year == voucherDate.Year).Count();

            vouchersCount++;
            voucherNo = voucherType.Code + "-" + vouchersCount.ToString().PadLeft(4, '0');
            //if (vouchersCount  == 0)
            //{               
            //    voucherNo = voucherType.Code + "-" + "000001";                
            //}
            //else
            //{

            //    //Voucher voucher = context.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId).OrderByDescending(x => x.VoucherNo).FirstOrDefault();
            //    //voucherNo = GenerateVoucherNo(voucher.VoucherNo);
            //}           
            return voucherNo;
        }

        public string GetVoucherNo(int voucherTypeId, int companyId)
        {
            string voucherNo = string.Empty;
            var vouchers = _db.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId);

            if (!vouchers.Any())
            {
                VoucherType voucherType = _db.VoucherTypes.Where(x => x.VoucherTypeId == voucherTypeId).FirstOrDefault();
                voucherNo = voucherType.Code + "-" + "0001";
                return voucherNo;
            }
            Voucher voucher = _db.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId).OrderByDescending(x => x.VoucherNo).FirstOrDefault();
            voucherNo = GenerateVoucherNo(voucher.VoucherNo);
            return voucherNo;
        }

        private string GenerateVoucherNo(string lastVoucherNo)
        {
            string prefix = lastVoucherNo.Substring(0, 4);
            int code = Convert.ToInt32(lastVoucherNo.Substring(4, 6));
            int newCode = code + 1;
            return prefix + newCode.ToString().PadLeft(6, '0');
        }
        public async Task<VMJournalSlave> GetCompaniesDetails(int companyId)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();
            vmJournalSlave = await Task.Run(() => (from t1 in _db.Companies.Where(x => x.IsActive && x.CompanyId == companyId)

                                                   select new VMJournalSlave
                                                   {
                                                       CompanyFK = t1.CompanyId,
                                                       CompanyName = t1.Name
                                                   }).FirstOrDefault());


            return vmJournalSlave;
        }

        public async Task<VMJournalSlave> GetVoucherDetails(int companyId, int voucherId)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();
            vmJournalSlave = await Task.Run(() => (from t1 in _db.Vouchers.Where(x => x.IsActive && x.VoucherId == voucherId && x.CompanyId == companyId)
                                                   join t4 in _db.VoucherTypes on t1.VoucherTypeId equals t4.VoucherTypeId
                                                   join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                                                   join t3 in _db.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                   //  join t5 in _db.HeadGLs on t1.VirtualHeadId equals t5.Id

                                                   select new VMJournalSlave
                                                   {
                                                       VoucherId = t1.VoucherId,
                                                       Accounting_CostCenterName = t3.Name,
                                                       VoucherNo = t1.VoucherNo,
                                                       Date = t1.VoucherDate,
                                                       Narration = t1.Narration,
                                                       CompanyFK = t1.CompanyId,
                                                       Status = t1.VoucherStatus,
                                                       ChqDate = t1.ChqDate,
                                                       ChqName = t1.ChqName,
                                                       ChqNo = t1.ChqNo,
                                                       Accounting_CostCenterFK = t1.Accounting_CostCenterFk,
                                                       Accounting_BankOrCashId = t1.VirtualHeadId,
                                                       //BankOrCashNane = "[" + t5.AccCode + "] " + t5.AccName,
                                                       CompanyName = t2.Name,
                                                       IsSubmit = t1.IsSubmit
                                                   }).FirstOrDefault());

            vmJournalSlave.DataListDetails = await Task.Run(() => (from t1 in _db.VoucherDetails.Where(x => x.IsActive && x.VoucherId == voucherId && !x.IsVirtual)
                                                                   join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id
                                                                   join t3 in _db.ProductCategories.Where(x => x.IsActive) on t1.ProductCategory equals t3.ProductCategoryId into Lt3
                                                                   from t3 in Lt3.DefaultIfEmpty()
                                                                   select new VMJournalSlave
                                                                   {
                                                                       VoucherDetailId = t1.VoucherDetailId,
                                                                       AccountingHeadName = t2.AccName,
                                                                       Code = t2.AccCode,
                                                                       Credit = t1.CreditAmount,
                                                                       Debit = t1.DebitAmount,
                                                                       Particular = t1.Particular,
                                                                       ProductCategoryName = t3.Name
                                                                   }).OrderByDescending(x => x.VoucherDetailId).AsEnumerable());
            if (vmJournalSlave.DataListDetails.Any())
            {
                vmJournalSlave.Particular = vmJournalSlave.DataListDetails.OrderByDescending(x => x.VoucherDetailId).Select(x => x.Particular).FirstOrDefault();
            }
            return vmJournalSlave;
        }

        public async Task<VMJournalSlave> GetStockVoucherDetails(int companyId, int voucherId)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();
            vmJournalSlave = await Task.Run(() => (from t1 in _db.Vouchers.Where(x => x.IsActive && x.VoucherId == voucherId && x.CompanyId == companyId)
                                                   join t4 in _db.VoucherTypes on t1.VoucherTypeId equals t4.VoucherTypeId
                                                   join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                                                   join t3 in _db.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId

                                                   select new VMJournalSlave
                                                   {
                                                       VoucherId = t1.VoucherId,
                                                       Accounting_CostCenterName = t3.Name,
                                                       VoucherNo = t1.VoucherNo,
                                                       Date = t1.VoucherDate,
                                                       Narration = t1.Narration,
                                                       CompanyFK = t1.CompanyId,
                                                       Status = t1.VoucherStatus,
                                                       ChqDate = t1.ChqDate,
                                                       ChqName = t1.ChqName,
                                                       ChqNo = t1.ChqNo,
                                                       Accounting_CostCenterFK = t1.Accounting_CostCenterFk,
                                                       Accounting_BankOrCashId = t1.VirtualHeadId,
                                                       IsSubmit = t1.IsSubmit

                                                   }).FirstOrDefault());

            vmJournalSlave.DataListDetails = await Task.Run(() => (from t1 in _db.VoucherDetails.Where(x => x.IsActive && x.VoucherId == voucherId && !x.IsVirtual)
                                                                   join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id
                                                                   select new VMJournalSlave
                                                                   {
                                                                       VoucherDetailId = t1.VoucherDetailId,
                                                                       AccountingHeadName = t2.AccName,
                                                                       Code = t2.AccCode,
                                                                       Credit = t1.CreditAmount,
                                                                       Debit = t1.DebitAmount,
                                                                       Particular = t1.Particular
                                                                   }).OrderByDescending(x => x.VoucherDetailId).AsEnumerable());
            return vmJournalSlave;
        }
        public async Task<long> VoucherAdd(VMJournalSlave vmJournalSlave)
        {
            long result = -1;
            //GetVoucherNo

            Voucher voucher = new Voucher
            {
                Narration = vmJournalSlave.Narration,
                VoucherNo = vmJournalSlave.VoucherNo,
                VoucherStatus = vmJournalSlave.Status,
                VoucherTypeId = vmJournalSlave.VoucherTypeId,
                ChqDate = vmJournalSlave.ChqDate,
                VirtualHeadId = vmJournalSlave.Accounting_BankOrCashId,
                ChqNo = vmJournalSlave.ChqNo,

                Accounting_CostCenterFk = vmJournalSlave.Accounting_CostCenterFK,
                ChqName = vmJournalSlave.ChqName,
                VoucherDate = vmJournalSlave.Date,
                CompanyId = vmJournalSlave.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                IsActive = true,
                IsStock = vmJournalSlave.IsStock
            };
            _db.Vouchers.Add(voucher);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }
            return result;
        }
        public async Task<long> VoucherDetailAdd(VMJournalSlave vmJournalSlave)
        {
            long result = -1;
            if ((vmJournalSlave.Accounting_HeadFK > 0) && (vmJournalSlave.Debit > 0 || vmJournalSlave.Credit > 0))
            {
                VoucherDetail voucherDetail = new VoucherDetail
                {
                    AccountHeadId = vmJournalSlave.Accounting_HeadFK,
                    CreditAmount = vmJournalSlave.Credit,
                    DebitAmount = vmJournalSlave.Debit,
                    Particular = vmJournalSlave.Particular,
                    TransactionDate = DateTime.Now,
                    VoucherId = vmJournalSlave.VoucherId,
                    ProductCategory = vmJournalSlave.ProductCategory,
                    IsActive = true
                };
                _db.VoucherDetails.Add(voucherDetail);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = voucherDetail.VoucherDetailId;
                }
            }


            return result;
        }



        public async Task<long> VoucherDetailsEdit(VMJournalSlave vmJournalSlave)
        {
            long result = -1;
            VoucherDetail model = await _db.VoucherDetails.FindAsync(vmJournalSlave.VoucherDetailId);

            model.AccountHeadId = vmJournalSlave.Accounting_HeadFK;
            model.CreditAmount = vmJournalSlave.Credit;
            model.DebitAmount = vmJournalSlave.Debit;
            model.Particular = vmJournalSlave.Particular;
            model.ProductCategory = vmJournalSlave.ProductCategory;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.VoucherDetailId;
            }

            return result;
        }
        public async Task<long> VoucherDelete(VoucherModel voucherModel)
        {
            long result = -1;
            Voucher model = await _db.Vouchers.FindAsync(voucherModel.VoucherId);

            model.IsActive = false;

            List<VoucherDetail> VoucherDetailList = _db.VoucherDetails.Where(x => x.VoucherId == voucherModel.VoucherId).ToList();
            VoucherDetailList.ForEach(x => x.IsActive = false);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.VoucherId;
            }

            return result;
        }
        public async Task<long> VoucherUndoSubmit(VoucherModel voucherModel)
        {
            long result = -1;
            Voucher model = await _db.Vouchers.FindAsync(voucherModel.VoucherId);
            model.IsSubmit = false;
            model.VoucherStatus = null;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.VoucherId;
            }
            return result;
        }

        public async Task<long> VoucherApproved(VoucherModel voucherModel)
        {
            long userId = Common.GetIntUserId();


            long result = -1;
            Voucher model = await _db.Vouchers.FindAsync(voucherModel.VoucherId);
            model.IsApproved = true;
            model.ApprovedDate = DateTime.Now;
            model.ApprovedBy = userId;
            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.VoucherId;
            }
            return result;
        }
        public async Task<long> VoucherDetailsDelete(long voucherDetailId)
        {
            long result = -1;
            VoucherDetail model = await _db.VoucherDetails.FindAsync(voucherDetailId);

            model.IsActive = false;

            if (await _db.SaveChangesAsync() > 0)
            {
                result = model.VoucherDetailId;
            }

            return result;
        }
        public List<object> CostCenterDropDownList(int companyId)
        {
            var List = new List<object>();
            _db.Accounting_CostCenter
        .Where(x => x.IsActive && x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.CostCenterId,
            Text = x.Name
        }));
            return List;

        }

        public List<object> ProductCategoryDropDownList(int companyId)
        {
            var list = new List<object>
            {
                new { Value = 0, Text = "Please select" } // Adding a default option  
            };

            _db.ProductCategories
                .Where(x => x.IsActive && x.CompanyId == companyId)
                .Select(x => new
                {
                    Value = x.ProductCategoryId,
                    Text = x.Name
                })
                .ToList()
                .ForEach(x => list.Add(x)); // Add the actual product categories to the list  

            return list;
        }

        public List<object> VoucherTypesCashAndBankDropDownList()
        {
            var List = new List<object>();
            _db.VoucherTypes
        .Where(x => x.IsActive && x.VoucherTypeId <= (int)JournalEnum.CashReceive).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VoucherTypeId,
            Text = x.Name
        }));
            return List;

        }

        public List<object> VoucherTypesDownList(int companyId)
        {
            var List = new List<object>();
            _db.VoucherTypes
        .Where(x => x.IsActive && x.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VoucherTypeId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> CRVVoucherTypesDownList(int companyId)
        {
            var List = new List<object>();
            var voucherCodes = new[] { "jsv", "crv" };
            _db.VoucherTypes
        .Where(x => x.IsActive
                && x.CompanyId == companyId
                && voucherCodes.Any(code => x.Code.ToLower().Contains(code))).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VoucherTypeId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> DRVVoucherTypesDownList(int companyId)
        {
            var List = new List<object>();
            _db.VoucherTypes
        .Where(x => x.IsActive && x.CompanyId == companyId && x.Code == "DRV").Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VoucherTypeId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> VoucherTypesJournalVoucherDropDownList()
        {
            var List = new List<object>();
            _db.VoucherTypes
        .Where(x => x.IsActive && x.VoucherTypeId == (int)JournalEnum.JournalVoucer).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.VoucherTypeId,
            Text = x.Name
        }));
            return List;

        }
        public List<object> SeedCashAndBankDropDownList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.Head4
                     join t2 in _db.Head3 on t1.ParentId equals t2.Id
                     where t2.AccCode == "1301" && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }

            return List;

        }

        public List<object> GCCLLCFactoryExpanceHeadGLList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.HeadGLs
                     join t2 in _db.Head5 on t1.ParentId equals t2.Id
                     join t3 in _db.Head4 on t2.ParentId equals t3.Id
                     join t4 in _db.Head3 on t3.ParentId equals t4.Id

                     where (t4.AccCode == "4103" || t4.AccCode == "4104" || t4.AccCode == "4105"
                     || t4.AccCode == "4106" || t2.AccCode == "1301002002" || t3.AccCode == "2401013" || t3.AccCode == "2401007")
                     && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t4.AccCode + " -" + t4.AccName + " " + t1.AccCode + " -" + t1.AccName
                     }).ToList();
            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }
            return List;

        }


        public List<object> GCCLOtherIncomeHeadGLList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.HeadGLs
                     join t2 in _db.Head5 on t1.ParentId equals t2.Id

                     where t2.AccCode == "3101002001"
                     && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t2.AccCode + " -" + t2.AccName + " " + t1.AccCode + " -" + t1.AccName
                     }).ToList();
            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }
            return List;

        }



        public List<object> GCCLCashAndBankDropDownList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.Head5
                     join t2 in _db.Head4 on t1.ParentId equals t2.Id
                     where (t2.AccCode == "1301001" || t1.AccCode == "1301002002") && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }

            return List;

        }
        public List<object> CashAndBankDropDownList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.Head5
                     join t2 in _db.Head4 on t1.ParentId equals t2.Id
                     where (t2.AccCode == "1301001") && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }
            return List;
        }
        public Company GetCompanyById(int companyId)
        {
            var company = _db.Companies.Where(x => x.CompanyId == companyId).FirstOrDefault();
            return company;
        }

        public HeadGL GetHeadGLById(int id)
        {
            var gl = _db.HeadGLs.Where(x => x.Id == id).FirstOrDefault();
            return gl;
        }
        public List<object> KPLCashAndBankDropDownList(int companyId)
        {
            var List = new List<object>();
            var v = (from t1 in _db.Head5
                     join t2 in _db.Head4 on t1.ParentId equals t2.Id
                     where (t2.AccCode == "1301001") && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }

            return List;

        }



        public List<object> StockDropDownList(int companyId)
        {
            var List = new List<object>();

            if (companyId == (int)CompanyName.NaturalFishFarmingLimited)
            {
                var v = (from t1 in _db.Head5
                         where t1.AccCode == "1301004001" && t1.CompanyId == companyId
                         select new
                         {
                             Value = t1.Id,
                             Text = t1.AccCode + " -" + t1.AccName
                         }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidBazaarLimited)
            {
                var v = (from t1 in _db.Head5
                         where t1.AccCode == "1301005001" && t1.CompanyId == companyId
                         select new
                         {
                             Value = t1.Id,
                             Text = t1.AccCode + " -" + t1.AccName
                         }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.OrganicPoultryLimited || companyId == (int)CompanyName.SonaliOrganicDairyLimited)
            {
                var v = (from t1 in _db.Head5
                         join t2 in _db.Head4 on t1.ParentId equals t2.Id
                         where t2.AccCode == "1301004" && t1.CompanyId == companyId
                         select new
                         {
                             Value = t1.Id,
                             Text = t1.AccCode + " -" + t1.AccName
                         }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited)
            {
                var v = (from t1 in _db.Head5
                         join t2 in _db.Head4 on t1.ParentId equals t2.Id
                         where t2.AccCode == "1305001" && t1.CompanyId == companyId
                         select new
                         {
                             Value = t1.Id,
                             Text = t1.AccCode + " -" + t1.AccName
                         }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }

            if (companyId == (int)CompanyName.KrishibidFoodAndBeverageLimited)
            {
                var v = (from t1 in _db.Head5
                         join t2 in _db.Head4 on t1.ParentId equals t2.Id
                         join t3 in _db.Head3 on t2.ParentId equals t3.Id

                         where t3.AccCode == "1305" && t1.CompanyId == companyId
                         select new
                         {
                             Value = t1.Id,
                             Text = t1.AccCode + " -" + t1.AccName
                         }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                var v = (
                    from t1 in _db.Head5
                    join t2 in _db.Head4 on t1.ParentId equals t2.Id
                    where t2.AccCode == "1301005" && t1.CompanyId == companyId
                    select new
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + " -" + t1.AccName
                    }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidFisheriesLimited)
            {
                var v = (
                    from t1 in _db.Head5
                    where t1.AccCode == "1301005001" && t1.CompanyId == companyId
                    select new
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + " -" + t1.AccName
                    }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidPoultryLimited)
            {
                var v = (
                    from t1 in _db.Head5
                    where t1.AccCode == "1301005001" && t1.CompanyId == companyId
                    select new
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + " -" + t1.AccName
                    }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidTradingLimited)
            {
                var v = (
                    from t1 in _db.Head5
                    where t1.AccCode == "1305001001" && t1.CompanyId == companyId
                    select new
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + " -" + t1.AccName
                    }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }
            if (companyId == (int)CompanyName.KrishibidSafeFood)
            {
                var v = (
                    from t1 in _db.Head5

                    where t1.ParentId == 50611881 && t1.CompanyId == companyId
                    select new
                    {
                        Value = t1.Id,
                        Text = t1.AccCode + " -" + t1.AccName
                    }).ToList();

                foreach (var item in v)
                {
                    List.Add(new { Text = item.Text, Value = item.Value });
                }
            }

            return List;
        }
        public async Task<long> FishariseAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&

    t2.VoucherDate >= fromExistDate

                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50611909,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50611909,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> PoultryAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&

                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&

    t2.VoucherDate >= fromExistDate

                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50611914,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50611914,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> TradingAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&

    t2.VoucherDate >= fromExistDate

                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50611914,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50611914,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> SafeFoodAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head5.ParentId == 50611881
                                 && t1.IsActive && t2.IsActive && t2.IsStock &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     where t2.CompanyId == companyId && head5.ParentId == 50611881
                                     && t1.IsActive && t2.IsActive && t2.IsStock &&
                                     t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&

    t2.VoucherDate >= fromExistDate

                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50612090,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50612090,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }
            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }
        public async Task<List<HeadGLModel>> Head5Get(int companyId, int parentId)
        {

            List<HeadGLModel> head5List =
               await Task.Run(() => (from t1 in _db.Head5
                                     where t1.ParentId == parentId && t1.CompanyId == companyId
                                     select new HeadGLModel
                                     {
                                         Id = t1.Id,
                                         AccName = t1.AccCode + " -" + t1.AccName
                                     }).ToList());
            return head5List;
        }
        public async Task<List<HeadGLModel>> HeadGLGet(int companyId, int parentId)
        {

            List<HeadGLModel> headGLList =
               await Task.Run(() => (from t1 in _db.HeadGLs
                                     where t1.ParentId == parentId && t1.CompanyId == companyId
                                     select new HeadGLModel
                                     {
                                         Id = t1.Id,
                                         AccName = t1.AccCode + " -" + t1.AccName
                                     }).ToList());
            return headGLList;
        }
        public async Task<List<HeadGLModel>> HeadGLByHead5ParentIdGet(int companyId, int parentId)
        {

            List<HeadGLModel> HeadGLModelList =
               await Task.Run(() => (from t1 in _db.HeadGLs
                                     join t2 in _db.Head5 on t1.ParentId equals t2.Id
                                     where t2.ParentId == parentId && t1.CompanyId == companyId
                                     select new HeadGLModel
                                     {
                                         Id = t1.Id,
                                         AccName = t1.AccCode + " -" + t1.AccName
                                     }).ToList());
            return HeadGLModelList;
        }

        public async Task<List<HeadGLModel>> HeadGLByHeadGLParentIdGet(int companyId, int parentId)
        {

            List<HeadGLModel> HeadGLModelList =
               await Task.Run(() => (from t1 in _db.HeadGLs
                                     where t1.ParentId == parentId && t1.CompanyId == companyId
                                     select new HeadGLModel
                                     {
                                         Id = t1.Id,
                                         AccName = t1.AccCode + " -" + t1.AccName
                                     }).ToList());
            return HeadGLModelList;
        }

        public object GetAutoCompleteHeadGL(string prefix, int companyId)
        {
            var v = (from hgl in _db.HeadGLs
                     join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                     join h4 in _db.Head4 on h5.ParentId equals h4.Id

                     where hgl.CompanyId == companyId && hgl.IsActive && h5.IsActive && h4.IsActive
                     && ((hgl.AccName.Contains(prefix)) || (hgl.AccCode.Contains(prefix)))
                     select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).OrderBy(x => x.label).Take(200).ToList();

            return v;
        }

        public object GetAutoCompleteHeadGLForSupplier(string prefix, int companyId)
        {
            var v = (from hgl in _db.HeadGLs
                     join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                     join h4 in _db.Head4 on h5.ParentId equals h4.Id

                     where hgl.CompanyId == companyId
                     && ((h4.AccCode == "1301001") || (h5.AccCode == "2401001001"))
                     && hgl.IsActive
                     && h5.IsActive
                     && h4.IsActive
                     && ((hgl.AccName.Contains(prefix)) || (hgl.AccCode.Contains(prefix)))
                     select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).OrderBy(x => x.label).Take(200).ToList();

            return v;
        }

        public object GetAutoCompleteHeadGLForCustomer(string prefix, int companyId)
        {
            var v = (from hgl in _db.HeadGLs
                     join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                     join h4 in _db.Head4 on h5.ParentId equals h4.Id
                     join h3 in _db.Head3 on h4.ParentId equals h3.Id
                     where hgl.CompanyId == companyId
                     && ((h3.AccCode == "1304") || (h4.AccCode == "1301001"))
                     && hgl.IsActive
                     && h5.IsActive
                     && h4.IsActive
                     && ((hgl.AccName.Contains(prefix)) || (hgl.AccCode.Contains(prefix)))
                     select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).OrderBy(x => x.label).Take(200).ToList();

            return v;
        }

        public object GetAutoCompleteHeadGLForCollectionVoucherAsync(string prefix, int companyId)
        {
            //AccCode 42 //AccCode 43 //AccCode 44 //AccCode 45

            var allowedHead2Ids = new HashSet<int> {
                50613417, 50613418, 50613419, 50613420
            };

            // Use async execution to prevent blocking
            var v = (from hgl in _db.HeadGLs
                        join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                        join h4 in _db.Head4 on h5.ParentId equals h4.Id
                        join h3 in _db.Head3 on h4.ParentId equals h3.Id
                       
                        where hgl.CompanyId == companyId
                        && hgl.IsActive
                        && h5.IsActive
                        && h4.IsActive
                        && (h3.AccCode == "1304" || h4.AccCode == "1301001" || (h3.ParentId.HasValue && allowedHead2Ids.Contains(h3.ParentId.Value)))
                        && (hgl.AccName.StartsWith(prefix) || hgl.AccCode.StartsWith(prefix))
                        select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).OrderBy(x => x.label).Take(100).ToList();

            //var results = await query.ToListAsync();

            //var v = results.Select(x => new
            //{
            //    label = $"[{x.AccCode}] {(x.H4AccName == x.H5AccName ? x.H5AccName : $"{x.H4AccName} {x.H5AccName}")} {x.AccName}",
            //    val = x.Id
            //})
            //.OrderBy(x => x.label)
            //.Take(200)
            //.ToList();

            return v;
        }

        public object GetAutoCompleteHeadGLForSeedProcessingGet(string prefix, int companyId)
        {
            var v = (from hgl in _db.HeadGLs
                     join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                     join h4 in _db.Head4 on h5.ParentId equals h4.Id
                     join h3 in _db.Head3 on h4.ParentId equals h3.Id
                     where hgl.CompanyId == companyId
                     && h4.AccCode == "1301001"
                     && hgl.IsActive
                     && h5.IsActive
                     && h4.IsActive
                     && ((hgl.AccName.Contains(prefix)) || (hgl.AccCode.Contains(prefix)))
                     select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).OrderBy(x => x.label).Take(200).ToList();

            return v;
        }

        public object GetAccountHeadNameByAccountHeadId(int headGLId)
        {
            var v = (from hgl in _db.HeadGLs
                     join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                     join h4 in _db.Head4 on h5.ParentId equals h4.Id
                     join h3 in _db.Head3 on h4.ParentId equals h3.Id
                     where hgl.Id == headGLId
                     && hgl.IsActive
                     && h5.IsActive
                     && h4.IsActive
                     select new
                     {
                         label = "[" + hgl.AccCode + "] " + (h4.AccName == h5.AccName ? h5.AccName : h4.AccName + " " + h5.AccName) + " " + hgl.AccName,
                         val = hgl.Id
                     }).FirstOrDefault();

            return v;
        }

        public object GetAutoCompleteVendorHeadGL(string prefix, int companyId, int vendorTypeId)
        {
            var userId = Common.GetIntUserId();
            if (userId <= 0)
            {
                return new { };
            }

            // Pre-filter prefix to avoid case-sensitive comparisons and improve index usage
            var lowerPrefix = prefix?.ToLower() ?? string.Empty;

            var query = from hgl in _db.HeadGLs
                        join h5 in _db.Head5 on hgl.ParentId equals h5.Id
                        join h4 in _db.Head4 on h5.ParentId equals h4.Id
                        join ven in _db.Vendors on hgl.Id equals ven.HeadGLId
                        join tt in _db.SubZones on ven.SubZoneId equals tt.SubZoneId
                        join e in _db.Employees on tt.SalesOfficerId equals e.Id
                        where hgl.CompanyId == companyId
                            && hgl.IsActive
                            && h5.IsActive
                            && h4.IsActive
                            && ven.CompanyId == companyId
                            && tt.IsActive
                            && tt.CompanyId == companyId
                            && e.Active
                            && e.CompanyId == companyId
                            && e.Id == userId
                            && (hgl.AccName.ToLower().Contains(lowerPrefix) || hgl.AccCode.ToLower().Contains(lowerPrefix))
                        select new
                        {
                            AccCode = hgl.AccCode,
                            AccName = hgl.AccName,
                            H4AccName = h4.AccName,
                            H5AccName = h5.AccName,
                            Id = hgl.Id
                        };

            // Execute query and process results in memory for better performance
            var results = query
                .OrderBy(x => x.AccCode) // Order by simpler field first
                .Take(200)
                .ToList()
                .Select(x => new
                {
                    label = $"[{x.AccCode}] {(x.H4AccName == x.H5AccName ? x.H5AccName : $"{x.H4AccName} {x.H5AccName}")} {x.AccName}",
                    val = x.Id
                })
                .OrderBy(x => x.label)
                .ToList();

            return results;
        }


        public async Task<long> AutoInsertVoucherDetails(int voucherId, int virtualHeadId, string virtualHeadParticular, int? productCategory = null)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);
            double totalDebitAmount = _db.VoucherDetails.Where(x => x.VoucherId == voucherId && x.IsActive == true).Select(x => x.DebitAmount).DefaultIfEmpty(0).Sum();
            double totalCreditAmount = _db.VoucherDetails.Where(x => x.VoucherId == voucherId && x.IsActive == true).Select(x => x.CreditAmount).DefaultIfEmpty(0).Sum();
            double newAmount = 0;
            if (totalDebitAmount > totalCreditAmount)
            {
                newAmount = totalDebitAmount - totalCreditAmount;
            }
            else
            {
                newAmount = totalCreditAmount - totalDebitAmount;
            }
            if (newAmount > 0 && virtualHeadId > 0)
            {
                VoucherDetail voucherDetail = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = virtualHeadId,
                    CreditAmount = totalDebitAmount > totalCreditAmount ? newAmount : 0,
                    DebitAmount = totalCreditAmount > totalDebitAmount ? newAmount : 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true,
                    Particular = virtualHeadParticular,
                    ProductCategory = productCategory
                };
                _db.VoucherDetails.Add(voucherDetail);
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = voucherDetail.VoucherDetailId;
                }
            }

            return result;

        }



        public async Task<long> NFFLAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                 join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                 where t2.CompanyId == companyId && t4.AccCode == "1301004001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual
                                 && t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate


                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentstockExist = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                     join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                     where t2.CompanyId == companyId && t4.AccCode == "1301004001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                         t2.VoucherDate >= fromExistDate
                                         && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId
                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentstockExist.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true
                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50601365,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50601365,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> OPLAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                 join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                 join t5 in _db.Head4 on t4.ParentId equals t5.Id
                                 where t2.CompanyId == companyId && t5.AccCode == "1301004"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();

            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);

            var curentExistStock = (from t1 in _db.VoucherDetails
                                    join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                    join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                    join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                    join t5 in _db.Head4 on t4.ParentId equals t5.Id
                                    where t2.CompanyId == companyId && t5.AccCode == "1301004"
                                    && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                        t2.VoucherDate >= fromExistDate
                                        && t2.VoucherDate <= voucher.VoucherDate.Value
                                                 && t1.VoucherId != voucher.VoucherId

                                    select new
                                    {
                                        AccountHeadId = t1.AccountHeadId,
                                        DebitAmount = t1.DebitAmount
                                    }).AsEnumerable();
            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!curentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50607042,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50607042,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> SODLAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);

            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                 join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                 join t5 in _db.Head4 on t4.ParentId equals t5.Id
                                 where t2.CompanyId == companyId && t5.AccCode == "1301004"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);

            var existCorrentMonthStock = (from t1 in _db.VoucherDetails
                                          join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                          join t3 in _db.HeadGLs on t1.AccountHeadId equals t3.Id
                                          join t4 in _db.Head5 on t3.ParentId equals t4.Id
                                          join t5 in _db.Head4 on t4.ParentId equals t5.Id
                                          where t2.CompanyId == companyId && t5.AccCode == "1301004"
                                          && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                              t2.VoucherDate >= fromExistDate
                                              && t2.VoucherDate <= voucher.VoucherDate.Value
                                              && t1.VoucherId != voucher.VoucherId
                                          select new
                                          {
                                              AccountHeadId = t1.AccountHeadId,
                                              DebitAmount = t1.DebitAmount
                                          }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!existCorrentMonthStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50607044,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50607044,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> PrintingAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head4.AccCode == "1305001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);

            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     where t2.CompanyId == companyId && head4.AccCode == "1305001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                          t2.VoucherDate >= fromExistDate
                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();
            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50607050,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50607050,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);
            }

            _db.VoucherDetails.AddRange(voucherDetailList);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }
            return result;

        }

        public async Task<long> FBLAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 join head3 in _db.Head3 on head4.ParentId equals head3.Id

                                 where t2.CompanyId == companyId && head3.AccCode == "1305"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();

            var startDateOfCurrentMonth = new DateTime(voucher.VoucherDate.Value.Year, voucher.VoucherDate.Value.Month, 1);
            var endDateOfCurrentMonth = startDateOfCurrentMonth.AddMonths(1).AddDays(-1);

            //var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);

            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     join head3 in _db.Head3 on head4.ParentId equals head3.Id

                                     where t2.CompanyId == companyId && head3.AccCode == "1305"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual && t2.IsStock &&
                                         t2.VoucherDate >= startDateOfCurrentMonth && t2.VoucherDate <= endDateOfCurrentMonth
                                         && t1.VoucherId != voucher.VoucherId


                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId
                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,
                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true
                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,
                        AccountHeadId = 50607519,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true
                    };
                    voucherDetailList.Add(voucherDetail1);
                }
            }

            foreach (var item in currentStock)
            {

                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50607519,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }
            return result;
        }

        public async Task<long> PackagingAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id

                                 where t2.CompanyId == companyId && head4.AccCode == "1301005"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();

            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);

            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id

                                     where t2.CompanyId == companyId && head4.AccCode == "1301005"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                         t2.VoucherDate >= fromExistDate
                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();
            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50605003,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {
                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50605003,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> KBLAutoInsertStockVoucherDetails(int companyId, int voucherId)
        {
            long result = -1;

            var voucher = await _db.Vouchers.FindAsync(voucherId);

            var fromDate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day).AddDays(-10);
            var todate = voucher.VoucherDate.Value.AddDays(0 - voucher.VoucherDate.Value.Day);
            var priviousStock = (from t1 in _db.VoucherDetails
                                 join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                 join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                 join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                 join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                 where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                 && t1.IsActive && t2.IsActive && !t1.IsVirtual &&
                                 t2.Accounting_CostCenterFk == voucher.Accounting_CostCenterFk &&
                                     t2.VoucherDate >= fromDate
                                     && t2.VoucherDate <= todate

                                 select new
                                 {
                                     AccountHeadId = t1.AccountHeadId,
                                     DebitAmount = t1.DebitAmount
                                 }).ToList();
            var fromExistDate = voucher.VoucherDate.Value.AddDays(-10);
            var currentExistStock = (from t1 in _db.VoucherDetails
                                     join t2 in _db.Vouchers on t1.VoucherId equals t2.VoucherId
                                     join headGL in _db.HeadGLs on t1.AccountHeadId equals headGL.Id
                                     join head5 in _db.Head5 on headGL.ParentId equals head5.Id
                                     join head4 in _db.Head4 on head5.ParentId equals head4.Id
                                     where t2.CompanyId == companyId && head5.AccCode == "1301005001"
                                     && t1.IsActive && t2.IsActive && !t1.IsVirtual &&

    t2.VoucherDate >= fromExistDate

                                                  && t2.VoucherDate <= voucher.VoucherDate.Value
                                                  && t1.VoucherId != voucher.VoucherId

                                     select new
                                     {
                                         AccountHeadId = t1.AccountHeadId,
                                         DebitAmount = t1.DebitAmount
                                     }).AsEnumerable();

            var currentStock = (from t1 in _db.VoucherDetails
                                where t1.VoucherId == voucher.VoucherId && t1.IsActive
                                select new
                                {
                                    DebitAmount = t1.DebitAmount,
                                    AccountHeadId = t1.AccountHeadId

                                }).ToList();

            List<VoucherDetail> voucherDetailList = new List<VoucherDetail>();
            if (!currentExistStock.Any())
            {
                foreach (var item in priviousStock.Where(x => x.DebitAmount > 0))
                {
                    VoucherDetail voucherDetail = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = item.AccountHeadId,
                        CreditAmount = item.DebitAmount,
                        DebitAmount = 0,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail);
                    VoucherDetail voucherDetail1 = new VoucherDetail
                    {
                        VoucherId = voucher.VoucherId,

                        AccountHeadId = 50608410,
                        CreditAmount = 0,
                        DebitAmount = item.DebitAmount,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        IsVirtual = true


                    };
                    voucherDetailList.Add(voucherDetail1);

                }
            }

            foreach (var item in currentStock)
            {


                VoucherDetail voucherDetail1 = new VoucherDetail
                {
                    VoucherId = voucher.VoucherId,

                    AccountHeadId = 50608410,
                    CreditAmount = item.DebitAmount,
                    DebitAmount = 0,
                    TransactionDate = DateTime.Now,
                    IsActive = true
                };
                voucherDetailList.Add(voucherDetail1);

            }

            _db.VoucherDetails.AddRange(voucherDetailList);

            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;
            }

            return result;

        }

        public async Task<long> UpdateVoucherStatus(int voucherId)
        {
            long result = -1;
            Voucher voucher = await _db.Vouchers.FindAsync(voucherId);


            //var data = (from t1 in _db.ReportApprovalDetails
            //            join t2 in _db.ReportApprovals on t1.ReportApprovalId equals t2.ReportApprovalId
            //            where t2.CompanyId == voucher.CompanyId &&
            //                  t2.Month == voucher.VoucherDate.Value.Month &&
            //                  t2.Year == voucher.VoucherDate.Value.Year &&
            //                  t1.ApprovalStatus == 3 && t2.IsActive == true
            //            orderby t1.ReportApprovalDetail1 descending
            //            select new
            //            {
            //                ReportApprovalId = t2.ReportApprovalId,
            //                Month = t2.Month,
            //                Year = t2.Year,
            //                ReportApprovalDetailId = t1.ReportApprovalDetail1,
            //                Status = t1.ApprovalStatus,
            //            }).FirstOrDefault();



            //List<dynamic> data;
            //var query = (from t1 in _db.ReportApprovalDetails
            //             join t2 in _db.ReportApprovals on t1.ReportApprovalId equals t2.ReportApprovalId
            //             where t2.CompanyId == voucher.CompanyId &&
            //                   t2.Month == voucher.VoucherDate.Value.Month &&
            //                   t2.Year == voucher.VoucherDate.Value.Year &&
            //                   (t1.ApprovalStatus == 3 || t1.ApprovalStatus == 4)
            //             orderby t1.ReportApprovalDetail1 descending
            //             select new
            //             {
            //                 ReportApprovalId = t2.ReportApprovalId,
            //                 Month = t2.Month,
            //                 Year = t2.Year,
            //                 ReportApprovalDetailId = t1.ReportApprovalDetail1,
            //                 Status = t1.ApprovalStatus,
            //             });

            //var res = await query.ToListAsync();

            //if (res.Any(item => item.Status == 4))
            //{
            //    data = null; 
            //}
            //else
            //{
            //    data = res.Cast<dynamic>().ToList(); 
            //}


            var totalAmount = _db.VoucherDetails
                .Where(x => x.VoucherId == voucherId && x.IsActive == true && x.IsVirtual == false)
                .Select(x => x.CreditAmount)
                .DefaultIfEmpty(0)
                .Sum();

            voucher.VoucherStatus = "A";
            voucher.IsSubmit = true;
            voucher.TotalAmount = totalAmount;




            if (await _db.SaveChangesAsync() > 0)
            {
                result = voucher.VoucherId;

            }
            //if (data == null)
            //{

            //    //int erpSM = await SMSPush(voucher);
            //}
            //else
            //{
            //    result = -1;
            //}


            return result;

        }


        public async Task<long> DraftVoucherStatusSubmit(int voucherId)
        {
            long result = -1;
            Voucher voucher = await _db.Vouchers.FindAsync(voucherId);



            var totalAmount = _db.VoucherDetails
                .Where(x => x.VoucherId == voucherId && x.IsActive == true && x.IsVirtual == false)
                .Select(x => x.CreditAmount)
                .DefaultIfEmpty(0)
                .Sum();

            voucher.VoucherStatus = "A";
            voucher.IsSubmit = true;
            voucher.TotalAmount = totalAmount;

            if (voucher != null)
            {
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = voucher.VoucherId;

                }
                //int erpSM = await SMSPush(voucher);
            }
            else
            {
                result = -1;
            }


            return result;

        }


        private async Task<int> SMSPush(Voucher voucher)
        {
            if (voucher.CompanyId == (int)CompanyName.KrishibidSeedLimited)
            {
                string bankOrCashName = (from t1 in _db.VoucherDetails
                                         join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id
                                         join t3 in _db.Head5 on t2.ParentId equals t3.Id
                                         where t1.VoucherId == voucher.VoucherId
                                         && t3.ParentId == 29395
                                         select t2.AccName).FirstOrDefault();

                var dataList = (from t1 in _db.VoucherDetails
                                join t2 in _db.Vendors on t1.AccountHeadId equals t2.HeadGLId
                                join t3 in _db.Vouchers on t1.VoucherId equals t3.VoucherId
                                where t1.VoucherId == voucher.VoucherId
                                && t2.VendorTypeId == 2 // vendorTypeId
                                && t3.VoucherTypeId == 19// voucherTypeId                                
                                && t2.Phone != null
                                select new
                                {
                                    VoucherNo = t3.VoucherNo,
                                    CompanyId = t3.CompanyId,
                                    VoucherDate = t3.VoucherDate,
                                    PhoneNo = t2.Phone,
                                    CreditAmount = t1.CreditAmount,
                                    Particular = t1.Particular,
                                }).AsEnumerable();
                List<ErpSMS> erpSmsList = new List<ErpSMS>();
                foreach (var item in dataList)
                {
                    ErpSMS erpSms = new ErpSMS()
                    {

                        CompanyId = item.CompanyId.Value,
                        Subject = "Collection",
                        Date = item.VoucherDate.Value,
                        PhoneNo = item.PhoneNo.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("+88", String.Empty),
                        Message = "Dear customer, We have received from you Tk. " + item.CreditAmount + " /= Via " + bankOrCashName + " On " + item.Particular + " Thank you staying with Krishibid seed Ltd. If need, call: 01700729665",
                        SmsType = 1,
                        Status = (int)EnumSmSStatus.Pending,
                        Remarks = item.VoucherNo,
                        RowTime = DateTime.Now

                    };
                    erpSmsList.Add(erpSms);

                    //ErpSMS erpSms2 = new ErpSMS()
                    //{
                    //    CompanyId = item.CompanyId.Value,
                    //    Subject = "Collection",
                    //    Date = item.VoucherDate.Value,
                    //    PhoneNo = "01700729665",
                    //    Message = "Dear customer, We have received from you Tk. " + item.CreditAmount + " /= Via " + bankOrCashName + " On " + item.Particular + " Thank you staying with Krishibid seed Ltd. If need, call: 01700729665",
                    //    SmsType = 1,
                    //    Status = (int)EnumSmSStatus.Pending,
                    //    Remarks = item.VoucherNo,
                    //    RowTime = DateTime.Now

                    //};
                    //erpSmsList.Add(erpSms2);
                }
                try
                {
                    _db.ErpSMS.AddRange(erpSmsList);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }


            }

            if (voucher.CompanyId == (int)CompanyName.GloriousCropCareLimited)
            {
                string bankOrCashName = (from t1 in _db.VoucherDetails
                                         join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id
                                         join t3 in _db.Head5 on t2.ParentId equals t3.Id
                                         where t1.VoucherId == voucher.VoucherId
                                         && t3.ParentId == 43904
                                         select t2.AccName).FirstOrDefault();

                var dataList = (from t1 in _db.VoucherDetails
                                join t2 in _db.Vendors on t1.AccountHeadId equals t2.HeadGLId
                                join t3 in _db.Vouchers on t1.VoucherId equals t3.VoucherId
                                where t1.VoucherId == voucher.VoucherId
                                && t2.VendorTypeId == 2 // vendorTypeId
                                //&& t3.VoucherTypeId == 10 // voucherTypeId                                
                                && t2.Phone != null
                                select new
                                {
                                    VoucherNo = t3.VoucherNo,
                                    CompanyId = t3.CompanyId,
                                    VoucherDate = t3.VoucherDate,
                                    PhoneNo = t2.Phone,
                                    CreditAmount = t1.CreditAmount,
                                    Particular = t1.Particular,
                                }).AsEnumerable();
                List<ErpSMS> erpSmsList = new List<ErpSMS>();
                foreach (var item in dataList)
                {
                    ErpSMS erpSms = new ErpSMS()
                    {

                        CompanyId = item.CompanyId.Value,
                        Subject = "Collection",
                        Date = item.VoucherDate.Value,
                        PhoneNo = item.PhoneNo.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("+88", String.Empty),
                        Message = "Dear customer, We have received from you Tk. " + item.CreditAmount + " /= Via " + bankOrCashName + " On " + item.Particular + " Thank you staying with Glorious Crop Care Limited. If need, call: 01700729903",
                        SmsType = 1,
                        Status = (int)EnumSmSStatus.Pending,
                        Remarks = item.VoucherNo,
                        RowTime = DateTime.Now

                    };
                    erpSmsList.Add(erpSms);

                }
                try
                {
                    _db.ErpSMS.AddRange(erpSmsList);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }

            }

            if (voucher.CompanyId == (int)CompanyName.KrishibidFeedLimited)
            {
                string bankOrCashName = (from t1 in _db.VoucherDetails
                                         join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id
                                         join t3 in _db.Head5 on t2.ParentId equals t3.Id
                                         where t1.VoucherId == voucher.VoucherId
                                         && t3.ParentId == 3290
                                         select t2.AccName).FirstOrDefault();

                var dataList = (from t1 in _db.VoucherDetails
                                join t2 in _db.Vendors on t1.AccountHeadId equals t2.HeadGLId
                                join t3 in _db.Vouchers on t1.VoucherId equals t3.VoucherId
                                where t1.VoucherId == voucher.VoucherId
                                && t2.VendorTypeId == 2 // vendorTypeId
                                //&& t3.VoucherTypeId == 10 // voucherTypeId                                
                                && t2.Phone != null
                                select new
                                {
                                    VoucherNo = t3.VoucherNo,
                                    CompanyId = t3.CompanyId,
                                    VoucherDate = t3.VoucherDate,
                                    PhoneNo = t2.Phone,
                                    CreditAmount = t1.CreditAmount,
                                    Particular = t1.Particular,
                                }).AsEnumerable();
                List<ErpSMS> erpSmsList = new List<ErpSMS>();
                foreach (var item in dataList)
                {
                    ErpSMS erpSms = new ErpSMS()
                    {

                        CompanyId = item.CompanyId.Value,
                        Subject = "Collection",
                        Date = item.VoucherDate.Value,
                        PhoneNo = item.PhoneNo.Replace(" ", String.Empty).Replace("-", String.Empty).Replace("+88", String.Empty),
                        Message = "Dear customer, We have received from you Tk. " + item.CreditAmount + " /= Via " + bankOrCashName + " On " + item.Particular + " Thank you staying with Krishibid Seed Limited. If need, call: 01700729195",
                        SmsType = 1,
                        Status = (int)EnumSmSStatus.Pending,
                        Remarks = item.VoucherNo,
                        RowTime = DateTime.Now

                    };
                    erpSmsList.Add(erpSms);

                }
                try
                {
                    _db.ErpSMS.AddRange(erpSmsList);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }

            }
            return 1;
        }

        public async Task<VMJournalSlave> GetSingleVoucherDetails(int voucherDetailId)
        {
            var v = await Task.Run(() => (from t1 in _db.VoucherDetails
                                          join t2 in _db.HeadGLs on t1.AccountHeadId equals t2.Id


                                          where t1.VoucherDetailId == voucherDetailId
                                          select new VMJournalSlave
                                          {
                                              VoucherId = t1.VoucherId.Value,
                                              VoucherDetailId = t1.VoucherDetailId,
                                              Accounting_HeadFK = t1.AccountHeadId.Value,
                                              AccountingHeadName = "[" + t2.AccCode + "] " + t2.AccName,
                                              Debit = t1.DebitAmount,
                                              Credit = t1.CreditAmount,
                                              Particular = t1.Particular
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<VMJournalSlave> GetSingleVoucher(int voucherId)
        {
            var v = await Task.Run(() => (from t1 in _db.Vouchers.Where(f => f.VoucherId == voucherId)
                                          select new VMJournalSlave
                                          {
                                              VoucherId = t1.VoucherId,
                                              VoucherNo = t1.VoucherNo,
                                              VoucherTypeId = t1.VoucherTypeId.Value,
                                              ChqDate = t1.ChqDate,
                                              ChqName = t1.ChqName,
                                              Narration = t1.Narration
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<VoucherTypeModel> GetSingleVoucherTypes(int voucherTypesId)
        {
            var v = await Task.Run(() => (from t1 in _db.VoucherTypes
                                          where t1.VoucherTypeId == voucherTypesId
                                          select new VoucherTypeModel
                                          {
                                              Code = t1.Code,
                                              IsBankOrCash = t1.IsBankOrCash,
                                              VoucherTypeId = t1.VoucherTypeId,
                                              Name = t1.Name,
                                              IsActive = t1.IsActive
                                          }).FirstOrDefault());
            return v;
        }

        public async Task<long> AccountingJournalPushGCCL(DateTime journalDate, int CompanyFK, int drHeadID, List<AccountList> crHead, decimal amount, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = Convert.ToDouble(amount),
                Credit = 0,
                Accounting_HeadFK = drHeadID
            });
            foreach (var item in crHead)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = description,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.Value),
                    Accounting_HeadFK = Convert.ToInt32(item.AccountingHeadId)
                });
            }
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }

        public async Task<long> AccountingProductionPushGCCL(DateTime journalDate, int CompanyFK, VMProdReferenceSlave vmProdReferenceSlave, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Raw Item Cr Integration Dr
            foreach (var item in vmProdReferenceSlave.FinishDataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.TotalPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = vmProdReferenceSlave.RawDataListSlave.Any() ? Convert.ToDouble(vmProdReferenceSlave.RawDataListSlave.Sum(x => x.TotalPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50606113 // ERP Raw Dr Integration
            });
            #endregion

            #region Production Manager Cr Factory Expenses Dr
            foreach (var item in vmProdReferenceSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.FactoryExpecsesHeadName,
                    Debit = Convert.ToDouble(item.FectoryExpensesAmount),
                    Credit = 0,
                    Accounting_HeadFK = item.FactoryExpensesHeadGLId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = vmProdReferenceSlave.DataListSlave.Any() ? Convert.ToDouble(vmProdReferenceSlave.DataListSlave.Sum(x => x.FectoryExpensesAmount)) : 0,
                Accounting_HeadFK = vmProdReferenceSlave.AdvanceHeadGLId.Value
            });
            #endregion

            #region Integration Cr Finish Item Dr
            foreach (var item in vmProdReferenceSlave.FinishDataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName,
                    Debit = Convert.ToDouble(((item.Quantity + item.QuantityOver) - item.QuantityLess) * item.CostingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = vmProdReferenceSlave.FinishDataListSlave.Any() ? Convert.ToDouble(vmProdReferenceSlave.FinishDataListSlave.Sum(x => ((x.Quantity + x.QuantityOver) - x.QuantityLess) * x.CostingPrice)) : 0,
                Accounting_HeadFK = 50606113 // ERP Integration
            });
            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }


        public async Task<long> AccountingProductionPushISS(DateTime journalDate, int CompanyFK, VMProdReferenceSlave vmProdReferenceSlave, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Raw Item Cr Integration Dr

            foreach (var group in vmProdReferenceSlave.FinishDataListSlave.GroupBy(x => x.ProductCategoryId))
            {
                var firstItem = group.First();
                var totalQuantity = group.Sum(x => x.Quantity);
                var costingPrice = firstItem.CostingPrice;


                if (costingPrice > 0 && firstItem.AccountingHeadId.HasValue)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = firstItem.ProductName,
                        Debit = Convert.ToDouble(totalQuantity * costingPrice),
                        Credit = 0,
                        Accounting_HeadFK = firstItem.AccountingHeadId.Value
                    });
                }
            }

            var totalSum = vmProdReferenceSlave.FinishDataListSlave.Any()
                ? vmProdReferenceSlave.FinishDataListSlave.Sum(x => x.Quantity) * vmProdReferenceSlave.FinishDataListSlave.First().CostingPrice
                : 0;
            var storeStockAccHead = _db.HeadGLs.Where(x => x.CompanyId == CompanyFK && x.AccCode == "4701001001001" && x.IsActive).FirstOrDefault();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = Convert.ToDouble(totalSum),
                Accounting_HeadFK = storeStockAccHead.Id // ERP Raw Dr Integration  
            });
            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            result = resultData.VoucherId;
            return result;
        }

        public async Task<long> AccountingProductionExpencePush(DateTime journalDate, int CompanyFK, VMProdReferenceSlave vmProdReferenceSlave, string title, string description, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = title,
                Narration = description,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();



            #region Production Manager Cr Factory Expenses Dr
            foreach (var item in vmProdReferenceSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.FactoryExpecsesHeadName,
                    Debit = Convert.ToDouble(item.FectoryExpensesAmount),
                    Credit = 0,
                    Accounting_HeadFK = item.FactoryExpensesHeadGLId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = description,
                Debit = 0,
                Credit = vmProdReferenceSlave.DataListSlave.Any() ? Convert.ToDouble(vmProdReferenceSlave.DataListSlave.Sum(x => x.FectoryExpensesAmount)) : 0,
                Accounting_HeadFK = vmProdReferenceSlave.AdvanceHeadGLId.Value
            });


            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var ProductionDetailVoucherStatusUpdate = await _db.ProductionDetails.FirstAsync(x => x.ProductionDetailId == vmProdReferenceSlave.ID);

                ProductionDetailVoucherStatusUpdate.IsVoucher = true;
                _db.Entry(ProductionDetailVoucherStatusUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return resultData.VoucherId;
        }

        public async Task<long> AccountiingPurchasePushFeed(int CompanyFK, VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {

                JournalType = journalType,
                Title = "<a href='" + _urlInfo + "Report/GetPurchaseOrderTemplateReport?purchaseOrderId=" + vmWareHousePOReceivingSlave.Procurement_PurchaseOrderFk + "&EXPORT=EXPORT&reportType=PDF'>" + vmWareHousePOReceivingSlave.POCID + "</a>" + " Date: " + vmWareHousePOReceivingSlave.PODate.Value.ToShortDateString(),
                Narration = " MRR No: " + vmWareHousePOReceivingSlave.ChallanCID + " Challan No: " + vmWareHousePOReceivingSlave.Challan + " Date: " + vmWareHousePOReceivingSlave.ReceivedDate.ToShortDateString() + " Received By: " + vmWareHousePOReceivingSlave.EmployeeName,
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReceivingSlave.ReceivedDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                strList.Add(item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.StockInQty + " " + item.StockInRate);
            }
            string perticular = String.Join(", ", strList.ToArray());

            double totalAmount = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.StockInQty * x.StockInRate)) : 0;
            double totalDeduction = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.StockInQty * x.StockInRate * (x.Deduction / 100))) : 0;
            decimal truckFare = vmWareHousePOReceivingSlave.TruckFare;
            decimal labourBill = vmWareHousePOReceivingSlave.LabourBill;
            decimal TDSDeduction = Convert.ToDecimal(totalAmount) / 100 * vmWareHousePOReceivingSlave.TDSDeduction;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular + " Amount: " + totalAmount + " Deduction: " + totalDeduction + " Truck Fare: " + truckFare + " labour Bill: " + labourBill,
                Debit = 0,
                Credit = totalAmount - (Convert.ToDouble(truckFare) + Convert.ToDouble(labourBill) + totalDeduction + Convert.ToDouble(TDSDeduction)),
                Accounting_HeadFK = vmWareHousePOReceivingSlave.AccountingHeadId.Value, //Supplier/ LC

            });
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.StockInQty + " " + item.StockInRate + " Amount: " + totalAmount + " Deduction: " + totalDeduction + " Truck Fare: " + truckFare + " labour Bill: " + labourBill,
                    Debit = Convert.ToDouble(item.StockInQty * item.StockInRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.StockInQty + " " + item.StockInRate + " Amount: " + totalAmount + " Deduction: " + totalDeduction + " Truck Fare: " + truckFare + " labour Bill: " + labourBill,
                    Debit = Convert.ToDouble(item.StockInQty * item.StockInRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true

                });
            }
            string info = "Challan: " + vmWareHousePOReceivingSlave.Challan + " Challan Date: " + vmWareHousePOReceivingSlave.ChallanDate.ToString() + " PO NO: " + vmWareHousePOReceivingSlave.POCID + " PO Date " + vmWareHousePOReceivingSlave.PODate.ToString();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = info,
                Debit = 0,
                Credit = Convert.ToDouble(labourBill),
                Accounting_HeadFK = 50613304, //Labour Bill Payable (RM)


            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = info,
                Debit = 0,
                Credit = Convert.ToDouble(truckFare),
                Accounting_HeadFK = 50613302, //Truck Fare Payable (RM)


            });
            if (TDSDeduction > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = info + "Supplier Name: " + vmWareHousePOReceivingSlave.SupplierName,
                    Debit = 0,
                    Credit = Convert.ToDouble(TDSDeduction),
                    Accounting_HeadFK = 50619946, //TDS Payable Against Supplier


                });

            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = info,
                Debit = 0,
                Credit = totalAmount + totalDeduction,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Cr
                IsVirtual = true

            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmWareHousePOReceivingSlave.CompanyFK.Value, vmWareHousePOReceivingSlave.MaterialReceiveId, vmWareHousePOReceivingSlave.IntegratedFrom);
            }
            return resultData.VoucherId;
        }
        public async Task<long> PackagingMRLCPush(int CompanyFK, VMWareHousePOReceivingSlave receivingVm, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = receivingVm.LcNo + " L/C Date: " + receivingVm.lcDate + " PI No: " + receivingVm.PiNO + " PI Date: " + receivingVm.PIDate,
                Narration = " MRR No: " + receivingVm.ChallanCID + " Challan No: " + receivingVm.Challan + " Date: " + receivingVm.ChallanDate.Value.ToShortDateString() + " Received By: " + receivingVm.EmployeeName,
                CompanyFK = CompanyFK,
                Date = receivingVm.ChallanDate,
                IsSubmit = true
            };
            decimal totalLandedValue = Math.Round(((receivingVm.InsuranceValue ?? 0) * receivingVm.CurrenceyRate), 2) +
                                   Math.Round(((receivingVm.CommissionValue ?? 0) * receivingVm.CurrenceyRate), 2) +
                                   Math.Round(((receivingVm.BankCharge ?? 0) * receivingVm.CurrenceyRate), 2) +
                                   Math.Round(((receivingVm.OtherCharge ?? 0) * receivingVm.CurrenceyRate), 2) +
                                   Math.Round((receivingVm.FinancialCharge ?? 0), 2) +
                                   Math.Round(receivingVm.TruckFare, 2) +
                                   Math.Round(receivingVm.LabourBill, 2) +
                                   Math.Round((receivingVm.CandFBill ?? 0), 2) +
                                   Math.Round((receivingVm.WareHouseRentBill ?? 0), 2);



            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Costing Head
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Insurance Value: " + Convert.ToDouble(Math.Round((receivingVm.InsuranceValue ?? 0) * receivingVm.CurrenceyRate, 2)),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.InsuranceValue ?? 0) * receivingVm.CurrenceyRate, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Commission Value: " + Convert.ToDouble(Math.Round((receivingVm.CommissionValue ?? 0) * receivingVm.CurrenceyRate, 2)),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.CommissionValue ?? 0) * receivingVm.CurrenceyRate, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Bank Charge: " + Convert.ToDouble(Math.Round((receivingVm.BankCharge ?? 0) * receivingVm.CurrenceyRate, 2)),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.BankCharge ?? 0) * receivingVm.CurrenceyRate, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Other Charge: " + Convert.ToDouble(Math.Round((receivingVm.OtherCharge ?? 0) * receivingVm.CurrenceyRate, 2)),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.OtherCharge ?? 0) * receivingVm.CurrenceyRate, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Financial Charge: " + receivingVm.FinancialCharge,
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round(receivingVm.FinancialCharge ?? 0, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Transport: " + receivingVm.TruckFare,
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round(receivingVm.TruckFare, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " Load/Unload Cost: " + receivingVm.LabourBill,
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round(receivingVm.LabourBill, 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " C&F Bill: " + Math.Round((receivingVm.CandFBill ?? 0), 2),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.CandFBill ?? 0), 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = " WareHouse Rent: " + Math.Round((receivingVm.WareHouseRentBill ?? 0), 2),
                Debit = 0,
                Credit = Convert.ToDouble(Math.Round((receivingVm.WareHouseRentBill ?? 0), 2)),
                Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
            });

            //Product
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(item.SubTotalInBDT, 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }


            #endregion
            // Stock 
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = Convert.ToDouble(Math.Round((Math.Round(item.SubTotalInBDT, 2) + (totalLandedValue * Math.Round(item.SubTotalInBDT, 2)) / Math.Round(receivingVm.TotalBDTPrice, 2)), 2)),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            // Purchase 
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = Convert.ToDouble(Math.Round((Math.Round(item.SubTotalInBDT, 2) + (totalLandedValue * Math.Round(item.SubTotalInBDT, 2)) / Math.Round(receivingVm.TotalBDTPrice, 2)), 2)),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = receivingVm.LcNo + " L/C Date: " + receivingVm.lcDate + " PI No: " + receivingVm.PiNO + " PI Date: " + receivingVm.PIDate,
                Debit = 0,
                Credit = Convert.ToDouble(receivingVm.DataListSlave.Any() ? (receivingVm.DataListSlave.Sum(x => Math.Round(x.SubTotalInBDT, 2)) + totalLandedValue) : 0),
                Accounting_HeadFK = 50605003 //Packaging Stock Adjust
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, receivingVm.CompanyFK.Value, receivingVm.MaterialReceiveId, receivingVm.IntegratedFrom);
            }
            return resultData.VoucherId;
        }

        public async Task<long> MaterialReceivedViaLCPushKFMAL(int CompanyFK, VMWareHousePOReceivingSlave receivingVm, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = receivingVm.LcNo + " L/C Date: " + receivingVm.lcDate + " PI No: " + receivingVm.PiNO + " PI Date: " + receivingVm.PIDate,
                Narration = " MRR No: " + receivingVm.ChallanCID + " Challan No: " + receivingVm.Challan + " Date: " + receivingVm.ChallanDate.Value.ToShortDateString() + " Received By: " + receivingVm.EmployeeName,
                CompanyFK = CompanyFK,
                Date = receivingVm.ChallanDate,
                IsSubmit = true
            };


            decimal totalLandedValue = receivingVm.BankCharge.Value +
                                        receivingVm.FreightCharges.Value +
                                        receivingVm.InsuranceValue.Value +
                                        receivingVm.CommissionValue.Value +
                                        receivingVm.OtherCharge.Value +

                                        receivingVm.FinancialCharge.Value +
                                        receivingVm.TruckFare +
                                        receivingVm.LabourBill +
                                        receivingVm.CandFBill.Value +
                                        receivingVm.ValueAdjustment.Value +
                                        receivingVm.WareHouseRentBill.Value +
                                        receivingVm.TotalLCAmendment
                                        ;



            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Costing Head
            if (receivingVm.FreightCharges.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Freight Charges: " + Convert.ToDouble(receivingVm.FreightCharges.Value),
                    Debit = 0,
                    Credit = Convert.ToDouble(receivingVm.FreightCharges.Value),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }

            if (receivingVm.BankCharge.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Bank Charge: " + Convert.ToDouble(receivingVm.BankCharge.Value),
                    Debit = 0,
                    Credit = Convert.ToDouble(receivingVm.BankCharge.Value),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.CommissionValue.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Commission Value: " + Convert.ToDouble(receivingVm.CommissionValue.Value),
                    Debit = 0,
                    Credit = Convert.ToDouble(receivingVm.CommissionValue.Value),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }

            if (receivingVm.InsuranceValue.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Insurance Value: " + Convert.ToDouble(receivingVm.InsuranceValue.Value),
                    Debit = 0,
                    Credit = Convert.ToDouble(receivingVm.InsuranceValue.Value),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.OtherCharge.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Other Charge: " + Convert.ToDouble(receivingVm.OtherCharge.Value),
                    Debit = 0,
                    Credit = Convert.ToDouble(receivingVm.OtherCharge.Value),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.LCAmendmentList.Any())
            {
                foreach (var item in receivingVm.LCAmendmentList)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.Remarks + "Amendment Date: " + item.AmendmentDate + Convert.ToDouble(item.AmendmentValue),
                        Debit = 0,
                        Credit = Convert.ToDouble(item.AmendmentValue),
                        Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                    });
                }
            }

            if (receivingVm.TruckFare > 0)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Transport: " + receivingVm.TruckFare,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(receivingVm.TruckFare, 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }

            if (receivingVm.LabourBill > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Load/Unload Cost: " + receivingVm.LabourBill,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(receivingVm.LabourBill, 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.CandFBill.Value > 0)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " C&F Bill: " + Math.Round((receivingVm.CandFBill.Value), 2),
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round((receivingVm.CandFBill.Value), 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.WareHouseRentBill.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " WareHouse Rent: " + Math.Round((receivingVm.WareHouseRentBill.Value), 2),
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round((receivingVm.WareHouseRentBill.Value), 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }
            if (receivingVm.FinancialCharge.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Financial Charge: " + receivingVm.FinancialCharge.Value,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(receivingVm.FinancialCharge.Value, 2)),
                    Accounting_HeadFK = 50622295 // Prov. Financial Charge Letter of Credit (LC)
                });
            }
            if (receivingVm.ValueAdjustment.Value > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Value Adjustment: " + receivingVm.ValueAdjustment.Value,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(receivingVm.ValueAdjustment.Value, 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }

            //Product
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = 0,
                    Credit = Convert.ToDouble(Math.Round(item.SubTotalInBDT, 2)),
                    Accounting_HeadFK = receivingVm.AccountingHeadId.Value //Supplier/ LC
                });
            }


            #endregion
            // Stock 
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = Convert.ToDouble(Math.Round((Math.Round(item.SubTotalInBDT, 2) + (totalLandedValue * Math.Round(item.SubTotalInBDT, 2)) / Math.Round(receivingVm.TotalBDTPrice, 2)), 2)),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            // Purchase 
            foreach (var item in receivingVm.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + item.ReceivedQuantity + " " + item.PurchasingPrice + " Currencey" + receivingVm.CurancYName + " Rate" + receivingVm.CurrenceyRate,
                    Debit = Convert.ToDouble(Math.Round((Math.Round(item.SubTotalInBDT, 2) + (totalLandedValue * Math.Round(item.SubTotalInBDT, 2)) / Math.Round(receivingVm.TotalBDTPrice, 2)), 2)),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = receivingVm.LcNo + " L/C Date: " + receivingVm.lcDate + " PI No: " + receivingVm.PiNO + " PI Date: " + receivingVm.PIDate,
                Debit = 0,
                Credit = Convert.ToDouble(receivingVm.DataListSlave.Any() ? (receivingVm.DataListSlave.Sum(x => Math.Round(x.SubTotalInBDT, 2)) + totalLandedValue) : 0),
                Accounting_HeadFK = 50616077 //KFMAL Stock Adjust
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, receivingVm.CompanyFK.Value, receivingVm.MaterialReceiveId, receivingVm.IntegratedFrom);
            }
            return resultData.VoucherId;
        }
        public async Task<long> AccountiingProductTpPricePushFeed(WareHouseProductPriceVm model, int journalType)
        {

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "Product : " + model.ProductName + " Current Stock : " + model.StockQuantity,
                Narration = "Previous Date :" + model.PreviousPriceDate.ToShortDateString() + "[" + model.PreviousPrice + "] to Update Date: " + model.PriceUpdateDate.ToShortDateString() + "[" + model.UpdatePrice + "]",
                CompanyFK = model.CompanyId,
                Date = model.PriceUpdateDate,
                IsSubmit = true
            };

            double amount = Convert.ToDouble((model.StockQuantity * model.UpdatePrice) - (model.StockQuantity * model.PreviousPrice));

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = model.ProductName,
                Debit = amount > 0 ? amount : 0,
                Credit = amount > 0 ? 0 : Math.Abs(amount),
                Accounting_HeadFK = model.AccountingHeadId.Value
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = amount > 0 ? 0 : Math.Abs(amount),
                Credit = amount > 0 ? amount : 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Cr

            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, model.CompanyId, model.PriceId, model.IntegratedFrom);
            }
            return resultData.VoucherId;
        }
        public async Task<long> AccountiingIssuePushFeed(int CompanyFK, WareHouseIssueSlaveVm wareHouseIssueSlave, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "<a href='" + _urlInfo + "Report/GetIssueReport?issueId=" + wareHouseIssueSlave.IssueId + "&EXPORT=EXPORT&reportType=PDF'>" + wareHouseIssueSlave.IssueNo + "</a>" + " Date: " + wareHouseIssueSlave.IssueDate.ToShortDateString(),
                Narration = wareHouseIssueSlave.IssueNo + " Date: " + wareHouseIssueSlave.IssueDate.ToShortDateString(),
                CompanyFK = CompanyFK,
                Date = wareHouseIssueSlave.IssueDate,
                IsSubmit = true
            };
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            foreach (var item in wareHouseIssueSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.Quantity * item.UnitPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = wareHouseIssueSlave.DataListSlave.Any() ? Convert.ToDouble(wareHouseIssueSlave.DataListSlave.Sum(x => x.Quantity * x.UnitPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Cr
                IsVirtual = true

            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, wareHouseIssueSlave.CompanyId, wareHouseIssueSlave.IssueId, wareHouseIssueSlave.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingPurchasePushGCCL(int CompanyFK, VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "<a href='" + _urlInfo + "Report/GCCLPurchseInvoiceReport?companyId=" + CompanyFK + "&materialReceiveId=" + vmWareHousePOReceivingSlave.MaterialReceiveId + "&reportName=GCCLPurchaseInvoiceReports'>" + vmWareHousePOReceivingSlave.POCID + "</a>" + " Date: " + vmWareHousePOReceivingSlave.PODate.ToString(),
                Narration = vmWareHousePOReceivingSlave.Challan + " Date: " + vmWareHousePOReceivingSlave.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReceivingSlave.ChallanDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = vmWareHousePOReceivingSlave.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = 50606113 //GCCL Stock Adjust With Erp Cr
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmWareHousePOReceivingSlave.CompanyFK.Value, vmWareHousePOReceivingSlave.MaterialReceiveId, vmWareHousePOReceivingSlave.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingPurchaseReturnPushGCCL(int CompanyFK, VMWareHousePOReturnSlave vmWareHousePOReturnSlave, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmWareHousePOReturnSlave.ReturnNo + " Date: " + vmWareHousePOReturnSlave.ReturnDate.ToString(),
                Narration = vmWareHousePOReturnSlave.Challan + " Date: " + vmWareHousePOReturnSlave.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReturnSlave.ReturnDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReturnSlave.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmWareHousePOReturnSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReturnSlave.DataListSlave.Sum(x => x.ReturnQuantity * x.Rate)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmWareHousePOReturnSlave.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmWareHousePOReturnSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.Rate),
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReturnSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.COGS),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = vmWareHousePOReturnSlave.DataListSlave.Any() ? Convert
                .ToDouble(vmWareHousePOReturnSlave.DataListSlave
                .Sum(x => x.ReturnQuantity * x.COGS)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50606113 //GCCL Stock Adjust With Erp Cr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmWareHousePOReturnSlave.CompanyFK.Value, vmWareHousePOReturnSlave.PurchaseReturnId, "PurchaseReturn");

            }


            return resultData.VoucherId;
        }


        public async Task<long> AccountingPurchaseReturnPushSeed(int CompanyFK, VMWareHousePOReturnSlave model, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = model.ReturnNo + " Date: " + model.ReturnDate.ToString(),
                Narration = model.Challan + " Date: " + model.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = model.ReturnDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in model.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = model.DataListSlave.Any() ? Convert.ToDouble(model.DataListSlave.Sum(x => x.ReturnQuantity * x.Rate)) : 0,
                Credit = 0,
                Accounting_HeadFK = model.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in model.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.Rate),
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in model.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.COGS),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = model.DataListSlave.Any() ? Convert
                .ToDouble(model.DataListSlave
                .Sum(x => x.ReturnQuantity * x.COGS)) : 0,
                Credit = 0,
                Accounting_HeadFK = 43576 //seed Stock Adjust With Erp Cr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, model.CompanyFK.Value, model.PurchaseReturnId, "PurchaseReturn");


            }


            return resultData.VoucherId;
        }



        public async Task<long> AccountingMaterialIssuePushFeed(int CompanyFK, VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmWareHousePOReceivingSlave.POCID + " Date: " + vmWareHousePOReceivingSlave.PODate.ToString(),
                Narration = vmWareHousePOReceivingSlave.Challan + " Date: " + vmWareHousePOReceivingSlave.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReceivingSlave.ChallanDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = vmWareHousePOReceivingSlave.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = 43576 //Seed Stock Adjust With Erp Cr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }
        public async Task<long> AccountingPurchasePushSEED(int CompanyFK, VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "<a href='" + _urlInfo + "Report/GCCLPurchseInvoiceReport?companyId=" + CompanyFK + "&materialReceiveId=" + vmWareHousePOReceivingSlave.MaterialReceiveId + "&reportName=GCCLPurchaseInvoiceReports'>" + vmWareHousePOReceivingSlave.POCID + "</a>" + " Date: " + vmWareHousePOReceivingSlave.PODate.ToString(),
                Narration = vmWareHousePOReceivingSlave.Challan + " Date: " + vmWareHousePOReceivingSlave.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReceivingSlave.ChallanDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = vmWareHousePOReceivingSlave.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.ReceivedQuantity * item.PurchasingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.ReceivedQuantity * x.PurchasingPrice)) : 0,
                Accounting_HeadFK = 43576 //Seed Stock Adjust With Erp Cr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmWareHousePOReceivingSlave.CompanyFK.Value, vmWareHousePOReceivingSlave.MaterialReceiveId, vmWareHousePOReceivingSlave.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccSalesPushKfmal(int CompanyFK, VMOrderDeliverDetail vmOrderDeliverDetail, int journalType)
        {
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmOrderDeliverDetail.OrderNo + " Date: " + vmOrderDeliverDetail.OrderDate.ToString("MM/dd/yyyy"),
                Narration = vmOrderDeliverDetail.ChallanNo + " Date: " + vmOrderDeliverDetail.DeliveryDate.Value.ToString("MM/dd/yyyy") + " Payment Method: " + ((VendorsPaymentMethodEnum)vmOrderDeliverDetail.PaymentMethod).ToString(),
                CompanyFK = CompanyFK,
                Date = vmOrderDeliverDetail.DeliveryDate,
                IsSubmit = true,
            };

            double unitDiscount = vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.DiscountUnit));
            double cashDiscount = vmOrderDeliverDetail.DataListDetail.Sum(item => item.DiscountUnit > 0 ? (((item.DeliveredQty * item.UnitPrice) - (item.DeliveredQty * Convert.ToDouble(item.DiscountUnit))) / 100 * Convert.ToDouble(item.DiscountRate ?? 0)) : ((item.DeliveredQty * item.UnitPrice) / 100 * Convert.ToDouble(item.DiscountRate ?? 0)));
            double spetialDiscount = vmOrderDeliverDetail.DataListDetail.Sum(item => Convert.ToDouble(item.SpecialDiscount ?? 0));

            List<string> strList = new List<string>();
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                string s = "Product: " + item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray())) + " Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount;
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? ((vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * x.UnitPrice)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmOrderDeliverDetail.AccountingHeadId.Value //Customer/ LC
            });

            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice,
                    Debit = 0,
                    Credit = (item.DeliveredQty * item.UnitPrice),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount,
                Debit = unitDiscount + cashDiscount + spetialDiscount,
                Credit = 0,
                Accounting_HeadFK = 50604937 // Sales Discount
            });
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Costing Price: " + item.COGSPrice,
                    Debit = 0,
                    Credit = item.DeliveredQty * Convert.ToDouble(item.COGSPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.COGSPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50616077, //KFMAL Stock Adjust
                IsVirtual = true
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmOrderDeliverDetail.CompanyFK.Value, vmOrderDeliverDetail.OrderDeliverId, vmOrderDeliverDetail.IntegratedFrom);


            }


            return resultData.VoucherId;
        }


        public async Task<long> AccServiceSalesPushKfmal(int CompanyFK, VMSalesOrderSlave vmSalesOrderSlave, int journalType)
        {
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmSalesOrderSlave.OrderNo + " Date: " + vmSalesOrderSlave.OrderDate.ToString("MM/dd/yyyy"),
                Narration = "Invoice Type: Govt. Subsidy.",
                CompanyFK = CompanyFK,
                Date = vmSalesOrderSlave.OrderDate,
                IsSubmit = true
            };

            double unitDiscount = vmSalesOrderSlave.DataListSlave.Sum(x => x.Qty * Convert.ToDouble(x.ProductDiscountUnit));
            double cashDiscount = vmSalesOrderSlave.DataListSlave.Sum(item => item.ProductDiscountUnit > 0 ? (((item.Qty * item.UnitPrice) - (item.Qty * Convert.ToDouble(item.ProductDiscountUnit))) / 100 * Convert.ToDouble(item.CashDiscountPercent)) : ((item.Qty * item.UnitPrice) / 100 * Convert.ToDouble(item.CashDiscountPercent)));
            double spetialDiscount = vmSalesOrderSlave.DataListSlave.Sum(item => Convert.ToDouble(item.SpecialDiscount));

            List<string> strList = new List<string>();
            foreach (var item in vmSalesOrderSlave.DataListSlave)
            {
                string s = "Product: " + item.ProductCategoryName + " " + item.ProductSubCategoryName + " " + item.ProductName + "Delivered Qty: " + item.Qty + " Unit Price: " + item.UnitPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray())) + " Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount;
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmSalesOrderSlave.DataListSlave.Any() ? ((vmSalesOrderSlave.DataListSlave.Sum(x => x.Qty * x.UnitPrice)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmSalesOrderSlave.AccountingHeadId.Value //Customer/ LC
            });

            foreach (var item in vmSalesOrderSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.Qty + " Unit Price: " + item.UnitPrice,
                    Debit = 0,
                    Credit = (item.Qty * item.UnitPrice),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount,
                Debit = unitDiscount + cashDiscount + spetialDiscount,
                Credit = 0,
                Accounting_HeadFK = 50604937 // Sales Discount
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmSalesOrderSlave.CompanyId, vmSalesOrderSlave.OrderMasterId, vmSalesOrderSlave.IntegratedFrom);


            }

            return resultData.VoucherId;
        }


        public async Task<long> AccountingSalesPushISS(VMOrderDeliverDetail vmOrderDeliverDetail)
        {
            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == vmOrderDeliverDetail.CompanyFK && x.Code == "SDV" && x.IsActive == true).FirstOrDefault();

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                Title = vmOrderDeliverDetail.OrderNo + " Date: " + vmOrderDeliverDetail.OrderDate.ToString("MM/dd/yyyy"),
                Narration = vmOrderDeliverDetail.ChallanNo + " Date: " + vmOrderDeliverDetail.DeliveryDate.Value.ToString("MM/dd/yyyy") + " Payment Method: " + ((VendorsPaymentMethodEnum)vmOrderDeliverDetail.PaymentMethod).ToString(),
                CompanyFK = vmOrderDeliverDetail.CompanyFK,
                Date = vmOrderDeliverDetail.DeliveryDate,
                IsSubmit = true,
                Accounting_CostCenterFK = vmOrderDeliverDetail.AcCostCenterId
            };

            double unitDiscount = vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.DiscountUnit));
            double spetialDiscount = vmOrderDeliverDetail.DataListDetail.Sum(item => Convert.ToDouble(item.SpecialDiscount ?? 0));

            List<string> strList = new List<string>();
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                string s = "Product: " + item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray())) + " Unit Discount: " + unitDiscount + " Spetial Discount: " + spetialDiscount;
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? ((vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * x.UnitPrice)) - (unitDiscount + spetialDiscount)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmOrderDeliverDetail.AccountingHeadId.Value //Customer/ LC
            });

            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice,
                    Debit = 0,
                    Credit = (item.DeliveredQty * item.UnitPrice),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }
            var salesCommition = _db.HeadGLs.Where(x => x.CompanyId == vmOrderDeliverDetail.CompanyFK && x.AccCode == "4501001001001" && x.IsActive).FirstOrDefault();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Unit Discount: " + unitDiscount + " Spetial Discount: " + spetialDiscount,
                Debit = unitDiscount + spetialDiscount,
                Credit = 0,
                Accounting_HeadFK = salesCommition.Id //Sales Commissiom

            });
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Costing Price: " + item.COGSPrice,
                    Debit = 0,
                    Credit = item.DeliveredQty * Convert.ToDouble(item.COGSPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true
                });
            }
            var storeStockAccHead = _db.HeadGLs.Where(x => x.CompanyId == vmOrderDeliverDetail.CompanyFK && x.AccCode == "4701001001001" && x.IsActive).FirstOrDefault();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.COGSPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = storeStockAccHead.Id, // Stock Adjust With Erp Cr
                IsVirtual = true
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmOrderDeliverDetail.CompanyFK.Value, vmOrderDeliverDetail.OrderDeliverId, vmOrderDeliverDetail.IntegratedFrom);


            }

            return resultData.VoucherId;
        }

        public async Task<long> AccountingPromotionalPushISS(IssueDetailInfoVM issueDetailInfoVM)
        {
            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == issueDetailInfoVM.CompanyFK && x.Code == "ADJV" && x.IsActive == true).FirstOrDefault();

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                Title = issueDetailInfoVM.IssueNo,
                Narration = " Date: " + issueDetailInfoVM.IssueDate.ToString("MM/dd/yyyy"),
                CompanyFK = issueDetailInfoVM.CompanyFK,
                Date = issueDetailInfoVM.IssueDate,
                IsSubmit = true,
            };

            double TotalPrice = (double)issueDetailInfoVM.DataListSlave.Sum(x => x.RMQ * x.CostingPrice);


            List<string> strList = new List<string>();
            foreach (var item in issueDetailInfoVM.DataListSlave)
            {
                string s = "Product: " + item.ProductName + "Delivered Qty: " + item.RMQ + " Unit Price: " + item.CostingPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray())) + " Total Price: " + TotalPrice;
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = (double)(issueDetailInfoVM.DataListSlave.Any() ? (issueDetailInfoVM.DataListSlave.Sum(x => x.RMQ * x.CostingPrice)) : 0),
                Credit = 0,
                Accounting_HeadFK = 50627595 //Promotional Materials Expenses
            });

            foreach (var item in issueDetailInfoVM.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.RMQ + " Unit Price: " + item.CostingPrice,
                    Debit = 0,
                    Credit = (double)(item.RMQ * item.CostingPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, issueDetailInfoVM.CompanyFK.Value, issueDetailInfoVM.IssueMasterId, "IssueMasterInfo");


            }

            return resultData.VoucherId;
        }

        public async Task<long> SalesTranserVoucherPush(SalesTransferDetailVM salesTransferDetailVM)
        {
            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == salesTransferDetailVM.CompanyFK && x.Code == "SDV" && x.IsActive == true).FirstOrDefault();

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                Title = salesTransferDetailVM.SalesTransferNo + " Date: " + salesTransferDetailVM.SalesTransferDate.ToString("MM/dd/yyyy"),
                Narration = salesTransferDetailVM.CreatedBy + " Date: " + salesTransferDetailVM.CreatedDate.ToString("MM/dd/yyyy"),
                CompanyFK = salesTransferDetailVM.CompanyFK,
                Date = salesTransferDetailVM.SalesTransferDate,
                IsSubmit = true,
            };

            double totalSalesTranserAmount = (double)salesTransferDetailVM.DataListDetail.Sum(x => x.DeliveredQty * x.UnitPrice);


            List<string> strList = new List<string>();
            foreach (var item in salesTransferDetailVM.DataListDetail)
            {
                string s = "Product: " + item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray()));

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = totalSalesTranserAmount,
                Credit = 0,
                Accounting_HeadFK = (int)salesTransferDetailVM.ToVenderHeadGLId
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = totalSalesTranserAmount,
                Accounting_HeadFK = (int)salesTransferDetailVM.FromVenderHeadGLId
            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, salesTransferDetailVM.CompanyFK.Value, salesTransferDetailVM.SalesTransferId, "Salestransfer");
            }

            return resultData.VoucherId;
        }

        public async Task<long> AccountingSalesPushSEED(int CompanyFK, VMOrderDeliverDetail vmOrderDeliverDetail, int journalType)
        {

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmOrderDeliverDetail.OrderNo + " Date: " + vmOrderDeliverDetail.OrderDate.ToString(),
                Narration = vmOrderDeliverDetail.ChallanNo + " Date: " + vmOrderDeliverDetail.DeliveryDate.ToString() +
                "Crops Group: " + vmOrderDeliverDetail.DataListDetail.Select(x => x.ProductCategory).FirstOrDefault()
                ,
                CompanyFK = CompanyFK,
                Date = vmOrderDeliverDetail.DeliveryDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Customer",
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(x => (x.DeliveredQty * x.UnitPrice - Convert.ToDouble(x.Discount ?? 0))) - Convert.ToDouble(vmOrderDeliverDetail.SpecialDiscount ?? 0)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmOrderDeliverDetail.AccountingHeadId.Value //Customer/ LC 
            });
            int count = 1;
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + " Delivered Qty " + item.DeliveredQty + " Unit Price: " + item.UnitPrice + " Total Price" + item.DeliveredQty * item.UnitPrice,
                    Debit = 0,
                    Credit = count == 1 ? (item.DeliveredQty * item.UnitPrice) - Convert.ToDouble(item.Discount) - Convert.ToDouble(vmOrderDeliverDetail.SpecialDiscount ?? 0) : (item.DeliveredQty * item.UnitPrice) - Convert.ToDouble(item.Discount),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
                count++;
            }
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + "Delivered Qty: " + item.DeliveredQty + " COGS Price: " + item.COGSPrice + " Total Costing" + Convert.ToDecimal(item.DeliveredQty) * item.COGSPrice,
                    Debit = 0,
                    Credit = item.DeliveredQty * Convert.ToDouble(item.COGSPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.COGSPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 43576 //SEED Stock Adjust With Erp Dr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmOrderDeliverDetail.CompanyFK.Value, vmOrderDeliverDetail.MaterialReceiveId, vmOrderDeliverDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingSalesPushFeed(int CompanyFK, VMOrderDeliverDetail vmOrderDeliverDetail, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmOrderDeliverDetail.OrderNo + " Date: " + vmOrderDeliverDetail.OrderDate.ToString(),
                Narration = vmOrderDeliverDetail.ChallanNo + " Date: " + vmOrderDeliverDetail.DeliveryDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmOrderDeliverDetail.DeliveryDate,
                IsSubmit = true,
                CreatedBy = vmOrderDeliverDetail.CreatedBy,
                CreatedDate = vmOrderDeliverDetail.CreatedDate
            };
            List<string> strList = new List<string>();
            if (vmOrderDeliverDetail.DataListDetail.Any())
            {
                foreach (var item in vmOrderDeliverDetail.DataListDetail)
                {
                    string s = item.ProductCategory + " " + (item.ProductCategory != item.ProductSubCategory ? item.ProductSubCategory : "") + item.ProductName + " Quantity " + item.DeliveredQty + " Unit Price: " + item.UnitPrice + " Total Price" + item.DeliveredQty * item.UnitPrice;
                    strList.Add(s);
                }
            }

            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => (item.DeliveredQty * (item.UnitPrice + Convert.ToDouble(item.AdditionPrice))) - (item.DeliveredQty * Convert.ToDouble(item.EBaseCommission) + item.DeliveredQty * Convert.ToDouble(item.ECarryingCommission) + item.DeliveredQty * Convert.ToDouble(item.ECashCommission) + item.DeliveredQty * Convert.ToDouble(item.SpecialDiscount) + item.DeliveredQty * Convert.ToDouble(item.MonthlyIncentive) + item.DeliveredQty * Convert.ToDouble(item.YearlyIncentive)))) : 0,
                Credit = 0,
                Accounting_HeadFK = vmOrderDeliverDetail.AccountingHeadId.Value //Customer/ LC
            });

            if (vmOrderDeliverDetail.DataListDetail.Any())
            {
                double Carrying = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.ECarryingCommission)));
                double cash = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.ECashCommission)));
                double Special = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.SpecialDiscount)));
                double monthlyInc = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.MonthlyIncentive)));
                double yearlyInc = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.YearlyIncentive)));
                double baseCommission = Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => item.DeliveredQty * Convert.ToDouble(item.EBaseCommission)));

                if (baseCommission > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Base Commission: " + baseCommission,
                        Debit = baseCommission,
                        Credit = 0,
                        Accounting_HeadFK = 50612377 //Feed Base Commission
                    });
                }


                if (Carrying > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Carrying Commission: " + Carrying,
                        Debit = Carrying,
                        Credit = 0,
                        Accounting_HeadFK = 50610267 //Feed Carrying Commission 
                    });
                }

                if (cash > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = " Cash Commission: " + cash,
                        Debit = cash,
                        Credit = 0,
                        Accounting_HeadFK = 50612378 //Feed Cash Commission
                    });
                }

                if (Special > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Special Discount: " + Special,
                        Debit = Special,
                        Credit = 0,
                        Accounting_HeadFK = 50612379 // Special Discount

                    });
                }

                if (monthlyInc > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Monthly Incentive: " + monthlyInc,
                        Debit = monthlyInc,
                        Credit = 0,
                        Accounting_HeadFK = 34381 // Special Discount

                    });
                }

                if (yearlyInc > 0)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Yearly Incentive: " + yearlyInc,
                        Debit = yearlyInc,
                        Credit = 0,
                        Accounting_HeadFK = 34381 // Special Discount

                    });
                }

            }




            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + " Delivered Qty " + item.DeliveredQty + " Unit Price: " + item.UnitPrice + " Total Price" + item.DeliveredQty * item.UnitPrice,
                    Debit = 0,
                    Credit = (item.DeliveredQty * (item.UnitPrice + Convert.ToDouble(item.AdditionPrice))),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });

            }
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " " + " Delivered Qty " + item.DeliveredQty + " COGS Price: " + item.COGSPrice,
                    Debit = 0,
                    Credit = item.DeliveredQty * Convert.ToDouble(item.COGSPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.COGSPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Dr
                IsVirtual = true
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmOrderDeliverDetail.CompanyFK.Value, vmOrderDeliverDetail.OrderDeliverId, vmOrderDeliverDetail.IntegratedFrom);

            }


            return resultData.VoucherId;
        }

        public async Task<long> AccountingSalesPushPackaging(int CompanyFK, VMOrderDeliverDetail vmOrderDeliverDetail, int journalType)
        {
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmOrderDeliverDetail.OrderNo + " Date: " + vmOrderDeliverDetail.OrderDate.ToString("MM/dd/yyyy"),
                Narration = vmOrderDeliverDetail.ChallanNo + " Date: " + vmOrderDeliverDetail.DeliveryDate.Value.ToString("MM/dd/yyyy") + " Payment Method: " + ((VendorsPaymentMethodEnum)vmOrderDeliverDetail.PaymentMethod).ToString(),
                CompanyFK = CompanyFK,
                Date = vmOrderDeliverDetail.DeliveryDate,
                IsSubmit = true,
            };

            //double unitDiscount = vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.DiscountUnit));
            //double cashDiscount = vmOrderDeliverDetail.DataListDetail.Sum(item => item.DiscountUnit > 0 ? (((item.DeliveredQty * item.UnitPrice) - (item.DeliveredQty * Convert.ToDouble(item.DiscountUnit))) / 100 * Convert.ToDouble(item.DiscountRate ?? 0)) : ((item.DeliveredQty * item.UnitPrice) / 100 * Convert.ToDouble(item.DiscountRate ?? 0)));
            //double spetialDiscount = vmOrderDeliverDetail.DataListDetail.Sum(item => Convert.ToDouble(item.SpecialDiscount ?? 0));
            double Vat = vmOrderDeliverDetail.DataListDetail.Sum(item => item.VATAmount);
            double tds = vmOrderDeliverDetail.DataListDetail.Sum(item => item.TDSAmount);

            List<string> strList = new List<string>();
            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                string s = "Product: " + item.ProductSubCategory + " " + item.ProductName + "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice;
                strList.Add(s);
            }
            string perticular = (String.Join(", ", strList.ToArray())); //+ " Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount;
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular + " VAT: " + Vat,
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? ((vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * x.UnitPrice) + Vat)) : 0,
                Credit = 0,
                Accounting_HeadFK = vmOrderDeliverDetail.AccountingHeadId.Value //Customer/ LC
            });

            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Unit Price: " + item.UnitPrice,
                    Debit = 0,
                    Credit = (item.DeliveredQty * item.UnitPrice),
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }

            //vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            //{
            //    Particular = "Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount,
            //    Debit = unitDiscount + cashDiscount + spetialDiscount,
            //    Credit = 0,
            //    Accounting_HeadFK = 37810 //Packaging new Head id // Sales Discount
            //});

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = Vat,
                Accounting_HeadFK = 40536 //Packaging new Head id // VAT Account
            });



            foreach (var item in vmOrderDeliverDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Delivered Qty: " + item.DeliveredQty + " Costing Price: " + item.COGSPrice,
                    Debit = 0,
                    Credit = item.DeliveredQty * Convert.ToDouble(item.COGSPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmOrderDeliverDetail.DataListDetail.Any() ? vmOrderDeliverDetail.DataListDetail.Sum(x => x.DeliveredQty * Convert.ToDouble(x.COGSPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50605003, //Packaging Stock Adjust With Erp Cr
                IsVirtual = true
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmOrderDeliverDetail.CompanyFK.Value, vmOrderDeliverDetail.OrderDeliverId, vmOrderDeliverDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }




        private bool VoucherMapping(long voucherId, int companyId, long integratedId, string integratedFrom)
        {
            var objectToSave = _db.VoucherMaps
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

                _db.VoucherMaps.Add(voucherMap);

                return _db.SaveChanges() > 0;

            }

        }
        public async Task<long> StockAdjustPushIss(int CompanyFK, VMStockAdjustDetail vmStockAdjust)
        {
            long result = -1;

            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == CompanyFK && x.Code == "ADJV" && x.IsActive == true).FirstOrDefault();
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                Title = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                Narration = vmStockAdjust.Remarks,
                CompanyFK = CompanyFK,
                Date = vmStockAdjust.AdjustDate,
                IsSubmit = true
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            foreach (var item in vmStockAdjust.DataListSlave)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            var IssStockAdjust = _db.HeadGLs.Where(x => x.CompanyId == CompanyFK && x.AccCode == "4701001001001" && x.IsActive).FirstOrDefault();

            foreach (var item in vmStockAdjust.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = IssStockAdjust.Id
                });
            }

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmStockAdjust.CompanyFK.Value, vmStockAdjust.StockAdjustId, vmStockAdjust.IntegratedFrom);

            }


            return resultData.VoucherId;
        }


        public async Task<long> AccountingStockAdjustPushFeed(int CompanyFK, VMStockAdjustDetail vmStockAdjust, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmStockAdjust.InvoiceNo,
                Narration = " Date: " + vmStockAdjust.AdjustDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmStockAdjust.AdjustDate,
                IsSubmit = true
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            foreach (var item in vmStockAdjust.DataListSlave)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            foreach (var item in vmStockAdjust.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = 50609522
                });
            }

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmStockAdjust.CompanyFK.Value, vmStockAdjust.StockAdjustId, vmStockAdjust.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingStockAdjustPushGCCL(int CompanyFK, VMStockAdjustDetail vmStockAdjust, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                Narration = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmStockAdjust.AdjustDate,
                IsSubmit = true
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            foreach (var item in vmStockAdjust.DataListSlave)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            foreach (var item in vmStockAdjust.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = 50606113
                });
            }


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmStockAdjust.CompanyFK.Value, vmStockAdjust.StockAdjustId, vmStockAdjust.IntegratedFrom);


            }


            return resultData.VoucherId;
        }
        public async Task<long> AccountingStockAdjustPushKfmal(int CompanyFK, VMStockAdjustDetail vmStockAdjust, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                Narration = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmStockAdjust.AdjustDate,
                IsSubmit = true
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            foreach (var item in vmStockAdjust.DataListSlave)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            foreach (var item in vmStockAdjust.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.LessQty > 0 ? "Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice : item.ExcessQty > 0 ? "Excess Qty " + item.ExcessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.ExcessQty * item.UnitPrice : "",
                    Debit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Credit = Convert.ToDouble(item.ExcessQty > 0 ? item.ExcessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = 50616077 //KFMAL Stock Adjust
                });
            }


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmStockAdjust.CompanyFK.Value, vmStockAdjust.StockAdjustId, vmStockAdjust.IntegratedFrom);

            }


            return resultData.VoucherId;
        }

        public async Task<long> SampleProductStockAdjustPushGCCL(int CompanyFK, VMStockAdjustDetail vmStockAdjust, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString(),
                Narration = vmStockAdjust.InvoiceNo + " Date: " + vmStockAdjust.AdjustDate.ToString() + " Description: " + vmStockAdjust.Remarks,
                CompanyFK = CompanyFK,
                Date = vmStockAdjust.AdjustDate,
                IsSubmit = true
            };
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            foreach (var item in vmStockAdjust.DataListSlave)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Sample Product Purpose Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            foreach (var item in vmStockAdjust.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Sample Product Purpose Less Qty " + item.LessQty + " Unit Price: " + item.UnitPrice + " Total Price: " + item.LessQty * item.UnitPrice,
                    Debit = Convert.ToDouble(item.LessQty > 0 ? item.LessQty * item.UnitPrice : 0),
                    Credit = 0,
                    Accounting_HeadFK = 50616660 // new Head Id is 50616660  old ld was 39492 //4105001001002 -- Business Promotion  - gccl
                });
            }


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmStockAdjust.CompanyFK.Value, vmStockAdjust.StockAdjustId, vmStockAdjust.IntegratedFrom);

            }


            return resultData.VoucherId;
        }

        public async Task<long> CullectionPushGCCL(int CompanyFK, VMPayment vmPayment, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "Collection No: " + vmPayment.PaymentNo,
                Narration = "Date: " + vmPayment.TransactionDate.ToShortDateString(),
                CompanyFK = CompanyFK,
                Date = vmPayment.TransactionDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Raw Item Cr Integration Dr
            List<string> strList = new List<string>();
            foreach (var item in vmPayment.DataList)
            {
                string s = "Invoice No: " + item.OrderNo + " Date: " + item.OrderDate.ToShortDateString();
                strList.Add(s);
            }
            string perticular = String.Join(", ", strList.ToArray());
            if (vmPayment.DataList.Any())
            {
                foreach (var item in vmPayment.DataList)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = "Invoice No: " + item.OrderNo + " Date: " + item.OrderDate.ToShortDateString() + " Money Receipt No" + item.MoneyReceiptNo + " MR Date: " + item.TransactionDate.ToShortDateString() + " Reference: " + item.ReferenceNo,
                        Debit = 0,
                        Credit = Convert.ToDouble(item.InAmount),
                        Accounting_HeadFK = item.PaymentFromHeadGLId.Value
                    });
                }
            }


            if (vmPayment.PaymentToHeadGLId != null)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular,
                    Debit = (Convert.ToDouble(vmPayment.DataList.Sum(x => x.InAmount)) + (vmPayment.DataListIncome != null ? Convert.ToDouble(vmPayment.DataListIncome.Sum(x => x.OthersIncomeAmount)) : 0)) - (Convert.ToDouble(vmPayment.BankCharge + (vmPayment.DataListExpenses != null ? vmPayment.DataListExpenses.Sum(x => x.ExpensesAmount) : 0))),
                    Credit = 0,
                    Accounting_HeadFK = vmPayment.PaymentToHeadGLId.Value
                });
            }
            if (vmPayment.DataListExpenses.Any())
            {
                foreach (var item in vmPayment.DataListExpenses)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ExpensessReference,
                        Debit = Convert.ToDouble(item.ExpensesAmount),
                        Credit = 0,
                        Accounting_HeadFK = item.ExpensesHeadGLId.Value
                    });


                }
            }

            if (vmPayment.DataListIncome.Any())
            {
                foreach (var item in vmPayment.DataListIncome)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.IncomeReference,
                        Debit = 0,
                        Credit = Convert.ToDouble(item.OthersIncomeAmount),
                        Accounting_HeadFK = item.OthersIncomeHeadGLId.Value
                    });


                }
            }

            if (vmPayment.BankCharge > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "",
                    Debit = Convert.ToDouble(vmPayment.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = vmPayment.BankChargeHeadGLId.Value
                });
            }

            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                //await SMSPush(resultData);
            }
            return resultData.VoucherId;
        }

        public async Task<long> CullectionPushFeed(int CompanyFK, VMFeedPayment vmPayment, int journalType)
        {
            List<string> strList = new List<string>();
            foreach (var item in vmPayment.DataListPayment)
            {
                var bank = _db.HeadGLs.Find(item.Accounting_BankOrCashId.Value);
                VMJournalSlave vMJournalSlave = new VMJournalSlave
                {
                    JournalType = journalType,
                    Title = "Collection No: " + item.MoneyReceiptNo,
                    Narration = " Collection Date: " + item.TransactionDate.ToShortDateString(),
                    CompanyFK = CompanyFK,
                    Date = item.TransactionDate,
                    IsSubmit = true
                };
                vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Customer: " + item.CommonCustomerName + " Customer Code: " + item.CommonCustomerCode + " Money Receipt No" + item.MoneyReceiptNo + " MR Date: " + item.TransactionDate.ToShortDateString() + " Reference: " + item.ReferenceNo + " Bank Name: " + bank.AccName,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.InAmount),
                    Accounting_HeadFK = item.HeadGLId.Value
                });
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = " Customer: " + item.CommonCustomerName + " Customer Code: " + item.CommonCustomerCode + "Money Receipt No" + item.MoneyReceiptNo + " MR Date: " + item.TransactionDate.ToShortDateString() + " Reference: " + item.ReferenceNo,
                    Debit = Convert.ToDouble(item.InAmount),
                    Credit = 0,
                    Accounting_HeadFK = item.Accounting_BankOrCashId.Value
                });
                var resultData = await AccountingJournalMasterPush(vMJournalSlave);
                //await SMSPush(resultData);
            }


            return 1;
        }


        public async Task<long> PaymentPushGCCL(int CompanyFK, VMPayment vmPayment, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "Payment No: " + vmPayment.PaymentNo + " Reference: " + vmPayment.ReferenceNo,
                Narration = "Date: " + vmPayment.TransactionDate.ToShortDateString()
                + " A/C Name: " + vmPayment.ACName + " A/C No: " + vmPayment.ACNo + " Bank Name: " + vmPayment.BankName +
                " Branch Name: " + vmPayment.BranchName
                ,
                ChqDate = vmPayment.MoneyReceiptDate,
                ChqName = vmPayment.MoneyReceiptName,
                ChqNo = vmPayment.MoneyReceiptNo,
                CompanyFK = CompanyFK,
                Date = vmPayment.TransactionDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Raw Item Cr Integration Dr
            List<string> strList = new List<string>();
            foreach (var item in vmPayment.DataList)
            {
                string s = "Purchase Order No: " + item.OrderNo + " Date: " + item.OrderDate.ToShortDateString();
                strList.Add(s);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = Convert.ToDouble(vmPayment.DataList.Sum(x => x.OutAmount)) + Convert.ToDouble(vmPayment.BankCharge),
                Accounting_HeadFK = vmPayment.PaymentFromHeadGLId.Value
            });
            if (vmPayment.BankCharge > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular,
                    Debit = Convert.ToDouble(vmPayment.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = vmPayment.BankChargeHeadGLId.Value
                });
            }

            foreach (var item in vmPayment.DataList)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular,
                    Debit = Convert.ToDouble(item.OutAmount.Value),
                    Credit = 0,
                    Accounting_HeadFK = item.PaymentToHeadGLId.Value
                });
            }

            #endregion




            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }
        public async Task<long> AccountingSalesReturnPushKfmal(int CompanyFK, VMSaleReturnDetail vmSaleReturnDetail, int journalType)
        {

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmSaleReturnDetail.SaleReturnNo + " Date: " + vmSaleReturnDetail.ReturnDate.ToString(),
                Narration = vmSaleReturnDetail.Reason,
                CompanyFK = CompanyFK,
                Date = vmSaleReturnDetail.ReturnDate,
                IsSubmit = true
            };

            decimal unitDiscount = vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty * x.DiscountUnit ?? 0);
            decimal cashDiscount = vmSaleReturnDetail.DataListDetail.Sum(item => item.DiscountUnit > 0 ? (((item.Qty * item.Rate) - (item.Qty * item.DiscountUnit)) / 100 * item.DiscountRate ?? 0) : ((item.Qty * item.Rate) / 100 * item.DiscountRate ?? 0));
            decimal spetialDiscount = vmSaleReturnDetail.DataListDetail.Sum(item => item.SpecialDiscount ?? 0);


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                strList.Add(item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate);
            }
            string perticular = String.Join(", ", strList.ToArray()) + " Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount; ;

            if (!vmSaleReturnDetail.IsUnitAsCost)
            {
                #region Actual Return Inirigration
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " Order No:" + vmSaleReturnDetail.OrderNo,
                    Debit = 0,
                    Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble((vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty.Value * x.Rate.Value)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0,
                    Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
                });


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount,
                    Debit = 0,
                    Credit = Convert.ToDouble(unitDiscount + cashDiscount + spetialDiscount),
                    Accounting_HeadFK = 39513 // Sales Commission & Discount
                });

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " Costing Price: " + item.COGSRate,
                        Debit = Convert.ToDouble(item.Qty.Value * item.COGSRate.Value),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingHeadId.Value
                    });
                }

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate,
                        Debit = Convert.ToDouble(item.Qty.Value * item.Rate.Value),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                    });
                }

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Adjust",
                    Debit = 0,
                    Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * x.COGSRate.Value)))) : 0,
                    Accounting_HeadFK = 50616077
                });

                #endregion
            }
            else
            {
                double crAmount = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble((vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty.Value * x.Rate.Value)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0;
                double drAmount = 0;
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " Order No:" + vmSaleReturnDetail.OrderNo,
                    Debit = 0,
                    Credit = crAmount,
                    Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
                });

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " COGSRate Price: " + item.COGSRate,
                        Debit = Convert.ToDouble((item.Qty.Value * item.COGSRate.Value)),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingHeadId.Value
                    });
                    drAmount += Convert.ToDouble((item.Qty.Value * item.COGSRate.Value));
                }

                if (crAmount > drAmount)
                {
                    VMJournalSlave vmJournalSlave = vMJournalSlave.DataListSlave.LastOrDefault();
                    vmJournalSlave.Debit = vmJournalSlave.Debit + (crAmount - drAmount);

                }
                else if (drAmount > crAmount)
                {
                    VMJournalSlave vmJournalSlave = vMJournalSlave.DataListSlave.LastOrDefault();
                    vmJournalSlave.Debit = vmJournalSlave.Debit - (drAmount - crAmount);
                }



            }
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmSaleReturnDetail.CompanyFK.Value, vmSaleReturnDetail.SaleReturnId, vmSaleReturnDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }




        public async Task<long> AccountingSalesReturnPushGCCL(int CompanyFK, VMSaleReturnDetail vmSaleReturnDetail, int journalType)
        {

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmSaleReturnDetail.SaleReturnNo + " Date: " + vmSaleReturnDetail.ReturnDate.ToString(),
                Narration = vmSaleReturnDetail.Reason,
                CompanyFK = CompanyFK,
                Date = vmSaleReturnDetail.ReturnDate,
                IsSubmit = true
            };

            decimal unitDiscount = vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty * x.DiscountUnit ?? 0);
            decimal cashDiscount = vmSaleReturnDetail.DataListDetail.Sum(item => item.DiscountUnit > 0 ? (((item.Qty * item.Rate) - (item.Qty * item.DiscountUnit)) / 100 * item.DiscountRate ?? 0) : ((item.Qty * item.Rate) / 100 * item.DiscountRate ?? 0));
            decimal spetialDiscount = vmSaleReturnDetail.DataListDetail.Sum(item => item.SpecialDiscount ?? 0);


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                strList.Add(item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate);
            }
            string perticular = String.Join(", ", strList.ToArray()) + " Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount; ;

            if (!vmSaleReturnDetail.IsUnitAsCost)
            {
                #region Actual Return Inirigration
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " Order No:" + vmSaleReturnDetail.OrderNo,
                    Debit = 0,
                    Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble((vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty.Value * x.Rate.Value)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0,
                    Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
                });


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Unit Discount: " + unitDiscount + " Cash Discount: " + cashDiscount + " Spetial Discount: " + spetialDiscount,
                    Debit = 0,
                    Credit = Convert.ToDouble(unitDiscount + cashDiscount + spetialDiscount),
                    Accounting_HeadFK = 39513 // Sales Commission & Discount
                });

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " Costing Price: " + item.COGSRate,
                        Debit = Convert.ToDouble(item.Qty.Value * item.COGSRate.Value),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingHeadId.Value
                    });
                }

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate,
                        Debit = Convert.ToDouble(item.Qty.Value * item.Rate.Value),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                    });
                }

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Adjust",
                    Debit = 0,
                    Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * x.COGSRate.Value)))) : 0,
                    Accounting_HeadFK = 50606113
                });

                #endregion
            }
            else
            {
                double crAmount = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble((vmSaleReturnDetail.DataListDetail.Sum(x => x.Qty.Value * x.Rate.Value)) - (unitDiscount + cashDiscount + spetialDiscount)) : 0;
                double drAmount = 0;
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " Order No:" + vmSaleReturnDetail.OrderNo,
                    Debit = 0,
                    Credit = crAmount,
                    Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
                });

                foreach (var item in vmSaleReturnDetail.DataListDetail)
                {
                    vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                    {
                        Particular = item.ProductName + " Return Qty: " + item.Qty + " COGSRate Price: " + item.COGSRate,
                        Debit = Convert.ToDouble((item.Qty.Value * item.COGSRate.Value)),
                        Credit = 0,
                        Accounting_HeadFK = item.AccountingHeadId.Value
                    });
                    drAmount += Convert.ToDouble((item.Qty.Value * item.COGSRate.Value));
                }

                if (crAmount > drAmount)
                {
                    VMJournalSlave vmJournalSlave = vMJournalSlave.DataListSlave.LastOrDefault();
                    vmJournalSlave.Debit = vmJournalSlave.Debit + (crAmount - drAmount);

                }
                else if (drAmount > crAmount)
                {
                    VMJournalSlave vmJournalSlave = vMJournalSlave.DataListSlave.LastOrDefault();
                    vmJournalSlave.Debit = vmJournalSlave.Debit - (drAmount - crAmount);
                }



            }
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmSaleReturnDetail.CompanyFK.Value, vmSaleReturnDetail.SaleReturnId, vmSaleReturnDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingPurchaseReturnPushFeed(int CompanyFK, PurchaseReturnnewViewModel purchaseModel, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = purchaseModel.ReturnNo + " Date: " + purchaseModel.ReturnDate.ToString() + " Reason: " + purchaseModel.ReturnReason,
                Narration = "Return By: " + purchaseModel.ReturnBy,
                CompanyFK = CompanyFK,
                Date = purchaseModel.ReturnDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in purchaseModel.PurchaseReturnDetailItem)
            {
                strList.Add(item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate);
            }
            string perticular = String.Join(", ", strList.ToArray());

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = purchaseModel.PurchaseReturnDetailItem.Any() ? Convert.ToDouble(purchaseModel.PurchaseReturnDetailItem.Sum(x => (Convert.ToDouble(x.Qty.Value * x.Rate.Value)))) : 0,
                Credit = 0,
                Accounting_HeadFK = purchaseModel.AccoutHeadId.Value //Supplier
            });

            foreach (var item in purchaseModel.PurchaseReturnDetailItem)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Price: " + item.COGS,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.Qty.Value * item.COGS.Value),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            foreach (var item in purchaseModel.PurchaseReturnDetailItem)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Costing Price: " + item.Rate,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.Qty.Value * item.Rate.Value),
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = purchaseModel.PurchaseReturnDetailItem.Any() ? Convert.ToDouble(purchaseModel.PurchaseReturnDetailItem.Sum(x => (Convert.ToDouble(x.Qty.Value * x.COGS.Value)))) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Dr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, purchaseModel.CompanyId, purchaseModel.PurchaseReturnId, purchaseModel.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingSalesReturnPushSeed(int CompanyFK, VMSaleReturnDetail vmSaleReturnDetail, int journalType)
        {
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmSaleReturnDetail.SaleReturnNo + " Date: " + vmSaleReturnDetail.ReturnDate.ToString() + " Reason: " + vmSaleReturnDetail.Reason,
                Narration = vmSaleReturnDetail.Reason,
                CompanyFK = CompanyFK,
                Date = vmSaleReturnDetail.ReturnDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                strList.Add(item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate);
            }
            string perticular = String.Join(", ", strList.ToArray());

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * x.Rate.Value)))) : 0,
                Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
            });

            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Price: " + item.Rate,
                    Debit = Convert.ToDouble(item.Qty.Value * item.Rate.Value),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Costing Price: " + item.COGSRate,
                    Debit = Convert.ToDouble(item.Qty.Value * item.COGSRate.Value),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * x.COGSRate.Value)))) : 0,
                Accounting_HeadFK = 43576 //Seed Stock Adjust With Erp Cr
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmSaleReturnDetail.CompanyFK.Value, vmSaleReturnDetail.SaleReturnId, vmSaleReturnDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }

        public async Task<long> AccountingSalesReturnPushFeed(int CompanyFK, VMSaleReturnDetail vmSaleReturnDetail, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vmSaleReturnDetail.SaleReturnNo + " Date: " + vmSaleReturnDetail.ReturnDate.ToString(),
                Narration = "Reason: " + vmSaleReturnDetail.Reason,
                CompanyFK = CompanyFK,
                Date = vmSaleReturnDetail.ReturnDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                strList.Add(item.ProductName + " Return Qty: " + item.Qty + " Price: " + item.Rate);
            }
            string perticular = String.Join(", ", strList.ToArray());

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * ((x.Rate.Value + x.AdditionPrice) - (x.CashCommission + x.BaseCommission + x.CarryingCommission + x.SpecialDiscount)))))) : 0,
                Accounting_HeadFK = vmSaleReturnDetail.AccountingHeadId.Value //Customer
            });

            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Price: " + item.Rate,
                    Debit = (Convert.ToDouble(item.Qty.Value * ((item.Rate.Value + item.AdditionPrice) - (item.CashCommission + item.BaseCommission + item.CarryingCommission + item.SpecialDiscount)))),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingIncomeHeadId.Value
                });
            }
            foreach (var item in vmSaleReturnDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Return Qty: " + item.Qty + " Costing Price: " + item.COGSRate,
                    Debit = Convert.ToDouble(item.Qty.Value * item.COGSRate.Value),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value,
                    IsVirtual = true
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmSaleReturnDetail.DataListDetail.Any() ? Convert.ToDouble(vmSaleReturnDetail.DataListDetail.Sum(x => (Convert.ToDouble(x.Qty.Value * x.COGSRate.Value)))) : 0,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Cr
                IsVirtual = true
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, CompanyFK, vmSaleReturnDetail.SaleReturnId, vmSaleReturnDetail.IntegratedFrom);

            }

            return resultData.VoucherId;
        }
        public async Task<long> AccountingFeedPurchasePushFeed(int CompanyFK, VMStoreDetail vMStoreDetail, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = vMStoreDetail.ReceivedCode + " Date: " + vMStoreDetail.ReceivedDate.ToString(),
                Narration = "Remarks: " + vMStoreDetail.Remarks,
                CompanyFK = CompanyFK,
                Date = vMStoreDetail.ReceivedDate,
                IsActive = true,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vMStoreDetail.DataListDetail)
            {
                strList.Add(item.ProductName + " Purchase Qty: " + item.Qty + " Unit Price: " + item.UnitPrice);
            }
            string perticular = "";

            if (strList.Count() > 1)
            {
                perticular = String.Join(", ", strList.ToArray());
            }
            perticular = strList.FirstOrDefault();

            var vjs = new VMJournalSlave();
            vjs.Particular = perticular;
            vjs.Debit = 0;
            vjs.Credit = vMStoreDetail.DataListDetail.Any() ? Convert.ToDouble(vMStoreDetail.DataListDetail.Sum(x => (x.Qty.Value * Convert.ToDouble(x.UnitPrice.Value)))) : 0;
            vjs.Accounting_HeadFK = vMStoreDetail.AccountingHeadId ?? 0; // Supplier

            vMJournalSlave.DataListSlave.Add(vjs);

            foreach (var item in vMStoreDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Purchase Qty: " + item.Qty + "Unit Price: " + item.UnitPrice,
                    Debit = item.Qty.Value * Convert.ToDouble(item.UnitPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value, // Stock Addition
                    IsVirtual = true,
                });
            }

            foreach (var item in vMStoreDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Purchase Qty: " + item.Qty + "Unit Price: " + item.UnitPrice,
                    Debit = item.Qty.Value * Convert.ToDouble(item.UnitPrice.Value),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vMStoreDetail.DataListDetail.Any() ? Convert.ToDouble(vMStoreDetail.DataListDetail.Sum(x => (x.Qty.Value * Convert.ToDouble(x.UnitPrice)))) : 0,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Cr
                IsVirtual = true
            });



            #region Bag Diduction

            foreach (var item in vMStoreDetail.DataListDetail)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.BagName + " Qty: " + item.BagQty + "Unit Price: " + item.BagUnitPrice,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.BagQty * item.BagUnitPrice),
                    Accounting_HeadFK = item.BagAccountingHeadId.Value, //Bag Stock Diduction
                    IsVirtual = true,
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Bag Adjust",
                Debit = vMStoreDetail.DataListDetail.Any() ? vMStoreDetail.DataListDetail.Sum(x => Convert.ToDouble(x.BagQty * x.BagUnitPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522, //Feed Stock Adjust With Erp Cr
                IsVirtual = true
            });
            #endregion



            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            return resultData.VoucherId;
        }
        public async Task<long> AccountingProductionPushFeed(int CompanyFK, RequisitionModel requisitionModel, int journalType)
        {

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "<a href='" + _urlInfo + "Report/GetRMDeliverReport?requisitionId=" + requisitionModel.RequisitionId + "'>" + requisitionModel.RequisitionNo + "</a>" + " Date: " + requisitionModel.RequisitionDate.ToString(),
                Narration = requisitionModel.DeliveryNo + " Date: " + requisitionModel.DeliveredDate.ToString(),
                CompanyFK = CompanyFK,
                Date = requisitionModel.RequisitionDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            // Raw Meterials 
            foreach (var item in requisitionModel.RequisitionItemDetailDataList)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.RProductName + " RM Qty " + item.RTotalQty + " Unit Price: " + item.RUnitPrice + " Total Price" + item.RTotalQty * item.RUnitPrice,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.RTotalQty.Value * item.RUnitPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }
            // Bag
            foreach (var item in requisitionModel.BagDataList)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName + " Bag Qty " + item.BagQty + " Unit Price: " + item.BagUnitPrice + " Total Price" + item.BagQty * item.BagUnitPrice,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.BagQty * item.BagUnitPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }

            // Bag
            foreach (var item in requisitionModel.RequisitionItemDataList)
            {

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName + " Production Qty " + item.OutputQty + " COGS: " + item.ProductionRate + " Total Price" + item.OutputQty * item.ProductionRate,
                    Debit = Convert.ToDouble(item.OutputQty * item.ProductionRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });

            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = requisitionModel.RequisitionItemDataList.Any() ? requisitionModel.RequisitionItemDataList.Sum(x => Convert.ToDouble(x.OutputQty * x.ProductionRate)) : 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Dr
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = requisitionModel.RequisitionItemDetailDataList.Any() ? requisitionModel.RequisitionItemDetailDataList.Sum(x => Convert.ToDouble(x.RTotalQty * x.RUnitPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Dr
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = requisitionModel.BagDataList.Any() ? requisitionModel.BagDataList.Sum(x => Convert.ToDouble(x.BagQty * x.BagUnitPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Dr
            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);


            #region Voucher Maps
            if (resultData != null)
            {
                VoucherMap voucherMap = new VoucherMap();
                voucherMap.VoucherId = resultData.VoucherId;
                voucherMap.IntegratedId = requisitionModel.RequisitionId;
                voucherMap.CompanyId = requisitionModel.CompanyId ?? 0;
                voucherMap.IntegratedFrom = requisitionModel.IntegratedFrom;

                _db.VoucherMaps.Add(voucherMap);
                _db.SaveChanges();
            }

            #endregion


            return resultData.VoucherId;
        }


        public async Task<int> OrderDeliverySMSPush(VMOrderDeliverDetail vmOrderDeliverDetail)
        {
            if (vmOrderDeliverDetail.CompanyFK == (int)CompanyName.KrishibidSeedLimited)
            {
                List<string> strList = new List<string>();
                double qty = 0;
                foreach (var item in vmOrderDeliverDetail.DataListDetail)
                {
                    qty += item.DeliveredQty;
                    strList.Add(item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " Delivered Qty " + item.DeliveredQty + " Unit Price: " + item.UnitPrice);
                }
                string items = String.Join(", ", strList.ToArray());
                if (vmOrderDeliverDetail.CustomerPhone != null)
                {
                    ErpSMS erpSMS = new ErpSMS
                    {


                        Message = "Dear Valued Customer - " + vmOrderDeliverDetail.CustomerName + ", your order has been successfully delivered from Krishibid Seed Ltd. " +
                               "\r\n\r\n" +
                              " Challan No: " + vmOrderDeliverDetail.ChallanNo +
                              " Invoice No: " + vmOrderDeliverDetail.OrderNo +
                              " Date : " + vmOrderDeliverDetail.DeliveryDate.Value.ToShortDateString() +
                              " Qty: " + qty + "kg." +
                              " Amount: " + (vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => (item.DeliveredQty * item.UnitPrice))) : 0).ToString() +
                              " Delivered from: " + vmOrderDeliverDetail.Warehouse +
                              "\r\n\r\n" +
                              " For any query - Please contact 01700729665.",
                        CompanyId = vmOrderDeliverDetail.CompanyFK.Value,
                        Date = vmOrderDeliverDetail.DeliveryDate.Value,
                        Status = (int)EnumSmSStatus.Pending,
                        PhoneNo = vmOrderDeliverDetail.CustomerPhone.Replace(" ", "").Replace("-", ""),
                        SmsType = 3,
                        Remarks = vmOrderDeliverDetail.OrderNo,
                        TryCount = 0,
                        RowTime = DateTime.Now,
                        Subject = "Order Delivery Notification"

                    };

                    try
                    {
                        _db.ErpSMS.Add(erpSMS);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var x = ex.Message;
                    }
                }



            }

            if (vmOrderDeliverDetail.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {
                List<string> strList = new List<string>();
                double qty = 0;
                foreach (var item in vmOrderDeliverDetail.DataListDetail)
                {
                    qty += item.DeliveredQty;
                    strList.Add(item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " Delivered Qty " + item.DeliveredQty + " Unit Price: " + item.UnitPrice);
                }
                string items = String.Join(", ", strList.ToArray());
                if (vmOrderDeliverDetail.CustomerPhone != null)
                {
                    ErpSMS erpSMS = new ErpSMS
                    {


                        Message = "Dear Valued Customer - " + vmOrderDeliverDetail.CustomerName + ", your order has been successfully delivered from Krishibid Feed Ltd. " +
                               "\r\n\r\n" +
                              " Challan No: " + vmOrderDeliverDetail.ChallanNo +
                              " Invoice No: " + vmOrderDeliverDetail.OrderNo +
                              " Date : " + vmOrderDeliverDetail.DeliveryDate.Value.ToShortDateString() +
                              " Qty: " + qty + "kg." +
                              " Amount: " + (vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => (item.DeliveredQty * (item.UnitPrice + Convert.ToDouble(item.AdditionPrice))) - (item.DeliveredQty * Convert.ToDouble(item.EBaseCommission) + item.DeliveredQty * Convert.ToDouble(item.ECarryingCommission) + item.DeliveredQty * Convert.ToDouble(item.ECashCommission) + item.DeliveredQty * Convert.ToDouble(item.SpecialDiscount)))) : 0).ToString() +
                              " Delivered from: " + vmOrderDeliverDetail.Warehouse +
                              "\r\n\r\n" +
                              " For any query - Please contact 01700729172.",
                        CompanyId = vmOrderDeliverDetail.CompanyFK.Value,
                        Date = vmOrderDeliverDetail.DeliveryDate.Value,
                        Status = (int)EnumSmSStatus.Pending,
                        PhoneNo = vmOrderDeliverDetail.CustomerPhone.Replace(" ", "").Replace("-", ""),
                        SmsType = 3,
                        Remarks = vmOrderDeliverDetail.OrderNo,
                        TryCount = 0,
                        RowTime = DateTime.Now,
                        Subject = "Order Delivery Notification"

                    };

                    try
                    {
                        _db.ErpSMS.Add(erpSMS);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var x = ex.Message;
                    }
                }



            }
            return 1;
        }
        public async Task<int> GCCLOrderDeliverySMSPush(VMOrderDeliverDetail vmOrderDeliverDetail)
        {
            if (vmOrderDeliverDetail.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {
                List<string> strList = new List<string>();
                double qty = 0;
                foreach (var item in vmOrderDeliverDetail.DataListDetail)
                {
                    qty += item.DeliveredQty;
                    strList.Add(item.ProductCategory + " " + item.ProductSubCategory + " " + item.ProductName + " Delivered Qty " + item.DeliveredQty + " " + item.UnitName
                        + " Unit Price: " + item.UnitPrice);
                }
                string items = String.Join(", ", strList.ToArray());
                if (vmOrderDeliverDetail.CustomerPhone != null)
                {
                    ErpSMS erpSMS = new ErpSMS
                    {


                        Message = "Dear Valued Customer - " + vmOrderDeliverDetail.CustomerName + ", your order has been successfully delivered from Glorious Crop Care Limited. " +
                               "\r\n\r\n" +
                               " Product: " + items +
                              " Challan No: " + vmOrderDeliverDetail.ChallanNo +
                              " Invoice No: " + vmOrderDeliverDetail.OrderNo +
                              " Date : " + vmOrderDeliverDetail.DeliveryDate.Value.ToShortDateString() +
                              " Amount after discount: " + (vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(x => (x.DeliveredQty * x.UnitPrice - Convert.ToDouble(x.Discount ?? 0))) - Convert.ToDouble(vmOrderDeliverDetail.SpecialDiscount ?? 0)) : 0) + // (vmOrderDeliverDetail.DataListDetail.Any() ? Convert.ToDouble(vmOrderDeliverDetail.DataListDetail.Sum(item => (item.DeliveredQty * (item.UnitPrice + Convert.ToDouble(item.AdditionPrice))) - (item.DeliveredQty * Convert.ToDouble(item.EBaseCommission) + item.DeliveredQty * Convert.ToDouble(item.ECarryingCommission) + item.DeliveredQty * Convert.ToDouble(item.ECashCommission) + item.DeliveredQty * Convert.ToDouble(item.SpecialDiscount)))) : 0).ToString() +
                              " Delivered from: " + vmOrderDeliverDetail.Warehouse +
                              "\r\n\r\n" +
                              " For any query - Please contact 01700729903.",
                        CompanyId = vmOrderDeliverDetail.CompanyFK.Value,
                        Date = vmOrderDeliverDetail.DeliveryDate.Value,
                        Status = (int)EnumSmSStatus.Pending,
                        PhoneNo = vmOrderDeliverDetail.CustomerPhone.Replace(" ", "").Replace("-", ""),
                        SmsType = 3,
                        Remarks = vmOrderDeliverDetail.OrderNo,
                        TryCount = 0,
                        RowTime = DateTime.Now,
                        Subject = "Order Delivery Notification"

                    };

                    try
                    {
                        _db.ErpSMS.Add(erpSMS);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var x = ex.Message;
                    }
                }



            }
            return 1;
        }
        public async Task<int> FeedMaterialsRecivedSMSPush(VMWareHousePOReceivingSlave vmPOReceiving)
        {
            if (vmPOReceiving.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {
                List<string> strList = new List<string>();
                foreach (var item in vmPOReceiving.DataListSlave)
                {
                    strList.Add(item.ProductSubCategory + " " + item.ProductName + ". Net Weight  " + item.StockInQty);
                }
                string items = String.Join(", ", strList.ToArray());

                ErpSMS erpSMS = new ErpSMS
                {
                    Message = "Dear Supplier - " + vmPOReceiving.SupplierName + ", we received " + items + " KG, from You. " +
                               " PO No: " + vmPOReceiving.POCID +
                               " Challan: " + vmPOReceiving.Challan +
                               " Received Date: " + vmPOReceiving.ReceivedDate +
                               " Labour Bill: " + vmPOReceiving.LabourBill +
                               " Truck No: " + vmPOReceiving.TruckNo +
                               " Truck Fare : " + vmPOReceiving.TruckFare +
                               " Contact us at 01700729163 if you have any query. Thanks" +
                               " KRISHIBID FEED LTD.",
                    CompanyId = vmPOReceiving.CompanyFK.Value,
                    Date = vmPOReceiving.ReceivedDate,
                    Status = (int)EnumSmSStatus.Pending,
                    PhoneNo = vmPOReceiving.SupplierPhone.Replace(" ", "").Replace("-", ""),
                    SmsType = 2,
                    Remarks = vmPOReceiving.POCID,
                    TryCount = 0,
                    RowTime = DateTime.Now,
                    Subject = "Material Receive Notification"

                };

                try
                {
                    _db.ErpSMS.Add(erpSMS);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }

            }
            return 1;
        }


        public async Task<long> AccountingProductConvertPushFeed(int CompanyFK, ConvertedProductModel convertedProductModel, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = convertedProductModel.InvoiceNo,//"<a href='" + _urlInfo + "Report/GCCLPurchseInvoiceReport?companyId=" + CompanyFK + "&materialReceiveId=" + vmWareHousePOReceivingSlave.MaterialReceiveId + "&reportName=GCCLPurchaseInvoiceReports'>" + vmWareHousePOReceivingSlave.POCID + "</a>" + " Date: " + vmWareHousePOReceivingSlave.PODate.ToString(),
                Narration = " Date: " + convertedProductModel.ConvertedDate.ToString(),
                CompanyFK = CompanyFK,
                Date = convertedProductModel.ConvertedDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            #region Convert From
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Convert From " + convertedProductModel.ProductFromName + " Convert To" + convertedProductModel.ProductToName + "Qty " + convertedProductModel.ConvertedQty,
                Debit = 0,
                Credit = Convert.ToDouble(convertedProductModel.ConvertedQty * convertedProductModel.ConvertFromUnitPrice),
                Accounting_HeadFK = convertedProductModel.ConvertFromAccountHeadId.Value
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = Convert.ToDouble(convertedProductModel.ConvertedQty * convertedProductModel.ConvertFromUnitPrice),
                Credit = 0,
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Cr
            });
            #endregion

            #region Convert To
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Convert From " + convertedProductModel.ProductFromName + " Convert To" + convertedProductModel.ProductToName + "Qty " + convertedProductModel.ConvertedQty,
                Debit = Convert.ToDouble(convertedProductModel.ConvertedQty * convertedProductModel.ConvertedUnitPrice),
                Credit = 0,
                Accounting_HeadFK = convertedProductModel.ConvertToAccountHeadId.Value
            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = Convert.ToDouble(convertedProductModel.ConvertedQty * convertedProductModel.ConvertedUnitPrice),
                Accounting_HeadFK = 50609522 //Feed Stock Adjust With Erp Cr
            });
            #endregion




            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, CompanyFK, convertedProductModel.ConvertedProductId, convertedProductModel.IntegratedFrom);

            }

            return resultData.VoucherId;
        }


        public async Task<long> AccountingSalesPushGLDL(int CompanyFK, GLDLBookingViewModel bookingVM, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "File No: " + bookingVM.FileNo + " " + bookingVM.BookingNo + " Date: " + bookingVM.BookingDate.ToString(),
                Narration = "Project: " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName,
                CompanyFK = CompanyFK,
                Date = bookingVM.BookingDate,
                IsSubmit = true,
                Accounting_CostCenterFK = bookingVM.AcCostCenterId
            };

            string particular = bookingVM.FileNo + " " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName + " " + " Size " + bookingVM.PlotSize + " Unit Price: " + bookingVM.RatePerKatha + " Total Price" + bookingVM.PlotSize * (double)bookingVM.RatePerKatha + " Total Cost: " + bookingVM.TotalCost + " Total Discount: " + bookingVM.TotalDiscount;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = Convert.ToDouble(bookingVM.GrandTotalAmount),
                Credit = 0,
                Accounting_HeadFK = bookingVM.HeadGLId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = 0,
                Credit = Convert.ToDouble(bookingVM.GrandTotalAmount),
                Accounting_HeadFK = bookingVM.AccountingIncomeHeadId.Value
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, bookingVM.CompanyId.Value, bookingVM.BookingId, bookingVM.IntegratedFrom);

            }

            return resultData.VoucherId;
        }

        public async Task<long> AccountingReSalesPush(int CompanyFK, GLDLBookingViewModel bookingVM, int journalType)
        {
            long result = -1;
            var booking = CustomerBookingView(bookingVM.CompanyId.Value, (long)bookingVM.CGId);
            var payable = addpaybalecode(booking);

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "File No: " + bookingVM.FileNo + " " + bookingVM.BookingNo + " Date: " + bookingVM.BookingDate.ToString(),
                Narration = "Project: " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName,
                CompanyFK = CompanyFK,
                Date = bookingVM.BookingDate,
                IsSubmit = true,
                Accounting_CostCenterFK = 20
            };

            string particular = bookingVM.FileNo + " " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName + " " + " Size " + bookingVM.PlotSize + " Unit Price: " + bookingVM.RatePerKatha + " Total Price" + bookingVM.PlotSize * (double)bookingVM.RatePerKatha + " Total Cost: " + bookingVM.TotalCost + " Total Discount: " + bookingVM.TotalDiscount;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();



            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = Convert.ToDouble(bookingVM.GrandTotalAmount),
                Credit = 0,
                Accounting_HeadFK = bookingVM.HeadGLId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = 0,
                Credit = Convert.ToDouble(bookingVM.PurchaseAmount),
                Accounting_HeadFK = payable.Id
            });


            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular + "Purchase Amount: " + bookingVM.PurchaseAmount,
                Debit = Convert.ToDouble(bookingVM.PurchaseAmount),
                Credit = 0,
                Accounting_HeadFK = 50619503 //purches code

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular + "Purchase Amount: " + bookingVM.PurchaseAmount,
                Debit = 0,
                Credit = Convert.ToDouble(bookingVM.GrandTotalAmount),
                Accounting_HeadFK = 50619501  ///"Sales int: code"
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, bookingVM.CompanyId.Value, bookingVM.BookingId, bookingVM.IntegratedFrom);

                var oldfile = _db.ProductBookingInfoes.FirstOrDefault(f => f.CGId == bookingVM.ResaleLogId);
                oldfile.FileStatus = 4;
                _db.Entry(oldfile).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return resultData.VoucherId;
        }

        private HeadGL addpaybalecode(GLDLBookingViewModel model)
        {
            VMHeadIntegration integration = new VMHeadIntegration();
            HeadGL headGlId = new HeadGL();
            if (model.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
            {
                var productCategorie = _db.ProductCategories.FirstOrDefault(x => x.ProductCategoryId == model.ProductCategoryId);
                integration = new VMHeadIntegration
                {
                    AccName = model.ClientName + "(" + model.FileNo + ")",
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = 50619500,

                    CompanyFK = productCategorie.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
            }
            if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                var productSubCategorie = _db.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == model.ProductSubCategoryId);
                integration = new VMHeadIntegration
                {
                    AccName = model.ClientName + "(" + model.FileNo + ")",
                    LayerNo = 6,
                    Remarks = "GL Layer",
                    IsIncomeHead = false,
                    ParentId = 50619500,

                    CompanyFK = productSubCategorie.CompanyId,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                };
            }

            headGlId = CustomerHeadIntegrationAdd(integration);
            return headGlId;
        }
        public HeadGL CustomerHeadIntegrationAdd(VMHeadIntegration vmHeadIntegration)
        {
            long result = -1;

            string newAccountCode = "";
            int orderNo = 0;

            Head5 parentHead = _db.Head5.Where(x => x.Id == vmHeadIntegration.ParentId).FirstOrDefault();

            IQueryable<HeadGL> childHeads = _db.HeadGLs.Where(x => x.ParentId == vmHeadIntegration.ParentId);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }

            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }


            HeadGL headGL = new HeadGL
            {
                Id = _db.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = 50619500,
                OrderNo = orderNo,
                IsActive = true,
                Remarks = vmHeadIntegration.Remarks
            };
            _db.HeadGLs.Add(headGL);
            if (_db.SaveChanges() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }


        private dynamic CustomerBookingView(int value, long cGId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model = (from t1 in _db.CustomerGroupInfoes
                     join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                     join t3 in _db.ProductBookingInfoes on t1.CGId equals t3.CGId
                     join t4 in _db.Products on t3.ProductId equals t4.ProductId into t4_join
                     from t4 in t4_join.DefaultIfEmpty()
                     join t9 in _db.Units on t4.UnitId equals t9.UnitId into t9_join
                     from t9 in t9_join.DefaultIfEmpty()
                     join t5 in _db.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                     from t5 in t5_join.DefaultIfEmpty()
                     join t6 in _db.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                     from t6 in t6_join.DefaultIfEmpty()
                     join t7 in _db.Employees on t3.SoldBy equals t7.Id into t7_join
                     from t7 in t7_join.DefaultIfEmpty()
                     join t8 in _db.Employees on t3.TeamLeadId equals t8.Id into t8_join
                     from t8 in t8_join.DefaultIfEmpty()
                     join t10 in _db.FacingInfoes on t4.FacingId equals t10.FacingId into t10_join
                     from t10 in t10_join.DefaultIfEmpty()
                     where t1.CGId == cGId
                     select new GLDLBookingViewModel
                     {
                         ProductStatus = t4.Status,
                         IntegratedFrom = "ProductBookingInfo",
                         HeadGLId = t1.HeadGLId,
                         AccountingIncomeHeadId = value == (int)CompanyName.KrishibidPropertiesLimited ? t6.AccountingIncomeHeadId : t5.AccountingIncomeHeadId,
                         CGId = t1.CGId,
                         CustomerGroupName = t1.GroupName,
                         KGRECompanyName = t2.Name,
                         PrimaryContactNo = t1.PrimaryContactNo,
                         PrimaryClientId = t1.PrimaryClientId,
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
                         FacingName = t10.Title,
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
                         PurchaseAmount = t3.PurchaseAmount
                     }).FirstOrDefault();
            return model;

        }

        public async Task<long> BookingMoneyCollectionPushGLDL(int CompanyFK, GLDLBookingViewModel bookingVM, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "File No: " + bookingVM.FileNo + " " + bookingVM.BookingNo + " Date: " + bookingVM.BookingDate.ToString(),
                Narration = "Project: " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName,
                CompanyFK = CompanyFK,
                Date = bookingVM.BookingDate,
                IsSubmit = true
            };

            string particular = "File No: " + bookingVM.FileNo + " " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName + " " + " Size " + bookingVM.PlotSize + " Unit Price: " + (double)bookingVM.RatePerKatha + " Total Price" + bookingVM.PlotSize * (double)bookingVM.RatePerKatha + " Total Cost: " + bookingVM.TotalCost + " Total Discount: " + bookingVM.TotalDiscount;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = 0,
                Credit = Convert.ToDouble(bookingVM.BookingMoney),
                Accounting_HeadFK = bookingVM.HeadGLId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = Convert.ToDouble(bookingVM.BookingMoney),
                Credit = 0,
                Accounting_HeadFK = bookingVM.Accounting_BankOrCashId.Value
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, bookingVM.CompanyId.Value, bookingVM.BookingId, bookingVM.IntegratedFrom);

            }

            return resultData.VoucherId;
        }

        public async Task<long> InstallmentCollectionPushGLDL(int CompanyFK, CollactionBillViewModel collactionVM, int journalType)
        {
            long result = -1;
            try
            {

                VMJournalSlave vMJournalSlave = new VMJournalSlave
                {
                    JournalType = journalType,
                    Title = collactionVM.PaymentNo + " Date: " + collactionVM.TransactionDate.ToString() + " " + collactionVM.ChequeNo,
                    Narration = "Project: " + collactionVM.ProductName + " " + collactionVM.BookingNo,
                    CompanyFK = CompanyFK,
                    Date = collactionVM.TransactionDate,
                    IsSubmit = true
                };

                vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
                List<string> strList = new List<string>();
                if (collactionVM.PaymentList.Any())
                {
                    foreach (var item in collactionVM.PaymentList)
                    {
                        strList.Add(item.Title + " Date: " + item.InstallmentDate + " Money Receipt No: " + item.MoneyReceiptNo);

                        vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                        {
                            Particular = item.Title + " Date: " + item.InstallmentDate + " Money Receipt No: " + item.MoneyReceiptNo,
                            Debit = 0,
                            Credit = Convert.ToDouble(item.InAmount),
                            Accounting_HeadFK = item.HeadGLId.Value
                        });
                    }
                }

                string perticular = String.Join(", ", strList.ToArray());

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular,
                    Debit = Convert.ToDouble(collactionVM.TotalInstallment - collactionVM.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = collactionVM.Accounting_BankOrCashId.Value //Bank Or Cash 

                });
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular,
                    Debit = Convert.ToDouble(collactionVM.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = collactionVM.BankChargeHeadGLId.Value //Bank Charge 

                });
                var resultData = await AccountingJournalMasterPush(vMJournalSlave);
                if (resultData.VoucherId > 0)
                {
                    var voucherMap = VoucherMapping(resultData.VoucherId, collactionVM.CompanyId.Value, collactionVM.PaymentMasterId, collactionVM.IntegratedFrom);

                }
                return resultData.VoucherId;
            }
            catch (Exception ex)
            {

                return result;
            }


        }


        public async Task<long> GldlKplCollectionPush(int CompanyId, MoneyReceiptViewModel moneyReceiptViewModel)
        {
            long result = -1;
            try
            {

                VMJournalSlave vMJournalSlave = new VMJournalSlave
                {
                    JournalType = moneyReceiptViewModel.VoucherTypeId,
                    Title = "File No: " + moneyReceiptViewModel.FileNo + " " + moneyReceiptViewModel.MoneyReceiptNo + " Date: " + moneyReceiptViewModel.MoneyReceiptDate.ToString() + " MR No: " + moneyReceiptViewModel.SerialNumber,
                    Narration = "Project: " + moneyReceiptViewModel.ProjectName + " " + moneyReceiptViewModel.BlockName + " " + moneyReceiptViewModel.PlotName,
                    CompanyFK = CompanyId,
                    Date = moneyReceiptViewModel.Submitdate,
                    IsSubmit = true,
                    Accounting_CostCenterFK = moneyReceiptViewModel.CostCenterId
                };

                vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
                List<string> strList = new List<string>();
                foreach (var item in moneyReceiptViewModel.MoneyReceiptList)
                {
                    strList.Add("Collection From" + item.CollectionFrom + " Amount: " + item.PaidAmount);
                }
                string perticular = String.Join(", ", strList.ToArray());

                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " " + moneyReceiptViewModel.PayTypeName,
                    Debit = 0,
                    Credit = Convert.ToDouble(moneyReceiptViewModel.TotalAmount),
                    Accounting_HeadFK = moneyReceiptViewModel.HeadGLId.Value
                });


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " " + moneyReceiptViewModel.PayTypeName,
                    Debit = Convert.ToDouble(moneyReceiptViewModel.TotalAmount - moneyReceiptViewModel.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = moneyReceiptViewModel.Accounting_BankOrCashId.Value //Bank Or Cash 

                });
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = perticular + " " + moneyReceiptViewModel.PayTypeName,
                    Debit = Convert.ToDouble(moneyReceiptViewModel.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = moneyReceiptViewModel.BankChargeAccHeahId //Bank Charge 

                });
                var resultData = await AccountingJournalMasterPush(vMJournalSlave);
                if (resultData.VoucherId > 0)
                {
                    var voucherMap = VoucherMapping(resultData.VoucherId, moneyReceiptViewModel.CompanyId, moneyReceiptViewModel.MoneyReceiptId, moneyReceiptViewModel.IntegratedFrom);

                }


                var res = await MoneyReceiptStatusUpdate(moneyReceiptViewModel);
                return resultData.VoucherId;
            }
            catch (Exception ex)
            {

                return result;
            }


        }
        public async Task<int> MoneyReceiptStatusUpdate(MoneyReceiptViewModel moneyReceiptViewModel)
        {

            var installments = moneyReceiptViewModel.MoneyReceiptList.Where(x => x.Indecator == (int)IndicatorEnum.Installment).ToList();
            if (installments != null)
            {
                foreach (var item in installments)
                {
                    BookingInstallmentSchedule schedule = _db.BookingInstallmentSchedules.FirstOrDefault(h => h.InstallmentId == item.CollectionFromId);
                    if (item.PaidAmount + schedule.PaidAmount == schedule.Amount)
                    {
                        schedule.PaidAmount = schedule.PaidAmount + item.PaidAmount ?? 0;
                        schedule.IsPaid = true;
                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        schedule.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        schedule.PaidAmount = schedule.PaidAmount + item.PaidAmount ?? 0;
                        schedule.IsPaid = false;
                        schedule.IsPartlyPaid = true;
                        schedule.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        schedule.ModifiedDate = DateTime.Now;
                    }
                    _db.Entry(schedule).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            var CostCollection = moneyReceiptViewModel.MoneyReceiptList.Where(x => x.Indecator == (int)IndicatorEnum.CostHead).ToList();
            if (CostCollection != null)
            {
                foreach (var item in CostCollection)
                {
                    BookingCostMapping bookingCostMapping = _db.BookingCostMappings.FirstOrDefault(h => h.CostsMappingId == item.CollectionFromId);
                    if (item.PaidAmount + bookingCostMapping.PaidAmount == bookingCostMapping.Amount)
                    {
                        bookingCostMapping.PaidAmount = bookingCostMapping.PaidAmount + item.PaidAmount ?? 0;
                        bookingCostMapping.IsPaid = true;
                        bookingCostMapping.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        bookingCostMapping.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        bookingCostMapping.PaidAmount = bookingCostMapping.PaidAmount + item.PaidAmount ?? 0;
                        bookingCostMapping.IsPaid = false;
                        bookingCostMapping.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        bookingCostMapping.ModifiedDate = DateTime.Now;
                    }
                    _db.Entry(bookingCostMapping).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            var bookingMoney = moneyReceiptViewModel.MoneyReceiptList.Where(x => x.Indecator == (int)IndicatorEnum.BookingMoney).FirstOrDefault();
            if (bookingMoney != null)
            {
                ProductBookingInfo productBookingInfoes = _db.ProductBookingInfoes.FirstOrDefault(h => h.BookingId == bookingMoney.CollectionFromId);
                productBookingInfoes.PaidBookingAmt = productBookingInfoes.PaidBookingAmt + bookingMoney.PaidAmount ?? 0;
                _db.Entry(productBookingInfoes).State = EntityState.Modified;
                _db.SaveChanges();
            }


            var downPayment = moneyReceiptViewModel.MoneyReceiptList.Where(x => x.Indecator == (int)IndicatorEnum.DownPayment).FirstOrDefault();
            if (downPayment != null)
            {
                ProductBookingInfo productBookingInfoes = _db.ProductBookingInfoes.FirstOrDefault(h => h.BookingId == downPayment.CollectionFromId);
                productBookingInfoes.PaidDownPaymentAmt = productBookingInfoes.PaidDownPaymentAmt + downPayment.PaidAmount ?? 0;
                _db.Entry(productBookingInfoes).State = EntityState.Modified;
                _db.SaveChanges();
            }

            MoneyReceipt moneyReceipt = _db.MoneyReceipts.Find(moneyReceiptViewModel.MoneyReceiptId);
            moneyReceipt.IsSubmitted = true;
            moneyReceipt.SubmittedDate = moneyReceiptViewModel.Submitdate;
            moneyReceipt.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            moneyReceipt.ModifiedDate = DateTime.Now;
            _db.Entry(moneyReceipt).State = EntityState.Modified;
            _db.SaveChanges();


            return 0;
        }
        private object VoucherMapping(long voucherId, int companyId, long moneyReceiptId, object integratedFrom)
        {
            throw new NotImplementedException();
        }
        public async Task<long> AccountingCostHeadUpdateGLDL(int CompanyFK, GLDLBookingViewModel bookingVM, int journalType)
        {
            long result = -1;

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = bookingVM.CostHeadName + "File No: " + bookingVM.FileNo,
                Narration = "Project: " + bookingVM.ProjectName + " " + bookingVM.BlockName + " " + bookingVM.PlotName,
                CompanyFK = CompanyFK,
                Date = bookingVM.AmandmentDate,
                IsSubmit = true,
                Accounting_CostCenterFK = bookingVM.AcCostCenterId
            };

            string particular = bookingVM.FileNo + " " + bookingVM.CostHeadName + " Amount: " + bookingVM.CostAmount;

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = Convert.ToDouble(bookingVM.CostAmount),
                Credit = 0,
                Accounting_HeadFK = bookingVM.HeadGLId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = particular,
                Debit = 0,
                Credit = Convert.ToDouble(bookingVM.CostAmount),
                Accounting_HeadFK = bookingVM.AccountingIncomeHeadId.Value
            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, bookingVM.CompanyId.Value, bookingVM.BookingId, bookingVM.IntegratedFrom);
            }
            return resultData.VoucherId;
        }


        public async Task<long> AccountingLCAdvancePushKFMAL(int Companyid, LcInfoViewModel lcvm, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "L/C No: " + lcvm.LCNo + " Date: " + lcvm.LCDate.ToString() + " PI No: " + lcvm.PINo + " PI Date: " + lcvm.PIDate,
                Narration = "L/C Value: " + lcvm.LCValue + " Currencey: " + lcvm.CurrenceyName + " Currencey Rate: " + lcvm.CurrenceyRate,
                CompanyFK = Companyid,
                Date = lcvm.LCDate,
                IsSubmit = true,
                Accounting_CostCenterFK = 23
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + lcvm.CurrenceyName + " Cash Margin Percent: " + lcvm.CashMarginPercent + " CashMargin: " + lcvm.CashMarginAmount,
                Debit = Convert.ToDouble(lcvm.CashMarginAmount),
                Credit = 0,
                Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

            });

            if (lcvm.FreightCharges > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Freight Charges: " + lcvm.FreightCharges,
                    Debit = Convert.ToDouble(lcvm.FreightCharges),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }
            if (lcvm.BankCharge > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Bank Charge: " + lcvm.BankCharge,
                    Debit = Convert.ToDouble(lcvm.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }

            if (lcvm.OtherCharge != 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Other Charge: " + lcvm.OtherCharge,
                    Debit = Convert.ToDouble(lcvm.OtherCharge),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }
            if (lcvm.CommissionValue > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Commission: " + lcvm.CommissionValue,
                    Debit = Convert.ToDouble(lcvm.CommissionValue),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }

            if (lcvm.InsuranceValue > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Insurance: " + lcvm.InsuranceValue,
                    Debit = Convert.ToDouble(lcvm.InsuranceValue),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }


            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + lcvm.CurrenceyName + " Cash Margin Percent: " + lcvm.CashMarginPercent + " Cash Margin: " + lcvm.CashMarginAmount +
                " Bank Charge: " + lcvm.BankCharge +
                " Other Charge: " + lcvm.OtherCharge +
                " Insurance Value: " + lcvm.InsuranceValue +
                " Freight Charges: " + lcvm.FreightCharges +
                " Commission: " + lcvm.CommissionValue,
                Debit = 0,
                Credit = Convert.ToDouble((lcvm.CashMarginAmount) +
                                            lcvm.FreightCharges +
                                            lcvm.BankCharge +
                                            lcvm.OtherCharge +
                                            lcvm.InsuranceValue +
                                            lcvm.CommissionValue),
                Accounting_HeadFK = lcvm.AccountingBankHeadId.Value //Customer/ LC 

            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, lcvm.CompanyId, lcvm.LCId, "LCInfo");
            }
            return resultData.VoucherId;
        }


        public async Task<long> AccLCValueAmendmentAdvancePushKFMAL(int Companyid, LcValueAmmendmentvm lcvm, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "L/C No: " + lcvm.LCNo + " Date: " + lcvm.AmendmentDate.ToString(),
                Narration = "L/C Value: " + lcvm.LCValue,
                CompanyFK = Companyid,
                Date = lcvm.AmendmentDate,
                IsSubmit = true,
                Accounting_CostCenterFK = 23
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + "Cash Margin Percent: " + lcvm.CashMarginPercent + " Cash Margin: " + lcvm.CashMarginAmount,
                Debit = Convert.ToDouble(lcvm.CashMarginAmount),
                Credit = 0,
                Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

            });

            if (lcvm.FreightCharges > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Freight Charges: " + lcvm.FreightCharges,
                    Debit = Convert.ToDouble(lcvm.FreightCharges),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }
            if (lcvm.BankCharge > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Bank Charge: " + lcvm.BankCharge,
                    Debit = Convert.ToDouble(lcvm.BankCharge),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }

            if (lcvm.OtherCharge != 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Other Charge: " + lcvm.OtherCharge,
                    Debit = Convert.ToDouble(lcvm.OtherCharge),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }
            if (lcvm.CommissionValue > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Commission: " + lcvm.CommissionValue,
                    Debit = Convert.ToDouble(lcvm.CommissionValue),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }

            if (lcvm.InsuranceValue > 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Insurance: " + lcvm.InsuranceValue,
                    Debit = Convert.ToDouble(lcvm.InsuranceValue),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }


            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + " Cash Margin Percent: " + lcvm.CashMarginPercent + " Cash Margin: " + lcvm.CashMarginAmount +
                " Bank Charge: " + lcvm.BankCharge +
                " Other Charge: " + lcvm.OtherCharge +
                " Insurance Value: " + lcvm.InsuranceValue +
                " Freight Charges: " + lcvm.FreightCharges +
                " Commission: " + lcvm.CommissionValue,
                Debit = 0,
                Credit = Convert.ToDouble((lcvm.CashMarginAmount) +
                                            lcvm.FreightCharges +
                                            lcvm.BankCharge +
                                            lcvm.OtherCharge +
                                            lcvm.InsuranceValue +
                                            lcvm.CommissionValue),
                Accounting_HeadFK = lcvm.AccountingBankHeadId.Value //Customer/ LC 

            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, lcvm.CompanyFK.Value, lcvm.LCId, "LCValueAmendment");
            }
            return resultData.VoucherId;
        }




        public async Task<long> AccountingLCAmendmentPushKFMAL(int Companyid, VMLCAmendment model, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "L/C No: " + model.LCNO + " Amendment Date: " + model.AmendmentDate.ToString(),
                Narration = "Status : " + (model.IsIncrase == true ? "Increase" : "Decrease"),
                CompanyFK = Companyid,
                Date = model.AmendmentDate,
                IsSubmit = true,
                Accounting_CostCenterFK = 23
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Amendment Value: " + model.AmendmentValue,
                Debit = Convert.ToDouble(model.AmendmentValue),
                Credit = 0,
                Accounting_HeadFK = model.AccountingHeadId.Value //Customer/ LC 

            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Amendment Value: " + model.AmendmentValue,
                Debit = 0,
                Credit = Convert.ToDouble(model.AmendmentValue),
                Accounting_HeadFK = model.AccountingBankHeadId.Value //Customer/ LC 

            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, model.CompanyId, model.LCAmendmentId, "LCAmendment");
            }
            return resultData.VoucherId;
        }



        public async Task<long> AccountingLCAdvancePushPackaging(int Companyid, LcInfoViewModel lcvm, int journalType)
        {


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,

                Title = "L/C No: " + lcvm.LCNo + " Date: " + lcvm.LCDate.ToString() + " PI No: " + lcvm.PINo + " PI Date: " + lcvm.PIDate,
                Narration = "L/C Value: " + lcvm.LCValue + " Currencey: " + lcvm.CurrenceyName + " Currencey Rate: " + lcvm.CurrenceyRate,
                CompanyFK = Companyid,
                Date = lcvm.LCDate,
                IsSubmit = true,
                Accounting_CostCenterFK = 23
            };


            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + lcvm.CurrenceyName + " Cash Margin Percent: " + lcvm.CashMarginPercent + " CashMargin: " + lcvm.CashMarginAmount + " " + lcvm.CurrenceyName,
                Debit = Convert.ToDouble(lcvm.CashMarginAmount * lcvm.CurrenceyRate),
                Credit = 0,
                Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Bank Charge: " + lcvm.BankCharge,
                Debit = Convert.ToDouble(lcvm.BankCharge),
                Credit = 0,
                Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

            });


            if (lcvm.OtherCharge != 0)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = "Other Charge: " + lcvm.OtherCharge,
                    Debit = Convert.ToDouble(lcvm.OtherCharge),
                    Credit = 0,
                    Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

                });
            }



            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Commission: " + lcvm.CommissionValue,
                Debit = Convert.ToDouble(lcvm.CommissionValue),
                Credit = 0,
                Accounting_HeadFK = lcvm.AccountingHeadId.Value //Customer/ LC 

            });

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "L/C Value: " + lcvm.LCValue + " " + lcvm.CurrenceyName + " Cash Margin Percent: " + lcvm.CashMarginPercent + " CashMargin: " + lcvm.CashMarginAmount + " " + lcvm.CurrenceyName +
                " Bank Charge: " + lcvm.BankCharge +
                " Other Charge: " + lcvm.OtherCharge +
                " Commission: " + lcvm.CommissionValue + " Currencey Rate: " + lcvm.CurrenceyRate,
                Debit = 0,
                Credit = Convert.ToDouble((lcvm.CashMarginAmount * lcvm.CurrenceyRate) +
                                            lcvm.BankCharge +
                                            lcvm.OtherCharge +
                                            lcvm.CommissionValue),
                Accounting_HeadFK = lcvm.AccountingBankHeadId.Value //Customer/ LC 

            });


            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, lcvm.CompanyId, lcvm.LCId, "LCInfo");
            }
            return resultData.VoucherId;
        }

        public async Task<long> ISSMRPush(int CompanyFK, VMWareHousePOReceivingSlave vmPOReceiving)
        {
            long result = -1;
            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == CompanyFK && x.Code == "MRV" && x.IsActive == true).FirstOrDefault();

            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                //Title = "<a href='" + _urlInfo + "Report/ISSPurchseInvoiceReport?companyId=" + CompanyFK + "&materialReceiveId=" + vmPOReceiving.MaterialReceiveId + "&reportName=GCCLPurchaseInvoiceReports'>" + vmPOReceiving.POCID + "</a>" + " Date: " + vmPOReceiving.PODate.ToString(),
                Title = "<a target='_blank' href='" + _urlInfo + "Report/PackagingPurchaseOrderReports?purchaseOrderId=" + vmPOReceiving.Procurement_PurchaseOrderFk + "&companyId=" + CompanyFK + "&reportName=ISSPurchaseOrderReports'>" + vmPOReceiving.POCID + "</a>" + " Date: " + vmPOReceiving.PODate.ToString(),

                Narration = vmPOReceiving.ChallanCID + " " + vmPOReceiving.Challan + " Date: " + vmPOReceiving.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmPOReceiving.ChallanDate,
                IsSubmit = true
            };

            List<string> strList = new List<string>();
            decimal totalStockInQty = 0;
            decimal totalStockInAmount = 0;

            foreach (var item in vmPOReceiving.DataListSlave)
            {
                strList.Add(item.ProductDescription);
                totalStockInQty += item.StockInQty;
                totalStockInAmount += item.StockInQty * item.StockInRate;
            }

            string perticular = "PO:" + vmPOReceiving.POCID + " MR: " + vmPOReceiving.Challan + " Date: " + vmPOReceiving.ChallanDate.ToString() + " Products: " + String.Join(", ", strList.ToArray()) + " Qty: " + Math.Round(totalStockInQty, 2) + " Total: " + Math.Round(totalStockInAmount, 2);
            decimal discount = vmPOReceiving.DataListSlave.Sum(x => x.ProductDiscount);
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmPOReceiving.DataListSlave.Any() ? Math.Ceiling((Convert.ToDouble((vmPOReceiving.DataListSlave.Sum(x => x.StockInQty * x.PurchasingPrice)) + vmPOReceiving.LabourBill + vmPOReceiving.TruckFare - Math.Round(discount, 2)))) : 0,
                Accounting_HeadFK = vmPOReceiving.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmPOReceiving.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription + " Qty: " + Math.Round(item.StockInQty, 2) + " Total: " + Math.Round(item.StockInQty * item.PurchasingPrice, 2) + vmPOReceiving.SupplierName,
                    Debit = Convert.ToDouble((Math.Round((item.StockInQty * item.PurchasingPrice)) - (item.IsReturn == true ? Math.Round(item.VATAddition, 2) : 0) - Math.Round(item.ProductDiscount, 2))),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }

            var vatAccount = _db.HeadGLs.Where(x => x.CompanyId == vmPOReceiving.CompanyFK && x.AccCode == "1306001001001" && x.IsActive).FirstOrDefault();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "VAT of " + vmPOReceiving.POCID,
                Debit = Math.Round(Convert.ToDouble(vmPOReceiving.DataListSlave.Sum(x => x.VATAddition)), 2),
                Credit = 0,
                Accounting_HeadFK = vatAccount.Id  //  1306001001001-Value Added Tax (VAT)
            });

            var purchaseOverHeadAccount = _db.HeadGLs.Where(x => x.CompanyId == vmPOReceiving.CompanyFK && x.AccCode == "4101001003001" && x.IsActive).FirstOrDefault();

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Truck Fare: " + vmPOReceiving.TruckFare + " Truck Fare: " + vmPOReceiving.LabourBill,
                Debit = Convert.ToDouble(vmPOReceiving.TruckFare + vmPOReceiving.LabourBill),
                Credit = 0,
                Accounting_HeadFK = purchaseOverHeadAccount.Id  // 4101001003001-Purchase Overhead Expenses
            });



            foreach (var item in vmPOReceiving.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription + " Qty: " + Math.Round(item.StockInQty, 2) + " Total: " + Math.Round(item.StockInQty * item.StockInRate, 2),
                    Debit = Convert.ToDouble(item.StockInQty * item.StockInRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            var storeStockAccHead = _db.HeadGLs.Where(x => x.CompanyId == vmPOReceiving.CompanyFK && x.AccCode == "4701001001001" && x.IsActive).FirstOrDefault();


            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmPOReceiving.DataListSlave.Any() ? Convert.ToDouble(vmPOReceiving.DataListSlave.Sum(x => x.StockInQty * x.StockInRate)) : 0,
                Accounting_HeadFK = storeStockAccHead.Id //4701001001001 - Store & Stock Adjustment
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmPOReceiving.CompanyFK.Value, vmPOReceiving.MaterialReceiveId, vmPOReceiving.IntegratedFrom);
            }
            return resultData.VoucherId;
        }

        public async Task<long> MaterialReceivedPushKFMAL(int CompanyFK, VMWareHousePOReceivingSlave vmWareHousePOReceivingSlave, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "<a href='" + _urlInfo + "Report/GCCLPurchseInvoiceReport?companyId=" + CompanyFK + "&materialReceiveId=" + vmWareHousePOReceivingSlave.MaterialReceiveId + "&reportName=GCCLPurchaseInvoiceReports'>" + vmWareHousePOReceivingSlave.POCID + "</a>" + " Date: " + vmWareHousePOReceivingSlave.PODate.ToString(),
                Narration = vmWareHousePOReceivingSlave.Challan + " Date: " + vmWareHousePOReceivingSlave.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = vmWareHousePOReceivingSlave.ChallanDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.StockInQty * x.StockInRate)) : 0,
                Accounting_HeadFK = vmWareHousePOReceivingSlave.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.StockInQty * item.StockInRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in vmWareHousePOReceivingSlave.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = Convert.ToDouble(item.StockInQty * item.StockInRate),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = 0,
                Credit = vmWareHousePOReceivingSlave.DataListSlave.Any() ? Convert.ToDouble(vmWareHousePOReceivingSlave.DataListSlave.Sum(x => x.StockInQty * x.StockInRate)) : 0,
                Accounting_HeadFK = 50616077 //KFMAL Stock Adjust
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmWareHousePOReceivingSlave.CompanyFK.Value, vmWareHousePOReceivingSlave.MaterialReceiveId, vmWareHousePOReceivingSlave.IntegratedFrom);
            }
            return resultData.VoucherId;
        }


        public List<object> FEEDCashAndBankDropDownList(int companyId)
        {

            //gf
            var List = new List<object>();
            var v = (from t1 in _db.Head5

                     where t1.ParentId == 3290 && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t1.AccCode + " -" + t1.AccName
                     }).ToList();

            foreach (var item in v)
            {
                List.Add(new { Text = item.Text, Value = item.Value });
            }

            return List;

        }
        public async Task<List<HeadGLModel>> FeedHeadGLByHeadGLParentIdGet(int companyId, int parentId)
        {

            //87
            List<HeadGLModel> HeadGLModelList =
               await Task.Run(() => (from t1 in _db.HeadGLs
                                     where t1.ParentId == parentId && t1.CompanyId == companyId
                                     select new HeadGLModel
                                     {
                                         Id = t1.Id,
                                         AccName = t1.AccCode + " -" + t1.AccName
                                     }).ToList());
            return HeadGLModelList;
        }
        public async Task<Head5> FeedHeadGLByHead5ParentIdGetForEdit(int companyId, int? BankId)

        {


            var obj = await _db.HeadGLs.Where(x => x.Id == BankId).FirstOrDefaultAsync();




            var anonymousResult = await (from t1 in _db.Head5
                                         where t1.CompanyId == companyId && t1.Id == obj.ParentId
                                         select new
                                         {
                                             Id = t1.Id,
                                             AccCode = t1.AccCode,
                                             AccName = t1.AccName
                                         }).FirstOrDefaultAsync();

            if (anonymousResult == null)
            {
                return null;
            }


            Head5 head5 = new Head5
            {
                Id = anonymousResult.Id,
                AccName = $"{anonymousResult.AccCode ?? string.Empty} - {anonymousResult.AccName ?? string.Empty}"
            };

            return head5;
        }

        public async Task<HeadGL> FeedHeadGLByHeadGLParentIdGetForEdit(int? BankId)

        {

            var anonymousResult = await (from t1 in _db.HeadGLs
                                         where t1.Id == BankId
                                         select new
                                         {
                                             Id = t1.Id,
                                             AccCode = t1.AccCode,
                                             AccName = t1.AccName
                                         }).FirstOrDefaultAsync();

            if (anonymousResult == null)
            {
                return null;
            }


            HeadGL headGl = new HeadGL
            {
                Id = anonymousResult.Id,
                AccName = $"{anonymousResult.AccCode ?? string.Empty} - {anonymousResult.AccName ?? string.Empty}"
            };

            return headGl;
        }


        public async Task<long> CustomerAdjustmentPushGccl(int CompanyFK, VendorAdjustmentVM model, int journalType)
        {
            long result = -1;


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {

                JournalType = journalType,
                Title = model.OrderNo + " Date: " + model.CreateDate.Value.ToShortDateString(),
                Narration = model.Narration,
                CompanyFK = CompanyFK,
                //Date = model.AdjustmentDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = model.Debit > 0 ? model.Debit : 0,
                Credit = model.Credit > 0 ? model.Credit : 0,
                Accounting_HeadFK = model.HeadGLId.Value

            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = model.Credit > 0 ? model.Credit : 0,
                Credit = model.Debit > 0 ? model.Debit : 0,
                Accounting_HeadFK = model.Accounting_HeadFK

            });

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, model.UserCompnayId, model.AdjustmentId, model.IntegratedFrom);
            }
            return resultData.VoucherId;
        }

        public async Task<long> CreditCullectionPushFeed(int CompanyId, ShortCreditViewModel vmModel, int journalType)
        {
            List<string> strList = new List<string>();
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = "Short Credit Collection",
                Narration = "Date: " + vmModel.CollectionDate.ToShortDateString(),
                CompanyFK = vmModel.CompanyId,
                Date = vmModel.CollectionDate,
                IsSubmit = true
            };
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Short Credit Collection to " + vmModel.BankName,
                Debit = 0,
                Credit = Convert.ToDouble(vmModel.CollectedAmount),
                Accounting_HeadFK = vmModel.HeadGLId.Value
            });
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Short Credit Collection from " + vmModel.VendorName,
                Debit = Convert.ToDouble(vmModel.CollectedAmount),
                Credit = 0,
                Accounting_HeadFK = vmModel.BankHeadId
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);


            return 1;
        }
        public async Task<long> AccountingProcessingPushISS(SeedProcessingDetailsVM vmReferenceSlave)
        {
            var journalType = _db.VoucherTypes.Where(x => x.CompanyId == vmReferenceSlave.CompanyFK && x.Code == "ADJV" && x.IsActive == true).FirstOrDefault();
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType.VoucherTypeId,
                Title = vmReferenceSlave.SeedProcessNo + " Date: " + vmReferenceSlave.SeedProcessDate,
                Narration = vmReferenceSlave.CreatedBy + " " + vmReferenceSlave.CreatedDate,
                CompanyFK = vmReferenceSlave.CompanyFK,
                Date = vmReferenceSlave.SeedProcessDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            List<string> strList = new List<string>();
            foreach (var item in vmReferenceSlave.DataList)
            {
                strList.Add(item.ProductName + " Processing Cost: " + item.Amount);
            }
            string perticular = String.Join(", ", strList.ToArray());
            #region Integration Cr Finish Item Dr


            foreach (var item in vmReferenceSlave.DataList)
            {


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName + " Processing Cost: " + item.Amount,
                    Debit = Convert.ToDouble(item.Amount),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }


            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = 0,
                Credit = Convert.ToDouble(vmReferenceSlave.DataList.Select(x => x.Amount).DefaultIfEmpty(0).Sum()),
                Accounting_HeadFK = vmReferenceSlave.BankOrCasgAccHeahId.Value
            });

            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmReferenceSlave.CompanyFK, vmReferenceSlave.SeedProcessingId, "SeedProcessing");
            }
            return resultData.VoucherId;
        }


        public async Task<long> AccountingPackagingPushISS(VMProdReferenceSlave vmReferenceSlave)
        {

            var voucherType = _db.VoucherTypes.Where(x => x.CompanyId == vmReferenceSlave.CompanyFK && x.Code == "PACV" && x.IsActive == true).FirstOrDefault();


            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = voucherType.VoucherTypeId,
                Title = vmReferenceSlave.ReferenceNo + " Date: " + vmReferenceSlave.ReferenceDate,
                Narration = vmReferenceSlave.CreatedBy + " " + vmReferenceSlave.CreatedDate,
                CompanyFK = vmReferenceSlave.CompanyFK,
                Date = vmReferenceSlave.ReferenceDate,
                IsSubmit = true
            };

            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Integration Cr Finish Item Dr


            foreach (var item in vmReferenceSlave.DataListSlave)
            {


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName + " Quantity: " + item.Quantity + " Costing Price: " + item.CostingPrice + " Total: " + item.Quantity * item.CostingPrice,
                    Debit = Convert.ToDouble(item.Quantity * item.CostingPrice),
                    Credit = 0,
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            foreach (var item in vmReferenceSlave.RowProductConsumeList)
            {


                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.RProductName + " Quantity: " + item.TotalConsumeQuantity + " Costing Price: " + item.COGS + " Total: " + item.TotalConsumeQuantity * item.COGS,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.TotalConsumeQuantity * item.COGS),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }

            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, vmReferenceSlave.CompanyFK.Value, vmReferenceSlave.ProdReferenceId, "Prod_Reference");
            }
            return resultData.VoucherId;
        }

        public async Task<long> AccountingRMIssuedPushPackaging(DateTime journalDate, int CompanyFK, VMPackagingPurchaseRequisition vmModel, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = vmModel.IssueNo + " Date: " + vmModel.IssueDate + "Issued by: " + vmModel.IssueBy,
                Narration = "Achknologe By: " + vmModel.AchknologeByName + " " + vmModel.AcknologeDate,
                CompanyFK = CompanyFK,
                Date = journalDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in vmModel.DataList)
            {
                strList.Add(item.ProductName + " Quantity: " + item.IssueQty + " Costing Price: " + item.CostingPrice + " Total: " + item.IssueQty * item.CostingPrice);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();

            #region Integration Cr Finish Item Dr
            foreach (var item in vmModel.DataList)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductName + " Quantity: " + item.IssueQty + " Costing Price: " + item.CostingPrice + " Total: " + item.IssueQty * item.CostingPrice,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.IssueQty * item.CostingPrice),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = vmModel.DataList.Any() ? Convert.ToDouble(vmModel.DataList.Sum(x => x.IssueQty * x.CostingPrice)) : 0,
                Credit = 0,
                Accounting_HeadFK = 50605003 // Packaging ERP Integration
            });
            #endregion

            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, CompanyFK, vmModel.IssueMasterId, "IssueMasterInfo");
            }
            return resultData.VoucherId;
        }



        public async Task<long> AccountingPurchaseReturnPushISS(int CompanyFK, VMWareHousePOReturnSlave model, int journalType)
        {
            long result = -1;
            VMJournalSlave vMJournalSlave = new VMJournalSlave
            {
                JournalType = journalType,
                Title = model.ReturnNo + " Date: " + model.ReturnDate.ToString(),
                Narration = model.Challan + " Date: " + model.ChallanDate.ToString(),
                CompanyFK = CompanyFK,
                Date = model.ReturnDate,
                IsSubmit = true
            };
            List<string> strList = new List<string>();
            foreach (var item in model.DataListSlave)
            {
                strList.Add(item.ProductDescription);
            }
            string perticular = String.Join(", ", strList.ToArray());
            vMJournalSlave.DataListSlave = new List<VMJournalSlave>();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = perticular,
                Debit = model.DataListSlave.Any() ? Convert.ToDouble(model.DataListSlave.Sum(x => x.ReturnQuantity * x.Rate)) : 0,
                Credit = 0,
                Accounting_HeadFK = model.AccountingHeadId.Value //Supplier/ LC
            });
            foreach (var item in model.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.Rate),
                    Accounting_HeadFK = item.AccountingExpenseHeadId.Value
                });
            }
            foreach (var item in model.DataListSlave)
            {
                vMJournalSlave.DataListSlave.Add(new VMJournalSlave
                {
                    Particular = item.ProductDescription,
                    Debit = 0,
                    Credit = Convert.ToDouble(item.ReturnQuantity * item.COGS),
                    Accounting_HeadFK = item.AccountingHeadId.Value
                });
            }
            var storeStockAccHead = _db.HeadGLs.Where(x => x.CompanyId == CompanyFK && x.AccCode == "4701001001001" && x.IsActive).FirstOrDefault();
            vMJournalSlave.DataListSlave.Add(new VMJournalSlave
            {
                Particular = "Adjust",
                Debit = model.DataListSlave.Any() ? Convert.ToDouble(model.DataListSlave.Sum(x => x.ReturnQuantity * x.COGS)) : 0,
                Credit = 0,
                /*Accounting_HeadFK = 50605003*/ //Packaging Stock Adjust With Erp Cr
                Accounting_HeadFK = storeStockAccHead.Id
            });
            var resultData = await AccountingJournalMasterPush(vMJournalSlave);
            if (resultData.VoucherId > 0)
            {
                var voucherMap = VoucherMapping(resultData.VoucherId, model.CompanyFK.Value, model.PurchaseReturnId, "PurchaseReturn");


            }


            return resultData.VoucherId;
        }

        public async Task<VoucherModel> GetVouchersList(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in _db.Vouchers
                                                          join t2 in _db.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in _db.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                          where t1.CompanyId == companyId && t1.IsActive
                                                          && (voucherTypeId > 0 ? t1.VoucherTypeId == voucherTypeId : t1.VoucherTypeId > 0)
                                                          && t1.VoucherDate >= fromDate && t1.VoucherDate <= toDate
                                                          && t1.IsSubmit == vStatus
                                                          select new VoucherModel
                                                          {
                                                              VoucherId = t1.VoucherId,
                                                              VoucherDate = t1.VoucherDate,
                                                              Narration = t1.Narration,
                                                              VoucherNo = t1.VoucherNo,
                                                              VoucherTypeId = t1.VoucherTypeId,
                                                              VoucherTypeName = t2.Name,
                                                              CompanyId = t1.CompanyId,
                                                              CreateDate = t1.CreateDate,
                                                              ChqNo = t1.ChqNo,
                                                              ChqDate = t1.ChqDate,
                                                              ChqName = t1.ChqName,
                                                              IsStock = t1.IsStock,
                                                              IsSubmit = t1.IsSubmit,
                                                              IsIntegrated = t1.IsIntegrated,
                                                              CostCenterName = t3.Name,
                                                              ReportApprovalId = ((from t1s in _db.ReportApprovalDetails
                                                                                   join t2s in _db.ReportApprovals on t1s.ReportApprovalId equals t2s.ReportApprovalId
                                                                                   where t2s.CompanyId == t1.CompanyId &&
                                                                                   t2s.Month == t1.VoucherDate.Value.Month &&
                                                                                   t2s.Year == t1.VoucherDate.Value.Year &&
                                                                                   t1s.ApprovalStatus == 3
                                                                                   select t2s.ReportApprovalId
                                                                                ).FirstOrDefault())
                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }

    }
}
