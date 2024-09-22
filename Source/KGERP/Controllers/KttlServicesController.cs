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
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KttlServicesController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKttlService kttlService = new KttlServiceService();
        IKttlCustomerService kttlCustomerService = new KttlCustomerService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());

        // GET: KttlServices
        public ActionResult Index(int? Page_No, string searchText)
        {
            //return View(db.KttlServices.ToList());
            searchText = searchText ?? "";
            List<KttlServiceModel> kttlServices = kttlService.GetKttlServices(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(kttlServices.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: KttlServices/Details/5
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KttlServiceModel kttlServices = kttlService.GetKttlService(id);
            // KttlService kttlService = db.KttlServices.Find(id);

            if (kttlServices == null)
            {
                return HttpNotFound();
            }
            return View(kttlServices);
        }

        // GET: KttlServices/Create
        public ActionResult Create(string clientId, int id, string searchText)
        {

            Session["FullName"] = string.Empty;
            Session["ClientId"] = string.Empty;
            //Session["Id"] = 0;
            string clientInfo = string.Empty;

            KttlCustomer kttlCustomer = db.KttlCustomers.Find(id);
            if (kttlCustomer != null)
            {
                ViewBag.FullName = kttlCustomer.FullName;
                Session["ClientId"] = kttlCustomer.ClientId;
            }

            KttlServiceModel model = new KttlServiceModel();
            model = kttlService.GetKttlService(id);

            if (model == null)
            {
                model = new KttlServiceModel();
                model.ServiceTypes = dropDownItemService.GetDropDownItemSelectModels(16);
            }
            else
            {
                model.ServiceTypes = dropDownItemService.GetDropDownItemSelectModels(16);
            }

            return View(model);
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int? clientId, int? id, string searchText, string companyId)
        {
            if (System.Web.HttpContext.Current.User.Identity.Name == "KG3068" || System.Web.HttpContext.Current.User.Identity.Name == "KG1223"
                 || System.Web.HttpContext.Current.User.Identity.Name == "KG0284" || System.Web.HttpContext.Current.User.Identity.Name == "KG1088"
                 || System.Web.HttpContext.Current.User.Identity.Name == "KG0129" || System.Web.HttpContext.Current.User.Identity.Name == "KG0286")
            {
                Session["FullName"] = string.Empty;
                Session["ClientId"] = string.Empty;
                //Session["Id"] = 0;
                string clientInfo = string.Empty;
                KttlCustomer _KttlCustomer = null;
                KttlCustomer _KttlCustomer2 = null;

                if (clientId > 0)
                {
                    _KttlCustomer = db.KttlCustomers.Where(x => x.ClientId == clientId).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    int searchText1 = Convert.ToInt32(searchText);
                    _KttlCustomer2 = db.KttlCustomers.Where(x => x.ClientId.Equals(searchText1)).FirstOrDefault();

                }
                KttlService _KttlServece = db.KttlServices.Find(id);
                if (_KttlCustomer2 != null)
                {
                    Session["FullName"] = _KttlCustomer2.FullName;
                    Session["ClientId"] = _KttlCustomer2.ClientId;
                    // Session["Id"] = _KttlCustomer2.ClientId;
                    clientInfo = GetKTTLClientInformation(_KttlCustomer2.ClientId);
                }

                if (_KttlCustomer != null)
                {
                    Session["ClientId"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    ViewBag.FullName = _KttlCustomer.FullName;
                    ViewBag.ClientId = _KttlCustomer.ClientId;
                    //ViewBag.Id = _KttlCustomer.ClientId;
                    Session["ClientId"] = _KttlCustomer.ClientId;
                    Session["FullName"] = _KttlCustomer.FullName;
                    clientInfo = GetKTTLClientInformation(_KttlCustomer.ClientId);
                }
                if (_KttlServece != null)
                {
                    Session["OID"] = _KttlServece.OID;
                }

                KttlServiceModel model = new KttlServiceModel();
                model = kttlService.GetKttlService(Convert.ToInt32(id));


                if (model == null)
                {
                    model = new KttlServiceModel();
                    model.ServiceTypes = dropDownItemService.GetDropDownItemSelectModels(16);
                    model.RoomTypes = dropDownItemService.GetDropDownItemSelectModels(54);
                    model.TransportTypes = dropDownItemService.GetDropDownItemSelectModels(55);
                    model.HotelTypes = dropDownItemService.GetDropDownItemSelectModels(53);
                    model.AirportNames = dropDownItemService.GetDropDownItemSelectModels(51);
                    model.AirlinesNames = dropDownItemService.GetDropDownItemSelectModels(52);


                    model.AirportDrops = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.AirportPickups = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMakkahZiyarah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMakkah2Madinah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMadinahZiyarah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.HazzPackages = dropDownItemService.GetDropDownItemSelectModels(17);
                    model.ServiceYears = kttlCustomerService.GetServeiceYear();

                }
                else
                {
                    if (!string.IsNullOrEmpty(Session["ClientId"].ToString()))
                    {
                        model.ClientId = Convert.ToInt32(Session["ClientId"]);
                        model.ClientName = Session["FullName"].ToString();
                    }
                    model.ServiceTypes = dropDownItemService.GetDropDownItemSelectModels(16);
                    model.RoomTypes = dropDownItemService.GetDropDownItemSelectModels(54);
                    model.TransportTypes = dropDownItemService.GetDropDownItemSelectModels(55);
                    model.HotelTypes = dropDownItemService.GetDropDownItemSelectModels(53);
                    model.AirportNames = dropDownItemService.GetDropDownItemSelectModels(51);
                    model.AirlinesNames = dropDownItemService.GetDropDownItemSelectModels(52);

                    model.AirportDrops = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.AirportPickups = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMakkahZiyarah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMakkah2Madinah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.IsMadinahZiyarah = dropDownItemService.GetDropDownItemSelectModels(56);
                    model.HazzPackages = dropDownItemService.GetDropDownItemSelectModels(17);
                    model.ServiceYears = kttlCustomerService.GetServeiceYear();
                }

                if (id > 0)
                {
                    KttlCustomer _KttlCustomerExist = db.KttlCustomers.Where(x => x.ClientId == model.ClientId).FirstOrDefault();

                    if (_KttlCustomerExist != null)
                    {
                        clientInfo = GetKTTLClientInformation(_KttlCustomerExist.ClientId);
                    }

                }
                ViewBag.clientInfo = clientInfo;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "KTTLCRM", new { @companyId = 18 });
            }
        }


        // POST: KttlServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KttlServiceModel model)
        {
            if (model.OID <= 0)
            {
                model.ClientId = (int)Session["ClientId"];
                kttlService.SaveKttlService(0, model);
            }
            else
            {
                KttlService kttlService2 = db.KttlServices.FirstOrDefault(x => x.OID == model.OID);
                if (kttlService2 == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("Create");
                }

                kttlService.SaveKttlService(model.OID, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(KttlServiceModel model)
        {
            string redirectPage = string.Empty;
            if (model.OID <= 0)
            {
                int serviceOid = Convert.ToInt32(model.OID);
                KttlService _KttlServiceExist = db.KttlServices.Where(x => x.OID == serviceOid).FirstOrDefault();

                if (_KttlServiceExist != null)
                {
                    if (_KttlServiceExist.ClientId <= 0)
                    {
                        TempData["errMessage"] = "Exists";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    model.ClientId = (int)Session["ClientId"];
                    kttlService.SaveKttlService(0, model);
                    Session["FullName"] = string.Empty;
                    Session["ClientId"] = string.Empty;
                }
                redirectPage = "Index";
            }
            else
            {
                KttlService _KttlService = db.KttlServices.Where(x => x.OID == model.OID).FirstOrDefault();
                if (_KttlService == null)
                {
                    TempData["errMessage1"] = "Data not found!";
                    return RedirectToAction("CreateOrEdit");
                }

                model.ClientId = Convert.ToInt32(Session["ClientId"]);
                kttlService.SaveKttlService(model.OID, model);
                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit";
                ViewBag.clientInfo = GetKTTLClientInformation(model.OID);
                Session["FullName"] = string.Empty;
                Session["ClientId"] = string.Empty;
            }

            return RedirectToAction(redirectPage);
        }


        // GET: KttlServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KttlService kttlService = db.KttlServices.Find(id);
            if (kttlService == null)
            {
                return HttpNotFound();
            }
            return View(kttlService);
        }

        // POST: KttlServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KttlService kttlService = db.KttlServices.Find(id);
            db.KttlServices.Remove(kttlService);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public string GetKTTLClientInformation(int id)
        {
            string htmlStr = "<table class='spacing - table' ";
            DataTable dt = new DataTable();
            dt = GetKTTLClientInformationByClientId(id);
            string style1 = "\"align:right;\"";
            string style12 = "\"background-color: #E9EDBE; vertical-align: middle;\"";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["ClientId"].ToString();
                dt.Rows[i]["FullName"].ToString();
                dt.Rows[i]["PassportNo"].ToString();
                dt.Rows[i]["ServiceYear"].ToString();
                dt.Rows[i]["ClientStatus"].ToString();
                dt.Rows[i]["MobileNo"].ToString();
                dt.Rows[i]["PermanentAddress"].ToString();
                //DateTime d1 = DateTime.Now;
                //DateTime dtstart = Convert.ToDateTime(dt.Rows[i]["JoiningDate"]);
                //string strDateTime2 = dtstart.ToString("dd/MM/yyyy");


                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Client Id: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["ClientId"].ToString() + "</td>";
                htmlStr += " <td align=" + "right" + "> <b> <span>Full Name: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["FullName"].ToString() + "</td></tr>";
                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Passport No: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["PassportNo"].ToString() + "</td>";
                htmlStr += "<td align=" + "right" + "> <b> <span>Service Year: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["ServiceYear"].ToString() + "</td></tr>";
                //htmlStr += "<tr><td align=" + "right" + "> <b> <span>Joining Date: </b></span></td><td style=" + style12 + ">" + strDateTime2 + "</td>"; 
                htmlStr += "<tr><td align=" + "right" + "> <b> <span>Client Status: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["ClientStatus"].ToString() + "</td>";
                htmlStr += "<td align=" + "right" + "> <b> <span>Services: </b></span></td><td style=" + style12 + ">" + dt.Rows[i]["Services"].ToString() + "</td></tr>";
                htmlStr += "<tr colspan='2'><td align='right' style =" + style1 + "> <b> <span>PermanentAddress: </b></span></td><td style=" + style12 + " colspan=" + "3" + ">" + dt.Rows[i]["PermanentAddress"].ToString() + "</td></tr>";
            }

            return htmlStr += "</table>";
        }

        private DataTable GetKTTLClientInformationByClientId(int Id)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("[KTTL_GetClientDataByClientId]", con))
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
    }
}
