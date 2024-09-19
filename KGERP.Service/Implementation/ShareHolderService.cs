using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KGERP.Service.Implementation
{
    public class ShareHolderService : IShareHolderService
    {
        private readonly ERPEntities context;
        public ShareHolderService(ERPEntities context)
        {
            this.context = context;
        }
        public IQueryable<ShareHolderModel> GetAllShareHolders(string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(ShareHolderId) from ShareHolder").FirstOrDefault();
            return context.Database.SqlQuery<ShareHolderModel>(@"exec spGetShareHolderSearch {0}", searchValue).AsQueryable();
        }




        public async Task<ShareHolderModel> GetShareHolders(int companyId)
        {
            ShareHolderModel shareHolderModel = new ShareHolderModel();
            shareHolderModel.CompanyId = companyId;
            shareHolderModel.DataList = await Task.Run(() => (from t1 in context.ShareHolders
                                                             join t2 in context.DropDownItems on t1.ShareHolderTypeId equals t2.DropDownItemId    
                                                             where t1.CompanyId == companyId    
                                                             select new ShareHolderModel
                                                             {
                                                                 ShareHolderId = t1.ShareHolderId,
                                                                 ShareHolderTypeId = t1.ShareHolderTypeId,
                                                                 ShareHolderType = t2.Name,
                                                                 Name = t1.Name,
                                                                 DateOfBirth = t1.DateOfBirth,
                                                                 NID = t1.NID,

                                                                 CompanyId = t1.CompanyId,
                                                                 Phone = t1.Phone,
                                                                 PresentAddress = t1.PresentAddress,
                                                                 IsActive = t1.IsActive,
                                                                 ShareHolderImage=t1.ShareHolderImage
                                                             }).OrderByDescending(o => o.ShareHolderId).AsEnumerable());
           

            return shareHolderModel;  
        }

        public bool BulkSave(List<ShareHolderModel> models)
        {
            if (!models.Any())
            {
                return false;
            }
            List<ShareHolder> shareHolders = ObjectConverter<ShareHolderModel, ShareHolder>.ConvertList(models.ToList()).ToList();
            context.ShareHolders.AddRange(shareHolders);
            return context.SaveChanges() > 0;
        }

        public ShareHolderModel GetShareHolder(int id)
        {

            ShareHolder shareHolder = context.ShareHolders.Where(x => x.ShareHolderId == id).FirstOrDefault();

            if (shareHolder == null)
            {
                return new ShareHolderModel
                {
                    IsActive = true,
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"])
                };
            }

            shareHolder.ShareHolderImage = shareHolder.ShareHolderImage ?? "default.png";
            shareHolder.NIDImage = shareHolder.NIDImage ?? "default.png";

            return ObjectConverter<ShareHolder, ShareHolderModel>.Convert(shareHolder);
        }

        public bool SaveShareHolder(long id, ShareHolderModel model, out string message)
        {
            message = string.Empty;
            ShareHolder shareHolder = ObjectConverter<ShareHolderModel, ShareHolder>.Convert(model);
            if (id > 0)
            {
                shareHolder = context.ShareHolders.Where(x => x.ShareHolderId == id).FirstOrDefault();
                if (shareHolder == null)
                {
                    throw new Exception("Share Holder not found!");
                }
                if (!string.IsNullOrEmpty(model.ShareHolderImage))
                {
                    shareHolder.ShareHolderImage = model.ShareHolderImage;
                }
                if (!string.IsNullOrEmpty(model.NIDImage))
                {
                    shareHolder.NIDImage = model.NIDImage;
                }
                shareHolder.ModifiedDate = DateTime.Now;
                shareHolder.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                shareHolder.ShareHolderImage = model.ShareHolderImage;
                shareHolder.NIDImage = model.NIDImage;
                shareHolder.CreatedDate = DateTime.Now;
                shareHolder.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }


            shareHolder.CompanyId = model.CompanyId;
            shareHolder.Name = model.Name;
            shareHolder.GenderId = model.GenderId;
            shareHolder.NID = model.NID;
            shareHolder.DateOfBirth = model.DateOfBirth;
            shareHolder.StartDate = model.StartDate;
            shareHolder.Phone = model.Phone;
            shareHolder.Email = model.Email;
            shareHolder.PresentAddress = model.PresentAddress;
            shareHolder.PermanentAdress = model.PermanentAdress;
            shareHolder.FatherName = model.FatherName;
            shareHolder.MotherName = model.MotherName;
            shareHolder.Spouse = model.Spouse;
            shareHolder.HomePhone = model.HomePhone;
            shareHolder.OfficePhone = model.OfficePhone;
            shareHolder.Fax = model.Fax;
            shareHolder.EducationQualificationId = model.EducationQualificationId;
            shareHolder.ProfessionId = model.ProfessionId;
            shareHolder.Organization = model.Organization;
            shareHolder.Designation = model.Designation;
            shareHolder.NoOfShare = model.NoOfShare;
            shareHolder.Amount = model.Amount;
            shareHolder.Nominee = model.Nominee;
            shareHolder.TIN = model.TIN;
            shareHolder.IsActive = model.IsActive;


            context.Entry(shareHolder).State = shareHolder.ShareHolderId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        //public bool GLBulkSave(List<AccGLModel> models)
        //{
        //    if (!models.Any())
        //    {
        //        return false;
        //    }
        //    List<AccGL> accGLs = ObjectConverter<AccGLModel, AccGL>.ConvertList(models.ToList()).ToList();
        //    context.AccGLs.AddRange(accGLs);
        //    return context.SaveChanges() > 0;
        //}
    }
}
