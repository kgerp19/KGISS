using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.ComparativeStatementService.Comparative_Statement
{
    public class ComparativeStatementService : IComparativeStatementService
    {
        ERPEntities context = new ERPEntities();
        public ComparativeStatementVm GetCS(int CompanyId)
        {
            ComparativeStatementVm vm = new ComparativeStatementVm();
            vm.ComparetiveStatemnetList = (from CS in context.ComparativeStatements
                                           join p in context.Products on CS.ProductID equals p.ProductId
                                           where CS.IsActive && CS.CompanyId == CompanyId
                                           select new ComparativeStatementVm
                                           {
                                               ProductID = CS.ProductID,
                                               CSID = CS.CSID,
                                               CSNO = CS.CSNO,
                                               CSDate = CS.CSDate,
                                               DeliveryDate = CS.DeliveryDate,
                                               PrductName = p.ProductName,
                                               Brand = CS.Brand,
                                               Origin = CS.Origin,
                                               RequiredQuantity = CS.RequiredQuantity,
                                               Specification = CS.Specification,
                                               CreatedBy = CS.CreatedBy,
                                               Status = CS.Status,
                                               CompanyId = CS.CompanyId
                                           }).OrderByDescending(c=>c.CSID).ToList();



            return vm;
        }

        public string GetCSNO(int CompanyId)
        {
            var obj = context.ComparativeStatements.Where(y => y.CompanyId == CompanyId)
                  .Select(x => x.CSNO)
                  .OrderByDescending(x => x)
                  .FirstOrDefault();

            if (obj != null)
            {
                string numericPart = obj.Substring(3);
                int numericValue = int.Parse(numericPart);
                numericValue++;
                obj = "CHD" + numericValue.ToString("D5");

            }
            else
            {
                obj = "CHD00001";
            }



            return obj;
        }
        public object GetProductsByCompany(int CompanyId, string prefix, string type)
        {
            var v = (from p in context.Products
                     join pc in context.ProductCategories on p.ProductCategoryId equals pc.ProductCategoryId
                     join psc in context.ProductSubCategories on p.ProductSubCategoryId equals psc.ProductSubCategoryId
                     where p.CompanyId == CompanyId && p.IsActive && p.ProductType == type && (p.ProductName.Contains(prefix))
                     select new
                     {
                         label = pc.Name + "-" + psc.Name + "-" + p.ProductName,
                         val = p.ProductId
                     }).Take(50).ToList();

            return v;
        }

        public object GetSupplier(int CompanyId, string prefix)
        {
            var v = (from p in context.Vendors

                     where p.CompanyId == CompanyId && p.IsActive && p.VendorTypeId == 2 && (p.Name.Contains(prefix))
                     select new
                     {
                         label = p.Name,
                         val = p.VendorId
                     }).Take(50).ToList();

            return v;
        }


        public long SaveComparativeStateMent(ComparativeStatementVm Model)
        {
            long savedCSID = -1;
            var obj = context.ComparativeStatements.Where(x => x.CSID == Model.CSID).FirstOrDefault();
            if (obj == null)
            {
                try
                {
                    ComparativeStatement comparativeStatement = new ComparativeStatement
                    {
                        CSNO = Model.CSNO,
                        CSDate = Model.CSDate,
                        DeliveryDate = Model.DeliveryDate,
                        ProductID = Model.ProductID,
                        Brand = Model.Brand,
                        Origin = Model.Origin,
                        RequiredQuantity = Model.RequiredQuantity,
                        Specification = Model.Specification,
                        CompanyId = Model.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    context.ComparativeStatements.Add(comparativeStatement);
                    context.SaveChanges();
                    savedCSID = comparativeStatement.CSID;

                }
                catch (Exception ex)
                {
                    var message = ex;
                }
                return savedCSID;
            }
            else
            {
                obj.CSDate = DateTime.Now;
                obj.DeliveryDate = DateTime.Now;
                obj.ProductID = Model.ProductID;
                obj.Brand = Model.Brand;
                obj.Origin = Model.Origin;
                obj.RequiredQuantity = Model.RequiredQuantity;
                obj.Specification = Model.Specification;
                context.SaveChanges();
            }
            return obj.CSID;
        }
        public long SaveComparativeStateMentDetails(ComparativeStatementVm Model)
        {
            long savedCSDetailID = -1;
            var obj = context.ComparativeStatementDetails.Where(x => x.CSDetailID == Model.DetaliesObjectVm.CSDetailID).FirstOrDefault();
            if (obj == null)
            {
                try
                {
                    ComparativeStatementDetail comparativeStatementDetail = new ComparativeStatementDetail
                    {
                        CSID = Model.CSID,
                        QuotedPrice = Model.DetaliesObjectVm.QuotedPrice,
                        SupplierID = Model.DetaliesObjectVm.SupplierID,
                        Remarks = Model.DetaliesObjectVm.Remarks,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    context.ComparativeStatementDetails.Add(comparativeStatementDetail);
                    context.SaveChanges();
                    savedCSDetailID = comparativeStatementDetail.CSDetailID;

                }
                catch (Exception ex)
                {
                    var message = ex;
                }
                return savedCSDetailID;
            }

            return obj.CSID;
        }
        public bool EditComparativeStateMentDetails(ComparativeStatementDetailVm Model)
        {

            var obj = context.ComparativeStatementDetails.Where(x => x.CSDetailID == Model.CSDetailID).FirstOrDefault();
            if (obj != null)
            {
                obj.QuotedPrice = Model.QuotedPrice;
                obj.Remarks = Model.Remarks;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();

                if (context.SaveChanges() > 0)
                {
                    return true;
                }


            }

            return false;
        }
        public ComparativeStatementVm GetComparativeStatement(long? CsId)
        {
            ComparativeStatementVm vm = new ComparativeStatementVm();
            vm = (from CS in context.ComparativeStatements
                  join p in context.Products on CS.ProductID equals p.ProductId
                  where CS.IsActive && CS.CSID == CsId
                  select new ComparativeStatementVm
                  {
                      ProductID = CS.ProductID,
                      CSID = CS.CSID,
                      CSNO = CS.CSNO,
                      CSDate = CS.CSDate,
                      DeliveryDate = CS.DeliveryDate,
                      PrductName = p.ProductName,
                      Brand = CS.Brand,
                      Origin = CS.Origin,
                      RequiredQuantity = CS.RequiredQuantity,
                      Specification = CS.Specification,
                      CreatedBy = CS.CreatedBy,
                      Status = CS.Status,
                  }).FirstOrDefault();
            int x = 0;

            vm.ComparativeStatementDetailvmList = (from CSD in context.ComparativeStatementDetails
                                                   join v in context.Vendors on CSD.SupplierID equals v.VendorId
                                                   where CSD.IsActive && CSD.CSID == CsId
                                                   select new ComparativeStatementDetailVm
                                                   {
                                                       CSDetailID = CSD.CSDetailID,
                                                       SupplierID = v.VendorId,
                                                       VendorName = v.Name,
                                                       QuotedPrice = CSD.QuotedPrice,
                                                       Remarks = CSD.Remarks,
                                                       Recommendation = CSD.Recommendation,

                                                   }).ToList();

            vm.RADataList = (from a in context.SignatoryApprovalMaps
                             join emp in context.Employees on a.EmployeeId equals emp.Id
                             join des in context.Designations on emp.DesignationId equals des.DesignationId
                            
                             where a.IntregratedFromId == CsId && a.TableName == "ComparativeStatement"
                             select new RequisitionApprovalVM
                             {
                                 EmployeeId = emp.Id,
                                 EmployeeIdString = emp.EmployeeId,
                                 EmployeeName = emp.Name,
                                 DesignationName = des.Name,
                                 OrderBy = a.OrderBy,
                                 Comment = a.Comment,
                                 Status = (SignatoryStatusEnum)a.Status,
                                 StatusString = ((SignatoryStatusEnum)a.Status).ToString() ,


                                 ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                             }).AsEnumerable();


            return vm;
        }



            public IEnumerable<RequisitionApprovalVM> GetSignatureList(int companyId, DateTime?fromDate, DateTime? toDate, long?userId, SignatoryStatusEnum? approvalStatus)
        {
            var obj= (from a in context.SignatoryApprovalMaps
                                join emp in context.Employees on a.EmployeeId equals emp.Id
                                join des in context.Designations on emp.DesignationId equals des.DesignationId
                                join com in context.ComparativeStatements on a.IntregratedFromId equals  com.CSID
                                join Pro in context.Products on com.ProductID equals Pro.ProductId
                                where a.EmployeeId == userId && a.TableName == "ComparativeStatement"
                               && a.IsActive           
                             && (
                             (approvalStatus == null && a.Status == (int)SignatoryStatusEnum.Pending)
                             || (approvalStatus != null && approvalStatus.HasValue && (SignatoryStatusEnum)a.Status == approvalStatus)
                             )
                             && ((fromDate == null || toDate == null) || com.CSDate >= fromDate && com.CSDate <= toDate)
                                select new RequisitionApprovalVM
                                {   IntregratedFromId=a.IntregratedFromId,
                                    CSNO=com.CSNO,
                                    CSDate=com.CSDate,
                                    ProductName=Pro.ProductName,
                                    RequirQuantity=com.RequiredQuantity,
                                    EmployeeId = emp.Id,
                                    EmployeeIdString = emp.EmployeeId,
                                    EmployeeName = emp.Name,
                                    DesignationName = des.Name,
                                    OrderBy = a.OrderBy,
                                    Comment = a.Comment,
                                    CompanyId=com.CompanyId,
                                    Status = (SignatoryStatusEnum)a.Status,
                                    StatusString = ((SignatoryStatusEnum)a.Status).ToString(),
                                    ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                                }).AsEnumerable();


            return obj;
        }


        public IEnumerable<RequisitionApprovalVM> GetAllApproval(int CSID)
        {


            var obj = (from a in context.SignatoryApprovalMaps
                       join emp in context.Employees on a.EmployeeId equals emp.Id
                       join des in context.Designations on emp.DesignationId equals des.DesignationId

                       where a.IntregratedFromId == CSID && a.TableName == "ComparativeStatement"
                       select new RequisitionApprovalVM
                       {
                           EmployeeId = emp.Id,
                           EmployeeIdString = emp.EmployeeId,
                           EmployeeName = emp.Name,
                           DesignationName = des.Name,
                           OrderBy = a.OrderBy,
                           Comment = a.Comment,
                           Status = (SignatoryStatusEnum)a.Status,
                           StatusString = ((SignatoryStatusEnum)a.Status).ToString(),


                           ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                       }).AsEnumerable();


            return obj;


        }






            [HttpPost]
        public bool DeleteComparativeStateMentDetails(long csDetailID)
        {

            var obj = context.ComparativeStatementDetails.Where(x => x.CSDetailID == csDetailID).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

            return false;
        }

        [HttpPost]
        public bool MakeRecomended(long csDetailID)
        {

            var obj = context.ComparativeStatementDetails.Where(x => x.CSDetailID == csDetailID).FirstOrDefault();
            if (obj != null)
            {
                var query = context.ComparativeStatementDetails.Where(x => x.CSID == obj.CSID && x.IsActive).ToList();
                if (query.Any())
                {
                    foreach (var statement in query)
                    {
                        statement.Recommendation = false;

                    }
                }
                obj.Recommendation = true;
                context.SaveChanges();
                return true;

            }
            else
            {
                return false;
            }

            return false;
        }


        public bool Submitstatus(ComparativeStatementVm vm)
        {
            var obj = context.ComparativeStatements.Where(x => x.CSID == vm.CSID).FirstOrDefault();
            if (obj != null)
            {
                obj.Status = true;
                if (context.SaveChanges() > 0)
                {

                    List<RequisitionSignatory> requisitionSignatorieList = context.RequisitionSignatories.Where(x => x.CompanyId == obj.CompanyId && x.IsActive && x.IntegrateWith == "ComparativeStatement").ToList();

                    List<SignatoryApprovalMap> signatoryApprovalMapList = new List<SignatoryApprovalMap>();

                    foreach (var item in requisitionSignatorieList)
                    {
                        SignatoryApprovalMap signatoryApprovalMap = new SignatoryApprovalMap
                        {
                            
                            EmployeeId = item.EmployeeId,
                            IntregratedFromId = obj.CSID,
                            IsActive = true,
                            Status = 0,
                            TableName = "ComparativeStatement",
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                            CreatedDate = DateTime.Now,

                        };
                        signatoryApprovalMapList.Add(signatoryApprovalMap);

                    }
                    context.SignatoryApprovalMaps.AddRange(signatoryApprovalMapList);
                    context.SaveChanges();
                }

                return true;

            }


            return false;
        }
        public ComparativeStatementVm GetForEdit(long? CSID)
        {
            ComparativeStatementVm comparativeStatement = new ComparativeStatementVm();
            comparativeStatement = (from CS in context.ComparativeStatements
                                    join p in context.Products on CS.ProductID equals p.ProductId
                                    where CS.IsActive && CS.CSID == CSID
                                    select new ComparativeStatementVm
                                    {
                                        ProductID = CS.ProductID,
                                        CSID = CS.CSID,
                                        CSNO = CS.CSNO,
                                        CSDate = CS.CSDate,
                                        DeliveryDate = CS.DeliveryDate,
                                        PrductName = p.ProductName,
                                        Brand = CS.Brand,
                                        Origin = CS.Origin,
                                        RequiredQuantity = CS.RequiredQuantity,
                                        Specification = CS.Specification,
                                        CreatedBy = CS.CreatedBy,
                                        Status = CS.Status,
                                        CompanyId = CS.CompanyId,
                                    }).FirstOrDefault();



            return comparativeStatement;
        }
        public bool Deletecs(long? CSID)
        {
            var obj = context.ComparativeStatements.Where(x => x.CSID == CSID).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                context.SaveChanges();
                return true;

            }


            return false;
        }
        public  bool UpdateCSSignatoryApprovalStatus(long id, int statusId, string comment)
        {
            var exist =  context.SignatoryApprovalMaps.FirstOrDefault(x => x.IntregratedFromId == id);
            if (exist != null)
            {
                exist.Status = statusId;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.Comment = comment;
            }
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }


    }
}
