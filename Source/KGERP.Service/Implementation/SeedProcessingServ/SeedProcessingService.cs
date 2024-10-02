﻿using KGERP.Data.Models;
using KGERP.Service.Implementation.Production;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.SeedProcessingModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.SeedProcessingServ
{
    public class SeedProcessingService : ISeedProcessingService
    {
        private readonly ERPEntities _db;
        private readonly AccountingService _accountingService;
        public SeedProcessingService(ERPEntities db)
        {
            _db = db;
            _accountingService = new AccountingService(db);
        }
        public async Task<long> CreateSeepProcessing(SeedProcessingDetailsVM modelSeed, MaterialReceiveDetailsWithProductVM materialReceiveDetailsWithProductVM)
        {
            long result = -1;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Generate Seed Process Number  
                    var poMax = await _db.SeedProcessings.MaxAsync(x => (long?)x.SeedProcessingId) ?? 0;
                    string poCid = $"SP-{DateTime.Now:yyMMdd}-{poMax + 1}";

                    // Create SeedProcessing entity  
                    var dbEntity = new SeedProcessing
                    {
                        SeedProcessNo = poCid,
                        SeedProcessDate = modelSeed.SeedProcessDate,
                        SeedProcessBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        HeadGLId= modelSeed.Accounting_HeadFK,
                        IsActive = true,
                        CompanyId = modelSeed.CompanyFK,
                        ModifiedDate=DateTime.Now
                    };

                    _db.SeedProcessings.Add(dbEntity);
                    await _db.SaveChangesAsync();

                    // Prepare SeedProcessingDetails  
                    var seedProcessingDetails = materialReceiveDetailsWithProductVM.DataListPro
                        .Where(x => x.Amount > 0)
                        .Select(item => new SeedProcessingDetail
                        {
                            SeedProcessingId = dbEntity.SeedProcessingId,
                            MaterialReceiveDetailId = item.MaterialReceiveDetailId,
                            Amount = item.Amount,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Today,
                            ProductId = item.ProductId,
                            IsActive = true
                        }).ToList();

                    // Add SeedProcessingDetails  
                    _db.SeedProcessingDetails.AddRange(seedProcessingDetails);
                    await _db.SaveChangesAsync();

                    // Commit transaction  
                    transaction.Commit();
                    result = dbEntity.SeedProcessingId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the exception (consider using a logging framework)  
                    throw new Exception("An error occurred while creating seed processing.", ex);
                }
            }

            return result;
        }

        public async Task<long> DeleteSeedProcessing(long SeedProcessingId)
        {
            long result = -1;
            var SeedProdessingData = await _db.SeedProcessings.FirstOrDefaultAsync(x => x.SeedProcessingId == SeedProcessingId);
            if (SeedProdessingData.SeedProcessingId > 0)
            {
                SeedProdessingData.IsActive = false;
                SeedProdessingData.ModifiedDate = DateTime.Now;
                SeedProdessingData.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                _db.Entry(SeedProdessingData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = SeedProdessingData.SeedProcessingId;
                }

            }
            return result;
        }

        public async Task<long> DeleteSeedProcessingDetails(long SeedProcessingDetailsId)
        {
            long result = -1;
            var SeedProdessingDetailsData = await _db.SeedProcessingDetails.FirstOrDefaultAsync(x => x.SeedProcessingDetailId == SeedProcessingDetailsId);
            if (SeedProdessingDetailsData.SeedProcessingDetailId > 0)
            {
                SeedProdessingDetailsData.IsActive = false;
                SeedProdessingDetailsData.ModifiedDate = DateTime.Now;
                SeedProdessingDetailsData.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                _db.Entry(SeedProdessingDetailsData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = SeedProdessingDetailsData.SeedProcessingId;
                }

            }
            return result;
        }

        public async Task<SeedProcessingDetailsVM> GetDataSeedProcessingAndDetailsBySeedProcessingId(long seedProcessingId)
        {
            SeedProcessingDetailsVM vmModel = new SeedProcessingDetailsVM();

            vmModel = await (from t1 in _db.SeedProcessings
                             join t5 in _db.HeadGLs on t1.HeadGLId equals t5.Id into HeadGLLeft
                             from t6 in HeadGLLeft.DefaultIfEmpty()
                             join t3 in _db.Employees on t1.SeedProcessBy.ToLower() equals t3.EmployeeId into EmpLeft
                             from t4 in EmpLeft.DefaultIfEmpty()
                             where t1.SeedProcessingId == seedProcessingId && t1.IsActive
                             select new SeedProcessingDetailsVM
                             {
                                 CompanyFK=t1.CompanyId.Value,
                                 BankOrCasgAccHeahId = t1.HeadGLId,
                                 SeedProcessingId = t1.SeedProcessingId,
                                 SeedProcessNo = t1.SeedProcessNo,
                                 SeedProcessDate = t1.SeedProcessDate,
                                 SeedProcessBy = t4.Name,
                                 CreatedDate = t1.CreatedDate,
                                 IsSumitted = t1.IsSubmitted,
                                 AccountName=t6.AccName,
                                 Accounting_HeadFK=(t1.HeadGLId.HasValue && t1.HeadGLId!=null)?t1.HeadGLId.Value:0
                             }).OrderBy(x => x.SeedProcessDate).FirstOrDefaultAsync();

            vmModel.DataList  = await (from l1 in _db.SeedProcessingDetails
                                    join l3 in _db.MaterialReceiveDetails on l1.MaterialReceiveDetailId equals l3.MaterialReceiveDetailId
                                    join l2 in _db.Products on l1.ProductId equals l2.ProductId
                                    join t3 in _db.ProductSubCategories on l2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                    join t4 in _db.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                    
                                    where l1.SeedProcessingId == seedProcessingId && l1.IsActive && l2.IsActive
                                    select new SeedProcessingDetailsVM
                                    {
                                        AccountingHeadId = t4.AccountingHeadId,
                                        ProductId = l1.ProductId.Value,
                                        ProductName = t4.Name + " " + t3.Name + " " + l2.ProductName,
                                        SeedProcessingDetailsId = l1.SeedProcessingDetailId,
                                        StockInQty = l3.StockInQty,
                                        UnitPrice = l3.StockInRate.Value,
                                        StockInAmount = l3.StockInQty.Value * l3.StockInRate.Value,
                                        Amount = l1.Amount,
                                        PreviousAmount = _db.SeedProcessingDetails
                                                        .Where(x => x.MaterialReceiveDetailId == l3.MaterialReceiveDetailId
                                                                    && x.SeedProcessingId != seedProcessingId
                                                                    && x.IsActive).Select(x => x.Amount).DefaultIfEmpty(0).Sum(),
                                         

                                    }).OrderByDescending(x => x.SeedProcessingDetailsId).ToListAsync();
             


            return vmModel;

        }

        public async Task<SeedProcessingDetailsVM> GetDataSeedProcessingList(int companyId)
        {
            SeedProcessingDetailsVM vmModel = new SeedProcessingDetailsVM();

            vmModel.DataList = await (from t1 in _db.SeedProcessings
                                      join t5 in _db.HeadGLs on t1.HeadGLId equals t5.Id into HeadLeft
                                      from t6 in HeadLeft.DefaultIfEmpty()
                                      join t3 in _db.Employees on t1.SeedProcessBy.ToLower() equals t3.EmployeeId into EmpLeft
                                      where t1.IsActive && t1.CompanyId==companyId
                                      from t4 in EmpLeft.DefaultIfEmpty()

                                      select new SeedProcessingDetailsVM
                                      {
                                          CompanyFK = t1.CompanyId.Value,
                                          SeedProcessingId = t1.SeedProcessingId,
                                          SeedProcessNo = t1.SeedProcessNo,
                                          SeedProcessDate = t1.SeedProcessDate,
                                          SeedProcessBy = t4.Name,
                                          CreatedDate = t1.CreatedDate,
                                          IsSumitted=t1.IsSubmitted,
                                          Accounting_HeadFK=t1.HeadGLId??0,
                                          AccountName=t6.AccCode

                                      }).OrderByDescending(x => x.SeedProcessingId).ToListAsync();
            return vmModel;
        }

        public async Task<long> SeedProcessingDetailsUpdate(SeedProcessingDetailsVM detailsVM)
        {
            long result = -1;
            var SeedProdessingDetailsData = await _db.SeedProcessingDetails.FirstOrDefaultAsync(x => x.SeedProcessingDetailId == detailsVM.SeedProcessingDetailsId);
            if (SeedProdessingDetailsData.SeedProcessingDetailId > 0)
            {
                SeedProdessingDetailsData.Amount = detailsVM.Amount;
                _db.Entry(SeedProdessingDetailsData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = SeedProdessingDetailsData.SeedProcessingId;
                }

            }
            return result;
        }

        public async Task<long> SeedProcessingSubmitted(SeedProcessingDetailsVM detailsVM)
        {
            long result = -1;
            bool SeedProcessingSubAndUnSub = !detailsVM.IsSumitted;
            var SeedProdessingData = await _db.SeedProcessings.FirstOrDefaultAsync(x => x.SeedProcessingId == detailsVM.SeedProcessingId);
            if (SeedProdessingData.SeedProcessingId > 0)
            {
                SeedProdessingData.IsSubmitted = SeedProcessingSubAndUnSub;
                SeedProdessingData.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                SeedProdessingData.ModifiedDate = DateTime.Today;
                _db.Entry(SeedProdessingData).State = EntityState.Modified;

                if (await _db.SaveChangesAsync() > 0)
                {
                    result = SeedProdessingData.SeedProcessingId;
                }
                if (result > 0 && SeedProdessingData.IsSubmitted)
                {
                    var productionVm = await Task.Run(() => GetDataSeedProcessingAndDetailsBySeedProcessingId(detailsVM.SeedProcessingId));
                    await _accountingService.AccountingProcessingPushISS(productionVm);
                }
            }
            return result;
        }

        

        public async Task<long> SeedProcessingUpdate(SeedProcessingDetailsVM detailsVM)
        {
            long result = -1;
            var SeedProdessingData = await _db.SeedProcessings.FirstOrDefaultAsync(x => x.SeedProcessingId == detailsVM.SeedProcessingId);
            if (SeedProdessingData.SeedProcessingId > 0)
            {
                SeedProdessingData.SeedProcessDate = detailsVM.SeedProcessDate;
                SeedProdessingData.HeadGLId = detailsVM.Accounting_HeadFK;
                _db.Entry(SeedProdessingData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync() > 0)
                {
                    result = SeedProdessingData.SeedProcessingId;
                }

            }
            return result;
        }
    }
}
