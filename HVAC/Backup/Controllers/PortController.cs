using HVAC.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    
    public class PortController : Controller
    {
        SourceMastersModel objSourceMastersModel = new SourceMastersModel();
        HVACEntities db = new HVACEntities();
        //
        // GET: /Designation/

        public ActionResult Index()
        {
            List<PortCountryVM> model = new List<PortCountryVM>();
            model = EnquiryDAO.GetPortList();
            return View(model);
        }

        //
        // GET: /Port/Details/5

        public ActionResult Details(int id = 0)
        {
            Port port = db.Ports.Find(id);

            if (port == null)
            {
                return HttpNotFound();
            }
            return View(port);
        }

        //
        // GET: /Port/Create

        public ActionResult Create(int id=0)
        {
            PortVM vm = new PortVM();
            if (id == 0)
            {
                vm.PortID = 0;
                ViewBag.Title = "CREATE";
            }
            else
            {
                Port p = db.Ports.Find(id);
                vm.PortID = p.PortID;
                vm.Port1 = p.Port1;
                vm.PortCode = p.PortCode;
                vm.PortType = p.PortType;
                vm.CityID = p.CityID;
                vm.CountryID = p.CountryID;
                vm.OriginCity = db.CityMasters.Find(vm.CityID) != null ? db.CityMasters.Find(vm.CityID).City : "";
                vm.OriginCountry = db.CountryMasters.Find(vm.CountryID) !=null ? db.CountryMasters.Find(vm.CountryID).CountryName : "";
                ViewBag.Title = "MODIFY";
            }
            

            return View(vm);
        }
        public static class DropDownList<T>
        {
            public static SelectList LoadItems(IList<T> collection, string value, string text)
            {
                return new SelectList(collection, value, text);
            }
        }

        //
        // POST: /Port/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Port port)
        {

            if (ModelState.IsValid)
            {
                ViewBag.country = db.CountryMasters.ToList();


                List<Port> query = (from t in db.Ports where t.Port1 == port.Port1 && t.PortID!=port.PortID && t.PortID>0 select t).ToList();

                if (query.Count > 0)
                {
                    PortVM vm1 = new PortVM();
                    vm1.PortID = port.PortID;
                    vm1.PortCode = port.PortCode;
                    vm1.Port1 = port.Port1;
                    vm1.PortType = port.PortType;
                    vm1.CountryID = port.CountryID;
                    vm1.CityID = port.CityID;
                    ViewBag.SuccessMsg = "Port is already exist";
                    return View(vm1);
                }
                Port vm = new Port();
                if (port.PortID == 0)
                {
                    var PORT = db.Ports.OrderByDescending(item => item.PortID).FirstOrDefault();
                    int PORTID = 0;
                    if (PORT == null)
                    {
                        PORTID = 1;
                    }
                    else
                    {
                        PORTID = PORT.PortID + 1;
                    }
                    vm.PortID = PORTID;
                }
                else
                {
                    vm = db.Ports.Find(port.PortID);
                }
                
                    vm.Port1 = port.Port1;
                    vm.PortType = port.PortType;
                    vm.CityID = port.CityID;
                    vm.CountryID = port.CountryID;
                    vm.PortCode = port.PortCode;
                if (port.PortID == 0)
                {
                    db.Ports.Add(vm);
                    db.SaveChanges();
                    TempData["SuccessMSG"] = "You have successfully added Port.";
                }
                else
                {
                    db.Entry(vm).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMSG"] = "You have successfully Updated Port.";
                }
                    
                
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErorMSG"] = "Error";
            }

            return View(port);
        }

     
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var port =db.Ports.Find(id);
            if (port!=null)
            {
                db.Ports.Remove(port);
                db.SaveChanges();
            }

            TempData["SuccessMSG"] = "You have successfully deleted Port.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowPortEntry()
        {
            PortCountryVM vm = new PortCountryVM();
            vm.OriginCity = "";
            vm.PortID = 0;
            return PartialView("PortEntry", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePortEntry(PortCountryVM model)
        {
            int uid = Convert.ToInt32(Session["UserID"].ToString());
            var cust = (from c in db.Ports where c.Port1.Trim().ToLower() == model.Port.Trim().ToLower() && c.PortType==model.PortType && c.PortID!=model.PortID select c).FirstOrDefault();
            if (cust != null)
            {
                model.PortID = cust.PortID;
                return Json(new { data = model, message = "Port Name Already Exist", status = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                Port vm = new Port();
                if (model.PortID == 0)
                {
                    var PORT = db.Ports.OrderByDescending(item => item.PortID).FirstOrDefault();
                    int PORTID = 0;
                    if (PORT == null)
                    {
                        PORTID = 1;
                    }
                    else
                    {
                        PORTID = PORT.PortID + 1;
                    }
                    vm.PortID = PORTID;
                }
                else
                {
                    vm = db.Ports.Find(model.PortID);
                }

                vm.Port1 = model.Port.ToUpper();
                vm.PortType = model.PortType;
                vm.CityID = model.CityID;
                vm.CountryID = model.CountryID;
                vm.PortCode = model.PortCode;
                if (model.PortID == 0)
                {
                    db.Ports.Add(vm);
                    db.SaveChanges();
                    return Json(new { data = vm, message = "Port Saved Successfully", status = "Ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(vm).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { data = vm, message = "Port Updated Successfully", status = "Ok" }, JsonRequestBehavior.AllowGet);

                }






            }
        }
        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}


        public JsonResult GetPortName(string term)
        {
            if (term != null && term != "")
            {
                var _clientlist = (from c in db.Ports join d in db.CountryMasters on c.CountryID equals d.CountryID where c.Port1.ToLower().Contains(term.ToLower()) orderby c.Port1 select new { PortID = c.PortID, PortName = c.Port1 + "," + d.CountryName }).ToList();
                return Json(_clientlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _clientlist = (from c in db.Ports join d in db.CountryMasters on c.CountryID equals d.CountryID  orderby c.Port1 select new { PortID = c.PortID, PortName = c.Port1 + "," + d.CountryName }).ToList();
                return Json(_clientlist, JsonRequestBehavior.AllowGet);
            }

        }
    }
}