using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]

    public class LCController : Controller
    {
        private ERPEntities db = new ERPEntities();
        private readonly IDropDownItemService dropDownItemService;
        private readonly IDistrictService districtService;
        public LCController(IDropDownItemService _dropDownItemService, IDistrictService districtService)
        {
            this.districtService = districtService;
            dropDownItemService = _dropDownItemService;
        }
        // GET: LC
        public async Task<ActionResult> Index(int CompanyId, DateTime? fromDate, DateTime? toDate, int? LCType = 0)
        {
            LCViewModel model = new LCViewModel();
            //var PoType= dropDownItemService.GetDropDownItemSelectModels(58);
            model.LCType = LCType > 0? LCType : null;
            model.CompanyId = CompanyId;
            model.StrFromDate = fromDate.HasValue ? fromDate.Value.ToString() : "";
            model.POtypeLst = dropDownItemService.GetDropDownItemSelectModels(58).Where(e => e.Text.ToString() != "Local").ToList();
            model.StrToDate = toDate.HasValue ? toDate.Value.ToString() : "";
            model.DataList = await (from t1 in db.LCInfoes
                                    join t2 in db.Vendors on t1.SupplierId equals t2.VendorId
                                    join t3 in db.DropDownItems on t1.LCType equals t3.DropDownItemId 
                                    where t1.IsActive == true && t1.CompanyId == CompanyId
                                    && (fromDate.HasValue ? t1.LCDate >= fromDate : t1.LCDate != null) &&
                                    (toDate.HasValue ? t1.LCDate <= toDate : t1.LCDate != null)
                                    && (LCType == 0?t1.LCType>0:t1.LCType== LCType)
                                    select new LCModel
                                    {
                                        CompanyId = t1.CompanyId,
                                        LCDate = t1.LCDate,
                                        LCId = t1.LCId,
                                        LCNo = t1.LCNo,
                                        LCType = t3.Name,
                                        Supplier = t2.Name
                                        //FreighterCharge = t1.FreighterCharge.HasValue?t1.FreighterCharge.Value:0,
                                        //LCValue = t1.LCValue.HasValue?t1.LCValue.Value:0,
                                        //OtherCharge = t1.OtherCharge.HasValue?t1.OtherCharge.Value:0,
                                        // IsSubmit=t1.IsSubmit,
                                        // POCreated=t1.POCreated

                                    }).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetLCList(int companyId,int LcType)
        {
            var List = await (from t1 in db.LCInfoes
                              join t2 in db.Vendors on t1.SupplierId equals t2.VendorId
                              join t3 in db.DropDownItems on t1.LCType equals t3.DropDownItemId
                              where t1.IsActive == true && t1.LCType == LcType 
                              //&& t1.IsSubmit == true 
                              //&& t1.POCreated==false
                              && t1.CompanyId == companyId
                              select new LCModel
                              {
                                  //HeadId=t1.HeadId,
                                  CompanyId = t1.CompanyId,
                                  LCDate = t1.LCDate,
                                  LCId = t1.LCId,
                                  LCNo = t1.LCNo,
                                  LCType = t3.Name,
                                  Supplier = t2.Name,
                                  PINo = t1.PINo,
                                  //FreighterCharge = t1.FreighterCharge.HasValue ? t1.FreighterCharge.Value : 0,
                                  //LCValue = t1.LCValue.HasValue ? t1.LCValue.Value : 0,
                                  //OtherCharge = t1.OtherCharge.HasValue ? t1.OtherCharge.Value : 0,
                                  //IsSubmit = t1.IsSubmit,
                                  //POCreated = t1.POCreated,
                                  //OriginCountry = t1.OriginCountryId.HasValue ? t1.OriginCountryId.Value : 0,
                                  //ProductCountry = t1.ProductOriginId.HasValue ? t1.ProductOriginId.Value : 0,
                                  //InsuranceNo = t1.InsuranceNo,
                                  //PremiumValue = t1.PremiumValue.HasValue ? t1.PremiumValue.Value : 0

                              }).ToListAsync();
            return Json(List);
        }

        [HttpGet]
        public async Task<ActionResult> Finalize(int companyId, long id = 0)
        {
            var LC = await db.LCInfoes.SingleOrDefaultAsync(e => e.IsActive == true  && e.CompanyId == companyId && e.LCId == id);
            if (LC != null)
            {
                //LC.IsSubmit = true;
                try
                {
                    await db.SaveChangesAsync();   
                }
                catch (Exception ex)
                {
                }
                return RedirectToAction(nameof(Index), new { CompanyId = companyId, LCType=LC.LCType });

            }
            return RedirectToAction(nameof(Index), new { CompanyId = companyId });
        }
        // GET: LC/Create
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int companyId, long id = 0)
        {
            LCCreateModel vm = new LCCreateModel();
            vm.LC = new LCInfo() { CompanyId = companyId, LCDate = DateTime.Now };
            vm.CompanyId = companyId;
            vm.Countries = districtService.GetCountriesSelectModels();
            vm.LstSupplier = await db.Vendors.Where(o => o.CompanyId == companyId && o.VendorTypeId == 1 && o.IsActive == true).Select(e => new LCSelectModel
            {
                Text = e.Name,
                Value = e.VendorId
            }).ToListAsync();
            vm.POtypeLst = dropDownItemService.GetDropDownItemSelectModels(58).Where(e => e.Text.ToString() != "Local").ToList();

            if (id != 0)
            {
                vm.LC = await db.LCInfoes.FindAsync(id);
            }
            return View(vm);
        }

        // POST: LC/Create
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(LCCreateModel model)
        {

            try
            {
                if (model.LC.LCId > 0)
                {
                    model.LC.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    model.LC.ModifiedDate = DateTime.Now;
                    var LC = await db.LCInfoes.AsNoTracking().Where(o=>o.LCId==model.LC.LCId).FirstOrDefaultAsync();
                    LC = model.LC;
                    db.Entry(model.LC).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["successMessage"] = $"Successfully Updated LC:{model.LC.LCNo}";
                }
                else
                {
                    model.LC.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    model.LC.CreatedDate = DateTime.Now;
                    model.LC.IsActive = true;
                    db.LCInfoes.Add(model.LC);
                    await db.SaveChangesAsync();
                    TempData["successMessage"] = $"Successfully Created LC:{model.LC.LCNo}";
                }

            }
            catch (Exception ex)
            {
                TempData["successMessage"] = $"Failed  Error:{ex.Message}";
            }
            return RedirectToAction("Index", new { CompanyId = model.CompanyId });
        }

       
        // POST: LC/Delete/5
      
        public async  Task<ActionResult> Delete(int companyId,long id)
        {
            var model = await db.LCInfoes.FindAsync(id);
            if (model == null)
            {
                TempData["successMessage"] = $"Not found";
            }
            else
            {
                try
                {
                    //if (model.IsSubmit || model.POCreated)
                    //{
                    //    TempData["successMessage"] = $"Cannot Delete this LC:{model.LCNo}. Its used in other processes";
                    //}
                    //else
                    //{
                    //    model.IsActive = false;
                    //    await db.SaveChangesAsync();
                    //    TempData["successMessage"] = $"Successfully  Deleted  LC:{model.LCNo}.";
                    //}


                }
                catch (Exception )
                {
                    TempData["successMessage"] = $"Cannot Delete this LC:{model.LCNo}.Error Occured";
                }
            }
           
            return RedirectToAction("Index", new { CompanyId = companyId });
        }
    }
}
