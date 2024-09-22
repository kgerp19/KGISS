using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KGERP.Service.Implementation
{
    public class ECMemberService : IECMemberService
    {
        private readonly ERPEntities context;
        public ECMemberService(ERPEntities context)
        {
            this.context = context;
        }

        public ECMemberModel GetECMember(int id)
        {
            if (id <= 0)
            {
                return new ECMemberModel()
                {
                    IsActive = true,
                    MemberImage = "default.png"
                };
            }
            ECMember member = context.ECMembers.FirstOrDefault(x => x.ECMemberId == id);
            return ObjectConverter<ECMember, ECMemberModel>.Convert(member);
        }

        public List<ECMemberModel> GetECMembers(string searchText)
        {
            IQueryable<ECMember> members = context.ECMembers.Where(x => x.IsActive && (x.MemberName.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).OrderBy(x => x.MemberOrder);
            return ObjectConverter<ECMember, ECMemberModel>.ConvertList(members.ToList()).ToList();
        }

        public bool SaveECMember(int id, ECMemberModel model)
        {
            HttpPostedFileBase ss = model.MemberImageUpload;
            if (model == null)
            {
                throw new Exception("EC Member data missing!");
            }

            bool exist = context.ECMembers.Where(x => x.MemberName.ToLower().Equals(model.MemberName.ToLower()) && x.StartDate == model.StartDate && x.ECMemberId != id).Any();

            if (exist)
            {
                throw new Exception("EC Member already exist!");
            }
            ECMember eCMember = ObjectConverter<ECMemberModel, ECMember>.Convert(model);
            if (id > 0)
            {
                eCMember = context.ECMembers.FirstOrDefault(x => x.ECMemberId == id);
                if (eCMember == null)
                {
                    throw new Exception("EC Member not found!");
                }
                //eCMember.ModifiedDate = DateTime.Now;
                //eCMember.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                //    eCMember.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //    eCMember.CreatedDate = DateTime.Now;
                eCMember.IsActive = true;
            }
            eCMember.MemberName = model.MemberName;
            eCMember.MemberOrder = model.MemberOrder;
            eCMember.StartDate = model.StartDate;
            eCMember.EndDate = model.EndDate;
            eCMember.Phone = model.Phone;
            eCMember.Email = model.Email;
            eCMember.Remarks = model.Remarks;
            if (ss != null) {
                eCMember.MemberImage = model.MemberImage;
            }
 

            context.Entry(eCMember).State = eCMember.ECMemberId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }
    }
}
