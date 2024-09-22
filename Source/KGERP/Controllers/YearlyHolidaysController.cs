using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class YearlyHolidaysController : Controller
    {
        private ERPEntities db = new ERPEntities();

        private readonly IYearlyHoliday _service;

        public YearlyHolidaysController(ERPEntities db, IYearlyHoliday service)
        {
            this.db = db;
            _service = service;
        }



        // GET: YearlyHolidays
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            YearlyHolidayModel model =await _service.GetYearlyHolidayEvent();         
            return View(model);
        }



        // GET: YearlyHolidays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YearlyHoliday yearlyHoliday = db.YearlyHolidays.Find(id);
            if (yearlyHoliday == null)
            {
                return HttpNotFound();
            }
            return View(yearlyHoliday);
        }

        public ActionResult Details()
        {

            YearlyHoliday yearlyHoliday = db.YearlyHolidays.Find();
            if (yearlyHoliday == null)
            {
                return HttpNotFound();
            }
            return View(yearlyHoliday);
        }

        // GET: YearlyHolidays/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: YearlyHolidays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "YearlyHolidayId,HolidayDate,HolidayCategory,Purpose,CreatedBy,CreatedDate")] YearlyHoliday yearlyHoliday)
        {

            var emplId = Convert.ToInt64(Session["UserName"].ToString()); ;
            if (ModelState.IsValid)
            {
                yearlyHoliday.CreatedBy = emplId.ToString();
                yearlyHoliday.CreatedDate = DateTime.Now;
                db.YearlyHolidays.Add(yearlyHoliday);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(yearlyHoliday);
        }

        // GET: YearlyHolidays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YearlyHoliday yearlyHoliday = db.YearlyHolidays.Find(id);
            if (yearlyHoliday == null)
            {
                return HttpNotFound();
            }
            return View(yearlyHoliday);
        }

        // POST: YearlyHolidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "YearlyHolidayId,HolidayDate,HolidayCategory,Purpose,CreatedBy,CreatedDate")] YearlyHoliday yearlyHoliday)
        {
            if (ModelState.IsValid)
            {
                db.Entry(yearlyHoliday).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(yearlyHoliday);
        }

        // GET: YearlyHolidays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YearlyHoliday yearlyHoliday = db.YearlyHolidays.Find(id);
            if (yearlyHoliday == null)
            {
                return HttpNotFound();
            }
            return View(yearlyHoliday);
        }

        // POST: YearlyHolidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            YearlyHoliday yearlyHoliday = db.YearlyHolidays.Find(id);
            db.YearlyHolidays.Remove(yearlyHoliday);
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
