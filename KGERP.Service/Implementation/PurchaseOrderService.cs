using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public PurchaseOrderService(ERPEntities context)
        {
            this.context = context;
        }

        public async Task<PurchaseOrderModel> GetPurchaseOrders(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();
            purchaseOrderModel.CompanyId = companyId;
            purchaseOrderModel.DataList = await Task.Run(()=> (from t1 in context.PurchaseOrders
                                           join t2 in context.Demands on t1.DemandId equals t2.DemandId
                                           join t3 in context.Vendors on t1.SupplierId equals t3.VendorId
                                           where t1.CompanyId == companyId
                                           && t1.PurchaseDate >= fromDate && t1.PurchaseDate <= toDate
                                           select new PurchaseOrderModel
                                           {
                                               CompanyId = t1.CompanyId,
                                               PurchaseOrderId = t1.PurchaseOrderId,
                                               PurchaseDate = t1.PurchaseDate,
                                               PurchaseOrderNo = t1.PurchaseOrderNo,
                                               DemandNo = t2.DemandNo,
                                               SupplierName = t3.Name,
                                               Remarks = t1.Remarks
                                           }).OrderByDescending(x => x.PurchaseOrderId));

            return purchaseOrderModel;
        }

        private string GenerateSequenceNumber(long lastReceivedNo)
        {
            string input = string.Empty;
            long num = ++lastReceivedNo;
            input = num.ToString();
            if (input != string.Empty)
            {
                num = Convert.ToInt32(input);
            }
            return num.ToString().PadLeft(6, '0');
        }





        public async Task<PurchaseOrderModel> GetPurchaseOrder(int companyId, long purchaseOrdersId)
        {
            
            string purchaseOrderNo = string.Empty;
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();

            if (purchaseOrdersId <= 0)
            {
                IQueryable<PurchaseOrder> purchaseOrders = context.PurchaseOrders.Where(x => x.CompanyId == companyId);
                int count = purchaseOrders.Count();
                if (count == 0)
                {
                    return new PurchaseOrderModel()
                    {
                        PurchaseOrderNo = GenerateSequenceNumber(0),
                        CompanyId = companyId,
                        IsActive = true
                    };
                }

                purchaseOrders = purchaseOrders
                    .Where(x => x.CompanyId == companyId)
                    .OrderByDescending(x => x.PurchaseOrderId)
                    .Take(1);
                purchaseOrderNo = purchaseOrders.ToList().FirstOrDefault().PurchaseOrderNo;
                string numberPart = purchaseOrderNo.Substring(3, 4);
                int lastNumberPart = Convert.ToInt32(numberPart);
                purchaseOrderNo = GenerateSequenceNumber(lastNumberPart);
                purchaseOrderModel.PurchaseOrderNo = purchaseOrderNo;
                purchaseOrderModel.CompanyId = companyId;
                purchaseOrderModel.IsActive = true;


                return purchaseOrderModel;
            }
            else
            {
               
                purchaseOrderModel = await Task.Run(() => (from t1 in context.PurchaseOrders
                                                           join t2 in context.Demands on t1.DemandId equals t2.DemandId
                                                           join t3 in context.Vendors on t1.SupplierId equals t3.VendorId
                                                           join t4 in context.Employees on t1.EmpId equals t4.Id
                                                           join t5 in context.DropDownItems on t1.ModeOfPurchaseId equals t5.DropDownItemId
                                                           join t6 in context.DropDownItems on t1.ProductOriginId equals t6.DropDownItemId
                                                           join t7 in context.Countries on t1.CountryId equals t7.CountryId into t7_country
                                                           from t7 in t7_country.DefaultIfEmpty()
                                                           
                                                           where t1.PurchaseOrderId == purchaseOrdersId
                                                           select new PurchaseOrderModel
                                                           {
                                                               PurchaseOrderId = t1.PurchaseOrderId,
                                                               CompanyId= t1.CompanyId,
                                                               Remarks=t1.Remarks,
                                                               PurchaseOrderNo= t1.PurchaseOrderNo,
                                                               PurchaseDate= t1.PurchaseDate,
                                                               EmployeeName= t4.Name,
                                                               DemandNo= t2.DemandNo,
                                                               ModeOfPurchase= t5.Name,
                                                               ProductOrigin= t6.Name,
                                                               LCNo= t1.LCNo,
                                                               CompanyName= t1.CompanyName,
                                                               DeliveryDate=t1.DeliveryDate,
                                                               CountryName= t7.CountryName,
                                                               IsActive= t1.IsActive

                                                           }).FirstOrDefaultAsync());

                purchaseOrderModel.ItemList = await Task.Run(()=>( from t1 in context.PurchaseOrderDetails
                                                                  
                                                                   join t2 in context.Products on t1.ProductId equals t2.ProductId
                                                                   join t3 in context.Units on t2.UnitId equals t3.UnitId
                                                                 
                                                                   where t1.PurchaseOrderId == purchaseOrdersId
                                                                   select new PurchaseOrderDetailModel
                                                                   {
                                                                       PurchaseOrderId= t1.PurchaseOrderId,
                                                                       PurchaseOrderDetailId= t1.PurchaseOrderDetailId,
                                                                       RawMaterial= t2.ProductName,
                                                                       ProductId = t1.ProductId,
                                                                       UnitName= t3.Name,
                                                                       UnitId= t2.UnitId,
                                                                       PresentStock= 0,
                                                                       PurchaseRate= t1.PurchaseRate,
                                                                       DemandRate= t1.DemandRate,
                                                                       PurchaseQty= t1.PurchaseQty,
                                                                       PackSize= t1.PackSize, 
                                                                       Amount= t1.PurchaseAmount,
                                                                     
                                                                       IsActive= t1.IsActive
                                                                       
                                                                   }).ToListAsync());

                purchaseOrderModel.TermAndCondition = await Task.Run(() => context.POTermsAndConditionMappings.Where(t1 => t1.IsActive && t1.PurchaseOrderId == purchaseOrdersId).Select(x=> x.TermAndCondition).FirstOrDefault());
            }
                   return purchaseOrderModel;
        }

        private string GenerateSequenceNumber(int lastDemandNo)
        {
            int num = ++lastDemandNo;
            return "PO-" + num.ToString().PadLeft(4, '0');
        }
        public long SavePurchaseOrder(long id, PurchaseOrderModel model)
        {            
            int noOfRowsAffected = 0;
            PurchaseOrder purchaseOrder = ObjectConverter<PurchaseOrderModel, PurchaseOrder>.Convert(model);
            if (id > 0)
            {
                purchaseOrder = context.PurchaseOrders.Where(x => x.PurchaseOrderId == id).FirstOrDefault();
                if (purchaseOrder == null)
                {
                    throw new Exception("Data data not found!");
                }
                purchaseOrder.PurchaseOrderStatus = model.PurchaseOrderStatus;
                purchaseOrder.ModifiedDate = DateTime.Now;
                purchaseOrder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                purchaseOrder.ReferenceNo = Guid.NewGuid();
                purchaseOrder.PurchaseOrderStatus = "OPEN";
                purchaseOrder.CreatedDate = DateTime.Now;
                purchaseOrder.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            purchaseOrder.ModeOfPurchaseId = model.ModeOfPurchaseId;
            purchaseOrder.Days = model.Days;
            purchaseOrder.Remarks = model.Remarks;
            purchaseOrder.CountryId = model.CountryId;
            purchaseOrder.ProductOriginId = model.ProductOriginId;
            purchaseOrder.CompanyName = model.CompanyName;
            purchaseOrder.EmpId = model.EmpId;
            purchaseOrder.DeliveryDate = model.DeliveryDate;
            purchaseOrder.IsActive = model.IsActive;
            context.PurchaseOrders.Add(purchaseOrder);
            context.Entry(purchaseOrder).State = purchaseOrder.PurchaseOrderId == 0 ? EntityState.Added : EntityState.Modified;

            bool isNew = purchaseOrder.PurchaseOrderId == 0?true:false;

            try
            {
                noOfRowsAffected = context.SaveChanges();

                if (noOfRowsAffected > 0 )
                {
                    if (isNew)
                    {
                        POTermsAndConditionMapping pOTermsAndConditionMapping = new POTermsAndConditionMapping()
                        {
                            IsActive = true,
                            CreatedBy = Common.GetUserId(),
                            CreatedDate = DateTime.Now,
                            PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                            TermAndCondition = model.TermAndCondition
                        };
                        context.POTermsAndConditionMappings.Add(pOTermsAndConditionMapping);
                        context.SaveChanges();
                    }
                    else
                    {
                        var existTermNCondition = context.POTermsAndConditionMappings.FirstOrDefault(x=> x.PurchaseOrderId ==  purchaseOrder.PurchaseOrderId);
                        if (existTermNCondition != null)
                        {
                            existTermNCondition.TermAndCondition = purchaseOrder.TermsAndCondition;
                            context.SaveChanges();
                        }
                    }
                }
            }

            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            if (noOfRowsAffected > 0)
            {
                //Insert data to PurchaseOrderTemplete Filed
                context.Database.ExecuteSqlCommand("exec spInsertToPurchaseOrderTemplete {0}", purchaseOrder.PurchaseOrderId);
            }

            return noOfRowsAffected > 0? purchaseOrder.PurchaseOrderId:0;
        }
        public List<SoreProductQty> GetStoreProductQty()
        {
            dynamic result = context.Database.SqlQuery<SoreProductQty>("sp_GetStoreProductQuantity ").ToList();
            return result;
        }

        public List<PurchaseOrderDetailModel> GetPurchaseOrderDetails(long demandId, int companyId)
        {
            return context.Database.SqlQuery<PurchaseOrderDetailModel>("spGetPurchaseOrderItems {0},{1}", demandId, companyId).ToList();
        }


        public PurchaseOrderModel GetPurchaseOrderWithInclude(int purchaseOrderId)
        {

            PurchaseOrder purchaseOrder = context.PurchaseOrders.Include(x => x.Demand).Include(x => x.Vendor).Where(x => x.PurchaseOrderId == purchaseOrderId).FirstOrDefault();
            PurchaseOrderModel model = ObjectConverter<PurchaseOrder, PurchaseOrderModel>.Convert(purchaseOrder);
            return model;
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

        public List<PurchaseOrderModel> GetQCPurchaseOrders(int companyId, DateTime? searchDate, string searchText)
        {
            IQueryable<PurchaseOrder> queryable = context.PurchaseOrders.Include(x => x.Demand).Include(x => x.Vendor).Where(x => x.CompanyId == companyId && x.PurchaseOrderStatus.Equals("N") &&
                (x.PurchaseDate <= searchDate ||
                x.PurchaseOrderNo.ToLower().Contains(searchText.ToLower()) ||
                x.Vendor.Name.ToLower().Contains(searchText.ToLower()) ||
                x.Demand.DemandNo.ToLower().Contains(searchText.ToLower())
                )).OrderByDescending(x => x.PurchaseDate);
            return ObjectConverter<PurchaseOrder, PurchaseOrderModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<PurchaseOrderDetailModel> GetPurchaseOrderItems(long purchaseOrderId)
        {
            return context.Database.SqlQuery<PurchaseOrderDetailModel>("exec sp_Feed_PurchaseOrderItems {0}", purchaseOrderId).ToList();
        }

        public PurchaseOrderDetailModel GetPurchaseOrderItemInfo(long demandId, int productId)
        {
            return context.Database.SqlQuery<PurchaseOrderDetailModel>("exec sp_Feed_PurchaseOrderItemInfo {0},{1}", demandId, productId).FirstOrDefault();
        }

        public List<StoreDetailModel> GetQCItemList(long purchaseOrderId, int companyId)
        {
            throw new NotImplementedException();
        }



        public List<MaterialReceiveDetailModel> GetPurchaseOrderItems(long purchaseOrderId, int companyId)
        {
            return context.Database.SqlQuery<MaterialReceiveDetailModel>("exec sp_Feed_MaterialReceiveItems {0},{1}", purchaseOrderId, companyId).ToList();
        }

        public string GetPurchaseOrderTemplateReportName(long purchaseOrderId)
        {
            return context.Database.SqlQuery<string>(@"spPurchaseOrderReportName {0}", purchaseOrderId).FirstOrDefault();
        }

        public bool DeletePurchaseOrder(long purchaseOrderId, out string message)
        {
            message = string.Empty;
            bool existInTransactions = context.MaterialReceives.Any(x => x.PurchaseOrderId == purchaseOrderId);
            if (existInTransactions)
            {
                message = "Can not Detele. This Purchase Order is used in transaction";
                return !existInTransactions;
            }
            PurchaseOrder purchaseOrder = context.PurchaseOrders.Include(x => x.PurchaseOrderDetails).Where(x => x.PurchaseOrderId == purchaseOrderId).First();
            if (purchaseOrder == null)
            {
                return false;
            }
            if (purchaseOrder.PurchaseOrderDetails.Any())
            {
                context.PurchaseOrderDetails.RemoveRange(purchaseOrder.PurchaseOrderDetails);
                context.SaveChanges();
            }
            context.PurchaseOrders.Remove(purchaseOrder);
            if (context.SaveChanges() > 0)
            {
                //delete from Erp.PurchaseOrderTemplate
                context.Database.ExecuteSqlCommand("delete from Erp.PurchaseOrderTemplate where PurchaseOrderId={0}", purchaseOrder.PurchaseOrderId);
            }
            return context.SaveChanges() > 0;
        }

        public bool CancelPurchaseOrder(long purchaseOrderId, PurchaseOrderModel model)
        {
            PurchaseOrder purchaseOrder = context.PurchaseOrders.Where(x => x.PurchaseOrderId == purchaseOrderId).First();
            if (purchaseOrder == null)
            {
                return false;
            }
            purchaseOrder.Remarks = model.Remarks;
            purchaseOrder.PurchaseOrderStatus = model.PurchaseOrderStatus;
            return context.SaveChanges() > 0;
        }

      
       
        public List<SelectModel> GetOpenedPurchaseByVendor(int vendorId)
        {

            var list= context.PurchaseOrders.
                Where(x => x.SupplierId == vendorId &&
                    x.PurchaseOrderStatus.Equals("OPEN") &&
                    (x.CompletionStatus != (int)EnumPOCompletionStatus.Complete))
                
                .ToList();
            return list.Select(x => new SelectModel { Text = x.PurchaseOrderNo, Value = x.PurchaseOrderId }).ToList();
        }
    }
}
