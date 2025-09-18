using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Data;
using HVAC.DAL;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class CountryMasterController : Controller
    {
        private HVACEntities db = new HVACEntities();

        //
        // GET: /CountryMaster/

        public ActionResult Index()
        {
            var data = db.CountryMasters.ToList();
            List<CountryMasterVM> lst = new List<CountryMasterVM>();
            foreach(var Item in data)
            {
                CountryMasterVM obj = new CountryMasterVM();
                obj.CountryID = Item.CountryID;
                obj.CountryName = Item.CountryName;
                obj.CountryCode = Item.CountryCode;
                obj.IATACode = Item.IATACode;
                lst.Add(obj);
            }
             
            return View(lst);
        }

        //
        // GET: /CountryMaster/Details/5

        public ActionResult Details(int id = 0)
        {
            CountryMaster countrymaster = db.CountryMasters.Find(id);
            if (countrymaster == null)
            {
                return HttpNotFound();
            }
            return View(countrymaster);
        }

        //
        // GET: /CountryMaster/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CountryMaster/Create

        [HttpPost]

        public ActionResult Create(CountryMasterVM c)
        {


            CountryMaster obj = new CountryMaster();
            int max = (from a in db.CountryMasters orderby a.CountryID descending select a.CountryID).FirstOrDefault();

            if (max == null)
            {
                obj.CountryID = 1;
                obj.CountryName = c.CountryName;
                obj.CountryCode = c.CountryCode;
                obj.IATACode = c.IATACode;
            }
            else
            {
                obj.CountryID = max + 1;
                obj.CountryName = c.CountryName;
                obj.CountryCode = c.CountryCode;
                obj.IATACode = c.IATACode;
            }
            db.CountryMasters.Add(obj);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully added Country.";

            return RedirectToAction("Index");

        }
      
            
      


        //
        // GET: /CountryMaster/Edit/5


        public ActionResult Edit(int id)
        {
            CountryMasterVM d = new CountryMasterVM();
            var data = (from c in db.CountryMasters where c.CountryID == id select c).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                d.CountryID = data.CountryID;
                d.CountryName = data.CountryName;
                d.CountryCode = data.CountryCode;
                d.IATACode = data.IATACode;
            }
            return View(d);
        }

        //
        // POST: /CountryMaster/Edit/5

        [HttpPost]
        public ActionResult Edit(CountryMasterVM a)
        {
            CountryMaster d = new CountryMaster();
            d.CountryID = a.CountryID;
            d.CountryName = a.CountryName;
            d.CountryCode = a.CountryCode;
            d.IATACode = a.IATACode;

            //if (ModelState.IsValid)
            //{
            //    db.Entry(d).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View();
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Updated Country.";

            return RedirectToAction("Index");
        }

       
       
        public ActionResult DeleteConfirmed(int id)
        {
            CountryMaster countrymaster = db.CountryMasters.Find(id);
            db.CountryMasters.Remove(countrymaster);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Country.";
            return RedirectToAction("Index");
        }
        #region "CountryCitySearch"
        [HttpGet, ActionName("GetCountryList")]
        public JsonResult GetCountryList(string SearchText)
        {
            //List<CountryMasterVM> lst =(List<CountryMasterVM>)Session["CountryList"];
            //if (lst == null)
            //{
            //    lst = PickupRequestDAO.GetCountryName();
            //    Session["CountryList"] = lst;
            //}
            //try
            //{
            //    if (SearchText.Trim() =="")
            //    {
            //        var list = lst.OrderBy(cc => cc.CountryName).Take(20);
            //        return Json(list, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        var list = lst.Where(cc => cc.CountryName.ToLower().StartsWith(SearchText.ToLower())).OrderBy(cc => cc.CountryName).Take(20);
            //        return Json(list, JsonRequestBehavior.AllowGet);
            //    }                

            //}
            //catch (Exception ex)
            //{
            //    return Json(ex.Message, JsonRequestBehavior.AllowGet);
            //}

            if (SearchText != null && SearchText.Trim() != "")
            {
                var list = (from c1 in db.CountryMasters
                            where c1.CountryName.ToLower().Contains(SearchText.ToLower())
                            orderby c1.CountryName ascending
                            select new { Id = c1.CountryID, CountryName = c1.CountryName, CountryCode = c1.CountryCode }).Take(10);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = (from c1 in db.CountryMasters
                            orderby c1.CountryName ascending
                            select new { Id = c1.CountryID, CountryName = c1.CountryName, CountryCode = c1.CountryCode }).Take(10);
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }



        [HttpGet, ActionName("GetCityList")]
        public JsonResult GetCityList(string SearchText)
        {
            //List<CityVM> lst = (List<CityVM>)Session["CityList"];
            //if (lst == null)
            //{
            //    lst = PickupRequestDAO.GetCityName();
            //    Session["CityList"] = lst;
            //}
            //try
            //{
            //    if (SearchText.Trim() == "")
            //    {
            //        var list = lst.OrderBy(cc => cc.City).Take(20);
            //        return Json(list, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        var list = lst.Where(cc => cc.City.ToLower().StartsWith(SearchText.ToLower())).OrderBy(cc=>cc.City).Take(20);
            //        return Json(list, JsonRequestBehavior.AllowGet);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    return Json(ex.Message, JsonRequestBehavior.AllowGet);
            //}

            if (SearchText != null && SearchText.Trim() != "")
            {
                var list = (from c1 in db.CityMasters
                            join c2 in db.CountryMasters on c1.CountryID equals c2.CountryID
                            where c1.City.ToLower().StartsWith(SearchText.ToLower())
                            orderby c1.City ascending
                            select new CityVM { CityID = c1.CityID, City = c1.City, CountryID = c2.CountryID, CountryName = c2.CountryName, CountryCode = c2.CountryCode }).Take(20).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = (from c1 in db.CityMasters
                            join c2 in db.CountryMasters on c1.CountryID equals c2.CountryID
                            orderby c1.City ascending
                            select new CityVM { CityID = c1.CityID, City = c1.City, CountryID = c2.CountryID, CountryName = c2.CountryName, CountryCode = c2.CountryCode }).Take(20);
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpGet, ActionName("GetLocationList")]
        public JsonResult GetLocationList(string SearchText)
        {
            List<LocationVM> lst = (List<LocationVM>)Session["LocationList"];
            if (lst == null || lst.Count == 0)
            {
                lst = EmailDAO.GetLocationName("");
                Session["LocationList"] = lst;
            }

            try
            {
                if (SearchText.Trim() == "")
                {
                    var list = lst.OrderBy(cc => cc.Location).Take(20);
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = lst.Where(cc => cc.Location.ToLower().Contains(SearchText.ToLower())).OrderBy(cc => cc.Location).Take(20);
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}