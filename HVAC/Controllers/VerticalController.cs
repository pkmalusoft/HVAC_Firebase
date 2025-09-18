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
    public class VerticalController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.Vertical> Protypes = db.Verticals.Where(temp => temp.VerticalName.Contains(SearchText)).ToList();
            return View(Protypes);

        }


        public ActionResult Create(int id = 0)
        {


            Models.Vertical obj = new Models.Vertical();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.VerticalID = 0;


            }
            else
            {
                ViewBag.Title = "Vertical - Modify";
                Models.Vertical type = (from c in db.Verticals where c.VerticalID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.VerticalID = type.VerticalID;
                    obj.VerticalName = type.VerticalName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProductVerticalExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.Verticals where b.VerticalName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.Verticals where b.VerticalName == PType && b.VerticalID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProductVertical(Models.Vertical ProType)
        {

            Models.Vertical Ptype = new Models.Vertical();

            if (ProType.VerticalID == 0)
            {

                Ptype.VerticalName = ProType.VerticalName;
                db.Verticals.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Vertical Addded Succesfully!" });
            }
            else
            {
                var item = db.Verticals.Where(cc => cc.VerticalID == ProType.VerticalID && cc.VerticalID != ProType.VerticalID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "Vertical already Exist!" });

                }

                Ptype = db.Verticals.Find(ProType.VerticalID);
                Ptype.VerticalName = ProType.VerticalName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Vertical Updated Succesfully!" });
            }

        }

        public JsonResult DeleteVertical(int id)
        {

            Models.Vertical protype = db.Verticals.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.Verticals.Where(cc => cc.VerticalID == id).ToList();
                db.Verticals.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Vertical Deleted Successfully!" });

            }

        }

    }
}