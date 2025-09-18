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
    public class BuildingTypeController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.BuildingType> Protypes = db.BuildingTypes.Where(temp => temp.BldgTypeName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.BuildingType obj = new Models.BuildingType();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.BldgTypeID = 0;


            }
            else
            {
                ViewBag.Title = "BuildingType - Modify";
                Models.BuildingType type = (from c in db.BuildingTypes where c.BldgTypeID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.BldgTypeID = type.BldgTypeID;
                    obj.BldgTypeName = type.BldgTypeName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.BuildingTypes where b.BldgTypeName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.BuildingTypes where b.BldgTypeName == PType && b.BldgTypeID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProjectType(Models.BuildingType ProType)
        {

            Models.BuildingType Ptype = new Models.BuildingType();

            if (ProType.BldgTypeID == 0)
            {

                int max = (from c in db.BuildingTypes orderby c.BldgTypeID descending select c.BldgTypeID).FirstOrDefault();

                if (max == null)
                {
                    Ptype.BldgTypeID = 1;
                    Ptype.BldgTypeName = ProType.BldgTypeName;
                  


                }
                else
                {
                    Ptype.BldgTypeID = max + 1;
                    Ptype.BldgTypeName = ProType.BldgTypeName;
                   


                }
                db.BuildingTypes.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "BuildingType Addded Succesfully!" });
            }
            else
            {
                var item = db.BuildingTypes.Where(cc => cc.BldgTypeID == ProType.BldgTypeID && cc.BldgTypeID != ProType.BldgTypeID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "BuildingType already Exist!" });

                }

                Ptype = db.BuildingTypes.Find(ProType.BldgTypeID);
                Ptype.BldgTypeName = ProType.BldgTypeName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "BuildingType Updated Succesfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.BuildingType protype = db.BuildingTypes.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.BuildingTypes.Where(cc => cc.BldgTypeID == id).ToList();
                db.BuildingTypes.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Building Type Deleted Successfully!" });

            }

        }

    }
}