using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.Utility.Util;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class AssetController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IAssetService assetService;
        private readonly IDistrictService districtService;
        private readonly IUpazilaService upazilaService;
        IDepartmentService departmentService = new DepartmentService();

        private ERPEntities db = new ERPEntities();
        string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public AssetController(IAssetService assetService,
            IDistrictService districtService,
            IUpazilaService upazilaService)
        {
            this.assetService = assetService;
            this.districtService = districtService;
            this.upazilaService = upazilaService;
        }

        
        public ActionResult AssetDashboard(int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            int comId = (int)Session["CompanyId"];
            DataTable dt = GetAllAsset();
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }
            return View();
        }

        public DataTable GetAllAsset()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Asset_Dashboard", con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
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
                logger.Error(ex);
                return null;
            }
        }


        
        [HttpGet]
        public ActionResult Index(int? Page_No, string type, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            var assetList = assetService.Index();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);

            return View(assetList.ToPagedList(No_Of_Page, Size_Of_Page));

        }
        
        [HttpGet]
        public async Task<ActionResult> OfficeAssetList(int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            AssetModelVm model = new AssetModelVm();
           
            model = await assetService.GetOfficeAssets(companyId);
            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> OfficeAssetList(AssetModelVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            return RedirectToAction(nameof(OfficeAssetList), new { companyId = model.CompanyId });
        }
        //
        //[HttpGet]
        //public ActionResult OfficeAssetList(int? Page_No, string searchText, int? CompanyId, int? AssetLocation)
        //{
        //    searchText = searchText ?? "";
        //    if (GetCompanyId() > 0)
        //    {
        //        Session["CompanyId"] = GetCompanyId();
        //    }

        //    ViewBag.Company = assetService.Company();
        //    ViewBag.AssetLocation = assetService.AssetLocation();


        //    List<OfficeAssetModel> _OfficeAssetList = null;
        //    IQueryable<AssetTrackingFinal> _assetTrackingFinal = null;
        //    _assetTrackingFinal = db.AssetTrackingFinals;


        //    var assetList = assetService.FinalAssetList();
        //    if (!string.IsNullOrWhiteSpace(searchText))
        //    {
        //        assetList.Where(x => x.Manufacturer.Contains(searchText) || x.DepartmentName.Contains(searchText) || x.CompanyName.Contains(searchText) || x.AssetsName.Contains(searchText) || x.DepartmentName.Contains(searchText) || x.AssetCategory.Contains(searchText) || x.AssetLocation.Contains(searchText)).ToList();
        //        //_assetTrackingFinal = _assetTrackingFinal.Where(m => m.CompanyName.Contains(searchText) || m.AssetsName.Contains(searchText) || m.AssetLocation.Contains(searchText) || m.UserName.Contains(searchText));
        //    }

        //    _OfficeAssetList = ObjectConverter<AssetTrackingFinal, OfficeAssetModel>.ConvertList(_assetTrackingFinal.ToList()).ToList();

        //    //foreach (AssetTrackingFinalModel data in _OfficeAssetList)
        //    //{
        //    //    assetService.SaveOrEditAsset(data);
        //    //}

        //    int Size_Of_Page = 10;
        //    int No_Of_Page = (Page_No ?? 1);
        //    return View(assetList.ToPagedList(No_Of_Page, Size_Of_Page));
        //}

        public ActionResult CreateOrEditAsset(int id)
        {
            AssetViewModel vm = new AssetViewModel();
            vm._asset = assetService.GetFinalAsset(id);
            vm.Company = assetService.Company();
            vm.AssetLocation = assetService.AssetLocation();
            vm.Departments = departmentService.GetDepartmentSelectModels();
            vm.AssetSubLocation = assetService.AssetSubLocationByLocationId(vm._asset.AssetLocationId);
            vm.AssetCategory = assetService.AssetCategory();
            vm.AssetType = assetService.AssetTypeByCategoryId(vm._asset.AssetCategoryId);
            vm.AssetStatus = assetService.AssetStatus();
            vm.Colour = assetService.Colour();
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEditAsset(AssetViewModel vm)
        {
            OfficeAssetModel model = vm._asset;
            string redirectPage = string.Empty;
            if (model.OID <= 0)
            {
                try
                {
                    assetService.SaveOrEditAsset(model);
                    redirectPage = "OfficeAssetList";
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                try
                {
                    assetService.SaveOrEditAsset(model);
                    TempData["DataUpdate"] = "Data Save Successfully!";
                    redirectPage = "CreateOrEditAsset";
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            return RedirectToAction(redirectPage);
        }

        public ActionResult AssetDetails(int id)
        {
            var asset = assetService.AssetDetails(id);
            return View(asset);
        }

        
        [HttpGet]
        public ActionResult ExportLandListToExcel(string DeedNo, string MoujaName,
            string SellerName, int? SelectedCompanyId,
            int? LandReceiverId, int? DistrictId, int? UpazilaId)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(DeedNo) || !string.IsNullOrEmpty(MoujaName) || !string.IsNullOrEmpty(SellerName)
                || SelectedCompanyId !=null || LandReceiverId !=null
                || DistrictId != null
                || UpazilaId !=null
               )
            {
                dt = GetFilterCaseList(DeedNo, MoujaName, SellerName, SelectedCompanyId, LandReceiverId, DistrictId,
               UpazilaId);
              
            }
            else
            {
                dt = GetFilterCaseList("", "", "", null, null, null, null);
            }
            
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    string[] selectedColumns = new[]
                    { "CompanyName", "DistrictName", "UpazillaName","DeedNo","ReceiverNameEn" };
                    DataTable dt1 = new DataView(dt).ToTable(false, selectedColumns);

                    gv.DataSource = dt1;
                    gv.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename= KG_LandList.xls");
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
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public DataTable GetFilterCaseList(string DeedNo, string MoujaName,
            string SellerName, int? SelectedCompanyId,
            int? LandReceiverId, int? DistrictId, int? UpazilaId)
        {
            try
            {
                DataTable dt = new DataTable();

                List<AssetModel2> _assetList = (from t1 in db.Assets
                                                join t2 in db.Companies on t1.CompanyId equals t2.CompanyId
                                                join t3 in db.Districts on t1.DistrictId equals t3.DistrictId
                                                join t4 in db.Upazilas on t1.UpazillaId equals t4.UpazilaId
                                                select new AssetModel2
                                                {
                                                    AssetId = t1.AssetId,
                                                    CompanyId = t1.CompanyId,
                                                    CompanyName = t2.Name,
                                                    ReceiverNameEn = t1.ReceiverNameEn,
                                                    DistrictId = t3.DistrictId,
                                                    DeedNo = t1.DeedNo,
                                                    DistrictName = t3.Name,
                                                    UpazillaName = t4.Name,
                                                    SellerName = t1.DonerNameEn,
                                                    AmountOfLandPurchasedEn = t1.AmountOfLandPurchasedEn

                                                }).ToList();

                if (!string.IsNullOrWhiteSpace(DeedNo))
                    _assetList = _assetList.Where(m => m.DeedNo == DeedNo).ToList();


                if (LandReceiverId != null)
                    _assetList = _assetList.Where(m => m.LandReceiverId == LandReceiverId).ToList();

                if (!string.IsNullOrWhiteSpace(MoujaName))
                {
                    _assetList = _assetList.Where(m => m.Mouja.ToUpper().Contains(MoujaName.ToUpper())).ToList();
                }

                if (!string.IsNullOrWhiteSpace(SellerName))
                {
                    _assetList = _assetList.Where(m => m.DonerNameEn.ToUpper().Contains(SellerName.ToUpper())).ToList();
                }

                if (SelectedCompanyId != null)
                    _assetList = _assetList.Where(m => m.CompanyId == SelectedCompanyId).ToList();

                if (DistrictId != null)
                    _assetList = _assetList.Where(m => m.DistrictId == DistrictId).ToList();

                if (UpazilaId != null)
                    _assetList = _assetList.Where(m => m.UpazillaId == UpazilaId).ToList();
                

                if (_assetList != null)
                {
                    dt = CreateDataTable(_assetList);
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        private DataTable CreateDataTable(IList<AssetModel2> item)
        {
           
            Type type = typeof(AssetModel2);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();

            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (AssetModel2 entity in item)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        
        [HttpPost]
        public async Task<ActionResult> LandIndex3(AssetModel2 model)
        {
            return RedirectToAction(nameof(LandIndex3), new {
                companyId= model.CompanyId,
                landReceiverId = model.SelectedLandReceiverId,
                districtId= model.SelectedDistrictId,
                upazilaId= model.SelectedUpzillaId,
                selectedCompanyId = model.SelectedCompanyId
            });
        }
            // GET : Land List
        
        
        [HttpGet]
        public async Task<ActionResult> LandIndex3(int companyId,int? landReceiverId, int? districtId, int? upazilaId, int? selectedCompanyId)
        { 
            AssetModel2 model = new AssetModel2();
            model = await assetService.GetLandList(companyId,landReceiverId,districtId,upazilaId,selectedCompanyId);

            model.SelectedCompanyId = selectedCompanyId;
            model.SelectedDistrictId = districtId;
            model.SelectedUpzillaId = upazilaId;
            model.SelectedLandReceiverId = landReceiverId;
            model.CompanyId=companyId;

            return View(model);
        }

        
        [HttpGet]
        public  async  Task<ActionResult> LandIndex2()
        {
            VMAsset vMAsset = new VMAsset();
            int companyId = (int)Session["CompanyId"];
            vMAsset = await assetService.LandList(companyId);
            return View(vMAsset);
        }

        
        [HttpGet]
        public ActionResult LandIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<AssetModel> assetList;
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            int companyId = (int)Session["CompanyId"];
            if (companyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                assetList = assetService.LandIndex(searchText).Where(c => c.CompanyId == companyId).ToList();
            }
            else
            {
                assetList = assetService.LandIndex(searchText);
            }
            //List<AssetModel> assetList = assetService.LandIndex().Where(x=>(x.Company.Name.ToLower().Contains(searchText.ToLower())) || (x.DeedNo.ToLower().Contains(searchText.ToLower()))).ToList();

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);


            return View(assetList.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult KGLandList(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<AssetModel> assetList;
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            int companyId = (int)Session["CompanyId"];
            if (companyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                assetList = assetService.KGLandList(searchText).Where(c => c.CompanyId == companyId).ToList();
            }
            else
            {
                assetList = assetService.KGLandList(searchText);
            }
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(assetList.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        public ActionResult CreateOrEditKGAsset(int id,int CompanyId)
        {
            AssetViewModel vm = new AssetViewModel();
            vm.Asset = assetService.GetKGAsset(id);
            vm.Company = assetService.Company();
            vm.AssetLocation = assetService.AssetLocation();
            vm.AssetSubLocation = assetService.AssetSubLocation(vm.Asset.AssetLocationId);
            vm.AssetCategory = assetService.AssetCategory();
            vm.AssetType = assetService.AssetType(vm.Asset.AssetCategoryId);
            vm.Project = assetService.Project();
            vm.DisputedList = assetService.DisputedList();
            vm.LandReceiver = assetService.LandReceiver();
            vm.LandUser = assetService.LandUser();
            vm.Asset.AssetLocationId = 3;
            vm.Asset.AssetSubLocationId = 3;
            vm.Asset.AssetCategoryId = 3;
            vm.Asset.AssetTypeId = 3;
            vm.Asset.Quantity = 1;
            vm.Colour = assetService.Colour();
            vm.Districts = districtService.GetDistrictSelectModels();
            vm.Upazilas = upazilaService.GetUpazilaSelectModelsByDistrict(vm.Asset.DistrictId ?? 0);
            vm.CompanyId = CompanyId;
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEditKGAsset(AssetViewModel vm)
        {
            AssetModel model = vm.Asset;
            bool result = assetService.SaveOrEditKGLandAsset(model);

            if (result == true && model.AssetId > 0)
            {
                int assetId = model.AssetId;
                string deedNo = "";
                var assetData = db.Assets.Where(x => x.AssetId == assetId).FirstOrDefault();
                if (assetData != null && !string.IsNullOrEmpty(assetData.DeedNo))
                {
                    deedNo = assetData.DeedNo;
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            FileAttachment fileDetail = new FileAttachment()
                            {
                                AttachFileName = deedNo + "_" + fileName,
                                AssetId = assetData.AssetId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CompanyId = 30,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                ModifiedDate = DateTime.Now
                            };

                            string folder = Server.MapPath(string.Format("~/{0}/{1}/{2}/{3}/", "KGFiles", "Aseet", "Land", deedNo));
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                                var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/{2}/{3}/", "KGFiles", "Aseet", "Land", deedNo)), deedNo + "_" + fileName);
                                file.SaveAs(path1);
                            }
                            else
                            {
                                //var path1 = Path.Combine(Server.MapPath(folder + fileName));
                                var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/{2}/{3}/", "KGFiles", "Aseet", "Land", deedNo)), deedNo + "_" + fileName);
                                file.SaveAs(path1);
                            }

                            try
                            {
                                db.FileAttachments.Add(fileDetail);
                                db.SaveChanges();
                            }
                            catch (DbEntityValidationException dbEx)
                            {
                                foreach (var validationErrors in dbEx.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                    }
                                }
                            }
                        }
                    }
                }

            }

            if (vm.Asset.AssetCategoryId == 3)
            {
                return RedirectToAction("LandIndex3", new { companyId = vm.CompanyId });

            }
            else
            {
                return RedirectToAction("LandIndex3", new { companyId = vm.CompanyId });

            }
        }

        public FileResult Download(String fileName, String deedNo)
        {                           
                return File(Path.Combine(Server.MapPath("~/KGFiles/Aseet/Land/" + deedNo), fileName), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                int guid = Convert.ToInt32(id);
                var path1 = string.Empty;
                FileAttachment fileDetail = db.FileAttachments.Find(guid);
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }
                else
                {
                    Asset aseet = db.Assets.Find(fileDetail.AssetId);
                    path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/{2}/{3}/", "KGFiles", "Aseet", "Land", aseet.DeedNo)), fileDetail.AttachFileName);
                }

                //Remove from database
                db.FileAttachments.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system                 
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult LandDetails(int id)
        {
            var land = assetService.LandDetails(id);
            return View(land);
        }

        public ActionResult CreateOrEdit(int id)
        {
            AssetViewModel vm = new AssetViewModel();
            vm.Asset = assetService.GetAsset(id);
            vm.Company = assetService.Company();
            vm.AssetLocation = assetService.AssetLocation();
            vm.AssetSubLocation = assetService.AssetSubLocation(vm.Asset.AssetLocationId);
            vm.AssetCategory = assetService.AssetCategory();
            vm.AssetType = assetService.AssetType(vm.Asset.AssetCategoryId);
            vm.AssetStatus = assetService.AssetStatus();
            vm.Colour = assetService.Colour();
            return View(vm);
        }

        public ActionResult CreateOrEditLand(int id)
        {
            AssetViewModel vm = new AssetViewModel();
            vm.Asset = assetService.GetAsset(id);
            vm.Company = assetService.Company();
            vm.AssetLocation = assetService.AssetLocation();
            vm.AssetSubLocation = assetService.AssetSubLocation(vm.Asset.AssetLocationId);
            vm.AssetCategory = assetService.AssetCategory();
            vm.AssetType = assetService.AssetType(vm.Asset.AssetCategoryId);
            vm.Project = assetService.Project();
            vm.DisputedList = assetService.DisputedList();
            vm.LandReceiver = assetService.LandReceiver();
            vm.LandUser = assetService.LandUser();
            vm.Asset.AssetLocationId = 3;
            vm.Asset.AssetSubLocationId = 3;
            vm.Asset.AssetCategoryId = 3;
            vm.Asset.AssetTypeId = 3;
            vm.Asset.Quantity = 1;
            vm.Colour = assetService.Colour();
            vm.Districts = districtService.GetDistrictSelectModels();
            vm.Upazilas = upazilaService.GetUpazilaSelectModelsByDistrict(vm.Asset.DistrictId ?? 0);
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEditLand(AssetViewModel vm)
        {
          
            AssetModel model = vm.Asset;
            try
            {
                bool result = assetService.SaveOrEdit(model);

                #region File Upload
                if (result == true && model.AssetId > 0)
                {
                    Int64 assetId = model.AssetId;
                    string assetID = "";
                    var assetData = db.Assets.Where(x => x.AssetId == assetId).FirstOrDefault();
                    if (assetData != null)
                    {
                        assetID = assetData.AssetId.ToString();
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                FileAttachment fileDetail = new FileAttachment()
                                {
                                    AttachFileName = assetData.AssetId + "_" + fileName,
                                    CompanyId = 227,
                                    CaseId = assetId,
                                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                    CreatedDate = DateTime.Now,
                                    ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                    ModifiedDate = DateTime.Now
                                };

                                string folder = Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "Asset"));
                                if (!Directory.Exists(folder))
                                {
                                    Directory.CreateDirectory(folder);
                                    var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "Asset")), assetID + "_" + fileName);
                                    file.SaveAs(path1);
                                }
                                else
                                {
                                    var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "Asset")), assetID + "_" + fileName);
                                    file.SaveAs(path1);
                                }

                                db.Entry(fileDetail).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                #endregion

                TempData["DataUpdate"] = "Data Save Successfully!";
               
                ViewBag.cHistory = GetAssetHistory(model.AssetId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (vm.Asset.AssetCategoryId == 3)
            {
                return RedirectToAction("LandIndex3");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public string GetAssetHistory(long id)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            DataTable dt = new DataTable();
            dt = GetAssetHistoryById(id);
            string style = "\"border-bottom:1pt solid #F3F3F3;background-color: #F5F5F5;\"";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Name"].ToString();
                dt.Rows[i]["Date"].ToString();
                dt.Rows[i]["Remarks"].ToString();
                dt.Rows[i]["Flag"].ToString();
                string NrOfDays = "";
                DateTime d1 = DateTime.Now;
                DateTime dtstart = Convert.ToDateTime(dt.Rows[i]["Date"]);
                string strDateTime2 = dtstart.ToString("dd-MM-yyyy");
                DateTime d2 = Convert.ToDateTime(dt.Rows[i]["Date"].ToString());
                TimeSpan t = d1 - d2;

                if (t.Days > 0)
                {
                    int days = t.Days;
                    NrOfDays = days + " Days".ToString();
                }
                else if (t.Hours > 0)
                {
                    int Hour = t.Hours;
                    NrOfDays = Hour + " Hour".ToString();
                }
                else if (t.Minutes > 0)
                {
                    int Minute = t.Minutes;
                    NrOfDays = Minute + " Minutes".ToString();
                }
                else if (t.Seconds > 0)
                {
                    int Second = t.Seconds;
                    NrOfDays = Second + " Second".ToString();
                }
                if (dt.Rows[i]["Flag"].ToString() == "Attachment Added")
                {
                    htmlStr += "<tr  style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: " + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
                else
                {
                    htmlStr += "<tr style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }
        private DataTable GetAssetHistoryById(long assetId)
        {
           // string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            //using (SqlConnection con = new SqlConnection(constr))
            //{
            //    using (SqlCommand cmd = new SqlCommand("GetCaseDetails", con))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.AddWithValue("@CaseId", caseId);
            //        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            //        {
            //            sda.Fill(dt);
            //        }
            //    }
            //}
            return dt;
        }

        
        [HttpPost]
        public JsonResult GetAssetTypeByAssetCategory(int categoryId)
        {
            var asset = assetService.AssetType(categoryId);
            return Json(asset, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult GetSubLocationByLocationId(int locationId)
        {
            var locatin = assetService.AssetSubLocation(locationId);
            return Json(locatin, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult ExportKGAssetList(int CompanyId, int DepartmentId, int AssetLocationId, int AssetSubLocationId)
        {
            int comId = (int)Session["CompanyId"];
            var gv = new GridView();
            string fileName = "KG Asset List";

            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (CompanyId != null || DepartmentId != null || AssetLocationId != null || AssetSubLocationId != null)
            {
                dt = GetAssetList(CompanyId, DepartmentId, AssetLocationId, AssetSubLocationId);
            }
            else
            {
                dt = GetAssetList(comId);
            }


            gv.DataSource = dt;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename= " + fileName + ".xls");
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

        public DataTable GetAssetList(int? CompanyId, int? DepartmentId, int? AssetLocationId, int? AssetSubLocationId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Asset_ExportAsset", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                        cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                        cmd.Parameters.AddWithValue("@AssetLocationId", AssetLocationId);
                        cmd.Parameters.AddWithValue("@AssetSubLocationId", AssetSubLocationId);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetAssetList(int comId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("KGRE_ExportClientLeadList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyId", comId);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

       


    }
}