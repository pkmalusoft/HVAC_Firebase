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
    public class BondTypeController : Controller
    {
         HVACEntities db = new HVACEntities();

      
        public ActionResult Index(string SearchText = "")
        {
            List<Models.BondType> Protypes = db.BondTypes.Where(temp => temp.BondType1.Contains(SearchText)).ToList();
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {


            Models.BondType obj = new Models.BondType();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.ID = 0;


            }
            else
            {
              //  ViewBag.Title = "BondType - Modify";
                Models.BondType type = (from c in db.BondTypes where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.ID = type.ID;
                    obj.BondType1 = type.BondType1;


                }
            }
            return View(obj);
        }

        public JsonResult BondTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.BondTypes where b.BondType1 == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.BondTypes where b.BondType1 == PType && b.ID != ID select b).FirstOrDefault();
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
        public JsonResult SaveBondType(Models.BondType ProType)
        {

            Models.BondType Ptype = new Models.BondType();

            if (ProType.ID == 0)
            {
                 Ptype.BondType1 = ProType.BondType1;
                db.BondTypes.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "BondType Addded Succesfully!" });
            }
            else
            {
                var item = db.BondTypes.Where(cc => cc.ID == ProType.ID && cc.ID != ProType.ID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "BondType already Exist!" });

                }

                Ptype = db.BondTypes.Find(ProType.ID);
                Ptype.BondType1 = ProType.BondType1;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "BondType Updated Succesfully!" });
            }







        }


        public ActionResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = MasterDAO.DeleteBondType(id);
                
              
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