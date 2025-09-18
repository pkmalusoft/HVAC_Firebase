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
    public class EnquiryStatusController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.EnquiryStatu> Protypes = db.EnquiryStatus.Where(temp => temp.EnqStatusName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.EnquiryStatu obj = new Models.EnquiryStatu();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.EnqStatusID = 0;


            }
            else
            {
                ViewBag.Title = "EnquiryStage - Modify";
                Models.EnquiryStatu type = (from c in db.EnquiryStatus where c.EnqStatusID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.EnqStatusID = type.EnqStatusID;
                    obj.EnqStatusName = type.EnqStatusName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.EnquiryStatus where b.EnqStatusName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.EnquiryStatus where b.EnqStatusName == PType && b.EnqStatusID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProjectType(Models.EnquiryStatu ProType)
        {

            Models.EnquiryStatu Ptype = new Models.EnquiryStatu();

            if (ProType.EnqStatusID == 0)
            {

                int max = (from c in db.EnquiryStatus orderby c.EnqStatusID descending select c.EnqStatusID).FirstOrDefault();

                if (max == null)
                {
                    Ptype.EnqStatusID = 1;
                    Ptype.EnqStatusName = ProType.EnqStatusName;
                  


                }
                else
                {
                    Ptype.EnqStatusID = max + 1;
                    Ptype.EnqStatusName = ProType.EnqStatusName;
                   


                }
                db.EnquiryStatus.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "EnquiryStatus Addded Succesfully!" });
            }
            else
            {
                var item = db.EnquiryStatus.Where(cc => cc.EnqStatusID == ProType.EnqStatusID && cc.EnqStatusID != ProType.EnqStatusID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "EnquiryStatus already Exist!" });

                }

                Ptype = db.EnquiryStatus.Find(ProType.EnqStatusID);
                Ptype.EnqStatusName = ProType.EnqStatusName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "EnquiryStatus Updated Succesfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.EnquiryStatu protype = db.EnquiryStatus.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.EnquiryStatus.Where(cc => cc.EnqStatusID == id).ToList();
                db.EnquiryStatus.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Enquiry status Deleted Successfully!" });

            }

        }

    }
}