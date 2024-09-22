using KGERP.Data.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class DropDownTypesController : Controller
    {
        private ERPEntities db = new ERPEntities();

        // GET: DropDownTypes
        public ActionResult Index()
        {
            return View(db.DropDownTypes.ToList());
        }

        // GET: DropDownTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownType dropDownType = db.DropDownTypes.Find(id);
            if (dropDownType == null)
            {
                return HttpNotFound();
            }
            return View(dropDownType);
        }

        // GET: DropDownTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DropDownTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Code,IsActive")] DropDownType dropDownType)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
                {
                    dropDownType.CreatedBy = Session["UserName"].ToString();
                    dropDownType.CreatedDate = DateTime.Now;
                    dropDownType.ModifiedBy = Session["UserName"].ToString();
                    dropDownType.ModifiedDate = DateTime.Now;
                }
                db.DropDownTypes.Add(dropDownType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dropDownType);
        }

        // GET: DropDownTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownType dropDownType = db.DropDownTypes.Find(id);
            if (dropDownType == null)
            {
                return HttpNotFound();
            }
            return View(dropDownType);
        }

        // POST: DropDownTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Code,IsActive")] DropDownType dropDownType)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
                {
                    dropDownType.ModifiedBy = Session["UserName"].ToString();
                    dropDownType.ModifiedDate = DateTime.Now;
                }
                db.Entry(dropDownType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dropDownType);
        }

        // GET: DropDownTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownType dropDownType = db.DropDownTypes.Find(id);
            if (dropDownType == null)
            {
                return HttpNotFound();
            }
            return View(dropDownType);
        }

        // POST: DropDownTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DropDownType dropDownType = db.DropDownTypes.Find(id);
            db.DropDownTypes.Remove(dropDownType);
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
    }
}
