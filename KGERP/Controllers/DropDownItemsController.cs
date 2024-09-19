using KGERP.Data.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class DropDownItemsController : Controller
    {
        private ERPEntities db = new ERPEntities();

        // GET: DropDownItems
        public ActionResult Index()
        {
            var dropDownItems = db.DropDownItems.Include(d => d.DropDownType);
            return View(dropDownItems.ToList());
        }

        // GET: DropDownItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownItem dropDownItem = db.DropDownItems.Find(id);
            if (dropDownItem == null)
            {
                return HttpNotFound();
            }
            return View(dropDownItem);
        }

        // GET: DropDownItems/Create
        public ActionResult Create()
        {
            ViewBag.DropDownTypeId = new SelectList(db.DropDownTypes, "DropDownTypeId", "Name");
            return View();
        }

        // POST: DropDownItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DropDownTypeId,Name,Value,Description")] DropDownItem dropDownItem)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
                {
                    dropDownItem.CreatedBy = "";
                    dropDownItem.CreatedDate = DateTime.Now;
                    dropDownItem.ModifiedDate = DateTime.Now;
                }

                db.DropDownItems.Add(dropDownItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DropDownTypeId = new SelectList(db.DropDownTypes, "DropDownTypeId", "Name", dropDownItem.DropDownTypeId);
            return View(dropDownItem);
        }

        // GET: DropDownItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownItem dropDownItem = db.DropDownItems.Find(id);
            if (dropDownItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.DropDownTypeId = new SelectList(db.DropDownTypes, "DropDownTypeId", "Name", dropDownItem.DropDownTypeId);
            return View(dropDownItem);
        }

        // POST: DropDownItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DropDownTypeId,Name,Value,Description")] DropDownItem dropDownItem)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Session["UserName"].ToString()))
                {
                    dropDownItem.ModifiedDate = DateTime.Now;
                }
                db.Entry(dropDownItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DropDownTypeId = new SelectList(db.DropDownTypes, "DropDownTypeId", "Name", dropDownItem.DropDownTypeId);
            return View(dropDownItem);
        }

        // GET: DropDownItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DropDownItem dropDownItem = db.DropDownItems.Find(id);
            if (dropDownItem == null)
            {
                return HttpNotFound();
            }
            return View(dropDownItem);
        }

        // POST: DropDownItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DropDownItem dropDownItem = db.DropDownItems.Find(id);
            db.DropDownItems.Remove(dropDownItem);
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
