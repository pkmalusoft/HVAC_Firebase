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
    public class ProductFamilyController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.ProductFamily> Protypes = db.ProductFamilies.Where(temp => temp.ProductFamilyName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.ProductFamily obj = new Models.ProductFamily();
            if (id == 0)
            {
                               

                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.ProductFamily type = (from c in db.ProductFamilies where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.ID= type.ID;
                    obj.ProductFamilyName = type.ProductFamilyName;

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.ProductFamilies where b.ProductFamilyName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.ProductFamilies where b.ProductFamilyName == PType && b.ID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProductFamily(Models.ProductFamily ProType)
        {

            Models.ProductFamily Ptype = new Models.ProductFamily();

            if (ProType.ID == 0)
            {

                var duplicate = (from c in db.ProductFamilies where c.ProductFamilyName == ProType.ProductFamilyName select c).FirstOrDefault();

                if (duplicate !=null)
                {
                   
                    return Json(new { status = "Failed", message = "Duplicate Product Family Name Not allowed!" });

                }
                Ptype.ProductFamilyName = ProType.ProductFamilyName;                
                db.ProductFamilies.Add(Ptype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Product Family Added Successfully!" });
            }
            else
            {
                var duplicate = (from c in db.ProductFamilies where c.ProductFamilyName == ProType.ProductFamilyName && c.ID != ProType.ID select c).FirstOrDefault();
                 
                if (duplicate != null)
                {
                    return Json(new { status = "Failed", message = "Product Family already Exist!" });

                }

                Ptype = db.ProductFamilies.Find(ProType.ID);
                Ptype.ProductFamilyName = ProType.ProductFamilyName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "ProductFamily Updated Successfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.ProductFamily protype = db.ProductFamilies.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var _enquiry = db.Equipments.Where(cc => cc.ProductFamilyID == id).FirstOrDefault();
                if (_enquiry!=null)
                {
                    return Json(new { status = "Failed", message = "ProductFamily referenced in the Equipment,Could not Delete!" });
                }
                var protypes = db.ProductFamilies.Where(cc => cc.ID == id).ToList();
                db.ProductFamilies.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "ProductFamily Deleted Successfully!" });

            }

        }

    }
}