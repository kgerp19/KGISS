using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.SeedProcessingModel;
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

        public SeedProcessingService(ERPEntities db)
        {
            _db = db;
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
                        CreatedDate = DateTime.Today,
                        IsActive = true,
                        CompanyId = modelSeed.CompanyFK
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
                             join t3 in _db.Employees on t1.SeedProcessBy.ToLower() equals t3.EmployeeId into EmpLeft
                             from t4 in EmpLeft.DefaultIfEmpty()
                             where t1.SeedProcessingId == seedProcessingId && t1.IsActive
                             select new SeedProcessingDetailsVM
                             {
                                 SeedProcessingId = t1.SeedProcessingId,
                                 SeedProcessNo = t1.SeedProcessNo,
                                 SeedProcessDate = t1.SeedProcessDate,
                                 SeedProcessBy = t4.Name,
                                 CreatedDate = t1.CreatedDate,
                                 IsSumitted = t1.IsSubmitted
                             }).OrderBy(x => x.SeedProcessDate).FirstOrDefaultAsync();

            var resultList = await (from l1 in _db.SeedProcessingDetails
                                    join l3 in _db.MaterialReceiveDetails on l1.MaterialReceiveDetailId equals l3.MaterialReceiveDetailId
                                    join l2 in _db.Products on l1.ProductId equals l2.ProductId
                                    where l1.SeedProcessingId == seedProcessingId && l1.IsActive && l2.IsActive
                                    select new SeedProcessingDetailsVM
                                    {
                                        ProductId = l1.ProductId.Value,
                                        ProductName = l2.ProductName,
                                        SeedProcessingDetailsId = l1.SeedProcessingDetailId,
                                        Amount = l1.Amount,
                                        ReceiveQty = l3.ReceiveQty,
                                        StockInQty = l3.StockInQty,
                                        UnitPrice = l3.UnitPrice
                                    }).ToListAsync();
            if (resultList.Count > 0)
            {
                vmModel.DataList = resultList;
            }


            return vmModel;

        }

        public async Task<SeedProcessingDetailsVM> GetDataSeedProcessingList(int companyId)
        {
            SeedProcessingDetailsVM vmModel = new SeedProcessingDetailsVM();

            vmModel.DataList = await (from t1 in _db.SeedProcessings
                                      join t3 in _db.Employees on t1.SeedProcessBy.ToLower() equals t3.EmployeeId into EmpLeft
                                      where t1.IsActive
                                      from t4 in EmpLeft.DefaultIfEmpty()

                                      select new SeedProcessingDetailsVM
                                      {
                                          CompanyFK = t1.CompanyId.Value,
                                          SeedProcessingId = t1.SeedProcessingId,
                                          SeedProcessNo = t1.SeedProcessNo,
                                          SeedProcessDate = t1.SeedProcessDate,
                                          SeedProcessBy = t4.Name,
                                          CreatedDate = t1.CreatedDate
                                      }).OrderBy(x => x.SeedProcessDate).ToListAsync();
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
