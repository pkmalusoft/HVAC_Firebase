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
    public class EnquiryStageController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.EnquiryStage> Protypes = db.EnquiryStages.Where(temp => temp.EnqStageName.Contains(SearchText)).ToList();
             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.EnquiryStage obj = new Models.EnquiryStage();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.EnqStageID = 0;

                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.EnquiryStage type = (from c in db.EnquiryStages where c.EnqStageID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.EnqStageID = type.EnqStageID;
                    obj.EnqStageName = type.EnqStageName;
                 

                }
            }
            return View(obj);
        }

        public JsonResult ProjectTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.EnquiryStages where b.EnqStageName == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.EnquiryStages where b.EnqStageName == PType && b.EnqStageID != ID select b).FirstOrDefault();
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
        public JsonResult SaveProjectType(Models.EnquiryStage ProType)
        {

            Models.EnquiryStage Ptype = new Models.EnquiryStage();

            if (ProType.EnqStageID == 0)
            {

                var duplicate = (from c in db.EnquiryStages where c.EnqStageName==ProType.EnqStageName select c).FirstOrDefault();

                if (duplicate !=null)
                {
                    Ptype.EnqStageID = 1;
                    return Json(new { status = "Failed", message = "Duplicate Enquiry Stage Not allowed!" });

                }
                Ptype.EnqStageName = ProType.EnqStageName;                
                db.EnquiryStages.Add(Ptype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Enquiry Stage Added Successfully!" });
            }
            else
            {
                var duplicate = (from c in db.EnquiryStages where c.EnqStageName == ProType.EnqStageName && c.EnqStageID!= ProType.EnqStageID select c).FirstOrDefault();
                 
                if (duplicate != null)
                {
                    return Json(new { status = "Failed", message = "Enquiry Stage already Exist!" });

                }

                Ptype = db.EnquiryStages.Find(ProType.EnqStageID);
                Ptype.EnqStageName = ProType.EnqStageName;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Enquiry Stage Updated Successfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.EnquiryStage protype = db.EnquiryStages.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var _enquiry = db.Enquiries.Where(cc => cc.EnquiryStageID == id).FirstOrDefault();
                if (_enquiry!=null)
                {
                    return Json(new { status = "Failed", message = "Enquiry Stage referenced in the Enquiry,Could not Delete!" });
                }
                var protypes = db.EnquiryStages.Where(cc => cc.EnqStageID == id).ToList();
                db.EnquiryStages.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Enquiry Stage Deleted Successfully!" });

            }

        }

    }
}