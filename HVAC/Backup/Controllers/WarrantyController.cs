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
    public class WarrantyController : Controller
    {
         HVACEntities db = new HVACEntities();

      
        public ActionResult Index()
        {
            List<Warranty> lst = new List<Warranty>();
            var data = db.Warranties.OrderBy(cc=>cc.WarrantyType).ToList();
            
            return View(data);
        }


        public ActionResult Create(int id = 0)
        {


            Models.Warranty obj = new Models.Warranty();
            if (id == 0)
            {
               // ViewBag.Title = "EntityType - CREATE";
                obj.WarrantyID = 0;


            }
            else
            {
                //ViewBag.Title = "Warranty - Modify";
                Models.Warranty type = (from c in db.Warranties where c.WarrantyID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.WarrantyID = type.WarrantyID;
                    obj.WarrantyType = type.WarrantyType;


                }
            }
            return View(obj);
        }

        public JsonResult WarrantyExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.Warranties where b.WarrantyType == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.Warranties where b.WarrantyType == PType && b.WarrantyID != ID select b).FirstOrDefault();
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
        public JsonResult SaveWarranty(Models.Warranty ProType)
        {

            Models.Warranty Ptype = new Models.Warranty();

            if (ProType.WarrantyID == 0)
            {

                int max = (from c in db.Warranties orderby c.WarrantyID descending select c.WarrantyID).FirstOrDefault();

                if (max == null)
                {
                    Ptype.WarrantyID = 1;
                    Ptype.WarrantyType = ProType.WarrantyType;



                }
                else
                {
                    Ptype.WarrantyID = max + 1;
                    Ptype.WarrantyType = ProType.WarrantyType;



                }
                db.Warranties.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Warranty Addded Succesfully!" });
            }
            else
            {
                var item = db.Warranties.Where(cc => cc.WarrantyID == ProType.WarrantyID && cc.WarrantyID != ProType.WarrantyID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "Warranty already Exist!" });

                }

                Ptype = db.Warranties.Find(ProType.WarrantyID);
                Ptype.WarrantyType = ProType.WarrantyType;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Warranty Updated Succesfully!" });
            }







        }



        public ActionResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = MasterDAO.DeleteWarranty(id);
                
              
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