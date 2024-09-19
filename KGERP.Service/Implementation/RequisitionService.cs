using KGERP.Data.Models;

using KGERP.Service.CommonModels.Model;
using KGERP.Service.Configuration;
using KGERP.Service.Contracts.KGERP.Requisitions.Command.RequestResponseModel;
using KGERP.Service.Contracts.KGERP.Requisitions.Queries.RequestModel;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Configuration;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class RequisitionService : IRequisitionService
    {
        private readonly ERPEntities context;
        public RequisitionService(ERPEntities context)
        {
            this.context = context;
        }
        public int GetRequisitionNo()
        {
            int requisitionId = 0;
            var value = context.Requisitions.OrderByDescending(x => x.RequisitionId).FirstOrDefault();
            if (value != null)
            {
                requisitionId = value.RequisitionId + 1;
            }
            else
            {
                requisitionId = requisitionId + 1;
            }
            return requisitionId;
        }

        public async Task<int> CreateProductionRequisition(RequisitionModel model)
        {
            int requisitionItemId = -0;

            int result = -1;
            #region Check Packing Materials
            List<Product> products = new List<Product>();
            Product product = context.Products.Where(x => x.ProductId == model.ProductId && x.PackId == null).FirstOrDefault();
            if (product != null)
            {
                products.Add(product);
            }

            if (products.Any())
            {
                var s = string.Join(",", products.Where(p => p.PackId == null)
                                    .Select(p => p.ProductName.ToString()));
                StringBuilder sb = new StringBuilder();
                sb.Append("Bag is not selected for product " + s + ". Please select bag first !");
                //message = sb.ToString();
                return result;
            }
            #endregion

            //Requisition requisition = ObjectConverter<RequisitionModel, Requisition>.Convert(model);
            #region Requisition No
            var lastReqNo = context.Requisitions.Where(q => q.CompanyId == (int)CompanyName.KrishibidFeedLimited)
               .OrderByDescending(o => o.RequisitionId)
               .Select(s => s.RequisitionNo).FirstOrDefault();


            string reNo = "";
            if (lastReqNo == null)
            {
                reNo = "R000001";
            }
            else
            {
                reNo = GenerateRequisitionNo(lastReqNo);
            }
            #endregion

            Requisition requisition = new Requisition
            {
                RequisitionDate = model.RequisitionDate,
                Description = model.Description,
                RequisitionNo = reNo,
                IsActive = true,
                RequisitionBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                RequisitionStatus = "N",
                CompanyId = model.CompanyId
            };
            context.Requisitions.Add(requisition);
            if (context.SaveChanges() > 0)
            {
                result = requisition.RequisitionId;
            }



            return result;
        }
        public async Task<int> CreateProductionRequisitionItem(RequisitionModel model)
        {
            int requisitionItemId = -0;

            int result = -1;
            #region Check Packing Materials
            //List<Product> products = new List<Product>();
            Product packingItemCheck = context.Products.Where(x => x.ProductId == model.ProductId && x.PackId == null).FirstOrDefault();

            if (packingItemCheck != null)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("Bag is not selected for product " + packingItemCheck.ProductName + ". Please select bag first !");

                return result;
            }
            #endregion

            //Requisition requisition = ObjectConverter<RequisitionModel, Requisition>.Convert(model);

            Product product = context.Products.Find(model.ProductId);


            RequisitionItem requisitionItem = new RequisitionItem
            {
                RequisitionId = model.RequisitionId,
                ProductId = model.ProductId,
                RequisitionItemStatus = "N",
                Qty = model.Qty,
                InputQty = model.InputQty,
                OutputQty = model.OutputQty,
                ActualProcessLoss = model.ActualProcessLoss,
                OverHead = model.OverHead,
                ProcessLoss = model.ProcessLoss,
                BagId = context.Products.SingleOrDefault(s => s.ProductId == model.ProductId)?.PackId,

                //  BagQty = Convert.ToInt32(Math.Ceiling(model.Qty/ Convert.ToDecimal(product.PackSize))),
                IsActive = true
            };
            context.RequisitionItems.Add(requisitionItem);
            if (context.SaveChanges() > 0)
            {
                result = requisitionItem.RequisitionItemId;
            }

            return result;
        }



        public List<RequisitionItemModel> GetRequisitionItemIssueStatus(int requisitionId)
        {
            IQueryable<RequisitionItem> requisitionItems = context.RequisitionItems.Include(x => x.Product).Where(x => x.RequisitionId == requisitionId);

            return ObjectConverter<RequisitionItem, RequisitionItemModel>.ConvertList(requisitionItems.ToList()).ToList();
        }

        public ProductModel GetProcessLossAmount(int productId)
        {
            ProductModel data = ObjectConverter<Product, ProductModel>.Convert(context.Products.Where(x => x.ProductId == productId).FirstOrDefault());


            return data;
        }

        public bool DeleteRequisition(int requisitionId)
        {
            return context.Database.ExecuteSqlCommand("sp_Feed_DeleteRequisition {0}", requisitionId) > 0;
        }
        public async Task<RequisitionModel> GetRequisition(int companyId, int requisitionId)
        {
            RequisitionModel model = new RequisitionModel();
            model = await Task.Run(() => (from t1 in context.Requisitions
                                          .Where(x => x.IsActive == true && x.RequisitionId == requisitionId && x.CompanyId == companyId)
                                          select new RequisitionModel
                                          {
                                              RequisitionNo = t1.RequisitionNo,
                                              RequisitionId = t1.RequisitionId,
                                              Description = t1.Description,
                                              CompanyId = t1.CompanyId,
                                              RequisitionDate = t1.RequisitionDate,
                                              IsSubmitted = t1.IsSubmitted,
                                              RequisitionStatus = t1.RequisitionStatus
                                          }).FirstOrDefaultAsync());

            model.Items = await Task.Run(() => (from t1 in context.RequisitionItems
                                                join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                where t1.IsActive == true && t1.RequisitionId == model.RequisitionId
                                                select new RequisitionItemModel
                                                {
                                                    RequisitionId = t1.RequisitionId,
                                                    ProductName = t3.ProductName,
                                                    ProductId = t3.ProductId,
                                                    InputQty = t1.InputQty,
                                                    ProcessLoss = t1.ProcessLoss,
                                                    RequisitionItemId = t1.RequisitionItemId,
                                                    Qty = t1.Qty
                                                }).OrderByDescending(x => x.RequisitionId)
                                                .ToListAsync());

            return model;
        }

        //public async Task<RequisitionModel> GetRequisition2( int companyId,int id)
        //{
        //    var model = new RequisitionModel();

        //    string requisitionNo = string.Empty;
        //    if (id <= 0)
        //    {
        //        IQueryable<Requisition> requisitions = context.Requisitions.Where(x => x.CompanyId == companyId);
        //        int count = requisitions.Count();
        //        if (count == 0)
        //        {
        //            return new RequisitionModel()
        //            {
        //                RequisitionNo = "R000001",
        //                CompanyId = companyId
        //            };
        //        }

        //        requisitions = requisitions
        //            .Where(x => x.CompanyId == companyId)
        //            .OrderByDescending(x => x.RequisitionId)
        //            .Take(1);

        //        requisitionNo = requisitions
        //            .ToList()
        //            .FirstOrDefault().RequisitionNo;

        //        requisitionNo = GenerateRequisitionNo(requisitionNo);
        //        return new RequisitionModel()
        //        {
        //            RequisitionNo = requisitionNo,
        //            CompanyId = companyId
        //        };


        //    }


        //        var requisition = context.Requisitions
        //            .Include(x => x.RequisitionItems)
        //            .Where(x => x.RequisitionId == id)
        //            .FirstOrDefault();


        //    model.RequisitionNo = requisition.RequisitionNo;
        //    model.RequisitionDate = requisition.RequisitionDate;
        //    model.Description = requisition.Description;
        //    model.CompanyId = requisition.CompanyId;
        //    model.IsSubmitted = requisition.IsSubmitted;
        //    model.RequisitionId = requisition.RequisitionId;
        //    model.RequisitionStatus = requisition.RequisitionStatus;
        //    foreach (var item in requisition.RequisitionItems)
        //    {
        //        var reqItem = new RequisitionItemModel()
        //        {
        //            ProductName = context.Products.SingleOrDefault(q => q.ProductId == item.ProductId)?.ProductName,
        //            InputQty = item.InputQty,
        //            Qty= item.Qty,
        //            ProcessLoss= item.ProcessLoss

        //        };
        //        model.Items.Add(reqItem);

        //    }

        //    // model = ObjectConverter<Requisition, RequisitionModel>.Convert(requisition);

        //    if (requisition == null)
        //    {
        //        throw new Exception("Data not found");
        //    }

        //   // model = ObjectConverter<Requisition, RequisitionModel>.Convert(requisition);
        //    return model;
        //}

        public List<RequisitionItemModel> GetRequisitionItems(int requisitionId)
        {
            List<RequisitionItemModel> requisitionItems = context.Database.SqlQuery<RequisitionItemModel>("exec sp_Feed_GetRequisitionItems {0}", requisitionId).ToList();
            return requisitionItems;
        }
        private string GenerateRequisitionNo(string lastRequisitionNo)
        {
            string numberPortion = lastRequisitionNo.Substring(1, 6);
            int num = Convert.ToInt32(numberPortion);
            num = ++num;
            return "R" + num.ToString().PadLeft(6, '0');
        }


        public async Task<RequisitionModel> RequisitionList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            RequisitionModel requisitionModel = new RequisitionModel();
            requisitionModel.CompanyId = companyId;
            requisitionModel.DataList = await Task.Run(() => (from t1 in context.Requisitions
                                                              where t1.CompanyId == companyId
                                                              && t1.RequisitionDate >= fromDate
                                                              && t1.RequisitionDate <= toDate
                                                              select new RequisitionModel
                                                              {
                                                                  CompanyId = t1.CompanyId,
                                                                  RequisitionId = t1.RequisitionId,
                                                                  RequisitionDate = t1.RequisitionDate,
                                                                  RequisitionNo = t1.RequisitionNo,
                                                                  RequisitionBy = t1.RequisitionBy,
                                                                  Description = t1.Description,
                                                                  RequisitionStatus = t1.RequisitionStatus,
                                                                  IsSubmitted = t1.IsSubmitted
                                                              }).OrderByDescending(x => x.RequisitionDate).AsEnumerable());




            return requisitionModel;
        }
        public async Task<RequisitionModel> RequisitionIssuePendingList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            RequisitionModel requisitionModel = new RequisitionModel();
            requisitionModel.CompanyId = companyId;
            requisitionModel.DataList = await Task.Run(() => (from t1 in context.Requisitions
                                                              where t1.CompanyId == companyId
                                                              && t1.RequisitionStatus == "D"
                                                              && t1.RequisitionDate >= fromDate
                                                              && t1.RequisitionDate <= toDate
                                                              && t1.IsSubmitted == false
                                                              select new RequisitionModel
                                                              {
                                                                  CompanyId = t1.CompanyId,
                                                                  RequisitionId = t1.RequisitionId,
                                                                  RequisitionDate = t1.RequisitionDate,
                                                                  RequisitionNo = t1.RequisitionNo,
                                                                  RequisitionBy = t1.RequisitionBy,
                                                                  Description = t1.Description,
                                                                  RequisitionStatus = t1.RequisitionStatus,
                                                                  IsSubmitted = t1.IsSubmitted
                                                              }).OrderByDescending(x => x.RequisitionDate).AsEnumerable());
            return requisitionModel;
        }

        public async Task<RequisitionModel> RequisitionDeliveryPendingList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            RequisitionModel requisitionModel = new RequisitionModel();
            requisitionModel.CompanyId = companyId;
            requisitionModel.DataList = await Task.Run(() => (from t1 in context.Requisitions
                                                              where t1.CompanyId == companyId
                                                              && t1.RequisitionStatus == "N"
                                                              && t1.RequisitionDate >= fromDate
                                                              && t1.RequisitionDate <= toDate
                                                              select new RequisitionModel
                                                              {
                                                                  CompanyId = t1.CompanyId,
                                                                  RequisitionId = t1.RequisitionId,
                                                                  RequisitionDate = t1.RequisitionDate,
                                                                  RequisitionNo = t1.RequisitionNo,
                                                                  RequisitionBy = t1.RequisitionBy,
                                                                  Description = t1.Description,
                                                                  RequisitionStatus = t1.RequisitionStatus

                                                              }).OrderByDescending(x => x.RequisitionDate).AsEnumerable());

            return requisitionModel;
        }

        public RequisitionModel GetRequisitionById(int requisitionId)
        {
            Requisition requisition = context.Requisitions.Where(x => x.RequisitionId == requisitionId).FirstOrDefault();
            return ObjectConverter<Requisition, RequisitionModel>.Convert(requisition);
        }

        /*need to update */
        public List<RequisitionItemDetailModel> GetRequisitionItemDetails(int requisitionId)
        {
            var deliveryDate = context.Requisitions.SingleOrDefault(q => q.RequisitionId == requisitionId)?.DeliveredDate;
            List<RequisitionItem> requisitionItems = context.RequisitionItems.Where(x => x.RequisitionId == requisitionId).ToList();
            foreach (var requisitionItem in requisitionItems)
            {
                context.Database.ExecuteSqlCommand("sp_Feed_GetRM_AccordingtoFM {0}", requisitionItem.RequisitionItemId);
            }

            var data = context.Database.SqlQuery<RequisitionItemDetailModel>("sp_GetRmFormulaWithAvailableQty {0},{1}", requisitionId, deliveryDate).ToList();
            return data;
        }

        public List<RequisitionItemDetailModel> GetRequisitionItemDetails(int requisitionId, DateTime deliveryDate)
        {
            List<RequisitionItem> requisitionItems = context.RequisitionItems.Where(x => x.RequisitionId == requisitionId).ToList();
            foreach (var requisitionItem in requisitionItems)
            {
                context.Database.ExecuteSqlCommand("sp_Feed_GetRM_AccordingtoFM {0}", requisitionItem.RequisitionItemId);
            }

            var data = context.Database.SqlQuery<RequisitionItemDetailModel>("sp_GetRmFormulaWithAvailableQty {0},{1}", requisitionId, deliveryDate).ToList();
            return data;
        }

        public int CreateOrEdit(RequisitionModel model)
        {
            int noOfRowsAffected = 0;
            Requisition requisition = ObjectConverter<RequisitionModel, Requisition>.Convert(model);

            requisition = context.Requisitions.Find(model.RequisitionId);
            if (requisition != null)
            {
                requisition.RequisitionStatus = "D";
                requisition.DeliveredBy = System.Web.HttpContext.Current.User.Identity.Name;
                requisition.DeliveredDate = model.DeliveredDate;
                requisition.DeliveryNo = model.DeliveryNo;

                context.Entry(requisition).State = requisition.RequisitionId == 0 ? EntityState.Added : EntityState.Modified;
                noOfRowsAffected = context.SaveChanges();
            }

            //int result = context.Database.ExecuteSqlCommand("EXEC sp_RequisitionDelivery {0},{1},{2},{3},{4}", model.RequisitionId, model.DeliveredDate, model.DeliveryNo, model.DeliveredBy, model.CompanyId);


            if (noOfRowsAffected > 0)
            {
                List<RequisitionItem> requisitionItems = context.RequisitionItems.Where(x => x.RequisitionId == model.RequisitionId).ToList();
                if (requisitionItems.Any())
                {
                    foreach (var item in requisitionItems)
                    {
                        noOfRowsAffected = context.Database.ExecuteSqlCommand("EXEC spCreateFormulaHistory {0},{1},{2},{3},{4},{5}", item.RequisitionId, item.RequisitionItemId, item.ProductFormulaId, item.ProductId, requisition.DeliveredBy, requisition.CompanyId);
                    }
                }
            }
            return noOfRowsAffected > 0 ? requisition.RequisitionId : 0;


        }

        public string GetFormulaMessage(int requisitionId)
        {
            return context.Database.SqlQuery<string>(@"exec spProductFormulaNotExistForProducts {0}", requisitionId).FirstOrDefault();
        }

        public List<RequisitionItemModel> GetProductionItems(int companyId, int requisitionId, DateTime issueDate)
        {
            return context.Database.SqlQuery<RequisitionItemModel>("exec GetProductionItems {0},{1},{2}", companyId, requisitionId, issueDate).ToList();
        }


        public async Task<long> EditProductionReqisitionDetail(RequisitionModel model)
        {
            long result = -1;
            RequisitionItem models = await context.RequisitionItems
                .SingleOrDefaultAsync(s => s.RequisitionItemId == model.RequisitionItemId);

            models.RequisitionId = model.RequisitionId;
            models.ProductId = model.ProductId;
            models.Qty = model.Qty;
            models.InputQty = model.InputQty;
            models.OutputQty = model.OutputQty;
            models.ActualProcessLoss = model.ActualProcessLoss;
            models.ProcessLoss = model.ProcessLoss;


            context.RequisitionItems.Add(models);

            if (await context.SaveChangesAsync() > 0)
            {
                result = model.RequisitionId;
            }
            return result;
        }

        //public async Task<RResult> RequisitionSaveForPKL(RequisitionMasterDetailsRM model)
        //{
        //    RResult rResult = new RResult();
        //    var resultCount = 0;

        //    try
        //    {
        //        if (model != null)
        //        {
        //            var poMax = context.Requisitions.Where(x => x.CompanyId == 20).Count() + 1;
        //            string poCid = "";
        //            poCid = @"RQ-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
        //                    DateTime.Now.ToString("dd") + "-" + poMax.ToString();


        //            Requisition PackagingRequisition = new Requisition
        //            {
        //                RequisitionType = (int)EnumRequisitionType.PackgingRequisition,
        //                RequisitionNo = poCid,
        //                OrderDetailsId = model.OrderDetailsId,
        //                RequisitionDate = model.RequisitionDate,
        //                CompanyId = model.CompanyId,
        //                FromRequisitionId = model.FromRequisitionId,
        //                ToRequisitionId = model.ToRequisitionId,
        //                CreatedBy = model.CreatedBy,
        //                CreatedDate = DateTime.Now,
        //                RequisitionBy = model.CreatedBy,
        //                RequisitionStatus = "N",

        //                IsActive = true,
        //                IsSubmitted = false
        //            };
        //            context.Requisitions.Add(PackagingRequisition);

        //            if (await context.SaveChangesAsync() > 0)
        //            {
        //                var lastRequisitionId = PackagingRequisition.RequisitionId;

        //                var reqData = model.RequisitionItemDetail.Where(x => x.RQty > 0).ToList();

        //                foreach (var item in reqData)
        //                {
        //                    resultCount = context.Database.ExecuteSqlCommand("EXEC spRequisitionItemDetailCreate {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
        //                        0,
        //                        lastRequisitionId,
        //                        item.RProductId,
        //                        null,
        //                        item.RQty,
        //                        null,
        //                        null,
        //                        null,
        //                        null,
        //                        0,
        //                        true,
        //                        item.FinishProductBOMId);
        //                }

        //                if (resultCount > 0)
        //                {
        //                    rResult.returnURL = $"/Procurement/PackagingProductionRequisitionDetails?companyId={model.CompanyId}&requisitionId={lastRequisitionId}";
        //                    rResult.result = lastRequisitionId;
        //                    rResult.message = "Data Save Successfully!";
        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rResult.result = 0;
        //        rResult.message = "Data can't Save!";
        //    }
        //    return rResult;
        //}

        public async Task<int> PackagingGeneralRequisition(VMPackagingPurchaseRequisition model)
        {
            int rResult = 0;
            var resultCount = 0;

            try
            {
                if (model != null)
                {
                    if (model.RequisitionId == 0)
                    {
                        var poMax = context.Requisitions.Where(x => x.CompanyId == 20).Count() + 1;
                        string poCid = "";
                        poCid = @"RQ-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                DateTime.Now.ToString("dd") + "-" + poMax.ToString();


                        Requisition PackagingRequisition = new Requisition
                        {
                            RequisitionType = (int)EnumRequisitionType.PackgingRequisition,
                            RequisitionNo = poCid,

                            RequisitionDate = model.RequisitionDate,
                            CompanyId = model.CompanyFK,
                            FromRequisitionId = model.FromDepartmentReqId,
                            ToRequisitionId = model.ToDepartmentReqId,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = DateTime.Now,
                            RequisitionBy = model.CreatedBy,
                            RequisitionStatus="N",
                            IsActive = true,
                            IsSubmitted = false,
                            OrderDetailsId = 0,
                        };
                        context.Requisitions.Add(PackagingRequisition);
                        if (await context.SaveChangesAsync() > 0)
                        {
                            model.RequisitionId = PackagingRequisition.RequisitionId;
                        }

                    }




                    if (model.RequisitionId > 0)
                    {


                        resultCount = context.Database.ExecuteSqlCommand("EXEC spRequisitionItemDetailCreate {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            0,
                            model.RequisitionId,
                            model.RProductId,
                            null,
                            model.Qty,
                            null,
                            null,
                            null,
                            null,
                            0,
                            true,
                            0);


                        if (resultCount > 0)
                        {
                            rResult = model.RequisitionId;

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rResult;
        }



        public async Task<RResult> PackagingProductionRequisitionDelete(int RequisitionId)
        {
            RResult rResult = new RResult();

            try
            {
                var dbEntity = await context.Requisitions.FindAsync(RequisitionId);
                if (dbEntity != null)
                {
                    // Soft delete by setting IsActive to false
                    dbEntity.IsActive = false;

                    // Update the entity state to Modified
                    context.Entry(dbEntity).State = EntityState.Modified;

                    if (await context.SaveChangesAsync() > 0)
                    {
                        var dbEntityDetails = await context.RequisitionItemDetails
                        .Where(rd => rd.RequisitionId == RequisitionId)
                        .ToListAsync();

                        foreach (var detail in dbEntityDetails)
                        {
                            // Update the entity state to Modified
                            await context.Database.ExecuteSqlCommandAsync("UPDATE Erp.RequisitionItemDetail SET IsActive = 0 WHERE RequisitionId = {0}", RequisitionId);
                            //context.Entry(detail).State = EntityState.Modified;
                        }

                        rResult.result = 1;
                        rResult.message = "Data Deleted Successfully!";
                    }
                    else
                    {
                        rResult.result = 0;
                        rResult.message = "Data Couldn't Be Deleted Successfully!";
                    }


                }
                else
                {
                    rResult.result = 0;
                    rResult.message = "Requisition Not Found!";
                }
            }
            catch (Exception ex)
            {
                rResult.result = 0;
                rResult.message = "Data Couldn't Be Deleted Successfully!";
                // Log the exception
            }

            return rResult;
        }

        public async Task<int> PackagingGeneralRequisitionSubmit(int RequisitionId, bool isSubmited = false)
        {
            int requisitionId = 0;

            bool isSubmitedChk = !isSubmited;
            var requisitions = await context.Requisitions.FindAsync(RequisitionId);
            if (requisitions != null)
            {
                requisitions.IsSubmitted = isSubmitedChk;
                // Update the entity state to Modified
                context.Entry(requisitions).State = EntityState.Modified;
                if (await context.SaveChangesAsync() > 0)
                {
                    requisitionId = requisitions.RequisitionId;
                }
            }
            return requisitionId;

        }

        public async Task<PackagingProductionRequisitionDetailsRM> PackagingProductionRequisitionDetails(int CompanyId, int RequisitionId, CancellationToken cancellationToken = default)
        {
            try
            {
                PackagingProductionRequisitionDetailsRM result =
                await (from r in context.Requisitions.Where(x => x.IsActive == true && x.RequisitionId == RequisitionId && x.CompanyId == CompanyId)
                       join c in context.Companies on r.CompanyId equals c.CompanyId
                       join s in context.StockInfoes on r.FromRequisitionId equals s.StockInfoId
                       join s1 in context.StockInfoes on r.ToRequisitionId equals s1.StockInfoId
                       join od in context.OrderDetails.Where(x => x.IsActive) on r.OrderDetailsId equals od.OrderDetailId
                       join p in context.Products on od.ProductId equals p.ProductId
                       join psc in context.ProductSubCategories on p.ProductSubCategoryId equals psc.ProductSubCategoryId
                       join om in context.OrderMasters on od.OrderMasterId equals om.OrderMasterId
                       join e in context.Employees on r.RequisitionBy.Trim().ToLower() equals e.EmployeeId.Trim().ToLower()
                       join v in context.Vendors on om.CustomerId equals v.VendorId


                       select new PackagingProductionRequisitionDetailsRM
                       {
                           CustomerName = v.Name,
                           JobOrderNo = od.JobOrderNo,
                           IsSubmited = r.IsSubmitted,
                           RequisitionId = r.RequisitionId,
                           RequisitionNo = r.RequisitionNo,
                           CompanyName = c.Name,
                           CompanyId = c.CompanyId,
                           RequisitionDate = r.RequisitionDate.Value,
                           FromRequisitionName = s.Name,
                           ToRequisitionName = s1.Name,
                           OrderNo = om.OrderNo,
                           OrderDate = om.OrderDate,
                           RequisitionBy = e.Name,
                           ExpectedDeliveryDate = om.ExpectedDeliveryDate,
                           StockName = s.Name,
                           OrderMasterId = om.OrderMasterId,
                           ProductNames = psc.Name + " " + p.ProductName
                       }).OrderByDescending(x => x.RequisitionId).FirstOrDefaultAsync(cancellationToken);


                result.RequisitionItemDetail = await (from d in context.RequisitionItemDetails
                                                      join bom in context.FinishProductBOMs on d.FinishProductBOMId equals bom.ID
                                                      join p in context.Products on d.RProductId equals p.ProductId
                                                      join psc in context.ProductSubCategories on p.ProductSubCategoryId equals psc.ProductSubCategoryId
                                                      join pc in context.ProductCategories on psc.ProductCategoryId equals pc.ProductCategoryId

                                                      where d.RequisitionId == RequisitionId && d.IsActive == true
                                                      select new RequisitionItemDetailListRM
                                                      {
                                                          AllocatedQty = bom.RequiredQuantity,
                                                          RQty = d.RQty.Value,
                                                          ProductName = pc.Name + " " + psc.Name + " " + p.ProductName,
                                                          RequisitionId = d.RequisitionId,
                                                          RequistionItemDetailId = d.RequistionItemDetailId,
                                                          RProductId = d.RProductId.Value
                                                      }).OrderByDescending(x => x.RequisitionId).ToListAsync(cancellationToken); ;

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<RResult> RequisitionItemDetailDelete(Guid? RequistionItemDetailId)
        {
            RResult rResult = new RResult();
            int resultCount = 0;
            if (RequistionItemDetailId.HasValue)
            {
                resultCount = await context.Database.ExecuteSqlCommandAsync("UPDATE Erp.RequisitionItemDetail SET IsActive = 0 WHERE RequistionItemDetailId = {0}", RequistionItemDetailId);
                if (resultCount > 0)
                {
                    rResult.result = 1;
                    rResult.message = "Deleted Successfully!";
                }

            }
            return rResult;
        }

        public async Task<RResult> UpdateProductAndQuantityInRequisitionItemDetail(int ProductId, decimal Quentity, Guid? RequistionItemDetailId)
        {
            RResult rResult = new RResult();
            int resultCount = 0;
            if (RequistionItemDetailId.HasValue)
            {
                resultCount = await context.Database.ExecuteSqlCommandAsync("UPDATE Erp.RequisitionItemDetail SET RProductId={0},RQty={1} WHERE RequistionItemDetailId = {2}", ProductId, Quentity, RequistionItemDetailId);
                if (resultCount > 0)
                {
                    rResult.result = 1;
                    rResult.message = "Update Successfully";
                }

            }
            else
            {
                rResult.result = 0;
                rResult.message = "RequistionItemDetailId Can't Found!";
            }
            return rResult;

        }

        public RResult RequisitionDetailsSaveForPKL(RequisitionMasterDetailsRM model)
        {
            RResult rResult = new RResult();
            var resultCount = 0;

            try
            {
                if (model != null)
                {
                    foreach (var item in model.RequisitionItemDetail)
                    {
                        resultCount = context.Database.ExecuteSqlCommand("EXEC spRequisitionItemDetailCreate {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            0,
                            item.RequisitionId,
                            item.RProductId,
                            null,
                            item.RQty,
                            null,
                            null,
                            null,
                            null,
                            0,
                            true,
                            item.FinishProductBOMId);
                    }

                    if (resultCount > 0)
                    {
                        rResult.result = 1;
                        rResult.message = "Data Save Successfully!";
                    }



                }
            }
            catch (Exception ex)
            {
                rResult.result = 0;
                rResult.message = "Data can't Save!";
            }
            return rResult;
        }

        public async Task<int> RequisitionSubmitied(int requisitionId, CancellationToken cancellationToken = default)
        {
            int rResult = -1;
            var requisitions = context.Requisitions.Find(requisitionId);
            if (requisitions != null)
            {
                requisitions.IsSubmitted = true;
                context.Entry(requisitions).State = EntityState.Modified;
                if (await context.SaveChangesAsync() > 0)
                {
                    rResult = requisitions.RequisitionId;
                }
                return rResult;
            }
            return rResult;
        }
        public async Task<ResuisitionRM> GetResuisitionDataByRequisitionId(int ReqId, CancellationToken cancellationToken = default)
        {
            var result = await (
                 from t1 in context.Requisitions.Where(x => x.IsActive && x.RequisitionType == 3)
                 join t2 in context.OrderDetails.Where(x => x.IsActive) on t1.OrderDetailsId equals t2.OrderDetailId
                 join t3 in context.OrderMasters.Where(x => x.IsActive) on t2.OrderMasterId equals t3.OrderMasterId
                 join t4 in context.Products.Where(x => x.IsActive) on t2.ProductId equals t4.ProductId
                 join t5 in context.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                 join t6 in context.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                 join t7 in context.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId

                 where t1.RequisitionId == ReqId
                 select new ResuisitionRM
                 {
                     RequisitionDate = t1.RequisitionDate,
                     FromRequisitionId = t1.FromRequisitionId.Value,
                     ToRequisitionId = t1.ToRequisitionId.Value,
                     OrderDetailsId = t1.OrderDetailsId,


                     RequisitionId = t1.RequisitionId,
                     RequisitionNo = t1.RequisitionNo,

                     RequisitionStatus = t1.RequisitionStatus,
                     OrderNo = t3.OrderNo,
                     OrderDate = t3.OrderDate,
                     JobOrderNo = t2.JobOrderNo,
                     FromRequisitionName = t6.Name,
                     ToRequisitionName = t7.Name,
                     ProductName = t5.Name + " " + t4.ProductName,
                     Description = t3.Remarks,
                     Structure = t2.Remarks,
                     Qty = t2.Qty,
                     OrderMasterId = t3.OrderMasterId
                 }
             ).FirstOrDefaultAsync(cancellationToken);



            if (result != null)
            {
                result.DDLGerOrderList = (from t1 in context.OrderDetails.Where(x => x.OrderMasterId == result.OrderMasterId)
                                          join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                          join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                          join t6 in context.FinishProductBOMs on t1.OrderDetailId equals t6.OrderDetailId
                                          where t1.IsActive
                                          select new SelectListItem
                                          {
                                              Value = t1.OrderDetailId.ToString(),
                                              Text = t4.Name + " " + t3.ProductName
                                          }).Distinct().ToList();
            }


            return result;
        }

        public async Task<ResuisitionRM> GetGeneralResuisitionDataByRequisitionId(int ReqId, CancellationToken cancellationToken = default)
        {
            var result = await (
                 from t1 in context.Requisitions.Where(x => x.IsActive && x.RequisitionType == 3)
                     //join t2 in context.OrderDetails.Where(x => x.IsActive) on t1.OrderDetailsId equals t2.OrderDetailId
                     //join t3 in context.OrderMasters.Where(x => x.IsActive) on t2.OrderMasterId equals t3.OrderMasterId
                     //join t4 in context.Products.Where(x => x.IsActive) on t2.ProductId equals t4.ProductId
                     //join t5 in context.ProductSubCategories.Where(x => x.IsActive) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                     //join t6 in context.StockInfoes.Where(x => x.IsActive) on t1.FromRequisitionId equals t6.StockInfoId
                     //join t7 in context.StockInfoes.Where(x => x.IsActive) on t1.ToRequisitionId equals t7.StockInfoId

                 where t1.RequisitionId == ReqId
                 select new ResuisitionRM
                 {
                     RequisitionDate = t1.RequisitionDate,
                     FromRequisitionId = t1.FromRequisitionId.Value,
                     ToRequisitionId = t1.ToRequisitionId.Value,
                     OrderDetailsId = t1.OrderDetailsId,


                     //RequisitionId = t1.RequisitionId,
                     //RequisitionNo = t1.RequisitionNo,

                     //RequisitionStatus = t1.RequisitionStatus,
                     //OrderNo = t3.OrderNo,
                     //OrderDate = t3.OrderDate,
                     //JobOrderNo = t2.JobOrderNo,
                     //FromRequisitionName = t6.Name,
                     //ToRequisitionName = t7.Name,
                     //ProductName = t5.Name + " " + t4.ProductName,
                     //Description = t3.Remarks,
                     //Structure = t2.Remarks,
                     //Qty = t2.Qty,
                     //OrderMasterId = t3.OrderMasterId
                 }
             ).FirstOrDefaultAsync(cancellationToken);



            //if (result != null)
            //{
            //    result.DDLGerOrderList = (from t1 in context.OrderDetails.Where(x => x.OrderMasterId == result.OrderMasterId)
            //                              join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
            //                              join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
            //                              join t6 in context.FinishProductBOMs on t1.OrderDetailId equals t6.OrderDetailId
            //                              where t1.IsActive
            //                              select new SelectListItem
            //                              {
            //                                  Value = t1.OrderDetailId.ToString(),
            //                                  Text = t4.Name + " " + t3.ProductName
            //                              }).Distinct().ToList();
            //}


            return result;
        }




        public async Task<RResult> KPLRequisitionUpdate(ResuisitionRM model, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            var dbEntity = context.Requisitions.Find(model.RequisitionId);
            if (dbEntity != null)
            {
                dbEntity.RequisitionDate = model.RequisitionDate;
                dbEntity.FromRequisitionId = model.FromRequisitionId;
                dbEntity.ToRequisitionId = model.ToRequisitionId;
                dbEntity.CreatedBy = model.CreadedBy;
                dbEntity.OrderDetailsId = model.OrderDetailsId;
                context.Entry(dbEntity).State = EntityState.Modified;
                if (await context.SaveChangesAsync() > 0)
                {
                    rResult.result = 1;
                    rResult.message = "Data Updated Successfully!";
                }
                else
                {
                    rResult.result = 0;
                    rResult.message = "Error";
                }
            }
            else
            {
                rResult.result = 0;
                rResult.message = "Error";
            }

            return rResult;
        }

        public async Task<RResult> KPLRequisitionIssuedDelete(long issuedMasterId, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();

            if (issuedMasterId <= 0)
            {
                rResult.result = 0;
                rResult.message = "Invalid issuedMasterId";
                return rResult;
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var issuedMasterData = await context.IssueMasterInfoes
                        .FirstOrDefaultAsync(x => x.IssueMasterId == issuedMasterId, cancellationToken);

                    if (issuedMasterData == null)
                    {
                        rResult.result = 0;
                        rResult.message = "Issued Master Data not found";
                        return rResult;
                    }

                    var issuedDetailData = await context.IssueDetailInfoes
                        .Where(x => x.IssueMasterId == issuedMasterId)
                        .ToListAsync(cancellationToken);

                    issuedMasterData.IsActive = false;
                    issuedMasterData.ModifedBy = Common.GetUserId();
                    issuedMasterData.ModifiedDate = DateTime.Now;

                    foreach (var item in issuedDetailData)
                    {
                        item.IsActive = false;
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    transaction.Commit();

                    rResult.result = 1;
                    rResult.message = "Data deleted successfully!";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    rResult.result = 0;
                    rResult.message = $"Error: {ex.Message}";
                    // Optionally log the error message for debugging purposes.
                }
            }

            return rResult;
        }

        public async Task<RResult> KPLRequisitionIssueUpdate(VMPackagingPurchaseRequisition model, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            if (model != null)
            {
                var issuedMasterData = await context.IssueMasterInfoes
                        .FirstOrDefaultAsync(x => x.IssueMasterId == model.IssueMasterId, cancellationToken);
                if (issuedMasterData == null)
                {
                    rResult.result = 0;
                    rResult.message = "Issued Master Data not found";
                    return rResult;
                }

                issuedMasterData.IssueDate = model.IssueDate;
                issuedMasterData.IssuedBy = model.IssueById;
                issuedMasterData.FromDepartmentId = model.FromDepartmentIssueId;
                issuedMasterData.ModifedBy = model.ModifiedBy;
                issuedMasterData.ModifiedDate = DateTime.Now;

                await context.SaveChangesAsync(cancellationToken);

                rResult.result = 1;
                rResult.message = "Data Update successfully!";
            }
            return rResult;
        }

        public async Task<RResult> KPLRequisitionIssuedDetailsDelete(VMPackagingPurchaseRequisition model, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            var dbData = await context.IssueDetailInfoes.FirstOrDefaultAsync(x => x.IssueDetailId == model.IssueDetailsId, cancellationToken);
            if (dbData == null)
            {
                rResult.result = 0;
                rResult.message = "Data can't found!";
                return rResult;
            }
            dbData.IsActive = false;
            await context.SaveChangesAsync(cancellationToken);
            rResult.result = 0;
            rResult.message = "Data Delete Successfully!";

            return rResult;
        }


        public async Task<VMPackagingPurchaseRequisition> GetRequisitionForPKLRM(int companyId, int requisitionId)
        {
            VMPackagingPurchaseRequisition vMPackagingPurchaseRequisition = new VMPackagingPurchaseRequisition();
            vMPackagingPurchaseRequisition = await (from r in context.Requisitions.Where(x => x.IsActive == true && x.RequisitionId == requisitionId && x.CompanyId == companyId)
                                                    join c in context.Companies on r.CompanyId equals c.CompanyId
                                                    join s in context.StockInfoes on r.FromRequisitionId equals s.StockInfoId
                                                    join s1 in context.StockInfoes on r.ToRequisitionId equals s1.StockInfoId
                                                    select new VMPackagingPurchaseRequisition
                                                    {
                                                        IsSubmited = r.IsSubmitted,
                                                        RequisitionId = r.RequisitionId,
                                                        RequisitionNo = r.RequisitionNo,
                                                        CompanyName = c.Name,
                                                        CompanyFK = c.CompanyId,
                                                        CompanyId = c.CompanyId,
                                                        RequisitionDate = r.RequisitionDate.Value,
                                                        FromRequisitionName = s.Name,
                                                        ToRequisitionName = s1.Name
                                                    }).OrderByDescending(x => x.RequisitionId).FirstOrDefaultAsync();


            vMPackagingPurchaseRequisition.DataListPro = await (from d in context.RequisitionItemDetails
                                                                join p in context.Products on d.RProductId equals p.ProductId
                                                                join psc in context.ProductSubCategories on p.ProductSubCategoryId equals psc.ProductSubCategoryId
                                                                where d.RequisitionId == requisitionId && d.IsActive == true
                                                                select new VMPackagingPurchaseRequisition
                                                                {
                                                                    RQty = d.RQty.Value,
                                                                    ProductName = psc.Name + " " + p.ProductName,
                                                                    RProductId = d.RProductId.Value,
                                                                    RequistionItemDetailId = d.RequistionItemDetailId
                                                                }).ToListAsync();

            return vMPackagingPurchaseRequisition;
        }


        public async Task<int> RequisitionItemDetailDeleteConfirm(int RequisitionId, Guid? RequistionItemDetailId)
        {
            int result = 0;
            int requisitionId = 0;
            if (RequistionItemDetailId.HasValue)
            {
                result = await context.Database.ExecuteSqlCommandAsync("UPDATE Erp.RequisitionItemDetail SET IsActive = 0 WHERE RequistionItemDetailId = {0}", RequistionItemDetailId);
                if (result > 0)
                {
                    requisitionId = RequisitionId;
                }

            }
            return requisitionId;
        }

        public async Task<int> RequisitionItemDetailUpdate(int requisitionId, decimal RQuantity, int RproductionId, Guid? RequistionItemDetailId)
        {
            int result = 0;
            int requisition = 0;

            if (RequistionItemDetailId.HasValue)
            {
                result = await context.Database.ExecuteSqlCommandAsync(
                    "UPDATE Erp.RequisitionItemDetail SET RProductId = {0}, RQty = {1} WHERE RequistionItemDetailId = {2}",
                    RproductionId, RQuantity, RequistionItemDetailId.Value);
                if (result > 0)
                {
                    requisition = requisitionId;
                }
            }

            return requisition;
        }
        public int RequisitionItemDetailSave(int requisitionId, decimal RQuantity, int RProductId, int FinishProductBOMId)
        {
            int RequisitionId = 0;
            if (RQuantity > 0 && RProductId > 0 && FinishProductBOMId > 0)
            {
                int result = context.Database.ExecuteSqlCommand("EXEC spRequisitionItemDetailCreate {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                                0,
                                                requisitionId,
                                                RProductId,
                                                null,
                                                RQuantity,
                                                null,
                                                null,
                                                null,
                                                null,
                                                0,
                                                true,
                                                FinishProductBOMId);
                if (result > 0)
                {
                    RequisitionId = requisitionId;
                }
            }


            return RequisitionId;
        }
    }
}

