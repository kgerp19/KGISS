using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class EmployeeOperationController : Controller
    {
        private ERPEntities db = new ERPEntities();
        //private readonly IDivisionService districtService;
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IEmployeeOperationService employeeOperationService = new EmployeeOperationService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        // GET: EmployeeOperation

       
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            List<EmployeeOperationModel> landNLegal = null;
            landNLegal = employeeOperationService.GetEmployeeOperations(searchText ?? "");
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(landNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

       
        [HttpGet]
        public ActionResult UpcomingCaseSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeOperationModel> operationModels = employeeOperationService.GetEmployeeOperationEvent();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = operationModels.Where(
                    x => x.EmployeeOperationType.Contains(searchText)
                || x.EmployeeId.Contains(searchText));
                return View(legaldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(operationModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

       
        [HttpGet]
        public ActionResult Previous7DaysCaseSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeOperationModel> operationModels = employeeOperationService.GetPrevious7DaysOperationSchedule();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = operationModels.Where(
                    x => x.EmployeeOperationType.Contains(searchText)
                || x.EmployeeId.Contains(searchText));
                return View(legaldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(operationModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }


       
        [HttpGet]
        public ActionResult CreateOrEdit(string employeeId, int id, string searchText)
        {
            if (System.Web.HttpContext.Current.User.Identity.Name == "KG3068"
                || System.Web.HttpContext.Current.User.Identity.Name == "KG3070"
                  || System.Web.HttpContext.Current.User.Identity.Name == "KG3071"
                  || System.Web.HttpContext.Current.User.Identity.Name == "KG3069"
                  || System.Web.HttpContext.Current.User.Identity.Name == "KG1088"
                  || System.Web.HttpContext.Current.User.Identity.Name == "KG3055")
            {

                Session["FullName"] = string.Empty;
                Session["EmployeeId"] = string.Empty;
                Session["Id"] = 0;
                string empInfo = string.Empty;
                Employee _Employee = db.Employees.Where(x => x.Active && x.EmployeeId.Contains(employeeId)).FirstOrDefault();//From Employee Page
                Employee _Employee2 = null;
                if (!string.IsNullOrEmpty(searchText))
                {
                    _Employee2 = db.Employees.Where(x => x.Active && x.EmployeeId.Contains(searchText)).FirstOrDefault();// From Serach 
                }
                //Employee _Employee = db.Employees.Find(id);
                EmployeeOperation _EmployeeOperation = db.EmployeeOperations.Find(id);
                if (_Employee2 != null)
                {
                    Session["FullName"] = _Employee2.Name;
                    Session["EmployeeId"] = _Employee2.EmployeeId;
                    Session["Id"] = _Employee2.Id;
                    empInfo = GetEmployeeOperation(_Employee2.Id);
                }

                if (_Employee != null)
                {
                    //Session["EmployeeId"] = string.Empty;
                    //Session["FullName"] = string.Empty;
                    ViewBag.FullName = _Employee.Name;
                    ViewBag.EmployeeId = _Employee.EmployeeId;
                    ViewBag.Id = _Employee.Id;
                    Session["EmployeeId"] = _Employee.EmployeeId;
                    Session["FullName"] = _Employee.Name;

                    empInfo = GetEmployeeOperation(_Employee.Id);

                }
                if (_EmployeeOperation != null)
                {
                    Session["OperationId"] = _EmployeeOperation.OperationId;
                }

                EmployeeOperationModel model = new EmployeeOperationModel();
                model = employeeOperationService.GetEmployeeOperation(id);

                if (model == null)
                {
                    model = new EmployeeOperationModel();
                    model.EmployeeOperationTypes = dropDownItemService.GetDropDownItemSelectModels(37);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
                    {
                        model.EmployeeId = Session["EmployeeId"].ToString();
                        model.Name = Session["FullName"].ToString();
                    }
                    model.EmployeeOperationTypes = dropDownItemService.GetDropDownItemSelectModels(37);
                }
                if (id > 0)
                {
                    Employee _Emp = db.Employees.FirstOrDefault(x => x.EmployeeId == _EmployeeOperation.EmployeeId);
                    if (_Emp != null)
                    {
                        empInfo = GetEmployeeOperation(_Emp.Id);
                    }

                }
                ViewBag.employeeInfo = empInfo;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Employee", new { @companyId = 26 });
            }
        }
        // POST: EmployeeOperation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult CreateOrEdit(EmployeeOperationModel model)
        {
            string redirectPage = string.Empty;
            if (model.OperationId <= 0)
            {
                EmployeeOperation _EmployeeOperationExist = db.EmployeeOperations.Where(x => x.EmployeeId.Equals(model.EmployeeId)).FirstOrDefault();

                if (_EmployeeOperationExist != null)
                {
                    if (_EmployeeOperationExist.EmployeeId != null)
                    {
                        TempData["errMessage"] = "Exists";
                        return RedirectToAction("CreateOrEdit");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
                    {
                        model.EmployeeId = Session["EmployeeId"].ToString();
                    }
                    employeeOperationService.SaveEmployeeOperation(0, model);
                    //Session["FullName"] = string.Empty;
                    //Session["EmployeeId"] = string.Empty;
                    //Session["Id"] = 0;
                }
                redirectPage = "Index";
            }
            else
            {
                EmployeeOperation employeeOperation = db.EmployeeOperations.FirstOrDefault(x => x.OperationId == model.OperationId);
                if (employeeOperation == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                //model.EmployeeId = Session["EmployeeId"].ToString();
                if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
                {
                    model.EmployeeId = Session["EmployeeId"].ToString();
                }
                employeeOperationService.SaveEmployeeOperation(model.OperationId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.employeeInfo = GetEmployeeOperation(model.OperationId);
                //Session["FullName"] = string.Empty;
                // Session["EmployeeId"] = string.Empty;
                Session["Id"] = 0;
            }

            return RedirectToAction(redirectPage);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CaseAttachmentDataBind(string employeeId)
        {

        }

        public string GetEmployeeOperation(long id)
        {
            string htmlStr = "<table class='spacing - table' ";
            DataTable dt = new DataTable();
            dt = GetEmployeeOperationByCaseId(id);
            string style1 = "\"align:right;\"";
            string style12 = "\"background-color: #E9EDBE; vertical-align: middle;\"";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["EmployeeId"].ToString();
                dt.Rows[i]["Name"].ToString();
                dt.Rows[i]["Department"].ToString();
                dt.Rows[i]["Designation"].ToString();
                dt.Rows[i]["JoiningDate"].ToString();
                dt.Rows[i]["MobileNo"].ToString();
                dt.Rows[i]["PermanentAddress"].ToString();
                DateTime d1 = DateTime.Now;
                DateTime dtstart = Convert.ToDateTime(dt.Rows[i]["JoiningDate"]);
                string strDateTime2 = dtstart.ToString("dd/MM/yyyy");


                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Employee Id: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["EmployeeId"].ToString() + "</td>";
                htmlStr += " <td align=" + "right" + "> <b> <span>Employee Name: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["Name"].ToString() + "</td></tr>";
                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Department: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["Department"].ToString() + "</td>";
                htmlStr += "<td align=" + "right" + "> <b> <span>Designation: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["Designation"].ToString() + "</td></tr>";
                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Joining Date: </b></span></td><td style=" + style12 + ">" + strDateTime2 + "</td>";
                htmlStr += "<td align=" + "right" + "> <b> <span>Mobile No: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["MobileNo"].ToString() + "</td></tr>";
                htmlStr += "<tr colspan='2'><td align='right' style =" + style1 + "> <b> <span>PermanentAddress: </b></span></td><td style=" + style12 + " colspan=" + "3" + ">" + dt.Rows[i]["PermanentAddress"].ToString() + "</td></tr>";
            }

            return htmlStr += "</table>";
        }

        private DataTable GetEmployeeOperationByCaseId(long Id)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("[GetEmployeeById]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }
       
        [HttpGet]
        public ActionResult ExportKGCaseToExcel(string searchText)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(searchText))
            {
                dt = GetCaseList(searchText);
            }
            else
            {
                dt = GetCaseList();
            }


            gv.DataSource = dt;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= KG_CaseList.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        public DataTable GetCaseList(string serachText)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetCaseList where ( [CaseNo] like  '%" + serachText + "%' or  [CaseType] like  '%" + serachText + "%' or [ResponsibleLayer] like  '%" + serachText + "%' or [CaseStatus] like  '%" + serachText + "%')"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable GetCaseList()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetCaseList"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

       
        public IList<EmployeeOperationModel> GetKGCaseList(string searchText)
        {
            List<EmployeeOperationModel> operationModels = null;
            return operationModels.ToList();

        }
    }
}
