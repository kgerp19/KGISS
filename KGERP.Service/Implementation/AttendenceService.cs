using Humanizer;
using KGERP.CustomModel;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.Leave.ViewModels;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.AttendanceModel;
using KGERP.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Humanizer.In;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace KGERP.Service.Implementation
{
    public class AttendenceService : IAttendenceService
    {
        private bool disposed = false;
        ERPEntities attendanceRepository = new ERPEntities();

        public List<AttendenceEntity> GetDailyAttendence(DateTime date)
        {
            dynamic result = attendanceRepository.Database.SqlQuery<AttendenceEntity>("exec sp_DailyAttendence {0} ", date).ToList();
            return result;
        }

        public async Task<AttendenceApproval> GetPersonalAttendenceStatus(long id)
        {
            AttendenceApproval model = new AttendenceApproval();
//            model.DataList = await Task.Run(()=> (from t1 in attendanceRepository.AttendenceApproveApplications
//                                                  join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.Id
//                                                  where t1.EmployeeId== id
//                                                  select new AttendenceApproval
//                                                  {
//                                                      Id= t1.Id,
//                                                      EmployeeId= t1.EmployeeId.Value,
//                                                      Name= t2.Name,
//                                                      ManagerId= t1.ManagerId.Value,
//                                                      UserId = t2.EmployeeId,
//                                                      Resion= t1.Resion,
//                                                      ApproveFor= t1.ApproveFor,
//                                                      AttendenceDate= t1.AttendenceDate,
//                                                      FromDateForOnField= t1.FromDateForOnField,
//                                                      ToDateForOnField= t1.ToDateForOnField,
//                                                      InTime= t1.InTime,
//                                                      ModifiedInTime= t1.ModifiedInTime,
//                                                      ModifiedOutTime= t1.ModifiedOutTime,
//                                                      HrNote= t1.HrNote,
//                                                      TourDays= (t1.FromDateForOnField.Value - t1.ToDateForOnField.Value) ?? 0 +1,
//                                                      ManagerNote= t1.ManagerNote,
                                                      


//case when ManagerStatus=0 then 'Pending' when ManagerStatus=1 then 'Approved' when ManagerStatus=2 then 'Denied' End as ManagerStatus,
//case when HrStatus=0 then 'Pending' when HrStatus=1 then 'Approved' when HrStatus=2 then 'Denied' End as HrStatus,
//Resion,
//                                                      ApproveFor,
//AttendenceDate,
//ApplicationDate,
//FromDateForOnField,
//ToDateForOnField,
//InTime,
//ModifiedInTime,
//ModifiedOutTime,
//HrNote,
//HrNote,
//DATEDIFF(day, IsNull(FromDateForOnField,0),IsNull(ToDateForOnField,0))+1 As TourDays,
//--IsNull(TourDays,0) As TourDays,
//ManagerNote
//                                                  }
//                                                  ))

            dynamic result = await Task.Run(()=> attendanceRepository.Database.SqlQuery<AttendenceApproval>("exec sp_GetPersonalAttendanceApplicationStatus {0} ", id).Where(x => x.ApproveFor == "Attendance Modify").AsEnumerable());

            model.DataList = result;
            return model;
        }
        public async Task<AttendenceApproval> GetPersonalAttendenceOnFieldTour(long id)
        {
            AttendenceApproval model = new AttendenceApproval();
            dynamic result = await Task.Run(() => attendanceRepository.Database.SqlQuery<AttendenceApproval>("exec sp_GetPersonalAttendanceApplicationStatus {0} ", id).Where(x => x.ApproveFor == "Tour" || x.ApproveFor == "On Field Duty").AsEnumerable());
            model.DataList = result;
            return model;
        }
        public AttendenceApproveApplicationModel GetAttendenceApprovalStatus(int id)
        {
            if (id == 0)
            {
                return new AttendenceApproveApplicationModel();
            }
            return ObjectConverter<AttendenceApproveApplication, AttendenceApproveApplicationModel>.Convert(attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id));
        }

        #region New On Field and Tour
        public bool SignatoryApprovalSave(long? empId, int applicationId)
        {
            try
            {
                //long? employeeId = model.EmployeeId;
                var requisitions = attendanceRepository.RequisitionSignatories.Where(r => r.EmployeeId == empId && r.IsActive == true && r.IntegrateWith == "LeaveApplication").ToList();
                int status = 0;
                foreach (var req in requisitions)
                {
                    if(req.OrderBy == 1)
                    {
                        status = 0;
                    }
                    else
                    {
                        status = -1;
                    }
                    var signatoryApprovalMap = new SignatoryApprovalMap
                    {
                        EmployeeId = req.SignatoryEmpId,
                        TableName = "AttendenceApproveApplication",
                        IntregratedFromId = applicationId,
                        OrderBy = req.OrderBy,
                        Status = status,
                        IsHRAdmin = req.IsHRAdmin,
                        CreatedDate = DateTime.Now,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        IsActive = true
                    };

                    attendanceRepository.SignatoryApprovalMaps.Add(signatoryApprovalMap);
                }
                attendanceRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<AttendenceApproval> GetOnFieldOrTourListByEmployee(long EmpID, DateTime? fromDate, DateTime? toDate)
        {
            AttendenceApproval model = new AttendenceApproval();

            var leaveApplicationIdsByEmp = attendanceRepository.AttendenceApproveApplications.Where(la => la.EmployeeId == EmpID).Join(attendanceRepository.SignatoryApprovalMaps, la => la.Id, sam => sam.IntregratedFromId, (la, sam) => la.Id).Distinct().ToList();
            var leaveViewModels = (from laId in leaveApplicationIdsByEmp
                                   join la in attendanceRepository.AttendenceApproveApplications on laId equals la.Id
                                   join e in attendanceRepository.Employees on la.EmployeeId equals e.Id
                                   join d in attendanceRepository.Designations on e.DesignationId equals d.DesignationId
                                   //join lc in attendanceRepository.LeaveCategories on la.LeaveCategoryId equals lc.LeaveCategoryId
                                   where (fromDate == null || la.FromDateForOnField >= fromDate)
                                   && (toDate == null || la.ToDateForOnField <= toDate)
                                   select new AttendenceApproval
                                   {
                                       //LeaveStatus = la.LeaveStatus,
                                       Id = la.Id,
                                       EmployeeId = e.Id,
                                       EmployeeKGId = e.EmployeeId,
                                       ApplicantName = e.Name,
                                       ApplicationType = la.ApproveFor,
                                       Designation = d.Name,
                                       ReasonAndLocation = la.Resion,
                                       TourDays = (int)la.TourDays,
                                       FromDateForOnField = la.FromDateForOnField,
                                       ToDateForOnField = la.ToDateForOnField,
                                       ApplicationDate = la.ApplicationDate,
                                       ComID = e.CompanyId ?? 0,
                                       LeaveStatus = ApplicationStatus(la.Id),
                                       TableName = "AttendenceApproveApplication"
                                   }).OrderByDescending(x => x.Id).ToList();

            model.DataList = leaveViewModels;

            return model;
        }
        public int ApplicationStatus(long LeaveApplicationId)
        {
            var status = -1;
            var sigs = attendanceRepository.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == LeaveApplicationId).Select(x => x.Status).ToList();
            int counter = 0;
            foreach (var sig in sigs)
            {
                if (sig == 1)
                {
                    counter++;
                }
                else if (sig == 2)
                {
                    status = 3;
                    return status;
                }
            }
            if (counter == sigs.Count)
            {
                status = 2;
                return status;
            }
            else
            {
                status = 1;
                return status;
            }
        }
        public async Task<LeaveAllDetailVMM> GetLeaveApprovalRelatedAllInfo(long SigId, DateTime? fromDate, DateTime? toDate)
        {
            LeaveAllDetailVMM model = new LeaveAllDetailVMM();
            model.LeaveApprovalDataList = await Task.Run(() => (from t1 in attendanceRepository.SignatoryApprovalMaps
                                                                join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.Id
                                                                join t3 in attendanceRepository.AttendenceApproveApplications on t1.IntregratedFromId equals t3.Id
                                                                join t4 in attendanceRepository.Employees on t3.EmployeeId equals t4.Id
                                                                where t1.EmployeeId == SigId && t1.IsActive
                                                                && t1.TableName == "AttendenceApproveApplication"
                                                                && (fromDate == null || t3.FromDateForOnField >= fromDate)
                                                                && (toDate == null || t3.ToDateForOnField <= toDate)
                                                                select new LeaveAllDetailVMM
                                                                {
                                                                    LeaveStatus = (t3.ManagerStatus == 1 && t3.HrStatus == 1) ? 2 : (t3.ManagerStatus == 1 && t3.HrStatus == 2) ? 3 : (t3.ManagerStatus == 2 && t3.HrStatus == 2) ? 3 : 1,
                                                                    LeaveApplicationID = t3.Id,
                                                                    EmployeeID = t4.Id,
                                                                    SigID = SigId,
                                                                    EmployeeKGid = t4.EmployeeId,
                                                                    ApplicantName = t4.Name,
                                                                    StartDate = t3.FromDateForOnField,
                                                                    EndDate = t3.ToDateForOnField,
                                                                    ApplyDate = t3.ApplicationDate,
                                                                    Reason = t3.Resion,
                                                                    LeaveDays = t3.TourDays,
                                                                    LeaveCategory = t3.ApproveFor,
                                                                    CompanyID = t4.CompanyId,
                                                                    TableName = t1.TableName,
                                                                    Status = attendanceRepository.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == t3.Id && x.EmployeeId == SigId).Select(x => x.Status).FirstOrDefault(),
                                                                    SignatoriesApprovalList = (from a1 in attendanceRepository.RequisitionSignatories.Where(z => z.EmployeeId == t3.EmployeeId)
                                                                                               join sa in attendanceRepository.SignatoryApprovalMaps on a1.SignatoryEmpId equals sa.EmployeeId
                                                                                               join e in attendanceRepository.Employees on a1.SignatoryEmpId equals e.Id
                                                                                               where sa.IntregratedFromId == t3.Id
                                                                                               select new SignatoryApprovalDetails
                                                                                               {
                                                                                                   SigAppMapID = sa.SignatoryApprovalMapId,
                                                                                                   SigIndID = a1.SignatoryEmpId,
                                                                                                   SigName = e.Name,
                                                                                                   SigDesignation = a1.DesignationName,
                                                                                                   ApprovalStatus = sa.Status,
                                                                                                   SigApprovalStatus = sa.Status == -1 ? "....." : sa.Status == 0 ? "Pending" : sa.Status == 1 ? "Approved" : sa.Status == 2 ? "Rejected" : "",
                                                                                                   OrderBy = sa.OrderBy
                                                                                               }).OrderByDescending(y => y.OrderBy).ToList()
                                                                }).OrderByDescending(x => x.LeaveApplicationID).ToListAsync());

            return model;
        }
        public async Task<SignatoryApprovalDetails> GetSignatoriesApprovalInfo(long applicationId, long sigId)
        {
            SignatoryApprovalDetails model = new SignatoryApprovalDetails();
            model.SignatoryDataList = await Task.Run(() => (from t1 in attendanceRepository.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == applicationId && x.TableName == "AttendenceApproveApplication")
                                                            join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.Id
                                                            join t3 in attendanceRepository.Designations on t2.DesignationId equals t3.DesignationId
                                                            select new SignatoryApprovalDetails
                                                            {
                                                                SigAppMapID = t1.SignatoryApprovalMapId,
                                                                SigName = t2.Name,
                                                                SigDesignation = t3.Name,
                                                                ApprovalStatus = t1.Status,
                                                                //SigApprovalStatus = t1.Status == -1 ? "....." : t1.Status == 0 ? "Pending" : t1.Status == 1 ? "Approved" : t1.Status == 2 ? "Rejected" : "",
                                                                SigApprovalStatus = t1.Status == -1 ? "....." :
                                                                                    t1.Status == 0 ? "Pending" :
                                                                                    t1.Status == 1 && t1.ModifiedBy == t2.EmployeeId ? "Approved" :
                                                                                    t1.Status == 1 && t1.ModifiedBy != t2.EmployeeId ? "-" :
                                                                                    t1.Status == 2 ? "Rejected" : "",
                                                                OrderBy = t1.OrderBy,
                                                                SigIndID = t1.EmployeeId,
                                                                UserId = sigId,
                                                                KgID = t2.EmployeeId,
                                                                ApproveDate = t1.ModifiedDate.Value
                                                            }).OrderByDescending(y => y.OrderBy).ToListAsync());
            return model;
        }
        public int DoLeaveApproval(LeaveAllDetailVM vm)
        {
            int result = -1;
            SignatoryApprovalMap dbModel = new SignatoryApprovalMap();
            dbModel = attendanceRepository.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID && x.EmployeeId == vm.SigID && x.TableName == "AttendenceApproveApplication").FirstOrDefault();

            var application = attendanceRepository.AttendenceApproveApplications.Find(dbModel.IntregratedFromId);
            bool isFinalApproved = false;

            if (dbModel != null)
            {
                var sameOrderSignatories = (from sam in attendanceRepository.SignatoryApprovalMaps
                                            join e in attendanceRepository.Employees on sam.EmployeeId equals e.Id
                                            where sam.IntregratedFromId == vm.LeaveApplicationID && sam.OrderBy == dbModel.OrderBy && sam.TableName == "AttendenceApproveApplication"
                                            select new
                                            {
                                                sam,
                                                EmpID = e.EmployeeId
                                            }).ToList();

                if (sameOrderSignatories != null)
                {
                    foreach (var sameOrder in sameOrderSignatories)
                    {
                        if (sameOrder.EmpID == Common.GetUserId() && sameOrder.sam.EmployeeId != Common.GetHRAdminId())
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                            sameOrder.sam.ModifiedDate = DateTime.Now;
                        }
                        else if (sameOrder.sam.EmployeeId == Common.GetHRAdminId())
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                            sameOrder.sam.ModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            sameOrder.sam.Status = vm.ApprovalStatus;
                            sameOrder.sam.ModifiedBy = Common.GetUserId();
                        }
                    }
                }
                var immediateHigherSig = attendanceRepository.SignatoryApprovalMaps.Where(x => x.IntregratedFromId == vm.LeaveApplicationID && x.OrderBy == (dbModel.OrderBy + 1) && x.TableName == "AttendenceApproveApplication").ToList();

                if (immediateHigherSig != null && vm.ApprovalStatus == (int)LeaveStatusNewEnum.Approved)
                {
                    foreach (var imSig in immediateHigherSig)
                    {
                        if (Common.GetIntUserId() == Common.GetHRAdminId() && imSig.EmployeeId == Common.GetHRAdminId())
                        {
                            imSig.Status = 1;
                        }
                        else
                        {
                            imSig.Status = 0;
                        }

                    }
                }
                if ((dbModel.IsHRAdmin || dbModel.EmployeeId == Common.GetHRAdminId()))
                {
                    if(vm.ApprovalStatus == (int)LeaveStatusNewEnum.Denied)
                    {
                        application.HrStatus = 2;
                        application.HRApprovalDate = DateTime.Now;
                    }
                    else
                    {
                        application.HrStatus = 1;
                        application.ManagerStatus = 1;
                        var inTime = application.ModifiedInTime;
                        var outTime = application.ModifiedOutTime;
                        var status = string.Empty;
                        if (application.ApproveFor == "On Field Duty")
                        {
                            status = "On Field";
                        }
                        else if (application.ApproveFor == "Tour")
                        {
                            status = "Tour";
                        }
                        else
                        {
                            status = "OK";
                        }
                        var empId = application.EmployeeId;
                        var date = application.AttendenceDate;
                        var fromDate = application.FromDateForOnField;
                        var toDate = application.ToDateForOnField;
                        var resultSP = attendanceRepository.Database.ExecuteSqlCommand("exec sp_HrApproveAction_1 {0},{1},{2},{3},{4},{5},{6},{7} ", inTime, outTime, status, empId, date, dbModel.IntregratedFromId, fromDate, toDate);
                    }
                }
                else
                {
                    if(vm.ApprovalStatus == (int)LeaveStatusNewEnum.Denied)
                    {
                        application.ManagerStatus = 2;
                        application.HrStatus = 2;
                        application.HRApprovalDate = DateTime.Now;
                    }
                }
                result = (attendanceRepository.SaveChanges() > 0) ? 1 : -1;
            }
            return result;
        }

        #endregion
        public bool SaveRequest(long id, AttendenceApproveApplicationModel model)
            {
            List<AttendenceApproveApplication> oldata;
            if (model.ApproveFor == "On Field Duty" || model.ApproveFor == "Tour")
            {
                oldata = attendanceRepository.AttendenceApproveApplications.Where(x => x.EmployeeId == model.EmployeeId && x.FromDateForOnField == model.FromDateForOnField && x.ToDateForOnField == model.ToDateForOnField && (x.ManagerStatus !=2 || x.HrStatus != 2)).ToList();
            }
            else
            {
                oldata = attendanceRepository.AttendenceApproveApplications.Where(x => x.EmployeeId == model.EmployeeId && x.AttendenceDate == model.AttendenceDate && x.ApplicationDate == model.ApplicationDate && (x.ManagerStatus != 2 || x.HrStatus != 2)).ToList();
            }

            if (oldata.Count == 0)
            {
                string body = string.Empty;
                string subject = string.Empty;
                bool isMailSentToEmployee = false;
                bool isMailSentToLineManager = false;
                if (model == null)
                {
                    throw new Exception("Request data missing!");
                }

                AttendenceApproveApplication attendenceApproveApplication = ObjectConverter<AttendenceApproveApplicationModel, AttendenceApproveApplication>.Convert(model);

                if (id > 0)
                {
                    attendenceApproveApplication = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
                    if (attendenceApproveApplication == null)
                    {
                        throw new Exception("Data not found!");
                    }
                    attendenceApproveApplication.EmployeeId = model.EmployeeId;
                    attendenceApproveApplication.ManagerId = model.ManagerId;
                    attendenceApproveApplication.ManagerStatus = 0;
                    attendenceApproveApplication.HrStatus = 0;
                    attendenceApproveApplication.Resion = model.Resion;
                    attendenceApproveApplication.ApproveFor = model.ApproveFor;
                    attendenceApproveApplication.AttendenceDate = model.AttendenceDate;
                    attendenceApproveApplication.InTime = model.InTime;
                    attendenceApproveApplication.OutTime = model.OutTime;
                    attendenceApproveApplication.ModifiedInTime = model.ModifiedInTime;
                    attendenceApproveApplication.ModifiedOutTime = model.ModifiedOutTime;
                    attendenceApproveApplication.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    attendenceApproveApplication.ModifiedDate = DateTime.Now;
                }
                else
                {
                    attendenceApproveApplication.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    attendenceApproveApplication.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    attendenceApproveApplication.CreatedDate = DateTime.Now;
                    attendenceApproveApplication.ModifiedDate = DateTime.Now;

                    attendanceRepository.AttendenceApproveApplications.Add(attendenceApproveApplication);
                }

                attendanceRepository.Entry(attendenceApproveApplication).State = attendenceApproveApplication.Id == 0 ? EntityState.Added : EntityState.Modified;
                var result = attendanceRepository.SaveChanges() > 0;

                Employee employee = attendanceRepository.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == attendenceApproveApplication.EmployeeId);
                if (attendenceApproveApplication.ApproveFor == "On Field Duty" || attendenceApproveApplication.ApproveFor == "Tour")
                {
                    body = EmailBody(attendenceApproveApplication, employee, "", ""); //Ashraf 15-10-2019
                }
                else
                {
                    body = EmailBody(attendenceApproveApplication, employee, "", ""); //Ashraf 15-10-2019
                }

                subject = "Application For - [" + attendenceApproveApplication.ApproveFor + "] " + employee.Name;
                if (result)
                {
                    SignatoryApprovalSave(model.EmployeeId, attendenceApproveApplication.Id);
                    //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //isMailSentToLineManager = MailService.SendMail(string.Empty, string.Empty, employee.Manager.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        public List<AttendenceApprovalAction> AttendenceApprovalAction(long managerId)
        {
            dynamic result = attendanceRepository.Database.SqlQuery<AttendenceApprovalAction>("exec sp_AttendenceRequestToManager {0} ", managerId).ToList();
            return result;
        }

        public List<AttendenceApprovalAction> HrAttendenceApprovalAction(long hrAdminId)
        {
            dynamic result = attendanceRepository.Database.SqlQuery<AttendenceApprovalAction>("exec sp_AttendenceRequestToHr {0} ", hrAdminId).ToList();
            return result;
        }

        public bool ApprovalAction(int id, string comments)
        {
            string body = string.Empty;
            string subject = string.Empty;
            //bool isMailSentToEmployee = false;
            //bool isMailSentToHr = false;


            AttendenceApproveApplication oldrqt = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
            Employee employee = attendanceRepository.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == oldrqt.EmployeeId);
            if (oldrqt == null)
            {
                throw new Exception("Employee not found!");
            }

            if (oldrqt.ApproveFor == "Attendance Modify")
            {
                if ((oldrqt.AttendenceDate > oldrqt.ApplicationDate) || (oldrqt.ApplicationDate - oldrqt.AttendenceDate).Value.TotalDays < 4)
                {

                    var inTime = oldrqt.ModifiedInTime;
                    var outTime = oldrqt.ModifiedOutTime;
                    var status = "OK";
                    var empId = oldrqt.EmployeeId;
                    var date = oldrqt.AttendenceDate;

                    var result = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForTimeApproval_1 {0},{1},{2},{3},{4},{5},{6} ", inTime, outTime, status, empId, date, comments, id);

                    if (result > 0)
                    {
                        var manStatus = "Approved";
                        var hrStatus = "Approved";
                        body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                        subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;

                        //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                        //isMailSentToHr = MailService.SendMail(string.Empty, string.Empty, employee.HrAdmin.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
                    }

                    return true;
                }
                else
                {

                    var manId = System.Web.HttpContext.Current.User.Identity.Name;
                    if (manId == "KG0115" || manId == "KG0002" || manId == "KG0001")
                    {
                        var inTime = oldrqt.ModifiedInTime;
                        var outTime = oldrqt.ModifiedOutTime;
                        var status = "OK";
                        var empId = oldrqt.EmployeeId;
                        var date = oldrqt.AttendenceDate;

                        var value = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForTimeApproval_1 {0},{1},{2},{3},{4},{5},{6} ", inTime, outTime, status, empId, date, comments, id);
                        if (value > 0)
                            return true;
                    }
                    else
                    {
                        oldrqt.ManagerStatus = 1;
                        oldrqt.ManagerApprovalDate = DateTime.Now;
                        oldrqt.HrStatus = 0;
                        return attendanceRepository.SaveChanges() > 0;
                    }



                    AttendenceApproveApplication newrqt = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
                    var manStatus = newrqt.ManagerStatus == 1 ? "Approved" : (newrqt.ManagerStatus == 2 ? "Denied" : "Pending");
                    var hrStatus = newrqt.HrStatus == 1 ? "Approved" : (newrqt.HrStatus == 2 ? "Denied" : "Pending");

                    // body = "Employee ID : " + employee.EmployeeId + "<br/>Name : " + employee.Name + "<br/> Application For: " + oldrqt.ApproveFor + "<br/>Applied Date : " + oldrqt.ApplicationDate.Value.ToString("dd MMM yyyy") + "<br/> Actual In Time :  " + oldrqt.InTime + " <br/> Actual Out Time  " + oldrqt.OutTime + "<br/> Modified In Time :  " + oldrqt.ModifiedInTime + " <br/> Modified Out Time  " + oldrqt.ModifiedOutTime + "<br/> Reason :  " + oldrqt.Resion + "<br /> Manager Status :" + manStatus + "<br /> HR Status :" + hrStatus;

                    body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                    subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;


                    return true;
                }
            }

            else if (oldrqt.ApproveFor == "Tour") // for Tour
            {
                if ((oldrqt.FromDateForOnField > oldrqt.ApplicationDate) || ((oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays < 4))
                {
                    var ddd = (oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays;
                    var status = "Tour";
                    var empId = oldrqt.EmployeeId;
                    var date = oldrqt.ApplicationDate;
                    var fromDate = oldrqt.FromDateForOnField;
                    var toDate = oldrqt.ToDateForOnField;
                    var result = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForTour {0},{1},{2},{3},{4},{5}", status, empId, date, id, fromDate, toDate);

                    //if (result > 0)
                    //{
                    //    var manStatus = "Approved";
                    //    var hrStatus = "Approved";
                    //    body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                    //    //subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
                    //    //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //}
                    return true;
                }
                else
                {

                    var manId = System.Web.HttpContext.Current.User.Identity.Name;
                    if (manId == "KG0115" || manId == "KG0002" || manId == "KG0001")
                    {
                        var ddd = (oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays;
                        var status = "Tour";
                        var empId = oldrqt.EmployeeId;
                        var date = oldrqt.ApplicationDate;
                        var fromDate = oldrqt.FromDateForOnField;
                        var toDate = oldrqt.ToDateForOnField;
                        var value = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForTour {0},{1},{2},{3},{4},{5}", status, empId, date, id, fromDate, toDate);
                        if (value > 0)
                            return true;
                    }
                    else
                    {
                        oldrqt.ManagerStatus = 1;
                        oldrqt.HrStatus = 0;
                        oldrqt.ManagerApprovalDate = DateTime.Now;
                        return attendanceRepository.SaveChanges() > 0;
                    }

                    //var manStatus = "Approved";
                    //var hrStatus = "Pending";
                    //body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                    //subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
                    //if (result)
                    //{
                    //    //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //    //isMailSentToHr = MailService.SendMail(string.Empty, string.Empty, employee.HrAdmin.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
                    //}
                    return true;

                }
            }
            else
            {
                if ((oldrqt.FromDateForOnField > oldrqt.ApplicationDate) || ((oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays < 4))
                {
                    var ddd = (oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays;
                    var status = "On Field";
                    var empId = oldrqt.EmployeeId;
                    var date = oldrqt.ApplicationDate;
                    var fromDate = oldrqt.FromDateForOnField;
                    var toDate = oldrqt.ToDateForOnField;
                    var result = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForOnField_1 {0},{1},{2},{3},{4},{5}", status, empId, date, id, fromDate, toDate);

                    //if (result > 0)
                    //{
                    //    var manStatus = "Approved";
                    //    var hrStatus = "Approved";
                    //   // body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                    //   // subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
                    //   // isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //}
                    return true;
                }

                else
                {

                    var manId = System.Web.HttpContext.Current.User.Identity.Name;
                    if (manId == "KG0115" || manId == "KG0002" || manId == "KG0001")
                    {
                        var ddd = (oldrqt.ApplicationDate - oldrqt.ToDateForOnField).Value.TotalDays;
                        var status = "On Field";
                        var empId = oldrqt.EmployeeId;
                        var date = oldrqt.ApplicationDate;
                        var fromDate = oldrqt.FromDateForOnField;
                        var toDate = oldrqt.ToDateForOnField;
                        var value = attendanceRepository.Database.ExecuteSqlCommand("exec sp_ManagerActionForOnField_1 {0},{1},{2},{3},{4},{5}", status, empId, date, id, fromDate, toDate);
                        if (value > 0)
                            return true;
                    }
                    else
                    {
                        oldrqt.ManagerStatus = 1;
                        oldrqt.HrStatus = 0;
                        oldrqt.ManagerApprovalDate = DateTime.Now;
                        return attendanceRepository.SaveChanges() > 0;
                    }

                    return true;
                    //var manStatus = "Approved";
                    //var hrStatus = "Pending";
                    //body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                    //subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
                    //if (result)
                    //{
                    //   // isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
                    //    //isMailSentToHr = MailService.SendMail(string.Empty, string.Empty, employee.HrAdmin.Email, employee.Manager.Name, string.Empty, string.Empty, subject, body, out sendStatus);
                    //}
                    //return true;
                }
            }

        }

        public bool HrApprovalAction(int id, string comments)
        {
            string body = string.Empty;
            string subject = string.Empty;
            bool isMailSentToEmployee = false;

            AttendenceApproveApplication oldrqt = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
            Employee employee = attendanceRepository.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == oldrqt.EmployeeId);
            if (oldrqt == null)
            {
                throw new Exception("Attedence Request not found!");
            }

            var inTime = oldrqt.ModifiedInTime;
            var outTime = oldrqt.ModifiedOutTime;
            var status = string.Empty; //oldrqt.ApproveFor == "On Field Duty" ? "On Field" : "OK";
            if (oldrqt.ApproveFor == "On Field Duty")
            {
                status = "On Field";
            }
            else if (oldrqt.ApproveFor == "Tour")
            {
                status = "Tour";
            }
            else
            {
                status = "OK";
            }
            var empId = oldrqt.EmployeeId;
            var date = oldrqt.AttendenceDate;
            var fromDate = oldrqt.FromDateForOnField;
            var toDate = oldrqt.ToDateForOnField;
            var result = attendanceRepository.Database.ExecuteSqlCommand("exec sp_HrApproveAction_1 {0},{1},{2},{3},{4},{5},{6},{7} ", inTime, outTime, status, empId, date, id, fromDate, toDate);

            if (result > 0)
            {
                var manStatus = "Approved";
                var hrStatus = "Approved";
                if (oldrqt.ApproveFor == "On Field Duty" || oldrqt.ApproveFor == "Tour")
                {
                    body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                }
                else
                {
                    body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
                }

                //subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
                //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
            }
            return true;
        }

        public bool DeniedAction(int id, string comments)
        {
            string body = string.Empty;
            string subject = string.Empty;
            bool isMailSentToEmployee = false;
            AttendenceApproveApplication oldrqt = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
            Employee employee = attendanceRepository.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == oldrqt.EmployeeId);

            if (oldrqt == null)
            {
                throw new Exception("Employee not found!");
            }

            oldrqt.ManagerStatus = 2;
            oldrqt.HrStatus = 2;
            oldrqt.ManagerNote = comments;

            oldrqt.HRApprovalDate = DateTime.Now;
            var result = attendanceRepository.SaveChanges() > 0;

            var manStatus = "Denied";
            var hrStatus = "Denied";

            if (oldrqt.ApproveFor == "On Field Duty" || oldrqt.ApproveFor == "Tour")
            {
                body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
            }
            else
            {
                body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
            }

            subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
            if (result)
            {
                // isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
            }

            return true;
        }

        public bool HrDeniedAction(int id, string comments)
        {
            string body = string.Empty;
            string subject = string.Empty;
            bool isMailSentToEmployee = false;

            AttendenceApproveApplication oldrqt = attendanceRepository.AttendenceApproveApplications.FirstOrDefault(x => x.Id == id);
            Employee employee = attendanceRepository.Employees.Include(x => x.Employee3).Include(x => x.Employee2).FirstOrDefault(x => x.Id == oldrqt.EmployeeId);

            if (oldrqt == null)
            {
                throw new Exception("Employee not found!");
            }
            oldrqt.HrStatus = 2;
            oldrqt.HRApprovalDate = DateTime.Now;
            var result = attendanceRepository.SaveChanges() > 0;

            var manStatus = "Approved";
            var hrStatus = "Denied";

            if (oldrqt.ApproveFor == "On Field Duty" || oldrqt.ApproveFor == "Tour")
            {
                body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
            }
            else
            {
                body = EmailBody(oldrqt, employee, manStatus, hrStatus);//Ashraf 15-10-2019
            }

            subject = "Application For - [" + oldrqt.ApproveFor + "] " + employee.Name;
            if (result)
            {
                //isMailSentToEmployee = MailService.SendMail(string.Empty, string.Empty, employee.Email, employee.Name, string.Empty, string.Empty, subject, body, out string sendStatus);
            }

            return true;
        }

        public List<AttendenceEntity> GetDailyAttendenceTeamWise(long managerId, DateTime date)
        {
            dynamic result = attendanceRepository.Database.SqlQuery<AttendenceEntity>("exec sp_NewTeamAttendance {0}, {1} ", managerId, date).ToList();
            return result;
        }

        public bool PrecessAttendenceInFinalTable(DateTime attendenceDate)
        {
            List<AttendenceSeviceModel> model = new List<AttendenceSeviceModel>();
            Attendence oldrqt = attendanceRepository.Attendences.FirstOrDefault(x => x.AttendenceDate == attendenceDate);
            if (oldrqt != null)
            {
                return false;
            }
            else
            {
                var insert = attendanceRepository.Database.ExecuteSqlCommandAsync("exec sp_FinalAttendence {0} ", attendenceDate);
                return true;
            }
        }

        public List<InTimeOutTime> GetTime(string empId, DateTime date)
        {
            List<InTimeOutTime> result = attendanceRepository.Database.SqlQuery<InTimeOutTime>("exec sp_InOutTime {0}, {1}", empId, date).ToList();
            return result;
        }

        public List<AttendenceEntity> GetEmployeeAttendence(string FromDate, string ToDate, string EmpId, int? DepartmentId)
        {
            var query = string.Format(@"select EmployeeId,Name,AttendenceDate,InTime,OutTime,TotalHour,ShiftName,EmpStatus from vw_Attendence where convert(nvarchar(8),AttendenceDate,112) between {0} and {1} ", FromDate, ToDate);
            if (EmpId != null)
            {
                query = string.Format(@"select EmployeeId,Name,AttendenceDate,InTime,OutTime,TotalHour,ShiftName,EmpStatus from vw_Attendence where EmployeeId='{0}' and (convert(nvarchar(8),AttendenceDate,112) between {1} and {2})", EmpId, FromDate, ToDate);
            }
            if (DepartmentId != null)
            {
                query = string.Format(@"select EmployeeId,Name,AttendenceDate,InTime,OutTime,TotalHour,ShiftName,EmpStatus from vw_Attendence where convert(nvarchar(8),AttendenceDate,112) between {0} and {1} and DepartmentId = {2}", FromDate, ToDate, DepartmentId);
            }
            if (DepartmentId != null && EmpId != null)
            {
                query = string.Format(@"select EmployeeId,Name,AttendenceDate,InTime,OutTime,TotalHour,ShiftName,EmpStatus from vw_Attendence where convert(nvarchar(8),AttendenceDate,112) between {0} and {1} and DepartmentId = {2} and EmployeeId='{3}'", FromDate, ToDate, DepartmentId, EmpId);
            }
            List<AttendenceEntity> result = attendanceRepository.Database.SqlQuery<AttendenceEntity>(query).ToList();
            return result;
        }

        public string GetEmpId(int? id)
        {
            var data = attendanceRepository.Employees.Where(x => x.Id == id).FirstOrDefault();
            var empId = data.EmployeeId;
            return empId;
        }

        public async Task<AttendanceVm> GetSelfAttendance(DateTime? fromDate, DateTime? toDate)
        {
            AttendanceVm model = new AttendanceVm();
            var empId = System.Web.HttpContext.Current.User.Identity.Name;
            var todate = DateTime.Now.Date;
            model.DataList = await Task.Run(() => (from t1 in attendanceRepository.ProcessAttenendances
                                                 .Where(q => q.EmployeeId == empId
                                                 && q.AttendenceDate== todate)
                                                   select new AttendanceVm
                                                   {
                                                       EmployeeId = t1.EmployeeId,
                                                       AttendanceDate = t1.AttendenceDate,
                                                       InTime = t1.InTime,
                                                       OutTime = t1.OutTime,
                                                       EmpStatus = t1.EmpStatus=="OK"?"Present": t1.EmpStatus
                                                   }).Union(
                (from t2 in attendanceRepository.Attendences.Where(q => q.EmployeeId == empId 
                 && q.AttendenceDate >= fromDate
                 && q.AttendenceDate <= toDate)
                 select new AttendanceVm
                 {
                     EmployeeId = t2.EmployeeId,
                     AttendanceDate = t2.AttendenceDate,
                     InTime = t2.InTime,
                     OutTime = t2.OutTime,
                     EmpStatus = t2.EmpStatus == "OK" ? "Present" : t2.EmpStatus
                 }
                )
            ).OrderByDescending(o=>o.AttendanceDate).ToListAsync());
            return model;
        }


        public async Task<AttendenceSummeries> MonthlyAttendanceSummery(DateTime? fromDate, DateTime? toDate)
        {
            var id = System.Web.HttpContext.Current.User.Identity.Name;
            AttendenceSummeries model = new AttendenceSummeries();
            
             model.DataList = attendanceRepository.Database.SqlQuery<AttendenceSummeries>
                ("exec sp_AttendenceSummery {0}, {1},{2} ", fromDate, toDate, id).ToList();
            model.EmployeeId = id;
            return model;
        }

        public async Task<AttendanceVm> GetDailyAttendanceTeamWise(long managerId, DateTime? fromDate, DateTime? toDate)
        {
            AttendanceVm model = new AttendanceVm();
           
            var dataList = await Task.Run(() => (from t1 in attendanceRepository.Attendences
                                                 join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.EmployeeId into t2_Join
                                                   from t2 in t2_Join.DefaultIfEmpty()
                                                   join t3 in attendanceRepository.Departments on t2.DepartmentId equals t3.DepartmentId into t3_Join
                                                   from t3 in t3_Join.DefaultIfEmpty()
                                                   join t4 in attendanceRepository.Designations on t2.DesignationId equals t4.DesignationId into t4_Join
                                                   from t4 in t4_Join.DefaultIfEmpty()
                                                   where t1.AttendenceDate >= fromDate
                                                 && t1.AttendenceDate <= toDate
                                                 select new AttendanceVm
                                                   {
                                                       EmployeeId = t1.EmployeeId,
                                                       ManagerId= t2.ManagerId,
                                                       EmployeeName= t2.Name,
                                                       DesignationName= t4.Name,
                                                       AttendanceDate = t1.AttendenceDate,
                                                       InTime = t1.InTime,
                                                       OutTime = t1.OutTime,
                                                     EmpStatus = t1.EmpStatus == "OK" ? "Present" : t1.EmpStatus
                                                 }).Union(
                (from t1 in attendanceRepository.ProcessAttenendances
                 join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.EmployeeId into t2_Join
                 from t2 in t2_Join.DefaultIfEmpty()
                 join t3 in attendanceRepository.Departments on t2.DepartmentId equals t3.DepartmentId into t3_Join
                 from t3 in t3_Join.DefaultIfEmpty()
                 join t4 in attendanceRepository.Designations on t2.DesignationId equals t4.DesignationId into t4_Join
                 from t4 in t4_Join.DefaultIfEmpty()
                 where t1.AttendenceDate.Value == toDate// DateTime.Now.Date
                 select new AttendanceVm
                 {
                     EmployeeId = t2.EmployeeId,
                     ManagerId= t2.ManagerId,
                     EmployeeName= t2.Name,
                     DesignationName = t4.Name,
                     AttendanceDate = t1.AttendenceDate,
                     InTime = t1.InTime,
                     OutTime = t1.OutTime,
                     EmpStatus = t1.EmpStatus == "OK" ? "Present" : t1.EmpStatus
                 }
                )
            ).OrderByDescending(o => o.AttendanceDate).AsEnumerable());

            if(managerId == 103 || managerId == 2 || managerId == 1)
            {
                model.DataList = dataList;
            }
            else
            {
                model.DataList = dataList.Where(q=>q.ManagerId== managerId);
            }

           
            return model;
        }
        public async Task<AttendanceVm> GetDailyAttendanceOfTeam(long managerId, DateTime? fromDate, DateTime? toDate)
        {
            AttendanceVm model = new AttendanceVm();

            //var employeeList = await attendanceRepository.RequisitionSignatories
            //    .Where(x => x.SignatoryEmpId == managerId && x.IntegrateWith == "LeaveApplication" && x.IsActive == true)
            //    .Select(y => y.EmployeeId)
            //    .ToListAsync();
            var employeeIdList = attendanceRepository.RequisitionSignatories.Where(rs => rs.SignatoryEmpId == managerId && rs.IntegrateWith == "LeaveApplication" && rs.IsActive == true).Join(attendanceRepository.Employees, rs => rs.EmployeeId, e => e.Id, (rs, e) => e.EmployeeId).ToList();
            //model.AttendenceList = await Task.Run(() => (from  a in attendanceRepository.ProcessAttenendances
            //                                             join e in attendanceRepository.Employees on a.EmployeeId equals e.EmployeeId
            //                                             join d in attendanceRepository.Departments on e.DepartmentId equals d.DepartmentId
            //                                             join des in attendanceRepository.Designations on e.DesignationId equals des.DesignationId
            //                                             where employeeIdList.Contains(a.EmployeeId) && (!fromDate.HasValue || a.AttendenceDate >= fromDate) && (!toDate.HasValue || a.AttendenceDate <= toDate)
            //                                             select new AttendanceVm
            //                                             {
            //                                                 EmployeeId = a.EmployeeId,
            //                                                 ManagerId = e.ManagerId,
            //                                                 EmployeeName = e.Name,
            //                                                 DesignationName = d.Name,
            //                                                 AttendanceDate = a.AttendenceDate,
            //                                                 InTime = a.InTime,
            //                                                 OutTime = a.OutTime,
            //                                                 EmpStatus = a.EmpStatus == "OK" ? "Present" : a.EmpStatus
            //                                             }).OrderByDescending(z => z.AttendanceDate).ToList());
            model.AttendenceList = await Task.Run(() => (from a in attendanceRepository.ProcessAttenendances
                                                         join e in attendanceRepository.Employees on a.EmployeeId equals e.EmployeeId
                                                         join d in attendanceRepository.Departments on e.DepartmentId equals d.DepartmentId
                                                         join des in attendanceRepository.Designations on e.DesignationId equals des.DesignationId
                                                         where employeeIdList.Contains(a.EmployeeId) && (!fromDate.HasValue || a.AttendenceDate >= fromDate) && (!toDate.HasValue || a.AttendenceDate <= toDate)
                                                         select new AttendanceVm
                                                         {
                                                             EmployeeId = a.EmployeeId,
                                                             ManagerId = e.ManagerId,
                                                             EmployeeName = e.Name,
                                                             DesignationName = d.Name,
                                                             AttendanceDate = a.AttendenceDate,
                                                             InTime = a.InTime,
                                                             OutTime = a.OutTime,
                                                             EmpStatus = a.EmpStatus == "OK" ? "Present" : a.EmpStatus
                                                         }).Union(
                (from t1 in attendanceRepository.Attendences
                 join t2 in attendanceRepository.Employees on t1.EmployeeId equals t2.EmployeeId into t2_Join
                 from t2 in t2_Join.DefaultIfEmpty()
                 join t3 in attendanceRepository.Departments on t2.DepartmentId equals t3.DepartmentId into t3_Join
                 from t3 in t3_Join.DefaultIfEmpty()
                 join t4 in attendanceRepository.Designations on t2.DesignationId equals t4.DesignationId into t4_Join
                 from t4 in t4_Join.DefaultIfEmpty()
                 where t1.AttendenceDate.Value == toDate// DateTime.Now.Date
                 select new AttendanceVm
                 {
                     EmployeeId = t2.EmployeeId,
                     ManagerId = t2.ManagerId,
                     EmployeeName = t2.Name,
                     DesignationName = t4.Name,
                     AttendanceDate = t1.AttendenceDate,
                     InTime = t1.InTime,
                     OutTime = t1.OutTime,
                     EmpStatus = t1.EmpStatus == "OK" ? "Present" : t1.EmpStatus
                 }
                )
            ).OrderByDescending(z => z.AttendanceDate).ToList());

            return model;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    attendanceRepository.Dispose();
                }
            }
            disposed = true;
        }

        public string EmailBody(AttendenceApproveApplication oldrqt, Employee employee, string managerStatus, string hrStatus)
        {
            string body = "";
            body = "<!DOCTYPE html>";
            body += "<html> <head> <style> ";
            body += "table { border: 0px solid #ddd;   width: 500px; } th, td { text - align: left; font - size:12; border: 0px solid #ddd;  padding: 0px;}";
            body += " tr: nth-child(even){ background-color: #f2f2f2}  th {background-color: #007f3d;text-align:right;  border: 1px solid #ddd; width: 200px;   color: white;} td {background-color: #C8E5EB;  border: 1px solid #ddd;  width: 300px;  color: black;} ";
            body += " h5 { color: red; } h4 { color: black; }</style></head><body>  ";
            body += "<H4>Dear Concern,</H4>";
            body += "Please" + "<a href=" + "http://192.168.0.7:90/user/login" + "> click here </a> for details and action of <b> " + oldrqt.ApproveFor + "</b></br>";

            body += "</br>";
            body += "<table>";
            body += "<tr>";
            body += "<th>" + "Employee ID :" + "</th>";
            body += "<td>" + employee.EmployeeId + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Name :" + "</th>";
            body += "<td>" + employee.Name + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Application For :" + "</th>";
            body += "<td>" + oldrqt.ApproveFor + "</td>";
            body += "</tr>";
            body += "<tr>";
            body += "<th>" + "Applied Date :" + "</th>";
            body += "<td>" + oldrqt.ApplicationDate + "</td>";
            body += "</tr>";
            if (oldrqt.ApproveFor == "On Field Duty" || oldrqt.ApproveFor == "Tour")
            {
                body += "<tr>";
                body += "<th>" + "On Field Date :" + "</th>";
                body += "<td>" + "From " + oldrqt.FromDateForOnField.Value.ToString("dd-MMM-yyyy") + " to " + oldrqt.ToDateForOnField.Value.ToString("dd-MMM-yyyy") + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "Location :  " + "</th>";
                body += "<td>" + oldrqt.Resion + " </td>";
                body += "</tr>";
            }
            else
            {
                body += "<tr>";
                body += "<th>" + "Actual In Time  :" + "</th>";
                body += "<td>" + oldrqt.InTime + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "Actual Out Time :  " + "</th>";
                body += "<td>" + oldrqt.OutTime + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "Modified In Time :  " + "</th>";
                body += "<td>" + oldrqt.ModifiedInTime + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "Modified Out Time :  " + "</th>";
                body += "<td>" + oldrqt.ModifiedOutTime + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "Reason :  " + "</th>";
                body += "<td>" + oldrqt.Resion + " </td>";
                body += "</tr>";
            }

            if (managerStatus == "")
            {
                body += "<tr>";
                body += "<th>" + "Manager Status :  " + "</th>";
                if (oldrqt.ManagerStatus == 0)
                {
                    managerStatus = "Pending";
                    body += "<td>" + managerStatus + " </td>";
                }
                else if (oldrqt.ManagerStatus == 1)
                {
                    managerStatus = "Approved";
                    body += "<td>" + managerStatus + " </td>";
                }
                else
                {
                    managerStatus = "Denied";
                    body += "<td>" + managerStatus + " </td>";
                }
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "HR Status :  " + "</th>";
                if (oldrqt.HrStatus == 0)
                {
                    managerStatus = "Pending";
                    body += "<td>" + managerStatus + " </td>";
                }
                else if (oldrqt.HrStatus == 1)
                {
                    managerStatus = "Approved";
                    body += "<td>" + managerStatus + " </td>";
                }
                else
                {
                    managerStatus = "Denied";
                    body += "<td>" + managerStatus + " </td>";
                }

                body += "</tr>";
            }
            else
            {
                body += "<tr>";
                body += "<th>" + "Manager Status :  " + "</th>";
                body += "<td>" + managerStatus + " </td>";
                body += "</tr>";

                body += "<tr>";
                body += "<th>" + "HR Status :  " + "</th>";
                body += "<td>" + hrStatus + " </td>";
                body += "</tr>";
            }

            body += "</table><br/><H5>[This is system generated emial notification.<b> HelpLine:Cell: 01700729805/8 PBX no.817<b/></H5></body></html>";

            return body;
        }
        public List<CalculativeAttendanceVM> GetCalculativeAttendance(int? companyId, int year, int month)
        {
            var _context = new ERPEntities();
            var firstDateOfMonth = new DateTime(year, month, 1);
            DateTime lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);
            var totalDaysInMonth = DateTime.DaysInMonth(year, month);
            bool isCurrentMonthJoin = false;
            bool isCurrentMonthResign = false;
            var employeeList = (from e in _context.Employees
                                join c in _context.Companies on e.CompanyId equals c.CompanyId
                                join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                where (companyId == null || c.CompanyId == companyId)
                                && e.JoiningDate.HasValue && e.JoiningDate <= lastDateOfMonth
                                && (e.Active || (!e.Active && e.EndDate.HasValue && e.EndDate >= firstDateOfMonth))
                                select new CalculativeAttendanceVM()
                                {
                                    Id = e.Id,
                                    EmployeeId = e.EmployeeId,
                                    EmployeeName = e.Name,
                                    JoiningDate = e.JoiningDate.Value,
                                    ResignDate = e.EndDate,
                                    IsActive = e.Active,
                                    CompanyId = c.CompanyId,
                                    CompanyName = c.Name,
                                    DepartmentId = d.DepartmentId,
                                    DepartmentName = d.Name,
                                    DesignationId = ds.DesignationId,
                                    DesignationName = ds.Name,

                                }).ToList();

            foreach (var emp in employeeList)
            {
                //employeeList = employeeList.Where(x => x.Id == 1304).ToList();
                int workingDay = totalDaysInMonth;
                var firstCheckDay = firstDateOfMonth;
                var lastCheckDay = lastDateOfMonth;
                if (emp.JoiningDate.Year == year && emp.JoiningDate.Month == month)
                {
                    isCurrentMonthJoin = true;
                    workingDay = (workingDay - emp.JoiningDate.Day)+1;
                    firstCheckDay = emp.JoiningDate;
                }
                if (!emp.IsActive && emp.ResignDate.HasValue && emp.ResignDate.Value.Month == month)
                {
                    isCurrentMonthResign = true;
                    workingDay -= (lastDateOfMonth.Day - emp.ResignDate.Value.Day);
                    lastCheckDay = emp.ResignDate.Value;
                }
                var attendanceList = _context.Attendences.Where(x=> x.EmployeeId == emp.EmployeeId && x.AttendenceDate >= firstCheckDay && x.AttendenceDate <= lastCheckDay).ToList();
                var onTime = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.Ok);
                var lateIn = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.LateIn);
                var earlyOut = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.EarlyOut);
                var lateInEarlyOut = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.LateInEarlyOut);
                var weekend = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.Weekend);
                var holiday = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.Holiday);
                var tour = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.Tour);
                var leave = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.OnLeave);
                var absent = attendanceList.Count(x => x.EmpStatus == AttendanceStatusModel.Absent);
                //var onFieldDuty = _context.AttendenceApproveApplications.Count(x => x.EmployeeId == emp.Id && x.HrStatus == 2 && x.AttendenceDate >= firstCheckDay && x.AttendenceDate <= lastCheckDay && x.ApproveFor == AttendanceStatusModel.OnFieldDutyRequest);
                //var attendanceModified = _context.AttendenceApproveApplications.Where(x => x.EmployeeId == emp.Id && x.HrStatus == 2 && x.AttendenceDate >= firstCheckDay && x.AttendenceDate <= lastCheckDay && x.ApproveFor == AttendanceStatusModel.AttendanceModifiedRequest).GroupBy(x=> x.ApplicationDate).Count();
                var attendanceModified = _context.AttendenceApproveApplications
                                                     .Where(x => x.EmployeeId == emp.Id &&
                                                                 x.HrStatus == 1 &&
                                                                 x.AttendenceDate >= firstCheckDay &&
                                                                 x.AttendenceDate <= lastCheckDay &&
                                                                 x.ApproveFor == AttendanceStatusModel.AttendanceModifiedRequest)
                                                     .GroupBy(x => x.ApplicationDate)
                                                     .Select(group => group.Key) // Select only the distinct ApplicationDates
                                                     .Count();
                var onFieldDuty = _context.AttendenceApproveApplications
                                                    .Where(x => x.EmployeeId == emp.Id &&
                                                                x.HrStatus == 1 &&
                                                                x.AttendenceDate >= firstCheckDay &&
                                                                x.AttendenceDate <= lastCheckDay &&
                                                                x.ApproveFor == AttendanceStatusModel.OnFieldDutyRequest)
                                                    .GroupBy(x => x.ApplicationDate)
                                                    .Select(group => group.Key) // Select only the distinct ApplicationDates
                                                    .Count();
                int leaveWithoutPay = 0;
                if (leave > 0)
                {
                    var leaveList = attendanceList.Where(x => x.EmpStatus == AttendanceStatusModel.OnLeave).Select(x=> x.AttendenceDate).ToArray();
                    var leaveApplicationList = _context.LeaveApplications.Where(x => x.Id == emp.Id && x.EndDate >= firstCheckDay && x.StartDate <= lastCheckDay && x.Status == (int)StatusEnum.Active && x.LeaveStatus == (int)LeaveStatusEnum.Approved).AsNoTracking().ToList();
                    foreach (var item in leaveList)
                    {
                        var ddddd = (from x in _context.LeaveApplications
                                     where x.Id == emp.Id && x.StartDate >= item && x.EndDate <= item
                                     && x.LeaveStatus == (int)LeaveStatusEnum.Approved && x.Status == (int)StatusEnum.Active
                                     && x.LeaveCategoryId == (int)LeaveTypeEnum.LeaveWithoutPay
                                     select x.Id).FirstOrDefault();
                        leaveWithoutPay += (from x in _context.LeaveApplications
                                            where x.Id == emp.Id && x.StartDate >= item && x.EndDate <= item
                                            && x.LeaveStatus == (int)LeaveStatusEnum.Approved && x.Status == (int)StatusEnum.Active
                                            && x.LeaveCategoryId == (int)LeaveTypeEnum.LeaveWithoutPay
                                            select x.Id).Count();
                    }
                    leave -= leaveWithoutPay;
                }

                emp.DaysOfPayment = onTime + weekend + holiday + tour + leave + lateIn + earlyOut + lateInEarlyOut;
                emp.TotalDays = totalDaysInMonth;
                emp.WorkingDay = workingDay;
                emp.Absent = absent;
                emp.Weekend = weekend;
                emp.Holiday = holiday;
                emp.Present = onTime + lateIn + earlyOut + lateInEarlyOut;
                emp.LeaveWitoutPay = leaveWithoutPay;
                emp.AttendanceModified = attendanceModified;
                emp.WorkOutSite = onFieldDuty;              


            }
            return employeeList;
        }
        public bool excuteattendance(string date)
        {
            DateTime dateTime = DateTime.Parse(date);

            var value = attendanceRepository.Database.ExecuteSqlCommand("exec sp_FinalAttendence  {0} ", dateTime);

            return true;
        }
        public bool excutemachinedata(string date)
        {
            DateTime dateTime = DateTime.Parse(date);

            var value = attendanceRepository.Database.ExecuteSqlCommand("exec sp_HRMS_GetMachineData  {0} ", dateTime);

            return true;
        }

    }
}
