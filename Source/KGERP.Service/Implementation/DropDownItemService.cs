using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class DropDownItemService : IDropDownItemService
    {
        private readonly ERPEntities context;
        public DropDownItemService(ERPEntities context)
        {
            this.context = context;
        }



        public List<DropDownItemModel> GetDropDownItems(string searchText)
        {
            IQueryable<DropDownItem> queryable = context.DropDownItems.Include(x => x.DropDownType).Where(x => (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
                                                                                                               (x.DropDownType.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).OrderBy(x => x.Name);
            return ObjectConverter<DropDownItem, DropDownItemModel>.ConvertList(queryable.ToList()).ToList();
        }

        public DropDownItemModel GetDropDownItem(int id)
        {
            if (id <= 0)
            {
                return new DropDownItemModel()
                {
                    CompanyId = (int)System.Web.HttpContext.Current.Session["CompanyId"],
                    IsActive = true
                };
            }
            DropDownItem dropDownItem = context.DropDownItems.FirstOrDefault(x => x.DropDownItemId == id);

            return ObjectConverter<DropDownItem, DropDownItemModel>.Convert(dropDownItem);
        }

        public List<SelectModel> GetDropDownItemSelectModels(int id)
        {
            if (id == 16)
            {
                return context.DropDownItems.ToList().Where(x => x.DropDownTypeId == id && x.IsActive).OrderBy(x => x.OrderNo).Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.DropDownItemId
                }).ToList();

            }
            else
            {
                return context.DropDownItems.ToList().Where(x => x.DropDownTypeId == id && x.IsActive).OrderBy(x => x.Name).Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.DropDownItemId
                }).ToList();
            }
        }

        public bool SaveDropDownItem(int id, DropDownItemModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Dropdown data missing!");
            }

            bool exist = context.DropDownItems.Where(x => x.DropDownTypeId == model.DropDownTypeId && x.Name.ToLower().Equals(model.Name.ToLower()) && x.DropDownItemId != id).Any();

            if (exist)
            {
                message = "Dropdown Item data already exist";
                return false;
            }
            DropDownItem dropDownItem = ObjectConverter<DropDownItemModel, DropDownItem>.Convert(model);
            if (id > 0)
            {
                dropDownItem = context.DropDownItems.FirstOrDefault(x => x.DropDownItemId == id);
                if (dropDownItem == null)
                {
                    throw new Exception("Dropdown Item not found!");
                }
                dropDownItem.ModifiedDate = DateTime.Now;
                dropDownItem.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                dropDownItem.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                dropDownItem.CreatedDate = DateTime.Now;

            }

            dropDownItem.DropDownTypeId = model.DropDownTypeId;
            dropDownItem.CompanyId = model.CompanyId;
            dropDownItem.ItemValue = model.ItemValue;
            dropDownItem.Name = model.Name;
            dropDownItem.Remarks = model.Remarks;
            dropDownItem.OrderNo = model.OrderNo;
            dropDownItem.IsActive = model.IsActive;
            context.Entry(dropDownItem).State = dropDownItem.DropDownItemId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteDropDownItem(int id)
        {
            DropDownItem dropDownItem = context.DropDownItems.Find(id);
            if (dropDownItem == null)
            {
                throw new Exception("Data not found");
            }
            context.DropDownItems.Remove(dropDownItem);
            return context.SaveChanges() > 0;
        }
        public async Task<DropDownTypeModel> GetDropDownTypes(int companyId)
        {
            DropDownTypeModel dropDownTypeModel = new DropDownTypeModel();
            dropDownTypeModel.CompanyId = companyId;
            dropDownTypeModel.DataList = await Task.Run(() => (from t1 in context.DropDownTypes
                                                               where t1.CompanyId == companyId
                                                               select new DropDownTypeModel
                                                               {
                                                                   DropDownTypeId = t1.DropDownTypeId,
                                                                   Name = t1.Name,
                                                                   Remarks = t1.Remarks
                                                               }).OrderBy(o => o.Name).AsEnumerable());
            return dropDownTypeModel;
        }
        public async Task<DropDownItemModel> GetDropDownItems(int companyId)
        {
            DropDownItemModel dropDownItemModel = new DropDownItemModel();
            dropDownItemModel.CompanyId = companyId;
            dropDownItemModel.DataList = await Task.Run(() =>(from t1 in context.DropDownItems
                                                              join t2 in context.DropDownTypes on t1.DropDownTypeId equals t2.DropDownTypeId
                                                              where t1.CompanyId == companyId
                                                              select new DropDownItemModel
                                                              {
                                                                  DropDownItemId = t1.DropDownItemId,
                                                                  TypeName = t2.Name,
                                                                  Name= t1.Name,
                                                                  ItemValue= t1.ItemValue ??0,
                                                                  Remarks= t1.Remarks,
                                                                  OrderNo= t1.OrderNo
                                                              }).AsEnumerable());

            return dropDownItemModel;
        }
        public List<SelectModel> GetRegionSelectModels()
        {
            return context.Districts.ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.DistrictId.ToString()
            }).ToList();
        }

        

       public List<SelectModel> GetDDLVendors(int companyId)
        {
            var result = context.Vendors.Where(x=>x.CompanyId==companyId).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VendorId.ToString()
            }).ToList();

            return result;
        }
        

        public async Task<List<SelectListItem>> GetDDLProductList(int companyId)
        {
            var query = from p in context.Products
                        where p.IsActive == true && p.CompanyId == companyId
                        select new SelectListItem
                        {
                            Text=p.ProductName,
                            Value=p.ProductId.ToString()
                        };
            return await query.ToListAsync();
        }


        public async Task<List<SelectListItem>> GetDDLCompanyWiseOrder(int CompanyId)
        {
            var Query = from o in context.OrderMasters
                        where o.IsActive == true && o.CompanyId == CompanyId
                        select new SelectListItem
                        {
                            Text = o.OrderNo,
                            Value = o.OrderMasterId.ToString()
                        };
            return await Query.ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDLOrderWiseProduct(int orderMasterId)
        {
            var result =await (from t1 in context.OrderDetails.Where(x => x.OrderMasterId == orderMasterId)

                     join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                     join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                     join t6 in context.FinishProductBOMs on t1.OrderDetailId equals t6.OrderDetailId
                     where t1.IsActive

                     select new SelectListItem
                     {
                         Value = t1.OrderDetailId.ToString(),

                         Text = t4.Name + " " + t3.ProductName


                     }).Distinct().ToListAsync();

            return result;
        }

        public async Task<List<SelectListItem>> GetDDLCustomerByCompany(int companyId, CancellationToken cancellationToken = default)
        {
            var customerDDL = await (from v in context.Vendors
                                     where v.IsActive
                                     && v.CompanyId == companyId
                                     select new SelectListItem
                                     {
                                         Text=v.Name+"-"+v.Code,
                                         Value=v.VendorId.ToString()
                                     }).ToListAsync(cancellationToken);
            return customerDDL;
        }

        public async Task<List<SelectListItem>> GetDDLOrderByCustomer(long VendorId, CancellationToken cancellationToken = default)
        {
            var orderDDL = await (from v in context.OrderMasters
                                     where v.IsActive
                                     && v.CustomerId == VendorId
                                     select new SelectListItem
                                     {
                                         Text = v.OrderNo + "("+ v.OrderDate +")",
                                         Value = v.OrderMasterId.ToString()
                                     }).ToListAsync(cancellationToken);
            return orderDDL;
        }

        public async Task<List<SelectListItem>> GetDDLOrderDeliveryByOrderMaster(long OrderMasterId, CancellationToken cancellationToken = default)
        {
            var orderDeliversDDL = await (from v in context.OrderDelivers
                                  where v.IsActive
                                  && v.OrderMasterId == OrderMasterId
                                  select new SelectListItem
                                  {
                                      Text = v.ChallanNo + "-" + v.DeliveryDate,
                                      Value = v.OrderDeliverId.ToString()
                                  }).ToListAsync(cancellationToken);
            return orderDeliversDDL;
        }

        public async Task<List<SelectListItem>> GetDDLOrderDeliveryDetailsByOrderDelivery(long OrderDeliveryId, CancellationToken cancellationToken = default)
        {
            var OrderDeliverDetailsDDL = await (from v in context.OrderDeliverDetails
                                          join p in context.Products on v.ProductId equals p.ProductId
                                          where v.IsActive
                                          && v.OrderDeliverId== OrderDeliveryId
                                          select new SelectListItem
                                          {
                                              Text =p.ProductType+" "+ p.ProductName,
                                              Value = v.OrderDeliverDetailId.ToString()
                                          }).ToListAsync(cancellationToken);
            return OrderDeliverDetailsDDL;
        }

        public async Task<List<SelectListItem>> GetDDLAllEmployeeByCompanyId(int CompanyId = 0, CancellationToken cancellationToken = default)
        {
            var resultList = await context.Employees
                                          .Where(x => x.Active && (CompanyId <= 0 || x.CompanyId == CompanyId))
                                          .Select(x => new SelectListItem
                                          {
                                              Value = x.Id.ToString(),
                                              Text = x.Name+" ("+x.EmployeeId+")"
                                          })
                                          .ToListAsync(cancellationToken);

            return resultList;
        }

    }
}
