using Microsoft.Office.Interop.Excel;
using HVAC.DAL;
using HVAC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class BrandController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.Brand> Protypes = db.Brands.Where(temp => temp.BrandName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.Brand obj = new Models.Brand();
            if (id == 0)
            {
                               

                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.Brand type = (from c in db.Brands where c.BrandID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.BrandID= type.BrandID;
                    obj.BrandName = type.BrandName;

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.Brands where b.BrandName == PType select b).FirstOrDefault();
                if (existtype != null)
                {
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    status = "false";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var existtypebyid = (from b in db.Brands where b.BrandName == PType && b.BrandID != ID select b).FirstOrDefault();
                if (existtypebyid != null)
                {
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    status = "false";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveBrand(Models.Brand ProType)
        {

            Models.Brand Ptype = new Models.Brand();

            if (ProType.BrandID == 0)
            {

                var duplicate = (from c in db.Brands where c.BrandName == ProType.BrandName select c).FirstOrDefault();

                if (duplicate !=null)
                {
                   
                    return Json(new { status = "Failed", message = "Duplicate Enquiry Stage Not allowed!" });

                }
                Ptype.BrandName = ProType.BrandName;                
                db.Brands.Add(Ptype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Brand Added Successfully!" });
            }
            else
            {
                var duplicate = (from c in db.Brands where c.BrandName == ProType.BrandName && c.BrandID != ProType.BrandID select c).FirstOrDefault();
                 
                if (duplicate != null)
                {
                    return Json(new { status = "Failed", message = "Brand already Exist!" });

                }

                Ptype = db.Brands.Find(ProType.BrandID);
                Ptype.BrandName = ProType.BrandName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Brand Updated Successfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.Brand protype = db.Brands.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var _enquiry = db.Equipments.Where(cc => cc.BrandID == id).FirstOrDefault();
                if (_enquiry!=null)
                {
                    return Json(new { status = "Failed", message = "Brand referenced in the Equipment,Could not Delete!" });
                }
                var protypes = db.Brands.Where(cc => cc.BrandID == id).ToList();
                db.Brands.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Brand Deleted Successfully!" });

            }

        }

    }
}