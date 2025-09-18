using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
using System.Web.Security;
using System.Data.SqlTypes;
using System.Data.Entity;

namespace HVAC.Controllers
{
    public class LoginController : Controller
    {
        HVACEntities db = new HVACEntities();

        public ActionResult Login()
        {
            var compdetail = db.AcCompanies.FirstOrDefault();            
            ViewBag.CompanyName = compdetail.AcCompany1;
            string userName = string.Empty;

            if (System.Web.HttpContext.Current != null &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                System.Web.Security.MembershipUser usr = Membership.GetUser();
                if (usr != null)
                {
                    userName = usr.UserName;
                }
            }

            UserLoginVM vm = new UserLoginVM();
            
            vm.UserName = userName;
            //ViewBag.Depot = db.tblDepots.ToList();
            //ViewBag.fyears = db.AcFinancialYearSelect().ToList();
            //TempData["SuccessMsg"] = "You have successfully Updated Customer.";
            //ViewBag.ErrorMessage = "not working";
            //var compdetail = db.AcCompanies.FirstOrDefault();
            //Session["CurrentCompanyID"] = compdetail.AcCompanyID;

            //Session["CompanyName"] = compdetail.AcCompany1;
            //ViewBag.CompanyName = compdetail.AcCompany1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Login(UserLoginVM u)
        {
            int proleid = 0;
            int userid = 0;
            string roletype = "";
            int currentyear = DateTime.Now.Year;
            List<int> rolelist = new List<int>();
            UserRegistration u1 = null;
            try
            {
                u1 = (from c in db.UserRegistrations where c.UserName == u.UserName && c.Password == u.Password select c).FirstOrDefault();
                if (u1 != null)
                {
                    userid = u1.UserID;
                    if (u1.RoleID == 14)
                    {
                        proleid = 14;
                        roletype = "Agent";
                        Session["CurrentDepot"] = "";
                    }
                    else if (u1.RoleID == 13 || u1.RoleID == 23)
                    {

                        proleid = Convert.ToInt32(u1.RoleID);
                        if (u1.RoleID == 13)
                            roletype = "Customer";
                        else
                            roletype = "CoLoader";
                        var custdetail = (from u2 in db.CustomerMasters where u2.UserID == userid select u2).FirstOrDefault();
                        //var custdetail = (from u2 in db.UserRegistrations join c1 in db.CustomerMasters on u2.UserID equals c1.UserID join d1 in db.tblDepots on c1.DepotID equals d1.ID where u1.UserID == userid select new { Customerid = c1.CustomerID, DepotId = d1.ID }).FirstOrDefault();
                        if (custdetail == null)
                        {
                            TempData["SuccessMsg"] = "Contact Administrator, Login is not linked with Customer!";
                            return RedirectToAction("Login", "Login");
                        }
                        else
                        {
                            Session["CustomerId"] = custdetail.CustomerID;
                            
                            int? branchid = custdetail.BranchID;
                            Session["CurrentBranchID"] = branchid;
                            
                            var branch = db.BranchMasters.Where(cc => cc.BranchID == branchid).FirstOrDefault();
                            Session["CurrentDepot"] = branch.BranchName;

                        }

                    }
                    else
                    {
                        proleid = Convert.ToInt32(u1.RoleID);
                        roletype = "Employee";

                        int? branchid = 1; 
                        Session["CurrentBranchID"] = branchid;
                        
                        var branch = db.BranchMasters.Where(cc => cc.BranchID == branchid).FirstOrDefault();
                        Session["CurrentDepot"] = branch.BranchName;

                    }

                }
                else
                {
                    //TempData["ErrorMsg"] = "User does not exists!";
                    //TempData["Modal"] = "Login";
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "User does not exists!";
                    //return RedirectToAction("Login");
                    return RedirectToAction("Home", "Home");
                }


                //User and role Setting
                rolelist.Add(proleid);
                Session["RoleID"] = rolelist;
                Session["UserRoleID"] = rolelist[0];
                Session["UserID"] = u1.UserID;
                Session["UserName"] = u1.UserName;
                //Session["CurrentBranchID"] = u.BranchID;                                                          

                var compdetail = db.AcCompanies.FirstOrDefault();
                Session["CurrentCompanyID"] = compdetail.AcCompanyID;
               
                Session["CompanyName"] = compdetail.AcCompany1;
                Session["EnableCashCustomerInvoice"] = compdetail.EnableCashCustomerInvoice;
                Session["EnableAPI"] = compdetail.EnableAPI;
                Session["AWBAlphaNumeric"] = compdetail.AWBAlphaNumeric;
                Session["CompanyAddress"] = compdetail.Address1 + "," + compdetail.Address2 + " " + compdetail.Address3 + compdetail.CityName + " " + compdetail.CountryName;
                //int accid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
               

                if (Session["CurrentBranchID"] == null)
                {
                    Session["CurrentBranchID"] = db.BranchMasters.FirstOrDefault().BranchID;
                }

                //var alldepot = db.tblDepots.Where(cc=>cc.BranchID!=null).OrderBy(cc => cc.Depot).ToList();
                var alldepot = (from c in db.BranchMasters join e in db.UserInBranches on c.BranchID equals e.BranchID where e.UserID == u1.UserID select c).ToList(); // db.tblDepots.Where(cc => cc.BranchID != null).OrderBy(cc => cc.Depot).ToList();
                if (alldepot == null)
                {
                    //if (Session["CurrentBranchID"] != null)
                    //{
                    //    int currentbranchid1 = alldepot[0].BranchID;//  Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    //    alldepot = db.BranchMasters.Where(cc => cc.BranchID == currentbranchid1).ToList();
                    //    Session["CurrentBranchID"] = currentbranchid1;
                    //}


                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Branch is not valid!";
                    //TempData["ErrorMsg"] = "Financial Year Selection not valid!";
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    int currentbranchid2 = alldepot[0].BranchID;//  Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    //alldepot = db.BranchMasters.Where(cc => cc.BranchID == currentbranchid2).ToList();
                    Session["CurrentBranchID"] = currentbranchid2;
                    Session["CurrentDepot"] = alldepot[0].BranchName;
                }

                Session["Depot"] = alldepot;
                //currency year setting
                Session["CurrencyId"] = alldepot[0].CurrencyID;
                Session["EXRATE"] = 1;
                int currencyid = alldepot[0].CurrencyID.Value; //  (from c in db.AcCompanies where c.AcCompanyID == accid select c.CurrencyID).FirstOrDefault().Value;
                var currency = (from c in db.CurrencyMasters where c.CurrencyID == currencyid select c).FirstOrDefault();
                short? noofdecimals = currency.NoOfDecimals;
                string monetaryunit = currency.MonetaryUnit;

                Session["Decimal"] = noofdecimals;
                Session["MonetaryUnit"] = monetaryunit;
                if (currency.NumberFormat == 1)
                   Session["NumberFormat"] = "Lakhs";
                else
                    Session["NumberFormat"] = "Millions";
                ////Year Setting
                ///

                int currentbranchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                int startyearid = Convert.ToInt32(db.BranchMasters.Find(currentbranchid).AcFinancialYearID);
                if (startyearid == 0)
                {
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Financial Year Selection not valid!";
                    //TempData["ErrorMsg"] = "Financial Year Selection not valid!";
                    return RedirectToAction("Home", "Home");

                }
                DateTime branchstartdate;
                List<AcFinancialYear> allyear = new List<AcFinancialYear>();
                AcFinancialYear finacialyear;


                if (roletype == "Employee")
                {
                    branchstartdate = Convert.ToDateTime(db.AcFinancialYears.Find(startyearid).AcFYearFrom);
                    //allyear = (from c in db.AcFinancialYears where c.BranchID == currentbranchid && c.AcFYearFrom >= branchstartdate select c).OrderByDescending(cc => cc.AcFYearFrom).ToList();
                    allyear = (from c in db.AcFinancialYears where c.BranchID == currentbranchid   select c).OrderByDescending(cc => cc.AcFYearFrom).ToList();
                    Session["FYear"] = allyear;
                    finacialyear = db.AcFinancialYears.Where(cc => cc.CurrentFinancialYear == true && cc.BranchID == currentbranchid && cc.AcFYearFrom >= branchstartdate).FirstOrDefault();
                }
                else //agent and custome list all year
                {
                    allyear = (from c in db.AcFinancialYears where c.BranchID == currentbranchid select c).OrderByDescending(cc => cc.AcFYearFrom).ToList();
                    finacialyear = db.AcFinancialYears.Where(cc => cc.CurrentFinancialYear == true && cc.BranchID == currentbranchid).FirstOrDefault();
                    Session["FYear"] = allyear;
                }

                if (finacialyear != null)
                {
                    Session["fyearid"] = finacialyear.AcFinancialYearID;
                    Session["CurrentYear"] = (finacialyear.AcFYearFrom.Date.ToString("dd MMM yyyy") + " - " + finacialyear.AcFYearTo.Date.ToString("dd MMM yyyy"));
                    Session["FyearFrom"] = finacialyear.AcFYearFrom;
                    Session["FyearTo"] = finacialyear.AcFYearTo;
                }
                else
                {
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Financial Year Selection not valid!";
                    //TempData["ErrorMsg"] = "Financial Year Selection not valid!";
                    return RedirectToAction("Home", "Home");
                }

                if (roletype == "Customer" || roletype == "CoLoader")
                {
                    Session["UserType"] = roletype;// "Customer";
                    Session["HomePage"] = "/CustomerMaster/Home";
                    return RedirectToAction("Home", "CustomerMaster");
                }
                else if (roletype == "Employee")
                {
                    Session["UserType"] = "Employee";
                    Session["HomePage"] = "/EmployeeMaster/Home";
                    return RedirectToAction("Home", "EmployeeMaster");
                }
                else if (roletype == "Agent")
                {
                    Session["UserType"] = "Agent";
                    Session["HomePage"] = "/Agent/Home";
                    return RedirectToAction("Home", "Agent");
                }
                else
                {
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Login Failed,Contact Admin!";
                    return RedirectToAction("Home", "Home");
                }
            }
            catch(Exception ex)
            {
                Session["StatusMessage"] = "Financial Year Selection not valid!";
                return RedirectToAction("Home", "Home");
            }
        }
        public ActionResult Signout()
        {

            Session.Abandon();

            // @ViewBag.SignOut = "You have successfully signout.";
            FormsAuthentication.SignOut();
            return RedirectToAction("Home","Home");
        }

        

        public ActionResult ForgotPassword()
        {
            var compdetail = db.AcCompanies.FirstOrDefault();
            ViewBag.CompanyName = compdetail.AcCompany1;
            return View();
        }

        public ActionResult ChangePassword()
        {
            var compdetail = db.AcCompanies.FirstOrDefault();
            ViewBag.CompanyName = compdetail.AcCompany1;
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserLoginVM vm)
          {
            string emailid = vm.UserName;
            var _user = db.UserRegistrations.Where(cc => cc.UserName == emailid).FirstOrDefault();
            if (_user!=null)
            {
                GeneralDAO _dao = new GeneralDAO();
                string newpassword = _dao.RandomPassword(6);

                _user.Password = newpassword;
                db.Entry(_user).State = EntityState.Modified;
                db.SaveChanges();
                EmailDAO _emaildao = new EmailDAO();                
                _emaildao.SendForgotMail(_user.UserName,"User",newpassword);
                TempData["SuccessMsg"] = "Reset Password Details are sent,Check Email!";

                return RedirectToAction("Home", "Home");
                //return Json(new { status = "ok", message = "Reset Password Details are sent,Check Email" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                 Session["ForgotStatus"] = "Forgot";
                Session["StatusMessage"] = "Invalid EmailId!";
                return RedirectToAction("Home", "Home");
                //return Json(new { status = "Failed", message = "Invalid EmailId!" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult ChangePassword(UserLoginVM vm)
        {
            string emailid = vm.UserName;
            var _user = db.UserRegistrations.Where(cc => cc.UserName == emailid && cc.Password==vm.Password).FirstOrDefault();
            if (_user != null)
            {

                _user.Password = vm.NewPassword;
                db.Entry(_user).State = EntityState.Modified;
                db.SaveChanges();
                EmailDAO _emaildao = new EmailDAO();
                _emaildao.SendForgotMail(_user.UserName, "User" , vm.NewPassword);
                TempData["SuccessMsg"] = "Password Changed Successfully!";
                return RedirectToAction("Home", "Home");
                //return Json(new { status = "ok", message = "Reset Password Details are sent,Check Email" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //TempData["ErrorMsg"] = "Invalid EmailId or Password!";
                Session["ResetStatus"] = "Reset";
                Session["StatusMessage"] = "Invalid Credential!";
                return RedirectToAction("Home", "Home");
                //return Json(new { status = "Failed", message = "Invalid EmailId!" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
