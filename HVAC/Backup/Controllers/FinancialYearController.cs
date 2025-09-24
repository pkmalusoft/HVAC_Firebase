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
    public class FinancialYearController : Controller
    {
        private HVACEntities db = new HVACEntities();

        // GET: PeriodLocks
        public ActionResult Index()
        {
            int BranchId = CommonFunctions.ParseInt(Session["CurrentBranchID"].ToString());
            var lst = db.AcFinancialYears.Where(cc => cc.BranchID == BranchId).ToList();
            return View(lst);
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
        public ActionResult Create(int id=0)
        {

            AcFinancialYear vm = new AcFinancialYear();
            if (id==0)
            {
                ViewBag.Title = "Create";
                vm.AcFYearFrom = CommonFunctions.GetCurrentDateTime();
                vm.AcFYearTo = CommonFunctions.GetCurrentDateTime();
            }
            else
            {
                ViewBag.Title = "Modify";
                vm= db.AcFinancialYears.Find(id);
            }
            return View(vm);
        }

        // POST: PeriodLocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcFinancialYear model)
        {
            try
            {
                AcFinancialYear year = new AcFinancialYear();

                //DateTime date = DateTime.UtcNow.Date;
                year.AcCompanyID=1;
                year.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                year.ReferenceName = model.ReferenceName;
                year.AcFYearFrom = model.AcFYearFrom;
                year.AcFYearTo = model.AcFYearTo;
                year.UserID = Convert.ToInt32(Session["UserID"].ToString());
                year.StatusClose = false;
                db.AcFinancialYears.Add(year);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Period Lock.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["ErrorMsg"] = "Getting Error."+ex.Message; 
                return View(model);
            }
            
        }

        // GET: PeriodLocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcFinancialYear year = db.AcFinancialYears.Find(id);
            if (year == null)
            {
                return HttpNotFound();
            }
            return View(year);
        }

        // POST: PeriodLocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveFinancialYear(AcFinancialYear model)
        {
            try
            {
                if (model.AcFinancialYearID == 0)
                {
                    AcFinancialYear year = new AcFinancialYear();

                    //DateTime date = DateTime.UtcNow.Date;
                    year.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    year.ReferenceName = model.ReferenceName;
                    year.AcFYearFrom = model.AcFYearFrom;
                    year.AcFYearTo = model.AcFYearTo;
                    year.UserID = Convert.ToInt32(Session["UserID"].ToString());
                    year.StatusClose = false;
                    db.AcFinancialYears.Add(year);
                    db.SaveChanges();

                   
                        return Json(new { status = "OK", AcFinancialYearID =0 , message = "Financial Year Added Succesfully!" });
                  
                   
                }
                else
                {

                    var vm = db.AcFinancialYears.Find(model.AcFinancialYearID);
                    vm.ReferenceName = model.ReferenceName;
                    vm.AcFYearFrom = model.AcFYearFrom;
                    vm.AcFYearTo = model.AcFYearTo;
                    db.Entry(vm).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMsg"] = "You have successfully Updated Period Lock.";
                  
                    return Json(new { status = "OK", AcFinancialYearID  = vm.AcFinancialYearID, message = "Financial Year Updated Succesfully!" });
                   

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Getting Error." + ex.Message;
                return Json(new { status = "Failed", AcFinancialYearID = model.AcFinancialYearID, message = ex.Message});
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

        public JsonResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = MasterDAO.DeleteFinancialYear(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            status = dt.Rows[0][0].ToString();
                            message = dt.Rows[0][1].ToString();

                            return Json(new { status = status, message = message });
                        }

                    }
                    else
                    {

                        return Json(new { status = "Failed", message = "Delete Failed!" });
                    }
                }
                else
                {

                    return Json(new { status = "Failed", message = "Delete Failed!" });
                }
            }

            return Json(new { status = "Failed", message = "Delete Failed!" });
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
