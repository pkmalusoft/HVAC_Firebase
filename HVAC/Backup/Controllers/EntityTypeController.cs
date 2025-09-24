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
    public class EntityTypeController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.EntityType> Protypes = db.EntityTypes.Where(temp => temp.EntityTypeName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.EntityType obj = new Models.EntityType();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.EntityTypeID = 0;


            }
            else
            {
                ViewBag.Title = "EntityType - Modify";
                Models.EntityType type = (from c in db.EntityTypes where c.EntityTypeID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.EntityTypeID = type.EntityTypeID;
                    obj.EntityTypeName = type.EntityTypeName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.EntityTypes where b.EntityTypeName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.EntityTypes where b.EntityTypeName == PType && b.EntityTypeID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProjectType(Models.EntityType ProType)
        {

            Models.EntityType Ptype = new Models.EntityType();

            if (ProType.EntityTypeID == 0)
            {

                int max = (from c in db.EntityTypes orderby c.EntityTypeID descending select c.EntityTypeID).FirstOrDefault();

                if (max == null)
                {
                    Ptype.EntityTypeID = 1;
                    Ptype.EntityTypeName = ProType.EntityTypeName;
                  


                }
                else
                {
                    Ptype.EntityTypeID = max + 1;
                    Ptype.EntityTypeName = ProType.EntityTypeName;
                   


                }
                db.EntityTypes.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Entity Type Addded Succesfully!" });
            }
            else
            {
                var item = db.EntityTypes.Where(cc => cc.EntityTypeID == ProType.EntityTypeID && cc.EntityTypeID != ProType.EntityTypeID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "EntityType already Exist!" });

                }

                Ptype = db.EntityTypes.Find(ProType.EntityTypeID);
                Ptype.EntityTypeName = ProType.EntityTypeName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Entity Type Updated Succesfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.EntityType protype = db.EntityTypes.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.EntityTypes.Where(cc => cc.EntityTypeID == id).ToList();
                db.EntityTypes.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Entity Type Deleted Successfully!" });

            }

        }

    }
}