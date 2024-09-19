using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class MaterialReceiveService : IMaterialReceiveService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public MaterialReceiveService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<MaterialReceiveModel> GetMaterialReceivedList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            MaterialReceiveModel materialReceive = new MaterialReceiveModel();
            materialReceive.CompanyId = companyId;
            materialReceive.DataList = await Task.Run(() => (from t1 in context.MaterialReceives
                                                             join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                                             join t3 in context.Vendors on t2.SupplierId equals t3.VendorId
                                                             join t4 in context.StockInfoes on t1.StockInfoId equals t4.StockInfoId
                                                             where t1.CompanyId == companyId
                                                             && t1.ReceivedDate >= fromDate
                                                             && t1.ReceivedDate <= toDate
                                                             select new MaterialReceiveModel
                                                             {
                                                                 MaterialReceiveId = t1.MaterialReceiveId,
                                                                 ReceivedDate = t1.ReceivedDate,
                                                                 ReceiveNo = t1.ReceiveNo,
                                                                 PurchaseOrderNo = t2.PurchaseOrderNo,
                                                                 SupplierName = t3.Name,
                                                                 StoreName = t4.Name,
                                                                 CompanyId = t1.CompanyId,
                                                                 Remarks = t1.Remarks,
                                                                 MaterialReceiveStatus = t1.MaterialReceiveStatus,
                                                                 IsSubmitted = t1.IsSubmitted

                                                             }).OrderByDescending(o => o.MaterialReceiveId).AsEnumerable());

            return materialReceive;
        }
        public async Task<List<VMMaterialRcvList>> GetMaterialRcvList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            List<VMMaterialRcvList> list = new List<VMMaterialRcvList>();
            //list = await context.vwSeedMaterialRcvLists
            //    .Where(o => (fromDate.HasValue ? o.ChallanDate >= fromDate.Value : o.ChallanDate != null)
            //    && (toDate.HasValue ? o.ChallanDate <= toDate.Value : toDate != null))
            //    .ToListAsync();
            //return list;

            list = await Task.Run(() => (from t1 in context.MaterialReceives
                                         join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                         join t3 in context.Vendors on t2.SupplierId equals t3.VendorId

                                         where t1.CompanyId == companyId
                                         && t1.IsActive == true
                                         && t1.ChallanDate >= fromDate
                                         && t1.ChallanDate <= toDate
                                         && ((companyId == (int)CompanyName.KrishibidFeedLimited ? t1.MaterialReceiveStatus == "GPO" : 0 == 0))
                                         select new VMMaterialRcvList
                                         {

                                             MaterialReceiveId = t1.MaterialReceiveId,
                                             CompanyId = t1.CompanyId,
                                             PurchaseOrderId = t1.PurchaseOrderId.Value,
                                             MaterialType = t1.MaterialType,
                                             ReceiveNo = t1.ReceiveNo,
                                             ReceivedBy = t1.ReceivedBy,
                                             ReceivedDate = t1.ReceivedDate,
                                             TotalAmount = t1.TotalAmount,
                                             Discount = t1.Discount,
                                             ChallanDate = t1.ChallanDate,
                                             PurchaseOrderNo = t2.PurchaseOrderNo,
                                             SupplierName = t3.Name,
                                             IsSubmitted = t1.IsSubmitted,
                                             Remarks = t1.Remarks,
                                             MaterialReceiveStatus = t1.MaterialReceiveStatus,
                                             SupplierId = t3.VendorId,

                                         }).OrderByDescending(o => o.MaterialReceiveId).ToListAsync());
            return list;
        }




        public async Task<GCCLMaterialRecieveVm> GCCLMaterialRcvList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            GCCLMaterialRecieveVm models = new GCCLMaterialRecieveVm();
            models.DataList = await Task.Run(() => (from t1 in context.MaterialReceives
                                                    join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                                    join t3 in context.Vendors on t2.SupplierId equals t3.VendorId

                                                    where t1.CompanyId == companyId
                                                    && t1.ChallanDate >= fromDate
                                                    && t1.ChallanDate <= toDate
                                                    && ((companyId == (int)CompanyName.KrishibidFeedLimited ? t2.DemandId == null : 0 == 0))
                                                    select new GCCLMaterialRecieveVm
                                                    {
                                                        MaterialReceiveId = t1.MaterialReceiveId,
                                                        CompanyId = t1.CompanyId,
                                                        ReceiveNo = t1.ReceiveNo,
                                                        ReceivedDate = (DateTime)t1.ReceivedDate,
                                                        SupplierName = t3.Name,
                                                        PoNo = t2.PurchaseOrderNo,
                                                        PoDate = t2.PurchaseDate.Value,
                                                    }).OrderByDescending(o => o.MaterialReceiveId).ToListAsync());
            return models;
        }

        public async Task<KFMALMaterialRecieveVm> KFMALMaterialRcvList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            KFMALMaterialRecieveVm models = new KFMALMaterialRecieveVm();
            models.DataList = await Task.Run(() => (from t1 in context.MaterialReceives
                                                    join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                                    join t3 in context.Vendors on t2.SupplierId equals t3.VendorId

                                                    where t1.CompanyId == companyId
                                                    && t1.ChallanDate >= fromDate
                                                    && t1.ChallanDate <= toDate
                                                    && ((companyId == (int)CompanyName.KrishibidFeedLimited ? t2.DemandId == null : 0 == 0))
                                                    select new KFMALMaterialRecieveVm
                                                    {
                                                        IsSubmitted = t1.IsSubmitted,
                                                        SupplierPaymentMethodId = t2.SupplierPaymentMethodEnumFK,
                                                        MaterialReceiveId = t1.MaterialReceiveId,
                                                        CompanyId = t1.CompanyId,
                                                        ReceiveNo = t1.ReceiveNo,
                                                        ReceivedDate = (DateTime)t1.ReceivedDate,
                                                        SupplierName = t3.Name,
                                                        PoNo = t2.PurchaseOrderNo,
                                                        PoDate = t2.PurchaseDate.Value,
                                                    }).OrderByDescending(o => o.MaterialReceiveId).ToListAsync());
            return models;
        }

        public async Task<KFMALMaterialRecieveVm> PackagingMaterialRcvList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            KFMALMaterialRecieveVm models = new KFMALMaterialRecieveVm();
            models.DataList = await Task.Run(() => (from t1 in context.MaterialReceives
                                                    join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                                    join t3 in context.Vendors on t2.SupplierId equals t3.VendorId

                                                    where t1.CompanyId == companyId && t1.IsActive
                                                    && t1.ChallanDate >= fromDate
                                                    && t1.ChallanDate <= toDate
                                                    
                                                    select new KFMALMaterialRecieveVm
                                                    {
                                                        ChallanNo =t1.ChallanNo,
                                                        MaterialReceiveId = t1.MaterialReceiveId,
                                                        CompanyId = t1.CompanyId,
                                                        ReceiveNo = t1.ReceiveNo,
                                                        ReceivedDate = (DateTime)t1.ChallanDate,
                                                        SupplierName = t3.Name,
                                                        PoNo = t2.PurchaseOrderNo,
                                                        PoDate = t2.PurchaseDate.Value,
                                                        IsSubmitted=t1.IsSubmitted
                                                    }).OrderByDescending(o => o.MaterialReceiveId).ToListAsync());
            return models;
        }














        public async Task<MaterialReceiveModel> GetMaterialIssuePendingList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            MaterialReceiveModel materialReceive = new MaterialReceiveModel();
            materialReceive.CompanyId = companyId;
            materialReceive.DataList = await Task.Run(() => (from t1 in context.MaterialReceives
                                                             join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                                             join t3 in context.Vendors on t1.VendorId equals t3.VendorId
                                                             join t4 in context.StockInfoes on t1.StockInfoId equals t4.StockInfoId
                                                             where t1.CompanyId == companyId
                                                             && t1.ReceivedDate >= fromDate
                                                             && t1.ReceivedDate <= toDate
                                                             && t1.MaterialReceiveStatus == "OPEN"
                                                             select new MaterialReceiveModel
                                                             {
                                                                 MaterialReceiveId = t1.MaterialReceiveId,
                                                                 ReceivedDate = t1.ReceivedDate,
                                                                 ReceiveNo = t1.ReceiveNo,
                                                                 PurchaseOrderNo = t2.PurchaseOrderNo,
                                                                 SupplierName = t3.Name,
                                                                 StoreName = t4.Name,
                                                                 CompanyId = t1.CompanyId,
                                                                 Remarks = t1.Remarks,
                                                                 MaterialReceiveStatus = t1.MaterialReceiveStatus,
                                                                 IsSubmitted = t1.IsSubmitted
                                                             }).OrderByDescending(o => o.ReceivedDate).AsEnumerable());

            return materialReceive;
        }
        public List<MaterialReceiveModel> GetMaterialReceives(int companyId, string searchDate, string searchText, string type)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            List<MaterialReceiveModel> stores = context.Database.SqlQuery<MaterialReceiveModel>(@"EXEC GetMaterialReceiveList {0}", companyId).ToList();

            if (dateSearch == null)
            {
                return stores.Where(x => (x.ReceiveNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                    (x.PurchaseOrderNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.ReceivedDate).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return stores.Where(x => x.ReceivedDate.Value.Date == dateSearch.Value.Date).ToList();
            }


            return stores.Where(x => x.ReceivedDate.Value.Date == dateSearch.Value.Date &&
                                (x.ReceiveNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                (x.PurchaseOrderNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.ReceivedDate).ToList();
        }
        public VMWareHousePOReceivingSlave GetMaterialReceive(int companyId, long materialReceiveId)
        {
            VMWareHousePOReceivingSlave materialReceiveModel = new VMWareHousePOReceivingSlave();
            materialReceiveModel = (from t1 in context.MaterialReceives
                                    join t2 in context.PurchaseOrders on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                    join t3 in context.StockInfoes on t1.StockInfoId equals t3.StockInfoId
                                    join t4 in context.Vendors on t2.SupplierId equals t4.VendorId
                                    join t5 in context.Demands on t2.DemandId equals t5.DemandId
                                    join t6 in context.Employees on t1.ReceivedBy equals t6.Id
                                    join t8 in context.VoucherMaps.Where(X => X.CompanyId == companyId && X.IntegratedFrom == "MaterialReceive") on t1.MaterialReceiveId equals t8.IntegratedId into t8_Join
                                    from t8 in t8_Join.DefaultIfEmpty()
                                    where t1.IsActive && t1.CompanyId == companyId && t1.MaterialReceiveId == materialReceiveId
                                    select new VMWareHousePOReceivingSlave
                                    {
                                        VoucherId = t8 != null ? t8.VoucherId : 0,
                                        Factory = t3.Name,
                                        AllowLabourBill = t1.AllowLabourBill,
                                        MaterialReceiveId = t1.MaterialReceiveId,
                                        CompanyId = t1.CompanyId,
                                        ChallanDate = t1.ChallanDate,
                                        Challan = t1.ChallanNo,
                                        DemandDate = t5.DemandDate,
                                        DemandNo = t5.DemandNo,
                                        Discount = t1.Discount,
                                        DriverName = t1.DriverName,
                                        IsSubmitted = t1.IsSubmitted,
                                        LabourBill = t1.LabourBill,
                                        POCID = t2.PurchaseOrderNo,
                                        PODate = t2.PurchaseDate,
                                        Procurement_PurchaseOrderFk = t2.PurchaseOrderId,
                                        SupplierName = t4.Name,
                                        StoreName = t3.Name,
                                        ReceivedDate = t1.ReceivedDate.Value,
                                        ChallanCID = t1.ReceiveNo,
                                        TruckFare = t1.TruckFare,
                                        TruckNo = t1.TruckNo,
                                        EmployeeName = t6.Name,
                                        StockInfoId = t1.StockInfoId,
                                        MaterialReceiveStatus = t1.MaterialReceiveStatus,
                                        MaterialType = t1.MaterialType,
                                        UnloadingDate = t1.UnloadingDate,
                                        TDSDeduction = t2.TDSDeduction

                                    }).FirstOrDefault();

            materialReceiveModel.DataListSlave = (from t1 in context.MaterialReceiveDetails
                                                  join t2 in context.Products on t1.ProductId equals t2.ProductId
                                                  join t3 in context.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                                  join t4 in context.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                                  join t5 in context.Units on t2.UnitId equals t5.UnitId

                                                  join t6 in context.Bags on t1.BagId equals t6.BagId into t6Join
                                                  from t6 in t6Join.DefaultIfEmpty()



                                                  where t1.IsActive && t1.MaterialReceiveId == materialReceiveId
                                                  select new VMWareHousePOReceivingSlave
                                                  {
                                                      BagName = t6 != null ? t6.BagName : "",
                                                      ProductName = t2.ProductName,
                                                      ProductSubCategory = t3.Name,
                                                      ProductCategory = t4.Name,
                                                      UnitName = t5.Name,
                                                      BagId = t1.BagId,
                                                      BagQty = t1.BagQty,
                                                      BagWeight = t1.BagWeight,
                                                      Deduction = t1.Deduction,
                                                      MaterialReceiveDetailId = t1.MaterialReceiveDetailId,
                                                      MaterialReceiveId = t1.MaterialReceiveId.Value,
                                                      Common_ProductFk = t1.ProductId.Value,
                                                      ReceivedQuantity = t1.ReceiveQty,
                                                      PurchasingPrice = t1.UnitPrice,
                                                      StockInQty = t1.StockInQty.Value,
                                                      StockInRate = t1.StockInRate.Value

                                                  }).OrderByDescending(x => x.MaterialReceiveId).AsEnumerable();

            return materialReceiveModel;

            //decimal labourBill = (decimal)context.DropDownItems.Where(x => x.CompanyId == companyId && x.Name.Equals("Unloading Bill")).First().ItemValue;
            //string receivedNo = string.Empty;
            //if (id <= 0)
            //{
            //    IQueryable<MaterialReceive> storeReceives = context.MaterialReceives.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.ReceiveNo);
            //    int count = storeReceives.Count();
            //    if (count == 0)
            //    {
            //        return new MaterialReceiveModel()
            //        {
            //            ReceiveNo = GenerateMaterialReceiveNo(0),
            //            CompanyId = companyId,
            //            StockInfoId = 2,
            //            LabourBill = labourBill,
            //            AllowLabourBill = true
            //        };
            //    }

            //    storeReceives = storeReceives.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.ReceiveNo).Take(1);
            //    receivedNo = storeReceives.ToList().FirstOrDefault().ReceiveNo;
            //    long lastReceivedNo = Convert.ToInt64(receivedNo.Substring(3, 6));
            //    receivedNo = GenerateMaterialReceiveNo(lastReceivedNo);
            //    return new MaterialReceiveModel()
            //    {
            //        ReceiveNo = receivedNo,
            //        CompanyId = companyId,
            //        LabourBill = labourBill,
            //        AllowLabourBill = true
            //    };

            //}
            //MaterialReceive materialReceive = context.MaterialReceives.Include(x => x.MaterialReceiveDetails).Where(x => x.MaterialReceiveId == id).FirstOrDefault();
            //if (materialReceive == null)
            //{
            //    throw new Exception("Material Receive not found");
            //}
            //MaterialReceiveModel model = ObjectConverter<MaterialReceive, MaterialReceiveModel>.Convert(materialReceive);
            //return model;
        }

        public MaterialReceiveModel GetMaterialReceiveEdit(long id)
        {
            MaterialReceive materialReceive = context.MaterialReceives.Include(x => x.MaterialReceiveDetails).Where(x => x.MaterialReceiveId == id).FirstOrDefault();
            if (materialReceive == null)
            {
                throw new Exception("Material Receive not found");
            }
            MaterialReceiveModel model = ObjectConverter<MaterialReceive, MaterialReceiveModel>.Convert(materialReceive);

            List<MaterialReceiveDetailModel> materialReceiveDetails = context.Database.SqlQuery<MaterialReceiveDetailModel>("exec sp_Feed_MaterialReceiveItems {0},{1}", model.PurchaseOrderId, model.CompanyId).ToList();

            decimal toTalQty = model.MaterialReceiveDetails.Select(x => x.ReceiveQty).Sum();
            if (toTalQty > 0)
            {
                model.LabourBill = (model.LabourBill / toTalQty) * 100;
            }
            else
            {
                model.LabourBill = 0;
            }
            foreach (var detail in model.MaterialReceiveDetails)
            {
                Product rawMaterial = context.Products.Include(x => x.Unit).Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                Bag bag = context.Bags.Where(x => x.BagId == detail.BagId).FirstOrDefault();

                detail.ProductName = rawMaterial.ProductName;
                detail.UnitName = rawMaterial.Unit.Name;
                detail.BagName = bag.BagName;

                //detail.POQty = context.Database.SqlQuery<decimal>(@"select CAST(pod.PurchaseQty as DECIMAL(18,2)) as POQty
                //                                                    from Erp.PurchaseOrder po
                //                                                    inner join Erp.PurchaseOrderDetail pod on pod.PurchaseOrderId=po.PurchaseOrderId
                //                                                    where po.PurchaseOrderId={0}", model.PurchaseOrderId).FirstOrDefault();
                //detail.PurchasedQty = context.Database.SqlQuery<decimal>(@"select ISNULL(sum(ReceiveQty),0) as PurchasedQty
                //                                                           from   Erp.MaterialReceiveDetail 
                //                                                           where  MaterialReceiveId={0}", model.MaterialReceiveId).FirstOrDefault();
                detail.Amount = detail.ReceiveQty * detail.UnitPrice;

                detail.StockAmount = (decimal)(detail.StockInQty * detail.StockInRate);

            }
            PurchaseOrder purchaseOrder = context.PurchaseOrders.Include(x => x.Demand).Where(x => x.PurchaseOrderId == model.PurchaseOrderId).FirstOrDefault();
            model.SupplierName = context.Vendors.Where(x => x.VendorId == model.VendorId).FirstOrDefault().Name;
            model.DemandNo = purchaseOrder.Demand.DemandNo;
            model.DemandDate = purchaseOrder.Demand.DemandDate;
            model.PurchaseOrderDate = purchaseOrder.PurchaseDate;
            model.ReceiverName = context.Employees.Where(x => x.Id == model.ReceivedBy).Select(x => x.Name + " [" + x.EmployeeId + "]").FirstOrDefault();
            return model;
        }


        private string GenerateMaterialReceiveNo(long lastReceivedNo)
        {
            lastReceivedNo = lastReceivedNo + 1;
            return "RM-" + lastReceivedNo.ToString().PadLeft(6, '0');
        }

        public async Task<long> SaveMaterialReceive(VMWareHousePOReceivingSlave vmPOReceivingSlave)
        {
            long result = -0;
            #region Receive No 
            var lastMaterialReceiveNo = context.MaterialReceives.Where(x => x.CompanyId == vmPOReceivingSlave.CompanyId && x.MaterialReceiveStatus != "GPO")
                                       .OrderByDescending(x => x.MaterialReceiveId).Take(1).Select(x => x.ReceiveNo).FirstOrDefault();

            long lastReceivedNo = Convert.ToInt64(lastMaterialReceiveNo.Substring(3, 6)) + 1;
            string receivedNo = "RM-" + lastReceivedNo.ToString().PadLeft(6, '0');

            #endregion
            var MaterialReceiveDetailData = vmPOReceivingSlave.MaterialReceiveDetailModel.Where(x => x.ReceiveQty > 0).ToList();
            decimal totalQuantity = MaterialReceiveDetailData.Sum(x => x.ReceiveQty);


            MaterialReceive materialReceive = new MaterialReceive
            {
                ReceivedDate = vmPOReceivingSlave.ReceivedDate,
                ReceiveNo = receivedNo,
                ReceivedBy = Common.GetIntUserId(),
                Remarks = vmPOReceivingSlave.Remarks,
                AllowLabourBill = vmPOReceivingSlave.AllowLabourBill,
                ChallanDate = vmPOReceivingSlave.ChallanDate,
                ChallanNo = vmPOReceivingSlave.Challan,
                CompanyId = vmPOReceivingSlave.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Discount = vmPOReceivingSlave.Discount,
                DriverName = vmPOReceivingSlave.DriverName,
                IsActive = true,
                LabourBill = vmPOReceivingSlave.AllowLabourBill ? totalQuantity * (vmPOReceivingSlave.LabourBill / 100) : 0,
                MaterialReceiveStatus = "OPEN",

                MaterialType = "R",
                PurchaseOrderId = vmPOReceivingSlave.Procurement_PurchaseOrderFk,
                StockInfoId = vmPOReceivingSlave.StockInfoId,
                TruckFare = vmPOReceivingSlave.TruckFare,
                TruckNo = vmPOReceivingSlave.TruckNo,
                UnloadingDate = vmPOReceivingSlave.ReceivedDate,
                VendorId = vmPOReceivingSlave.Common_SupplierFK

            };
            context.MaterialReceives.Add(materialReceive);
            if (await context.SaveChangesAsync() > 0)
            {
                result = materialReceive.MaterialReceiveId;
            }
            if (result > 0 && MaterialReceiveDetailData.Any())
            {
                List<MaterialReceiveDetail> materialReceiveDetailsList = new List<MaterialReceiveDetail>();
                List<VMRawMaterialStock> vmRawMaterialStockList = new List<VMRawMaterialStock>();
                foreach (var item in MaterialReceiveDetailData)
                {
                    MaterialReceiveDetail materialReceiveDetail = new MaterialReceiveDetail
                    {
                        MaterialReceiveId = result,
                        BagId = item.BagId,
                        BagQty = item.BagQty,
                        BagWeight = item.BagWeight,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        Deduction = item.Deduction,
                        IsActive = true,
                        ProductId = item.ProductId,
                        PurchaseOrderDetailFk = Convert.ToInt32(item.PurchaseOrderDetailFk),
                        ReceiveQty = item.ReceiveQty,
                        StockInQty = item.StockInQty,
                        StockInRate = item.StockInRate,
                        UnitPrice = item.UnitPrice,
                    };
                    materialReceiveDetailsList.Add(materialReceiveDetail);

                }
                context.MaterialReceiveDetails.AddRange(materialReceiveDetailsList);
                if (await context.SaveChangesAsync() > 0)
                {
                    UpdateProductCostingPrice(materialReceive.CompanyId, MaterialReceiveDetailData);

                }
            }
            return result;
            //List<int> temp = new List<int>();
            //foreach (var item in vmPOReceivingSlave.MaterialReceiveDetailModel.ToList())
            //{
            //    if (item.BagId <= 0 || item.StockAmount <= 0 || item.BagQty <= 0)
            //    {
            //        Removing MaterialReceiveDetails if it is not validated or user doesn't calculate this line from UI
            //        model.MaterialReceiveDetails.Remove(item);
            //    }
            //    else
            //    {
            //        if (item.POQty > item.PurchasedQty + item.ReceiveQty) { temp.Add(2); }
            //        else { temp.Add(3); }
            //    }

            //    item.CreatedDate = DateTime.Now;
            //    item.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;


            //}

            //decimal toTalQty = model.MaterialReceiveDetails.Select(x => x.ReceiveQty).Sum();

            //MaterialReceive materialReceive = ObjectConverter<MaterialReceiveModel, MaterialReceive>.Convert(model);

            //PurchaseOrder purchaseOrder = context.PurchaseOrders.Where(x => x.PurchaseOrderId == model.PurchaseOrderId).FirstOrDefault();
            //purchaseOrder.CompletionStatus = temp.Where(e => e == 2).ToList().Count() > 0 ? (int)EnumPOCompletionStatus.Partially_Complete : (int)EnumPOCompletionStatus.Complete;
            //if (id > 0)
            //{
            //    materialReceive = context.MaterialReceives.Where(x => x.MaterialReceiveId == id).FirstOrDefault();
            //    if (materialReceive == null)
            //    {
            //        throw new Exception("Data not found!");
            //    }
            //    materialReceive.MaterialReceiveStatus = model.MaterialReceiveStatus;
            //    materialReceive.ModifiedDate = DateTime.Now;
            //    materialReceive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            //}

            //else
            //{
            //    materialReceive.MaterialReceiveStatus = "OPEN";
            //    materialReceive.IsActive = true;
            //    materialReceive.CreatedDate = DateTime.Now;
            //    materialReceive.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            //}
            //materialReceive.AllowLabourBill = model.AllowLabourBill;
            //materialReceive.LabourBill = model.AllowLabourBill ? toTalQty * (model.LabourBill / 100) : 0;
            //materialReceive.ReceivedDate = model.ReceivedDate;
            //materialReceive.PurchaseOrder = null;
            //context.MaterialReceives.Add(materialReceive);

            //context.Entry(materialReceive).State = materialReceive.MaterialReceiveId == 0 ? EntityState.Added : EntityState.Modified;

            //try
            //{
            //    noOfRowsAffected = context.SaveChanges();
            //    if (noOfRowsAffected > 0)
            //    {


            //        context.Database.ExecuteSqlCommand("EXEC UpdateProductStoreRawMaterialsInventory {0},{1}", materialReceive.CompanyId, materialReceive.ReceiveNo);
            //    }
            //}

            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    return new MaterialMsgModel { Status = noOfRowsAffected > 0, Id = materialReceive.MaterialReceiveId };

            //}
            //return new MaterialMsgModel { Status = noOfRowsAffected > 0, Id = materialReceive.MaterialReceiveId };
            //return null;
        }

        private void UpdateProductCostingPrice(int companyId, List<MaterialReceiveDetailModel> materialReceiveDetailData)
        {
            foreach (var item in materialReceiveDetailData)
            {
                Product product = context.Products.Find(item.ProductId);
                var priviousStockHistory = context.Database.SqlQuery<VMRawMaterialStock>("exec FeedRawMaterialsStockByProductId {0}, {1}", companyId, item.ProductId).FirstOrDefault();

                if ((priviousStockHistory.ClosingQty + item.StockInQty.Value) > 0)
                {
                    decimal priviousClosingAmount = priviousStockHistory.ClosingQty * priviousStockHistory.ClosingRate;
                    decimal currentStockinAmount = item.StockInQty.Value * item.StockInRate.Value;
                    decimal totalStockQuantity = priviousStockHistory.ClosingQty + item.StockInQty.Value;
                    product.CostingPrice = (priviousClosingAmount + currentStockinAmount) / totalStockQuantity;

                    context.SaveChanges();

                }

            }
        }

        public List<MaterialReceiveModel> GetMaterialIssuePendingList(int companyId, string searchDate, string searchText)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            List<MaterialReceiveModel> stores = context.Database
                .SqlQuery<MaterialReceiveModel>(@"select MaterialReceiveId,ReceiveNo,ReceivedDate,
                                                  (select Name from Erp.Vendor where VendorId=Erp.MaterialReceive.VendorId)
as SupplierName, 
 (select Name from Erp.StockInfo where StockInfoId=Erp.MaterialReceive.StockInfoId) as StoreName,
  MaterialReceiveStatus
 from Erp.MaterialReceive
   where MaterialReceiveStatus='OPEN' and CompanyId={0}", companyId).ToList();

            if (dateSearch == null)
            {
                return stores.Where(x => (x.ReceiveNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                    (x.PurchaseOrderNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.ReceivedDate).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return stores.Where(x => x.ReceivedDate.Value.Date == dateSearch.Value.Date).ToList();
            }


            return stores.Where(x => x.ReceivedDate.Value.Date == dateSearch.Value.Date &&
                                (x.ReceiveNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                (x.PurchaseOrderNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.ReceivedDate).ToList();
        }

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

        public bool MaterialReceiveIssue(MaterialReceiveModel model)
        {
            string user = System.Web.HttpContext.Current.User.Identity.Name;
            MaterialReceive materialReceive = context.MaterialReceives.Where(x => x.MaterialReceiveId == model.MaterialReceiveId).FirstOrDefault();
            if (materialReceive == null)
            {
                throw new Exception("Data not found!");
            }

            materialReceive.ModifiedDate = DateTime.Now;
            materialReceive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            int noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                //----------Insert date from Material Receive to Store & StoreDetail
                noOfRowsAffected = context.Database.ExecuteSqlCommand("exec sp_Feed_RawMaterialStockInAfterIssue {0},{1}", materialReceive.MaterialReceiveId, user);
                if (noOfRowsAffected > 0)
                {
                    noOfRowsAffected = context.Database.ExecuteSqlCommand("update Erp.MaterialReceive set MaterialReceiveStatus='ISSUE' where MaterialReceiveId={0}", materialReceive.MaterialReceiveId);
                }


            }
            return noOfRowsAffected > 0;
        }

        public MaterialReceiveModel GetMaterialReceiveIssue(long id)
        {
            return context.Database.SqlQuery<MaterialReceiveModel>("exec sp_Feed_MaterialReceiveIssue {0}", id).FirstOrDefault();
        }

        public IList<MaterialReceiveDetailModel> GetMaterialReceiveDetailIssue(long id)
        {
            return context.Database.SqlQuery<MaterialReceiveDetailModel>(@"exec sp_Feed_MRRreport {0}", id).ToList();
        }

        public bool MaterialIssueCancel(VMWareHousePOReceivingSlave VMReceivingSlave)
        {
            MaterialReceive materialReceive = context.MaterialReceives.Find(VMReceivingSlave.MaterialReceiveId);
            if (materialReceive == null)
            {
                return false;
            }
            materialReceive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            materialReceive.ModifiedDate = DateTime.Now;
            materialReceive.Remarks = VMReceivingSlave.Remarks;
            materialReceive.MaterialReceiveStatus = "CANCEL";
            return context.SaveChanges() > 0;
        }

        public bool MaterialReceiveEdit(MaterialReceiveModel model)
        {
            /**MaterialReceive Update*/

            MaterialReceive materialReceive = context.MaterialReceives.Where(x => x.MaterialReceiveId == model.MaterialReceiveId).FirstOrDefault();

            materialReceive.CompanyId = model.CompanyId;
            materialReceive.PurchaseOrderId = model.PurchaseOrderId;
            materialReceive.MaterialType = model.MaterialType;
            materialReceive.ReceiveNo = model.ReceiveNo;
            materialReceive.StockInfoId = model.StockInfoId;
            materialReceive.VendorId = model.VendorId;
            materialReceive.TotalAmount = model.TotalAmount;
            materialReceive.Discount = model.Discount;
            materialReceive.ChallanNo = model.ChallanNo;
            materialReceive.ChallanDate = model.ChallanDate;
            materialReceive.UnloadingDate = model.UnloadingDate;
            materialReceive.TruckNo = model.TruckNo;
            materialReceive.DriverName = model.DriverName;
            materialReceive.TruckFare = model.TruckFare;

            decimal toTalQty = model.MaterialReceiveDetails.Select(x => x.ReceiveQty).Sum();
            materialReceive.LabourBill = model.AllowLabourBill ? toTalQty * (model.LabourBill / 100) : 0;

            materialReceive.Remarks = model.Remarks;
            materialReceive.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            materialReceive.ModifiedDate = DateTime.Now;
            materialReceive.MaterialReceiveStatus = "OPEN";

            context.Entry(materialReceive).State = materialReceive.MaterialReceiveId == 0 ? EntityState.Added : EntityState.Modified;
            int noOfRowsAfffected = context.SaveChanges();


            /**MaterialReceiveDetail Update*/
            if (noOfRowsAfffected > 0)
            {
                foreach (var detail in model.MaterialReceiveDetails)
                {
                    // MaterialReceiveDetail materialReceiveDetail = ObjectConverter<MaterialReceiveDetailModel, MaterialReceiveDetail>.Convert(detail);
                    MaterialReceiveDetail materialReceiveDetail = context.MaterialReceiveDetails.Where(x => x.MaterialReceiveDetailId == detail.MaterialReceiveDetailId).FirstOrDefault();

                    materialReceiveDetail.ProductId = detail.ProductId;
                    materialReceiveDetail.ReceiveQty = detail.ReceiveQty;
                    materialReceiveDetail.UnitPrice = detail.UnitPrice;
                    materialReceiveDetail.Deduction = detail.Deduction;
                    materialReceiveDetail.StockInQty = detail.StockInQty;
                    materialReceiveDetail.StockInRate = detail.StockInRate;
                    materialReceiveDetail.BagId = detail.BagId;
                    materialReceiveDetail.BagWeight = detail.BagWeight;
                    materialReceiveDetail.BagQty = detail.BagQty;

                    context.Entry(materialReceiveDetail).State = materialReceiveDetail.MaterialReceiveDetailId == 0 ? EntityState.Added : EntityState.Modified;
                    noOfRowsAfffected = context.SaveChanges();
                }
            }
            return noOfRowsAfffected > 0;
        }

        public Task<long> SubmitMaterialIssue(MaterialReceiveModel model)
        {
            throw new NotImplementedException();
        }
    }
}
