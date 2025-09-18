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
    public class EnquirySourceController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.EnquirySource> Protypes = db.EnquirySources.Where(temp => temp.EnquirySourceName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.EnquirySource obj = new Models.EnquirySource();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.EnquirySourceID = 0;


            }
            else
            {
                ViewBag.Title = "EnquirySource - Modify";
                Models.EnquirySource type = (from c in db.EnquirySources where c.EnquirySourceID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.EnquirySourceID = type.EnquirySourceID;
                    obj.EnquirySourceName = type.EnquirySourceName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.EnquirySources where b.EnquirySourceName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.EnquirySources where b.EnquirySourceName == PType && b.EnquirySourceID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProjectType(Models.EnquirySource ProType)
        {

            Models.EnquirySource Ptype = new Models.EnquirySource();

            if (ProType.EnquirySourceID== 0)
            {


                Ptype.EnquirySourceName = ProType.EnquirySourceName;
                  
                db.EnquirySources.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "EnquirySource Addded Succesfully!" });
            }
            else
            {
                var item = db.EnquirySources.Where(cc => cc.EnquirySourceName == ProType.EnquirySourceName && cc.EnquirySourceID != ProType.EnquirySourceID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "EnquirySource already Exist!" });

                }

                Ptype = db.EnquirySources.Find(ProType.EnquirySourceID);
                Ptype.EnquirySourceName = ProType.EnquirySourceName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "EnquirySource Updated Succesfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.EnquirySource protype = db.EnquirySources.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.EnquirySources.Where(cc => cc.EnquirySourceID== id).ToList();
                db.EnquirySources.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Enquiry Source Deleted Successfully!" });

            }

        }

    }
}