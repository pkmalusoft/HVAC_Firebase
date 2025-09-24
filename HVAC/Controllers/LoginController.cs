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
using System.Security.Cryptography;
using System.Text;

namespace HVAC.Controllers
{
    public class LoginController : Controller
    {
        HVACEntities db = new HVACEntities();
        
        // Role ID constants
        private const int AGENT_ROLE_ID = 14;
        private const int CUSTOMER_ROLE_ID = 13;
        private const int COLOADER_ROLE_ID = 23;
        
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

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
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginVM u)
        {
            // Input validation
            if (!ModelState.IsValid)
            {
                return View(u);
            }

            if (string.IsNullOrEmpty(u.UserName) || string.IsNullOrEmpty(u.Password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View(u);
            }

            int proleid = 0;
            int userid = 0;
            string roletype = "";
            int currentyear = DateTime.Now.Year;
            List<int> rolelist = new List<int>();
            UserRegistration u1 = null;
            try
            {
                u1 = (from c in db.UserRegistrations where c.UserName == u.UserName select c).FirstOrDefault();
                if (u1 != null && !VerifyPassword(u.Password, u1.Password))
                {
                    u1 = null; // Invalid password
                }
                if (u1 != null)
                {
                    userid = u1.UserID;
                    if (u1.RoleID == AGENT_ROLE_ID)
                    {
                        proleid = AGENT_ROLE_ID;
                        roletype = "Agent";
                        Session["CurrentDepot"] = "";
                    }
                    else if (u1.RoleID == CUSTOMER_ROLE_ID || u1.RoleID == COLOADER_ROLE_ID)
                    {

                        proleid = Convert.ToInt32(u1.RoleID);
                        if (u1.RoleID == CUSTOMER_ROLE_ID)
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
                            
                            var customerBranch = db.BranchMasters.Where(cc => cc.BranchID == branchid).FirstOrDefault();
                            Session["CurrentDepot"] = customerBranch.BranchName;

                        }

                    }
                    else
                    {
                        proleid = Convert.ToInt32(u1.RoleID);
                        roletype = "Employee";

                        int? branchid = 1; 
                        Session["CurrentBranchID"] = branchid;
                        
                        var employeeBranch = db.BranchMasters.Where(cc => cc.BranchID == branchid).FirstOrDefault();
                        Session["CurrentDepot"] = employeeBranch.BranchName;

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
                if (alldepot == null || alldepot.Count == 0)
                {
                    //if (Session["CurrentBranchID"] != null)
                    //{
                    //    int currentbranchid1 = alldepot[0].BranchID;//  Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    //    alldepot = db.BranchMasters.Where(cc => cc.BranchID == currentbranchid1).ToList();
                    //    Session["CurrentBranchID"] = currentbranchid1;
                    //}

                    // Debug: Log the issue
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Branch is not valid! User ID: " + u1.UserID + " - No branches assigned to user.";
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
                var branch = db.BranchMasters.Find(currentbranchid);
                if (branch == null || branch.AcFinancialYearID == null)
                {
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Branch configuration not valid!";
                    return RedirectToAction("Home", "Home");
                }
                int startyearid = Convert.ToInt32(branch.AcFinancialYearID);
                if (startyearid == 0)
                {
                    Session["LoginStatus"] = "Login";
                    Session["StatusMessage"] = "Financial Year Selection not valid! Branch ID: " + currentbranchid + " - No financial year configured.";
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
                    Session["StatusMessage"] = "Financial Year Selection not valid! Branch ID: " + currentbranchid + " - No current financial year found.";
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
                    Session["StatusMessage"] = "Login Failed,Contact Admin! Role Type: " + roletype + " - Unrecognized role type.";
                    return RedirectToAction("Home", "Home");
                }
            }
            catch(Exception ex)
            {
                Session["StatusMessage"] = "Login Error: " + ex.Message;
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

        /// <summary>
        /// Diagnostic method to check database setup
        /// </summary>
        public ActionResult CheckDatabaseSetup()
        {
            var diagnostics = new
            {
                CompanyCount = db.AcCompanies.Count(),
                BranchCount = db.BranchMasters.Count(),
                UserCount = db.UserRegistrations.Count(),
                UserInBranchCount = db.UserInBranches.Count(),
                FinancialYearCount = db.AcFinancialYears.Count(),
                CurrencyCount = db.CurrencyMasters.Count(),
                BranchesWithFinancialYear = db.BranchMasters.Count(b => b.AcFinancialYearID != null),
                CurrentFinancialYears = db.AcFinancialYears.Count(f => f.CurrentFinancialYear == true),
                SampleUsers = db.UserRegistrations.Take(5).Select(u => new { u.UserID, u.UserName, u.RoleID }).ToList(),
                SampleBranches = db.BranchMasters.Take(5).Select(b => new { b.BranchID, b.BranchName, b.AcFinancialYearID }).ToList()
            };

            return Json(diagnostics, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create test data for login testing
        /// </summary>
        public ActionResult CreateTestData()
        {
            try
            {
                // Check if we already have data
                if (db.AcCompanies.Any() && db.BranchMasters.Any() && db.UserRegistrations.Any())
                {
                    return Json(new { status = "info", message = "Test data already exists" }, JsonRequestBehavior.AllowGet);
                }

                // Create company if not exists
                if (!db.AcCompanies.Any())
                {
                    var company = new AcCompany
                    {
                        AcCompany1 = "Test HVAC Company",
                        KeyPerson = "Admin",
                        Phone = "1234567890",
                        EMail = "admin@test.com",
                        Address1 = "Test Address",
                        CityName = "Test City",
                        CountryName = "Test Country",
                        AcceptSystem = true,
                        EnableCashCustomerInvoice = true,
                        EnableAPI = true,
                        AWBAlphaNumeric = true
                    };
                    db.AcCompanies.Add(company);
                    db.SaveChanges();
                }

                // Create currency if not exists
                if (!db.CurrencyMasters.Any())
                {
                    var currency = new CurrencyMaster
                    {
                        CurrencyName = "US Dollar",
                        CurrencyCode = "USD",
                        NoOfDecimals = 2,
                        MonetaryUnit = "Dollar",
                        NumberFormat = 1
                    };
                    db.CurrencyMasters.Add(currency);
                    db.SaveChanges();
                }

                // Create branch if not exists
                if (!db.BranchMasters.Any())
                {
                    var currency = db.CurrencyMasters.FirstOrDefault();
                    var branch = new BranchMaster
                    {
                        BranchName = "Main Branch",
                        CurrencyID = currency?.CurrencyID,
                        AcFinancialYearID = null // Will be set after creating financial year
                    };
                    db.BranchMasters.Add(branch);
                    db.SaveChanges();

                    // Create financial year
                    var financialYear = new AcFinancialYear
                    {
                        BranchID = branch.BranchID,
                        AcFYearFrom = DateTime.Now.Date,
                        AcFYearTo = DateTime.Now.AddYears(1).Date,
                        CurrentFinancialYear = true
                    };
                    db.AcFinancialYears.Add(financialYear);
                    db.SaveChanges();

                    // Update branch with financial year
                    branch.AcFinancialYearID = financialYear.AcFinancialYearID;
                    db.SaveChanges();
                }

                // Create test user if not exists
                if (!db.UserRegistrations.Any())
                {
                    var user = new UserRegistration
                    {
                        UserName = "admin@test.com",
                        Password = HashPassword("admin123"),
                        RoleID = 1, // Assuming role 1 is admin/employee
                        IsActive = true
                    };
                    db.UserRegistrations.Add(user);
                    db.SaveChanges();

                    // Create user-branch relationship
                    var branch = db.BranchMasters.FirstOrDefault();
                    if (branch != null)
                    {
                        var userBranch = new UserInBranch
                        {
                            UserID = user.UserID,
                            BranchID = branch.BranchID
                        };
                        db.UserInBranches.Add(userBranch);
                        db.SaveChanges();
                    }
                }

                return Json(new { status = "success", message = "Test data created successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Error creating test data: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(UserLoginVM vm)
          {
            string emailid = vm.UserName;
            var _user = db.UserRegistrations.Where(cc => cc.UserName == emailid).FirstOrDefault();
            if (_user!=null)
            {
                GeneralDAO _dao = new GeneralDAO();
                string newpassword = _dao.RandomPassword(6);

                _user.Password = HashPassword(newpassword);
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
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserLoginVM vm)
        {
            string emailid = vm.UserName;
            var _user = db.UserRegistrations.Where(cc => cc.UserName == emailid).FirstOrDefault();
            if (_user != null && VerifyPassword(vm.Password, _user.Password))
            {

                _user.Password = HashPassword(vm.NewPassword);
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
