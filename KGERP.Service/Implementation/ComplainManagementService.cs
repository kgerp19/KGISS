using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace KGERP.Service.Implementation
{
    public class ComplainManagementService : IComplainManagementService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public ComplainManagementService(ERPEntities context)
        {
            this.context = context;
        }
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

        public (List<ComplainManagementModel> complain, int totalRows) GetAllComplain(int start, int length, string searchValue, string sortColumnName, string sortDirection, int companyId)
        {

            var totalRows = context.ComplainManagements.Count();

            List<ComplainManagementModel> complain = context.Database.SqlQuery<ComplainManagementModel>(@"select        ComplainId, CustomerName,Address,MobileNo,InvoiceNo,ComplainDescription,ComplainTypeId,ActionDescription,
                                                                            isnull(replace(convert(NVARCHAR, InvoiceDate, 105), ' ', '/'),'') as OrderDate,
                                                                            isnull(replace(convert(NVARCHAR, CreatedDate, 105), ' ', '/'),'') as ComplainDate,
																			
																		    isnull((select Name from Employee where EmployeeId=ComplainManagement.CreatedBy),'') as CreatedBy,
                                                                            isnull((select Description from ComplainType where ComplainTypeId=ComplainManagement.ComplainTypeId),'') as ComplainTypeName,
																			ActionDescription,ActionAssignTo,SolvingDescription,ActionTakenBy,IsActionTaked ,
                                                                            Case when IsActionTaked=0 then  'New' when  IsActionTaked=1 then 'Open' when IsActionTaked=2 then 'Solved' else 'Closed' end as ComplainStatus
                                                              from          ComplainManagement
                                                              order by      IsActionTaked").ToList();

            return (complain, totalRows);

        }

        public List<SelectItemList> GetComplainType(int companyId)
        {
            return context.ComplainTypes.Select(x => new SelectItemList()
            {
                Text = x.Description.ToString(),
                Value = x.ComplainTypeId
            }).ToList();
        }



        public bool SaveOrEdit(ComplainManagementModel model)
        {
            //var complain = ObjectConverter<ComplainManagementModel, ComplainManagement>.Convert(model);
            ComplainManagement complain = context.ComplainManagements.Where(x => x.ComplainId == model.ComplainId).FirstOrDefault();


            if (complain == null)
            {
                complain = new ComplainManagement();
                complain.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                complain.CreatedDate = DateTime.Now;


            }
            else
            {
                complain.ActionTakerBy = System.Web.HttpContext.Current.User.Identity.Name;
                complain.ActionTakerDate = DateTime.Now;
                complain.IsActionTaked = 1;
            }
            complain.CustomerName = model.CustomerName;
            complain.Address = model.Address;
            complain.MobileNo = model.MobileNo;
            complain.ComplainTypeId = model.ComplainTypeId;
            complain.InvoiceNo = model.InvoiceNo;
            complain.InvoiceDate = model.InvoiceDate;
            complain.ComplainDescription = model.ComplainDescription;
            complain.ActionAssignTo = model.ActionAssignTo;
            complain.ActionDescription = model.ActionDescription;
            complain.IsComplainClosed = model.IsComplainClosed;
            complain.SolvingDescription = model.SolvingDescription;
            complain.Remarks = model.Remarks;
            complain.IsComplainSolved = model.IsComplainSolved;
            if (complain.IsComplainSolved)
            {
                complain.IsActionTaked = 2;
            }
            if (complain.IsComplainClosed)
            {
                complain.IsActionTaked = 3;
            }


            context.Entry(complain).State = complain.ComplainId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;


        }

        public ComplainManagementModel GetComplain(int id)
        {
            if (id == 0)
            {
                ComplainManagementModel model = new ComplainManagementModel();
                return model;
            }
            else
            {
                var complain = context.ComplainManagements.Where(x => x.ComplainId == id).FirstOrDefault();
                return ObjectConverter<ComplainManagement, ComplainManagementModel>.Convert(complain);
            }
        }

        public bool DeleteComplain(int id)
        {
            ComplainManagement complain = context.ComplainManagements.Find(id);
            if (complain == null)
            {
                return false;
            }
            context.ComplainManagements.Remove(complain);
            return context.SaveChanges() > 0;
        }
    }
}

