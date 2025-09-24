using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.DAL;
using HVAC.Models;

namespace HVAC.Controllers
{
    public class QuotationStatusController : Controller
    {
         HVACEntities db = new HVACEntities();

      
        public ActionResult Index()
        {
            List<QuotationStatu> lst = new List<QuotationStatu>();
            var data = db.QuotationStatus.OrderBy(cc=>cc.Status).ToList();
            
            return View(data);
        }


        public ActionResult Create(int id = 0)
        {


            Models.QuotationStatu obj = new Models.QuotationStatu();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.ID = 0;


            }
            else
            {
                //   ViewBag.Title = "BriefScope - Modify";
                Models.QuotationStatu type = (from c in db.QuotationStatus where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.ID = type.ID;
                    obj.Status = type.Status;


                }
            }
            return View(obj);
        }

        public JsonResult CheckQuotationStatusExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.QuotationStatus where b.Status == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.QuotationStatus where b.Status == PType && b.ID != ID select b).FirstOrDefault();
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
        public JsonResult SaveQuotationStatus(Models.QuotationStatu ProType)
        {

            Models.QuotationStatu Ptype = new Models.QuotationStatu();

            if (ProType.ID == 0)
            {                  
                Ptype.Status = ProType.Status;
                db.QuotationStatus.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "QuotationStatus Addded Succesfully!" });
            }
            else
            {
                var item = db.QuotationStatus.Where(cc => cc.ID == ProType.ID && cc.ID != ProType.ID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "QuotationStatus already Exist!" });

                }

                Ptype = db.QuotationStatus.Find(ProType.ID);
                Ptype.Status = ProType.Status;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "QuotationStatus Updated Succesfully!" });
            }
                    }




        public ActionResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = MasterDAO.DeleteQuotationStatus(id);
                
              
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            status = dt.Rows[0][0].ToString();
                            message = dt.Rows[0][1].ToString();
                            //TempData["ErrorMsg"] = "Transaction Exists. Deletion Restricted !";
                            return Json(new { status = status, message = message });
                        }

                    }
                    else
                    {
                        //TempData["SuccessMsg"] = "You have successfully Deleted Cost !!";
                        return Json(new { status = "Failed", message = "Delete Failed!" });
                    }
                }
                else
                {
                    //TempData["SuccessMsg"] = "You have successfully Deleted Cost !!";
                    return Json(new { status = "Failed", message = "Delete Failed!" });
                }
            }

            return Json(new { status = "Failed", message = "Delete Failed!" });
          
          
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}