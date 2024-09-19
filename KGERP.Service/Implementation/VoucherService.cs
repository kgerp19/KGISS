using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class VoucherService : IVoucherService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public VoucherService(ERPEntities context)
        {
            this.context = context;
        }
        public List<VoucherModel> GetVouchers(int companyId, string searchDate, string searchText)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            IQueryable<VoucherModel> vouchers = context.Database.SqlQuery<VoucherModel>("exec spGetVoucherList {0}", companyId).AsQueryable();
            if (dateSearch == null)
            {
                return vouchers.Where(x => (x.VoucherNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                    (x.Narration.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.VoucherDate).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return vouchers.Where(x => x.VoucherDate == dateSearch).OrderByDescending(x => x.VoucherDate).ToList();
            }


            return vouchers.Where(x => x.VoucherDate == dateSearch &&
                                (x.VoucherNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                (x.Narration.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.VoucherDate).ToList();
        }
        public async Task<VoucherModel> GetVouchersList(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus,int? voucherTypeId)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers 
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in context.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                          where t1.CompanyId == companyId && t1.IsActive
                                                          && (voucherTypeId > 0? t1.VoucherTypeId == voucherTypeId: t1.VoucherTypeId > 0)
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
                                                              IsIntegrated= t1.IsIntegrated,
                                                              CostCenterName=t3.Name,
                                                              ReportApprovalId = ((from t1s in context.ReportApprovalDetails
                                                                                join t2s in context.ReportApprovals on t1s.ReportApprovalId equals t2s.ReportApprovalId
                                                                                where t2s.CompanyId == t1.CompanyId &&
                                                                                t2s.Month == t1.VoucherDate.Value.Month &&
                                                                                t2s.Year == t1.VoucherDate.Value.Year &&
                                                                                t1s.ApprovalStatus == 3
                                                                                select t2s.ReportApprovalId
                                                                                ).FirstOrDefault())

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }
        public async Task<VoucherModel> GetDraftVouchersList(int companyId, DateTime? fromDate, DateTime? toDate, int? voucherTypeId, bool? vStatus)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in context.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
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
                                                              ReportApprovalId = ((from t1s in context.ReportApprovalDetails
                                                                                   join t2s in context.ReportApprovals on t1s.ReportApprovalId equals t2s.ReportApprovalId
                                                                                   where t2s.CompanyId == t1.CompanyId &&
                                                                                   t2s.Month == t1.VoucherDate.Value.Month &&
                                                                                   t2s.Year == t1.VoucherDate.Value.Year &&
                                                                                   t1s.ApprovalStatus == 3
                                                                                   select t2s.ReportApprovalId
                                                                                ).FirstOrDefault())

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }
        public async Task<VoucherModel> GetVouchersListWithDelete(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in context.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                          where t1.CompanyId == companyId && t1.IsActive
                                                          && (voucherTypeId > 0 ? t1.VoucherTypeId == voucherTypeId : t1.VoucherTypeId > 0)
                                                          && t1.VoucherDate >= fromDate && t1.VoucherDate <= toDate
                                                          && t1.IsSubmit == vStatus && t1.IsIntegrated
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
                                                              ReportApprovalId = ((from t1s in context.ReportApprovalDetails
                                                                                   join t2s in context.ReportApprovals on t1s.ReportApprovalId equals t2s.ReportApprovalId
                                                                                   where t2s.CompanyId == t1.CompanyId &&
                                                                                   t2s.Month == t1.VoucherDate.Value.Month &&
                                                                                   t2s.Year == t1.VoucherDate.Value.Year &&
                                                                                   t1s.ApprovalStatus == 3
                                                                                   select t2s.ReportApprovalId
                                                                                ).FirstOrDefault())

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }


        public async Task<VoucherModel> GetVouchersListByVoucherType(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int voucherTypeId)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in context.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                          where t1.CompanyId == companyId && t1.IsActive
                                                          && t1.VoucherTypeId == voucherTypeId
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
                                                              CostCenterName = t3.Name

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }

        public async Task<VoucherModel> GetStockVouchersList(int companyId)
        {
            VoucherModel voucherModel = new VoucherModel();
            voucherModel.CompanyId = companyId;
            //voucherModel.VoucherTypeId = voucherTypeId;
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          join t3 in context.Accounting_CostCenter on t1.Accounting_CostCenterFk equals t3.CostCenterId
                                                          where t1.CompanyId == companyId && t1.IsActive && t1.IsStock
                                                          
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
                                                              CostCenterName = t3.Name

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }

        public async Task<VoucherModel> GetVouchersList(VoucherModel voucherModel)
        {
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          where t1.CompanyId == voucherModel.CompanyId && t1.IsActive
                                                          && t1.VoucherDate > voucherModel.FromDate && t1.VoucherDate < voucherModel.ToDate
                                                          && t1.IsSubmit == voucherModel.IsSubmit
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
                                                              IsSubmit = t1.IsSubmit

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());
            return voucherModel;
        }
        public async Task<List<VoucherModel>> GetAllVouchersList(int companyId,int? voucherTypeId, DateTime? fromDate, DateTime? toDate)
        {
            List<VoucherModel> modelList = new List<VoucherModel>();

            VoucherModel voucherModel = new VoucherModel();
            voucherModel.DataList = await Task.Run(() => (from t1 in context.Vouchers
                                                          join t2 in context.VoucherTypes on t1.VoucherTypeId equals t2.VoucherTypeId
                                                          where t1.CompanyId == voucherModel.CompanyId && t1.IsActive
                                                          && t1.VoucherDate > voucherModel.FromDate && t1.VoucherDate < voucherModel.ToDate
                                                          && t1.IsSubmit == voucherModel.IsSubmit
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
                                                              IsSubmit = t1.IsSubmit

                                                          }).OrderByDescending(x => x.VoucherId).AsEnumerable());

            modelList.Add(voucherModel);

            return modelList;
        }



        public VoucherModel GetVoucher(int companyId, long id)
        {
            if (id == 0)
            {
                return new VoucherModel() { VoucherId = id };
            }
            Voucher voucher = context.Vouchers.Find(id);
            return ObjectConverter<Voucher, VoucherModel>.Convert(voucher);
        }


        //public VoucherModel CreateTempVoucher(VoucherModel model)
        //{
        //    HeadGL headGL = context.HeadGLs.Find(model.AccountHeadId);

        //    TempVoucher tempVoucher = new TempVoucher
        //    {
        //        CompanyId = model.CompanyId,
        //        VoucherTypeId = model.VoucherTypeId,
        //        VoucherNo = model.VoucherNo,
        //        VoucherDate = model.VoucherDate,
        //        AccountHeadId = model.AccountHeadId,
        //        ChqNo = model.ChqNo,
        //        ChqName = model.ChqName,
        //        ChqDate = model.ChqDate,
        //        ProjectId = model.ProjectId,
        //        Narration = model.Narration,
        //        Particular = model.Particular,
        //        DebitAmount = model.DebitAmount ?? 0,
        //        CreditAmount = model.CreditAmount ?? 0,
        //        AccCode = headGL.AccCode,
        //        AccName = headGL.AccName,
        //        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
        //        CreateDate = DateTime.Now,
        //    };

        //    context.TempVouchers.Add(tempVoucher);
        //    context.SaveChanges();

        //    List<VoucherDetailModel> voucherDetails = new List<VoucherDetailModel>();
        //    voucherDetails = context.Database.SqlQuery<VoucherDetailModel>("exec spGetVoucherDetailGrid {0}", model.CompanyId).ToList();

        //    model.VoucherDetails = voucherDetails;
        //    return model;

        //}




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }






        public string GetVoucherNo(int voucherTypeId, int companyId, DateTime voucherDate)
        {
            //Mamunnnnnnnnnnnnnnn
            VoucherType voucherType = context.VoucherTypes.Where(x => x.VoucherTypeId == voucherTypeId).FirstOrDefault();
            string voucherNo = string.Empty;
            int vouchersCount = context.Vouchers.Where(x => x.VoucherTypeId == voucherTypeId && x.CompanyId == companyId
            && x.VoucherDate.Value.Month == voucherDate.Month
            && x.VoucherDate.Value.Year == voucherDate.Year).Count();

            vouchersCount++;
            voucherNo = voucherType.Code + "-" + vouchersCount.ToString().PadLeft(4, '0');
            
            return voucherNo;
        }

        private string GenerateVoucherNo(string lastVoucherNo)
        {
            string prefix = lastVoucherNo.Substring(0, 4);
            int code = Convert.ToInt32(lastVoucherNo.Substring(4, 6));
            int newCode = code + 1;
            return prefix + newCode.ToString().PadLeft(6, '0');
        }

        public bool SaveVoucher(VoucherModel model, out string message)
        {
            message = string.Empty;
            int noOfRowsAffected = 0;
            double sumDebitAmount = model.VoucherDetails.Sum(x => x.DebitAmount) ?? 0;
            double sumCreditAmount = model.VoucherDetails.Sum(x => x.CreditAmount) ?? 0;

            if (Math.Round(sumDebitAmount, 2) != Math.Round(sumCreditAmount, 2))
            {
                message = "Voucher posting failed. Debit and credit amount did not match.";
                return noOfRowsAffected > 0;
            }
            Voucher voucher = new Voucher()
            {
                CompanyId = model.CompanyId,
                VoucherTypeId = model.VoucherTypeId,
                VoucherNo = model.VoucherNo,
                VoucherDate = model.VoucherDate,
                ChqNo = model.ChqNo,
                ChqName = model.ChqName,
                ChqDate = model.ChqDate,
                Narration = model.Narration,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreateDate = DateTime.Now,
                Accounting_CostCenterFk = model.Accounting_CostCenterFk,
                IsActive = true,
                VoucherStatus = "A",
                IsSubmit = false,
            };

            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();

            voucherDetails = model.VoucherDetails.Select(x =>
            new VoucherDetail
            {
                AccountHeadId = x.AccountHeadId,
                DebitAmount = x.DebitAmount ?? 0,
                CreditAmount = x.CreditAmount ?? 0,
                Particular = x.Particular,
                TransactionDate = model.VoucherDate,
                IsActive = true
            }).ToList();

            voucher.VoucherDetails = voucherDetails;
            context.Vouchers.Add(voucher);
            noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                context.Database.ExecuteSqlCommand("exec spVoucerPostingToIncomeAccount {0},{1}", voucher.VoucherId, voucher.CompanyId);
                message = "Voucher posted successfully.";
            }

            return noOfRowsAffected > 0;
        }

        public object GetVoucherNoAutoComplete(string prefix, int companyId)
        {
            return context.Vouchers.Where(x => x.CompanyId == companyId && x.VoucherNo.StartsWith(prefix)).Select(x => new
            {
                label = x.VoucherNo,
                val = x.VoucherNo
            }).OrderBy(x => x.label).Take(20).ToList();
        }

        //public VoucherModel RemoveVoucherItem(long id)
        //{
        //    var tempVoucher = context.TempVouchers.Find(id);

        //    VoucherModel model = new VoucherModel
        //    {
        //        CompanyId = tempVoucher.CompanyId,
        //        VoucherTypeId = tempVoucher.VoucherTypeId,
        //        VoucherNo = tempVoucher.VoucherNo,
        //        VoucherDate = tempVoucher.VoucherDate,
        //        AccountHeadId = tempVoucher.AccountHeadId??0,
        //        ChqNo = tempVoucher.ChqNo,
        //        ChqName = tempVoucher.ChqName,
        //        ChqDate = tempVoucher.ChqDate,
        //        ProjectId = tempVoucher.ProjectId,
        //        Narration = tempVoucher.Narration,
        //        Particular = tempVoucher.Particular,
        //        DebitAmount = tempVoucher.DebitAmount,
        //        CreditAmount = tempVoucher.CreditAmount,   
        //    };
        //    if (tempVoucher !=null)
        //    {
        //        context.TempVouchers.Remove(tempVoucher);
        //        context.SaveChanges();
        //    }

        //    List<VoucherDetailModel> voucherDetails = new List<VoucherDetailModel>();
        //    voucherDetails = context.Database.SqlQuery<VoucherDetailModel>("exec spGetVoucherDetailGrid {0}", model.CompanyId).ToList();
        //    model.VoucherDetails = voucherDetails;

        //    return model;
        //}
    }
}
