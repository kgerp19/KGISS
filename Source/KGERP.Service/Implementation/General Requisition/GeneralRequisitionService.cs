using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KGERP.Service.Implementation.General_Requisition
{
    public class GeneralRequisitionService : IGeneralRequisitionService
    {
        private readonly ERPEntities _context;
        public GeneralRequisitionService(ERPEntities eRPEntities)
        {
            _context = eRPEntities;
        }
        public async Task<bool> Add(GeneralRequisitionMasterVM generalRequisitionMasterVM)
        {
            GeneralRequisition generalRequisition = new GeneralRequisition()
            {
                RequisitionDate = generalRequisitionMasterVM.RequisitionDate,
                RequisitionNumber = generalRequisitionMasterVM.RequisitionNumber,
                Remarks = generalRequisitionMasterVM.Remarks,
                GRequisitionCategoryId = generalRequisitionMasterVM.CategoryId,
                GRequisitionType = (int)generalRequisitionMasterVM.GeneralRequisitionType,
                CompanyId = generalRequisitionMasterVM.UserCompanyId,
                DivisionId = generalRequisitionMasterVM.DivisionId,
                Status = (int)GeneralRequisitionStatusEnum.Draft,
                AITParcent = generalRequisitionMasterVM.AITParcent,
                DiscountParcent = generalRequisitionMasterVM.DiscountParcent,

                IsActive = true,
                CreatedBy = generalRequisitionMasterVM.CreatedBy,
                CreatedDate = generalRequisitionMasterVM.CreatedDate,
                RequisitionToCompanyId = generalRequisitionMasterVM.RequisitionToCompanyId,
                EmployeeId = generalRequisitionMasterVM.EemployeeId,
                IsAsset = generalRequisitionMasterVM.IsAsset,
                ProjectId = generalRequisitionMasterVM.ProjectId != null && generalRequisitionMasterVM.ProjectId > 0 ? generalRequisitionMasterVM.ProjectId : null
            };
            _context.GeneralRequisitions.Add(generalRequisition);
            int status = await _context.SaveChangesAsync();
            if (status > 0 && generalRequisitionMasterVM.TotalPrice > 0 && generalRequisitionMasterVM.UnitPrice > 0 && !string.IsNullOrEmpty(generalRequisitionMasterVM.ProductName))
            {
                generalRequisitionMasterVM.Id = generalRequisition.Id;
                var childStatus = await AddRequisitionItemDetail(generalRequisitionMasterVM);
            }
            if (status > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<GeneralRequisitionMasterVM> Get(int id)
        {
            var requisition = (from r in _context.GeneralRequisitions
                               join e in _context.Employees on r.EmployeeId equals e.Id
                               join c in _context.Companies on r.CompanyId equals c.CompanyId

                               where r.IsActive && r.Id == id
                               select new GeneralRequisitionMasterVM()
                               {
                                   AITParcent = r.AITParcent,
                                   DiscountParcent = r.DiscountParcent,
                                   Id = r.Id,
                                   RequisitionToCompanyId = r.RequisitionToCompanyId ?? null,
                                   CompanyName = r.RequisitionToCompanyId == null ? "" : (from c in _context.Companies
                                                                                          where c.CompanyId == r.RequisitionToCompanyId
                                                                                          select c.ShortName).FirstOrDefault(),
                                   DivisionId = r.DivisionId,
                                   DivisionName = r.DivisionId == null ? "" : (from c in _context.Departments
                                                                               where c.DepartmentId == r.DivisionId
                                                                               select c.Name).FirstOrDefault(),
                                   CategoryId = r.GRequisitionCategoryId ?? 0,
                                   CategoryName = (from c in _context.GeneralRequisitionProductCategories
                                                   where c.Id == r.GRequisitionCategoryId
                                                   select c.CategoryName).FirstOrDefault(),
                                   IsAsset = r.IsAsset,
                                   ProjectName = r.ProjectId.HasValue ? _context.Accounting_CostCenter.FirstOrDefault(x => x.CostCenterId == r.ProjectId.Value).Name : "---",
                                   UserCompanyId = r.CompanyId,
                                   UserCompanyName = c.Name,
                                   CreatedBy = e.Name + " (" + r.CreatedBy + ")",
                                   CreatedDate = r.CreatedDate,
                                   Status = (GeneralRequisitionStatusEnum)r.Status,
                                   ModifiedBy = r.ModifiedBy,
                                   ModifiedDate = r.ModifiedDate,
                                   Remarks = r.Remarks,
                                   RequisitionDate = r.RequisitionDate,
                                   RequisitionNumber = r.RequisitionNumber,
                                   GeneralRequisitionType = (BusinessTypeEnum)r.GRequisitionType,
                                   RequisitionProductList = (
                                     from item in _context.GeneralRequisitionDetails
                                     join u in _context.Units on item.UnitId equals u.UnitId
                                     where item.RequisitionId == r.Id && item.IsActive
                                     select new GeneralRequisitionMasterDetailsVM()
                                     {
                                         Id = item.Id,
                                         UnitId = item.UnitId,
                                         UniName = u.Name,
                                         UnitPrice = item.UnitPrice,
                                         CreatedBy = item.CreatedBy,
                                         CreatedDate = item.CreatedDate,
                                         Quantity = item.Quantity,
                                         RequisitionId = item.RequisitionId,
                                         ProductName = item.ProductName,
                                         DiscountProductParcent = item.DiscountParcent
                                     }).ToList()
                               }).FirstOrDefault();
            return requisition;

        }

        public async Task<IEnumerable<GeneralRequisitionMasterVM>> GetAllAsync(int? companyId, DateTime? fromDate, DateTime? toDate, GeneralRequisitionStatusEnum? status)
        {
            var userId = Common.GetIntUserId();
            var requisitionList = (from r in _context.GeneralRequisitions
                                   join e in _context.Employees on r.EmployeeId equals e.Id
                                   join userCompany in _context.Companies on r.CompanyId equals userCompany.CompanyId
                                   join category in _context.GeneralRequisitionProductCategories on r.GRequisitionCategoryId equals category.Id
                                   where r.IsActive && r.EmployeeId == userId
                                   && (fromDate == null || r.RequisitionDate >= fromDate)
                                   && (toDate == null || r.RequisitionDate <= toDate)
                                   && (status == null || (GeneralRequisitionStatusEnum)r.Status == status)
                                   select new GeneralRequisitionMasterVM()
                                   {
                                       AITParcent = r.AITParcent,
                                       DiscountParcent = r.DiscountParcent,
                                       Id = r.Id,
                                       RequisitionToCompanyId = r.RequisitionToCompanyId,
                                       CompanyName = (r.RequisitionToCompanyId != null && r.RequisitionToCompanyId > 0) ? (_context.Companies.FirstOrDefault(x => x.CompanyId == r.RequisitionToCompanyId).ShortName) : null,
                                       DivisionId = r.DivisionId,
                                       DivisionName = (r.DivisionId != null && r.DivisionId > 0) ? (_context.Departments.FirstOrDefault(x => x.DepartmentId == r.DivisionId).Name) : null,
                                       CategoryId = (int)r.GRequisitionCategoryId,
                                       CategoryName = category.CategoryName,
                                       UserCompanyId = r.CompanyId,
                                       UserCompanyName = userCompany.ShortName,
                                       Remarks = r.Remarks,
                                       RequisitionDate = r.RequisitionDate,
                                       GeneralRequisitionType = (BusinessTypeEnum)r.GRequisitionType,
                                       RequisitionNumber = r.RequisitionNumber,
                                       Status = (GeneralRequisitionStatusEnum)r.Status,
                                       IsAsset = r.IsAsset,
                                       ProjectId = r.ProjectId,
                                       ProjectName = r.ProjectId.HasValue ? _context.Accounting_CostCenter.FirstOrDefault(x => x.CostCenterId == r.ProjectId.Value).Name : "---"
                                   }).OrderByDescending(x => x.Id).ToListAsync();
            return await requisitionList;
        }


        public async Task<IEnumerable<GeneralRequisitionMasterVM>> GetAllRequisitionAsync(int? companyId, DateTime? fromDate, DateTime? toDate, int? Status)
        {

            var requisitionList = (from r in _context.GeneralRequisitions
                                   join e in _context.Employees on r.EmployeeId equals e.Id
                                   join userCompany in _context.Companies on r.CompanyId equals userCompany.CompanyId
                                   join category in _context.GeneralRequisitionProductCategories on r.GRequisitionCategoryId equals category.Id
                                   where r.IsActive && r.RequisitionToCompanyId == companyId
                                   && (fromDate == null || r.RequisitionDate >= fromDate)
                                   && (toDate == null || r.RequisitionDate <= toDate)
                                   && (Status == 0 || r.Status == Status)


                                   select new GeneralRequisitionMasterVM()
                                   {
                                       AITParcent = r.AITParcent,
                                       DiscountParcent = r.DiscountParcent,
                                       Id = r.Id,
                                       RequisitionToCompanyId = r.RequisitionToCompanyId,
                                       CompanyName = (r.RequisitionToCompanyId != null && r.RequisitionToCompanyId > 0) ? (_context.Companies.FirstOrDefault(x => x.CompanyId == r.RequisitionToCompanyId).ShortName) : null,
                                       DivisionId = r.DivisionId,
                                       DivisionName = (r.DivisionId != null && r.DivisionId > 0) ? (_context.Departments.FirstOrDefault(x => x.DepartmentId == r.DivisionId).Name) : null,
                                       CategoryId = (int)r.GRequisitionCategoryId,
                                       CategoryName = category.CategoryName,
                                       UserCompanyId = r.CompanyId,
                                       UserCompanyName = userCompany.ShortName,
                                       Remarks = r.Remarks,
                                       RequisitionDate = r.RequisitionDate,
                                       GeneralRequisitionType = (BusinessTypeEnum)r.GRequisitionType,
                                       RequisitionNumber = r.RequisitionNumber,
                                       Status = (GeneralRequisitionStatusEnum)r.Status,
                                       IsAsset = r.IsAsset,
                                       ProjectId = r.ProjectId,
                                       ProjectName = r.ProjectId.HasValue ? _context.Accounting_CostCenter.FirstOrDefault(x => x.CostCenterId == r.ProjectId.Value).Name : "---"
                                   }).OrderByDescending(x => x.Id).ToListAsync();
            return await requisitionList;
        }


        public IEnumerable<GeneralRequisitionMasterVM> GetAll(int? companyId, DateTime? fromDate, DateTime? toDate)
        {

            var requisitionList = (from r in _context.GeneralRequisitions
                                   join e in _context.Employees on r.EmployeeId equals e.Id
                                   join userCompany in _context.Companies on r.CompanyId equals userCompany.CompanyId
                                   join category in _context.GeneralRequisitionProductCategories on r.GRequisitionCategoryId equals category.Id
                                   where r.IsActive && (companyId == null || r.CompanyId == companyId)
                                   && (fromDate == null || r.RequisitionDate >= fromDate)
                                   && (toDate == null || r.RequisitionDate <= toDate)
                                   select new GeneralRequisitionMasterVM()
                                   {
                                       AITParcent = r.AITParcent,
                                       DiscountParcent = r.DiscountParcent,
                                       Id = r.Id,
                                       RequisitionToCompanyId = r.RequisitionToCompanyId,
                                       CompanyName = (r.RequisitionToCompanyId != null && r.RequisitionToCompanyId > 0) ? (_context.Companies.FirstOrDefault(x => x.CompanyId == r.RequisitionToCompanyId).ShortName) : null,
                                       DivisionId = r.DivisionId,
                                       DivisionName = (r.DivisionId != null && r.DivisionId > 0) ? (_context.Departments.FirstOrDefault(x => x.DepartmentId == r.DivisionId).Name) : null,
                                       CategoryId = (int)r.GRequisitionCategoryId,
                                       CategoryName = category.CategoryName,
                                       UserCompanyId = r.CompanyId,
                                       UserCompanyName = userCompany.ShortName,
                                       Remarks = r.Remarks,
                                       RequisitionDate = r.RequisitionDate,
                                       GeneralRequisitionType = (BusinessTypeEnum)r.GRequisitionType,
                                       RequisitionNumber = r.RequisitionNumber,
                                       Status = (GeneralRequisitionStatusEnum)r.Status,
                                       IsAsset = r.IsAsset,
                                       ProjectId = r.ProjectId,
                                       ProjectName = r.ProjectId.HasValue ? _context.Accounting_CostCenter.FirstOrDefault(x => x.CostCenterId == r.ProjectId.Value).Name : "---"
                                   }).AsEnumerable();

            return requisitionList;
        }
        public IEnumerable<GeneralRequisitionMasterVM> GetAllRequisitionApprovalList(int? companyId, DateTime? fromDate, DateTime? toDate, long? userId, SignatoryStatusEnum? approvalStatus)
        {
            var requisitionList = (from r in _context.GeneralRequisitions
                                   join approval in _context.RequisitionSignatoryApprovals on r.Id equals approval.RequisitionId
                                   join category in _context.GeneralRequisitionProductCategories on r.GRequisitionCategoryId equals category.Id
                                   join fromCompany in _context.Companies on r.CompanyId equals fromCompany.CompanyId
                                   where r.IsActive
                                   && approval.EmployeeId == userId
                                   //&& ((approvalStatus == null &&  approval.Status == (int)SignatoryStatusEnum.Pending) || (approvalStatus != null && approval.Status == (int)approvalStatus))                                
                                   && (
                                   (approvalStatus == null && approval.Status == (int)SignatoryStatusEnum.Pending)
                                   || (approvalStatus != null && approvalStatus.HasValue && (SignatoryStatusEnum)approval.Status == approvalStatus)
                                   )
                                   && ((fromDate == null || toDate == null) || r.RequisitionDate >= fromDate && r.RequisitionDate <= toDate)
                                   select new GeneralRequisitionMasterVM()
                                   {
                                       AITParcent = r.AITParcent,
                                       DiscountParcent = r.DiscountParcent,
                                       Id = r.Id,
                                       RequisitionToCompanyId = r.RequisitionToCompanyId,
                                       CompanyName = (r.RequisitionToCompanyId != null && r.RequisitionToCompanyId > 0) ? (_context.Companies.FirstOrDefault(x => x.CompanyId == r.RequisitionToCompanyId).Name) : null,
                                       DivisionId = r.DivisionId,
                                       DivisionName = (r.DivisionId != null && r.DivisionId > 0) ? (_context.Departments.FirstOrDefault(x => x.DepartmentId == r.DivisionId).Name) : null,
                                       CategoryId = (int)r.GRequisitionCategoryId,
                                       CategoryName = category.CategoryName,
                                       UserCompanyId = r.CompanyId,
                                       UserCompanyName = fromCompany.Name,
                                       Remarks = r.Remarks,
                                       RequisitionTotalAmount = (_context.GeneralRequisitionDetails.Where(x => x.RequisitionId == r.Id).Sum(x => x.Quantity * x.UnitPrice)),
                                       RequisitionDate = r.RequisitionDate,
                                       GeneralRequisitionType = (BusinessTypeEnum)r.GRequisitionType,
                                       RequisitionNumber = r.RequisitionNumber,
                                       RequisitionSignatoryStatus = (SignatoryStatusEnum)approval.Status,
                                       RequisitionSignatoryId = approval.RequisitionSignatoryApprovalId,
                                       IsAsset = r.IsAsset,
                                       ProjectId = r.ProjectId,
                                       ProjectName = r.ProjectId.HasValue ? _context.Accounting_CostCenter.FirstOrDefault(x => x.CostCenterId == r.ProjectId.Value).Name : "---",
                                       PreviousSignatoryStatus = (from rsa in _context.RequisitionSignatoryApprovals
                                                                  where rsa.RequisitionId == r.Id
                                                                  && (approval.OrderBy <= 1 || (rsa.OrderBy == (approval.OrderBy - 1) && rsa.Status == (int)SignatoryStatusEnum.Approved))
                                                                  //&& rsa.Status > 0
                                                                  select rsa
                                                                  ).FirstOrDefault() != null ? true : false
                                   }).Where(x => x.PreviousSignatoryStatus).OrderByDescending(x => x.Id).AsEnumerable();
            return requisitionList;
        }
        public async Task<bool> UpdateRequisitionSignatoryApprovalStatus(long id, int statusId, string comment)
        {
            var exist = await _context.RequisitionSignatoryApprovals.FirstOrDefaultAsync(x => x.RequisitionSignatoryApprovalId == id);
            if (exist != null)
            {
                exist.Status = statusId;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.Comment = comment;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                var generalRequisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == exist.RequisitionId);
                if (statusId == (int)SignatoryStatusEnum.Rejected)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Rejected;
                    await _context.SaveChangesAsync();
                    return true;
                }
                generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Pending;
                var isFinalApproval = _context.RequisitionSignatoryApprovals.Where(x => x.IsActive && x.RequisitionId == exist.RequisitionId).Max(x => x.OrderBy);
                if (isFinalApproval == exist.OrderBy && statusId == (int)SignatoryStatusEnum.Approved)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Approved;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateEDocumentSignatoryApprovalStatus(long id, EFileSignatoryStatusEnum statusId, string comment)
        {
            var exist = await _context.RequisitionSignatoryApprovals.FirstOrDefaultAsync(x => x.RequisitionSignatoryApprovalId == id);
            if (exist != null)
            {
                exist.Status = (int)statusId;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.Comment = comment;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                var generalRequisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == exist.RequisitionId);
                if (statusId == EFileSignatoryStatusEnum.Rejected)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Rejected;
                    await _context.SaveChangesAsync();
                    return true;
                }
                // if caller is 3rd approval, not the same person
                if (statusId == EFileSignatoryStatusEnum.Return && generalRequisition.CreatedBy != exist.CreatedBy)
                {
                    var empId = _context.Employees.FirstOrDefault(x => x.EmployeeId == exist.CreatedBy).Id;
                    var totalCount = _context.RequisitionSignatoryApprovals.Count(x => x.RequisitionId == exist.RequisitionId && x.IsActive);
                    RequisitionSignatoryApproval forward = new RequisitionSignatoryApproval()
                    {
                        RequisitionId = exist.RequisitionId,
                        EmployeeId = empId,
                        OrderBy = ++totalCount,
                        IsActive = true,
                        CreatedBy = Common.GetUserId(),
                        CreatedDate = DateTime.Now,
                        Status = (int)EFileSignatoryStatusEnum.Pending

                    };
                    _context.RequisitionSignatoryApprovals.Add(forward);

                    //forward.CreatedBy = "KG3070";
                    await _context.SaveChangesAsync();
                }


                generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Pending;
                //var isFinalApproval = _context.RequisitionSignatoryApprovals.Where(x => x.IsActive && x.RequisitionId == exist.RequisitionId).Max(x => x.OrderBy);
                var isAllApprovalOk = _context.RequisitionSignatoryApprovals.Count(x => x.IsActive && x.RequisitionId == exist.RequisitionId && (x.Status == (int)EFileSignatoryStatusEnum.Pending || x.Status == (int)EFileSignatoryStatusEnum.Rejected));
                if (isAllApprovalOk == 0 && statusId == EFileSignatoryStatusEnum.Approved)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Approved;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public IEnumerable<RequisitionApprovalVM> GetAllApproval(int requisitionId)
        {


            var requisitionList = (from a in _context.RequisitionSignatoryApprovals
                                   join emp in _context.Employees on a.EmployeeId equals emp.Id
                                   join des in _context.Designations on emp.DesignationId equals des.DesignationId
                                   join signatory in _context.RequisitionSignatories.Where(x => x.IntegrateWith == "GeneralRequisition") on a.RequisitionSignatoryId equals signatory.RequisitionSignatoryId into signatoryGroup
                                   from signatory in signatoryGroup.DefaultIfEmpty()
                                   where a.RequisitionId == requisitionId
                                   select new RequisitionApprovalVM()
                                   {
                                       EmployeeId = emp.Id,
                                       EmployeeIdString = emp.EmployeeId,
                                       EmployeeName = emp.Name,
                                       DesignationName = des.Name,
                                       OrderBy = a.OrderBy,
                                       Comment = a.Comment,
                                       Status = (SignatoryStatusEnum)a.Status,
                                       StatusString = (from rsa in _context.RequisitionSignatoryApprovals
                                                       where rsa.RequisitionId == a.RequisitionId
                                                       && (a.OrderBy <= 1 || (rsa.OrderBy == (a.OrderBy - 1) && rsa.Status == (int)SignatoryStatusEnum.Approved))
                                                       select rsa).FirstOrDefault() != null ? ((SignatoryStatusEnum)a.Status).ToString() : "...",
                                       ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                                   }).OrderBy(x => x.OrderBy).AsEnumerable();


            return requisitionList;
        }

        public IEnumerable<EDocumentApprovalVM> GetAllEDocumentApprovalList(int requisitionId)
        {


            var requisitionList = (from a in _context.RequisitionSignatoryApprovals
                                   join emp in _context.Employees on a.EmployeeId equals emp.Id
                                   join des in _context.Designations on emp.DesignationId equals des.DesignationId
                                   join signatory in _context.RequisitionSignatories.Where(x => x.IntegrateWith == "GeneralRequisition") on a.RequisitionSignatoryId equals signatory.RequisitionSignatoryId into signatoryGroup
                                   from signatory in signatoryGroup.DefaultIfEmpty()
                                   where a.RequisitionId == requisitionId && a.IsActive
                                   select new EDocumentApprovalVM()
                                   {
                                       EmployeeId = emp.Id,
                                       EmployeeIdString = emp.EmployeeId,
                                       EmployeeName = emp.Name,
                                       DesignationName = des.Name,
                                       OrderBy = a.OrderBy,
                                       Comment = a.Comment,
                                       Status = (EFileSignatoryStatusEnum)a.Status,
                                       //StatusString = (from rsa in _context.RequisitionSignatoryApprovals
                                       //                where rsa.RequisitionId == a.RequisitionId
                                       //                && (a.OrderBy <= 1 || (rsa.OrderBy == (a.OrderBy - 1) && rsa.Status == (int)EFileSignatoryStatusEnum.Approved))
                                       //                select rsa).FirstOrDefault() != null ? ((EFileSignatoryStatusEnum)a.Status).ToString() : "...",
                                       StatusString = ((EFileSignatoryStatusEnum)a.Status).ToString(),
                                       ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                                   }).OrderBy(x => x.OrderBy).AsEnumerable();


            return requisitionList;
        }
        public async Task<bool> UpdateStatus(long id, int status)
        {
            var exist = await _context.GeneralRequisitions.FirstOrDefaultAsync(x => x.Id == id);
            if (exist != null)
            {
                if (status == (int)GeneralRequisitionStatusEnum.Submitted)
                {
                    List<RequisitionSignatoryApproval> requisitionApprovalList = new List<RequisitionSignatoryApproval>();
                    var totalAmount = _context.GeneralRequisitionDetails.Where(x => x.RequisitionId == exist.Id && x.IsActive).Sum(x => x.Quantity * x.UnitPrice);
                    var userId = Common.GetUserId();
                    int orderBy = 1;
                    List<long> approvalIdList = new List<long>();
                    var businessHead = _context.Employees.FirstOrDefault(x => x.Id == exist.EmployeeId).ManagerId;
                    if (businessHead != null && businessHead > 0)
                    {
                        approvalIdList.Add(businessHead.Value);
                        //businessHead
                        RequisitionSignatoryApproval requisitionApproval = new RequisitionSignatoryApproval()
                        {
                            RequisitionId = exist.Id,
                            RequisitionSignatoryId = 0,
                            EmployeeId = businessHead.Value,
                            IsActive = true,
                            Status = (int)SignatoryStatusEnum.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            OrderBy = orderBy
                        };
                        requisitionApprovalList.Add(requisitionApproval);
                        orderBy++;
                    }

                    var toCompanyHead = _context.BusinessHeads.FirstOrDefault(x => x.IsActive && x.BusinessType == (int)BusinessTypeEnum.Company && x.BusineesId_Fk == exist.RequisitionToCompanyId.Value && !approvalIdList.Any(y => y == x.EmployeeId));
                    if (toCompanyHead != null)
                    {
                        approvalIdList.Add(toCompanyHead.EmployeeId);
                        //businessHead
                        RequisitionSignatoryApproval requisitionApproval = new RequisitionSignatoryApproval()
                        {
                            RequisitionId = exist.Id,
                            RequisitionSignatoryId = 0,
                            EmployeeId = toCompanyHead.EmployeeId,
                            IsActive = true,
                            Status = (int)SignatoryStatusEnum.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            OrderBy = orderBy
                        };
                        requisitionApprovalList.Add(requisitionApproval);
                        orderBy++;
                    }

                    var categorywiseApprovar = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == "GeneralRequisition" &&  x.IsActive && x.CategoryId == exist.GRequisitionCategoryId && !approvalIdList.Any(y => y == x.EmployeeId));

                    if (categorywiseApprovar != null)
                    {
                        approvalIdList.Add(categorywiseApprovar.EmployeeId);
                        RequisitionSignatoryApproval requisitionApproval = new RequisitionSignatoryApproval()
                        {
                            RequisitionId = exist.Id,
                            RequisitionSignatoryId = categorywiseApprovar.RequisitionSignatoryId,
                            IsActive = true,
                            Status = (int)SignatoryStatusEnum.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            EmployeeId = categorywiseApprovar.EmployeeId,
                            OrderBy = orderBy
                        };
                        requisitionApprovalList.Add(requisitionApproval);
                        orderBy++;
                    }

                    if (totalAmount <= 100000 && !exist.IsAsset)
                    {
                        var financeDiractor = _context.Employees.FirstOrDefault(x => x.DesignationId == 177 && x.Active).Id;
                        RequisitionSignatoryApproval requisitionApproval = new RequisitionSignatoryApproval()
                        {
                            RequisitionId = exist.Id,
                            RequisitionSignatoryId = 0,
                            IsActive = true,
                            Status = (int)SignatoryStatusEnum.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            EmployeeId = financeDiractor,
                            OrderBy = orderBy
                        };
                        requisitionApprovalList.Add(requisitionApproval);
                    }
                    else
                    {
                        var gmd = _context.Employees.FirstOrDefault(x => x.DesignationId == 181 && x.Active).Id;
                        RequisitionSignatoryApproval requisitionApproval = new RequisitionSignatoryApproval()
                        {
                            RequisitionId = exist.Id,
                            RequisitionSignatoryId = 0,
                            IsActive = true,
                            Status = (int)SignatoryStatusEnum.Pending,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            EmployeeId = gmd,
                            OrderBy = orderBy

                        };
                        requisitionApprovalList.Add(requisitionApproval);
                    }
                    _context.RequisitionSignatoryApprovals.AddRange(requisitionApprovalList);
                }
                exist.Status = status;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> Remove(int id)
        {
            var exist = await _context.GeneralRequisitions.FirstOrDefaultAsync(x => x.Id == id);
            if (exist != null)
            {
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.IsActive = false;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RemoveRequisitionDetail(GeneralRequisitionMasterVM generalRequisitionMasterVM)
        {
            var exist = await _context.GeneralRequisitionDetails.FirstOrDefaultAsync(x => x.Id == generalRequisitionMasterVM.RequisitionItemId);
            if (exist != null)
            {
                exist.ModifiedBy = generalRequisitionMasterVM.ModifiedBy;
                exist.ModifiedDate = generalRequisitionMasterVM.ModifiedDate;
                exist.IsActive = false;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Update(GeneralRequisitionMasterVM generalRequisitionMasterVM)
        {
            var exist = await _context.GeneralRequisitions.FirstOrDefaultAsync(x => x.Id == generalRequisitionMasterVM.Id);
            if (exist != null)
            {
                exist.ModifiedBy = generalRequisitionMasterVM.ModifiedBy;
                exist.ModifiedDate = generalRequisitionMasterVM.ModifiedDate;
                exist.Remarks = generalRequisitionMasterVM.Remarks;
                exist.RequisitionToCompanyId = generalRequisitionMasterVM.RequisitionToCompanyId;
                exist.GRequisitionType = (int)generalRequisitionMasterVM.GeneralRequisitionType;
                exist.DivisionId = generalRequisitionMasterVM.DivisionId;
                exist.GRequisitionCategoryId = generalRequisitionMasterVM.CategoryId;
                exist.ProjectId = generalRequisitionMasterVM.ProjectId;
                exist.AITParcent = generalRequisitionMasterVM.AITParcent;
                exist.DiscountParcent = generalRequisitionMasterVM.DiscountParcent;
                exist.IsAsset = generalRequisitionMasterVM.IsAsset;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> AddRequisitionItemDetail(GeneralRequisitionMasterVM generalRequisitionMasterVM)
        {

            GeneralRequisitionDetail generalRequisitionDetail = new GeneralRequisitionDetail()
            {
                IsActive = true,
                RequisitionId = (int)generalRequisitionMasterVM.Id,
                ProductName = generalRequisitionMasterVM.ProductName,
                UnitId = generalRequisitionMasterVM.UnitId,
                UnitPrice = generalRequisitionMasterVM.UnitPrice,
                Quantity = generalRequisitionMasterVM.Quantity,
                CreatedBy = generalRequisitionMasterVM.CreatedBy,
                CreatedDate = generalRequisitionMasterVM.CreatedDate,
                DiscountParcent = generalRequisitionMasterVM.DiscountProductParcent
            };
            _context.GeneralRequisitionDetails.Add(generalRequisitionDetail);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateRequisitionDetail(GeneralRequisitionMasterVM generalRequisitionMasterVM)
        {
            var exist = await _context.GeneralRequisitionDetails.FirstOrDefaultAsync(x => x.Id == generalRequisitionMasterVM.RequisitionItemId);
            if (exist != null)
            {
                exist.ModifiedBy = generalRequisitionMasterVM.ModifiedBy;
                exist.ModifiedDate = generalRequisitionMasterVM.ModifiedDate;
                exist.UnitPrice = generalRequisitionMasterVM.UnitPrice;
                exist.Quantity = generalRequisitionMasterVM.Quantity;
                exist.ProductName = generalRequisitionMasterVM.ProductName;
                exist.UnitId = generalRequisitionMasterVM.UnitId;
                exist.DiscountParcent = generalRequisitionMasterVM.DiscountProductParcent;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }

            }
            return false;
        }
        public async Task<string> GetGeneralRequisitionName(int companyOrDivisionId, BusinessTypeEnum companyType)
        {
            int maxId = await _context.GeneralRequisitions.CountAsync(x => companyType == BusinessTypeEnum.Company && x.RequisitionToCompanyId == companyOrDivisionId) + 1;
            var companySerialNo = _context.Companies.FirstOrDefault(x => x.CompanyId == companyOrDivisionId).SerialNo;
            int minLength = 4;
            char preFillupChar = '0';
            var companyNewSerialNo = Common.GenerateLengthWiseString(companySerialNo.ToString(), 2, preFillupChar);
            var requisitionNumber = Common.GenerateLengthWiseString(maxId.ToString(), minLength, preFillupChar);
            var number = "FR" + companyNewSerialNo + requisitionNumber;
            return number;
        }
        public async Task<string> GetEFileName()
        {
            string efileNumber = "KGNOTE";
            var count = _context.GeneralRequisitions.Count(x => x.IsActive && x.GRequisitionType == 3) + 1;
            return efileNumber + count;
        }
        #region GeneralRequisitionCategory
        public async Task<bool> AddRequisitionCategory(GeneralRequisitionProductCategoryVM categoryVM)
        {
            GeneralRequisitionProductCategory category = new GeneralRequisitionProductCategory()
            {
                CategoryName = categoryVM.CategoryName,
                CreatedBy = categoryVM.CreatedBy,
                CreatedDate = categoryVM.CreatedDate,
                IsActive = true
            };
            _context.GeneralRequisitionProductCategories.Add(category);
            int status = await _context.SaveChangesAsync();
            if (status > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateRequisitionCategory(GeneralRequisitionProductCategoryVM categoryVM)
        {
            var exist = _context.GeneralRequisitionProductCategories.FirstOrDefault(x => x.Id == categoryVM.CategoryId);
            if (exist != null)
            {
                exist.CategoryName = categoryVM.CategoryName;
                exist.ModifiedDate = categoryVM.ModifiedDate;
                exist.ModifiedBy = categoryVM.ModifiedBy;
            }
            int status = await _context.SaveChangesAsync();
            if (status > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<GeneralRequisitionProductCategoryVM> GetRequisitionCategory(int id)
        {
            var categoryVM = (from r in _context.GeneralRequisitionProductCategories
                              where r.IsActive && r.Id == id
                              select new GeneralRequisitionProductCategoryVM()
                              {
                                  CategoryId = r.Id,
                                  CreatedBy = r.CreatedBy,
                                  CreatedDate = r.CreatedDate,
                                  IsActive = r.IsActive,
                                  CategoryName = r.CategoryName
                              }).FirstOrDefault();
            return categoryVM;

        }

        public async Task<IEnumerable<GeneralRequisitionProductCategoryVM>> GetAllRequisitionCategoryAsync()
        {

            var categoryVM = (from r in _context.GeneralRequisitionProductCategories
                              where r.IsActive
                              select new GeneralRequisitionProductCategoryVM()
                              {
                                  CategoryId = r.Id,
                                  CreatedBy = r.CreatedBy,
                                  CreatedDate = r.CreatedDate,
                                  IsActive = r.IsActive,
                                  CategoryName = r.CategoryName
                              }).ToListAsync();
            return await categoryVM;
        }
        public IEnumerable<GeneralRequisitionProductCategoryVM> GetAllGeneralRequisition()
        {
            var categoryVMList = (from r in _context.GeneralRequisitionProductCategories
                                  where r.IsActive
                                  select new GeneralRequisitionProductCategoryVM()
                                  {
                                      CategoryId = r.Id,
                                      CreatedBy = r.CreatedBy,
                                      CreatedDate = r.CreatedDate,
                                      IsActive = r.IsActive,
                                      CategoryName = r.CategoryName
                                  }).ToList();
            return categoryVMList;
        }
        public async Task<bool> RemoveGeneralRequisitionCategory(int id)
        {
            var exist = await _context.GeneralRequisitionProductCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (exist != null)
            {
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.IsActive = false;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion GeneralRequisitionCategory

        #region Requisition Signatory
        public async Task<bool> AddRequisitionSignatory(RequisitionSignatoryVM model)
        {
            var exist = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == model.IntegrateWith &&  x.EmployeeId == model.EmployeeId && x.Limitation == model.Limitation && x.CompanyId == model.CompanyId && x.CategoryId == model.CategoryId);
            if (exist != null)
            {
                if (!exist.IsActive)
                {
                    exist.EmployeeId = model.EmployeeId;
                    exist.OrderBy = model.OrderBy;
                    exist.ModifiedBy = model.ModifiedBy;
                    exist.ModifiedDate = DateTime.Now;
                    exist.Limitation = model.Limitation;
                    exist.DesignationName = model.DesignationName;
                    exist.IntegrateWith = model.IntegrateWith;
                    if (model.IsCategorizedSignatory)
                    {
                        exist.CategoryId = model.CategoryId;
                        exist.CompanyId = 0;
                    }
                    exist.IsActive = true;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            var signatory = new RequisitionSignatory()
            {
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
                CompanyId = model.CompanyId,
                DesignationName = model.DesignationName,
                IntegrateWith = model.IntegrateWith,
                EmployeeId = model.EmployeeId,
                IsActive = true,
                Limitation = model.Limitation,
                OrderBy = model.OrderBy
            };
            if (model.IsCategorizedSignatory)
            {
                var exists = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == model.IntegrateWith && x.IsActive && x.CategoryId == model.CategoryId);
                if (exists != null)
                {
                    return false;
                }
                signatory.CategoryId = model.CategoryId;
                signatory.CompanyId = 0;
            }
            //if (model.CategoryId > 0 && !string.IsNullOrEmpty(model.CategoryName))
            //{
            //    signatory.CategoryId = model.CategoryId;
            //    signatory.CompanyId = 0;
            //}
            _context.RequisitionSignatories.Add(signatory);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateRequisitionSignatory(RequisitionSignatoryVM model)
        {
            var exist = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == model.IntegrateWith && x.RequisitionSignatoryId == model.RequisitionSignatoryId);
            if (exist != null)
            {
                var sameRuleExist = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == model.IntegrateWith && x.EmployeeId == model.EmployeeId && x.Limitation == model.Limitation && x.OrderBy == model.OrderBy && x.DesignationName == model.DesignationName && x.CategoryId == model.CategoryId);
                if (sameRuleExist != null)
                {
                    return false;
                }
                exist.EmployeeId = model.EmployeeId;
                exist.OrderBy = model.OrderBy;
                exist.ModifiedBy = model.ModifiedBy;
                exist.ModifiedDate = DateTime.Now;
                exist.Limitation = model.Limitation;
                exist.DesignationName = model.DesignationName;
                exist.IntegrateWith = model.IntegrateWith;
                if (model.IsCategorizedSignatory)
                {
                    exist.CategoryId = model.CategoryId;
                    exist.CompanyId = 0;
                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> DeleteRequisitionSignatory(RequisitionSignatoryVM model)
        {
            var exist = _context.RequisitionSignatories.FirstOrDefault(x => x.IntegrateWith == model.IntegrateWith && x.RequisitionSignatoryId == model.RequisitionSignatoryId);
            if (exist != null)
            {
                exist.IsActive = false;
                exist.ModifiedBy = model.ModifiedBy;
                exist.ModifiedDate = DateTime.Now;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatory()
        {
            var exist = (from r in _context.RequisitionSignatories
                         join e in _context.Employees on r.EmployeeId equals e.Id
                         join c in _context.Companies on r.CompanyId equals c.CompanyId into companyGroup
                         from c in companyGroup.DefaultIfEmpty()
                         join category in _context.GeneralRequisitionProductCategories on r.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         where r.IsActive && r.IntegrateWith == "GeneralRequisition"
                         select new RequisitionSignatoryVM()
                         {
                             RequisitionSignatoryId = r.RequisitionSignatoryId,
                             CompanyId = r.CompanyId,
                             CompanyName = c.Name,
                             DesignationName = r.DesignationName,
                             KGEmployeeId = e.EmployeeId,
                             EmployeeId = r.EmployeeId,
                             EmployeeName = e.Name,
                             OrderBy = r.OrderBy,
                             Limitation = r.Limitation,
                             IsActive = r.IsActive,
                             CategoryName = category.CategoryName,
                             CategoryId = category.Id
                         }).OrderBy(x => x.CompanyId).ThenBy(x => x.Limitation).ThenBy(x => x.OrderBy).ToListAsync();
            return await exist;
        }
        public async Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByCompany(int companyId , string integrateWith)
        {
            var exist = (from r in _context.RequisitionSignatories
                         join e in _context.Employees on r.EmployeeId equals e.Id
                         join c in _context.Companies on r.CompanyId equals c.CompanyId into companyGroup
                         from c in companyGroup.DefaultIfEmpty()
                         join category in _context.GeneralRequisitionProductCategories on r.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         where r.IsActive && r.CompanyId == companyId  && r.IntegrateWith == integrateWith
                         select new RequisitionSignatoryVM()
                         {
                             RequisitionSignatoryId = r.RequisitionSignatoryId,
                             CompanyId = r.CompanyId,
                             CompanyName = c.Name,
                             DesignationName = r.DesignationName,
                             KGEmployeeId = e.EmployeeId,
                             EmployeeId = r.EmployeeId,
                             EmployeeName = e.Name,
                             OrderBy = r.OrderBy,
                             Limitation = r.Limitation,
                             IsActive = r.IsActive,
                             CategoryName = category.CategoryName,
                             CategoryId = category.Id
                         }).OrderBy(x => x.CompanyId).ThenBy(x => x.Limitation).ThenBy(x => x.OrderBy).ToListAsync();
            return await exist;
        }
        public async Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByCategory(int categoryId)
        {
            var exist = (from r in _context.RequisitionSignatories
                         join e in _context.Employees on r.EmployeeId equals e.Id
                         join c in _context.Companies on r.CompanyId equals c.CompanyId into companyGroup
                         from c in companyGroup.DefaultIfEmpty()
                         join category in _context.GeneralRequisitionProductCategories on r.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         where r.IsActive && r.CategoryId.HasValue && r.CategoryId == categoryId && r.IntegrateWith == "GeneralRequisition"
                         select new RequisitionSignatoryVM()
                         {
                             RequisitionSignatoryId = r.RequisitionSignatoryId,
                             CompanyId = r.CompanyId,
                             CompanyName = c.Name,
                             DesignationName = r.DesignationName,
                             KGEmployeeId = e.EmployeeId,
                             EmployeeId = r.EmployeeId,
                             EmployeeName = e.Name,
                             OrderBy = r.OrderBy,
                             Limitation = r.Limitation,
                             IsActive = r.IsActive,
                             CategoryName = category.CategoryName,
                             CategoryId = category.Id
                         }).ToListAsync();
            return await exist;
        }
        public async Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByDivision(int divisionId)
        {
            var exist = (from r in _context.RequisitionSignatories
                         join e in _context.Employees on r.EmployeeId equals e.Id
                         join c in _context.Companies on r.CompanyId equals c.CompanyId into companyGroup
                         from c in companyGroup.DefaultIfEmpty()
                         join category in _context.GeneralRequisitionProductCategories on r.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         where r.IsActive && r.CategoryId.HasValue && r.CategoryId == divisionId && r.IntegrateWith == "GeneralRequisition"
                         select new RequisitionSignatoryVM()
                         {
                             RequisitionSignatoryId = r.RequisitionSignatoryId,
                             CompanyId = r.CompanyId,
                             CompanyName = c.Name,
                             DesignationName = r.DesignationName,
                             KGEmployeeId = e.EmployeeId,
                             EmployeeId = r.EmployeeId,
                             EmployeeName = e.Name,
                             OrderBy = r.OrderBy,
                             Limitation = r.Limitation,
                             IsActive = r.IsActive,
                             CategoryName = category.CategoryName,
                             CategoryId = category.Id
                         }).ToListAsync();
            return await exist;
        }

        #endregion requisition signatory


        #region ERequisition
        public async Task<bool> AddERequisition(ERequisitionVM generalRequisitionMasterVM)
        {
            //GeneralRequisitionMasterVM
            GeneralRequisition generalRequisition = new GeneralRequisition()
            {
                RequisitionDate = generalRequisitionMasterVM.RequisitionDate,
                RequisitionNumber = generalRequisitionMasterVM.RequisitionNumber,
                Remarks = generalRequisitionMasterVM.Remarks,
                GRequisitionType = 3,
                CompanyId = 0,
                Status = (int)GeneralRequisitionStatusEnum.Draft,
                IsActive = true,
                CreatedBy = generalRequisitionMasterVM.CreatedBy,
                CreatedDate = generalRequisitionMasterVM.CreatedDate,
                // RequisitionToCompanyId = generalRequisitionMasterVM.RequisitionToCompanyId,
                EmployeeId = 0,
                //IsAsset = generalRequisitionMasterVM.IsAsset,
                // ProjectId = generalRequisitionMasterVM.ProjectId == null && generalRequisitionMasterVM.ProjectId > 0 ? generalRequisitionMasterVM.ProjectId : null
            };
            _context.GeneralRequisitions.Add(generalRequisition);
            int status = await _context.SaveChangesAsync();
            //if (status > 0 && !string.IsNullOrEmpty(generalRequisitionMasterVM.Description))
            //{
            //    generalRequisitionMasterVM.Id = generalRequisition.Id;
            //    var childStatus = await AddERequisitionItemDetail(generalRequisitionMasterVM);
            //}
            if (status > 0)
            {
                generalRequisitionMasterVM.Id = generalRequisition.Id;
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateERequisition(ERequisitionVM model)
        {
            var requisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == model.Id);
            if (requisition != null)
            {
                requisition.Remarks = model.Remarks;
                requisition.ModifiedBy = Common.GetUserId();
                requisition.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> AddERequisitionItemDetail(ERequisitionVM generalRequisitionMasterVM)
        {
            GeneralRequisitionDetail generalRequisitionDetail = new GeneralRequisitionDetail()
            {
                IsActive = true,
                RequisitionId = (int)generalRequisitionMasterVM.Id,
                ProductName = generalRequisitionMasterVM.Description,
                CreatedBy = generalRequisitionMasterVM.CreatedBy,
                CreatedDate = generalRequisitionMasterVM.CreatedDate
                 

            };
            _context.GeneralRequisitionDetails.Add(generalRequisitionDetail);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateERequisitionItemDetail(ERequisitionVM generalRequisitionMasterVM)
        {
            var exist = _context.GeneralRequisitionDetails.FirstOrDefault(x => x.Id == generalRequisitionMasterVM.ERequisitionDetailId);
            if (exist != null)
            {
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.ProductName = generalRequisitionMasterVM.Description;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteERequisitionItemDetail(int id)
        {
            var exist = _context.GeneralRequisitionDetails.FirstOrDefault(x => x.Id == id);
            if (exist != null)
            {
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.IsActive = false;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<ERequisitionVM> GetERequistionById(int id)
        {
            var requisition = (from r in _context.GeneralRequisitions
                               join e in _context.Employees on r.CreatedBy equals e.EmployeeId
                               where r.IsActive && r.Id == id
                               select new ERequisitionVM()
                               {
                                   Id = r.Id,
                                   UserCompanyId = (int)e.CompanyId,
                                   CreatedBy = e.Name + " (" + r.CreatedBy + ")",
                                   CreatedDate = r.CreatedDate,
                                   ERequisitionStatus = (GeneralRequisitionStatusEnum)r.Status,
                                   ModifiedBy = r.ModifiedBy,
                                   ModifiedDate = r.ModifiedDate,
                                   Remarks = r.Remarks,
                                   RequisitionDate = r.RequisitionDate,
                                   RequisitionNumber = r.RequisitionNumber
                                   //AttachmentId = (from a in _context.CustomerBookingFileMappings
                                   //                join b in _context.FileArchives on a.DocId equals b.docid
                                   //                where b.FileCatagoryId == 8 &&  a.CGId == r.Id
                                   //                select b.docid
                                   //                ).FirstOrDefault(),
                                   //DetailsList = (
                                   //  from item in _context.GeneralRequisitionDetails
                                   //  where item.RequisitionId == r.Id && item.IsActive
                                   //  select new ERequisitionDetailVM()
                                   //  {
                                   //      Id = item.Id,
                                   //      Description = item.ProductName
                                   //  }).ToList()
                               }).FirstOrDefault();
            if (requisition != null)
            {
                requisition.SignatoryList = (from s in _context.RequisitionSignatoryApprovals
                                             join e in _context.Employees on s.EmployeeId equals e.Id
                                             join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                             join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                             where s.RequisitionId == requisition.Id && s.IsActive
                                             select new ERequisitionSignatoryVM()
                                             {
                                                 Id = s.RequisitionSignatoryApprovalId,
                                                 EmployeeId = e.EmployeeId,
                                                 EmployeeName = e.Name,
                                                 EmployeeIdInt = s.EmployeeId,
                                                 DepartmentName = d.Name,
                                                 DesignationName = ds.Name,
                                                 Comment = s.Comment,
                                                 SignatoryStatus = (EFileSignatoryStatusEnum)s.Status,
                                                 OrderBy = s.OrderBy,
                                                 ApprovedTime = s.ModifiedDate ?? DateTime.MinValue
                                             }).OrderBy(x => x.OrderBy).ToList();

                requisition.Attachments = (from a in _context.CustomerBookingFileMappings
                                           join b in _context.FileArchives on a.DocId equals b.docid
                                           where b.FileCatagoryId == 8 && a.CGId == requisition.Id
                                           && a.IsActive
                                           select new AttachmentVM()
                                           {
                                               DocId = a.DocId,
                                               Title = a.FileTitel,
                                               Extension = b.fileext
                                           }).ToList();

                if (requisition.SignatoryList != null && requisition.SignatoryList.Count() > 0)
                {
                    int hasPending = requisition.SignatoryList.Count(x => x.SignatoryStatus == EFileSignatoryStatusEnum.Pending);
                    if (hasPending > 0)
                    {
                        requisition.IsApprovedForwardStatus = true;
                    }
                }

            }

            return requisition;

        }

        public async Task<bool> ForwardRequisition(ERequisitionVM model)
        {
            //for update signatory
            if (model.SignatoryId > 0)
            {
                var exist = _context.RequisitionSignatoryApprovals.FirstOrDefault(x => x.RequisitionSignatoryApprovalId == model.SignatoryId && x.IsActive);
                if (exist != null)
                {
                    exist.EmployeeId = model.EmployeeId;
                    exist.ModifiedBy = model.CreatedBy;
                    exist.ModifiedDate = model.CreatedDate;
                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
            }

            ////requisition status change to submit
            var previousSignatory = _context.RequisitionSignatoryApprovals.Count(x => x.RequisitionId == model.Id && x.IsActive && x.EmployeeId == model.EmployeeId);
            if (previousSignatory == 0)
            {
                var requisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == model.Id);
                if (requisition != null)
                {
                    requisition.Status = (int)GeneralRequisitionStatusEnum.Submitted;
                    await _context.SaveChangesAsync();
                }

            }
            else
            {
                //if previous pending , new forwarding make false
                int isPreviousApproved = _context.RequisitionSignatoryApprovals.Count(x => x.RequisitionId == model.Id && x.IsActive && x.Status == (int)SignatoryStatusEnum.Pending);
                if (isPreviousApproved > 0)
                {
                    return false;
                }
                else
                {
                    var requisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == model.Id);
                    if (requisition != null)
                    {
                        requisition.Status = (int)GeneralRequisitionStatusEnum.Pending;
                        await _context.SaveChangesAsync();
                    }

                }
            }
            //new forwarding
            var ids = model.EmployeeIdList.Split(',').Where(x => x.Length > 0).ToArray();

            foreach (var item in ids)
            {
                int empId = Convert.ToInt32(item);
                if (empId <= 0)
                {
                    continue;
                }
                RequisitionSignatoryApproval requisitionForward = new RequisitionSignatoryApproval()
                {
                    IsActive = true,
                    RequisitionId = model.Id,
                    OrderBy = ++previousSignatory,
                    EmployeeId = empId,
                    Status = (int)SignatoryStatusEnum.Pending,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate
                };
                _context.RequisitionSignatoryApprovals.Add(requisitionForward);
            }

            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteRequisitionSignatory(int signatoryId)
        {
            var exist = _context.RequisitionSignatoryApprovals.FirstOrDefault(x => x.RequisitionSignatoryApprovalId == signatoryId);
            if (exist != null)
            {
                exist.IsActive = false;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }



        //load all edocument request
        public async Task<IEnumerable<ERequisitionVM>> GetAllEDocumentList(int? companyId, DateTime? fromDate, DateTime? toDate, GeneralRequisitionStatusEnum? status)
        {
            var userId = Common.GetUserId();
            //userId = "";
            var requisitionList = (from r in _context.GeneralRequisitions
                                   join e in _context.Employees on r.CreatedBy equals e.EmployeeId
                                   join userCompany in _context.Companies on e.CompanyId equals userCompany.CompanyId
                                   where r.IsActive && e.EmployeeId == userId && r.GRequisitionType == 3
                                   && (fromDate == null || r.RequisitionDate >= fromDate)
                                   && (toDate == null || r.RequisitionDate <= toDate)
                                   && (status == null || (GeneralRequisitionStatusEnum)r.Status == status)
                                   select new ERequisitionVM()
                                   {
                                       Id = r.Id,
                                       ERequisitionStatus = (GeneralRequisitionStatusEnum)r.Status,
                                       Remarks = r.Remarks,
                                       EmployeeName = e.Name,
                                       RequisitionNumber = r.RequisitionNumber,
                                       RequisitionDate = r.CreatedDate,
                                       LastSignatoryStatusInt = (from s in _context.RequisitionSignatoryApprovals
                                                                 where s.RequisitionId == r.Id
                                                                 orderby s.OrderBy descending
                                                                 select s.Status
                                                              ).FirstOrDefault(),
                                       Attachments = (from a in _context.CustomerBookingFileMappings
                                                      join b in _context.FileArchives on a.DocId equals b.docid
                                                      where b.FileCatagoryId == 8 && a.CGId == r.Id && b.isactive
                                                      select new AttachmentVM()
                                                      {
                                                          DocId = b.docid,
                                                          Title = b.docdesc
                                                      }
                                                   ).ToList()

                                   }).OrderByDescending(x => x.Id).ToListAsync();
            return await requisitionList;
        }
        public async Task<IEnumerable<ERequisitionVM>> GetAllEDocumentApprovalList(int? companyId, DateTime? fromDate, DateTime? toDate, long? userId, EFileSignatoryStatusEnum? approvalStatus)
        {
            var requisitionList = (from r in _context.GeneralRequisitions
                                   join approval in _context.RequisitionSignatoryApprovals on r.Id equals approval.RequisitionId
                                   join fromEmployee in _context.Employees on r.CreatedBy equals fromEmployee.EmployeeId
                                   join d in _context.Departments on fromEmployee.DepartmentId equals d.DepartmentId
                                   join ds in _context.Designations on fromEmployee.DesignationId equals ds.DesignationId
                                   where r.IsActive
                                   && (userId == null || approval.EmployeeId == userId)
                                   && (approvalStatus == null || (EFileSignatoryStatusEnum)approval.Status == approvalStatus)
                                   && (fromDate == null || r.RequisitionDate >= fromDate)
                                   && (toDate == null || r.RequisitionDate <= toDate)
                                   select new ERequisitionVM()
                                   {
                                       Id = r.Id,
                                       SignatoryId = (int)approval.RequisitionSignatoryApprovalId,
                                       RequisitionNumber = r.RequisitionNumber,
                                       Remarks = r.Remarks,
                                       RequisitionDate = r.RequisitionDate,
                                       EmployeeName = fromEmployee.Name,
                                       DepartmentName = d.Name,
                                       DesignationName = ds.Name,
                                       SignatoryStatus = (EFileSignatoryStatusEnum)approval.Status,
                                       Comment = approval.OrderBy > 1 ? (from a in _context.RequisitionSignatoryApprovals
                                                                         where a.RequisitionId == approval.RequisitionId && a.OrderBy == (approval.OrderBy - 1)
                                                                         && ((EFileSignatoryStatusEnum?)a.Status == EFileSignatoryStatusEnum.Forwarded
                                                                         ||
                                                                         (EFileSignatoryStatusEnum?)a.Status == EFileSignatoryStatusEnum.Return)
                                                                         //orderby a.OrderBy descending
                                                                         select a.Comment
                                                                       ).FirstOrDefault() : "",
                                       Attachments = (from a in _context.CustomerBookingFileMappings
                                                      join b in _context.FileArchives on a.DocId equals b.docid
                                                      where b.FileCatagoryId == 8 && a.CGId == r.Id && a.IsActive
                                                      select new AttachmentVM()
                                                      {
                                                          DocId = b.docid,
                                                          Title = b.docdesc
                                                      }
                                                   ).ToList()
                                   }).OrderBy(x => x.SignatoryStatus).ToListAsync();
            return await requisitionList;
        }
        public async Task<bool> UpdateEDocumentSignatoryApprovalStatus(long id, int statusId, string comment)
        {
            var exist = await _context.RequisitionSignatoryApprovals.FirstOrDefaultAsync(x => x.RequisitionSignatoryApprovalId == id);
            if (exist != null)
            {
                exist.Status = statusId;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.Comment = comment;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                var generalRequisition = _context.GeneralRequisitions.FirstOrDefault(x => x.Id == exist.RequisitionId);
                if (statusId == (int)SignatoryStatusEnum.Rejected)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Rejected;
                    await _context.SaveChangesAsync();
                    return true;
                }
                generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Pending;
                var isFinalApproval = _context.RequisitionSignatoryApprovals.Where(x => x.IsActive && x.RequisitionId == exist.RequisitionId).Max(x => x.OrderBy);
                if (isFinalApproval == exist.OrderBy && statusId == (int)SignatoryStatusEnum.Approved)
                {
                    generalRequisition.Status = (int)GeneralRequisitionStatusEnum.Approved;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



        #endregion ERequisition
    }
}
