using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class SmsController : BaseController
    {
        private readonly ISmsServices _smsServices;
        ICompanyService companyService = new CompanyService(new ERPEntities());
        IDepartmentService departmentService = new DepartmentService();
        IEmployeeService employeeService = new EmployeeService(new ERPEntities());

        public SmsController(ISmsServices smsServices)
        {
            _smsServices = smsServices;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, int? type, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            fromDate = fromDate == null ? DateTime.Now.AddMonths(-1) : fromDate.Value.AddDays(-1);
            toDate = toDate == null ? DateTime.Now : toDate.Value.AddDays(1);
            type = type == null ? 0 : type;
            SmsListVm dataList = new SmsListVm();
            dataList.CompanyId = companyId;
            dataList = await _smsServices.GetSmsCompanyWiseList(companyId, type.Value, fromDate, toDate);
            return View(dataList);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(SmsListVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, type = model.type, fromDate = model.FromDate, toDate = model.ToDate });
        }

        public async Task<ActionResult> Create(int companyId)
        {
            SendSMSViewModel Sms = new SendSMSViewModel();

            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;

            }
            Sms = await InitializeModel(companyId);

            //vm.SmsTypeList = await _smsServices.GetSmsTypeList();
            return View(Sms);

        }


        private async Task<SendSMSViewModel> InitializeModel(int companyId)
        {
            SendSMSViewModel model = new SendSMSViewModel();
            model.MessageDate = DateTime.Now;
            model.CompanyList = await Task.Run(() => companyService.GetCompanySelectModels());
            model.CompanyId = companyId;
            model.SelectedCompanyId = companyId;
            model.DepartmentList = await Task.Run(() => departmentService.GetDepartmentSelectModels());
            model.AllEmployeeList = await Task.Run(() => employeeService.GetEmployeesForSmsByCompanyId(companyId));
            model.SmsTypeList = await _smsServices.GetSmsTypeList();
            model.SmsTypeList=model.SmsTypeList.OrderBy(e => e.Name).ToList();
            return model;
        }
        public string convertBanglatoUnicode(string banglaText)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in banglaText)
            {
                sb.AppendFormat("{1:x4}", c, (int)c);
            }
            string unicode = sb.ToString().ToUpper();
            return unicode;
        }


        public async Task<ActionResult> SendSMS([Bind(Exclude = "Attachment")] SendSMSViewModel viewModel)
        {
            List<ErpSMS> lstSms = new List<ErpSMS>();
            bool result = true;
            string errMsg = string.Empty;
            string SMSSenderAndText = string.Empty;
            errMsg = "Message has been sent successfully added to the sending queue";
            try
            {
                if (viewModel.MessageOption == "S" && viewModel.SelectedEmployee.Count() > 0)
                {
                    lstSms = viewModel.SelectedEmployee.Where(e => e.Contains("-") == false || e.Contains("/") || e.Contains(","))
                        .Select(o => new ErpSMS
                        {
                            CompanyId = viewModel.CompanyId.HasValue ? viewModel.CompanyId.Value : 0,
                            Date = viewModel.MessageDate,
                            Message = viewModel.Message,
                            PhoneNo = o,
                            SmsType=viewModel.SmsTypeId,
                            Remarks = "",
                            RowTime = DateTime.Now,
                            Subject = String.IsNullOrEmpty(viewModel.Subject) ? "No Subject" : viewModel.Subject,
                            Status = (int)EnumSmSStatus.Pending,
                            TryCount = 0
                        }).ToList();
                }
                else if (viewModel.MessageOption == "A")
                {

                    var empList = employeeService.GetEmployeesForSmsByCompanyId(0);
                    lstSms = empList.Where(e => e.Value.ToString().Contains("-") == false || e.Value.ToString().Contains("/") || e.Value.ToString().Contains(","))
                        .Select(o => new ErpSMS
                        {
                            CompanyId = viewModel.CompanyId.HasValue ? viewModel.CompanyId.Value : 0,
                            Date = viewModel.MessageDate,
                            Message = viewModel.Message,
                            SmsType = viewModel.SmsTypeId,
                            PhoneNo = o.Value.ToString(),
                            Remarks = "",
                            RowTime = DateTime.Now,
                            Subject = String.IsNullOrEmpty(viewModel.Subject) ? "No Subject" : viewModel.Subject,
                            Status = (int)EnumSmSStatus.Pending,
                            TryCount = 0
                        }).ToList();


                }
                else if (viewModel.MessageOption == "Z")
                {
                    int CompId = viewModel.SelectedCompanyId.HasValue == false ? 0 : viewModel.SelectedCompanyId.Value;
                    int DepartmentId = viewModel.DepartmentId.HasValue == false ? 0 : viewModel.DepartmentId.Value;
                    var empList = employeeService.GetEmployeesForSmsByCompanyId(CompId, DepartmentId);
                    lstSms = empList.Where(e => e.Value.ToString().Contains("-") == false || e.Value.ToString().Contains("/") || e.Value.ToString().Contains(","))
                        .Select(o => new ErpSMS
                        {
                            CompanyId = viewModel.CompanyId.HasValue ? viewModel.CompanyId.Value : 0,
                            Date = viewModel.MessageDate,
                            Message = viewModel.Message,
                            SmsType = viewModel.SmsTypeId,
                            PhoneNo = o.Value.ToString(),
                            Remarks = "",
                            RowTime = DateTime.Now,
                            Subject = String.IsNullOrEmpty(viewModel.Subject) ? "No Subject" : viewModel.Subject,
                            Status = (int)EnumSmSStatus.Pending,
                            TryCount = 0
                        }).ToList();
                }
                else if (viewModel.MessageOption == "D")
                {
                    if (String.IsNullOrEmpty(viewModel.PhoneNo.Trim()) || viewModel.PhoneNo.Trim().Length != 11)
                    {
                        errMsg = "phone number must be 11 digit";
                    }
                    else
                    {
                        lstSms.Add(new ErpSMS
                        {
                            CompanyId = viewModel.CompanyId.HasValue ? viewModel.CompanyId.Value : 0,
                            Date = viewModel.MessageDate,
                            Message = viewModel.Message,
                            SmsType = viewModel.SmsTypeId,
                            PhoneNo = viewModel.PhoneNo,
                            Remarks = "",
                            RowTime = DateTime.Now,
                            Subject = String.IsNullOrEmpty(viewModel.Subject) ? "No Subject" : viewModel.Subject,
                            Status = (int)EnumSmSStatus.Pending,
                            TryCount = 0
                        });
                    }

                }
                else
                {
                    //Exnter Member
                    HttpPostedFileBase file = Request.Files["attachment"];
                    // var attachment = Request.Files["attachment"];
                    if (Request.Files["Attachment"].ContentLength > 0)
                    {
                        OleDbConnection conn = new OleDbConnection();
                        OleDbCommand cmd = new OleDbCommand();
                        OleDbDataAdapter da = new OleDbDataAdapter();
                        DataSet ds = new DataSet();
                        string connString = "";
                        string strFileName = Guid.NewGuid().ToString();
                        string strFileType = Path.GetExtension(file.FileName).ToString().ToLower();
                        var fileName = strFileName  + strFileType;
                        var path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);
                        if (strFileType == ".xls" || strFileType == ".xlsx")
                        {
                            file.SaveAs(path);
                            if (strFileType.Trim() == ".xls")
                            {
                                connString = string.Format(ConfigurationManager.ConnectionStrings["Excel03ConString"].ToString(), path);//"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            else 
                            {
                                connString = string.Format(ConfigurationManager.ConnectionStrings["Excel07ConString"].ToString(), path);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            connString = string.Format(connString, path);
                            OleDbConnection connExcel = new OleDbConnection(connString);
                            OleDbCommand cmdExcel = new OleDbCommand();
                            OleDbDataAdapter oda = new OleDbDataAdapter();
                            DataTable dt = new DataTable();
                            cmdExcel.Connection = connExcel;
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
                            oda.SelectCommand = cmdExcel;
                            oda.Fill(ds);
                            dt = ds.Tables[0];
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["Mobile No"].ToString().Trim().Length == 11)
                                {
                                    var x = dr["Mobile No"].ToString().Trim();
                                    ErpSMS sms = new ErpSMS
                                    {
                                        CompanyId = viewModel.CompanyId.HasValue ? viewModel.CompanyId.Value : 0,
                                        Date = viewModel.MessageDate,
                                        Message = viewModel.Message,
                                        SmsType = viewModel.SmsTypeId,
                                        PhoneNo = dr["Mobile No"].ToString().Trim(),
                                        Remarks = "",
                                        RowTime = DateTime.Now,
                                        Subject = String.IsNullOrEmpty(viewModel.Subject) ? "No Subject" : viewModel.Subject,
                                        Status = (int)EnumSmSStatus.Pending,
                                        TryCount = 0
                                    };
                                    lstSms.Add(sms);
                                }
                            }
                        }
                        else
                        {
                            errMsg = "Invalid file type";
                        }
                    }
                }
                if (lstSms.Count > 0)
                {
                    var IsSend = await _smsServices.SendSms(lstSms);
                    if (IsSend == false)
                    {
                        errMsg = "Failed.Please check & Try again";
                    }
                }



            }
            catch (Exception ex)
            {
                result = false;
                errMsg = "Message sending failed. Please try again";
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);

        }
    }
}