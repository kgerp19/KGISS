using KGERP.Data.Models;
using KGERP.Service.Implementation.Marketing;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Short__CreditService
{
    public class Short__CreditService : Ishort__CreditService
    {
        private readonly ERPEntities context;
        private readonly AccountingService _accountingService;

        public Short__CreditService(ERPEntities context)
        {
            this.context = context;
            _accountingService = new AccountingService(context);

        }
        public async Task<ShortCreditViewModel> AddPayment(ShortCreditViewModel model)
        {           
            SCreditCollection collection = new SCreditCollection
            {
                CompanyId = model.CompanyId,
                BankHeadId = model.BankHeadId,
                BankCharge = model.BankCharge,
                CollectedAmount = model.Amount,
                CollectionDate = model.CollectionDate,
                VendorId = model.VendorId,
                IsActive = true,
                CreateDate = DateTime.Now,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name
            };
            context.SCreditCollections.Add(collection);
            if (await context.SaveChangesAsync() > 0)
            {
                model.ShortCreditCollectionId = collection.ShortCreditCollectionId;
            }
            

            var result = await context.Payments
                        .Where(f => f.VendorId == model.VendorId &&
                        f.CompanyId == model.CompanyId &&
                        f.BranchName == "Short Credit" &&
                        f.InAmount > f.OutAmount).ToListAsync();
            decimal amount = 0;
            decimal collaction = 0;
            string mes = "";
            amount = model.Amount;
            if (amount > 0)
            {
                if (result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        var amt = item.InAmount - item.OutAmount;
                        if (amt > 0 && amount > 0)
                        {
                            var a = (decimal)(amount - amt);
                            item.OutAmount = a > 0 ? item.OutAmount + amt : item.OutAmount + amount;
                            amount = (decimal)(a > 0 ? amount - amt : 0);
                            item.SCreditCollectionId = model.ShortCreditCollectionId;
                            item.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            item.ModifiedDate = DateTime.Now;
                            context.Entry(item).State = EntityState.Modified;
                            context.SaveChanges();
                            collaction = (decimal)(collaction + a);
                        }
                        else
                        {
                            break;
                        }
                    }
                    model.Message = "Total Disbursement Amount is :" + (amount > 0 ? model.Amount - amount : model.Amount).ToString() + ".Your Submit amount id: " + model.Amount + ".";
                    return model;
                }
                model.Message = "Payment Not Found";
                return model;


            }
            model.Message = "Wrong input";
            return model;
        }

        public async Task<long> SubmitCollectionMasters(ShortCreditViewModel model)
        {
            long result = -1;
            SCreditCollection screditCollection = await context.SCreditCollections.FindAsync(model.ShortCreditCollectionId);
            screditCollection.IsSubmitted = true;

            screditCollection.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            screditCollection.ModifiedDate = DateTime.Now;
            if (await context.SaveChangesAsync() > 0)
            {
                result = screditCollection.ShortCreditCollectionId;
            }
            if (result > 0 && model.CompanyId == (int)CompanyName.KrishibidFeedLimited)
            {
                var data = await Task.Run(() => GetShortCreditCollectedAmount(model.CompanyId, model.ShortCreditCollectionId));
                await _accountingService.CreditCullectionPushFeed(model.CompanyId, data, (int)FeedJournalEnum.CreditVoucher);
            }
            return result;
        }


        public async Task<ShortCreditViewModel> GetShortCreditCollectedAmount(int companyId, long sCreditCollectionId)
        {
            ShortCreditViewModel vmMmodel = new ShortCreditViewModel();

            vmMmodel = await Task.Run(() => (from t1 in context.SCreditCollections.Where(x => x.IsActive && x.ShortCreditCollectionId == sCreditCollectionId)
                                             join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                             join t3 in context.HeadGLs on t1.BankHeadId equals t3.Id
                                             where t2.CompanyId == companyId
                                             select new ShortCreditViewModel
                                             {
                                                 CompanyId = t1.CompanyId,
                                                 CollectionDate = t1.CollectionDate,
                                                 ShortCreditCollectionId = t1.ShortCreditCollectionId,
                                                 CollectedAmount = t1.CollectedAmount,
                                                 BankCharge = t1.BankCharge,
                                                 BankHeadId = t1.BankHeadId,
                                                 VendorName = t2.Name,
                                                 HeadGLId = t2.HeadGLId,
                                                 BankName = t3.AccCode + "-"+ t3.AccName,
                                                 IsSubmitted = t1.IsSubmitted
                                             }).FirstOrDefaultAsync());




            return vmMmodel;
        }

        public VMFeedPayment GetShortList(int Vendrid)
        {
            VMFeedPayment vMFeedPayment = new VMFeedPayment();
            vMFeedPayment.DataListPayment = (IEnumerable<VMFeedPayment>)(from t1 in context.Payments
                                                                         join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                                         where t1.VendorId == Vendrid && t1.BranchName == "Short Credit" && t1.IsActive == true

                                                                         select new VMFeedPayment
                                                                         {
                                                                             Vendorname = t2.Name,
                                                                             VendorId = t1.VendorId,
                                                                             BranchName = t1.BranchName,
                                                                             TransactionDate = t1.TransactionDate,
                                                                             ReferenceNo = t1.ReferenceNo,
                                                                             InAmount = t1.InAmount,
                                                                             OutAmount = t1.OutAmount,



                                                                         }).ToList();
             vMFeedPayment.totalInAmount = vMFeedPayment.DataListPayment.Sum(item => item.InAmount);
            vMFeedPayment.totalOutAmount = vMFeedPayment.DataListPayment.Sum(item => item.OutAmount);

            vMFeedPayment.minustotal = vMFeedPayment.totalInAmount - vMFeedPayment.totalOutAmount;

            return vMFeedPayment;
        }
    }
}
