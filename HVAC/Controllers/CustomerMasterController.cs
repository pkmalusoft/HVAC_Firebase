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
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging framework)
                ModelState.AddModelError("", "An error occurred while loading customers. Please try again.");
                return View();
            }
        }

       
        public ActionResult Create()
        {
             
            return View();
        }

        //
        // POST: /Currency/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrencyVM v)
        {
            try
            {
                // Input validation
                if (!ModelState.IsValid)
                {
                    return View(v);
                }

                // Additional custom validation
                if (string.IsNullOrEmpty(v.CurrencyName))
                {
                    ModelState.AddModelError("CurrencyName", "Currency name is required.");
                    return View(v);
                }

                return View();
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging framework)
                ModelState.AddModelError("", "An error occurred while creating the customer. Please try again.");
                return View(v);
            }
        }

        

       






       



 
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}