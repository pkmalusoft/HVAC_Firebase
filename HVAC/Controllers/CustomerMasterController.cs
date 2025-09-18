using HVAC.DAL;
using HVAC.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using System;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class CustomerMasterController : Controller
    {
        HVACEntities db = new HVACEntities();

       
        public ActionResult Index()
        {
             

            return View();
        }

       
        public ActionResult Create()
        {
             
            return View();
        }

        //
        // POST: /Currency/Create

        [HttpPost]

        public ActionResult Create(CurrencyVM v)
        {
             

            return View();


        }

        

       






       



 
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}