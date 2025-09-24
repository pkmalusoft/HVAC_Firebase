using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Data;
using System.Data.Entity;

namespace HVAC.Controllers
{
    public class RoleController : Controller
    {

        HVACEntities db = new HVACEntities();

        public ActionResult Index()
        {
            List<RoleMasterVM> lst = new List<RoleMasterVM>();

            var data = db.RoleMasters.ToList();
            foreach (var item in data)
            {
                RoleMasterVM v = new RoleMasterVM();
                v.RoleID = item.RoleID;
                v.RoleName = item.RoleName;
                v.Type = item.Type;
                lst.Add(v);
            }
            return View(lst);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleMasterVM v)
        {
            if (ModelState.IsValid)
            {
                RoleMaster a = new RoleMaster();
                
                int max=(from c in db.RoleMasters orderby c.RoleID descending select c.RoleID).FirstOrDefault();

               
                    a.RoleID = max+1;
                    a.RoleName = v.RoleName;
                    a.Type = v.Type; 

                db.RoleMasters.Add(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Role.";

            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            RoleMasterVM v = new RoleMasterVM();
            var data = (from c in db.RoleMasters where c.RoleID == id select c).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.RoleID = data.RoleID;
                v.RoleName = data.RoleName;
                v.Type = data.Type;
            }
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoleMasterVM v)
        {
            RoleMaster a = new RoleMaster();
            a.RoleID = v.RoleID;
            a.RoleName = v.RoleName;
            a.Type = v.Type;
            if (ModelState.IsValid)
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMSG"] = "You have successfully Updated Role.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult DeleteConfirmed(int id)
        {
            RoleMaster role = db.RoleMasters.Find(id);
            db.RoleMasters.Remove(role);
            db.SaveChanges();
            TempData["SuccessMSG"] = "You have successfully Deleted Role.";
            return RedirectToAction("Index");
        }
    }
}
