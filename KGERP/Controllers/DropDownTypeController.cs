using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class DropDownTypeController : BaseController
    {
        private readonly IDropDownTypeService dropDownTypeService;
        public DropDownTypeController(IDropDownTypeService dropDownTypeService)
        {
            this.dropDownTypeService = dropDownTypeService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            DropDownTypeModel dropDownTypeModel = new DropDownTypeModel();
            dropDownTypeModel = await dropDownTypeService.GetDropDownTypes(companyId);
            return View(dropDownTypeModel);
        }
       

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            DropDownTypeModel model = dropDownTypeService.GetDropDownType(id);
            return View(model);
        }


        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(DropDownTypeModel model)
        {
            string message = string.Empty;
            if (model.DropDownTypeId <= 0)
            {
                dropDownTypeService.SaveDropDownType(0, model, out message);
            }
            else
            {
                dropDownTypeService.SaveDropDownType(model.DropDownTypeId, model, out message);
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId });
        }

        [HttpGet]
        
        public ActionResult DeleteDropDownType(int id)
        {
            int companyId = (int)Session["CompanyId"];
            bool result = dropDownTypeService.DeleteDropDownType(id);
            if (result)
            {
                return RedirectToAction("Index", new { companyId });
            }
            return View();
        }

        //// GET: DropDownTypes/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DropDownType dropDownType = db.DropDownTypes.Find(id);
        //    if (dropDownType == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dropDownType);
        //}

        //// POST: DropDownTypes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Name,Code,IsActive")] DropDownType dropDownType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
        //        { 
        //            dropDownType.ModifiedBy = Session["UserName"].ToString();
        //            dropDownType.ModifiedDate = DateTime.Now;
        //        }
        //        db.Entry(dropDownType).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(dropDownType);
        //}

        //// GET: DropDownTypes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DropDownType dropDownType = db.DropDownTypes.Find(id);
        //    if (dropDownType == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dropDownType);
        //}

        //// POST: DropDownTypes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    DropDownType dropDownType = db.DropDownTypes.Find(id);
        //    db.DropDownTypes.Remove(dropDownType);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
