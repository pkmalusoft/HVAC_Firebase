using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Data;
using HVAC.DAL;
using System.Data.Entity;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class EmployeeMasterController : Controller
    {
        HVACEntities db = new HVACEntities();

        public ActionResult Home()
        {
            //
            List<int> RoleId = (List<int>)Session["RoleID"];

            int roleid = RoleId[0];

            if (roleid == 1)
            {
                var Query = (from t in db.Menus where t.IsAccountMenu.Value == false && t.IsActive == 1 && t.RoleID == null orderby t.MenuOrder select t).ToList();
                Session["Menu"] = Query;
                ViewBag.UserName = SourceMastersModel.GetUserFullName(Convert.ToInt32(Session["UserId"].ToString()), Session["UserType"].ToString());
                return View();
            }
            else
            {
                //List<Menu> Query2 = new List<Menu>();
                var Query = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.MenuID where t1.RoleID == roleid && t.IsAccountMenu.Value == false && t.IsActive == 1 orderby t.MenuOrder select t).ToList();

                var Query1 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == roleid && t.ParentID == 0 && t.IsActive == 1 && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

                var Query2 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == roleid && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

                if (Query2 != null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query2.Add(q);
                    }
                }

                if (Query1 != null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query.Add(q);
                    }
                }



                Session["Menu"] = Query;

                ViewBag.UserName = SourceMastersModel.GetUserFullName(Convert.ToInt32(Session["UserId"].ToString()), Session["UserType"].ToString());
                return View();
            }
        }

        public ActionResult Index()
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = 1; //Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            List<EmployeeVM> lst = (from c in db.EmployeeMasters
                                    join t in db.Designations on c.DesignationID
                                    equals t.DesignationID
                                    join br in db.UserInBranches on c.BranchId equals br.BranchID
                                    where c.UserID != -1 &&
                                    br.UserID == userid
                                    //   c.BranchId == branchid 
                                    select new EmployeeVM { EmployeeID = c.EmployeeID, FirstName = c.FirstName, EmployeeCode = c.EmployeeCode, Designation = t.Designation1, Email = c.Email }).ToList();
            return View(lst);
        }

        public ActionResult Create(int id = 0)
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());



            //ViewBag.Depot = data;
            ViewBag.Designation = db.Designations.ToList();
            ViewBag.roles = db.RoleMasters.ToList();

            ViewBag.Department = db.Departments.ToList();
            ViewBag.City = db.CityMasters.ToList();

            EmployeeVM v = new EmployeeVM();
            if (id == 0)
            {
                ViewBag.Title = "Create";
                v.EmployeeID = 0;
                v.JoinDate = CommonFunctions.GetCurrentDateTime();
                v.StatusActive = true;
                v.EmployeeCode = ReceiptDAO.GetMaxEmployeeCode();

            }
            else
            {
                EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
                if (a == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    v.EmployeeID = a.EmployeeID;
                    v.FirstName = a.FirstName;
                    v.LastName = a.LastName;
                    v.EmployeeCode = a.EmployeeCode;
                    v.Address1 = a.Address1;
                    v.Address2 = a.Address2;
                    v.Address3 = a.Address3;
                    v.Phone = a.Phone;
                    v.JoinDate = a.JoinDate;
                    v.CountryID = a.CountryID;
                    v.CityID = a.CityID;
                    if (v.CountryID > 0)
                    {
                        var CountryName = db.CountryMasters.Find(v.CountryID).CountryName;
                        v.CountryName = CountryName;
                    }
                    if (v.CityID > 0)
                    {
                        var cityname = db.CityMasters.Find(v.CityID).City;
                        v.CityName = cityname;
                    }
                    
                    v.DesignationID = a.DesignationID;
                    v.EmployeePrefix = a.employeeprefix;
                    v.DepartmentID = a.DepartmentID;
                    v.Email = a.Email;
                    v.RoleID = Convert.ToInt32(a.RoleID);

                    if (a.StatusActive != null)
                    {
                        v.StatusActive = Convert.ToBoolean(a.StatusActive);
                    }
                    else
                    {
                        v.StatusActive = false;
                    }

                    if (a.Approver != null)
                    {
                        v.Approver = Convert.ToBoolean(a.Approver);
                    }
                    else
                    {
                        v.Approver = false;
                    }

                    //v.CountryName=a.CountryMaster.CountryName

                    ViewBag.Title = "Modify";
                }
            }
            return View(v);
        }

       
        public JsonResult DeleteConfirmed(int id)
        {
            EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
            UserRegistration u = (from c in db.UserRegistrations where c.UserID == a.UserID select c).FirstOrDefault();
            if (a == null)
            {
                return Json(new { status = "Failed", message = "Employee Not Found!" });
            }
            else
            {
                try
                {
                    if (a != null)
                    {
                        db.EmployeeMasters.Remove(a);
                        db.SaveChanges();
                    }
                    if (u != null)
                    {
                        db.UserRegistrations.Remove(u);
                        db.SaveChanges();
                    }

                    //check branch in userinbranches
                    var userbranch = db.UserInBranches.Where(xx => xx.UserID == a.UserID).ToList();
                    db.UserInBranches.RemoveRange(userbranch);
                    db.SaveChanges();
                    TempData["SuccessMsg"] = "You have successfully Deleted Employee.";
                    return Json(new { status = "OK", message = "Employee  Deleted Successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = "Failed", message = ex.Message });
                }
            }

        }

        [HttpGet]
        public JsonResult GetEmployeeName()
        {
            var employeelist = (from c1 in db.EmployeeMasters where c1.StatusActive == true select c1.FirstName).ToList();

            return Json(new { data = employeelist }, JsonRequestBehavior.AllowGet);

        }




        public JsonResult CheckUserEmailExist(string EmailId, int UserId = 0)
        {
            string status = "true";
            //UserRegistration x = (from b in db.UserRegistrations where b.UserName == EmailId && (b.UserID != UserId || UserId == 0) select b).FirstOrDefault();
            EmployeeMaster x = (from b in db.EmployeeMasters where b.Email == EmailId && (b.EmployeeID != UserId || UserId == 0) select b).FirstOrDefault();
            if (x != null)
            {
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                status = "false";
                return Json(status, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult UserProfile()
        {
            int id = 49;
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            EmployeeVM v = new EmployeeVM();
            //ViewBag.Country = db.CountryMasters.ToList();
            ViewBag.Designation = db.Designations.ToList();
            //ViewBag.Depots = db.tblDepots.ToList();
            ViewBag.roles = db.RoleMasters.ToList();





            EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {

                v.EmployeeID = a.EmployeeID;
                v.FirstName = a.FirstName;
                v.LastName = a.LastName;
                v.EmployeeCode = a.EmployeeCode;
                v.Address1 = a.Address1;
                v.Address2 = a.Address2;
                v.Address3 = a.Address3;
                v.Phone = a.Phone;
                v.Email = a.Email;
                v.CountryID = 1;
                //     a.CountryMaster.CountryID = v.CountryID;
                // v.JoinDate = a.JoinDate.Value;


                if (a.UserID != null)
                {
                    var user = db.UserRegistrations.Where(cc => cc.UserID == a.UserID).FirstOrDefault();

                    if (user != null)
                    {
                        v.RoleID = Convert.ToInt32(user.RoleID);
                        v.Password = user.Password;
                    }
                }







                v.StatusActive = a.StatusActive.Value;
                v.UserID = a.UserID;
                int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());



            }

            return View("UserProfile", v);
            //return PartialView("_UserProfile", v);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEmployeeCode(string EmployeeName)
        {
            string status = "ok";
            string customercode = "";
            //List<CourierStatu> _cstatus = new List<CourierStatu>();
            try
            {
                string custform = "000000";
                string maxcustomercode = (from d in db.EmployeeMasters orderby d.EmployeeID descending select d.EmployeeCode).FirstOrDefault();
                string last6digit = "";
                if (maxcustomercode == null)
                {
                    //maxcustomercode="AA000000";
                    last6digit = "0";

                }
                else
                {
                    last6digit = maxcustomercode.Substring(maxcustomercode.Length - 6); //, maxcustomercode.Length - 6);
                }
                if (last6digit != "")
                {

                    string customerfirst = EmployeeName.Substring(0, 1);
                    string customersecond = "";
                    try
                    {
                        customersecond = EmployeeName.Split(' ')[1];
                        customersecond = customersecond.Substring(0, 1);
                    }
                    catch (Exception ex)
                    {

                    }

                    if (customerfirst != "" && customersecond != "")
                    {
                        customercode = customerfirst + customersecond + String.Format("{0:000000}", Convert.ToInt32(last6digit) + 1);
                    }
                    else
                    {
                        customercode = customerfirst + "E" + String.Format("{0:000000}", Convert.ToInt32(last6digit) + 1);
                    }

                }

                return Json(new { data = customercode, result = status }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                status = ex.Message;
            }

            return Json(new { data = "", result = "failed" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveEmployee(EmployeeVM v)
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            EmployeeMaster a = new EmployeeMaster();
            if (v.EmployeeID > 0)
            {
                a = db.EmployeeMasters.Find(v.EmployeeID);
                UserRegistration u1 = new UserRegistration();
                if (a.UserID > 0 && a.UserID != null)
                {
                    u1 = db.UserRegistrations.Find(a.UserID);
                }
                if (u1 == null || a.UserID == null)
                {
                    u1 = new UserRegistration();
                    int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                    u1.UserID = max1 + 1;
                    u1.UserName = v.Email;
                    u1.EmailId = v.Email;
                    u1.Password = "12345";
                    u1.Phone = v.Phone;
                    u1.IsActive = true;
                    u1.RoleID = v.RoleID;
                    db.UserRegistrations.Add(u1);
                    db.SaveChanges();
                }
                else if (u1 != null)
                {
                    u1.RoleID = v.RoleID;
                    db.Entry(u1).State = EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {

                }
                a.UserID = u1.UserID;

                //check branch in userinbranches
                var userbranch = db.UserInBranches.Where(xx => xx.UserID == a.UserID && xx.BranchID == a.BranchId).FirstOrDefault();

                //Adding default branch
                if (userbranch == null)
                {
                    UserInBranch ub1 = new UserInBranch();
                    ub1.UserID = u1.UserID;
                    ub1.BranchID = a.BranchId;
                    db.UserInBranches.Add(ub1);
                    db.SaveChanges();
                }


            }
            else if (v.EmployeeID == 0)
            {
                a.EmployeeCode = GeneralDAO.GetMaxEmployeeCode();
                a.BranchId = BranchID;
                //a.AcCompanyID = companyid;
                UserRegistration u = new UserRegistration();

                UserRegistration x = (from b in db.UserRegistrations where b.UserName == v.Email select b).FirstOrDefault();
                if (x == null)
                {

                    int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                    u.UserID = max1 + 1;
                    u.UserName = v.Email;
                    u.EmailId = v.Email;
                    u.Password = "12345";
                    u.Phone = v.Phone;
                    u.IsActive = true;
                    u.RoleID = v.RoleID;
                    db.UserRegistrations.Add(u);
                    db.SaveChanges();
                }

                a.UserID = u.UserID;

                //check branch in userinbranches
                var userbranch = db.UserInBranches.Where(xx => xx.UserID == a.UserID && xx.BranchID == a.BranchId).FirstOrDefault();

                //Adding default branch
                if (userbranch == null)
                {
                    UserInBranch ub1 = new UserInBranch();
                    ub1.UserID = u.UserID;
                    ub1.BranchID = a.BranchId;
                    db.UserInBranches.Add(ub1);
                    db.SaveChanges();
                }

            }


            a.FirstName = v.FirstName;
            a.LastName = v.LastName;

            a.Address1 = v.Address1;
            a.Address2 = v.Address2;
            a.Address3 = v.Address3;
            a.Phone = v.Phone;
            a.CountryID =v.CountryID;
            a.CityID = v.CityID;

            a.Email = v.Email;
            a.DepartmentID = v.DepartmentID;
            a.DesignationID = v.DesignationID;
            a.StatusActive = v.StatusActive;
            a.CreatedOn = CommonFunctions.GetBranchDateTime();
            a.JoinDate = Convert.ToDateTime(v.JoinDate);
            a.RoleID = v.RoleID;
            a.Approver = v.Approver;
            if (v.EmployeeID == 0)
            {
                db.EmployeeMasters.Add(a);
                db.SaveChanges();
                return Json(new { status = "OK", EmployeeID = 0, message = "Employee Added Succesfully! \n" + "Login User :" + v.Email + "\n Password: 12345" });
            }
            else
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", EmployeeID = a.EmployeeID, message = "Employee Update Succesfully!" });
            }


        }


        //public ActionResult Document(int id = 0)
        //{
        //    var job = db.EmployeeMasters.Find(id);
        //    List<DocumentMasterVM> List = new List<DocumentMasterVM>();
        //    DocumentMasterVM vm = new DocumentMasterVM();
        //    if (job != null)
        //    {

        //        vm.BranchName = job.EmployeeName;
        //        vm.AcJournalID = 0;
        //        vm.CompanyID = 0;
        //        vm.BranchID = 0;
        //        vm.EmployeeID = job.EmployeeID;

        //        List =  AccountsDAO.GetEmployeeDocument(id);
        //        vm.Details = List;
        //    }
        //    else
        //    {

        //        vm.Details = List;

        //    }
        //    ViewBag.DocumentTypes = db.DocumentTypes.ToList();
        //    return View(vm);
        //}

        //[HttpPost]
        //public ActionResult ListDocument(int id)
        //{
        //    DocumentMasterVM vm = new DocumentMasterVM();
        //    List<DocumentMasterVM> List = new List<DocumentMasterVM>();
        //    List = AccountsDAO.GetEmployeeDocument(id);
        //    vm.Details = List;
        //    return PartialView("DocumentList", vm);
        //}
        //[HttpPost]
        //public JsonResult EditDocument(int id)
        //{
        //    DocumentMasterVM vm = new DocumentMasterVM();
        //    ViewBag.DocumentTypes = db.DocumentTypes.ToList();
        //    var item = db.DocumentMasters.Find(id);
        //    if (item != null)
        //    {
        //        vm.DocumentID = item.DocumentID;
        //        vm.DocumentTypeID = item.DocumentTypeID;
        //        vm.DocumentTitle = item.DocumentTitle;
        //        vm.Filename = item.FileName;
        //    }
        //    else
        //    {
        //        item = new DocumentMaster();
        //        vm.DocumentID = 0;
        //        vm.DocumentTitle = "";
        //        vm.DocumentTypeID = 0;
        //        vm.Filename = "";
        //    }
        //    return Json(new { status = "OK", data = vm });

        //}
        //[HttpPost]
        //public ActionResult DeleteDocument(int DocumentId, int ID)
        //{
        //    int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

        //    DocumentMaster obj = db.DocumentMasters.Find(DocumentId);
        //    db.DocumentMasters.Remove(obj);
        //    db.SaveChanges();

        //    DocumentMasterVM vm = new DocumentMasterVM();
        //    List<DocumentMasterVM> List = new List<DocumentMasterVM>();
        //    List = AccountsDAO.GetEmployeeDocument(ID);
        //    vm.Details = List;

        //    return PartialView("DocumentList", vm);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDocument(int DocumentID, string DocumentTitle, int DocumentTypeID, int EmployeeID, string Filename)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int UserID = Convert.ToInt32(Session["UserID"].ToString());

            DocumentMaster model = new DocumentMaster();
            if (DocumentID == 0)
            {
                model = new DocumentMaster();
            }
            else
            {
                model = db.DocumentMasters.Find(DocumentID);
            }
            model.DocumentTypeID = DocumentTypeID;
            model.DocumentTitle = DocumentTitle;
            model.FileName = Filename;
            if (DocumentID == 0)
            {
                model.CreatedBy = UserID;
                model.CreatedDate = CommonFunctions.GetCurrentDateTime();
                model.EmployeeID = EmployeeID;

                db.DocumentMasters.Add(model);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Document Added Succesfully!" });
            }
            else
            {
                model.ModifiedDate = CommonFunctions.GetCurrentDateTime();
                model.ModifiedBy = UserID;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Document updated Succesfully!" });
            }



        }







    }


}
