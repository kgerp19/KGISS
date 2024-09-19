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
    public class MemberBasePFSummaryService : IMemberBasePFSummaryService
    {
        ERPEntities context = new ERPEntities();
        public List<MemberBasePFSummaryModel> GetMemberBasePFSummaryByEmployeeId(string EmployeeId)
        {
            dynamic result = context.Database.SqlQuery<MemberBasePFSummaryModel>("exec spGetMemberPFSummary {0} ", EmployeeId).ToList();
            return result;
        }
        public List<MemberBasePFSummaryModel> GetPFDetialsByEmployeeId(string EmployeeId)
        {
            dynamic result = context.Database.SqlQuery<MemberBasePFSummaryModel>("exec spGetPFDetialsByEmployeeId {0} ", EmployeeId).ToList();
            return result;
        }

        public MemberBasePFSummaryModel GetPFLastMonthUpdatedByEmployeeId(string EmployeeId)
        {
            dynamic result = context.Database.SqlQuery<MemberBasePFSummaryModel>("exec GetPFLastMonthUpdatedByEmployeeId {0} ", EmployeeId).FirstOrDefault();
            return result;
        }

        public List<MemberBasePFSummaryModel> ExportPFDetialsByEmployeeId(string employeeId)
        {
            dynamic result = context.Database.SqlQuery<MemberBasePFSummaryModel>("exec spExportPFDetialsByEmployeeId {0} ", employeeId).FirstOrDefault();
            return result;
        }


        public bool SaveEmployeePF(int id, MemberBasePFSummaryModel model)
        {
            PfData kGPFData = ObjectConverter<MemberBasePFSummaryModel, PfData>.Convert(model);
            if (id > 0)
            {
                kGPFData.ModifiedDate = DateTime.Now;
                kGPFData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                kGPFData.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kGPFData.PFCreatedDate = DateTime.Now;
            }

            kGPFData.CompanyName = model.CompanyName;
            if (model.SelfContribution != null)
            {
                kGPFData.SelfContribution = model.SelfContribution;
            }
            kGPFData.SelfProfit = model.SelfProfit;
            kGPFData.OfficeContribution = model.CompanyContribution;
            kGPFData.OfficeProfit = model.OfficeProfit;
            kGPFData.ProfitDate = model.ProfitDate;
            kGPFData.PfMonth = model.PfMonth;
            kGPFData.Description = model.Description;
            context.Entry(kGPFData).State = kGPFData.PFDataId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

    }
}
