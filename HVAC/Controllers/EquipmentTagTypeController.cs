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
    public class EquipmentTagTypeController : Controller
    {
         HVACEntities db = new HVACEntities();

      
        public ActionResult Index()
        {
            List<EquipmentTagTypeController> lst = new List<EquipmentTagTypeController>();
            var data = db.EquipmentTagTypes.OrderBy(cc=>cc.EquipmentTagType1).ToList();
            
            return View(data);
        }

      
        //
        // GET: /TypeOfGood/Create

        public ActionResult Create(int id=0)
        {
            EquipmentTagType model = new EquipmentTagType();
            if (id==0)
            {
                ViewBag.Title = "Create";
                return View(model);
            }
            else
            {
                ViewBag.Title = "Modify";
                model =db.EquipmentTagTypes.Find(id);
                return View(model);
            }
            
        }

        //
        // POST: /TypeOfGood/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipmentTagType v)
        {

            EquipmentTagType t = new EquipmentTagType();

           
            if (v.ID ==0)
            {
                db.EquipmentTagTypes.Add(v);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added EquipmentTagType";
                return RedirectToAction("Index");
            }
            else
            {
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated EquipmentTagType";
                return RedirectToAction("Index");
            }
                 
        }
 
        
    
  
        public ActionResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = MasterDAO.DeleteEquipmentTagType(id);
                
              
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