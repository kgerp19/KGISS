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
    public class EmployeeLoanCollectionController : Controller
    {
        private ERPEntities db = new ERPEntities();
        //private readonly IDivisionService districtService;
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IEmployeeLoanCollectionService employeeLoanService = new EmployeeLoanCollectionService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        // GET: EmployeeLoanCollection

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            List<EmployeeLoanCollectionModel> landNLegal = null;
            landNLegal = employeeLoanService.GetEmployeeLoanCollections(searchText ?? "");
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(landNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult UpcomingCaseSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeLoanCollectionModel> operationModels = employeeLoanService.GetEmployeeLoanCollectionEvent();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = operationModels.Where(
                    x => x.Remarks.Contains(searchText)
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

            Session["FullName"] = string.Empty;
            Session["EmployeeId"] = string.Empty;
            Session["LoanID"] = 0;
            string empInfo = string.Empty;
            EmployeeLoan _EmployeeLoan = db.EmployeeLoans.Where(x => x.LoanID == id).FirstOrDefault();//From Employee Page
            EmployeeLoan _EmployeeLoan2 = null;
            if (!string.IsNullOrEmpty(searchText))
            {
                _EmployeeLoan2 = db.EmployeeLoans.Where(x => x.LoanID == id).FirstOrDefault(); ;// From Serach 
            }
            LoanCollection _LoanCollection = db.LoanCollections.Find(id);
            if (_EmployeeLoan2 != null)
            {
                Session["EmployeeId"] = _EmployeeLoan2.EmployeeId;
                Session["LoanID"] = _EmployeeLoan2.LoanID;
                //empInfo = GetEmployeeLoan(_EmployeeLoan2.LoanID);
            }

            if (_EmployeeLoan != null)
            {
                ViewBag.EmployeeId = _EmployeeLoan.EmployeeId;
                ViewBag.Id = _EmployeeLoan.LoanID;
                Session["EmployeeId"] = _EmployeeLoan.EmployeeId;
                // Session["FullName"] = _Employee.Name;

                empInfo = GetEmployeeLoan(_EmployeeLoan.LoanID);

            }
            if (_LoanCollection != null)
            {
                Session["LoanCollectionId"] = _LoanCollection.LoanCollectionId;
            }

            EmployeeLoanCollectionModel model = new EmployeeLoanCollectionModel();
            model = employeeLoanService.GetEmployeeLoanCollection(id);

            if (model == null)
            {
                model = new EmployeeLoanCollectionModel();
                //model.LoanTypes = dropDownItemService.GetDropDownItemSelectModels(62);
                if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()) && !string.IsNullOrEmpty(Session["LoanID"].ToString()))
                {
                    model.EmployeeId = Session["EmployeeId"].ToString();
                    model.LoanId = Convert.ToInt32(Session["LoanID"].ToString());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
                {
                    model.EmployeeId = Session["EmployeeId"].ToString();
                    model.LoanId = Convert.ToInt32(Session["LoanID"].ToString());
                }
                //model.LoanTypes = dropDownItemService.GetDropDownItemSelectModels(62);
            }
            //if (id > 0)
            //{
            //    Employee _Emp = db.Employees.FirstOrDefault(x => x.EmployeeId == _EmployeeLoan.EmployeeId);
            //    if (_Emp != null)
            //    {
            //        empInfo = GetEmployeeLoan(_Emp.Id);
            //    }

            //}
            //ViewBag.employeeInfo = empInfo;
            return View(model);

        }
        // POST: EmployeeLoanCollection/CreateOrEdit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(EmployeeLoanCollectionModel model)
        {
            string redirectPage = string.Empty;
            if (model.LoanCollectionId <= 0)
            {
                LoanCollection _EmployeeLoanExist = db.LoanCollections.Where(x => x.EmployeeId.Equals(model.EmployeeId)).FirstOrDefault();

                if (_EmployeeLoanExist != null)
                {
                    if (_EmployeeLoanExist.EmployeeId != null)
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
                    employeeLoanService.SaveEmployeeLoanCollection(0, model);
                    //Session["FullName"] = string.Empty;
                    //Session["EmployeeId"] = string.Empty;
                    //Session["Id"] = 0;
                }
                redirectPage = "Index";
            }
            else
            {
                LoanCollection LoanCollection = db.LoanCollections.FirstOrDefault(x => x.LoanCollectionId == model.LoanCollectionId);
                if (LoanCollection == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                //model.EmployeeId = Session["EmployeeId"].ToString();
                if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
                {
                    model.EmployeeId = Session["EmployeeId"].ToString();
                }
                employeeLoanService.SaveEmployeeLoanCollection(model.LoanCollectionId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.employeeInfo = GetEmployeeLoan(model.LoanCollectionId);
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

        public string GetEmployeeLoan(long id)
        {
            string htmlStr = "<table class='spacing - table' ";
            DataTable dt = new DataTable();
            dt = GetEmployeeLoanByCaseId(id);
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

        private DataTable GetEmployeeLoanByCaseId(long Id)
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

        
        public IList<EmployeeLoanModel> GetKGCaseList(string searchText)
        {
            List<EmployeeLoanModel> operationModels = null;
            return operationModels.ToList();

        }
    }
}
