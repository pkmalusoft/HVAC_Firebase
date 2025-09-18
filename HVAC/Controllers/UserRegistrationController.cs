using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
using System.Data;
using System.Data.Entity;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class UserRegistrationController : Controller
    {
        HVACEntities db = new HVACEntities();

        public ActionResult Index(int? RoleId=0)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            List<RoleMasterVM> rollist = new List<RoleMasterVM>();
            RoleMasterVM _role = new RoleMasterVM { RoleID = 0, RoleName = "Select All" };
            rollist.Add(_role);

            var roles = db.RoleMasters.ToList();
            foreach(var item in roles)
            {
                _role = new RoleMasterVM { RoleID = item.RoleID, RoleName = item.RoleName };
                rollist.Add(_role);
            }

            ViewBag.UserRole = rollist;

            List<UserRegistrationVM> _users = new List<UserRegistrationVM>();

            if (RoleId == 13 || RoleId==23 || RoleId==0)
            {
                var query = (from t in db.UserRegistrations
                             join t1 in db.RoleMasters
                             on t.RoleID equals t1.RoleID
                             join t2 in db.CustomerMasters
                             on t.UserID equals t2.UserID // into gj   from subpet in gj.DefaultIfEmpty()
                             where (t.RoleID == RoleId || RoleId == 0)
                             where (t2.BranchID == branchid)
                             select new UserRegistrationVM
                             {
                                 RoleName = t1.RoleName,
                                 UserName = t2.CustomerName,
                                 EmailId = t.EmailId,
                                 IsActive = t.IsActive.Value,
                                 UserID = t.UserID

                             }).ToList();

                _users = query;
                
            }
            
            if (RoleId ==14 || RoleId==0)
            {
                //var query = (from t in db.UserRegistrations
                //             join t1 in db.RoleMasters
                //             on t.RoleID equals t1.RoleID
                //             join t2 in db.SupplierMasters
                //             on t.UserID equals t2.UserID //into gj from subpet in gj.DefaultIfEmpty()

                //             where (t.RoleID == RoleId || RoleId == 0)
                //             where (t2.BranchID == branchid)
                //             select new UserRegistrationVM
                //             {
                //                 RoleName = t1.RoleName,
                //                 UserName = t2.Name,
                //                 EmailId = t.EmailId,
                //                 IsActive = t.IsActive.Value,
                //                 UserID = t.UserID

                //             }).ToList();

                //foreach(var item in query)
                //{
                //    _users.Add(item);
                //}
                
            }
            if (RoleId != 14 && RoleId !=13 && RoleId==0)
            {
                var query = (from t in db.UserRegistrations
                             join t1 in db.RoleMasters
                             on t.RoleID equals t1.RoleID
                             join t2 in db.EmployeeMasters
                             on t.UserID equals t2.UserID 
                             //into gj //from subpet in gj.DefaultIfEmpty()

                             where (t.RoleID == RoleId || RoleId == 0)
                             where (t2.BranchId == branchid)
                             select new UserRegistrationVM
                             {
                                 RoleName = t1.RoleName,
                                 UserName = t2.FirstName,
                                 EmailId = t.EmailId,
                                 IsActive = t.IsActive.Value,
                                 UserID = t.UserID

                             }).ToList();

                foreach (var item in query)
                {
                    _users.Add(item);
                }
            }
            ViewBag.StatusId = Convert.ToInt32(RoleId);
            return View(_users);
            //from subpet in gj.DefaultIfEmpty()
            //join pet1 in db.CustomerMasters on t.UserID equals pet1.CourierStatusID into gj1



        }


        public ActionResult Create(int id=0)
        {
            if (id == 0) //create mode
                ViewBag.UserRole = db.RoleMasters.Where(cc => cc.RoleID == 13 || cc.RoleID == 14 || cc.RoleID==23 ).ToList();
            else
                ViewBag.UserRole = db.RoleMasters.ToList();

            UserRegistrationVM v = new UserRegistrationVM();
            
            ViewBag.Branch = db.BranchMasters.ToList();
            if (id==0)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                v.UserID = 0;
                v.IsActive = true;
                v.DefaultBranchId = branchid;
                ViewBag.EditMode = "false";
                ViewBag.Title = "Create";
                v.Details = new List<UserBranchVM>();
                ViewBag.DefaultBranch = new List<BranchMaster>();

                return View(v);
            }
            else
            {
                ViewBag.Title = "Modify";
                var Branch = (from c in db.BranchMasters join d in db.UserInBranches on c.BranchID equals d.BranchID where d.UserID==id select c).ToList();
                if (Branch==null)
                {
                    ViewBag.DefaultBranch = new List<BranchMaster>();
                }
                else
                {
                    ViewBag.DefaultBranch = Branch;
                }
                var a = (from c in db.UserRegistrations where c.UserID == id select c).FirstOrDefault();
                if (a == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    v.UserID = a.UserID;
                    v.RoleID = a.RoleID.Value;
                    v.EmailId = a.EmailId;
                    //v.DefaultBranchId =a.
                    if (v.RoleID == 13)
                    {
                        var User = db.CustomerMasters.Where(cc => cc.UserID == v.UserID).FirstOrDefault();
                        if (User != null)
                        {
                            v.UserName = User.CustomerName;
                            v.UserReferenceId = User.CustomerID;
                            v.DefaultBranchId = Convert.ToInt32(User.BranchID);
                            if (v.DefaultBranchId > 0)
                            {
                                var branchname = db.BranchMasters.Find(v.DefaultBranchId).BranchName;
                                v.DefaultBranchName = branchname;
                            }
                        }
                        else
                        {
                            v.UserName = "";
                            v.UserReferenceId = 0;
                        }

                    }
                    
                    else
                    {
                        var employee = db.EmployeeMasters.Where(cc => cc.UserID == v.UserID).FirstOrDefault();
                        if (employee!=null)
                        {
                            v.UserName = employee.FirstName;
                            v.UserReferenceId = employee.EmployeeID;
                            if (employee.BranchId != null)
                            {
                                v.DefaultBranchId = Convert.ToInt32(employee.BranchId);
                                if (v.DefaultBranchId > 0)
                                {
                                    var branchname = db.BranchMasters.Find(v.DefaultBranchId).BranchName;
                                    v.DefaultBranchName = branchname;
                                }
                            }

                        }
                        else { v.UserName = "";
                            v.UserReferenceId = 0;
                        }

                    }
                    
                    v.Password = a.Password;
                    v.IsActive = true;
                    var userbranch = db.UserInBranches.Where(u => u.UserID == a.UserID && u.BranchID != v.DefaultBranchId).ToList();

                    if (userbranch != null)
                    {
                        foreach (var item in userbranch)
                        {
                            if (v.BranchId == "" || v.BranchId == null)
                            {
                                v.BranchId = item.BranchID.ToString();
                            }
                            else
                            {
                                v.BranchId = v.BranchId + "," + item.BranchID.ToString();
                            }
                        }
                    }
                     

                    

                    var list = (from c in db.UserInBranches join c2 in db.BranchMasters on c.BranchID equals c2.BranchID where c.UserID == id select new UserBranchVM { UserBranchID = c.UserBranchID, UserID = c.UserID, BranchID = c.BranchID, BranchName = c2.BranchName }).ToList();
                    if (list != null)
                        v.Details = list;
                    else
                        v.Details = new List<UserBranchVM>();

                    ViewBag.EditMode = "true";
                    return View(v);

                }
              
            }
           
        }

        [HttpPost]
        public ActionResult Create(UserRegistrationVM v)
        {

            if (v.UserID == 0)
            {

                string status = "true";
                UserRegistration x = (from b in db.UserRegistrations where b.UserName == v.EmailId select b).FirstOrDefault();
                if (x != null)
                {
                    TempData["ErrorMsg"] = "Email Id already exist!";
                    ViewBag.UserRole = db.RoleMasters.ToList();
                    return View(v);                    
                }
                
                UserRegistration a = new UserRegistration();
                int max = (from c in db.UserRegistrations orderby c.UserID descending select c.UserID).FirstOrDefault();


                a.UserID = 0;
                a.UserName = v.EmailId;
                a.Password = v.Password;
                a.RoleID = v.RoleID;
                 a.Phone = "";
                a.EmailId = v.EmailId;
                a.IsActive = true; ;
                //string userid=AWBDAO.SaveUserRegistration(a);
                //a.UserID = Convert.ToInt32(userid);

                db.UserRegistrations.Add(a);
                db.SaveChanges();

               
                    var employee = db.EmployeeMasters.Find(v.UserReferenceId);
                    if (employee != null)
                    {
                        employee.UserID = a.UserID;
                        if (v.DefaultBranchId!=null && v.DefaultBranchId!=0)
                            employee.BranchId = v.DefaultBranchId;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

              

                var userbranch = db.UserInBranches.Where(u => u.UserID == a.UserID).ToList();
                db.UserInBranches.RemoveRange(userbranch);
                db.SaveChanges();
                if (v.Details != null)
                {
                    foreach (var item in v.Details)
                    {
                        if (item.IsDeleted == false)
                        {
                            UserInBranch ub = new UserInBranch();
                            ub.UserID = a.UserID;
                            ub.BranchID = item.BranchID;
                            db.UserInBranches.Add(ub);
                            db.SaveChanges();
                        }
                    }
                }

                if (v.EmailNotify == true)
                {
                    EmailDAO _emaildao = new EmailDAO();
                    _emaildao.SendCustomerEmail(v.EmailId, v.UserName, v.Password);
                    TempData["SuccessMsg"] = "You have successfully added User and Notification Mail has sent!";
                }
                else
                {
                    TempData["SuccessMsg"] = "You have successfully added User.";
                }
                
            }
            else
            {
                //UserRegistration uv = db.UserRegistrations.Find(v.UserID);
                var uv = db.UserRegistrations.Find(v.UserID);//  (from c in db.UserRegistrations where c.UserID == v.UserID select c).FirstOrDefault();
                //UserRegistration a = new UserRegistration();
                //a.UserID = v.UserID;
                uv.UserName = v.EmailId;
                if (v.Password != null)
                {
                    if (v.Password != uv.Password)
                    {
                        uv.Password = v.Password;
                    }
                }
                //uv.RoleID = v.RoleID;
                
                uv.EmailId = v.EmailId;
                uv.IsActive = true;                
                db.Entry(uv).State = EntityState.Modified;
                db.SaveChanges();

                if (uv.RoleID == 13) //customer
                {
                    var customer = db.CustomerMasters.Find(v.UserReferenceId);
                    if (customer != null)
                    {
                        customer.UserID = uv.UserID;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                
                else
                {
                    var employee = db.EmployeeMasters.Find(v.UserReferenceId);
                    if (employee != null)
                    {
                        employee.UserID = uv.UserID;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                var userbranch = db.UserInBranches.Where(u => u.UserID == uv.UserID).ToList();
                db.UserInBranches.RemoveRange(userbranch);
                db.SaveChanges();
                //Adding default branch
                UserInBranch ub1 = new UserInBranch();
                ub1.UserID = uv.UserID;
                ub1.BranchID = v.DefaultBranchId;
                db.UserInBranches.Add(ub1);
                db.SaveChanges();

                if (v.Details != null)
                {
                    foreach (var item in v.Details)
                    {
                        if (item.IsDeleted == false)
                        {
                            var ub2 = db.UserInBranches.Where(cc => cc.UserID == uv.UserID && cc.BranchID == item.BranchID).FirstOrDefault();
                            if (ub2 == null)
                            {

                                UserInBranch ub = new UserInBranch();
                                ub.UserID = uv.UserID;
                                ub.BranchID = item.BranchID;
                                db.UserInBranches.Add(ub);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                if (v.EmailNotify == true)
                {
                    EmailDAO _emaildao = new EmailDAO();
                    _emaildao.SendCustomerEmail(v.EmailId, v.UserName, v.Password);
                    TempData["SuccessMsg"] = "You have successfully Updated User Detail and Notification Mail has sent.";
                }
                else
                {
                    TempData["SuccessMsg"] = "You have successfully Updated User.";
                }
                

            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {

            ViewBag.UserRole = db.RoleMasters.ToList();
            UserRegistrationVM v = new UserRegistrationVM();

            var  a = (from c in db.UserRegistrations where c.UserID == id select c).FirstOrDefault();
            if (a == null)
            {

                return HttpNotFound();
            }
            else
            {
                v.UserID = a.UserID;
                v.UserName = a.UserName;
                v.Phone = a.Phone;
                v.EmailId = a.EmailId;
                v.RoleID = a.RoleID.Value;
                v.Password = a.Password;
                v.IsActive = a.IsActive.Value;

            }
            return View(v);
        }


        public JsonResult GetRandomPassword()
        {
            GeneralDAO _dao = new GeneralDAO();
            string passw = _dao.RandomPassword(6);
            return Json(new { data = passw}, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetUserEmail(string username,int roleid)
        {
            string emailid = "";
            bool pstatus = false;
            int UserReferenceId = 0;
            string message = "";
             
                var _employee = db.EmployeeMasters.Where(cc => cc.Email == username).FirstOrDefault();
                if (_employee != null)
                {
                    emailid = _employee.Email;
                    UserReferenceId = _employee.EmployeeID;
                    pstatus = true;
                }
                else
                {
                    message = "Employee name not found!";
                }
        
            
            return Json(new {status=pstatus, data = emailid,refid=UserReferenceId, message=message }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit(UserRegistrationVM v)
        {
            UserRegistration a = new UserRegistration();
            a.UserID = v.UserID;
            a.UserName = v.UserName;
            a.Password = v.Password;
            a.RoleID = v.RoleID;
            a.Phone = v.Phone;
            a.EmailId = v.EmailId;
            a.IsActive = v.IsActive;

            if (ModelState.IsValid)
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated User.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            UserRegistration a = (from c in db.UserRegistrations where c.UserID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (a.RoleID==13)
                {
                    var customer = db.CustomerMasters.Where(cc => cc.UserID == a.UserID).FirstOrDefault();
                    if (customer!=null)
                    {
                        customer.UserID = null;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }
                else if(a.RoleID==13 && a.RoleID!=14) //not customer and not agent
                {
                    var employee = db.EmployeeMasters.Where(cc => cc.UserID == a.UserID).FirstOrDefault();
                    if (employee!=null)
                    {
                        employee.UserID = null;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

                db.UserRegistrations.Remove(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Deleted User.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult GetCustomerName(string term )
        {
            if (term.Trim() == "")
            {
                var customerlist = (from c1 in db.CustomerMasters where c1.CustomerType == "CR"  && c1.UserID==null orderby c1.CustomerName select new { ID = c1.CustomerID, UserName = c1.CustomerName , Email=c1.Email}).ToList();

                //return Json(new { data = customerlist }, JsonRequestBehavior.AllowGet);
                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customerlist = (from c1 in db.CustomerMasters where c1.CustomerType == "CR" && c1.UserID==null && c1.CustomerName.ToLower().StartsWith(term.ToLower()) orderby c1.CustomerName select new { ID = c1.CustomerID, UserName = c1.CustomerName,Email=c1.Email }).ToList();

                //return Json(new { data = customerlist }, JsonRequestBehavior.AllowGet);
                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }

        }
        
       
     

    }
}
