using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HVAC.DAL;
using HVAC.Models;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class PeriodLocksController : Controller
    {
        private HVACEntities db = new HVACEntities();

        // GET: PeriodLocks
        public ActionResult Index()
        {
            var AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            return View(db.PeriodLocks.ToList());
        }

        // GET: PeriodLocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodLock periodLock = db.PeriodLocks.Find(id);
            if (periodLock == null)
            {
                return HttpNotFound();
            }
            return View(periodLock);
        }

        // GET: PeriodLocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PeriodLocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PeriodLock periodLock)
        {
            try
            {
                var AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                //DateTime date = DateTime.UtcNow.Date;
                periodLock.UserName = Convert.ToInt32(Session["UserID"].ToString());
                periodLock.StatusChangeDate = DateTime.UtcNow.Date;
                periodLock.BranchId = branchid;
                periodLock.FYearID = AcFinancialYearID;
                db.PeriodLocks.Add(periodLock);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Period Lock.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["ErrorMsg"] = "Getting Error."+ex.Message; 
                return View(periodLock);
            }
            
        }

        // GET: PeriodLocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodLock periodLock = db.PeriodLocks.Find(id);
            if (periodLock == null)
            {
                return HttpNotFound();
            }
            return View(periodLock);
        }

        // POST: PeriodLocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PeriodLock periodLock)
        {
            try
            {
                periodLock.UserName = Convert.ToInt32(Session["UserID"].ToString());
                db.Entry(periodLock).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Period Lock.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Getting Error." + ex.Message;
                return View(periodLock);
            }
        }

        // GET: PeriodLocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodLock periodLock = db.PeriodLocks.Find(id);
            if (periodLock == null)
            {
                return HttpNotFound();
            }
            return View(periodLock);
        }

        // POST: PeriodLocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PeriodLock periodLock = db.PeriodLocks.Find(id);
            db.PeriodLocks.Remove(periodLock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult CheckPeriodLock(string vEntryDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            bool PeriodLock = false;
            StatusModel result = GeneralDAO.CheckDateValidate(vEntryDate, yearid);
            
            string PeriodLockMessage = "";
            if (result.Status == "PeriodLock") //Period locked
            {
                PeriodLock = true;
                PeriodLockMessage = result.Message;
            }
            else
            {
                PeriodLock = false;
                PeriodLockMessage = "Period is active";
            }

            return Json(new { PeriodLock = PeriodLock, message = PeriodLockMessage }, JsonRequestBehavior.AllowGet);
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
