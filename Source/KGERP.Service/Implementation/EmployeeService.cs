using AutoMapper.XpressionMapper;
using KGERP.Data.Models;
using KGERP.Service.Implementation.EmployeeLogService;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.Utility;
using KGERP.Utility;
using KGERP.Utility.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class EmployeeService : IEmployeeService, IDisposable
    {
        private bool disposed = false;
        private long? managerId;
        private readonly ERPEntities context;
        public EmployeeService(ERPEntities context)
        {
            this.context = context;
        }
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<EmployeeVm> GetEmployees(int companyId)
        {
            EmployeeVm model = new EmployeeVm();

            model.DataList = await Task.Run(() => (from t1 in context.Employees
                                                   join t2 in context.Departments on t1.DepartmentId equals t2.DepartmentId into x
                                                   from t2 in x.DefaultIfEmpty()
                                                   join t3 in context.Designations on t1.DesignationId equals t3.DesignationId into y
                                                   from t3 in y.DefaultIfEmpty()
                                                   join t4 in context.Grades on t1.GradeId equals t4.GradeId
                                                   where t1.Active && t1.CompanyId == companyId
                                                   select new EmployeeVm
                                                   {
                                                       Id = t1.Id,
                                                       EmployeeId = t1.EmployeeId,
                                                       EmployeeName = t1.Name,
                                                       DepartmentName = t2 != null ? t2.Name : "",
                                                       DesignationName = t3 != null ? t3.Name : "",
                                                       JoiningDate = t1.JoiningDate.Value,
                                                       MobileNo = t1.MobileNo,
                                                       OfficeEmail = t1.OfficeEmail,
                                                       GradeCode = t4.GradeCode,
                                                       GradeName = t4.Name,
                                                       PABX = t1.PABX,
                                                       Remarks = t1.Remarks

                                                   }).OrderBy(o => o.EmployeeId)
                                                   .AsEnumerable());

            return model;
        }
        public async Task<EmployeeVm> GetKSSLEmployees(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();

            model.DataList = await Task.Run(() => (from t1 in context.Employees
                                                   join t2 in context.Departments on t1.DepartmentId equals t2.DepartmentId into x
                                                   from t2 in x.DefaultIfEmpty()
                                                   join t3 in context.Designations on t1.DesignationId equals t3.DesignationId into y
                                                   from t3 in y.DefaultIfEmpty()
                                                   where t1.Active && t1.CompanyId == CompanyId
                                                   select new EmployeeVm
                                                   {
                                                       Id = t1.Id,
                                                       EmployeeId = t1.EmployeeId,
                                                       EmployeeName = t1.Name,
                                                       DepartmentName = t2 != null ? t2.Name : "",
                                                       DesignationName = t3 != null ? t3.Name : "",
                                                       JoiningDate = t1.JoiningDate.Value,
                                                       MobileNo = t1.MobileNo,
                                                       OfficeEmail = t1.OfficeEmail,
                                                       CompanyId = t1.CompanyId,

                                                   }).OrderBy(o => o.EmployeeId)
                                                   .AsEnumerable());

            return model;
        }
        public async Task<EmployeeVm> GetEmployeesCompanyWise(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();

            model.DataList = await Task.Run(() => (from t1 in context.Employees
                                                   join t2 in context.Departments on t1.DepartmentId equals t2.DepartmentId into x
                                                   from t2 in x.DefaultIfEmpty()
                                                   join t3 in context.Designations on t1.DesignationId equals t3.DesignationId into y
                                                   from t3 in y.DefaultIfEmpty()
                                                   join t5 in context.BankBranches on t1.BankBranchId equals t5.BankBranchId into b
                                                   from t5 in b.DefaultIfEmpty()
                                                   where t1.Active && t1.CompanyId == CompanyId
                                                   join t4 in context.Banks on t5.BankId equals t4.BankId into z
                                                   from t4 in z.DefaultIfEmpty()
                                                   select new EmployeeVm
                                                   {
                                                       Id = t1.Id,
                                                       EmployeeId = t1.EmployeeId,
                                                       EmployeeName = t1.Name,
                                                       DepartmentName = t2 != null ? t2.Name : "",
                                                       DesignationName = t3 != null ? t3.Name : "",
                                                       JoiningDate = t1.JoiningDate.Value,
                                                       MobileNo = t1.MobileNo,
                                                       OfficeEmail = t1.OfficeEmail,
                                                       SainatureName = t1.SignatureFileName,
                                                       BankAccNum = t1.BankAccount,
                                                       AccName = t1.BankAccountName,
                                                       BankName = t4.Name + " " + t5.Name
                                                   }).OrderBy(o => o.EmployeeId)
                                                   .AsEnumerable());

            return model;
        }

        public async Task<EmployeeVm> GetAllEmployeesWise()
        {
            EmployeeVm model = new EmployeeVm();

            model.DataList = await Task.Run(() => (from t1 in context.Employees
                                                   join t2 in context.Departments on t1.DepartmentId equals t2.DepartmentId into x
                                                   from t2 in x.DefaultIfEmpty()
                                                   join t3 in context.Designations on t1.DesignationId equals t3.DesignationId into y
                                                   from t3 in y.DefaultIfEmpty()
                                                   where t1.Active
                                                   select new EmployeeVm
                                                   {
                                                       Id = t1.Id,
                                                       EmployeeId = t1.EmployeeId,
                                                       EmployeeName = t1.Name,
                                                       DepartmentName = t2 != null ? t2.Name : "",
                                                       DesignationName = t3 != null ? t3.Name : "",
                                                       JoiningDate = t1.JoiningDate.Value,
                                                       MobileNo = t1.MobileNo,
                                                       OfficeEmail = t1.OfficeEmail,
                                                       SainatureName = t1.SignatureFileName,
                                                       BankAccNum = t1.BankAccount,
                                                       AccName = t1.BankAccountName
                                                   }).OrderBy(o => o.EmployeeId)
                                                   .AsEnumerable());

            return model;
        }

        public async Task<VmPayRoll> GetMySalaery(int myCompanyId)
        {
            VmPayRoll model = new VmPayRoll();

            model.DataList = await Task.Run(() => (from t1 in context.PRoll_PayRoll

                                                   where t1.IsActive && t1.CompanyId == myCompanyId
                                                   select new VmPayRoll
                                                   {
                                                       PayRollId = t1.PayRollId,
                                                       CompanyId = t1.CompanyId,
                                                       CreatedBy = t1.CreatedBy,
                                                       CreatedDate = t1.CreatedDate,
                                                       IsActive = t1.IsActive,
                                                       IsClose = t1.IsClose,
                                                       IsTest = t1.IsTest,
                                                       Month = t1.Month,
                                                       Year = t1.Year,
                                                       Note = t1.Note
                                                   }).OrderBy(o => o.PayRollId)
                                                   .AsEnumerable());

            return model;
        }

        public async Task<VmPayRollDetail> GetMySalaeryByPayrollId(long payRollId, long myEmployeeId)
        {
            VmPayRollDetail model = new VmPayRollDetail();

            model = await Task.Run(() => (from t2 in context.PRoll_PayRoll
                                          join t3 in context.Employees on myEmployeeId equals t3.Id
                                          join t1 in context.Designations on t3.DesignationId equals t1.DesignationId
                                          join t4 in context.Grades on t3.GradeId equals t4.GradeId
                                          join t5 in context.Companies on t3.CompanyId equals t5.CompanyId
                                          where t2.IsActive && t2.PayRollId == payRollId

                                          select new VmPayRollDetail
                                          {
                                              EmployeesName = t3.Name,
                                              EmployeesJoiningDate = t3.JoiningDate,
                                              EmployeesPABX = t3.PABX,
                                              EmployeesOfficeEmail = t3.OfficeEmail,
                                              EmployeesMobileNo = t3.MobileNo,
                                              DesignationsName = t1.Name,
                                              GradesName = t4.Name,
                                              GradesCode = t4.GradeCode,
                                              CompanyName = t5.Name,
                                              CompanyAddress = t5.Address,
                                              CompanyEmail = t5.Email,
                                              CompanyPhone = t5.Phone,
                                              PayRollId = t2.PayRollId,
                                              CompanyId = t2.CompanyId,
                                              CreatedBy = t2.CreatedBy,
                                              CreatedDate = t2.CreatedDate,
                                              IsActive = t2.IsActive,
                                              IsClose = t2.IsClose,
                                              IsTest = t2.IsTest,
                                              Month = t2.Month,
                                              Year = t2.Year,
                                              Note = t2.Note
                                          }).FirstOrDefault());

            var data = await Task.Run(() => (from t1 in context.PRoll_PayRollDetail
                                             join t3 in context.PRoll_PaymentPurpose on t1.PaymentPurposeId equals t3.PaymentPurposeId
                                             where t1.IsActive && t1.PayRollId == payRollId && t1.EmployeeId == myEmployeeId
                                             select new VmPayRollDetail
                                             {
                                                 Amount = t1.Amount,
                                                 EmployeeId = t1.EmployeeId,
                                                 CalculationType = t1.CalculationType,
                                                 PaymentDate = t1.PaymentDate,
                                                 PayRollDetailId = t1.PayRollDetailId,
                                                 PaymentPurposeId = t3.PaymentPurposeId,
                                                 PaymentPurposeName = t3.Name
                                             }).AsEnumerable());


            model.DataListAddition = data.Where(x => x.CalculationType == (int)CalculationTypeEnum.Addition).OrderBy(x => x.PaymentPurposeId).AsEnumerable();
            model.DataListDeduction = data.Where(x => x.CalculationType == (int)CalculationTypeEnum.Deduction).OrderBy(x => x.PaymentPurposeId).AsEnumerable();
            model.DataListNotDefined = data.Where(x => x.CalculationType == (int)CalculationTypeEnum.NotDefined).OrderBy(x => x.PaymentPurposeId).AsEnumerable();
            DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            model.MonthName = dtfi.GetMonthName(model.Month);

            return model;
        }

        public async Task<List<EmployeeModel>> GetEmployeesAsync(bool employeeType, string searchText)
        {
            List<Employee> employees = await context.Employees.Include("Department").Include("Designation").Where(x => x.Active == employeeType && (x.EmployeeId.Contains(searchText) || x.Name.Contains(searchText) || x.Department.Name.Contains(searchText) || x.Designation.Name.Contains(searchText) || x.MobileNo.Contains(searchText) || x.Email.Contains(searchText))).OrderBy(x => x.EmployeeId).ToListAsync();
            return ObjectConverter<Employee, EmployeeModel>.ConvertList(employees.ToList()).ToList();
        }

        private string GetEmployeeId(string employeeId)
        {
            string kg = employeeId.Substring(0, 2);

            string kgNumber = employeeId.Substring(2);
            int num = 0;
            if (employeeId != string.Empty)
            {
                num = Convert.ToInt32(kgNumber);
                ++num;
            }
            string newKgNumber = num.ToString().PadLeft(4, '0');
            return kg + newKgNumber;
        }
        private string GetEmployeeIdISS(string employeeId, int companyId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                throw new ArgumentException("Invalid employee ID.");
            }
            int index = 0;
            while (index < employeeId.Length && !char.IsDigit(employeeId[index]))
            {
                index++;
            }
            string textPart = employeeId.Substring(0, index);
            string numericPart = employeeId.Substring(index);
            textPart = context.Companies.Where(x => x.CompanyId == companyId).Select(x => x.ShortName).FirstOrDefault();
            if (int.TryParse(numericPart, out int number))
            {
                number++;
                string incrementedNumericPart = number.ToString(new string('0', numericPart.Length));
                return textPart + incrementedNumericPart;
            }
            else
            {
                throw new ArgumentException("Invalid numeric part in employee ID.");
            }
        }

        private string GetEmployeeIdKSSL(string employeeId, int CompanyId)
        {
            string kg = employeeId.Substring(0, 4);

            string kgNumber = employeeId.Substring(4);
            int num = 0;
            if (employeeId != string.Empty)
            {
                num = Convert.ToInt32(kgNumber);
                ++num;
            }
            string newKgNumber = num.ToString().PadLeft(4, '0');
            return kg + newKgNumber;
        }
        public List<SelectModel> GetEmployeesForSmsByCompanyId(int companyId = 0, int departmentId = 0)
        {
            List<SelectModel> list = new List<SelectModel>();
            list = context.Employees.Where(e =>
            e.MobileNo != null &&
            e.MobileNo.Length >= 11 &&
            (departmentId == 0 ? e.DepartmentId != 0 : e.DepartmentId == departmentId)
            &&
            (companyId == 0 ? e.CompanyId != 0 : e.CompanyId == companyId)
            ).ToList().
                Select(o => new SelectModel
                {
                    Text = $"{o.Name}[{o.EmployeeId}]-[{o.MobileNo}]",
                    Value = $"{o.MobileNo}"

                }).ToList();

            return list;
        }


        public EmployeeModel GetEmployee(long id, int companyId)
        {

            if (id <= 0)
            {
                //Employee lastEmployee = context.Employees.OrderByDescending(x => x.Id).FirstOrDefault();

                //Employee lastEmployee = context.Employees.OrderByDescending(x => x.EmployeeId).FirstOrDefault();
                //Employee lastEmployee = context.Employees.Where(x => x.EmployeeId.StartsWith("KG")).OrderByDescending(x => x.EmployeeId).FirstOrDefault();
                Employee lastEmployee = context.Employees.Where(x => x.CompanyId == companyId && x.Active).OrderByDescending(x => x.EmployeeId).FirstOrDefault();

                if (lastEmployee == null)
                {
                    int eid = 1;
                    string formattedEid = eid.ToString("D4");
                    return new EmployeeModel() { EmployeeId = context.Companies.Where(x => x.CompanyId == companyId).Select(x => x.ShortName).FirstOrDefault() + formattedEid };
                }
                return new EmployeeModel()
                {
                    EmployeeId = GetEmployeeIdISS(lastEmployee.EmployeeId, companyId)
                };
            }
            this.context.Database.CommandTimeout = 180;
            //Employee employee = context.Employees.Include(x => x.FileAttachments).Include("Employee3").Include("Company").Include("Department").Include("Designation").Include("District").Include("Shift").Include("Grade").Include("Bank").Include("BankBranch").Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Include("DropDownItem3").Include("DropDownItem4").Include("DropDownItem5").Include("DropDownItem6").Include("DropDownItem7").Include("DropDownItem8").Include("DropDownItem9").OrderByDescending(x => x.Id == id).FirstOrDefault();
            string PassText;
            var PassText1 = (from e in context.Employees
                               join user in context.AdminSetUps
                               on e.EmployeeId equals user.EmployeeId into userGroup
                               from user in userGroup.DefaultIfEmpty()
                               where e.Id == id && e.Active
                               select new
                               {
                                   Password = user.Remarks
                               }).FirstOrDefault();

            if (PassText1!=null)
            {
                PassText = PassText1.Password;
            }
            else
            {
                PassText = null;
            }

            Employee employee = context.Employees.Include(x => x.FileAttachments).Include("Employee3").Include("Company").Include("Department").Include("Designation").Include("District").Include("Grade").Include("Bank").Include("BankBranch").Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Include("DropDownItem3").Include("DropDownItem4").Include("DropDownItem5").Include("DropDownItem6").Include("DropDownItem7").Include("DropDownItem8").Include("DropDownItem9").OrderByDescending(x => x.Id == id).FirstOrDefault();

            if (employee.ShiftId != null)
            {
                employee.Shift = context.Shifts.Find(employee.ShiftId);
            }
            if (employee.RegionDistictId != null)
            {
                employee.District1 = context.Districts.Find(employee.RegionDistictId);
            }
            if (employee.VendorId != null && employee.VendorId > 0)
            {
                employee.Vendor = context.Vendors.Find(employee.VendorId);
            }

            this.context.Database.CommandTimeout = 180;
            var ObjData = ObjectConverter<Employee, EmployeeModel>.Convert(employee);
            ObjData.PasswordText = PassText == null ? "Password Not Found!" : PassText;
            return ObjData;
        }

        public EmployeeModel GetEmployeeForKSSL(long id, int CompanyId)
        {
            if (id <= 0)
            {
                //Employee lastEmployee = context.Employees.OrderByDescending(x => x.Id).FirstOrDefault();

                Employee lastEmployee = context.Employees
     .Where(x => x.EmployeeId.StartsWith("KSSL") && x.CompanyId == CompanyId)
     .OrderByDescending(x => x.EmployeeId)
     .FirstOrDefault();


                if (CompanyId == (int)CompanyName.KrishibidSecurityAndServicesLimited)
                {
                    if (lastEmployee == null)
                    {
                        return new EmployeeModel() { EmployeeId = "KSSL0001" };
                    }
                    return new EmployeeModel()
                    {
                        EmployeeId = GetEmployeeIdKSSL(lastEmployee.EmployeeId, CompanyId)
                    };
                }
            }
            this.context.Database.CommandTimeout = 180;
            //Employee employee = context.Employees.Include(x => x.FileAttachments).Include("Employee3").Include("Company").Include("Department").Include("Designation").Include("District").Include("Shift").Include("Grade").Include("Bank").Include("BankBranch").Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Include("DropDownItem3").Include("DropDownItem4").Include("DropDownItem5").Include("DropDownItem6").Include("DropDownItem7").Include("DropDownItem8").Include("DropDownItem9").OrderByDescending(x => x.Id == id).FirstOrDefault();
            Employee employee = context.Employees.Include(x => x.FileAttachments).Include("Employee3").Include("Company").Include("Department").Include("Designation").Include("District").Include("Grade").Include("Bank").Include("BankBranch").Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Include("DropDownItem3").Include("DropDownItem4").Include("DropDownItem5").Include("DropDownItem6").Include("DropDownItem7").Include("DropDownItem8").Include("DropDownItem9").OrderByDescending(x => x.Id == id).FirstOrDefault();
            if (employee.ShiftId != null)
            {
                employee.Shift = context.Shifts.Find(employee.ShiftId);
            }
            if (employee.RegionDistictId != null)
            {
                employee.District1 = context.Districts.Find(employee.RegionDistictId);
            }
            this.context.Database.CommandTimeout = 180;
            return ObjectConverter<Employee, EmployeeModel>.Convert(employee);
        }

        public EmployeeModel GetEmployeeForAcc(long id)
        {
            EmployeeModel model = new EmployeeModel();


            Employee lastEmployee = context.Employees.OrderByDescending(x => x.Id).FirstOrDefault();
            if (lastEmployee != null)
            {
                model = (from t1 in context.Employees
                         where t1.Id == id && t1.Active
                         join t2 in context.Designations on t1.DesignationId equals t2.DesignationId into designationJoin
                         from t2 in designationJoin.DefaultIfEmpty()
                         join t3 in context.BankBranches on t1.BankBranchId equals t3.BankBranchId into bankBranchJoin
                         from t3 in bankBranchJoin.DefaultIfEmpty()
                         select new EmployeeModel
                         {
                             Id = t1.Id,
                             Name = t1.Name,
                             DesignationId = t1.DesignationId,
                             DesignationName = t2 != null ? t2.Name : null,
                             BankAccount = t1.BankAccount,
                             BankBranchId = t1.BankBranchId,
                             AccouName = t1.BankAccountName,
                             CompanyId = t1.CompanyId
                         }).FirstOrDefault();



                model.DropdownForBank = (from t1 in context.Banks
                                         join t2 in context.BankBranches on t1.BankId equals t2.BankId
                                         where t1.IsActive && t1.CompanyId == model.CompanyId
                                         select new SelectVmAcc
                                         {

                                             Id = t2.BankBranchId,
                                             value = t1.Name + "-" + t2.Name

                                         }).ToList();

            }
            return model;
        }
        //public EmployeeViewModel GetSignatureByID(long Id)
        //{
        //    EmployeeViewModel model = new EmployeeViewModel();
        //    Employee lastEmployee = context.Employees.OrderByDescending(x => x.Id).FirstOrDefault();
        //    if (lastEmployee != null)
        //    {
        //         model = (from t1 in context.Employees
        //                                   join t2 in context.Designations on t1.DesignationId equals t2.DesignationId
        //                                   where t1.Id == Id && t1.Active
        //                                   select new EmployeeViewModel
        //                                   {

        //                                       Id = t1.Id,
        //                                       Name = t1.Name,
        //                                       Designation = t2.Name

        //                                   }).FirstOrDefault();

        //    }
        //    return model;
        //}








        public EmployeeModel GetEmployee(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                //Employee lastEmployee = context.Employees.OrderByDescending(x => x.Id).FirstOrDefault();

                Employee lastEmployee = context.Employees.OrderByDescending(x => x.EmployeeId).FirstOrDefault();

                if (lastEmployee == null)
                {
                    return new EmployeeModel() { EmployeeId = "KG0001" };
                }
                return new EmployeeModel()
                {
                    EmployeeId = GetEmployeeId(lastEmployee.EmployeeId)
                };
            }
            this.context.Database.CommandTimeout = 180;
            Employee employee = context.Employees.Include(x => x.FileAttachments).Include("Employee3").Include("Company").Include("Department").Include("Designation").Include("District").Include("Shift").Include("Grade").Include("Bank").Include("BankBranch").Include("DropDownItem").Include("DropDownItem1").Include("DropDownItem2").Include("DropDownItem3").Include("DropDownItem4").Include("DropDownItem5").Include("DropDownItem6").Include("DropDownItem7").Include("DropDownItem8").Include("DropDownItem9").OrderByDescending(x => x.EmployeeId == employeeId).FirstOrDefault();
            this.context.Database.CommandTimeout = 180;
            return ObjectConverter<Employee, EmployeeModel>.Convert(employee);
        }
        public bool CheckEmployee(long id)
        {
            var emp = context.Employees.FirstOrDefault(x => x.Active && x.Id == id);
            if (emp != null)
            {
                return true;
            }
            return false;
        }

        public EmployeeModel GetEmployeeByKGID(string employeeId)
        {
            Employee lastEmployee = context.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            return ObjectConverter<Employee, EmployeeModel>.Convert(lastEmployee);
        }

        //private async void EmployeeDependenceUpdate(long EmplpyeeId, int CompanyId)
        //{
        //    DateTime currentDate = DateTime.Now;

        //    DateTime firstDateOfLastMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-1);
        //    DateTime lastDayOfMonth = firstDateOfLastMonth.AddMonths(1).AddDays(-1);


        //    var currentMonthDataOfAttendanceLogDetail = await context.PRoll_AttendanceLogDetail.Where(x => x.CreatedDate >= firstDateOfLastMonth
        //                                                                                              && x.CreatedDate <= lastDayOfMonth
        //                                                                                              && x.CompanyId == CompanyId
        //                                                                                              && x.EmployeeId == EmplpyeeId
        //                                                                                              ).ToListAsync();
        //    if (currentMonthDataOfAttendanceLogDetail.Count>0)
        //    {
        //        currentMonthDataOfAttendanceLogDetail.ForEach(x => x.CompanyId = CompanyId);
        //    }

        //    if (await context.SaveChangesAsync() > 0)
        //    {

        //    }
        //}
        public bool SaveEmployee(long id, EmployeeModel model)
        {
            if (model == null)
            {
                throw new Exception(Constants.DATA_NOT_FOUND);
            }
            if (model == null)
            {
                throw new Exception(Constants.DATA_NOT_FOUND);
            }
            Employee employee = ObjectConverter<EmployeeModel, Employee>.Convert(model);

            bool IsCompanyChange = false;

            if (id > 0)
            {
                employee = context.Employees.FirstOrDefault(x => x.Id == id);
                IsCompanyChange = employee.CompanyId != model.CompanyId;
                if (IsCompanyChange)
                {
                    PRoll_SalaryConfiguration pRoll_SalaryConfiguration = context.PRoll_SalaryConfiguration.FirstOrDefault(x => x.EmployeeId == id);
                    if (pRoll_SalaryConfiguration != null)
                    {
                        pRoll_SalaryConfiguration.CompanyId = (int)model.CompanyId;
                        context.Entry(pRoll_SalaryConfiguration).State = EntityState.Modified;
                        if (context.SaveChanges() > 0)
                        {
                            //EmployeeDependenceUpdate(id, model.CompanyId.Value);
                        }
                    }

                }
                managerId = employee.ManagerId;
                if (employee == null)
                {
                    throw new Exception(Constants.DATA_NOT_FOUND);
                }

                employee.ModifiedDate = DateTime.Now;
                employee.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
                if (!string.IsNullOrEmpty(model.ImageFileName))
                {
                    employee.ImageFileName = model.ImageFileName;
                }

                if (!string.IsNullOrEmpty(model.SignatureFileName))
                {
                    employee.SignatureFileName = model.SignatureFileName;
                }

                if (model.Active == false)
                {

                    employee.EndReason = model.EndReason;
                    employee.EndDate = model.EndDate;
                    if (!model.EndDate.HasValue)
                    {
                        employee.EndDate = DateTime.Today;
                    }
                    if (employee.EndDate <= DateTime.Today)
                    {
                        User user = context.Users.FirstOrDefault(d => d.UserName == model.EmployeeId);
                        if (user != null)
                        {
                            user.Active = false;
                            user.IsEmailVerified = false;

                            context.Users.Add(user);
                            context.Entry(user).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }


                }
                //else
                //{
                //    employee.EndDate = null;
                //    employee.EndReason = null;
                //    User user = context.Users.FirstOrDefault(d => d.UserName == model.EmployeeId);
                //    if (user != null)
                //    {
                //        user.Active = true;
                //        user.IsEmailVerified = true;
                //        context.Users.Add(user);
                //        context.Entry(user).State = EntityState.Modified;
                //        context.SaveChanges();
                //    }

                //}



            }
            else
            {
                UserModel userModel = new UserModel();
                userModel.UserName = model.EmployeeId;
                userModel.Email = "kgerp19@gmail.com";
                userModel.Active = model.Active;
                userModel.IsEmailVerified = true;

                userModel.Password = Crypto.Hash(userModel.UserName.ToLower());
                // userModel.Password = userModel.UserName.ToLower();
                userModel.ConfirmPassword = userModel.Password;
                userModel.ActivationCode = Guid.NewGuid();
                User user = ObjectConverter<UserModel, User>.Convert(userModel);

                context.Users.Add(user);
                int isUserSaved = context.SaveChanges();
                if (isUserSaved <= 0)
                    throw new Exception(Constants.OPERATION_FAILE);

                employee.HrAdminId = Common.GetHRAdminId();
                employee.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                employee.CreatedDate = DateTime.Now;
                employee.Active = true;

                context.Employees.Add(employee);
                try
                {
                    if (context.SaveChanges() > 0)
                    {
                        context.Database.ExecuteSqlCommand("exec insertInvalidException {0},{1}", userModel.UserName, userModel.UserName.ToLower());
                        //-----------------Default Menu Assign--------------------
                        int noOfRowsAffected = context.Database.ExecuteSqlCommand("spHRMSAssignDefaultMenu {0},{1}", employee.EmployeeId, employee.CreatedBy);
                        //LeaveService.InsertSickLeave(employee.Id, employee.JoiningDate ?? DateTime.Today, true);
                        id = employee.Id;
                        model.Id = employee.Id;
                        var newSignatory = new RequisitionSignatory();
                        for (int z = 0; z < 2; z++)
                        {
                            newSignatory.EmployeeId = employee.Id;
                            if (z == 0 && model.ManagerId != null)
                            {
                                newSignatory.SignatoryEmpId = (long)model.ManagerId;
                                newSignatory.IsHRAdmin = false;
                                newSignatory.OrderBy = 1;
                            }
                            else
                            {
                                newSignatory.SignatoryEmpId = Common.GetHRAdminId();
                                newSignatory.IsHRAdmin = true;
                                newSignatory.OrderBy = 2;
                            }
                            newSignatory.CreatedBy = Common.GetUserId();
                            newSignatory.CreatedDate = DateTime.Now;
                            newSignatory.IntegrateWith = "LeaveApplication";
                            newSignatory.IsActive = true;
                            if (model.DesignationId != null)
                            {
                                var designationName = context.Designations.Where(x => x.DesignationId == model.DesignationId).Select(x => x.Name).FirstOrDefault();
                                newSignatory.DesignationName = designationName;
                            }
                            newSignatory.Limitation = 0;

                            context.RequisitionSignatories.Add(newSignatory);
                            context.SaveChanges();
                        }

                        return noOfRowsAffected > 0;
                    }
                }
                catch (DbEntityValidationException e)
                {
                    context.Users.Remove(user);
                    return context.SaveChanges() > 0;
                }
            }

            if (model.Active)
            {
                employee.Active = model.Active;
                employee.EmployeeId = model.EmployeeId;
                employee.ManagerId = model.ManagerId;
                employee.HrAdminId = Common.GetHRAdminId();
                employee.CardId = model.CardId;
                employee.ShortName = model.ShortName;
                employee.Name = model.Name;
                employee.GenderId = model.GenderId;
                employee.PresentAddress = model.PresentAddress;
                employee.FatherName = model.FatherName;
                employee.MotherName = model.MotherName;
                employee.SpouseName = model.SpouseName;
                employee.Telephone = model.Telephone;
                employee.MobileNo = model.MobileNo;
                employee.PABX = model.PABX;
                employee.FaxNo = model.FaxNo;
                employee.Email = model.Email;
                employee.SocialId = model.SocialId;
                employee.OfficeEmail = model.OfficeEmail;
                employee.PermanentAddress = model.PermanentAddress;
                employee.DepartmentId = model.DepartmentId;
                employee.DesignationId = model.DesignationId;
                employee.EmployeeCategoryId = model.EmployeeCategoryId;
                employee.ServiceTypeId = model.ServiceTypeId;
                employee.JobStatusId = model.JobStatusId;
                employee.JoiningDate = model.JoiningDate;
                employee.ProbationEndDate = model.ProbationEndDate;
                employee.PermanentDate = model.PermanentDate;
                employee.CompanyId = model.CompanyId;
                employee.ShiftId = model.ShiftId;
                employee.DateOfBirth = model.DateOfBirth;
                employee.DateOfMarriage = model.DateOfMarriage;
                employee.GradeId = model.GradeId;
                employee.CountryId = model.CountryId;
                employee.MaritalTypeId = model.MaritalTypeId;
                employee.DivisionId = model.DivisionId;
                employee.DistrictId = model.DistrictId;
                employee.UpzillaId = model.UpzillaId;
                employee.BankId = model.BankId;
                employee.BankBranchId = model.BankBranchId;
                employee.BankAccount = model.BankAccount;
                employee.DrivingLicenseNo = model.DrivingLicenseNo;
                employee.PassportNo = model.PassportNo;
                employee.NationalId = model.NationalId;
                employee.TinNo = model.TinNo;
                employee.ReligionId = model.ReligionId;
                employee.BloodGroupId = model.BloodGroupId;
                employee.DesignationFlag = model.DesignationFlag;
                employee.DisverseMethodId = model.DisverseMethodId;
                employee.OfficeTypeId = model.OfficeTypeId;
                employee.Remarks = model.Remarks;
                employee.EmployeeOrder = model.EmployeeOrder;
                employee.SalaryTag = model.SalaryTag;
                employee.RegionDistictId = model.RegionDistictId;
                employee.VendorId = model.VendorId;
            }
            else if(!model.Active)
            {
                employee.Active = model.Active;
            }
            employee.ModifedBy = Common.GetUserId();
            employee.ModifiedDate = DateTime.Now;





            //employee.ServiceCompany = model.ServiceCompany;
            //var existEmployee = context.Employees.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);

            //long employeeId = (from i in context.Employees
            //                   where i.EmployeeId == model.EmployeeId
            //                   select i.Id).FirstOrDefault();
            try
            {

                bool u = context.SaveChanges() > 0;

                if (u == true)
                {
                    //model.Id = employee.Id;
                    //context.LeaveApplications.Where(w => w.Id == employeeId && w.ManagerStatus == "Pending").ToList().ForEach(i => i.ManagerId = model.ManagerId);
                    //context.AttendenceApproveApplications.Where(w => w.EmployeeId == employeeId && w.ManagerStatus == 0).ToList().ForEach(i => i.ManagerId = model.ManagerId);

                    //if (true)
                    //{
                    //    LeaveService.InsertSickLeave(id, employee.JoiningDate??DateTime.Today, false);
                    //}
                    //Manager update start
                    if (managerId != model.ManagerId && model.ManagerId != 0)
                    {
                        var AttendenceApprove = context.AttendenceApproveApplications.Where(x => x.EmployeeId == id && x.ManagerStatus == 0).ToList();
                        var LeaveApply = context.LeaveApplications.Where(x => x.Id == id && x.ManagerStatus == LeaveStatusEnum.Pending.ToString() && x.Status == (int)StatusEnum.Active).ToList();

                        if (AttendenceApprove.Count() != 0)
                        {
                            foreach (var item in AttendenceApprove)
                            {
                                item.ManagerId = model.ManagerId;
                                context.AttendenceApproveApplications.Add(item);
                                context.Entry(item).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }

                        if (LeaveApply.Count() != 0)
                        {
                            foreach (var liv in LeaveApply)
                            {
                                liv.ManagerId = model.ManagerId;
                                context.LeaveApplications.Add(liv);
                                context.Entry(liv).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }
                    }

                    //Manager update end

                }
                id = employee.Id;
                model.Id = employee.Id;
                return u;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public bool DeleteEmployee(long id)
        {
            Employee employee = context.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                throw new Exception(Constants.DATA_NOT_FOUND);
            }

            context.Employees.Remove(employee);
            return context.SaveChanges() > 0;

        }

        public List<SelectModel> GetEmployeeSelectModels()
        {
            var data = (from e in context.Employees
                        join ds in context.Designations on e.DesignationId equals ds.DesignationId
                        where e.Active
                        select new SelectModel
                        {
                            Text = "[" + e.EmployeeId.ToString() + "] " + e.Name + " (" + ds.Name + ")",
                            Value = e.Id.ToString()
                        }).AsNoTracking().ToList();
            return data;
        }
        public List<SelectModel> GetEmployeeSelectModelsISS(int companyId)
        {
            var data = (from e in context.Employees
                        join ds in context.Designations on e.DesignationId equals ds.DesignationId
                        where e.Active && e.CompanyId == companyId
                        select new SelectModel
                        {
                            Text = "[" + e.EmployeeId.ToString() + "] " + e.Name + " (" + ds.Name + ")",
                            Value = e.Id.ToString()
                        }).AsNoTracking().ToList();
            return data;
        }

        public bool SaveAdminSetUp(long id, AdminSetUpModel adminSetUp)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeModel> EmployeeSearch(string searchText)
        {
            IQueryable<Employee> employees = context.Employees.Include("Department").Include("Designation").Include("DropDownItem").Where(x => x.Active && (x.EmployeeId.Contains(searchText) || x.Name.Contains(searchText) || x.Department.Name.Contains(searchText) || x.Designation.Name.Contains(searchText) || x.PABX.Contains(searchText) || x.MobileNo.Contains(searchText) || x.OfficeEmail.Contains(searchText) || x.EndReason.Contains(searchText) || x.DropDownItem.Name.Contains(searchText))).OrderBy(x => x.EmployeeOrder);
            return ObjectConverter<Employee, EmployeeModel>.ConvertList(employees.ToList()).ToList();
        }

        public List<EmployeeModel> GetBirthday()
        {
            var b = ObjectConverter<Employee, EmployeeModel>.ConvertList(
                context.Employees.Include("Department").Include("Designation").Where(
                e => e.DateOfBirth.Value.Day == DateTime.Now.Day
                && e.DateOfBirth.Value.Month == DateTime.Now.Month).OrderBy(x => x.Id).ToList())
                .ToList();

            var bw = ObjectConverter<Employee, EmployeeModel>.ConvertList(
                context.Employees.Include("Department").Include("Designation").Where(
                e => e.DateOfBirth.Value.Day == DateTime.Now.Day
                && e.DateOfBirth.Value.Month == DateTime.Now.Month).OrderBy(x => x.Id).ToList())

                .ToList();
            return b;
        }

        public List<EmployeeModel> GetEmployeeEvent()
        {
            dynamic result = context.Database.SqlQuery<EmployeeModel>("exec sp_GetEmployeeEvent").ToList();
            return result;
        }

        public List<EmployeeModel> GetEmployeeTodayEvent()
        {
            dynamic result = context.Database.SqlQuery<EmployeeModel>("exec sp_Employee_TodayAniversaryEvent").ToList();
            return result;
        }

        public List<EmployeeModel> GetProbitionPreiodEmployeeList()
        {
            dynamic result = context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetProbitionPreiodEmployeeList").ToList();
            return result;
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
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public object GetEmployeeAutoComplete(string prefix)
        {
            return context.Employees.Where(x => x.Active
                                        && (x.Name.Contains(prefix) || x.EmployeeId.Contains(prefix))
            ).Select(x => new
            {
                label = x.Name + " [" + x.EmployeeId + "]",
                val = x.Id
            }).OrderBy(x => x.label).Take(10).ToList();

        }

        public object GetEmployeeAutoCompleteByCompany(string prefix, int ComapnyId)
        {
            return context.Employees.Where(x => x.CompanyId == ComapnyId && x.Active
                                             && x.Name.Contains(prefix)

            ).Select(x => new
            {
                label = x.Name + " [" + x.EmployeeId + "]",
                val = x.Id
            }).OrderBy(x => x.label).Take(10).ToList();

        }
        public object GetEmployeeAutoCompleteOfficer(string prefix, int CompanyId)
        {
            return context.Employees.Where(x => x.Active && x.CompanyId == CompanyId && x.Name.Contains(prefix)).Select(x => new
            {
                label = x.Name + " [" + x.EmployeeId + "]",
                val = x.Id
            }).OrderBy(x => x.label).Take(10).ToList();

        }











        public List<EmployeeModel> GetTeamMembers(string searchText)
        {
            string managerId = System.Web.HttpContext.Current.User.Identity.Name;
            IQueryable<EmployeeModel> members = context.Database.SqlQuery<EmployeeModel>("spGetTeamMembers {0}", managerId).AsQueryable();
            return members.Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)).ToList();
        }

        public EmployeeModel GetTeamMember(long id)
        {
            Employee employee = context.Employees.Find(id);
            return ObjectConverter<Employee, EmployeeModel>.Convert(employee);
        }

        public bool UpdateTeamMember(EmployeeModel model)
        {
            if (model == null)
            {
                throw new Exception("Data missing!");
            }
            Employee member = ObjectConverter<EmployeeModel, Employee>.Convert(model);

            member = context.Employees.Where(x => x.Id == model.Id).First();
            member.Active = model.Active;
            member.EndDate = model.EndDate;
            member.EndReason = model.EndReason;

            member.ModifedBy = System.Web.HttpContext.Current.User.Identity.Name;
            member.ModifiedDate = DateTime.Now;
            member.Department = null;

            context.Entry(member).State = member.Id == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public List<EmployeeModel> GetEmployeeAdvanceSearch(int? departmentId, int? designationId, string searchText)
        {
            IQueryable<EmployeeModel> queryable = context.Database.SqlQuery<EmployeeModel>("sp_HRMS_GetEmployeeAdvanceSearch {0},{1},{2}", departmentId, designationId, searchText).AsQueryable();
            return queryable.ToList();
        }

        public long GetIdByKGID(string kgId)
        {
            try
            {
                return context.Employees.Where(x => x.EmployeeId.ToLower().Equals(kgId.ToLower())).First().Id;
            }
            catch (Exception)
            {

                return 0;
            }

        }


        public bool savesignature(EmployeeModel vm)
        {
            if (vm != null)
            {
                var obj = context.Employees.Where(x => x.Id == vm.Id).FirstOrDefault();
                obj.SignatureFileName = vm.SignatureFileName;
                context.SaveChanges();
            }
            return true;
        }

        public bool saveAccountNumber(EmployeeModel vm)
        {
            if (vm != null)
            {
                var obj = context.Employees.Where(x => x.Id == vm.Id).FirstOrDefault();
                obj.BankAccount = vm.BankAccount;
                obj.BankBranchId = vm.BankBranchId;
                obj.BankAccountName = vm.AccouName;
                context.SaveChanges();
            }
            return true;
        }


        public List<EmployeeModel> EmployeeSearch()
        {
            return context.Database.SqlQuery<EmployeeModel>(@"select e.EmployeeId, e.Name,
isnull(replace(convert(NVARCHAR, e.JoiningDate, 105), ' ', '/'),'') as StrJoiningDate,
IsNull(d.Name,'') as DepartmentName,
IsNull(g.GradeCode,'') Grade,
IsNull(g.Name,'') GradeName,
IsNull(ds.Name,'') as DesignationName,
isnull(OfficeEmail,'') as OfficeEmail,isnull(PABX,'') as PABX,
isnull(MobileNo,'') as MobileNo,
IsNull(di.Name,'') as BloodGroupName,
isnull(e.Remarks,'') as Remarks
from Employee e
left join  Department d on  e.DepartmentId= d.DepartmentId
left Join Designation ds on e.DesignationId= ds.DesignationId
left join DropDownItem di on e.BloodGroupId = di.DropDownItemId
left join Grade g on e.GradeId = g.GradeId
where e.Active = 1").ToList();
        }




        public List<EmployeeModel> EmployeeSearchByCoId(int? idd)
        {
            string query = @"
            SELECT e.EmployeeId, e.Name,
            ISNULL(REPLACE(CONVERT(NVARCHAR, e.JoiningDate, 105), ' ', '/'), '') AS StrJoiningDate,
            ISNULL(d.Name, '') AS DepartmentName,
            ISNULL(g.GradeCode, '') AS Grade,
            ISNULL(g.Name, '') AS GradeName,
            ISNULL(ds.Name, '') AS DesignationName,
            ISNULL(OfficeEmail, '') AS OfficeEmail,
            ISNULL(PABX, '') AS PABX,
            ISNULL(MobileNo, '') AS MobileNo,
            ISNULL(di.Name, '') AS BloodGroupName,
            ISNULL(e.Remarks, '') AS Remarks
            FROM Employee e
            LEFT JOIN Department d ON e.DepartmentId = d.DepartmentId
            LEFT JOIN Designation ds ON e.DesignationId = ds.DesignationId
            LEFT JOIN DropDownItem di ON e.BloodGroupId = di.DropDownItemId
            LEFT JOIN Grade g ON e.GradeId = g.GradeId
            WHERE e.Active = 1 AND e.SalaryTag = 4 AND e.CompanyId = @idd";

            var iddParameter = new SqlParameter("@idd", idd);

            return context.Database.SqlQuery<EmployeeModel>(query, iddParameter).ToList();
        }

        public object GetEmployeeDesignationAutoComplete(string prefix)
        {
            return context.Employees.Include(x => x.Designation).Where(x => x.Active && x.Name.Contains(prefix)).Select(x => new
            {
                label = x.Name + " [" + x.Designation.Name + "]",
                val = x.Id
            }).OrderBy(x => x.label).Take(10).ToList();
        }

        #region exitinterview
        public ExitInterviewVM GetExitInterView(int id)
        {

            ExitInterviewVM exitInterviewVM = new ExitInterviewVM();
            exitInterviewVM.CareerOpportunities = new CareerOpportunity();

            if (id > 0)
            {
                var model = context.ExitInterviews.Find(id);
                if (model != null)
                {
                    if (!string.IsNullOrEmpty(model.EmployeeId))
                    {
                        var emp = GetEmployee(model.EmployeeId);
                        if (emp != null)
                        {
                            // exitInterviewVM.Id = emp.Id;
                            exitInterviewVM.EmployeeId = emp.EmployeeId;
                            exitInterviewVM.EmployeeName = emp.Name;
                            exitInterviewVM.DepartmentName = emp.DepartmentName;
                            exitInterviewVM.DesignationName = emp.DesignationName;
                            exitInterviewVM.CompanyName = emp.CompanyName;
                            exitInterviewVM.JoiningDate = emp.JoiningDate.Value;
                            exitInterviewVM.ResignDate = emp.EndDate ?? model.ExpectedResignDate;
                            exitInterviewVM.ManagerName = GetEmployee((long)emp.ManagerId, emp.CompanyId ?? 0).Name;
                        }

                    }


                    exitInterviewVM.Id = model.Id;
                    exitInterviewVM.ResignDate = model.ExpectedResignDate;
                    exitInterviewVM.ReasonForLeaving = model.ReasonForLeaving;
                    exitInterviewVM.IsAcceptedAnotherPosition = model.IsAcceptedAnotherPosition ?? false;
                    exitInterviewVM.WhatPromptedToSeekAnotherJOb = model.MotivationForJobChanges;
                    exitInterviewVM.WhenBeginSearchingAnotherJob = model.JobSearchStartFrom;
                    exitInterviewVM.WhatMakeNewJobMoreAttractive = model.AttractiveFactors;

                    exitInterviewVM.WhatMadeCareerGoalsBetter = model.CareerGoalsElseWhere;


                    exitInterviewVM.HaveUShereUrGoal = model.CareerGoalDiscussion;
                    exitInterviewVM.IsAdequateCareerOpportunitiesAvailableHere = model.IsCareerOpportunitiesSatisfaction ?? false;


                    //checkbox
                    exitInterviewVM.CareerOpportunities.PromotionalOpportunities = model.IsPromotionalOpportunities ?? false;
                    exitInterviewVM.CareerOpportunities.PositionRotations = model.IsPositionRotations ?? false;
                    exitInterviewVM.CareerOpportunities.IncreasedResponsibilities = model.IsIncreasedResponsibilities ?? false;
                    exitInterviewVM.CareerOpportunities.SpecialProjects = model.IsSpecialProjects ?? false;
                    exitInterviewVM.CareerOpportunities.Overseas = model.IsOverseas ?? false;
                    exitInterviewVM.CareerOpportunities.NoProgression = model.IsNoProgression ?? false;
                    exitInterviewVM.CareerOpportunities.Other = model.IsOther ?? false;

                    //rating job satisfaction

                    exitInterviewVM.RatingJobResponsibility = model.RatingJobResponsibilities ?? 0;
                    exitInterviewVM.RatingAchievingGoal = model.RatingGoalOpportunities ?? 0;
                    exitInterviewVM.RatingWorkEnvironment = model.RatingWorkEnvironment ?? 0;
                    exitInterviewVM.RatingDirectorOrManager = model.RatingManager ?? 0;
                    exitInterviewVM.RatingPay = model.RatingPay ?? 0;
                    exitInterviewVM.RatingBenefits = model.RatingBenefits ?? 0;


                    exitInterviewVM.WhatDidEnjoyMostAboutYourJob = model.JobEnjoyment;
                    exitInterviewVM.WhatDidEnjoyLeastAboutYourJob = model.JobChallenges;
                    exitInterviewVM.WhatMakesKrishibidGroupGoodPlaceToWork = model.PositiveAspectsOfWorkPlace;
                    exitInterviewVM.WhatMakesKrishibidGroupPoorPlaceToWork = model.NegativeAspectsOfWorkPlace;
                    exitInterviewVM.WhatRecommendationToMakingBetterToWork = model.WorkplaceImprovementSuggestions;
                    exitInterviewVM.WouldHaveUStatyedIfMoreStatisfactoryArrangementWorkedOUt = model.IsConsideredStaying ?? false;


                    exitInterviewVM.Status = (ApprovalStatusEnum)model.Status;

                    //checkbox
                }
            }
            else
            {
                long userId = Common.GetIntUserId();
                int companyId = Common.GetCompanyId();
                var emp = GetEmployee(userId, companyId);
                if (emp != null)
                {
                    // exitInterviewVM.Id = emp.Id;
                    exitInterviewVM.EmployeeId = emp.EmployeeId;
                    exitInterviewVM.EmployeeName = emp.Name;
                    exitInterviewVM.DepartmentName = emp.DepartmentName;
                    exitInterviewVM.DesignationName = emp.DesignationName;
                    exitInterviewVM.CompanyName = emp.CompanyName;
                    exitInterviewVM.JoiningDate = emp.JoiningDate.Value;
                    exitInterviewVM.ResignDate = emp.EndDate;
                    exitInterviewVM.ManagerName = GetEmployee((long)emp.ManagerId, companyId).Name;
                }
            }
            return exitInterviewVM;
        }
        public IEnumerable<ExitInterviewVM> GetAllExitInterView(string employeeId)
        {
            //var userId = Common.GetUserId();
            var data = (from e in context.ExitInterviews
                        join emp in context.Employees on e.EmployeeId equals emp.EmployeeId
                        where (string.IsNullOrEmpty(employeeId) || e.EmployeeId == employeeId)
                        where e.IsActive
                        select new ExitInterviewVM()
                        {
                            Id = e.Id,
                            EmployeeName = emp.Name,
                            EmployeeId = e.EmployeeId,
                            ResignDate = e.ExpectedResignDate,
                            CreatedDate = e.CreatedDate,
                            ReasonForLeaving = e.ReasonForLeaving,
                            Status = (ApprovalStatusEnum)e.Status,

                        }).AsEnumerable();
            return data;
        }
        public bool SaveExitInterView(ExitInterviewVM modelVM)
        {
            ExitInterview model = new ExitInterview();
            model.EmployeeId = modelVM.EmployeeId;
            model.ExpectedResignDate = modelVM.ResignDate;
            model.ReasonForLeaving = modelVM.ReasonForLeaving;
            model.IsAcceptedAnotherPosition = modelVM.IsAcceptedAnotherPosition;
            model.MotivationForJobChanges = modelVM.WhatPromptedToSeekAnotherJOb;
            model.JobSearchStartFrom = modelVM.WhenBeginSearchingAnotherJob;
            model.AttractiveFactors = modelVM.WhatMakeNewJobMoreAttractive;
            model.CareerGoalsElseWhere = modelVM.WhatMadeCareerGoalsBetter;
            model.CareerGoalDiscussion = modelVM.HaveUShereUrGoal;
            model.IsCareerOpportunitiesSatisfaction = modelVM.IsAdequateCareerOpportunitiesAvailableHere;


            //checkbox
            model.IsPromotionalOpportunities = modelVM.CareerOpportunities.PromotionalOpportunities;
            model.IsPositionRotations = modelVM.CareerOpportunities.PositionRotations;
            model.IsIncreasedResponsibilities = modelVM.CareerOpportunities.IncreasedResponsibilities;
            model.IsSpecialProjects = modelVM.CareerOpportunities.SpecialProjects;
            model.IsOverseas = modelVM.CareerOpportunities.Overseas;
            model.IsNoProgression = modelVM.CareerOpportunities.NoProgression;
            model.IsOther = modelVM.CareerOpportunities.Other;

            //rating job satisfaction
            // model.RatingJobResponsibilities = modelVM.RatingJobResponsibility;
            model.RatingJobResponsibilities = modelVM.RatingJobResponsibility;
            model.RatingGoalOpportunities = modelVM.RatingAchievingGoal;
            model.RatingWorkEnvironment = modelVM.RatingWorkEnvironment;
            // model.RatingManager = modelVM.RatingDirectorOrManager;
            model.RatingManager = modelVM.RatingDirectorOrManager;
            model.RatingPay = modelVM.RatingPay;
            model.RatingBenefits = modelVM.RatingBenefits;


            model.JobEnjoyment = modelVM.WhatDidEnjoyMostAboutYourJob;
            model.JobChallenges = modelVM.WhatDidEnjoyLeastAboutYourJob;
            model.PositiveAspectsOfWorkPlace = modelVM.WhatMakesKrishibidGroupGoodPlaceToWork;
            model.NegativeAspectsOfWorkPlace = modelVM.WhatMakesKrishibidGroupPoorPlaceToWork;
            model.WorkplaceImprovementSuggestions = modelVM.WhatRecommendationToMakingBetterToWork;
            model.IsConsideredStaying = modelVM.WouldHaveUStatyedIfMoreStatisfactoryArrangementWorkedOUt;



            model.CreatedDate = DateTime.Now;
            model.CreatedBy = Common.GetUserId();
            model.Status = (int)ApprovalStatusEnum.Draft;

            model.IsActive = true;

            context.ExitInterviews.Add(model);
            if (context.SaveChanges() > 0)
            {
                modelVM.Id = model.Id;
                return true;
            }
            return false;
        }
        public bool UpdateExitInterView(ExitInterviewVM modelVM)
        {
            ExitInterview model = context.ExitInterviews.Find(modelVM.Id);
            if (model == null)
            {
                return false;
            }
            model.EmployeeId = modelVM.EmployeeId;
            model.ExpectedResignDate = modelVM.ResignDate;
            model.ReasonForLeaving = modelVM.ReasonForLeaving;
            model.IsAcceptedAnotherPosition = modelVM.IsAcceptedAnotherPosition;
            model.MotivationForJobChanges = modelVM.WhatPromptedToSeekAnotherJOb;
            model.JobSearchStartFrom = modelVM.WhenBeginSearchingAnotherJob;
            model.AttractiveFactors = modelVM.WhatMakeNewJobMoreAttractive;
            model.CareerGoalsElseWhere = modelVM.WhatMadeCareerGoalsBetter;
            model.CareerGoalDiscussion = modelVM.HaveUShereUrGoal;
            model.IsCareerOpportunitiesSatisfaction = modelVM.IsAdequateCareerOpportunitiesAvailableHere;


            //checkbox
            model.IsPromotionalOpportunities = modelVM.CareerOpportunities.PromotionalOpportunities;
            model.IsPositionRotations = modelVM.CareerOpportunities.PositionRotations;
            model.IsIncreasedResponsibilities = modelVM.CareerOpportunities.IncreasedResponsibilities;
            model.IsSpecialProjects = modelVM.CareerOpportunities.SpecialProjects;
            model.IsOverseas = modelVM.CareerOpportunities.Overseas;
            model.IsNoProgression = modelVM.CareerOpportunities.NoProgression;
            model.IsOther = modelVM.CareerOpportunities.Other;

            //rating job satisfaction
            // model.RatingJobResponsibilities = modelVM.RatingJobResponsibility;
            model.RatingJobResponsibilities = modelVM.RatingJobResponsibility;
            model.RatingGoalOpportunities = modelVM.RatingAchievingGoal;
            model.RatingWorkEnvironment = modelVM.RatingWorkEnvironment;
            // model.RatingManager = modelVM.RatingDirectorOrManager;
            model.RatingManager = modelVM.RatingDirectorOrManager;
            model.RatingPay = modelVM.RatingPay;
            model.RatingBenefits = modelVM.RatingBenefits;


            model.JobEnjoyment = modelVM.WhatDidEnjoyMostAboutYourJob;
            model.JobChallenges = modelVM.WhatDidEnjoyLeastAboutYourJob;
            model.PositiveAspectsOfWorkPlace = modelVM.WhatMakesKrishibidGroupGoodPlaceToWork;
            model.NegativeAspectsOfWorkPlace = modelVM.WhatMakesKrishibidGroupPoorPlaceToWork;
            model.WorkplaceImprovementSuggestions = modelVM.WhatRecommendationToMakingBetterToWork;
            model.IsConsideredStaying = modelVM.WouldHaveUStatyedIfMoreStatisfactoryArrangementWorkedOUt;



            model.CreatedDate = DateTime.Now;
            model.CreatedBy = Common.GetUserId();
            //model.Status = (int)ApprovalStatusEnum.Draft;
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }
        public bool DeleteExitInterView(int id)
        {
            var exist = context.ExitInterviews.Find(id);
            if (exist != null)
            {
                exist.IsActive = false;
                if (context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public BusinessHeadModel GetClearanceSignatoryById(int id)
        {
            var data = (from e in context.ClearanceSignatories
                        join emp in context.Employees on e.EmployeeId equals emp.Id
                        where e.ClearanceSignatoryId == id && e.IsActive
                        select new BusinessHeadModel
                        {
                            Id = e.ClearanceSignatoryId,
                            EmployeeId = e.EmployeeId,
                            BusinessName = e.CompanyId.HasValue ? context.Companies.FirstOrDefault(c => c.CompanyId == e.CompanyId).Name
                                                                 : context.Departments.FirstOrDefault(d => d.DepartmentId == e.DepartmentId).Name,
                            BusinessType = e.CompanyId.HasValue ? BusinessTypeEnum.Company : BusinessTypeEnum.Division,
                            BusinessTypeInt = e.CompanyId.HasValue ? (int)BusinessTypeEnum.Company : (int)BusinessTypeEnum.Division,
                            BusineesId_Fk = e.CompanyId.HasValue ? e.CompanyId.Value : e.DepartmentId.Value,
                            EmployeeName = emp.Name
                        })
                       .FirstOrDefault();
            return data;
        }
        public IEnumerable<BusinessHeadModel> GetClearanceSignatory(int? companyId, int? departmentId)
        {
            var data = (from e in context.ClearanceSignatories
                        join emp in context.Employees on e.EmployeeId equals emp.Id
                        where e.IsActive &&
                        ((e.CompanyId.HasValue && e.DepartmentId == null) || (e.DepartmentId.HasValue && e.CompanyId == null))
                        select new BusinessHeadModel
                        {
                            Id = e.ClearanceSignatoryId,
                            EmployeeId = e.EmployeeId,
                            BusinessName = e.CompanyId.HasValue ? context.Companies.FirstOrDefault(c => c.CompanyId == e.CompanyId).Name
                                                                 : context.Departments.FirstOrDefault(d => d.DepartmentId == e.DepartmentId).Name,
                            BusinessType = e.CompanyId.HasValue ? BusinessTypeEnum.Company : BusinessTypeEnum.Division,
                            EmployeeName = emp.Name

                        })
                        .AsEnumerable();
            return data;
        }
        public bool SaveClearanceSignatory(BusinessHeadModel model)
        {
            bool iscompanywise = model.BusinessType == BusinessTypeEnum.Company ? true : false;
            var existClearance = (from x in context.ClearanceSignatories
                                  where x.EmployeeId == model.EmployeeId && x.IsActive
                                  && ((iscompanywise && x.CompanyId == model.BusineesId_Fk)
                                  || (!iscompanywise && x.DepartmentId == model.BusineesId_Fk))
                                  select x).FirstOrDefault();
            if (existClearance == null)
            {
                ClearanceSignatory signatory = new ClearanceSignatory();
                signatory.IsActive = true;
                signatory.CreatedBy = Common.GetUserId();
                signatory.CreatedDate = DateTime.Now;
                signatory.EmployeeId = model.EmployeeId;
                if (iscompanywise)
                {

                    signatory.CompanyId = model.BusineesId_Fk;
                    signatory.DepartmentId = null;
                }
                else
                {
                    signatory.DepartmentId = model.BusineesId_Fk;
                    signatory.CompanyId = null;
                }

                context.ClearanceSignatories.Add(signatory);
                if (context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;

        }
        public bool UpdateClearanceSignatory(BusinessHeadModel model)
        {
            bool iscompanywise = model.BusinessType == BusinessTypeEnum.Company ? true : false;
            var signatory = (from x in context.ClearanceSignatories
                             where x.ClearanceSignatoryId == model.Id
                             select x).FirstOrDefault();
            if (signatory != null)
            {
                signatory.IsActive = true;
                signatory.ModifiedBy = Common.GetUserId();
                signatory.ModifiedDate = DateTime.Now;
                signatory.EmployeeId = model.EmployeeId;
                if (iscompanywise)
                {

                    signatory.CompanyId = model.BusineesId_Fk;
                    signatory.DepartmentId = null;
                }
                else
                {
                    signatory.DepartmentId = model.BusineesId_Fk;
                    signatory.CompanyId = null;
                }
                if (context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<RequisitionApprovalVM> GetAllExitInterviewApprovalList(int id)
        {


            var requisitionList = (from mapping in context.ClearanceSignatoryMaps
                                   join signatory in context.ClearanceSignatories on mapping.ClearanceSignatoryId equals signatory.ClearanceSignatoryId
                                   join emp in context.Employees on signatory.EmployeeId equals emp.Id
                                   join des in context.Designations on emp.DesignationId equals des.DesignationId
                                   where mapping.ExitInterviewId == id
                                   select new RequisitionApprovalVM()
                                   {
                                       EmployeeId = emp.Id,
                                       EmployeeIdString = emp.EmployeeId,
                                       EmployeeName = emp.Name,
                                       DesignationName = des.Name,
                                       OrderBy = 1,
                                       Comment = mapping.Note,
                                       Status = (SignatoryStatusEnum)mapping.Status,
                                       StatusString = ((SignatoryStatusEnum)mapping.Status).ToString(),
                                       //StatusString = (from rsa in _context.RequisitionSignatoryApprovals
                                       //                where rsa.RequisitionId == a.RequisitionId
                                       //                && (a.OrderBy <= 1 || (rsa.OrderBy == (a.OrderBy - 1) && rsa.Status == (int)SignatoryStatusEnum.Approved))
                                       //                select rsa).FirstOrDefault() != null ? ((SignatoryStatusEnum)a.Status).ToString() : "...",
                                       ApprovedTime = mapping.ModifiedDate.HasValue ? mapping.ModifiedDate.Value.ToString() : "...."
                                   }).OrderBy(x => x.OrderBy).AsEnumerable();


            return requisitionList;
        }



        public bool RemoveClearanceSignatory(int id)
        {
            var existHead = context.ClearanceSignatories.FirstOrDefault(x => x.ClearanceSignatoryId == id && x.IsActive);
            if (existHead != null)
            {
                existHead.IsActive = false;
                existHead.ModifiedBy = Common.GetUserId();
                existHead.ModifiedDate = DateTime.Now;
            }
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }
        public IEnumerable<EmployeeClearanceVM> GetClearanceApprovalData(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status)
        {
            long userId = Common.GetIntUserId();

            var data = (from a in context.ClearanceSignatoryMaps
                        join b in context.ClearanceSignatories on a.ClearanceSignatoryId equals b.ClearanceSignatoryId
                        join exitinterview in context.ExitInterviews on a.ExitInterviewId equals exitinterview.Id
                        join emp in context.Employees on exitinterview.EmployeeId equals emp.EmployeeId
                        join com in context.Companies on emp.CompanyId equals com.CompanyId
                        join dep in context.Departments on emp.DepartmentId equals dep.DepartmentId
                        join des in context.Designations on emp.DesignationId equals des.DesignationId

                        // join emp in context.Employees e
                        where a.IsActive && b.EmployeeId == userId
                        && (status == null || a.Status == (int?)status)
                        && (fromDate == null || DbFunctions.TruncateTime(a.CreatedDate) >= fromDate)
                        && (toDate == null || DbFunctions.TruncateTime(a.CreatedDate) <= toDate)
                        select new EmployeeClearanceVM
                        {
                            Id = a.ClearanceSignatoryMapId,
                            ExitInterviewId = a.ExitInterviewId,
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.Name,
                            DepartmentName = dep.Name,
                            DesignationName = des.Name,
                            CompanyName = com.Name,
                            JoiningDate = emp.JoiningDate.Value,
                            ResignDate = exitinterview.ExpectedResignDate.HasValue ? exitinterview.ExpectedResignDate.Value : DateTime.MaxValue,
                            SignatoryStatus = (SignatoryStatusEnum)a.Status,
                        }).AsNoTracking().OrderBy(x => x.SignatoryStatus).ToList();

            return data;

        }

        public bool UpdateClearanceSignatoryApprovalStatus(long id, SignatoryStatusEnum status, string comment)
        {
            var exist = context.ClearanceSignatoryMaps.FirstOrDefault(x => x.ClearanceSignatoryMapId == id && x.IsActive);
            if (exist != null)
            {
                exist.Status = (int)status;
                exist.Note = comment;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                if (context.SaveChanges() > 0)
                {
                    return true;
                }

            }

            return false;
        }
        public bool SaveClearanceSignatoryMapping(int exitInterviewId)
        {
            ExitInterview exitInterView = context.ExitInterviews.Find(exitInterviewId);
            if (exitInterView == null)
            {
                return false;
            }
            var listOfSignatory = (from x in context.ClearanceSignatories
                                   where x.IsActive
                                   select x.ClearanceSignatoryId).ToArray();
            int result = 0;
            foreach (var item in listOfSignatory)
            {
                ClearanceSignatoryMap model = new ClearanceSignatoryMap()
                {
                    ClearanceSignatoryId = item,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    ExitInterviewId = exitInterviewId,
                    Status = (int)SignatoryStatusEnum.Pending
                };

                context.ClearanceSignatoryMaps.Add(model);
                result += context.SaveChanges();
            }
            exitInterView.Status = (int)ApprovalStatusEnum.Submitted;
            context.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            return false;

        }
        #endregion exitinterview
    }
}
