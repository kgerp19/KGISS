using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Implementation.EmployeeResults;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.EmployeeAppResult
{
    public class EmployeeResultService : IEmployeeResultService
    {
        private readonly ERPEntities _db;

        public EmployeeResultService(ERPEntities db)
        {
            _db = db;
        }

        public async Task<AnnualPerformanceEmpResultVM> AnnualPerformanceEmployeeResult(long AnnualPerformanceId, CancellationToken cancellationToken = default)
        {
            var resultData = await _db.AnnualPerformances
                                      .Where(x => x.IsActive && x.AnnualPerformanceId == AnnualPerformanceId)
                                      .Select(x => new
                                      {
                                          x.AnnualPerformanceId,
                                          x.APRNO,
                                          x.AssessmentFrom,
                                          x.AssessmentTo,
                                          x.Remarks,
                                          x.AprEndDate
                                      })
                                      .FirstOrDefaultAsync(cancellationToken);

            if (resultData == null)
            {
                return null;
            }

            return new AnnualPerformanceEmpResultVM
            {
                AnnualPerformanceId = resultData.AnnualPerformanceId,
                APRNO = resultData.APRNO,
                AssessmentFrom = resultData.AssessmentFrom,
                AssessmentTo = resultData.AssessmentTo,
                Remarks = resultData.Remarks,
                AprEndingDate = resultData.AprEndDate ?? DateTime.Now.AddDays(15)
            };
        }


        public async Task<long> AnnualPerformanceEmployeeResultSaveAndUpdate(AnnualPerformanceEmpResultVM model, CancellationToken cancellationToken = default)
        {
            long AnnualPerformanceId = 0;
            if (model != null && model.AnnualPerformanceId <= 0)
            {
                AnnualPerformance dbEntity = new AnnualPerformance();
                dbEntity.APRNO = model.APRNO;
                dbEntity.AssessmentFrom = model.AssessmentFrom;
                dbEntity.AssessmentTo = model.AssessmentTo;
                dbEntity.Remarks = model.Remarks;
                dbEntity.CreatedBy = Common.GetUserId();
                dbEntity.CreatedDate = DateTime.Now.Date;
                dbEntity.IsActive = true;
                dbEntity.IsSubmitted = true;
                dbEntity.AprEndDate = model.AprEndingDate;
                _db.AnnualPerformances.Add(dbEntity);
                if (await _db.SaveChangesAsync() > 0)
                {
                    AnnualPerformanceId = dbEntity.AnnualPerformanceId;
                }
            }
            else if (model != null && model.AnnualPerformanceId > 0)
            {
                var dbData = await _db.AnnualPerformances.FirstOrDefaultAsync(x => x.IsActive && x.AnnualPerformanceId == model.AnnualPerformanceId);
                if (dbData != null)
                {
                    dbData.APRNO = model.APRNO;
                    dbData.AssessmentFrom = model.AssessmentFrom;
                    dbData.AssessmentTo = model.AssessmentTo;
                    dbData.Remarks = model.Remarks;
                    dbData.ModifiedBy = Common.GetUserId();
                    dbData.ModifiedDate = DateTime.Now.Date;
                    dbData.AprEndDate = model.AprEndingDate;
                    _db.Entry(dbData).State = EntityState.Modified;
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        AnnualPerformanceId = dbData.AnnualPerformanceId;
                    }
                }
            }
            return AnnualPerformanceId;
        }

        public async Task<RResult> DeleteEducationById(int EduId, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            var dbData = await _db.Educations.FirstOrDefaultAsync(x => x.EducationId == EduId, cancellationToken);

            if (dbData != null)
            {
                dbData.IsActive = false;
                _db.Entry(dbData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync(cancellationToken) > 0)
                {
                    rResult.result = 1;
                    rResult.longId = 1;
                    rResult.message = "Delete Successfully";
                }

            }
            else
            {
                rResult.result = 0;
                rResult.longId = 1;
                rResult.message = "Delete Failed!";
            }

            return rResult;
        }

        public async Task<RResult> EducationContentSave(int DropDownTypeId, string Name, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            if (DropDownTypeId > 0)
            {
                bool isDuplicateChk = await _db.DropDownItems.AnyAsync(x => x.DropDownTypeId == DropDownTypeId &&
                   x.Name.Replace(" ", "").ToLower() == Name.Replace(" ", "").ToLower());

                if (isDuplicateChk)
                {
                    rResult.result = 0;
                    rResult.message = "Already exist!";
                    rResult.longId = 2;
                    return rResult;
                }
                else
                {
                    DropDownItem mode = new DropDownItem();
                    mode.DropDownTypeId = DropDownTypeId;
                    mode.Name = Name;
                    mode.CreatedBy = Common.GetUserId();
                    mode.CreatedDate = DateTime.Now.Date;
                    mode.IsActive = true;
                    _db.DropDownItems.Add(mode);
                    if (await _db.SaveChangesAsync(cancellationToken) > 0)
                    {
                        rResult.result = 1;
                        rResult.message = "Data Save Successfully!";
                        rResult.longId = 2;
                    }
                    else
                    {
                        rResult.result = 0;
                        rResult.message = "Data can't Save Successfully!";
                        rResult.longId = 2;
                    }
                }

                
            }
            return rResult;
        }

        public async Task<AnnualPerformanceEmpResultVM> GetAnnualPerformanceEmployeeResultList(long EmpId = 0, CancellationToken cancellationToken = default)
        {
            var model = new AnnualPerformanceEmpResultVM();

            var query = from ap in _db.AnnualPerformances.Where(x => x.IsActive)
                        join apd in _db.AnnualPerformanceDetails.Where(x => x.IsActive && x.EmployeeId == EmpId)
                        on ap.AnnualPerformanceId equals apd.AnnualPerformanceId into details
                        from apd in details.DefaultIfEmpty()
                        select new AnnualPerformanceEmpResultVM
                        {
                            AnnualPerformanceId = ap.AnnualPerformanceId,
                            APRNO = ap.APRNO,
                            AssessmentFrom = ap.AssessmentFrom,
                            AssessmentTo = ap.AssessmentTo,
                            AnnualPerformanceDetailId = apd.AnnualPerformanceDetailId,
                            Remarks = ap.Remarks,
                            AprEndingDate=ap.AprEndDate.Value
                            
                        };

            model.GetDDLAnnualPerformanceEmpResult = await query.ToListAsync(cancellationToken);
            return model;
        }


        public async Task<RResult> GetAnnualPerformanceEmployeeResultDelete(long AnnualPerformanceId, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            if (AnnualPerformanceId > 0)
            {
                var dbEntity = await _db.AnnualPerformances.FirstOrDefaultAsync(x => x.IsActive && x.AnnualPerformanceId == AnnualPerformanceId);
                if (dbEntity != null)
                {
                    dbEntity.IsActive = false;
                    _db.Entry(dbEntity).State = EntityState.Modified;
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        rResult.result = 1;
                        rResult.message = "Date Deleted Successfully!";
                    }
                    else
                    {
                        rResult.result = 0;
                        rResult.message = "Date Can't Deleted Successfully!";
                    }
                }
            }
            return rResult;
        }

        public async Task<EmployeeResultsVM> GetEmployeeDataByEmpId(long EmpId, long AnnualPerformanceId, long annualPerformanceDetailId, CancellationToken cancellationToken = default)
        {
            try
            {
                EmployeeResultsVM modes = new EmployeeResultsVM();

                modes = await (from t1 in _db.Employees
                               join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                               join t3 in _db.Grades on t1.GradeId equals t3.GradeId
                               join t4 in _db.Departments on t1.DepartmentId equals t4.DepartmentId
                               join t5 in _db.Designations on t1.DesignationId equals t5.DesignationId
                               join t6 in _db.Employees on t1.ManagerId equals t6.Id
                               join t7 in _db.DropDownItems on t1.MaritalTypeId equals t7.DropDownItemId into t7Group
                               from t7 in t7Group.DefaultIfEmpty()
                               join t11 in _db.AnnualPerformances on AnnualPerformanceId equals t11.AnnualPerformanceId into t11Group
                               from t11 in t11Group.DefaultIfEmpty()
                               join t9 in _db.AnnualPerformanceDetails on new { AnnualPerformanceId = t11.AnnualPerformanceId, EmployeeId = EmpId }
                               equals new { AnnualPerformanceId = t9.AnnualPerformanceId, EmployeeId = t9.EmployeeId } into t9Group
                               from t10 in t9Group.DefaultIfEmpty()
                               where t1.Active && t1.Id == EmpId
                               select new EmployeeResultsVM
                               {
                                   EmployeeId = t1.Id,
                                   EmployeeCode = t1.EmployeeId,
                                   FullName = t1.Name,
                                   CompanyName = t2.Name,
                                   Department = t4.Name == null ? "N/A" : t4.Name,
                                   Designation = t5.Name,
                                   ManagerName = t6.Name,
                                   AppointmentDate = t1.JoiningDate,
                                   DateOfBirth = t1.DateOfBirth,
                                   GradeName = t3.Name,
                                   MaritalStatus = t7.Name,
                                   CompanyId = t10.CompanyId ?? 0,
                                   DepartmentId = t10.DepartmentId ?? 0,
                                   PerformanceInReportingYear = t10.PerformanceInReportingYear,
                                   AdditionalPerformance = t10.AdditionalPerformance,
                                   PlanforNextYear = t10.PlanforNextYear,
                                   AssessmentFrom = t11.AssessmentFrom == null ? DateTime.Now : t11.AssessmentFrom,
                                   AssessmentTo = t11.AssessmentTo == null ? DateTime.Now : t11.AssessmentTo,
                                   IsSubmited = t10 != null ? t10.IsSubmitted : false,
                                   performingPlanForNextYear=t10.PerformingPlanForNextYear,
                                   MajorWorkOne=t10.MajorWorkOne,
                                   MajorWorkTwo=t10.MajorWorkTwo,
                                   MajorWorkThree=t10.MajorWorkThree,
                                   MajorWorkFour=t10.MajorWorkFour,
                                   AprEndingDate=t11.AprEndDate.Value

                               }).FirstOrDefaultAsync(cancellationToken);

                if (modes.IsSubmited)
                {
                    modes.signatoryApprovalList = await (from sm in _db.SignatoryApprovalMaps
                                                         join e in _db.Employees on sm.EmployeeId equals e.Id
                                                         join d in _db.Designations on e.DesignationId equals d.DesignationId
                                                         where sm.IntregratedFromId == annualPerformanceDetailId
                                                            && sm.TableName == "AnnualPerformanceDetail"
                                                         select new SignatoryApprovalVM
                                                         {
                                                             SignatoryName = e.Name,
                                                             DesignationName = d.Name,
                                                             Status = sm.Status,
                                                             OrderBy = sm.OrderBy
                                                         }).OrderBy(x => x.OrderBy).ToListAsync(cancellationToken);

                }

                modes.EmployeeEqucationList = await (from t1 in _db.Educations
                                                     join t2 in _db.DropDownItems on t1.ExaminationId equals t2.DropDownItemId into t2Group
                                                     from t2 in t2Group.DefaultIfEmpty()
                                                     join t3 in _db.DropDownItems on t1.InstituteId equals t3.DropDownItemId into t3Group
                                                     from t3 in t3Group.DefaultIfEmpty()
                                                     join t4 in _db.DropDownItems on t1.SubjectId equals t4.DropDownItemId into t4Group
                                                     from t4 in t4Group.DefaultIfEmpty()
                                                     where t1.IsActive && t1.Id == EmpId
                                                     select new EmployeeEqucationList
                                                     {
                                                         Subject = t4.Name,
                                                         EducationId = t1.EducationId,
                                                         Qualification = t2 != null ? t2.Name : null,
                                                         Result = t1.Result,
                                                         PassedYear = t1.PassingYear,
                                                         Institute = t3 != null ? t3.Name : null,
                                                         ExaminationDropDownTypeId = t2 != null ? t2.DropDownTypeId.Value : 0,
                                                         SubjectDropDownTypeId = t4 != null ? t4.DropDownTypeId.Value : 0,
                                                         InstituteDropDownTypeId = t3 != null ? t3.DropDownTypeId.Value : 0,


                                                     }).OrderByDescending(x=>x.PassedYear).ToListAsync(cancellationToken);

                modes.ProfileQualitiesList = await (from dt in _db.DropDownTypes
                                                    join di in _db.DropDownItems on dt.DropDownTypeId equals di.DropDownTypeId
                                                    where new[] { 71, 72 }.Contains(dt.DropDownTypeId)
                                                    && dt.IsActive
                                                    && di.IsActive
                                                    select new ProfileQualitiesList
                                                    {
                                                        DropDownTypeId = dt.DropDownTypeId,
                                                        DropDownItemId = di.DropDownItemId,
                                                        ProfileQualitiesGroupName = dt.Name,
                                                        ProfileQualitiesName = di.Name
                                                    }).ToListAsync(cancellationToken);
                return modes;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task<RResult> ProfileQualitiesReportSubmit(EmployeeResultsVM dataModel, CancellationToken cancellationToken = default)
        {

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    string createdBy = Common.GetUserId();
                    if (string.IsNullOrEmpty(createdBy))
                    {
                        return new RResult { result = 1, message = "User is not authenticated.", longId = 1 };
                    }

                    if (dataModel.ProfileQualitiesList == null || !dataModel.ProfileQualitiesList.Any())
                    {
                        return new RResult { result = 1, message = "No profile qualities data provided.", longId = 1 };
                    }

                    var dbEntityDataList = await _db.SignatoryApprovalMaps
                        .Where(x => x.IsActive && x.IntregratedFromId == dataModel.AnnualPerformanceDetailId && x.TableName == "AnnualPerformanceDetail")
                        .OrderBy(x => x.OrderBy)
                        .ToListAsync(cancellationToken);

                    var dbEntityData = dbEntityDataList.FirstOrDefault(x => x.SignatoryApprovalMapId == dataModel.SigAppMapID);
                    if (dbEntityData == null)
                    {
                        return new RResult { result = 1, message = "No matching SignatoryApprovalMap found.", longId = 1 };
                    }

                    dbEntityData.Comment = dataModel.GeneralSpecialRemarks;
                    dbEntityData.Status = 1;
                    dbEntityData.ModifiedBy = createdBy;
                    dbEntityData.ModifiedDate = DateTime.Now;
                    _db.Entry(dbEntityData).State = EntityState.Modified;

                    if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                    {
                        transaction.Rollback();
                        return new RResult { result = 0, message = "Failed to update SignatoryApprovalMap.", longId = 1 };
                    }
                    //int? orderByInc = null;
                    //foreach (var item in dbEntityDataList)
                    //{
                    //    if (dbEntityData.SignatoryApprovalMapId != item.SignatoryApprovalMapId)
                    //    {
                    //        orderByInc = item.OrderBy;
                    //        break;
                    //    }
                    //}
                    int? orderByInc = dbEntityDataList
                                        .OrderBy(n => n.OrderBy)
                                        .SkipWhile(n => n.OrderBy <= dbEntityData.OrderBy)
                                        .Select(n => (int?)n.OrderBy)
                                        .FirstOrDefault();
                    //int orderByInc = dbEntityData.OrderBy + 1;

                    if (orderByInc > 0)
                    {
                        var nextStepOfSignatoryApproval = dbEntityDataList.FirstOrDefault(x => x.OrderBy == orderByInc);
                        if (nextStepOfSignatoryApproval != null)
                        {
                            nextStepOfSignatoryApproval.Status = 0;
                            nextStepOfSignatoryApproval.ModifiedBy = createdBy;
                            //nextStepOfSignatoryApproval.ModifiedDate = DateTime.Now;
                            _db.Entry(nextStepOfSignatoryApproval).State = EntityState.Modified;

                            if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                            {
                                transaction.Rollback();
                                return new RResult { result = 0, message = "Failed to update next step SignatoryApprovalMap.", longId = 1 };
                            }
                        }
                    }

                    var profileQualities = dataModel.ProfileQualitiesList.Select(item => new APRProfileQuality
                    {
                        AnnualPerformanceDetailId = dataModel.AnnualPerformanceDetailId,
                        Score = item.ItemValue,
                        DropDownItemId = item.DropDownItemId,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    }).ToList();

                    _db.APRProfileQualities.AddRange(profileQualities);

                    if (await _db.SaveChangesAsync(cancellationToken) > 0)
                    {
                        var AnnualPerformanceDetailData = await _db.AnnualPerformanceDetails.FirstOrDefaultAsync(x => x.AnnualPerformanceDetailId == dataModel.AnnualPerformanceDetailId);
                        if (AnnualPerformanceDetailData != null)
                        {
                            if (dataModel.SalesTarget > 0 || dataModel.RecoveryTarget > 0)
                            {
                                AnnualPerformanceDetailData.SalesTarget = dataModel.SalesTarget;
                                AnnualPerformanceDetailData.SalesAchievement = dataModel.SalesAchievement;
                                AnnualPerformanceDetailData.RecoveryTarget = dataModel.RecoveryTarget;
                                AnnualPerformanceDetailData.RecoveryAchievement = dataModel.RecoveryAchievement;
                            }
                            AnnualPerformanceDetailData.Recommendation = dataModel.Recommendation;
                            AnnualPerformanceDetailData.IncreaseProMonth = dataModel.IncreaseProMonth;
                            //AnnualPerformanceDetailData.MajorWorkOne = dataModel.MajorWorkOne;
                            //AnnualPerformanceDetailData.MajorWorkTwo = dataModel.MajorWorkTwo;
                            //AnnualPerformanceDetailData.MajorWorkThree = dataModel.MajorWorkThree;
                            //AnnualPerformanceDetailData.MajorWorkFour = dataModel.MajorWorkFour;
                            AnnualPerformanceDetailData.WorkWithReportingOfficer = dataModel.WorkWithReportingOfficer;

                            _db.Entry(AnnualPerformanceDetailData).State = EntityState.Modified;
                            await _db.SaveChangesAsync(cancellationToken);
                        }
        
                        transaction.Commit();
                        return new RResult { result = 1, message = "Profile qualities submitted successfully.", longId = 1 };
                    }

                    transaction.Rollback();
                    return new RResult { result = 0, message = "Failed to submit profile qualities.", longId = 1 };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new RResult { result = 0, message = $"An error occurred while submitting profile qualities report: {ex.Message}" };
                }
            }

        }



        public async Task<RResult> AnnualPerformanceDetailSaveUpdate(EmployeeResultsVM model, CancellationToken cancellationToken = default)
        {
            string CreateBy =Common.GetUserId();
            RResult rResult = new RResult();
            var AprEndDateChk = await _db.AnnualPerformances.FirstAsync(x => x.AnnualPerformanceId == model.AnnualPerformanceId);

            if (model != null && !string.IsNullOrEmpty(CreateBy))
            {
                if (DateTime.Now.Date > AprEndDateChk.AprEndDate)
                {
                    rResult.result = 0;
                    rResult.message = "APR is already closed!<br/> Please Contract Admin And HR";
                }
                else
                {
                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (model.AnnualPerformanceDetailId > 0)
                            {
                                var dbModelUpdate = await _db.AnnualPerformanceDetails
                                                            .FirstOrDefaultAsync(apd => apd.IsActive && apd.AnnualPerformanceDetailId == model.AnnualPerformanceDetailId, cancellationToken);
                                if (dbModelUpdate != null)
                                {

                                    dbModelUpdate.AnnualPerformanceId = model.AnnualPerformanceId;
                                    dbModelUpdate.MajorWorkOne = model.MajorWorkOne;
                                    dbModelUpdate.MajorWorkTwo = model.MajorWorkTwo;
                                    dbModelUpdate.MajorWorkThree = model.MajorWorkThree;
                                    dbModelUpdate.MajorWorkFour = model.MajorWorkFour;
                                    dbModelUpdate.EmployeeId = (long)model.EmployeeId;
                                    dbModelUpdate.CompanyId = model.CompanyId;
                                    dbModelUpdate.DepartmentId = model.DepartmentId;
                                    dbModelUpdate.PerformanceInReportingYear = model.PerformanceInReportingYear;
                                    dbModelUpdate.PlanforNextYear = model.PlanforNextYear;
                                    dbModelUpdate.AdditionalPerformance = model.AdditionalPerformance;
                                    dbModelUpdate.PerformingPlanForNextYear = model.performingPlanForNextYear;
                                    dbModelUpdate.CreatedBy = CreateBy;
                                    dbModelUpdate.CreatedDate = DateTime.Now;
                                    dbModelUpdate.IsActive = true;


                                    _db.Entry(dbModelUpdate).State = EntityState.Modified;
                                    await _db.SaveChangesAsync(cancellationToken);

                                    rResult.returnURL = $"/EmployeeResult/EmployeeResultCreate?annualPerformanceId={model.AnnualPerformanceId}&empId={model.EmployeeId}&annualPerformanceDetailId={model.AnnualPerformanceDetailId}";
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                            }
                            else
                            {
                                AnnualPerformanceDetail dbModel = new AnnualPerformanceDetail
                                {
                                    AnnualPerformanceId = model.AnnualPerformanceId,
                                    MajorWorkOne = model.MajorWorkOne,
                                    MajorWorkTwo = model.MajorWorkTwo,
                                    MajorWorkThree = model.MajorWorkThree,
                                    MajorWorkFour = model.MajorWorkFour,
                                    EmployeeId = (long)model.EmployeeId,
                                    CompanyId = model.CompanyId,
                                    DepartmentId = model.DepartmentId,
                                    PerformanceInReportingYear = model.PerformanceInReportingYear,
                                    PlanforNextYear = model.PlanforNextYear,
                                    AdditionalPerformance = model.AdditionalPerformance,
                                    PerformingPlanForNextYear = model.performingPlanForNextYear,
                                    CreatedBy = CreateBy,
                                    CreatedDate = DateTime.Now,
                                    IsActive = true
                                };

                                _db.AnnualPerformanceDetails.Add(dbModel);
                                await _db.SaveChangesAsync(cancellationToken);
                                rResult.returnURL = $"/EmployeeResult/EmployeeResultCreate?annualPerformanceId={model.AnnualPerformanceId}&empId={model.EmployeeId}&annualPerformanceDetailId={dbModel.AnnualPerformanceDetailId}";
                            }



                            if (model.EmployeeEqucationList != null && model.EmployeeEqucationList.Count > 0)
                            {
                                foreach (var item in model.EmployeeEqucationList)
                                {
                                    Education education = new Education
                                    {
                                        ExaminationId = item.ExaminationDropDownTypeId,
                                        SubjectId = item.SubjectDropDownTypeId,
                                        InstituteId = item.InstituteDropDownTypeId,
                                        PassingYear = item.PassedYear,
                                        Result = item.Result,
                                        CreatedBy = CreateBy,
                                        CreatedDate = DateTime.Now,
                                        IsActive = true,
                                        Id = model.EmployeeId
                                    };

                                    _db.Educations.Add(education);
                                }

                                await _db.SaveChangesAsync(cancellationToken);
                            }

                            transaction.Commit();
                            rResult.result = 1;
                            rResult.message = "Data Save Successfully!";

                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            rResult.result = 0;
                            rResult.message = "Data can't Save!";
                        }
                    }
                }
                
            }
            else
            {
                rResult.result = 0;
                rResult.message = "This Page is not Close,Session Out Please Login Then Resubmit Your APR";
                rResult.longId = 8;
                rResult.returnURL = "/User/Login";
            }

            return rResult;
        }

        #region AnnualPerformanceSubmit

        public async Task<AnnualPerformanceEmpResultVM> AnnualPerformanceSubmitAsync(EmployeeResultsVM model, CancellationToken cancellationToken = default)
        {
            string createBy = Common.GetUserId();
            var AprEndDateChk = await _db.AnnualPerformances.FirstAsync(x => x.AnnualPerformanceId == model.AnnualPerformanceId);
            if (model.AnnualPerformanceId <= 0 || model.EmployeeId <= 0 || string.IsNullOrEmpty(createBy) || (DateTime.Now.Date> AprEndDateChk.AprEndDate))
            {

                return new AnnualPerformanceEmpResultVM
                {
                    AnnualPerformanceId = model.AnnualPerformanceId,
                    EmployeeId = model.EmployeeId,
                    AnnualPerformanceDetailId = model.AnnualPerformanceDetailId
                };
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var dbEntityData = await GetActiveAnnualPerformanceDetailsAsync(model.AnnualPerformanceDetailId, cancellationToken);
                    if (dbEntityData == null)
                    {
                        return new AnnualPerformanceEmpResultVM();
                    }

                    await UpdateAnnualPerformanceDetailsAsync(dbEntityData, createBy, cancellationToken);
                    var signatories = await GetActiveSignatoriesAsync(model.EmployeeId, cancellationToken);
                    if (signatories == null || signatories.Count == 0)
                    {
                        return new AnnualPerformanceEmpResultVM();
                    }

                    await AddSignatoryApprovalMapsAsync(signatories, model.AnnualPerformanceDetailId, createBy, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    transaction.Commit();

                    return new AnnualPerformanceEmpResultVM
                    {
                        AnnualPerformanceId = model.AnnualPerformanceId,
                        EmployeeId = model.EmployeeId,
                        AnnualPerformanceDetailId = model.AnnualPerformanceDetailId

                    };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task<AnnualPerformanceDetail> GetActiveAnnualPerformanceDetailsAsync(long? annualPerformanceDetailsId, CancellationToken cancellationToken)
        {
            return await _db.AnnualPerformanceDetails
                .FirstOrDefaultAsync(x => x.IsActive && x.AnnualPerformanceDetailId == annualPerformanceDetailsId, cancellationToken);
        }

        private async Task UpdateAnnualPerformanceDetailsAsync(AnnualPerformanceDetail dbEntityData, string modifiedBy, CancellationToken cancellationToken)
        {
            dbEntityData.IsSubmitted = true;
            dbEntityData.ModifiedBy = modifiedBy;
            _db.Entry(dbEntityData).State = EntityState.Modified;
            await _db.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<RequisitionSignatory>> GetActiveSignatoriesAsync(long? employeeId, CancellationToken cancellationToken)
        {
            return await _db.RequisitionSignatories
                .Where(x => x.IsActive && x.EmployeeId == employeeId)
                .OrderBy(x => x.OrderBy)
                .ToListAsync(cancellationToken);
        }

        private async Task AddSignatoryApprovalMapsAsync(List<RequisitionSignatory> signatories, long? AnnualPerformanceDetailId, string createdBy, CancellationToken cancellationToken)
        {
            foreach (var signatory in signatories)
            {
                var signatoryApprovalMap = new SignatoryApprovalMap
                {
                    EmployeeId = signatory.SignatoryEmpId,
                    IntregratedFromId = AnnualPerformanceDetailId ?? 0,
                    TableName = signatory.IntegrateWith,
                    OrderBy = signatory.OrderBy,
                    IsActive = true,
                    Status = signatory.OrderBy > 1 ? -1 : 0,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                _db.SignatoryApprovalMaps.Add(signatoryApprovalMap);
            }
            await _db.SaveChangesAsync(cancellationToken);
        }

        #endregion

        public async Task<EmployeeResultsVM> GetAnnualPerformanceAllInfo(long SigId, DateTime? fromDate, DateTime? toDate)
        {
            EmployeeResultsVM model = new EmployeeResultsVM();
            
            model.CustomRecommendationList = await (from dt in _db.DropDownTypes
                                                    join di in _db.DropDownItems on dt.DropDownTypeId equals di.DropDownTypeId
                                                    where new[] { 73, 74 }.Contains(dt.DropDownTypeId)
                                                    && dt.IsActive
                                                    && di.IsActive
                                                    select new CustomRecommendationList
                                                    {
                                                        Text = di.Name,
                                                        Value = di.DropDownItemId.ToString(),
                                                        DropDownTypeId = di.DropDownTypeId
                                                    }).OrderBy(x => x.DropDownTypeId).ToListAsync();


            //model.employeeResultsList = await (from t1 in _db.SignatoryApprovalMaps
            //                                   join t2 in _db.Employees on t1.EmployeeId equals t2.Id
            //                                   join t3 in _db.AnnualPerformanceDetails on t1.IntregratedFromId equals t3.AnnualPerformanceDetailId
            //                                   join t4 in _db.AnnualPerformances on t3.AnnualPerformanceId equals t4.AnnualPerformanceId
            //                                   join t5 in _db.Employees on t3.EmployeeId equals t5.Id
            //                                   where t1.EmployeeId == SigId && t1.IsActive
            //                                        && t3.IsSubmitted==true
            //                                       && t1.TableName == "AnnualPerformanceDetail"
            //                                       && (fromDate == null || t4.AssessmentFrom >= fromDate)
            //                                       && (toDate == null || t4.AssessmentTo <= toDate)
            //                                   group new { t1, t3, t4, t5 } by t3.AnnualPerformanceDetailId into g
            //                                   select new EmployeeResultsVM
            //                                   {
            //                                       SigAppMapID = g.Key,
            //                                       EmpRsultStatus = g.Select(x => x.t3.Status).FirstOrDefault(),
            //                                       FullName = g.Select(x => x.t5.Name).FirstOrDefault(),
            //                                       AnnualPerformanceDetailId = g.Select(x => x.t3.AnnualPerformanceDetailId).FirstOrDefault(),
            //                                       EmployeeCode = g.Select(x => x.t5.EmployeeId).FirstOrDefault(),
            //                                       EmployeeId = g.Select(x => x.t3.EmployeeId).FirstOrDefault(),
            //                                       AnnualPerformanceId = g.Select(x => x.t4.AnnualPerformanceId).FirstOrDefault(),
            //                                       APREmpCreateDate = g.Select(x => x.t3.CreatedDate).FirstOrDefault(),
            //                                       AssessmentFrom = g.Select(x => x.t4.AssessmentFrom).FirstOrDefault(),
            //                                       AssessmentTo = g.Select(x => x.t4.AssessmentTo).FirstOrDefault(),
            //                                       EmployeeServiceType = g.Select(x => x.t5.ServiceTypeId).FirstOrDefault(),
            //                                       Status = g.Where(x => x.t1.IntregratedFromId == x.t3.AnnualPerformanceDetailId && x.t1.TableName == "AnnualPerformanceDetail" && x.t1.EmployeeId == SigId)
            //                                                .Select(x => x.t1.Status)
            //                                                .FirstOrDefault(),
            //                                       OrderBy = g.Where(x => x.t1.IntregratedFromId == x.t3.AnnualPerformanceDetailId && x.t1.TableName == "AnnualPerformanceDetail" && x.t1.EmployeeId == SigId)
            //                                                  .Select(x => x.t1.OrderBy)
            //                                                  .FirstOrDefault()
            //                                   }).OrderByDescending(x => x.AnnualPerformanceDetailId).ToListAsync();

            model.employeeResultsList = await (from t1 in _db.SignatoryApprovalMaps
                                               join t2 in _db.Employees on t1.EmployeeId equals t2.Id
                                               join t3 in _db.AnnualPerformanceDetails on t1.IntregratedFromId equals t3.AnnualPerformanceDetailId
                                               join t4 in _db.AnnualPerformances on t3.AnnualPerformanceId equals t4.AnnualPerformanceId
                                               join t5 in _db.Employees on t3.EmployeeId equals t5.Id
                                               join t6 in _db.Designations on t5.DesignationId equals t6.DesignationId into DesignationLeft
                                               from t7  in DesignationLeft.DefaultIfEmpty()
                                               where t1.EmployeeId == SigId && t1.IsActive
                                                   && t1.TableName == "AnnualPerformanceDetail"
                                                   && (fromDate == null || t4.AssessmentFrom >= fromDate)
                                                   && (toDate == null || t4.AssessmentTo <= toDate)
                                               select new EmployeeResultsVM
                                               {
                                                   SigAppMapID = t1.SignatoryApprovalMapId, 
                                                   EmpRsultStatus = t3.Status,
                                                   FullName = t5.Name + (t7 != null ? " (" + t7.Name + ")" : ""),
                                                   AnnualPerformanceDetailId = t3.AnnualPerformanceDetailId,
                                                   EmployeeCode = t5.EmployeeId,
                                                   EmployeeId = t3.EmployeeId,
                                                   AnnualPerformanceId = t4.AnnualPerformanceId,
                                                   APREmpCreateDate = t3.CreatedDate,
                                                   AssessmentFrom = t4.AssessmentFrom,
                                                   AssessmentTo = t4.AssessmentTo,
                                                   EmployeeServiceType = t5.ServiceTypeId,
                                                   Status = t1.Status,
                                                   OrderBy = t1.OrderBy,
                                                   IsSubmited=t3.IsSubmitted
                                               }).OrderByDescending(x => x.AnnualPerformanceDetailId).ToListAsync();



            model.ProfileQualitiesList = await (from dt in _db.DropDownTypes
                                                join di in _db.DropDownItems on dt.DropDownTypeId equals di.DropDownTypeId
                                                where new[] { 71, 72 }.Contains(dt.DropDownTypeId)
                                                && dt.IsActive
                                                && di.IsActive
                                                select new ProfileQualitiesList
                                                {
                                                    DropDownTypeId = dt.DropDownTypeId,
                                                    DropDownItemId = di.DropDownItemId,
                                                    ProfileQualitiesGroupName = dt.Name,
                                                    ProfileQualitiesName = di.Name
                                                }).ToListAsync();


            return model;
        }


        public async Task<long> SignatoryApprovalSubmission(EmployeeResultsVM dataModel, CancellationToken cancellationToken = default)
        {
            long result = 0;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    string createdBy = Common.GetUserId();

                    // Check for valid createdBy and valid AnnualPerformanceDetailId
                    if (string.IsNullOrEmpty(createdBy) || dataModel.AnnualPerformanceDetailId <= 0)
                        return result;

                    // Fetch the AnnualPerformanceDetail entity
                    var dbEntityDataAnnualPerformanceDetail = await _db.AnnualPerformanceDetails
                        .FirstOrDefaultAsync(x => x.AnnualPerformanceDetailId == dataModel.AnnualPerformanceDetailId, cancellationToken);

                    if (dbEntityDataAnnualPerformanceDetail == null)
                        return result;

                    // Fetch all relevant SignatoryApprovalMaps
                    var dbEntityDataList = await _db.SignatoryApprovalMaps
                        .Where(x => x.IsActive && x.IntregratedFromId == dataModel.AnnualPerformanceDetailId && x.TableName == "AnnualPerformanceDetail")
                        .OrderBy(x => x.OrderBy)
                        .ToListAsync(cancellationToken);

                    // Find the specific SignatoryApprovalMap to update
                    var dbEntityData = dbEntityDataList.FirstOrDefault(x => x.SignatoryApprovalMapId == dataModel.SigAppMapID);
                    if (dbEntityData == null)
                        return result;

                    // Determine the final approval step
                    int finalApproval = dbEntityDataList.Max(x => x.OrderBy);

                    // Update AnnualPerformanceDetail entity
                    //if (dataModel.OrderBy == dbEntityDataList[1].OrderBy)
                    //{
                    //    dbEntityDataAnnualPerformanceDetail.Recommendation = dataModel.Recommendation;
                    //    dbEntityDataAnnualPerformanceDetail.IncreaseProMonth = dataModel.IncreaseProMonth;
                    //    dbEntityDataAnnualPerformanceDetail.ModifiedBy = createdBy;
                    //    dbEntityDataAnnualPerformanceDetail.ModifiedDate = DateTime.Now;
                    //}
                    if (dataModel.OrderBy == dbEntityDataList[1].OrderBy)
                    {
                        dbEntityDataAnnualPerformanceDetail.FinalMarking = dataModel.FinalMarkings;
                        dbEntityDataAnnualPerformanceDetail.FinalRecommendation = dataModel.AssessmentOfTheApplicant;
                        dbEntityDataAnnualPerformanceDetail.ModifiedBy = createdBy;
                        dbEntityDataAnnualPerformanceDetail.ModifiedDate = DateTime.Now;
                    }
                    //if (dataModel.OrderBy == dbEntityDataList[3].OrderBy)
                    //{
                    //    //dbEntityDataAnnualPerformanceDetail.FinalMarking = dataModel.FinalMarkings;
                    //    dbEntityDataAnnualPerformanceDetail.FinalRecommendation = dataModel.AssessmentOfTheApplicant;
                    //    dbEntityDataAnnualPerformanceDetail.ModifiedBy = createdBy;
                    //    dbEntityDataAnnualPerformanceDetail.ModifiedDate = DateTime.Now;
                    //}



                    // Set status to 1 if it's the final approval step
                    if (finalApproval == dataModel.OrderBy)
                        dbEntityDataAnnualPerformanceDetail.Status = 1;

                    _db.Entry(dbEntityDataAnnualPerformanceDetail).State = EntityState.Modified;

                    // Save changes to AnnualPerformanceDetail
                    if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                    {
                        transaction.Rollback();
                        return result;
                    }

                    // Update the current SignatoryApprovalMap
                    dbEntityData.Comment = dataModel.FinalCommentOfManagement;
                    dbEntityData.Status = 1;
                    dbEntityData.ModifiedBy = createdBy;
                    dbEntityData.ModifiedDate = DateTime.Now;
                    _db.Entry(dbEntityData).State = EntityState.Modified;

                    // Save changes to current SignatoryApprovalMap
                    if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                    {
                        transaction.Rollback();
                        return result;
                    }

                    // Find next approval step and update its status
                    int? nextNumber = dbEntityDataList
                        .OrderBy(n => n.OrderBy)
                        .SkipWhile(n => n.OrderBy <= dbEntityData.OrderBy)
                        .Select(n => (int?)n.OrderBy)
                        .FirstOrDefault();

                    if (nextNumber > 0 && finalApproval >= dbEntityData.OrderBy)
                    {
                        var nextStepOfSignatoryApproval = dbEntityDataList.FirstOrDefault(x => x.OrderBy == nextNumber);
                        if (nextStepOfSignatoryApproval != null)
                        {
                            nextStepOfSignatoryApproval.Status = 0;
                            nextStepOfSignatoryApproval.ModifiedBy = createdBy;
                            //nextStepOfSignatoryApproval.ModifiedDate = DateTime.Now;
                            _db.Entry(nextStepOfSignatoryApproval).State = EntityState.Modified;

                            // Save changes to the next approval step
                            if (await _db.SaveChangesAsync(cancellationToken) > 0)
                            {
                                transaction.Commit();
                                return result = 1;
                            }
                        }
                    }

                    // Commit transaction if it's the final approval step
                    if (finalApproval == dbEntityData.OrderBy)
                    {
                        transaction.Commit();
                        return result = 1;
                    }

                    transaction.Rollback();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<List<SignatoryApprovalMapVM>> GetSignatoryApprovalList(long AnnualPerformanceDetailId, CancellationToken cancellationToken = default)
        {
            try
            {
                var queryResult = from sm in _db.SignatoryApprovalMaps
                                  join e in _db.Employees on sm.EmployeeId equals e.Id
                                  join d in _db.Designations on e.DesignationId equals d.DesignationId
                                  where sm.IntregratedFromId == AnnualPerformanceDetailId && sm.TableName == "AnnualPerformanceDetail"
                                  select new SignatoryApprovalMapVM
                                  {
                                      SignatoryApprovalName = e.Name,
                                      EmployeeCode = e.EmployeeId,
                                      EmpDigName = d.Name,
                                      Status = sm.Status,
                                      OrderBy = sm.OrderBy,
                                      CreatedDate = sm.ModifiedDate
                                  };

                return await queryResult.OrderBy(x => x.OrderBy).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeResultsVM> GetEmployeeResultAllInfo(long AnnualPerformanceId, long EmpId, long annualPerformanceDetailId, CancellationToken cancellationToken = default)
        {
            try
            {
                EmployeeResultsVM models = new EmployeeResultsVM();
                if (annualPerformanceDetailId > 0 && EmpId > 0 && annualPerformanceDetailId > 0)
                {
                    models = await (from t1 in _db.Employees
                                    join t2 in _db.Companies on t1.CompanyId equals t2.CompanyId
                                    join t3 in _db.Grades on t1.GradeId equals t3.GradeId
                                    join t4 in _db.Departments on t1.DepartmentId equals t4.DepartmentId
                                    join t5 in _db.Designations on t1.DesignationId equals t5.DesignationId
                                    join t6 in _db.Employees on t1.ManagerId equals t6.Id
                                    join t7 in _db.DropDownItems on t1.MaritalTypeId equals t7.DropDownItemId into t7Group
                                    from t7 in t7Group.DefaultIfEmpty()
                                    join t11 in _db.AnnualPerformances on AnnualPerformanceId equals t11.AnnualPerformanceId into t11Group
                                    from t11 in t11Group.DefaultIfEmpty()
                                    join t9 in _db.AnnualPerformanceDetails on new { AnnualPerformanceId = t11.AnnualPerformanceId, EmployeeId = EmpId }
                                    equals new { AnnualPerformanceId = t9.AnnualPerformanceId, EmployeeId = t9.EmployeeId } into t9Group
                                    from t10 in t9Group.DefaultIfEmpty()
                                    where t1.Active && t1.Id == EmpId
                                    select new EmployeeResultsVM
                                    {
                                        EmployeeId = t1.Id,
                                        EmployeeServiceType = t1.ServiceTypeId,
                                        EmployeeCode = t1.EmployeeId,
                                        FullName = t1.Name,
                                        CompanyName = t2.Name,
                                        Department = t4.Name == null ? "N/A" : t4.Name,
                                        Designation = t5.Name,
                                        ManagerName = t6.Name,
                                        AppointmentDate = t1.JoiningDate,
                                        DateOfBirth = t1.DateOfBirth,
                                        GradeName = t3.Name,
                                        MaritalStatus = t7.Name,
                                        CompanyId = t10.CompanyId ?? 0,
                                        DepartmentId = t10.DepartmentId ?? 0,
                                        PerformanceInReportingYear = t10.PerformanceInReportingYear,
                                        AdditionalPerformance = t10.AdditionalPerformance,
                                        PlanforNextYear = t10.PlanforNextYear,
                                        AssessmentFrom = t11.AssessmentFrom == null ? DateTime.Now : t11.AssessmentFrom,
                                        AssessmentTo = t11.AssessmentTo == null ? DateTime.Now : t11.AssessmentTo,
                                        IsSubmited = t10.IsSubmitted,
                                        SalesTarget = t10.SalesTarget,
                                        SalesAchievement = t10.SalesAchievement,
                                        RecoveryTarget = t10.RecoveryTarget,
                                        RecoveryAchievement = t10.RecoveryAchievement,
                                        FinalMarkings = t10.FinalMarking==null?0: t10.FinalMarking,
                                        AssessmentOfTheApplicant = t10.FinalRecommendation,
                                        performingPlanForNextYear=t10.PerformingPlanForNextYear,
                                        MajorWorkOne=t10.MajorWorkOne,
                                        MajorWorkTwo=t10.MajorWorkTwo,
                                        MajorWorkThree=t10.MajorWorkThree,
                                        MajorWorkFour=t10.MajorWorkFour,
                                        WorkWithReportingOfficer=t10.WorkWithReportingOfficer,
                                        ContractNumber=t1.MobileNo,
                                        APREmpCreateDate=t10.CreatedDate==null?DateTime.Now: t10.CreatedDate
                                    }).FirstOrDefaultAsync(cancellationToken);

                    models.DDLRecommendationModel = await (from dt in _db.DropDownTypes
                                                           join di in _db.DropDownItems on dt.DropDownTypeId equals di.DropDownTypeId
                                                           join apd in _db.AnnualPerformanceDetails.Where(x => x.AnnualPerformanceDetailId == annualPerformanceDetailId) on di.DropDownItemId equals apd.Recommendation into apdGroup
                                                           from apd in apdGroup.DefaultIfEmpty()
                                                           where new[] { 73, 74 }.Contains(dt.DropDownTypeId)
                                                           && dt.IsActive
                                                           && di.IsActive
                                                           select new Recommendation
                                                           {
                                                               RecommendationName = di.Name,
                                                               DropDownItemId = di.DropDownItemId,
                                                               DropDownItemIdEmpApr = apd.Recommendation,
                                                               IncreaseProMonth = apd.IncreaseProMonth,
                                                               DropDownTypeId = di.DropDownTypeId
                                                           }).OrderBy(x => x.DropDownTypeId).ToListAsync();

                    if (models.IsSubmited)
                    {
                        models.signatoryApprovalList = await (from sm in _db.SignatoryApprovalMaps
                                                              join e in _db.Employees on sm.EmployeeId equals e.Id
                                                              join d in _db.Designations on e.DesignationId equals d.DesignationId
                                                              join dp in _db.Departments on e.DepartmentId equals dp.DepartmentId
                                                              join c in _db.Companies on e.CompanyId equals c.CompanyId
                                                              where sm.IntregratedFromId == annualPerformanceDetailId
                                                                && sm.Status==1
                                                                 && sm.TableName == "AnnualPerformanceDetail"
                                                              select new SignatoryApprovalVM
                                                              {
                                                                  SignatoryName = e.Name,
                                                                  DesignationName = d.Name,
                                                                  SignatoryCompany=c.Name,
                                                                  SignatoryDepartment = dp.Name,
                                                                  Status = sm.Status,
                                                                  OrderBy = sm.OrderBy,
                                                                  Comment = sm.Comment,
                                                                  ApproveDate = sm.ModifiedDate
                                                              }).OrderBy(x => x.OrderBy).ToListAsync(cancellationToken);

                    }

                    models.EmployeeEqucationList = await (from t1 in _db.Educations
                                                          join t2 in _db.DropDownItems on t1.ExaminationId equals t2.DropDownItemId into t2Group
                                                          from t2 in t2Group.DefaultIfEmpty()
                                                          join t3 in _db.DropDownItems on t1.InstituteId equals t3.DropDownItemId into t3Group
                                                          from t3 in t3Group.DefaultIfEmpty()
                                                          join t4 in _db.DropDownItems on t1.SubjectId equals t4.DropDownItemId into t4Group
                                                          from t4 in t4Group.DefaultIfEmpty()
                                                          where t1.IsActive && t1.Id == EmpId
                                                          select new EmployeeEqucationList
                                                          {
                                                              Subject = t4.Name,
                                                              EducationId = t1.EducationId,
                                                              Qualification = t2 != null ? t2.Name : null,
                                                              Result = t1.Result,
                                                              PassedYear = t1.PassingYear,
                                                              Institute = t3 != null ? t3.Name : null,
                                                              ExaminationDropDownTypeId = t2 != null ? t2.DropDownTypeId.Value : 0,
                                                              SubjectDropDownTypeId = t4 != null ? t4.DropDownTypeId.Value : 0,
                                                              InstituteDropDownTypeId = t3 != null ? t3.DropDownTypeId.Value : 0,
                                                          }).OrderByDescending(x=>x.PassedYear).ToListAsync(cancellationToken);

                    models.ProfileQualitiesList = await (from dt in _db.DropDownTypes
                                                         join di in _db.DropDownItems on dt.DropDownTypeId equals di.DropDownTypeId
                                                         join aprpq in _db.APRProfileQualities.Where(x => x.AnnualPerformanceDetailId == annualPerformanceDetailId) on di.DropDownItemId equals aprpq.DropDownItemId into aprpqGroup
                                                         from aprpq in aprpqGroup.DefaultIfEmpty()
                                                         where new[] { 71, 72 }.Contains(dt.DropDownTypeId)
                                                         && dt.IsActive
                                                         && di.IsActive
                                                         select new ProfileQualitiesList
                                                         {
                                                             DropDownTypeId = dt.DropDownTypeId,
                                                             DropDownItemId = di.DropDownItemId,
                                                             ProfileQualitiesGroupName = dt.Name,
                                                             ProfileQualitiesName = di.Name,
                                                             DropDownItemIdAPR = aprpq.DropDownItemId,
                                                             ScoreApr = aprpq.Score
                                                         }).ToListAsync(cancellationToken);

                }
                return models;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<EmployeeResultsVM> GetAnnualPerformanceDetailsById(long annualPerformanceId = 0, CancellationToken cancellationToken = default)
        {
            EmployeeResultsVM model = new EmployeeResultsVM();

            model = await (from t1 in _db.AnnualPerformances
                           where t1.AnnualPerformanceId == annualPerformanceId
                           && t1.IsActive
                           select new EmployeeResultsVM
                           {
                               AnnualPerformanceId = t1.AnnualPerformanceId,
                               APRNO = t1.APRNO,
                               AssessmentFrom = t1.AssessmentFrom,
                               AssessmentTo = t1.AssessmentTo,
                               Remarks = t1.Remarks,
                               CreatedBy = t1.CreatedBy,
                               CreatedDate = t1.CreatedDate,
                               Status = t1.Status
                           }).FirstOrDefaultAsync();
            model.DataList = await Task.Run(() => (from t1 in _db.AnnualPerformanceDetails
                                                   join e in _db.Employees.Where(x => x.Active == true) on t1.EmployeeId equals e.Id
                                                   where t1.IsActive && t1.AnnualPerformanceId == annualPerformanceId
                                                   select new EmployeeResultsVM
                                                   {
                                                       AnnualPerformanceId = t1.AnnualPerformanceId,
                                                       AnnualPerformanceDetailId = t1.AnnualPerformanceDetailId,
                                                       EmployeeId = t1.EmployeeId,
                                                       EmployeeName = e.Name,
                                                       CreatedDate = t1.CreatedDate,
                                                       EmployeeCode = e.EmployeeId,
                                                       IsSubmited = t1.IsSubmitted,
                                                       Status = t1.Status
                                                   }).OrderByDescending(x => x.AnnualPerformanceId).AsEnumerable());





            return model;
        }

        //public EmployeeMarks GetEmployeeMarks(long employeeId, string employeeCode, DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        var empStatuses = _db.Database.SqlQuery<EmpAttendanceStatus>(
        //        "EXEC sp_CalculateEmployeeMarks @LevEmployeeId, @EmployeeId, @StartDate, @EndDate",
        //        new SqlParameter("LevEmployeeId", employeeId),
        //        new SqlParameter("EmployeeId", employeeCode),
        //        new SqlParameter("StartDate", startDate),
        //        new SqlParameter("EndDate", endDate)).ToList();

        //        var leaveCategories = _db.Database.SqlQuery<EmpLeaveCategory>(
        //        "EXEC sp_CalculateEmployeeMarks @LevEmployeeId, @EmployeeId, @StartDate, @EndDate",
        //        new SqlParameter("LevEmployeeId", employeeId),
        //        new SqlParameter("EmployeeId", employeeCode),
        //        new SqlParameter("StartDate", startDate),
        //        new SqlParameter("EndDate", endDate)).ToList();

        //        var employeeDisciplineStatuses = _db.Database.SqlQuery<EmpDisciplineStatus>(
        //        "EXEC sp_CalculateEmployeeMarks @LevEmployeeId, @EmployeeId, @StartDate, @EndDate",
        //        new SqlParameter("LevEmployeeId", employeeId),
        //        new SqlParameter("EmployeeId", employeeCode),
        //        new SqlParameter("StartDate", startDate),
        //        new SqlParameter("EndDate", endDate)).ToList();

        //        return new EmployeeMarks
        //        {
        //            EmpAttendanceStatus = empStatuses,
        //            EmpLeaveCategory = leaveCategories,
        //            EmpDisciplineStatus = employeeDisciplineStatuses
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public EmployeeMarks GetEmployeeMarks(long employeeId, string employeeCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                var empStatuses = new List<EmpAttendanceStatus>();
                var leaveCategories = new List<EmpLeaveCategory>();
                var employeeDisciplineStatuses = new List<EmpDisciplineStatus>();

                using (var command = _db.Database.Connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "sp_CalculateEmployeeMarks";
                    command.Parameters.Add(new SqlParameter("@LevEmployeeId", employeeId));
                    command.Parameters.Add(new SqlParameter("@EmployeeId", employeeCode));
                    command.Parameters.Add(new SqlParameter("@StartDate", startDate));
                    command.Parameters.Add(new SqlParameter("@EndDate", endDate));

                    _db.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Read EmpAttendanceStatus
                        while (reader.Read())
                        {
                            empStatuses.Add(new EmpAttendanceStatus
                            {
                                EmpStatus = reader.GetString(0),
                                StatusCount = reader.GetInt32(1),
                                MarkingEmpAttendance = reader.GetDecimal(2),
                                YourAttendanceMark = reader.GetDecimal(3)
                            });
                        }

                        // Move to the next result set
                        reader.NextResult();

                        // Read EmpLeaveCategory
                        while (reader.Read())
                        {
                            leaveCategories.Add(new EmpLeaveCategory
                            {
                                LeaveCategoryId = reader.GetInt32(0),
                                LeaveCategory = reader.GetString(1),
                                TotalLeaveDays = reader.GetDecimal(2),
                                GetLeavs = reader.GetDecimal(3),
                                Marking = reader.GetDecimal(4),
                                YourMark = reader.GetDecimal(5)
                            });
                        }

                        // Move to the next result set
                        reader.NextResult();

                        // Read EmpDisciplineStatus
                        while (reader.Read())
                        {
                            employeeDisciplineStatuses.Add(new EmpDisciplineStatus
                            {
                                LogTypeId = reader.GetInt32(0),
                                EmployeeDisciplineStatus = reader.GetString(1),
                                LogCount = reader.GetInt32(2),
                                Marking = reader.GetInt32(3),
                                YourMark = reader.GetDecimal(4)
                            });
                        }
                    }
                }


                decimal attendanceMark = (empStatuses.Sum(x => x.YourAttendanceMark) + leaveCategories.Sum(x => x.YourMark));
                decimal disciplineMarks = employeeDisciplineStatuses.Sum(x => x.YourMark);
                return new EmployeeMarks
                {
                    EmpAttendanceStatus = empStatuses,
                    EmpLeaveCategory = leaveCategories,
                    EmpDisciplineStatus = employeeDisciplineStatuses,
                    AttendanceMark = Math.Ceiling(attendanceMark),
                    DisciplineMarks = Math.Ceiling(disciplineMarks),
                    TotalEmployeePerformanceMark = Math.Ceiling(attendanceMark + disciplineMarks)


                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeInfo> GetEmployeeInfo(long EmpId, CancellationToken cancellationToken = default)
        {
            EmployeeInfo employeeInfo = new EmployeeInfo();
            var EmployeeInfo = await _db.Employees.FirstOrDefaultAsync(x => x.Id == EmpId);
            employeeInfo.EmployeeNameSrt = EmployeeInfo.Name;
            employeeInfo.EmployeeCodeSrt = EmployeeInfo.EmployeeId;
            return employeeInfo;
        }
    }
}
