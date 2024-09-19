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
    public class EducationService : IEducationService
    {
        private readonly ERPEntities context;
        public EducationService(ERPEntities context)
        {
            this.context = context;
        }

        public List<EducationModel> GetEducations(long id)
        {
            return ObjectConverter<Education, EducationModel>.ConvertList(context.Educations.Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Where(x => x.Id == id).OrderByDescending(x => x.PassingYear).ToList()).ToList();
        }

        public EducationModel GetEducation(long id, int educationId)
        {
            if (educationId <= 0)
            {
                return new EducationModel() { Id = id };
            }
            Education education = context.Educations.Find(educationId);
            return ObjectConverter<Education, EducationModel>.Convert(education);
        }

        public bool SaveEducation(int id, EducationModel model, out string message)
        {
            message = string.Empty;

            if (model == null)
            {
                throw new Exception("Education data missing!");
            }

            bool exist = context.Educations.Where(x => x.ExaminationId == model.ExaminationId && x.SubjectId == model.SubjectId && x.InstituteId == model.InstituteId && x.EducationId != id && x.Id==model.Id).Any();

            if (exist)
            {
                message = "Education data already exist";
                return false;
            }

            Education education = ObjectConverter<EducationModel, Education>.Convert(model);
            if (id > 0)
            {
                education = context.Educations.Where(x => x.EducationId == id).FirstOrDefault();
                if (education == null)
                {
                    throw new Exception("Education not found!");
                }
                education.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                education.ModifiedDate = DateTime.Now;
            }

            else
            {
                education.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                education.CreatedDate = DateTime.Now;

            }

            if (!string.IsNullOrEmpty(model.CertificateName))
            {
                education.CertificateName = model.CertificateName;
            }

            education.Id = model.Id;
            education.ExaminationId = model.ExaminationId;
            education.SubjectId = model.SubjectId;
            education.InstituteId = model.InstituteId;
            education.PassingYear = model.PassingYear;
            education.RollNo = model.RollNo;
            education.RegNo = model.RegNo;
            education.Result = model.Result;
            education.Remarks = model.Remarks;
            education.IsActive = true;

            context.Entry(education).State = education.EducationId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteEducation(long id, int educationId)
        {
            Education education = context.Educations.FirstOrDefault(x => x.EducationId == educationId);
            if (education == null)
            {
                throw new Exception("Education not found !");
            }

            context.Educations.Remove(education);
            return context.SaveChanges() > 0;

        }

        public string GetCertificateName(int educationId)
        {
            return context.Educations.FirstOrDefault(x => x.EducationId == educationId).CertificateName;
        }
    }
}
