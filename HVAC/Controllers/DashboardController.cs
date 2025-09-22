using HVAC.DAL;
using HVAC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class DashboardController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

        HVACEntities db = new HVACEntities();
        // GET: Dashboard
        public ActionResult Index()
        { 
            if (Session["UserID"] == null || Session["fyearid"] == null || 
                Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
            {
                return RedirectToAction("Home", "Home");
            }
            
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int RoleID = Convert.ToInt32(Session["UserRoleID"].ToString());
            int employeeId = 0;
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            if (useremployee != null)
            {
                employeeId = useremployee.EmployeeID;
            }
            var vm = new FinancialsChartViewModel
            {
                Months = Enumerable.Range(1, 12)
            .Select(i => new SelectListItem { Value = i.ToString(), Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i) })
            .ToList(),
                Years = Enumerable.Range(2020, 10)
            .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
            .ToList(),
                SelectedMonth = DateTime.Now.Month,
                SelectedYear = DateTime.Now.Year
            };
            var model = new DashboardViewModel();
            model.RevenueSeriesA = "2102,23412,525,2414";
            model.RevenueSeriesB = "2302,2671,2425,2414";
            model.RevenueSeriesC = "502,241,425,414";
            model.CountSeriesA  = "10,41,25,44";
            model.CountSeriesB = "30,71,25,34";
            model.CountSeriesC = "50,41,25,34";
            model = DashboardDAO.GetDashboardSummary(branchid, yearid, employeeId, RoleID);
            model.RecentOrders = DashboardDAO.GetDashboardEnquiryList(branchid, yearid, employeeId, RoleID);
          //  model.FinancialsChartModel = vm;
            return View(model);
            // Pass model to Skote dashboard view
                                //int userid = Convert.ToInt32(Session["UserID"].ToString());
                                //int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                                //int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());
                                //int yearid = Convert.ToInt32(Session["fyearid"].ToString());
                                //DashBoardInfo model = db.DashBoardInfoes.Where(cc => cc.BranchId == branchid && cc.FyearId == yearid).FirstOrDefault();
                                //DashboardVM vm = new DashboardVM();
                                //if (model != null)
                                //{
                                //    vm.ID = model.ID;
                                //    vm.RevenueYTD = model.RevenueYTD;
                                //    vm.RevenueMTD = model.RevenueMTD;
                                //    vm.TotalJobs = model.TotalJobs;
                                //    vm.TotalInStock = model.TotalInStock;
                                //    vm.InStockCBM = model.InStockCBM;
                                //    vm.Top1City = model.Top1City;
                                //    vm.Top1CityName = model.Top1CityName;
                                //    vm.Top2City = model.Top2City;
                                //    vm.Top2CityName = model.Top2CityName;
                                //    vm.Top3City = model.Top3City;
                                //    vm.Top3CityName = model.Top3CityName;
                                //    vm.RevenueSeriesA = model.RevenueSeriesA;
                                //    vm.RevenueSeriesB = model.RevenueSeriesB;
                                //    vm.RevenueSeriesC = model.RevenueSeriesC;
                                //    vm.CountSeriesA = model.CountSeriesA;
                                //    vm.CountSeriesB = model.CountSeriesB;
                                //    vm.CountSeriesC = model.CountSeriesC;
                                //    vm.ShipmentList = DashboardDAO.GetDashboardConsignmentList(branchid, yearid);

            //}
            //else
            //{
            //    vm.ShipmentList = new List<QuickAWBVM>();                

            //}

            
        }


        [HttpPost]
        public ActionResult ProjectAnalysisPartial(int month, int year)
        {
            var model = new FinancialsChartViewModel
            {
                Months = Enumerable.Range(1, 12)
                    .Select(i => new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)
                    }).ToList(),
                Years = Enumerable.Range(2011, 15)
                    .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() }).ToList(),
                SelectedMonth = month,
                SelectedYear = year
               
            };

            return PartialView("ProjectAnalysis", model);
        }

        [HttpGet]
        public JsonResult GetHVACChartData(int month, int year)
        {
            var data = new List<object>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("HVAC_DashboardChart", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            EnquiryType = reader["EnquiryType"].ToString(),
                            Revenue = Convert.ToDecimal(reader["Revenue"]),
                            Cost = Convert.ToDecimal(reader["Cost"]),
                            Margin = Convert.ToDecimal(reader["Margin"])
                        });
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
        public List<SelectListItem> GetAllMonths()
        {
            return Enumerable.Range(1, 12)
                .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)
                }).ToList();
        }
        //DashboardReprocess
        //public JsonResult DashboardReprocess()
        //{
        //int userid = Convert.ToInt32(Session["UserID"].ToString());
        //int yearid = Convert.ToInt32(Session["fyearid"].ToString());
        //int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
        //var dta = DashboardDAO.DashboardReprocess(branchid, yearid, userid);
        //return Json(new { status = "OK", message = "ReProcessed Succesfully!" });
        //}
    }
   

}