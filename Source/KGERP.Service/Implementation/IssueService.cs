using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class IssueService : IIssueService
    {

        private readonly ERPEntities context;
        public IssueService(ERPEntities context)
        {
            this.context = context;
        }

        public List<IssueDetailInfoModel> GetRmProducts(int productId, decimal qty)
        {
            var data = context.Database.SqlQuery<IssueDetailInfoModel>("Exec sp_getRmFormula {0},{1}", productId, qty).ToList();
            return data;
        }

        public bool SaveIssueInformation(IssueMasterInfoModel model)
        {
            IssueMasterInfo issue = ObjectConverter<IssueMasterInfoModel, IssueMasterInfo>.Convert(model);

            issue.CreatedDate = DateTime.Now;
            issue.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            context.IssueMasterInfoes.Add(issue);
            return context.SaveChanges() > 0;

        }
    }
}
