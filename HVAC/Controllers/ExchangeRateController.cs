using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ExchangeRateController : Controller
    {
         HVACEntities db = new HVACEntities();


        public ActionResult Index()
        {
            List<CurrencyRateVM> lst = new List<CurrencyRateVM>();

            var data = (from c in db.CurrencyRates join b in db.CurrencyMasters on c.BranchCurrencyID equals b.CurrencyID join t in db.CurrencyMasters on c.TransactionCurrencyID equals t.CurrencyID select new CurrencyRateVM { CurrencyRateID = c.CurrencyRateID, BaseCurrency = b.CurrencyName, TransactionCurrency = t.CurrencyName, CurrencyRate1 = c.CurrencyRate1, TransDate = c.TransDate }).ToList();




            return View(data);
            
        }

        //
        // GET: /Currency/Details/5

        public ActionResult Details(int id = 0)
        {
            CurrencyMaster currencymaster = db.CurrencyMasters.Find(id);
            if (currencymaster == null)
            {
                return HttpNotFound();
            }
            return View(currencymaster);
        }

        //
        // GET: /Currency/Create

        public ActionResult Create()
        {
            CurrencyRateVM vm = new CurrencyRateVM();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            vm.TransDate = CommonFunctions.GetCurrentDateTime();
            return View(vm);
        }

        public JsonResult CheckBaseCurrency()
        {
            IsExist a = new IsExist();

            var data = (from c in db.CurrencyMasters where c.StatusBaseCurrency == true select c).FirstOrDefault();
            if (data != null)
            {
                a.x = 1;
            }
            else
            {
                a.x = 0;
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public class IsExist
        {
            public int x { get; set; }
        }

        //
        // POST: /Currency/Create

        [HttpPost]
       
        public ActionResult Create(CurrencyRateVM v)
        {
            if (ModelState.IsValid)
            {
                CurrencyRate ob = new CurrencyRate();


                int max = (from d in db.CurrencyRates orderby d.CurrencyRateID descending select d.CurrencyRateID).FirstOrDefault();

                if (max == null)
                {
                    ob.CurrencyRateID= 1;
                    ob.BranchCurrencyID = v.BranchCurrencyID;
                    ob.TransactionCurrencyID = v.TransactionCurrencyID;
                    ob.TransDate = v.TransDate;
                    ob.CurrencyRate1 = v.CurrencyRate1;
                    
                }
                else
                {
                    ob.CurrencyRateID = max + 1;
                    ob.BranchCurrencyID = v.BranchCurrencyID;
                    ob.TransactionCurrencyID = v.TransactionCurrencyID;
                    ob.TransDate = v.TransDate;
                    ob.CurrencyRate1 = v.CurrencyRate1;
                }

                db.CurrencyRates.Add(ob);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Currency Rate.";
                return RedirectToAction("Index");
            }

            return View();

          
        }

        public JsonResult GetCurrencyName(string name)
        {
            var currency = (from c in db.CurrencyMasters where c.CurrencyName == name select c).FirstOrDefault();
            Status s = new Status();
            if (currency == null)
            {
                s.flag = 0;
            }
            else
            {
                s.flag = 1;
            }





            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public class Status
        {
            public int flag { get; set; }
        }







        public ActionResult Edit(int id)
        {
            ViewBag.Currency = db.CurrencyMasters.ToList();
            CurrencyRateVM c = new CurrencyRateVM();
            ViewBag.country = db.CountryMasters.ToList();
            var data = (from d in db.CurrencyRates where d.CurrencyRateID == id select d).FirstOrDefault();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                c.CurrencyRateID = data.CurrencyRateID;
                c.BranchCurrencyID= data.BranchCurrencyID;
                c.TransactionCurrencyID = data.TransactionCurrencyID;
                c.TransDate = data.TransDate;
                c.CurrencyRate1 = data.CurrencyRate1;
                
            }
            return View(c);
        }

        //
        // POST: /Currency/Edit/5

        [HttpPost]
       
        public ActionResult Edit(CurrencyRateVM v)
        {
            CurrencyRate c = new CurrencyRate();
            c = db.CurrencyRates.Find(v.CurrencyRateID);            
            c.BranchCurrencyID = v.BranchCurrencyID;
            c.TransactionCurrencyID = v.TransactionCurrencyID;
            c.TransDate = v.TransDate;
            c.CurrencyRate1 = v.CurrencyRate1;

            if (ModelState.IsValid)
            {
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Currency Rate.";
                return RedirectToAction("Index");
            }
            return View();
        }

      
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CurrencyRate currencymaster = db.CurrencyRates.Find(id);
                db.CurrencyRates.Remove(currencymaster);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Deleted Currency Rate.";
            }
            catch (Exception ex)
            {
                 TempData["ErrorMsg"] = "Can Not Delete This Currency. Already in Use.";
                
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}