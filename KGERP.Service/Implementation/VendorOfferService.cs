using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class VendorOfferService : IVendorOfferService
    {
        private readonly ERPEntities context;
        public VendorOfferService(ERPEntities context)
        {
            this.context = context;
        }

        public VendorOfferModel GetVendorOffer(int id)
        {
            if (id == 0)
            {
                return new VendorOfferModel() { VendorId = id };
            }
            var v = context.Database.SqlQuery<VendorOfferModel>("exec spGetProductOffersById {0}", id).FirstOrDefault();
            return v;
        }

        public List<VendorOfferModel> GetVendorOffers(int vendorId, string productType, string searchText)
        {
            IQueryable<VendorOfferModel> models = context.Database.SqlQuery<VendorOfferModel>("exec spGetProductOffers {0},{1}", vendorId, productType).AsQueryable();
            return models.Where(x => x.ProductName.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)).OrderBy(x => x.ProductCategory).ThenBy(x => x.ProductCode).ToList();
        }



        public int InsertCustomerOffer(int companyId)
        {
            int count = 0;
            List<Vendor> customers = context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == (int)Provider.Customer && x.IsActive).ToList();
            foreach (var customer in customers)
            {
                List<VendorOffer> vendorOffers = new List<VendorOffer>();
                List<Product> products = context.Products.Include(x => x.ProductCategory).Include(x => x.ProductSubCategory).Where(x => x.ProductCategory.ProductType == "F" && x.ProductCategory.CompanyId == 8).ToList();
                foreach (var product in products)
                {
                    decimal cashCommission = 0;
                    decimal carryingCommission = 0;
                    if (customer.CustomerType == "Cash")
                    {
                        cashCommission = product.ProductCategory.CashCustomerRate ?? 0;
                    }

                    VendorOffer vendorOffer = new VendorOffer
                    {
                        VendorId = customer.VendorId,
                        ProductId = product.ProductId,
                        BaseCommission = product.ProductSubCategory.BaseCommissionRate,
                        CashCommission = cashCommission,
                        CarryingCommission = carryingCommission,
                        SpecialCommission = 0,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    vendorOffers.Add(vendorOffer);
                    count++;
                }
                context.VendorOffers.AddRange(vendorOffers);
                context.SaveChanges();
            }
            return count;
        }

        public bool SaveVendorOffer(long id, VendorOfferModel model)
        {
            int noOfRowsAffected = 0;
            if (model == null)
            {
                throw new Exception("Data missing!");
            }

            //bool exist = context.VendorOffers.Where(x => x.VendorId == model.VendorId && x.ProductId == model.ProductId && x.VendorOfferId != id).Any();

            //if (exist)
            //{
            //    throw new Exception("Already exist!");
            //}
            VendorOffer vendorOffer = ObjectConverter<VendorOfferModel, VendorOffer>.Convert(model);
            if (id > 0)
            {
                vendorOffer = context.VendorOffers.Find(id);
                if (vendorOffer == null)
                {
                    throw new Exception("Data not found!");
                }
                vendorOffer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                vendorOffer.ModifiedDate = DateTime.Now;
            }

            else
            {
                vendorOffer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                vendorOffer.CreatedDate = DateTime.Now;
            }


            vendorOffer.VendorId = model.VendorId;
            vendorOffer.ProductId = model.ProductId;
            vendorOffer.BaseCommission = model.BaseCommission;
            vendorOffer.CashCommission = model.CashCommission;

            vendorOffer.CarryingCommission = model.CarryingCommission;
            vendorOffer.FactoryCarryingCommission = model.FactoryCarryingCommission;
            vendorOffer.SpecialCommission = model.SpecialCommission;
            vendorOffer.AdditionPrice = model.AdditionPrice;
            vendorOffer.MonthlyIncentive = model.MonthlyIncentive;
            vendorOffer.YearlyIncentive = model.YearlyIncentive;


            vendorOffer.IsActive = model.IsActive;

            context.Entry(vendorOffer).State = vendorOffer.VendorOfferId == 0 ? EntityState.Added : EntityState.Modified;
            noOfRowsAffected = context.SaveChanges();

            if (noOfRowsAffected > 0)
            {
                if (model.IsAllBase)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set BaseCommission= {0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.BaseCommission, model.VendorId, model.ProductCategoryId);
                }
                if (model.IsAllCash)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set CashCommission={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.CashCommission, model.VendorId, model.ProductCategoryId);
                }

                if (model.IsAllCarrying)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set CarryingCommission={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.CarryingCommission, model.VendorId, model.ProductCategoryId);
                }
                if (model.IsAllFactoryCarrying)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set FactoryCarryingCommission={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.FactoryCarryingCommission, model.VendorId, model.ProductCategoryId);
                }
                if (model.IsAllSpecial)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set SpecialCommission={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.SpecialCommission, model.VendorId, model.ProductCategoryId);
                }

                if (model.IsAllMonthlyIncentive)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set MonthlyIncentive={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.MonthlyIncentive, model.VendorId, model.ProductCategoryId);
                }

                if (model.IsAllYearlyIncentive)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set YearlyIncentive={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.YearlyIncentive, model.VendorId, model.ProductCategoryId);
                }

                if (model.IsAllAdditionPrice)
                {
                    context.Database.ExecuteSqlCommand("update Erp.VendorOffer set AdditionPrice={0} From Erp.VendorOffer vo Join Erp.Product p on vo.ProductId = p.ProductId Join Erp.ProductSubCategory psc on p.ProductSubCategoryId = psc.ProductSubCategoryId where vo.VendorId={1} And psc.ProductCategoryId = {2}", model.AdditionPrice, model.VendorId, model.ProductCategoryId);
                }

            }
            return noOfRowsAffected > 0;
        }
    }
}
