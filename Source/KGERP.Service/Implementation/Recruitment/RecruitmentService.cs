using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Implementation.Recruitment.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Recruitment
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly ERPEntities _context;
        public RecruitmentService(ERPEntities context)
        {
            _context = context;
        }
        public MethodFeedbackVM Add(RecruitmentVM modelVM)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            RecruitmentRequisition recruitmentRequisition = new RecruitmentRequisition()
            {
                RequisitionNumber = modelVM.RequisitionNumber,
                CreatedBy = Common.GetUserId(),
                CreatedDate = DateTime.Now,
                Remarks = modelVM.Remarks,
                IsActive = true,
                Status = (int)RecruitmentStatusEnum.Draft,
                RequisitionDate = modelVM.RequisitionDate,
                
            };
            _context.RecruitmentRequisitions.Add(recruitmentRequisition);
            var status =  _context.SaveChanges();
            if (status > 0)
            {
                modelVM.Id = recruitmentRequisition.Id;
                RecruitmentRequisitionDetail rqsDetails = new RecruitmentRequisitionDetail();
                rqsDetails.IsActive = true;


                rqsDetails.BusinessType = (int)modelVM.BusinessType;
                rqsDetails.CompanyId = modelVM.CompanyId;
                rqsDetails.DepartmentId = modelVM.DepartmentId;


                rqsDetails.CreatedDate = DateTime.Now;
                rqsDetails.CreatedBy = Common.GetUserId();
                rqsDetails.JobNature = (int)modelVM.JobNature;
                rqsDetails.JobType= (int)modelVM.JobType;
                rqsDetails.VacancyType = (int)modelVM.VacancyType;
                rqsDetails.NumberOfVacancy = modelVM.NumberOfRecruitment;
                rqsDetails.JobTitle = modelVM.JobTitle;
                rqsDetails.JobLocation = modelVM.JobLocation;
                rqsDetails.RecruitmentRequisitionId = recruitmentRequisition.Id;
                rqsDetails.AdvertisementMedia = modelVM.Advertisement;
                rqsDetails.ExpectedJoiningDate = modelVM.ExpectedDateOfJoining;
                rqsDetails.ReportingManagerId = modelVM.ManagerId;
                rqsDetails.TargetSalary = modelVM.TargetSalary;
                rqsDetails.JobDetails = modelVM.JobDetails;
                _context.RecruitmentRequisitionDetails.Add(rqsDetails);
                status +=  _context.SaveChanges();
                if (status > 1)
                {
                    modelVM.RequisitionDetailId = rqsDetails.Id;
                   result.Status = true;
                    result.Message = "Your requisition has been successfully created";
                }
                else
                {
                    result.Message = "Sorry! something went worng";
                }

            }
            return result;
        }

        public MethodFeedbackVM Update(RecruitmentVM modelVM)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            var exist = _context.RecruitmentRequisitions.FirstOrDefault(x=> x.IsActive && x.Id ==  modelVM.Id);
            if (exist != null)
            {
                //exist.CompanyId = modelVM.CompanyId;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                exist.Remarks = modelVM.Remarks;
                //exist.Status = (int)StatusEnum.Active;
                //exist.RequisitionStatus = (int)RecruitmentStatusEnum.Draft;
                if (modelVM.RequisitionDate > DateTime.MinValue && modelVM.RequisitionDate <= DateTime.MaxValue)
                {

                    exist.RequisitionDate = modelVM.RequisitionDate;
                }
                int status =  _context.SaveChanges();
                if (status >= 1)
                {
                    result.Status = true;
                    result.Message = "Your requisition has been successfully updated";
                }
                else
                {
                    result.Message = "Sorry! something went worng";
                }
            }
            return  result;
        }
        public MethodFeedbackVM UpdateStatus(int Id, RecruitmentStatusEnum status, out RecruitmentRequisition model)
        {
            var result = new MethodFeedbackVM();
            model = _context.RecruitmentRequisitions.FirstOrDefault(x=> x.Id == Id);
            if (model != null)
            {
                model.Status = (int)status;
                model.ModifiedBy = Common.GetUserId();
                model.ModifiedDate = DateTime.Now;
              
                if (_context.SaveChanges() > 0)
                {
                    result.Status = true;
                    result.Message = "Your requisition has been successfully updated";

                    if (status == RecruitmentStatusEnum.Open)
                    {
                        //long[] approvarList = new long[6];
                        var hOdId = Common.GetHRAdminId();
                        var gmdId = Common.GetGroupManagingDirectorId();
                        List<long> approvarList = new List<long>();

                        approvarList.Add(gmdId);
                        approvarList.Add(hOdId);

                        var detailsList = _context.RecruitmentRequisitionDetails.Where(x => x.RecruitmentRequisitionId == Id && x.IsActive).AsNoTracking().ToList();
                        if (detailsList != null && detailsList.Count() > 0)
                        {
                            var managerIdList = detailsList.Select(x => x.ReportingManagerId).Distinct().ToList();
                            //add managerlist to approver list
                            approvarList.AddRange(managerIdList);

                            var companyHeadList = detailsList.Where(x => x.BusinessType == (int)BusinessTypeEnum.Company).Select(x => x.CompanyId).Distinct();
                            var departmentHeadList = detailsList.Where(x => x.BusinessType == (int)BusinessTypeEnum.Division).Select(x => x.DepartmentId).Distinct();

                            if (companyHeadList != null && companyHeadList.Count() > 0)
                            {
                                var companyHeadEmployeeId = _context.BusinessHeads.Where(x => x.IsActive && x.BusinessType == (int)BusinessTypeEnum.Company && companyHeadList.Any(y => y == x.BusineesId_Fk)
                                                              && !approvarList.Any(a => a == x.EmployeeId)
                                                             ).Select(x => x.EmployeeId).ToList();
                                approvarList.AddRange(companyHeadEmployeeId);
                            }
                            if (departmentHeadList != null && departmentHeadList.Count() > 0)
                            {
                                var ids = _context.BusinessHeads.Where(x => x.IsActive && x.BusinessType == (int)BusinessTypeEnum.Division && departmentHeadList.Any(y => y == x.BusineesId_Fk)
                                            && !approvarList.Any(a => a == x.EmployeeId)
                                            ).Select(x => x.EmployeeId).ToList();
                                approvarList.AddRange(ids);
                            }

                            approvarList.Reverse();

                            int precedence = 0;
                            foreach (var item in approvarList)
                            {
                                precedence++;
                                RecruitmentRequisitionApproval approval = new RecruitmentRequisitionApproval()
                                {
                                    CreatedBy = Common.GetUserId(),
                                    CreatedDate = DateTime.Now,
                                    IsActive = true,
                                    EmployeeId = item,
                                    RecruitmentRequisitionId = Id,
                                    Status = (int)SignatoryStatusEnum.Pending,
                                    Precedence = precedence
                                };
                                _context.RecruitmentRequisitionApprovals.Add(approval);
                                _context.SaveChanges();
                            }
                            //RecruitmentRequisitionApproval approvalHod = new RecruitmentRequisitionApproval()
                            //{
                            //    CreatedBy = Common.GetUserId(),
                            //    CreatedDate = DateTime.Now,
                            //    IsActive = true,
                            //    EmployeeId = hOdId,
                            //    RecruitmentRequisitionId = Id,
                            //    Status = (int)SignatoryStatusEnum.Pending,
                            //    Precedence = ++precedence
                            //};
                            //_context.RecruitmentRequisitionApprovals.Add(approvalHod);

                            //RecruitmentRequisitionApproval approvalGmd = new RecruitmentRequisitionApproval()
                            //{
                            //    CreatedBy = Common.GetUserId(),
                            //    CreatedDate = DateTime.Now,
                            //    IsActive = true,
                            //    EmployeeId = gmdId,
                            //    RecruitmentRequisitionId = Id,
                            //    Status = (int)SignatoryStatusEnum.Pending,
                            //    Precedence = ++precedence
                            //};
                            //_context.RecruitmentRequisitionApprovals.Add(approvalGmd);
                            //_context.SaveChanges();
                        }

                    }



                }
                else
                {
                    result.Message = "Sorry! Something went worng.";
                }
            }
            else
            {
                result.Message = "Sorry! No record found.";
            }
            return result;
        }
        public MethodFeedbackVM AddRecruitmentItemDetail(RecruitmentVM modelVM)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            RecruitmentRequisitionDetail rqsDetails = new RecruitmentRequisitionDetail();

            rqsDetails.BusinessType = (int)modelVM.BusinessType;
            rqsDetails.CompanyId = modelVM.CompanyId;
            rqsDetails.DepartmentId = modelVM.DepartmentId;


            rqsDetails.IsActive = true;
            rqsDetails.CreatedDate = DateTime.Now;
            rqsDetails.CreatedBy = Common.GetUserId();
            rqsDetails.JobNature = (int)modelVM.JobNature;
            rqsDetails.JobType = (int)modelVM.JobType;
            rqsDetails.VacancyType = (int)modelVM.VacancyType;
            rqsDetails.NumberOfVacancy = modelVM.NumberOfRecruitment;
            rqsDetails.JobTitle = modelVM.JobTitle;
            rqsDetails.JobLocation = modelVM.JobLocation;
            rqsDetails.RecruitmentRequisitionId = modelVM.Id;
            rqsDetails.AdvertisementMedia = modelVM.Advertisement;
            rqsDetails.ExpectedJoiningDate = modelVM.ExpectedDateOfJoining;
            rqsDetails.ReportingManagerId = modelVM.ManagerId;
            rqsDetails.TargetSalary = modelVM.TargetSalary;
            _context.RecruitmentRequisitionDetails.Add(rqsDetails);
            if ( _context.SaveChanges() > 0)
            {
                modelVM.RequisitionDetailId = rqsDetails.Id;
                result.Status = true;
                result.Message = "This item has been successfully saved";
            }
            else
            {
                result.Message = "Sorry! Something went wrong,plz try later.";
            }
            return result;
        }
        public MethodFeedbackVM UpdateRecruitmentItemDetail(RecruitmentVM modelVM)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            RecruitmentRequisitionDetail rqsDetails = _context.RecruitmentRequisitionDetails.FirstOrDefault(x => x.RecruitmentRequisitionId == modelVM.Id && x.Id == modelVM.RequisitionDetailId);
            if (rqsDetails != null)
            {
                // rqsDetails.Status = (int)StatusEnum.Active;

                rqsDetails.BusinessType = (int)modelVM.BusinessType;
                rqsDetails.CompanyId = modelVM.CompanyId;
                rqsDetails.DepartmentId = modelVM.DepartmentId;


                rqsDetails.ModifiedDate = DateTime.Now;
                rqsDetails.ModifiedBy = Common.GetUserId();
                rqsDetails.JobNature = (int)modelVM.JobNature;
                rqsDetails.JobType = (int)modelVM.JobType;
                rqsDetails.VacancyType = (int)modelVM.VacancyType;
                rqsDetails.NumberOfVacancy = modelVM.NumberOfRecruitment;
                rqsDetails.JobTitle = modelVM.JobTitle;
                rqsDetails.JobLocation = modelVM.JobLocation;
                rqsDetails.RecruitmentRequisitionId = modelVM.Id;
                rqsDetails.AdvertisementMedia = modelVM.Advertisement;
                //rqsDetails.DepartmentId = (int)modelVM.DepartmentId;
                rqsDetails.ExpectedJoiningDate = modelVM.ExpectedDateOfJoining;
                rqsDetails.ReportingManagerId = modelVM.ManagerId;
                rqsDetails.TargetSalary = modelVM.TargetSalary;
                rqsDetails.JobDetails = modelVM.JobDetails;
                if (_context.SaveChanges() > 0)
                {
                    result.Status = true;
                    result.Message = "This item has been successfully updated";
                }
                else
                {
                    result.Message = "Sorry! Something went wrong,plz try later.";
                }
            }
            else
            {
                result.Message = "Sorry! No record found.";
            }
            return result;
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
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }

            }
            return false;
        }
        public MethodFeedbackVM Remove(int id,out RecruitmentRequisition exist)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            exist = _context.RecruitmentRequisitions.FirstOrDefault(x => x.IsActive && x.Id == id);
            if (exist != null)
            {
                exist.IsActive = false;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    result.Status = true;
                    result.Message = "Your requisition has been successfully deleted";
                }
                else
                {
                    result.Message = "Sorry! something went worng";
                }
            }
            else
            {
                result.Message = "Sorry! No requisition found.";
            }
            
            return result;
        }
        public MethodFeedbackVM RemoveRequisitionDetials(int id, out RecruitmentRequisitionDetail model)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            model = _context.RecruitmentRequisitionDetails.FirstOrDefault(x => x.IsActive && x.Id == id);
            if (model != null)
            {
                model.IsActive = false;
                model.ModifiedBy = Common.GetUserId();
                model.ModifiedDate = DateTime.Now;
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    result.Status = true;
                    result.Message = "Your requisition has been successfully deleted";
                }
                else
                {
                    result.Message = "Sorry! something went worng";
                }
            }
            else
            {
                result.Message = "Sorry! No requisition found.";
            }

            return result;
        }
        public IEnumerable<RecruitmentVM> GetRequisitionListChildwise(RecruitmentStatusEnum? status, DateTime? fromDate, DateTime? toDate, int? companyId = 0)
        {            
            var data = (from r in _context.RecruitmentRequisitions                       
                        join rd in _context.RecruitmentRequisitionDetails on r.Id equals rd.RecruitmentRequisitionId
                        join c in _context.Companies on rd.CompanyId equals c.CompanyId
                      join d in _context.Departments on rd.DepartmentId equals d.DepartmentId
                       join manager in _context.Employees on rd.ReportingManagerId equals manager.Id
                       join emp in _context.Employees on r.CreatedBy equals emp.EmployeeId
                       where r.Status == (int)StatusEnum.Active
                       && (status == null || r.Status == (int?)status)
                       select new RecruitmentVM()
                       {
                           Id = r.Id,
                           CompanyId = rd.CompanyId.Value,
                           CompanyName = c.ShortName,
                           Remarks = r.Remarks,
                           RequisitionDate = r.RequisitionDate,
                           ManagerId = (int)rd.ReportingManagerId,
                           ManagerName = manager.Name,
                           NumberOfRecruitment = rd.NumberOfVacancy,
                           Advertisement = rd.AdvertisementMedia,
                           DepartmentId = (int)rd.DepartmentId,
                           DepartmentName = d.Name,
                           JobDetails = rd.JobDetails,
                           TargetSalary =rd.TargetSalary,
                           ExpectedDateOfJoining = rd.ExpectedJoiningDate,
                           JobTitle =rd.JobTitle,
                           JobLocation = rd.JobLocation,
                           JobNature = (JobNatureEnum)rd.JobNature,
                           JobType = (JobTypeEnum)rd.JobType,
                           RecruitmentStatus = (RecruitmentStatusEnum)r.Status,
                           VacancyType = (VacancyTypeEnum)rd.VacancyType
                       }).AsEnumerable();
            return data;

        }

        public IEnumerable<RecruitmentVM> GetRequisitionList(RecruitmentStatusEnum? status, DateTime? fromDate, DateTime? toDate, int? companyId = 0)
        {
  
            var data = (from r in _context.RecruitmentRequisitions
                        join creator in _context.Employees on r.CreatedBy equals creator.EmployeeId
                        where r.IsActive
                        select new RecruitmentVM()
                        {
                            Id = r.Id,
                            Remarks = r.Remarks,
                            RequisitionNumber = r.RequisitionNumber,
                            RequisitionDate = r.RequisitionDate,
                            EmployeeId = creator.EmployeeId,
                            EmployeeName = creator.Name,
                            RecruitmentStatus = (RecruitmentStatusEnum)r.Status,
                            RecruitmentDetailsList = (from a in _context.RecruitmentRequisitionDetails
                                                      join manager in _context.Employees on a.ReportingManagerId equals manager.Id
                                                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                                                      into company
                                                      from c in company.DefaultIfEmpty()
                                                      join d in _context.Departments on a.DepartmentId equals d.DepartmentId
                                                      into department
                                                      from d in department.DefaultIfEmpty()
                                                      where a.IsActive && a.RecruitmentRequisitionId == r.Id
                                                      select new RecruitmentDetailsVM()
                                                      {
                                                          RequisitionDetailId = a.Id,
                                                          NumberOfRecruitment = a.NumberOfVacancy,
                                                          Advertisement = a.AdvertisementMedia,
                                                          DepartmentName = d.Name,
                                                          DepartmentId = d.DepartmentId,
                                                          CompanyId = a.CompanyId,
                                                          CompanyName = c.Name,
                                                          BusinessType = (BusinessTypeEnum)a.BusinessType,

                                                          JobDetails = a.JobDetails,
                                                          TargetSalary = a.TargetSalary,
                                                          ExpectedDateOfJoining = a.ExpectedJoiningDate,
                                                          JobTitle = a.JobTitle,
                                                          JobLocation = a.JobLocation,
                                                          JobNature = (JobNatureEnum)a.JobNature,
                                                          JobType = (JobTypeEnum)a.JobType,
                                                          VacancyType = (VacancyTypeEnum)a.VacancyType,

                                                          ManagerId = manager.Id,
                                                          ManagerName = manager.Name
                                                      }).ToList()

                        }).AsNoTracking().ToList();



            return data;

        }

        public RecruitmentVM GetById(int id)
        {


            var recruitmentVM = (from r in _context.RecruitmentRequisitions
                                join e in _context.Employees on r.CreatedBy equals e.EmployeeId
                                where r.Id == id && r.IsActive
                                select new RecruitmentVM()
                                {
                                    Id = r.Id,
                                    RequisitionNumber = r.RequisitionNumber,
                                    Remarks = r.Remarks,
                                    RecruitmentStatus = (RecruitmentStatusEnum)r.Status,
                                    RequisitionDate = r.RequisitionDate,
                                    CreatedBy =  e.Name+"("+r.CreatedBy+")",
                                    EmployeeId = e.EmployeeId,
                                    EmployeeName = e.Name
                                }).FirstOrDefault();

            if (recruitmentVM != null)
            {

                var childs = _context.RecruitmentRequisitionDetails.Where(x => x.RecruitmentRequisitionId == id && x.IsActive).AsNoTracking().ToList();
                if (childs != null)
                {
                    recruitmentVM.RecruitmentDetailsList = (from a in childs
                                                            join manager in _context.Employees on a.ReportingManagerId equals manager.Id
                                              join c in _context.Companies on a.CompanyId equals c.CompanyId
                                              into company
                                              from c in company.DefaultIfEmpty()
                                              join d in _context.Departments on a.DepartmentId equals d.DepartmentId
                                              into department
                                              from d in department.DefaultIfEmpty()
                                              select new RecruitmentDetailsVM()
                                              {
                                                  RequisitionDetailId = a.Id,
                                                  NumberOfRecruitment =a.NumberOfVacancy,
                                                  Advertisement = a.AdvertisementMedia,
                                                  DepartmentName = d?.Name,
                                                  DepartmentId = d?.DepartmentId,
                                                  CompanyId = a?.CompanyId,
                                                  CompanyName = c?.Name,
                                                  BusinessType = (BusinessTypeEnum)a.BusinessType,

                                                  JobDetails =a.JobDetails,
                                                  TargetSalary =a.TargetSalary,
                                                  ExpectedDateOfJoining =a.ExpectedJoiningDate,
                                                  JobTitle =a.JobTitle,
                                                  JobLocation =a.JobLocation,
                                                  JobNature = (JobNatureEnum)a.JobNature,
                                                  JobType = (JobTypeEnum)a.JobType,
                                                  VacancyType = (VacancyTypeEnum)a.VacancyType,

                                                  ManagerId = manager.Id,
                                                  ManagerName = manager.Name

                                              }).ToList();
                }



                if (recruitmentVM.RecruitmentStatus >  RecruitmentStatusEnum.Draft)
                {
                    recruitmentVM.RequisitionApprovalLists = (from a in _context.RecruitmentRequisitionApprovals
                                                              join emp in _context.Employees on a.EmployeeId equals emp.Id
                                                              join des in _context.Designations on emp.DesignationId equals des.DesignationId                                                           
                                                              where a.RecruitmentRequisitionId == id && a.IsActive
                                                              select new RequisitionApprovalVM()
                                                              {
                                                                  EmployeeId = emp.Id,
                                                                  EmployeeIdString = emp.EmployeeId,
                                                                  EmployeeName = emp.Name,
                                                                  DesignationName = des.Name,
                                                                  Status = (SignatoryStatusEnum)a.Status ,                                                       
                                                                  ApprovedTime = a.ModifiedDate.HasValue ? a.ModifiedDate.Value.ToString() : "...."
                                                              }).ToList();
                }
            }

            return recruitmentVM;



            //var data = (from r in _context.RecruitmentRequisitions
            //            join rd in _context.RecruitmentRequisitionDetails on r.Id equals rd.RecruitmentRequisitionId
            //            join c in _context.Companies on r.CompanyId equals c.CompanyId/
                
            //            join d in _context.Departments on rd.DepartmentId equals d.DepartmentId
            //            join manager in _context.Employees on rd.ReportingManagerId equals manager.Id
            //            join emp in _context.Employees on r.CreatedBy equals emp.EmployeeId
            //            where r.Status == (int)StatusEnum.Active && r.Id == id
            //            select new RecruitmentVM()
            //            {
            //                Id = r.Id,
            //                CompanyId = r.CompanyId,
            //                CompanyName = c.ShortName,
            //                Remarks = r.Remarks,
            //                RequisitionDate = r.RequisitionDate,
            //                ManagerId = (int)rd.ReportingManagerId,
            //                ManagerName = manager.Name,
            //                NumberOfRecruitment = rd.NumberOfVacancy,
            //                Advertisement = rd.AdvertisementMedia,
            //                DepartmentId = (int)rd.DepartmentId,
            //                DepartmentName = d.Name,
            //                JobDetails = rd.JobDetails,
            //                TargetSalary = rd.TargetSalary,
            //                ExpectedDateOfJoining = rd.ExpectedJoiningDate,
            //                JobTitle = rd.JobTitle,
            //                JobLocation = rd.JobLocation,
            //                JobNature = (JobNatureEnum)rd.JobNature,
            //                JobType = (JobTypeEnum)rd.JobType,
            //                RecruitmentStatus = (RecruitmentStatusEnum)r.RequisitionStatus,
            //                VacancyType = (VacancyTypeEnum)rd.VacancyType,
            //                CreatedBy = emp.Name
            //            }).FirstOrDefault();


            //var data = (from r in _context.RecruitmentRequisitions
            //            join rd in _context.RecruitmentRequisitionDetails on r.Id equals rd.RecruitmentRequisitionId
            //            join c in _context.Companies on rd.CompanyId equals c.CompanyId
            //            join d in _context.Departments on rd.DepartmentId equals d.DepartmentId
            //            join manager in _context.Employees on rd.ReportingManagerId equals manager.Id
            //            join emp in _context.Employees on r.CreatedBy equals emp.EmployeeId
            //            where r.Id == id && rd.IsActive
            //            group new { rd, d, manager,emp } by new { r, c } into grouped
            //            select new RecruitmentVM()
            //            {
            //                Id = grouped.Key.r.Id,
            //                CompanyId = grouped.Key.c.CompanyId,
            //                CompanyName = grouped.Key.c.ShortName,
            //                Remarks = grouped.Key.r.Remarks,
            //                RecruitmentStatus = (RecruitmentStatusEnum)grouped.Key.r.Status,
            //                RequisitionDate = grouped.Key.r.RequisitionDate,
            //                CreatedBy = grouped.Key.r.CreatedBy,
            //                RecruitmentDetailsList =
            //                (grouped.Select(x => new RecruitmentDetailsVM()
            //                {
            //                    RequisitionDetailId = x.rd.Id,
            //                    NumberOfRecruitment = x.rd.NumberOfVacancy,
            //                    Advertisement = x.rd.AdvertisementMedia,
            //                    DepartmentName = x.d.Name,
            //                    JobDetails = x.rd.JobDetails,
            //                    TargetSalary = x.rd.TargetSalary,
            //                    ExpectedDateOfJoining = x.rd.ExpectedJoiningDate,
            //                    JobTitle = x.rd.JobTitle,
            //                    JobLocation = x.rd.JobLocation,
            //                    JobNature = (JobNatureEnum)x.rd.JobNature,
            //                    JobType = (JobTypeEnum)x.rd.JobType,
            //                    VacancyType = (VacancyTypeEnum)x.rd.VacancyType,
            //                })).ToList()

            //            }).FirstOrDefault();
            //return data;
        }

        public RecruitmentDetailsVM GetRequisitionItemDetailById(int id)
        {
            var data = (
                        from rd in _context.RecruitmentRequisitionDetails
                        join d in _context.Departments on rd.DepartmentId equals d.DepartmentId
                        into department from d in department.DefaultIfEmpty()
                        join c in _context.Companies on rd.CompanyId equals c.CompanyId
                        into company from c in company.DefaultIfEmpty()
                        join manager in _context.Employees on rd.ReportingManagerId equals manager.Id
                        join emp in _context.Employees on rd.CreatedBy equals emp.EmployeeId
                        where rd.Id == id && rd.IsActive
                        select new RecruitmentDetailsVM()
                        {
                            RequisitionDetailId = rd.Id,
                                BusinessType = (BusinessTypeEnum)rd.BusinessType,
                                BusineesId_Fk = (BusinessTypeEnum)rd.BusinessType == BusinessTypeEnum.Company?rd.CompanyId.Value:rd.DepartmentId.Value,
                                RecruitmentId_Fk = rd.RecruitmentRequisitionId,
                                NumberOfRecruitment = rd.NumberOfVacancy,
                                Advertisement = rd.AdvertisementMedia,
                                //DepartmentId = d?.DepartmentId,
                                ManagerId = (int)rd.ReportingManagerId,
                                ManagerName = manager.Name,
                                DepartmentName = d.Name,
                                JobDetails = rd.JobDetails,
                                TargetSalary = rd.TargetSalary,
                                ExpectedDateOfJoining = rd.ExpectedJoiningDate,
                                JobTitle = rd.JobTitle,
                                JobLocation = rd.JobLocation,
                                JobNature = (JobNatureEnum)rd.JobNature,
                                JobType = (JobTypeEnum)rd.JobType,
                                VacancyType = (VacancyTypeEnum)rd.VacancyType,
                               
                           
                        }).FirstOrDefault();
            return data;
        }


        public List<RecruitmentApprovalVM> GetApprovalList(SignatoryStatusEnum? signatoryStatus, DateTime? fromDate, DateTime? toDate)
        {
            var userId = Common.GetIntUserId();
            //userId = 42146;
            var data = (from a in _context.RecruitmentRequisitionApprovals
                        join r in _context.RecruitmentRequisitions on a.RecruitmentRequisitionId equals r.Id
                        join fromEmployee in _context.Employees on r.CreatedBy equals fromEmployee.EmployeeId
                        where a.EmployeeId == userId && a.IsActive
                        &&
                        (signatoryStatus == null || a.Status == (int?)signatoryStatus)
                        && (fromDate == null || r.RequisitionDate >= fromDate)
                        && (toDate == null || r.RequisitionDate <= toDate)
                        select new RecruitmentApprovalVM()
                        {
                            RecruitmentId = r.Id,
                            RecruitmentApprovalId = a.Id,
                            RecruitmentNumber = r.RequisitionNumber,
                            EmployeeId = fromEmployee.EmployeeId,
                            EmployeeName = fromEmployee.Name,
                            RecruitmentDate = r.RequisitionDate,
                            Remarks = r.Remarks,
                            SignatoryStatus = (SignatoryStatusEnum)a.Status,
                            RecruitmentRequisitionSummaryList = _context.RecruitmentRequisitionDetails.Where(rd => rd.RecruitmentRequisitionId == r.Id && rd.IsActive).Select(x=> new RecruitmentRequisitionSummaryVM()
                            {
                                NumberOfRecruitment = x.NumberOfVacancy,
                                Salary = x.TargetSalary * x.NumberOfVacancy
                            }).ToList()
                        })
                      .AsEnumerable() // Switch to in-memory operations
                      .Select(x => new RecruitmentApprovalVM
                      {
                          RecruitmentId = x.RecruitmentId,
                          RecruitmentApprovalId = x.RecruitmentApprovalId,
                          SignatoryStatus = x.SignatoryStatus,
                          RecruitmentNumber = x.RecruitmentNumber,
                          EmployeeId = x.EmployeeId,
                          EmployeeName = x.EmployeeName,
                          RecruitmentDate = x.RecruitmentDate,
                          Remarks = x.Remarks,
                          TotalNumberOfRecruitment = x.RecruitmentRequisitionSummaryList.Sum(y=> y.NumberOfRecruitment),
                          TotalSalary = x.RecruitmentRequisitionSummaryList.Sum(y => y.Salary)
                      })
                      .ToList();


            return data;
        }
        public List<RequisitionApprovalVM> GetRecruitmentSignatoryList(int id)
        {
            var requisitionList = (from mapping in _context.RecruitmentRequisitionApprovals
                                   join emp in _context.Employees on mapping.EmployeeId equals emp.Id
                                   join des in _context.Designations on emp.DesignationId equals des.DesignationId
                                   where mapping.RecruitmentRequisitionId == id
                                   && mapping.IsActive
                                   select new RequisitionApprovalVM()
                                   {
                                       EmployeeId = emp.Id,
                                       EmployeeIdString = emp.EmployeeId,
                                       EmployeeName = emp.Name,
                                       DesignationName = des.Name,
                                       OrderBy = 1,
                                      // Comment = mapping.Note,
                                       Status = (SignatoryStatusEnum)mapping.Status,
                                       StatusString = ((SignatoryStatusEnum)mapping.Status).ToString(),
                                       //StatusString = (from rsa in _context.RequisitionSignatoryApprovals
                                       //                where rsa.RequisitionId == a.RequisitionId
                                       //                && (a.OrderBy <= 1 || (rsa.OrderBy == (a.OrderBy - 1) && rsa.Status == (int)SignatoryStatusEnum.Approved))
                                       //                select rsa).FirstOrDefault() != null ? ((SignatoryStatusEnum)a.Status).ToString() : "...",
                                       ApprovedTime = mapping.ModifiedDate.HasValue ? mapping.ModifiedDate.Value.ToString() : "...."
                                   }).OrderBy(x => x.OrderBy).ToList();


            return requisitionList;
        }

        public bool UpdateRecruitmentSignatoryApprovalStatus(long id, SignatoryStatusEnum status, string comment)
        {
            var exist = _context.RecruitmentRequisitionApprovals.FirstOrDefault(x => x.Id == id && x.IsActive);
            if (exist != null)
            {
                exist.Status = (int)status;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }

            }

            return false;
        }
        public string GetRecruitmentNumber()
        {
            var totalCount =  _context.RecruitmentRequisitions.Count(x=> x.IsActive) + 1;

            return "R0" + totalCount;
        }
    }
}
