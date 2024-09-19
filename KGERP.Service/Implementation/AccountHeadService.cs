using KGERP.Data.Models;
using KGERP.Service.Interface;
using System;

namespace KGERP.Service.Implementation
{
    public class AccountHeadService : IAccountHeadService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public AccountHeadService(ERPEntities context)
        {
            this.context = context;
        }

        //public List<AccountHeadModel> GetAccountHeads(int companyId, string searchText)
        //{
        //    IQueryable<AccountHead> AccountHeads = context.AccountHeads.Include(x => x.ParentHead).Where(x => x.CompanyId==companyId && (x.AccCode.StartsWith(searchText) || x.AccName.StartsWith(searchText))).OrderBy(x => x.AccCode);
        //    return ObjectConverter<AccountHead, AccountHeadModel>.ConvertList(AccountHeads.ToList()).ToList();
        //}

        //public AccountHeadModel GetAccountHead(long id)
        //{
        //    if (id == 0)
        //    {
        //        return new AccountHeadModel() { AccountHeadId = id };
        //    }
        //    AccountHead accountHead = context.AccountHeads.Include(x => x.ParentHead).Where(x => x.AccountHeadId == id).FirstOrDefault();
        //    if (accountHead.ParentId == null)
        //    {
        //        return ObjectConverter<AccountHead, AccountHeadModel>.Convert(accountHead);
        //    }
        //    accountHead.ParentHead.AccName = "[" + accountHead.ParentHead.AccCode + "] " + accountHead.ParentHead.AccName;

        //    return ObjectConverter<AccountHead, AccountHeadModel>.Convert(accountHead);
        //}


        //public bool SaveAccountHead(long id, AccountHeadModel model)
        //{
        //    AccountHead AccountHead = ObjectConverter<AccountHeadModel, AccountHead>.Convert(model);
        //    if (id > 0)
        //    {
        //        AccountHead.ModifiedDate = DateTime.Now;
        //        AccountHead.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //    }
        //    else
        //    {
        //        AccountHead.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        AccountHead.CreateDate = DateTime.Now;
        //    }
        //    AccountHead.ParentHead = null;
        //    AccountHead.AccCode = model.AccCode;
        //    AccountHead.AccName = model.AccName;
        //    AccountHead.ParentId = model.ParentId;
        //    AccountHead.TierNo = GetTierNoByParent(model.ParentId);
        //    AccountHead.Remarks = model.Remarks;
        //    context.Entry(AccountHead).State = AccountHead.AccountHeadId == 0 ? EntityState.Added : EntityState.Modified;
        //    return context.SaveChanges() > 0;
        //}

        //private int? GetTierNoByParent(long? parentId)
        //{
        //    if (parentId == null)
        //    {
        //        return 1;
        //    }
        //    return context.AccountHeads.Where(x => x.AccountHeadId == parentId).First().TierNo + 1;
        //}
        //public List<SelectModel> GetParentAccountHeadSelectModelByCompany(int companyId)
        //{
        //    IQueryable<AccountHead> queryable = context.AccountHeads.Where(x => x.CompanyId == companyId).OrderBy(x => x.AccCode);
        //    return queryable.ToList().Select(x => new SelectModel { Text = x.AccName, Value = x.AccountHeadId }).ToList();
        //}

        //public bool DeleteAccountHead(long id)
        //{
        //    AccountHead accountHead = context.AccountHeads.Find(id);
        //    context.AccountHeads.Remove(accountHead);
        //    return context.SaveChanges() > 0;
        //}




        //public object GetAccountHeadAutoComplete(string prefix, int companyId)
        //{
        //    return context.AccountHeads.Where(x => x.CompanyId == companyId && (x.AccName.StartsWith(prefix) || x.AccCode.StartsWith(prefix))).Select(x => new
        //    {
        //        label = "[" + x.AccCode + "] " + x.AccName,
        //        val = x.AccountHeadId
        //    }).OrderBy(x => x.label).Take(20).ToList();
        //}


        //public string GenerateNewAccountHead(int? accountHeadId)
        //{
        //    if (accountHeadId == null)
        //        return string.Empty;
        //    AccountHead accountHead = context.AccountHeads.Where(x => x.AccountHeadId == accountHeadId).FirstOrDefault();
        //    if (accountHead == null)
        //        return string.Empty;

        //    //For Second Layer
        //    if (accountHead.AccCode.Length==1)
        //    {
        //        var items = context.AccountHeads.Where(x => x.ParentId == accountHeadId);
        //        if (!items.Any())
        //        {
        //            return accountHead.AccCode + "1";
        //        }

        //        AccountHead lastHead = items.ToList().OrderBy(x => x.AccCode).LastOrDefault();
        //        string parentItem = lastHead.AccCode.Substring(0, 1);
        //        string childItem = lastHead.AccCode.Substring(1, 1);
        //        if (Convert.ToInt32(childItem) >= 9)
        //        {
        //            return "Limit Exceeded!";
        //        }

        //        return parentItem + (Convert.ToInt32(childItem) + 1).ToString();
        //    }


        //    //For third Layer
        //    if (accountHead.AccCode.Length == 2)
        //    {
        //        var items = context.AccountHeads.Where(x => x.ParentId == accountHeadId);
        //        if (!items.Any())
        //        {
        //            return accountHead.AccCode + "01";
        //        }

        //        AccountHead lastHead = items.ToList().OrderBy(x => x.AccCode).LastOrDefault();
        //        string parentItem = lastHead.AccCode.Substring(0, 2);
        //        string childItem = lastHead.AccCode.Substring(2, 2);
        //        if (Convert.ToInt32(childItem) >= 99)
        //        {
        //            return "Limit Exceeded!";
        //        }

        //        return parentItem + (Convert.ToInt32(childItem) + 1).ToString().PadLeft(2,'0');
        //    }

        //    //For Fourth Layer
        //    if (accountHead.AccCode.Length == 4)
        //    {
        //        var items = context.AccountHeads.Where(x => x.ParentId == accountHeadId);
        //        if (!items.Any())
        //        {
        //            return accountHead.AccCode + "001";
        //        }

        //        AccountHead lastHead = items.ToList().OrderBy(x => x.AccCode).LastOrDefault();
        //        string parentItem = lastHead.AccCode.Substring(0, 4);
        //        string childItem = lastHead.AccCode.Substring(4, 3);
        //        if (Convert.ToInt32(childItem) >= 999)
        //        {
        //            return "Limit Exceeded!";
        //        }

        //        return parentItem + (Convert.ToInt32(childItem) + 1).ToString().PadLeft(3, '0');
        //    }


        //    //For Fifth Layer
        //    if (accountHead.AccCode.Length == 7)
        //    {
        //        var items = context.AccountHeads.Where(x => x.ParentId == accountHeadId);
        //        if (!items.Any())
        //        {
        //            return accountHead.AccCode + "001";
        //        }

        //        AccountHead lastHead = items.ToList().OrderBy(x => x.AccCode).LastOrDefault();
        //        string parentItem = lastHead.AccCode.Substring(0, 7);
        //        string childItem = lastHead.AccCode.Substring(7, 3);
        //        if (Convert.ToInt32(childItem) >= 999)
        //        {
        //            return "Limit Exceeded!";
        //        }

        //        return parentItem + (Convert.ToInt32(childItem) + 1).ToString().PadLeft(3, '0');
        //    }
        //    return "Something Went Worng !";
        //}




        //public object GetAccountHeadTreeView()
        //{
        //    return context.AccountHeads.Select(x => new { id = x.AccountHeadId, text = x.AccName }).ToList();
        //}

        //public List<SelectModel> GetAccountHeadSelectModelsByCompany(int companyId)
        //{
        //    IQueryable<AccountHead> queryable = context.AccountHeads.Where(x => x.CompanyId == companyId).OrderBy(x => x.AccCode);
        //    return queryable.ToList().Select(x => new SelectModel { Text = x.AccName, Value = x.AccountHeadId }).ToList();
        //}

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //public List<AccountHeadModel> GetAccountHeadsByCompany(int companyId)
        //{
        //    IQueryable<AccountHead> AccountHeads = context.AccountHeads.Where(x =>x.CompanyId == companyId && !x.ParentId.HasValue).OrderBy(x=>x.OrderNo);
        //    return ObjectConverter<AccountHead, AccountHeadModel>.ConvertList(AccountHeads.ToList()).ToList();
        //}

        //public List<AccountHeadProcessModel> GetAccountHeadsTreeViewByCompany(int companyId)
        //{
        //    int onOfRowsAffected = context.Database.ExecuteSqlCommand("spGetAccountHead  {0}", companyId);
        //    List<AccountHeadProcess> AccountHeads= context.AccountHeadProcesses.Where(x => !x.ParentId.HasValue).ToList();
        //    return ObjectConverter<AccountHeadProcess, AccountHeadProcessModel>.ConvertList(AccountHeads.ToList()).ToList();

        //}
    }
}
