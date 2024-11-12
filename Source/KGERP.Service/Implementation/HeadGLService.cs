using KGERP.Data.CustomModel;
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
    public class HeadGLService : IHeadGLService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public HeadGLService(ERPEntities context)
        {
            this.context = context;
        }

        //public List<AccountHeadModel> GetAccountHeads(int companyId, string searchText)
        //{
        //    IQueryable<AccountHead> AccountHeads = context.AccountHeads.Include(x => x.ParentHead).Where(x => x.CompanyId == companyId && (x.AccCode.StartsWith(searchText) || x.AccName.StartsWith(searchText))).OrderBy(x => x.AccCode);
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
        //    if (accountHead.AccCode.Length == 1)
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

        //        return parentItem + (Convert.ToInt32(childItem) + 1).ToString().PadLeft(2, '0');
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
        //    IQueryable<AccountHead> AccountHeads = context.AccountHeads.Where(x => x.CompanyId == companyId && !x.ParentId.HasValue).OrderBy(x => x.OrderNo);
        //    return ObjectConverter<AccountHead, AccountHeadModel>.ConvertList(AccountHeads.ToList()).ToList();
        //}

        public List<Head1> GetAccountHeadsTreeViewByCompany(int companyId)
        {
            //----1st layer---------------
            List<Head1> parentList = context.Head1.Include(x => x.Head2).Where(x => x.ParentId == null && x.CompanyId == companyId && x.IsActive == true).OrderBy(x => x.AccCode).ToList();
            foreach (var h1Item in parentList)
            {
                //----2nd layer---------------
                h1Item.Head11 = h1Item.Head2.Where(x => x.ParentId == h1Item.Id && x.CompanyId == h1Item.CompanyId && x.LayerNo == 2 && x.IsActive == true).Select(x => new Head1 { Id = x.Id, ParentId = x.ParentId, CompanyId = x.CompanyId, AccCode = x.AccCode, AccName = x.AccName, LayerNo = x.LayerNo }).OrderBy(x  => x.AccCode).ToList();

                foreach (var h2Item in h1Item.Head11)
                {
                    //----3rd layer---------------
                    h2Item.Head11 = context.Head3.Where(x => x.ParentId == h2Item.Id && x.CompanyId == h2Item.CompanyId && x.LayerNo == 3 && x.IsActive == true).ToList().Select(x => new Head1 { Id = x.Id, ParentId = x.ParentId, CompanyId = x.CompanyId, AccCode = x.AccCode, AccName = x.AccName, LayerNo = x.LayerNo }).OrderBy(x => x.AccCode).ToList();

                    foreach (var h3Item in h2Item.Head11)
                    {
                        //----4th layer---------------
                        h3Item.Head11 = context.Head4.Where(x => x.ParentId == h3Item.Id && x.CompanyId == h3Item.CompanyId && x.LayerNo == 4 && x.IsActive == true).ToList().Select(x => new Head1 { Id = x.Id, ParentId = x.ParentId, CompanyId = x.CompanyId, AccCode = x.AccCode, AccName = x.AccName, LayerNo = x.LayerNo }).OrderBy(x => x.AccCode).ToList();
                        foreach (var h4Item in h3Item.Head11)
                        {
                            //----5th layer---------------
                            h4Item.Head11 = context.Head5.Where(x => x.ParentId == h4Item.Id && x.CompanyId == h4Item.CompanyId && x.LayerNo == 5 && x.IsActive == true).ToList().Select(x => new Head1 { Id = x.Id, ParentId = x.ParentId, CompanyId = x.CompanyId, AccCode = x.AccCode, AccName = x.AccName, LayerNo = x.LayerNo }).OrderBy(x => x.AccCode).ToList();
                            foreach (var h5Item in h4Item.Head11)
                            {
                                //----6th/GL  layer---------------
                                h5Item.Head11 = context.HeadGLs.Where(x => x.ParentId == h5Item.Id && x.CompanyId == h5Item.CompanyId && x.LayerNo == 6 && x.IsActive == true).ToList().Select(x => new Head1 { Id = x.Id, ParentId = x.ParentId, CompanyId = x.CompanyId, AccCode = x.AccCode, AccName = x.AccName, LayerNo = x.LayerNo }).OrderBy(x => x.AccCode).ToList();

                            }
                        }

                    }
                }
            }
            return parentList;
        }

        public AccountHeadProcessModel GetAccountHeadProcessCreate(int accountHeadId, int layerNo, string status)
        {
            string newAccountCode = string.Empty;
            int orderNo = 0;
            AccountHeadProcessModel model = new AccountHeadProcessModel { ButtonName = "Create" };
            if (layerNo == 1)
            {
                Head1 parentHead = context.Head1.Where(x => x.Id == accountHeadId).FirstOrDefault();
                IQueryable<Head2> childHeads = context.Head2.Where(x => x.ParentId == accountHeadId);

                if (childHeads.Count() > 0)
                {
                    string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                    string parentPart = lastAccCode.Substring(0, 1);
                    string childPart = lastAccCode.Substring(1, 1);
                    newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString();
                    orderNo = childHeads.Count() + 1;
                }

                else
                {
                    string parentPart = parentHead.AccCode.Substring(0, 1);
                    string childPart = "1";
                    newAccountCode = parentPart + childPart;
                    orderNo = childHeads.Count() + 1;
                }


                model.ParentId = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = newAccountCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo + 1;
                model.OrderNo = orderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = "2nd Layer";
                model.Status = status;
                return model;
            }


            if (layerNo == 2)
            {
                Head2 parentHead = context.Head2.Where(x => x.Id == accountHeadId).FirstOrDefault();
                IQueryable<Head3> childHeads = context.Head3.Where(x => x.ParentId == accountHeadId);

                if (childHeads.Count() > 0)
                {
                    string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                    string parentPart = lastAccCode.Substring(0, 2);
                    string childPart = lastAccCode.Substring(2, 2);
                    newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(2, '0');
                    orderNo = childHeads.Count() + 1;
                }


                else
                {
                    newAccountCode = parentHead.AccCode + "01";
                    orderNo = orderNo + 1;
                }


                model.ParentId = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = newAccountCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo + 1;
                model.OrderNo = orderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = "3rd Layer";
                model.Status = status;

                return model;
            }

            if (layerNo == 3)
            {
                Head3 parentHead = context.Head3.Where(x => x.Id == accountHeadId).FirstOrDefault();
                IQueryable<Head4> childHeads = context.Head4.Where(x => x.ParentId == accountHeadId);

                if (childHeads.Count() > 0)
                {
                    string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                    string parentPart = lastAccCode.Substring(0, 4);
                    string childPart = lastAccCode.Substring(4, 3);
                    newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                    orderNo = childHeads.Count() + 1;
                }

                else
                {
                    newAccountCode = parentHead.AccCode + "001";
                    orderNo = orderNo + 1;
                }


                model.ParentId = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = newAccountCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo + 1;
                model.OrderNo = orderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = "4th Layer";
                model.Status = status;
                return model;
            }

            if (layerNo == 4)
            {
                Head4 parentHead = context.Head4.Where(x => x.Id == accountHeadId).FirstOrDefault();
                IQueryable<Head5> childHeads = context.Head5.Where(x => x.ParentId == accountHeadId);

                if (childHeads.Count() > 0)
                {
                    string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                    string parentPart = lastAccCode.Substring(0, 7);
                    string childPart = lastAccCode.Substring(7, 3);
                    newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                    orderNo = childHeads.Count();
                }

                else
                {
                    newAccountCode = parentHead.AccCode + "001";
                    orderNo = orderNo + 1;
                }


                model.ParentId = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = newAccountCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo + 1;
                model.OrderNo = orderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = "5th Layer";
                model.Status = status;

                return model;
            }


            if (layerNo == 5)
            {
                Head5 parentHead = context.Head5.Where(x => x.Id == accountHeadId).FirstOrDefault();
                IQueryable<HeadGL> childHeads = context.HeadGLs.Where(x => x.ParentId == accountHeadId);

                if (childHeads.Count() > 0)
                {
                    string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                    string parentPart = lastAccCode.Substring(0, 10);
                    string childPart = lastAccCode.Substring(10, 3);
                    newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                    orderNo = childHeads.Count();
                }

                else
                {
                    newAccountCode = parentHead.AccCode + "001";
                    orderNo = orderNo + 1;
                }


                model.ParentId = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = newAccountCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo + 1;
                model.OrderNo = orderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = "GL Layer";
                model.Status = status;
                return model;
            }


            return model;
        }

        public AccountHeadProcessModel GetSelectedItem(int accountHeadId, int layerNo, string accCode, string accName)
        {

            bool isPrimaryHead = false;

            switch (layerNo)
            {
                case 1:
                    isPrimaryHead = context.Head1.Any(x => x.Id == accountHeadId && x.IsPrimaryHead1 && x.IsActive);
                    break;
                case 2:
                    isPrimaryHead = context.Head2.Any(x => x.Id == accountHeadId && x.IsPrimaryHead2 && x.IsActive);
                    break;
                case 3:
                    isPrimaryHead = context.Head3.Any(x => x.Id == accountHeadId && x.IsPrimaryHead3 && x.IsActive);
                    break;
                case 4:
                    isPrimaryHead = context.Head4.Any(x => x.Id == accountHeadId && x.IsPrimaryHead4 && x.IsActive);
                    break;
                case 5:
                    isPrimaryHead = context.Head5.Any(x => x.Id == accountHeadId && x.IsPrimaryHead5 && x.IsActive);
                    break;
                case 6:
                    isPrimaryHead = context.HeadGLs.Any(x => x.Id == accountHeadId && x.IsPrimary && x.IsActive);
                    break;

                default:
                    isPrimaryHead = false;
                    break;
            }
            

            return new AccountHeadProcessModel { Id = accountHeadId, LayerNo = layerNo, AccCode = accCode, AccName = accName,IsPrimaryHead= isPrimaryHead };
        }

        public bool SaveAccountHead(AccountHeadProcessModel model)
        {
            if (model.Status.Equals("create"))
            {
                if (model.LayerNo == 2)
                {
                    int id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault();
                    Head2 head2 = new Head2()
                    {
                        Id = id,
                        ParentId = model.ParentId,
                        CompanyId = model.CompanyId,
                        AccCode = model.AccCode,
                        AccName = model.AccName,
                        OrderNo = model.OrderNo,
                        LayerNo = model.LayerNo,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreateDate = DateTime.Now,
                        Remarks = model.Remarks,
                        IsActive = true
                    };
                    context.Head2.Add(head2);
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 3)
                {
                    int id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault();
                    Head3 head3 = new Head3()
                    {
                        Id = id,
                        ParentId = model.ParentId,
                        CompanyId = model.CompanyId,
                        AccCode = model.AccCode,
                        AccName = model.AccName,
                        OrderNo = model.OrderNo,
                        LayerNo = model.LayerNo,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreateDate = DateTime.Now,
                        Remarks = model.Remarks,
                        IsActive = true
                    };
                    context.Head3.Add(head3);
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 4)
                {
                    int id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault();
                    Head4 head4 = new Head4()
                    {
                        Id = id,
                        ParentId = model.ParentId,
                        CompanyId = model.CompanyId,
                        AccCode = model.AccCode,
                        AccName = model.AccName,
                        OrderNo = model.OrderNo,
                        LayerNo = model.LayerNo,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreateDate = DateTime.Now,
                        Remarks = model.Remarks,
                        IsActive = true
                    };
                    context.Head4.Add(head4);
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 5)
                {
                    int id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault();
                    Head5 head5 = new Head5()
                    {
                        Id = id,
                        ParentId = model.ParentId,
                        CompanyId = model.CompanyId,
                        AccCode = model.AccCode,
                        AccName = model.AccName,
                        OrderNo = model.OrderNo,
                        LayerNo = model.LayerNo,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreateDate = DateTime.Now,
                        Remarks = model.Remarks,
                        IsActive = true
                    };
                    context.Head5.Add(head5);
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 6)
                {
                    int id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault();
                    HeadGL glHead = new HeadGL()
                    {
                        Id = id,
                        ParentId = model.ParentId,
                        CompanyId = model.CompanyId,
                        AccCode = model.AccCode,
                        AccName = model.AccName,
                        OrderNo = model.OrderNo,
                        LayerNo = model.LayerNo,
                       
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreateDate = DateTime.Now,
                        Remarks = model.Remarks,
                        IsActive = true
                    };
                    context.HeadGLs.Add(glHead);
                    return context.SaveChanges() > 0;
                }
            }

            if (model.Status.Equals("update"))
            {
                if (model.LayerNo == 1)
                {
                    Head1 head1 = context.Head1.Where(x => x.Id == model.Id).FirstOrDefault();

                    head1.AccName = model.AccName;
                    head1.AccCode = model.AccCode;
                    head1.CompanyId = model.CompanyId;
                    head1.LayerNo = model.LayerNo;
                    head1.OrderNo = model.OrderNo;
                    head1.Remarks = model.Remarks;
                    head1.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    head1.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 2)
                {
                    Head2 head2 = context.Head2.Where(x => x.Id == model.Id).FirstOrDefault();

                    head2.ParentId = model.ParentId;
                    head2.AccName = model.AccName;
                    head2.AccCode = model.AccCode;
                    head2.CompanyId = model.CompanyId;
                    head2.LayerNo = model.LayerNo;
                    head2.OrderNo = model.OrderNo;
                    head2.Remarks = model.Remarks;
                    head2.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    head2.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 3)
                {
                    Head3 head3 = context.Head3.Where(x => x.Id == model.Id).FirstOrDefault();

                    head3.ParentId = model.ParentId;
                    head3.AccName = model.AccName;
                    head3.AccCode = model.AccCode;
                    head3.CompanyId = model.CompanyId;
                    head3.LayerNo = model.LayerNo;
                    head3.OrderNo = model.OrderNo;
                    head3.Remarks = model.Remarks;
                    head3.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    head3.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 4)
                {
                    Head4 head4 = context.Head4.Where(x => x.Id == model.Id).FirstOrDefault();

                    head4.ParentId = model.ParentId;
                    head4.AccName = model.AccName;
                    head4.AccCode = model.AccCode;
                    head4.CompanyId = model.CompanyId;
                    head4.LayerNo = model.LayerNo;
                    head4.OrderNo = model.OrderNo;
                    head4.Remarks = model.Remarks;
                    head4.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    head4.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }

                if (model.LayerNo == 5)
                {
                    Head5 head5 = context.Head5.Where(x => x.Id == model.Id).FirstOrDefault();

                    head5.ParentId = model.ParentId;
                    head5.AccName = model.AccName;
                    head5.AccCode = model.AccCode;
                    head5.CompanyId = model.CompanyId;
                    head5.LayerNo = model.LayerNo;
                    head5.OrderNo = model.OrderNo;
                    head5.Remarks = model.Remarks;
                    head5.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    head5.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }
                if (model.LayerNo == 6)
                {
                    HeadGL headGL = context.HeadGLs.Where(x => x.Id == model.Id).FirstOrDefault();

                    headGL.ParentId = model.ParentId;
                    headGL.AccName = model.AccName;
                    headGL.AccCode = model.AccCode;
                    headGL.CompanyId = model.CompanyId;
                    headGL.LayerNo = model.LayerNo;
                    headGL.OrderNo = model.OrderNo;
                    headGL.Remarks = model.Remarks;
                    headGL.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    headGL.ModifiedDate = DateTime.Now;
                    return context.SaveChanges() > 0;
                }
            }

            if (model.Status.Equals("delete"))
            {
                //if (model.LayerNo == 2)
                //{
                //    var obj = context.Head1.SingleOrDefault(e => e.Id == model.Id);
                //    if (obj == null)
                //    {
                //        return false;
                //    }
                //    else
                //    {
                //        obj.IsActive = false;
                //        if (context.SaveChanges() > 0)
                //            return true;

                //    }

                //    //int id = context.Database.ExecuteSqlCommand("delete from Head2 where Id={0}", model.Id);
                //    //return id > 0;
                //}
                if (model.LayerNo == 2)
                {
                    //int id = context.Database.ExecuteSqlCommand("delete from HeadGL where Id={0}", model.Id);
                    //return id > 0;
                    var obj = context.Head2.SingleOrDefault(e => e.Id == model.Id);
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        var chiled2 = context.Head3.Where(e => e.ParentId == model.Id && e.IsActive).ToList();

                        if (chiled2.Count == 0)
                        {
                            obj.IsActive = false;
                            if (context.SaveChanges() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return false;
                       

                    }

                }


                if (model.LayerNo == 3)
                {
                    //int id = context.Database.ExecuteSqlCommand("delete from HeadGL where Id={0}", model.Id);
                    //return id > 0;
                    var obj = context.Head3.SingleOrDefault(e => e.Id == model.Id);
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        var chiled3=context.Head4.Where(e=>e.ParentId==model.Id && e.IsActive).ToList();
                        if(chiled3.Count == 0)
                        {
                            obj.IsActive = false;
                            if (context.SaveChanges() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                     

                    }

                }
                if (model.LayerNo == 4)
                {
                    //int id = context.Database.ExecuteSqlCommand("delete from HeadGL where Id={0}", model.Id);
                    //return id > 0;
                    var obj = context.Head4.SingleOrDefault(e => e.Id == model.Id);
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        var chiled4 = context.Head5.Where(e => e.ParentId == model.Id && e.IsActive).ToList();

                        if( chiled4.Count == 0)
                        {
                            obj.IsActive = false;
                            if (context.SaveChanges() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        

                    }

                }
                if (model.LayerNo == 5)
                {
                    //int id = context.Database.ExecuteSqlCommand("delete from HeadGL where Id={0}", model.Id);
                    //return id > 0;
                    var obj = context.Head5.SingleOrDefault(e => e.Id == model.Id);
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {

                        var child5 = context.HeadGLs.Where(e => e.ParentId == model.Id && e.IsActive).ToList();

                        if (child5.Count == 0)
                        {
                            obj.IsActive = false;
                            if (context.SaveChanges() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                       

                    }

                }

                //if (model.LayerNo == 3)
                //{
                //    int id = context.Database.ExecuteSqlCommand("delete from Head3 where Id={0}", model.Id);
                //    return id > 0;
                //}

                //if (model.LayerNo == 4)
                //{
                //    int id = context.Database.ExecuteSqlCommand("delete from Head4 where Id={0}", model.Id);
                //    return id > 0;
                //}

                //if (model.LayerNo == 5)
                //{
                //    int id = context.Database.ExecuteSqlCommand("delete from Head5 where Id={0}", model.Id);
                //    return id > 0;
                //}

                if (model.LayerNo == 6)
                {
                    //int id = context.Database.ExecuteSqlCommand("delete from HeadGL where Id={0}", model.Id);
                    //return id > 0;
                    var obj = context.HeadGLs.SingleOrDefault(e => e.Id == model.Id);
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        var childGl = context.VoucherDetails.Where(e => e.AccountHeadId == model.Id && e.IsActive).ToList();

                        if (childGl.Count == 0)
                        {
                            obj.IsActive = false;
                            if (context.SaveChanges() > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                      

                    }
                   
                }
            }
            return false;
        }

        public AccountHeadProcessModel GetAccountHeadProcessUpdate(int accountHeadId, int layerNo, string status)
        {
            AccountHeadProcessModel model = new AccountHeadProcessModel { ButtonName = "Update" };
            if (layerNo == 1)
            {
                Head1 parentHead = context.Head1.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = parentHead.Id;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }


            if (layerNo == 2)
            {
                Head2 parentHead = context.Head2.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = parentHead.Id;
                model.ParentId = parentHead.ParentId;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }

            if (layerNo == 3)
            {
                Head3 parentHead = context.Head3.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = parentHead.Id;
                model.ParentId = parentHead.ParentId;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }

            if (layerNo == 4)
            {
                Head4 parentHead = context.Head4.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = parentHead.Id;
                model.ParentId = parentHead.ParentId;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }


            if (layerNo == 5)
            {
                Head5 parentHead = context.Head5.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = parentHead.Id;
                model.ParentId = parentHead.ParentId;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }

            if (layerNo == 6)
            {
                HeadGL parentHead = context.HeadGLs.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = parentHead.Id;
                model.ParentId = parentHead.ParentId;
                model.AccName = parentHead.AccName;
                model.AccCode = parentHead.AccCode;
                model.CompanyId = parentHead.CompanyId;
                model.LayerNo = parentHead.LayerNo;
                model.OrderNo = parentHead.OrderNo;
                model.ParentAccountName = parentHead.AccCode + "-" + parentHead.AccName;
                model.Remarks = parentHead.Remarks;
                model.Status = "update";
                return model;
            }
            return model;
        }
        public object AllAccountsHead(string prefix, int companyId)
        {
            var head1 = (from t1 in context.Head1
                         where t1.CompanyId == companyId
                         && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                         select new
                         {
                             label = t1.AccCode + "- " + t1.AccName,
                             val = t1.Id,
                             LayerNo = t1.LayerNo
                         }).AsEnumerable();
            var head2 = (from t1 in context.Head2
                         where t1.CompanyId == companyId
                          && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                         select new
                         {
                             label = t1.AccCode + "- " + t1.AccName,
                             val = t1.Id,
                             LayerNo = t1.LayerNo
                         }).AsEnumerable();
            var head3 = (from t1 in context.Head3
                         where t1.CompanyId == companyId
                          && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                         select new
                         {
                             label = t1.AccCode + "- " + t1.AccName,
                             val = t1.Id,
                             LayerNo = t1.LayerNo
                         }).AsEnumerable();
            var head4 = (from t1 in context.Head4
                         where t1.CompanyId == companyId
                          && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                         select new
                         {
                             label = t1.AccCode + "- " + t1.AccName,
                             val = t1.Id,
                             LayerNo = t1.LayerNo
                         }).AsEnumerable();
            var head5 = (from t1 in context.Head5
                         where t1.CompanyId == companyId
                          && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                         select new
                         {
                             label = t1.AccCode + "- " + t1.AccName,
                             val = t1.Id,
                             LayerNo = t1.LayerNo
                         }).AsEnumerable();
            var headGLs = (from t1 in context.HeadGLs
                           where t1.CompanyId == companyId
                            && (t1.AccName.Contains(prefix) || t1.AccCode.Contains(prefix))
                           select new
                           {
                               label = t1.AccCode + "- " + t1.AccName,
                               val = t1.Id,
                               LayerNo = t1.LayerNo
                           }).AsEnumerable();

            var List = head1.Union(head2).Union(head3).Union(head4).Union(head5).Union(headGLs).ToList();
            return List;
        }
        public object GetAccountHeadAutoComplete(string prefix, int companyId)
        {
            return context.HeadGLs.Where(x => x.CompanyId == companyId
            && (x.AccName.StartsWith(prefix) || x.AccCode.StartsWith(prefix))).Select(x => new
            {
                label = "[" + x.AccCode + "] " + x.AccName,
                val = x.Id
            }).OrderBy(x => x.label).Take(100).ToList();
        }

        public object GetMemberHeadAutoComplete(string prefix, int companyId)
        {
            
            string accCode = "2101001001";

            if(companyId == ((int)CompanyName.KrishibidPackagingLimited))
            {
                accCode = "2101000101";
            }
           var hd = (from t1 in context.HeadGLs
                     join t2 in context.Head5 on t1.ParentId equals t2.Id
                     where t2.AccCode == accCode && t1.CompanyId == companyId
            && (t1.AccName.Contains(prefix) || t1.AccCode.StartsWith(prefix))
            select new
            {
                label = "[" + t1.AccCode + "] " + t1.AccName,
                val = t1.Id
            }).OrderBy(x => x.label).Take(100).ToList();

            return hd;
        }


        public object GetPFHeadAutoComplete(string prefix, int companyId)
        {

            string accCode = "2419001001";
            if(companyId == ((int)CompanyName.KrishibidFeedLimited))
            {
                   accCode = "2418001001";
            }
            if (companyId == ((int)CompanyName.KrishibidSeedLimited))
            {
                accCode = "2415001001";
            }
            if (companyId == ((int)CompanyName.KrishibidPackagingLimited))
            {
                accCode = "2419001001";
            }
            if (companyId == ((int)CompanyName.KrishibidBazaarLimited))
            {
                accCode = "2417001002";
            }
            if (companyId == ((int)CompanyName.KrishibidSecurityAndServicesLimited))
            {
                accCode = "2414001001";
            }
            if (companyId == ((int)CompanyName.KrishibidPropertiesLimited))
            {
                accCode = "2401015001";
            }
            if (companyId == ((int)CompanyName.GloriousLandsAndDevelopmentsLimited))
            {
                accCode = "2401015001";
            }

            var hd = (from t1 in context.HeadGLs
                      join t2 in context.Head5 on t1.ParentId equals t2.Id
                      where t2.AccCode == accCode && t1.CompanyId == companyId
             && (t1.AccName.Contains(prefix) || t1.AccCode.StartsWith(prefix))
                      select new
                      {
                          label = "[" + t1.AccCode + "] " + t1.AccName,
                          val = t1.Id
                      }).OrderBy(x => x.label).Take(100).ToList();

            return hd;
        }


        public AccountHeadProcessModel GetAccountHeadProcessDelete(int accountHeadId, int layerNo, string status)
        {
            AccountHeadProcessModel model = new AccountHeadProcessModel { ButtonName = "Delete" };
            int countVouchers = context.VoucherDetails.Count(x => x.AccountHeadId == accountHeadId && x.IsActive);
            if (countVouchers > 0)
            {
                model.DeletedStatus = $"Could not be deleted! Already Have {countVouchers} voucher details using this";
            }
            if (layerNo == 1)
            {
                Head1 head1 = context.Head1.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = head1.Id;
                model.ParentId = head1.Id;
                model.AccName = head1.AccName;
                model.AccCode = head1.AccCode;
                model.CompanyId = head1.CompanyId;
                model.LayerNo = head1.LayerNo;
                model.OrderNo = head1.OrderNo;
                model.ParentAccountName = head1.AccCode + "-" + head1.AccName;
                model.Remarks = head1.Remarks;
                model.Status = status;              
                return model;
            }


            if (layerNo == 2)
            {
                Head2 head2 = context.Head2.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = head2.Id;
                model.ParentId = head2.Id;
                model.AccName = head2.AccName;
                model.AccCode = head2.AccCode;
                model.CompanyId = head2.CompanyId;
                model.LayerNo = head2.LayerNo;
                model.OrderNo = head2.OrderNo;
                model.ParentAccountName = head2.AccCode + "-" + head2.AccName;
                model.Remarks = head2.Remarks;
                model.Status = status; 
                return model;
            }

            if (layerNo == 3)
            {
                Head3 head3 = context.Head3.Where(x => x.Id == accountHeadId).FirstOrDefault();
                model.Id = head3.Id;
                model.ParentId = head3.Id;
                model.AccName = head3.AccName;
                model.AccCode = head3.AccCode;
                model.CompanyId = head3.CompanyId;
                model.LayerNo = head3.LayerNo;
                model.OrderNo = head3.OrderNo;
                model.ParentAccountName = head3.AccCode + "-" + head3.AccName;
                model.Remarks = head3.Remarks;
                model.Status = status;
                return model;
            }

            if (layerNo == 4)
            {
                Head4 head4 = context.Head4.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = head4.Id;
                model.ParentId = head4.Id;
                model.AccName = head4.AccName;
                model.AccCode = head4.AccCode;
                model.CompanyId = head4.CompanyId;
                model.LayerNo = head4.LayerNo;
                model.OrderNo = head4.OrderNo;
                model.ParentAccountName = head4.AccCode + "-" + head4.AccName;
                model.Remarks = head4.Remarks;
                model.Status = status;
                return model;
            }


            if (layerNo == 5)
            {
                Head5 head5 = context.Head5.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = head5.Id;
                model.ParentId = head5.Id;
                model.AccName = head5.AccName;
                model.AccCode = head5.AccCode;
                model.CompanyId = head5.CompanyId;
                model.LayerNo = head5.LayerNo;
                model.OrderNo = head5.OrderNo;
                model.ParentAccountName = head5.AccCode + "-" + head5.AccName;
                model.Remarks = head5.Remarks;
                model.Status = status;
                return model;
            }
            if (layerNo == 6)
            {
                HeadGL headGL = context.HeadGLs.Where(x => x.Id == accountHeadId).FirstOrDefault();

                model.Id = headGL.Id;
                model.ParentId = headGL.Id;
                model.AccName = headGL.AccName;
                model.AccCode = headGL.AccCode;
                model.CompanyId = headGL.CompanyId;
                model.LayerNo = headGL.LayerNo;
                model.OrderNo = headGL.OrderNo;
                model.ParentAccountName = headGL.AccCode + "-" + headGL.AccName;
                model.Remarks = headGL.Remarks;
                model.Status = status;
               
                return model;
            }
            return model;
        }

        public List<SelectModel> GetTeritorySelectModelsByZone(int companyId, int zoneId)
        {
            return context.Head5.Where(x => x.CompanyId == companyId && x.ParentId == zoneId).ToList().Select(x => new SelectModel()
            {
                Text = "[" + x.AccCode.ToString() + "] " + x.AccName.ToString(),
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
        }

        public List<IntDropDownModel> GetInsertCustomerCode(int vendorTypeId, int companyId)
        {

            if (vendorTypeId == (int)Provider.Customer)
            {
                string query = string.Format(@"select '['+AccCode+'] '+ AccName as [Text], Id as Value from HeadGL
                                         where CompanyId={0} and AccCode like '%1304%' and Id not in (select isnull(HeadGLId,0) from Erp.Vendor)", companyId);
                return context.Database.SqlQuery<IntDropDownModel>(query).ToList();
            }
            if (vendorTypeId == (int)Provider.Supplier)
            {
                string query = string.Format(@"select '['+AccCode+'] '+ AccName as [Text], Id as Value from HeadGL
                                         where CompanyId={0} and AccCode like '%2401%' and Id not in (select isnull(HeadGLId,0) from Erp.Vendor)", companyId);
                return context.Database.SqlQuery<IntDropDownModel>(query).ToList();
            }
            return new List<IntDropDownModel>();
        }

        public List<IntDropDownModel> GetUpdateCustomerCode(int vendorTypeId, int companyId)
        {

            if (vendorTypeId == (int)Provider.Customer)
            {
                string query = string.Format(@"select  '['+AccCode+'] '+ AccName as [Text], Id as Value 
                                               from     HeadGL
                                               where    CompanyId={0} and AccCode like '1304%' --and Id not in (select HeadGLId From Erp.Vendor where VendorTypeId=2)
                                               order by AccName", companyId);
                return context.Database.SqlQuery<IntDropDownModel>(query).ToList();
            }

            if (vendorTypeId == (int)Provider.Supplier)
            {
                string query = string.Format(@"select   '['+AccCode+'] '+ AccName as [Text], Id as Value 
                                               from      HeadGL
                                               where     CompanyId={0} and AccCode like '2401%' --and Id not in (select HeadGLId From Erp.Vendor where VendorTypeId=1)
                                               order by  AccName", companyId);
                return context.Database.SqlQuery<IntDropDownModel>(query).ToList();
            }
            return new List<IntDropDownModel>();
        }

        public async Task<VMAccountHead> GetAccountingHeadList(int companyId)
        {
            VMAccountHead vmAccountHead = new VMAccountHead();
            vmAccountHead.CompanyId = companyId;
            vmAccountHead.DataList = await Task.Run(() => (from t1 in context.HeadGLs
                                                           join t2 in context.Head5 on t1.ParentId equals t2.Id
                                                           join t3 in context.Head4 on t2.ParentId equals t3.Id
                                                           join t4 in context.Head3 on t3.ParentId equals t4.Id
                                                           join t5 in context.Head2 on t4.ParentId equals t5.Id
                                                           join t6 in context.Head1 on t5.ParentId equals t6.Id

                                                           where t1.CompanyId == companyId
                                                           select new VMAccountHead
                                                           {
                                                               Id = t1.Id,
                                                               GLName = t1.AccCode + "-" + t1.AccName,
                                                               Head5Name = t2.AccCode + "-" + t2.AccName,
                                                               Head4Name = t3.AccCode + "-" + t3.AccName,
                                                               Head3Name = t4.AccCode + "-" + t4.AccName,
                                                               Head2Name = t5.AccCode + "-" + t5.AccName,
                                                               Head1Name = t6.AccCode + "-" + t6.AccName,
                                                               CompanyId = t1.CompanyId
                                                           }).OrderByDescending(x => x.Id).AsEnumerable());
            return vmAccountHead;
        }
    }
}
