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
    public class KGREProjectsController : Controller
    {
        private ERPEntities db = new ERPEntities();
        //private readonly IDivisionService districtService;
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IKGREProjectService kGREProjectService = new KGREProjectService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        // GET: KGREProject
       
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText, int? companyId)
        {
            List<KGREProjectModel> kGREProjects = null;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            //companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0
            kGREProjects = kGREProjectService.GetKGREProjects(searchText ?? "").Where(x => x.CompanyId == companyId).ToList();
            //kGREProjects = kGREProjectService.GetKGREProjectByCompanyId(companyId);
            int Size_Of_Page = 100;
            int No_Of_Page = (Page_No ?? 1);
            return View(kGREProjects.ToPagedList(No_Of_Page, Size_Of_Page));
        }

       
        [HttpGet]
        public ActionResult CreateOrEdit(int id, int? companyId)
        {
            KGREProjectModel model = new KGREProjectModel();

            if (companyId > 0)
            {
                model.CompanyId = companyId;
                Session["CompanyId"] = companyId;
            }
            model = kGREProjectService.GetKGREProject(id);
            model.Companies = companyService.GetKGRECompnay();
            return View(model);
        }

        // POST: KGREProject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult CreateOrEdit(KGREProjectModel model)
        {
            string redirectPage = string.Empty;
            if (model.ProjectId <= 0)
            {
                KGREProject _KGREProject = db.KGREProjects.FirstOrDefault(x => x.ProjectName == model.ProjectName);
                if (_KGREProject != null)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit");
                }
                else
                {
                    kGREProjectService.SaveKGREProject(0, model);
                }
                redirectPage = "Index";
            }
            else
            {
                KGREProject _KGREProject = db.KGREProjects.FirstOrDefault(x => x.ProjectId == model.ProjectId);
                if (_KGREProject == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                kGREProjectService.SaveKGREProject(model.ProjectId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "Index";
            }

            return RedirectToAction(redirectPage, new { companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0 });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

       
        public IList<KGREProjectModel> GetKGCaseList(string searchText)
        {
            List<KGREProjectModel> operationModels = null;
            return operationModels.ToList();

        }

        #region // Plot Information
       
        [HttpGet]
        public ActionResult PlotList(int? Page_No, string searchText, int? KGREProject, int? companyId)
        {
            List<KGREProjectModel> kGREProjects = null;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if ((int)Session["CompanyId"] > 0)
            {
                companyId = (int)Session["CompanyId"];
            }
            int comId = (int)Session["CompanyId"];

            ViewBag.Projects = kGREProjectService.GetProjects(companyId);
            if (KGREProject > 0)
            {
                kGREProjects = kGREProjectService.GetKGREPlotList(searchText ?? "").Where(x => x.ProjectId == KGREProject).ToList();
            }
            else
            {
                kGREProjects = kGREProjectService.GetKGREPlotList(searchText ?? "").ToList();
            }
            int Size_Of_Page = 10000;
            int No_Of_Page = (Page_No ?? 1);
            return View(kGREProjects.ToPagedList(No_Of_Page, Size_Of_Page));
        }

       
        [HttpGet]
        public ActionResult CreateOrEditPlot(int id, int? companyId)
        {
            KGREProjectModel model = new KGREProjectModel();
            if (companyId > 0)
            {
                model.CompanyId = companyId;
                Session["CompanyId"] = companyId;
            }
            model = kGREProjectService.GetKGREPlot(id);
            model.KGREProjects = kGREProjectService.GetProjects(companyId);
            model.Companies = companyService.GetKGRECompnay();
            model.PStatus = dropDownItemService.GetDropDownItemSelectModels(62);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult CreateOrEditPlot(KGREProjectModel model)
        {
            string redirectPage = string.Empty;
            if (model.ProjectId <= 0)
            {
                KGREPlot _KGREPlot = db.KGREPlots.FirstOrDefault(x => x.PlotId == model.PlotId && x.ProjectId == model.ProjectId);

                if (_KGREPlot != null)
                {
                    TempData["errMessage"] = "Exists";
                    return RedirectToAction("CreateOrEdit");
                }
                else
                {
                    kGREProjectService.SaveKGREPlot(0, model);
                }
                redirectPage = "PlotList";
            }
            else
            {
                KGREPlot _KGREPlot = db.KGREPlots.FirstOrDefault(x => x.PlotNo == model.PlotNo && x.ProjectId == model.ProjectId);
                if (_KGREPlot == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                kGREProjectService.SaveKGREPlot(model.PlotId, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "PlotList";
            }

            return RedirectToAction(redirectPage, new { companyId = (int)Session["CompanyId"] > 0 ? (int)Session["CompanyId"] : 0 });
        }

        #endregion
    }
}
