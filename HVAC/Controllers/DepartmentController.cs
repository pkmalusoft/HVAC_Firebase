using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;

namespace HVAC.Controllers
{
    public class DepartmentController : Controller
    {
         HVACEntities db = new HVACEntities();

        
        public ActionResult Index()
        {
            List<DepartmentVM> lst = new List<DepartmentVM>();

            var data = db.Departments.ToList();

            foreach (var item in data)
            {
                DepartmentVM d = new DepartmentVM();
                d.DepartmentID = item.DepartmentID;
                d.Department = item.Department1;
                d.AcCompanyID = item.AcCompanyID.Value;
                if (item.UserID != null)
                {
                    d.UserID = item.UserID.Value;
                }

               
                d.StatusDefaultID = item.StatusDefaultD;
               
                lst.Add(d);
            }

           
            
            return View(lst);
        }

        //
        // GET: /Department/Details/5

        public ActionResult Details(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Department/Create

        [HttpPost]
        public ActionResult Create(DepartmentVM v)
        {
            if (ModelState.IsValid)
            {

                Department d = new Department();

                int max = (from c in db.Departments orderby c.DepartmentID descending select c.DepartmentID).FirstOrDefault();

                if (max == null)
                {
                    d.DepartmentID = 1;
                    d.Department1 = v.Department;
                    d.AcCompanyID = v.AcCompanyID;
                    d.UserID = v.UserID;
                   
                    d.StatusDefaultD = v.StatusDefaultID;
                  
                }
                else
                {
                    d.DepartmentID = max + 1;
                    d.Department1 = v.Department;
                    d.AcCompanyID = v.AcCompanyID;
                    d.UserID = v.UserID;
                   
                    d.StatusDefaultD = v.StatusDefaultID;
                    
                
                }



                db.Departments.Add(d);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Department.";
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int id)
        {

            DepartmentVM c = new DepartmentVM();
            var data = (from d in db.Departments where d.DepartmentID == id select d).FirstOrDefault();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                c.DepartmentID = data.DepartmentID;
                c.Department = data.Department1;
                c.AcCompanyID = data.AcCompanyID.Value;
                c.UserID = data.UserID.Value;
               
                c.StatusDefaultID = data.StatusDefaultD;
                

            }



            return View(c);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        public ActionResult Edit(DepartmentVM v)
        {
            if (ModelState.IsValid)
            {
                Department c = new Department();
                c.DepartmentID = v.DepartmentID;
                c.Department1 = v.Department;
                c.AcCompanyID = v.AcCompanyID;
                c.UserID = v.UserID;
                
                c.StatusDefaultD= v.StatusDefaultID;
              

                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Upadated Department.";
                return RedirectToAction("Index");
            }
            return View();
        }

   

     
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Department.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}