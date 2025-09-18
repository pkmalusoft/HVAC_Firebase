using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.VariantTypes;
using HVAC.DAL;
using HVAC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ReportsController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: Reports

        #region
        [HttpGet]
        public ActionResult EnquiryQuotedAnalysis()
        {
       
            // Default to current year (or latest available year)
            int defaultYear = DateTime.Now.Year;

            // Fetch initial data
            var summaryList = HVACReportsDAO.GetEnquirySummaryList(defaultYear);

            //ViewBag.SelectedYear = defaultYear;
            //ViewBag.Months = summaryList.Select(m => m.MonthName).ToArray();
            //ViewBag.EnqRcd = summaryList.Select(m => m.EnquiriesReceived).ToArray();
            //ViewBag.EnqQtd = summaryList.Select(m => m.EnquiriesQuoted).ToArray();
            AnalysisReportModel model = new AnalysisReportModel();
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.SelectedYear = defaultYear;
            //model.Months= summaryList.Select(m => m.MonthName).ToArray();
            model.EnqRcd = summaryList.Select(m => m.EnquiriesReceived).ToArray();
            model.EnqQtd = summaryList.Select(m => m.EnquiriesQuoted).ToArray();
            return View(model);
        }

        [HttpPost]
        public ActionResult EnquiryQuotedAnalysis(int year)
        {
            // Reload dropdown years for POST too
            //ViewBag.Years = HVACReportsDAO.GetAvailableYears();

            // Fetch for the selected year
            var summaryList = HVACReportsDAO.GetEnquirySummaryList(year);

            //ViewBag.SelectedYear = year;
            //ViewBag.Months = summaryList.Select(m => m.MonthName).ToArray();
            //ViewBag.EnqRcd = summaryList.Select(m => m.EnquiriesReceived).ToArray();
            //ViewBag.EnqQtd = summaryList.Select(m => m.EnquiriesQuoted).ToArray();
            AnalysisReportModel model = new AnalysisReportModel();
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.SelectedYear = year;
            //model.Months = summaryList.Select(m => m.MonthName).ToArray();
            model.EnqRcd = summaryList.Select(m => m.EnquiriesReceived).ToArray();
            model.EnqQtd = summaryList.Select(m => m.EnquiriesQuoted).ToArray();
            return View(model);
        }
        public ActionResult EnquiryQuotedAnalysisPrintPdf(int year)
        {
            var summaryList = HVACReportsDAO.GetEnquirySummaryList(year);
            AnalysisReportModel model = new AnalysisReportModel();
            model.Years = HVACReportsDAO.GetAvailableYears();
            //model.Months = summaryList.Select(m => m.MonthName).ToArray();
            model.EnqRcd = summaryList.Select(m => m.EnquiriesReceived).ToArray();
            model.EnqQtd = summaryList.Select(m => m.EnquiriesQuoted).ToArray();
            return new ViewAsPdf("EnquiryQuotedAnalysis",model)
            {
                FileName = "EnquiryQuotedAnalysis_"+ year + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };
        }
        public JsonResult GetAvailableYears()
        {
            List<int> years = new List<int>();

            years = HVACReportsDAO.GetAvailableYears();

            return Json(years, JsonRequestBehavior.AllowGet);
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
        //current month secured jobs

        #region Securedjobssummary
        [HttpGet]
        public ActionResult SecuredJobs()
        {
          
            int defaultYear = DateTime.Now.Year;
            ViewBag.SelectedYear = defaultYear;

            ViewBag.SelectedMonth = DateTime.Now.Month;
            // 1. Get data from DAO
            List<SecuredJobModel> jobs = HVACReportsDAO.GetCurrentMonthSecuredJobs(defaultYear, ViewBag.SelectedMonth);
            
            // 2. Group by category for easy display
            //var groupedJobs = new List<SecuredJobGroupModel>();
            var groupedJobs = jobs
                .GroupBy(j => j.Category)
                .Select(g => new SecuredJobGroupModel
                {
                    Category = g.Key,
                    Jobs = g.ToList(),
                    CategoryTotal = g.Sum(x => x.ValueInOMR)
                })
                .ToList();

            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.ValueInOMR);

            var model = new CurrentMonthSecuredJobsViewModel
            {
                Groups = groupedJobs,
                GrandTotal = grandTotal
            };
            model.SelectedMonth = DateTime.Now.Month;
            model.SelectedYear = defaultYear;
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.Months = GetAllMonths();
            
            model.ReportClass = "";
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            return View(model);
        }

        [HttpPost]
        public ActionResult SecuredJobs(int SelectedMonth,int SelectedYear)
        {
            int Year = SelectedYear;
            int Month = SelectedMonth;
            ViewBag.Years = HVACReportsDAO.GetAvailableYears();
            var _Months = GetAllMonths();
            
            ViewBag.SelectedMonth = Month;
            ViewBag.SelectedYear = Year;
            // 1. Get data from DAO
            List<SecuredJobModel> jobs = HVACReportsDAO.GetCurrentMonthSecuredJobs(SelectedYear,SelectedMonth);

            // 2. Group by category for easy display
            //var groupedJobs = new List<SecuredJobGroupModel>();
            var groupedJobs = jobs
                .GroupBy(j => j.Category)
                .Select(g => new SecuredJobGroupModel
                {
                    Category = g.Key,
                    Jobs = g.ToList(),
                    CategoryTotal = g.Sum(x => x.ValueInOMR)
                })
                .ToList();

            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.ValueInOMR);

            var model = new CurrentMonthSecuredJobsViewModel
            {
                Groups = groupedJobs,
                GrandTotal = grandTotal
            };
            model.SelectedMonth = Month;
            model.SelectedYear = Year;
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.Months = GetAllMonths();
            model.ReportClass = "";
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            return View(model);
        }

        public ActionResult SecuredJobsPrint(int SelectedMonth, int SelectedYear)
        {
                        
            // 1. Get data from DAO
            List<SecuredJobModel> jobs = HVACReportsDAO.GetCurrentMonthSecuredJobs(SelectedYear,SelectedMonth);

            // 2. Group by category for easy display
            //var groupedJobs = new List<SecuredJobGroupModel>();
            var groupedJobs = jobs
                .GroupBy(j => j.Category)
                .Select(g => new SecuredJobGroupModel
                {
                    Category = g.Key,
                    Jobs = g.ToList(),
                    CategoryTotal = g.Sum(x => x.ValueInOMR)
                })
                .ToList();

            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.ValueInOMR);

            var model = new CurrentMonthSecuredJobsViewModel
            {
                Groups = groupedJobs,
                GrandTotal = grandTotal
            };
            model.SelectedMonth = SelectedMonth;
            model.SelectedYear = SelectedYear;
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.Months = GetAllMonths();
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            model.ReportClass = "pdfenable";
            model.Years = HVACReportsDAO.GetAvailableYears();
            

            return new Rotativa.ViewAsPdf("SecuredJobs", model)
            {
                FileName = "SecuredJobsSummary_" + model.SelectedMonth + "_" + model.SelectedYear + ".pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };
        }

        #endregion


        #region SecuredjobsRegister
        [HttpGet]
        public ActionResult SecuredJobsDetail()
        {
            
            int defaultYear = DateTime.Now.Year;
            int _SelectedYear = defaultYear;

            int _SelectedMonth = DateTime.Now.Month;
            // 1. Get data from DAO
            List<SecuredJobDetailModel> jobs = HVACReportsDAO.GetSecuredJobsDetail(_SelectedYear,_SelectedMonth);


            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.POValue);
            decimal grandOrderValue = jobs.Sum(j => j.OrderValue);
            decimal grandTotalvat = jobs.Sum(j => j.VAT);
            var model = new CurrentMonthSecuredJobsViewModel
            {
                JobDetail = jobs,
                GrandTotal = grandTotal,
                GrandOrderValue = grandOrderValue,
                GrandVAT = grandTotalvat
            };
            model.SelectedMonth = DateTime.Now.Month;
            model.SelectedYear = defaultYear;
            model.ReportClass = "";
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.Months = GetAllMonths();
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            return View(model);
        }

        [HttpPost]
        public ActionResult SecuredJobsDetail(int SelectedMonth, int SelectedYear)
        {
            
            // 1. Get data from DAO
            List<SecuredJobDetailModel> jobs = HVACReportsDAO.GetSecuredJobsDetail(SelectedYear,SelectedMonth);



            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.POValue);
            decimal grandOrderValue = jobs.Sum(j => j.OrderValue);
            decimal grandTotalvat = jobs.Sum(j => j.VAT);

            var model = new CurrentMonthSecuredJobsViewModel
            {
                JobDetail = jobs,
                GrandTotal = grandTotal,
                GrandOrderValue =grandOrderValue,
                GrandVAT =grandTotalvat
            };
            model.SelectedMonth = SelectedMonth;
            model.SelectedYear = SelectedYear;
            model.ReportClass = "";
            model.Months = GetAllMonths();
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            return View(model);
        }

        public ActionResult SecuredJobsDetailPrint(int SelectedMonth, int SelectedYear)
        {
            
            // 1. Get data from DAO
            List<SecuredJobDetailModel> jobs = HVACReportsDAO.GetSecuredJobsDetail(SelectedYear, SelectedMonth);

            // 2. Group by category for easy display
            //var groupedJobs = new List<SecuredJobGroupModel>();
            

            // 3. Calculate overall grand total
            decimal grandTotal = jobs.Sum(j => j.POValue);
            decimal grandOrderValue = jobs.Sum(j => j.OrderValue);
            decimal grandTotalvat = jobs.Sum(j => j.VAT);
            var model = new CurrentMonthSecuredJobsViewModel
            {
                JobDetail = jobs,
                GrandTotal = grandTotal,
                 GrandOrderValue = grandOrderValue,
                GrandVAT = grandTotalvat
            };
            model.SelectedMonth = SelectedMonth;
            model.SelectedYear = SelectedYear; 
            model.Months = GetAllMonths();
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.SelectedMonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth);
            model.ReportClass = "pdfenable";
            return new Rotativa.ViewAsPdf("SecuredJobsDetail", model)
            {
                FileName = "SecuredJobsRegister_"+ model.SelectedMonth + "_" + model.SelectedYear + ".pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A3
            };
        }
        public ActionResult SecuredJobsDetailExcel(int SelectedMonth, int SelectedYear)
        {
            List<SecuredJobDetailModel> jobs = HVACReportsDAO.GetSecuredJobsDetail(SelectedYear, SelectedMonth);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Secured Jobs Register " +  SelectedMonth.ToString() + " Year :" + SelectedYear.ToString() );
                worksheet.Cell(1, 1).Value = "Sl No.";
                worksheet.Cell(1, 2).Value = "Job No.";
                worksheet.Cell(1, 3).Value = "PO Reference";
                worksheet.Cell(1, 4).Value = "PO Date";
                worksheet.Cell(1, 5).Value = "Project Title";
                worksheet.Cell(1, 6).Value = "Order Value";
                worksheet.Cell(1, 7).Value = "VAT";
                worksheet.Cell(1, 8).Value = "Total Value";
                worksheet.Cell(1, 9).Value = "Margin";
                worksheet.Cell(1, 10).Value = "Quoted By";

                int row = 2;
                foreach (var job in jobs)
                {
                    worksheet.Cell(row, 1).Value = job.SLNo;
                    worksheet.Cell(row, 2).Value = job.JobNo;
                    worksheet.Cell(row, 3).Value = job.POReference;
                    worksheet.Cell(row, 4).Value = job.PODate.ToShortDateString();
                    worksheet.Cell(row, 5).Value = job.ProjectTitle;
                    worksheet.Cell(row, 6).Value = job.OrderValue;
                    worksheet.Cell(row, 7).Value = job.VAT;
                    worksheet.Cell(row, 8).Value = job.POValue;
                    worksheet.Cell(row, 9).Value = job.EstimateMargin;
                    worksheet.Cell(row, 10).Value = job.QuotedBy;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "SecuredJobsRegister_" + SelectedMonth.ToString() + "_" + SelectedYear.ToString() + ".xlsx");
                }
            }
        }

        #endregion 
        public ActionResult EngineerBooking()
        {
            AnalysisReportModel model = new AnalysisReportModel();
            // Default to current year (or latest available year)
            int defaultYear = DateTime.Now.Year;
            int _SelectedMonth = DateTime.Now.Month;
            // Fetch initial data
            var summaryList = HVACReportsDAO.GetEnquiryEngineerBooking(defaultYear,_SelectedMonth);

            model.SelectedMonth = _SelectedMonth;
            model.SelectedYear = defaultYear;
            model.Months = GetAllMonths();                        
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.SelectedYear = defaultYear; 
            model.ProjectValue = summaryList.Sum(cc => cc.ProjectValue);

            model.chartdata = summaryList;
            return View(model);
                       
            
    //        var chartData = new List<object>
    //{
    //    new { name = "Shujat", y = 200160 },
    //    new { name = "Afreed T", y = 160686 },
    //    new { name = "Rehan", y = 42097 },
    //    new { name = "Vinayak", y = 15534 },
    //    new { name = "Saurav", y = 11000 },
    //    new { name = "Jaison", y = 6318 },
    //    new { name = "Afreed", y = 880 },
    //    new { name = "Sundar", y = 800 }
    //};

    //        ViewBag.ChartData = JsonConvert.SerializeObject(chartData);
            return View(model);

        }

        [HttpPost]
        public ActionResult EngineerBooking(int SelectedMonth,int SelectedYear)
        {
            AnalysisReportModel model = new AnalysisReportModel();
           
            // Fetch initial data
            var summaryList = HVACReportsDAO.GetEnquiryEngineerBooking(SelectedYear, SelectedMonth);

            model.SelectedMonth = SelectedMonth;
            model.SelectedYear = SelectedYear;
            model.Months = GetAllMonths();
            model.Years = HVACReportsDAO.GetAvailableYears();
            model.chartdata = summaryList;
            model.ProjectValue = summaryList.Sum(cc => cc.ProjectValue);
            return View(model);


        }
        public ActionResult MonthSalesChart()
        {
            var sales = new[] {
        new { name = "Jan", y = 361172, color = "#1e3c72" },
        new { name = "Feb", y = 256746, color = "#a0522d" },
        new { name = "Mar", y = 437474, color = "#555555" }
    };
            ViewBag.SalesData = JsonConvert.SerializeObject(sales);
            return View();

        }

        public ActionResult SalesFunnelChart()
        {
            var funnelData = new[] {
        new { name = "Active", y = 29897168, color = "#66cc66" },
        new { name = "Under Negotiation", y = 7150073, color = "#66a3ff" },
        new { name = "Under MAS", y = 319003, color = "#ffcc00" },
        new { name = "Secured", y = 766506, color = "#336633" },
        new { name = "Lost", y = 3852966, color = "#cc0000" }
    };

            var quotationData = new List<object>
    {
        new { Name = "Active", Value = 29897168, Color = "#6aa84f" },       // Green
        new { Name = "Under Negotiation", Value = 7150073, Color = "#3d85c6" }, // Blue
        new { Name = "Under MAS", Value = 319003, Color = "#ffd966" },      // Yellow
        new { Name = "Secured", Value = 766506, Color = "#38761d" },        // Dark Green
        new { Name = "Lost", Value = 3852966, Color = "#cc0000" }           // Red
    };

            ViewBag.QuotationData = quotationData;
           var data = HVACReportsDAO.GetQuotationStatusData();
            QuotationStatusViewModel obj = new QuotationStatusViewModel();
            obj.name = "Under Negotiation";
            obj.y = 734343;
            obj.color = "#3d85c6";
            data.Add(obj);
            obj.name = "Lost";
            obj.y = 3852966;
            obj.color = "#cc0000";
            data.Add(obj);


            //ViewBag.Color = new List<string> { "#66cc66", "#66a3ff", "#ffcc00", "#cc0000" }; 
            //return Json(data, JsonRequestBehavior.AllowGet);
            //ViewBag.FunnelData = JsonConvert.SerializeObject(funnelData);
            return View(data);
           // return View();

        }

        public ActionResult SalesEquipmentChart()
        {
    //        var funnelData = new[] {
    //    new { name = "Active", y = 29897168, color = "#66cc66" },
    //    new { name = "Under Negotiation", y = 7150073, color = "#66a3ff" },
    //    new { name = "Under MAS", y = 319003, color = "#ffcc00" },
    //    new { name = "Secured", y = 766506, color = "#336633" },
    //    new { name = "Lost", y = 3852966, color = "#cc0000" }
    //};

    //        var quotationData = new List<object>
    //{
    //    new { Name = "Active", Value = 29897168, Color = "#6aa84f" },       // Green
    //    new { Name = "Under Negotiation", Value = 7150073, Color = "#3d85c6" }, // Blue
    //    new { Name = "Under MAS", Value = 319003, Color = "#ffd966" },      // Yellow
    //    new { Name = "Secured", Value = 766506, Color = "#38761d" },        // Dark Green
    //    new { Name = "Lost", Value = 3852966, Color = "#cc0000" }           // Red
    //};

            //ViewBag.QuotationData = quotationData;
            var data = HVACReportsDAO.GetEquipmentStatusData();
            


            //ViewBag.Color = new List<string> { "#66cc66", "#66a3ff", "#ffcc00", "#cc0000" }; 
            //return Json(data, JsonRequestBehavior.AllowGet);
            //ViewBag.FunnelData = JsonConvert.SerializeObject(funnelData);
            return View(data);
            // return View();

        }
        public ActionResult TraneOrderChart()
        {
            var chartData = new[] {
        new { name = "Applied", y = 305984.46, color = "#1e5eb6" },
        new { name = "Unitary Light Commercial", y = 72845.00, color = "#f26b1d" }
         };

            ViewBag.ChartData = JsonConvert.SerializeObject(chartData);
            return View();

        }



        #endregion

        #region EnquiryRegister
        public ActionResult EnquiryRegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
            ViewBag.Employee = db.EmployeeMasters.ToList();
            RegisterReportModel model = (RegisterReportModel)Session["EnquiryRegister"];
            var enquiries = ReportsDAO.GetEnquiryRegisterList(CommonFunctions.GetFirstDayofMonth().Date, DateTime.Now, 1, 2025);
            DataTable dt = GetEnquiryRegisterData(CommonFunctions.GetFirstDayofMonth().Date, DateTime.Now);

            if (model == null)
            {
                model = new RegisterReportModel
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    Output = "PDF",
                    ReportType = "Summary",
                    Groups= dt
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }
            Session["EnquiryRegister"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Enquiry Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();

                if (!currentreport.Contains("EnquiryRegister"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }
        [HttpPost]
        public ActionResult EnquiryRegister2(ReportParam1 picker)
        {

            ReportParam1 model = new ReportParam1
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Output = picker.Output,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["EnquiryRegister"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            ReportsDAO.EnquiryRegister();

            return RedirectToAction("EnquiryRegister", "Reports");


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnquiryRegister(RegisterReportModel model)
        {
            DataTable dt = GetEnquiryRegisterData(model.FromDate, model.ToDate);
            ViewBag.ReportName = "Enquiry Register";
            model.Groups = dt;

            return View(model);

        }
        public ActionResult EnquiryRegisterPrint(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Enquiry Register";
            DataTable dt = ReportsDAO.GetEnquiryRegisterData(fromDate, toDate, branchid, yearid);
             dt = GetEnquiryRegisterData(fromDate, toDate);

            byte[] pdfBytes = GeneratePdf(fromDate, toDate, title, dt);
            return File(pdfBytes, "application/pdf", "EnquiryRegister.pdf");

        }
        public ActionResult EnquiryRegisterExcel(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Enquiry Register";
            DataTable dt = ReportsDAO.GetEnquiryRegisterData(fromDate, toDate, branchid, yearid);
             dt = GetEnquiryRegisterData(fromDate, toDate);

            byte[] excelBytes = GenerateExcel(fromDate, toDate, title, dt);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EnquiryRegister.xlsx");
           
        }
        private DataTable GetEnquiryRegisterData(DateTime fromDate, DateTime toDate)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var yearid = Convert.ToInt32(Session["fyearid"]);
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtResult = new DataTable();

            // Define only required columns
            dtResult.Columns.Add("Sl. No.", typeof(int));
            dtResult.Columns.Add("Enquiry No.", typeof(string));
            dtResult.Columns.Add("Enquiry Date", typeof(string));
            dtResult.Columns.Add("Project/Client", typeof(string));
            dtResult.Columns.Add("Project Location", typeof(string));
            dtResult.Columns.Add("Enquiry Type", typeof(string));
            dtResult.Columns.Add("Assigned To", typeof(string));
            dtResult.Columns.Add("Enquiry Stage", typeof(string));
            dtResult.Columns.Add("Due Date", typeof(string));
            dtResult.Columns.Add("Status", typeof(string));
            dtResult.Columns.Add("Remarks", typeof(string));

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_EnquiryRegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId", yearid);
                cmd.Parameters.AddWithValue("@BranchId", branchid);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            int slno = 1;
            foreach (DataRow dr in dtFromDb.Rows)
            {
                DataRow newRow = dtResult.NewRow();
                newRow["Sl. No."] = slno++;
                newRow["Enquiry No."] = dr["EnquiryNo"]?.ToString();
                newRow["Enquiry Date"] = dr["EnquiryDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["EnquiryDate"]).ToString("dd-MM-yyyy")
                    : "";
                newRow["Project/Client"] = dr["ProjectName"]?.ToString();
                newRow["Project Location"] = dr["City"]?.ToString();
                newRow["Enquiry Type"] = dr["EntityTypeName"]?.ToString();
                newRow["Assigned To"] = dr["EmpName"]?.ToString();
                newRow["Enquiry Stage"] = dr["EnqStageName"]?.ToString();
                newRow["Due Date"] = dr["DueDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["DueDate"]).ToString("dd-MM-yyyy")
                    : "";
                newRow["Status"] = dr["EnqStatusName"]?.ToString();
                newRow["Remarks"] = "";

                dtResult.Rows.Add(newRow);
            }

            return dtResult;
        }
        #endregion

        #region QuotationRegister
        public ActionResult QuotationRegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
                   
            ViewBag.Employee = db.EmployeeMasters.ToList();
            RegisterReportModel model = (RegisterReportModel)Session["QuotationRegister"];
            DataTable dt = GetQuotationRegisterData(CommonFunctions.GetFirstDayofMonth().Date, DateTime.Now);
            if (model == null)
            {
                model = new RegisterReportModel
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    Output = "PDF",
                    ReportType = "Summary",
                    Groups = dt 
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }

            Session["QuotationRegister"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Quotation Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();

                if (!currentreport.Contains("QuotationRegister"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }
        [HttpPost]
        public ActionResult QuotationRegister2(ReportParam1 picker)
        {

            ReportParam1 model = new ReportParam1
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Output = picker.Output,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["QuotationRegister"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            
            ReportsDAO.QuotationRegister();

            return RedirectToAction("QuotationRegister", "Reports");


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuotationRegister(RegisterReportModel model)
        {

            DataTable dt = GetQuotationRegisterData(model.FromDate, model.ToDate);
            model.Groups = dt;
            ViewBag.ReportName = "Quotation Register";
            //if (model.Output == "PDF")
            //{
            //    byte[] pdfBytes = GeneratePdf(model.FromDate, model.ToDate, title, dt);
            //    return File(pdfBytes, "application/pdf", "QuotationRegister.pdf");
            //}
            //else if (model.Output == "EXCEL")
            //{
            //    byte[] excelBytes = GenerateExcel(model.FromDate, model.ToDate, title, dt);
            //    return File(excelBytes,
            //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //        "QuotationRegister.xlsx");
            //}
            //else
            //{
            //    return Content("Unsupported format");
            //}
            return View(model);
        }

        private DataTable GetQuotationRegisterData(DateTime fromDate, DateTime toDate)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var yearid = Convert.ToInt32(Session["fyearid"]);
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtResult = new DataTable();


            dtResult.Columns.Add("Sl. No.", typeof(int));
            dtResult.Columns.Add("Quotation No.", typeof(string));
            dtResult.Columns.Add("Quotation Date", typeof(string));
            dtResult.Columns.Add("Enquiry No.", typeof(string));
            dtResult.Columns.Add("Client Name / Company", typeof(string));
            dtResult.Columns.Add("Project Name / Description", typeof(string));
            dtResult.Columns.Add("Location", typeof(string));
            dtResult.Columns.Add("Prepared By", typeof(string));
            dtResult.Columns.Add("Quotation Value", typeof(string));
            dtResult.Columns.Add("Status", typeof(string));
            dtResult.Columns.Add("Remarks", typeof(string));

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_QuotationRegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId", yearid);
                cmd.Parameters.AddWithValue("@BranchId", branchid);
                cmd.Parameters.AddWithValue("@EnquiryNo", DBNull.Value);
                cmd.Parameters.AddWithValue("@QuotationNo", DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            int slno = 1;
            foreach (DataRow dr in dtFromDb.Rows)
            {
                DataRow newRow = dtResult.NewRow();
                newRow["Sl. No."] = slno++;


                newRow["Quotation No."] = dr["QuotationNo"]?.ToString();
                newRow["Quotation Date"] = dr["QuotationDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["QuotationDate"]).ToString("dd-MM-yyyy")
                    : "";


                newRow["Enquiry No."] = dr["EnquiryNo"]?.ToString();


                newRow["Client Name / Company"] = dr["ClientName"]?.ToString();
                newRow["Project Name / Description"] = dr["ProjectName"]?.ToString();
                newRow["Location"] = dr["City"]?.ToString();


                newRow["Prepared By"] = dr["EmployeeName"]?.ToString();


                if (dr["QuotationValue"] != DBNull.Value)
                {
                    decimal value = Convert.ToDecimal(dr["QuotationValue"]);
                    string currency = dr["CurrencyCode"]?.ToString() ?? "";
                    if (currency == "OMR")
                        newRow["Quotation Value"] = $"{currency} {value:F3}";
                    else
                        newRow["Quotation Value"] = $"{currency} {value:F2}";
                }
                else
                {
                    newRow["Quotation Value"] = "";
                }


                newRow["Status"] = dr["QuotationStatus"]?.ToString() ?? "";
                newRow["Remarks"] = dr["Remarks"]?.ToString() ?? "";

                dtResult.Rows.Add(newRow);
            }

            return dtResult;
        }

        public ActionResult QuotationRegisterPrint(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Quotation Register";
            DataTable dt = GetQuotationRegisterData(fromDate, toDate);
            // 1. Get data from DAO
            byte[] pdfBytes = GeneratePdf(fromDate, toDate, title, dt);
            return File(pdfBytes, "application/pdf", "QuotationRegister.pdf");

        }
        public ActionResult QuotationRegisterExcel(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Quotation Register";
            DataTable dt = GetQuotationRegisterData(fromDate, toDate);
            // 1. Get data from DAO
            byte[] excelBytes = GenerateExcel(fromDate, toDate, title, dt);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "QuotationRegister.xlsx");

        }
        #endregion

        #region EstimationRegister
        public ActionResult EstimationRegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
            ViewBag.Employee = db.EmployeeMasters.ToList();
            RegisterReportModel model = (RegisterReportModel)Session["EstimationRegister"];
            DataTable dt = GetEstimationRegisterData(CommonFunctions.GetFirstDayofMonth().Date, DateTime.Now);
            if (model == null)
            {
                model = new RegisterReportModel
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    Output = "PDF",
                    ReportType = "Summary",
                    Groups = dt
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }

            Session["EstimationRegister"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Estimation Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();

                if (!currentreport.Contains("EstimationRegister"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstimationRegister(RegisterReportModel model)
        {

            DataTable dt = GetEstimationRegisterData(model.FromDate, model.ToDate);
            ViewBag.ReportName = "Estimation Register";
            model.Groups = dt;
            return View(model);

        }
        private DataTable GetEstimationRegisterData(DateTime fromDate, DateTime toDate)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var yearid = Convert.ToInt32(Session["fyearid"]);
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtResult = new DataTable();


            dtResult.Columns.Add("Sl. No.", typeof(int));
            dtResult.Columns.Add("Estimation No.", typeof(string));
            dtResult.Columns.Add("Estimation Date", typeof(string));
            dtResult.Columns.Add("Enquiry No.", typeof(string));
            dtResult.Columns.Add("Client/Company Name", typeof(string));
            dtResult.Columns.Add("Project/Description Name", typeof(string));
            dtResult.Columns.Add("Location", typeof(string));
            dtResult.Columns.Add("Prepared By", typeof(string));
            dtResult.Columns.Add("Estimated Material Cost", typeof(decimal));
            dtResult.Columns.Add("Total Landing Cost", typeof(decimal));


            dtResult.Columns.Add("Margin %", typeof(decimal));
            dtResult.Columns.Add("Remarks", typeof(string));

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_EstimationRegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId", yearid);
                cmd.Parameters.AddWithValue("@BranchId", branchid);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            int slno = 1;
            foreach (DataRow dr in dtFromDb.Rows)
            {
                DataRow newRow = dtResult.NewRow();
                newRow["Sl. No."] = slno++;
                newRow["Estimation No."] = dr["EstimationNo"]?.ToString();
                newRow["Estimation Date"] = dr["EstimationDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["EstimationDate"]).ToString("dd-MM-yyyy")
                    : "";
                newRow["Enquiry No."] = dr["EnquiryNo"]?.ToString();
                newRow["Client/Company Name"] = dr["ClientName"]?.ToString();
                newRow["Project/Description Name"] = dr["ProjectName"]?.ToString();
                newRow["Location"] = dr["City"]?.ToString();
                newRow["Prepared By"] = dr["EmpName"]?.ToString();


                newRow["Estimated Material Cost"] = dr["EstimatedMaterialCost"] != DBNull.Value ? Convert.ToDecimal(dr["EstimatedMaterialCost"]) : 0;
                newRow["Total Landing Cost"] = dr["TotalLandingCostOMR"] != DBNull.Value ? Convert.ToDecimal(dr["TotalLandingCostOMR"]) : 0;
                //newRow["Overheads"] = dr["Overheads"] != DBNull.Value ? Convert.ToDecimal(dr["Overheads"]) : 0;
                //newRow["Total Estimate"] = dr["TotalEstimate"] != DBNull.Value ? Convert.ToDecimal(dr["TotalEstimate"]) : 0;
                newRow["Margin %"] = dr["MarginPercent"] != DBNull.Value ? Convert.ToDecimal(dr["MarginPercent"]) : 0;

                newRow["Remarks"] = dr["Notes"]?.ToString();

                dtResult.Rows.Add(newRow);
            }

            return dtResult;
        }

        public ActionResult EstimationRegisterPrint(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Estimation Register";
            DataTable dt = GetEstimationRegisterData(fromDate, toDate);
            // 1. Get data from DAO
            byte[] pdfBytes = GeneratePdf(fromDate, toDate, title, dt);
            return File(pdfBytes, "application/pdf", "EstimationRegister.pdf");

        }
        public ActionResult EstimationRegisterExcel(DateTime fromDate, DateTime toDate)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string title = "Estimation Register";
            DataTable dt = GetEstimationRegisterData(fromDate, toDate);
            // 1. Get data from DAO
            byte[] excelBytes = GenerateExcel(fromDate, toDate, title, dt);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EstimationRegister.xlsx");

        }
        #endregion

        #region ClientPORegister
        public ActionResult ClientPORegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
            ViewBag.Employee = db.EmployeeMasters.ToList();
            RegisterReportModel model = (RegisterReportModel)Session["ClientPORegister"];
            DataTable dt = GetClientPORegisterData(CommonFunctions.GetFirstDayofMonth().Date, DateTime.Now);
            if (model == null)
            {
                model = new RegisterReportModel
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    Output = "PDF",
                    ReportType = "Summary",
                    Groups = dt
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }

            Session["ClientPORegister"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "ClientPO Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();

                if (!currentreport.Contains("ClientPORegister"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientPORegister(RegisterReportModel model)
        {

            DataTable dt = GetClientPORegisterData(model.FromDate, model.ToDate);
            ViewBag.ReportName = "ClientPO Register";
            model.Groups = dt;
            return View(model);

        }
        private DataTable GetClientPORegisterData(DateTime fromDate, DateTime toDate)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var yearid = Convert.ToInt32(Session["fyearid"]);
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtResult = new DataTable();

            dtResult.Columns.Add("Sl. No.", typeof(int));
            dtResult.Columns.Add("Client PO No.", typeof(string));
            dtResult.Columns.Add("PO Date", typeof(string));
            dtResult.Columns.Add("Client/Company", typeof(string));
            dtResult.Columns.Add("Project/Site", typeof(string));
            dtResult.Columns.Add("Enquiry No.", typeof(string));
            dtResult.Columns.Add("Quotation No.", typeof(string));
            dtResult.Columns.Add("Currency", typeof(string));
            dtResult.Columns.Add("PO Value", typeof(string));           
            dtResult.Columns.Add("Tax %", typeof(decimal));
            dtResult.Columns.Add("Tax Amount", typeof(decimal));
            dtResult.Columns.Add("Net PO Value", typeof(decimal));

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_ClientPORegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            int slno = 1;
            foreach (DataRow dr in dtFromDb.Rows)
            {
                DataRow newRow = dtResult.NewRow();
                newRow["Sl. No."] = slno++;

                newRow["Client PO No."] = dr["Client PO No."].ToString();
                newRow["PO Date"] = dr["PO Date"] != DBNull.Value
                    ? Convert.ToDateTime(dr["PO Date"]).ToString("dd-MM-yyyy")
                    : "";
                newRow["Client/Company"] = dr["Client/Company"].ToString();
                newRow["Project/Site"] = dr["Project/Site"].ToString();
                newRow["Enquiry No."] = dr["Enquiry No."].ToString();
                newRow["Quotation No."] = dr["Quotation No."].ToString();
                newRow["Currency"] = dr["Currency"].ToString();
                newRow["PO Value"] = dr["PO Value"].ToString();

                newRow["Tax %"] = dr["Tax %"] != DBNull.Value ? Convert.ToDecimal(dr["Tax %"]) : 0;
                newRow["Tax Amount"] = dr["Tax Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Tax Amount"]) : 0;
                newRow["Net PO Value"] = dr["Net PO Value"] != DBNull.Value ? Convert.ToDecimal(dr["Net PO Value"]) : 0;

                dtResult.Rows.Add(newRow);


            }

            return dtResult;
        }

        public ActionResult ClientPORegisterPrint(DateTime fromDate, DateTime toDate)
        {
            
            string title = "ClientPO Register";
            DataTable dt = GetClientPORegisterData(fromDate, toDate);
          
            byte[] pdfBytes = GeneratePdf(fromDate, toDate, title, dt);
            return File(pdfBytes, "application/pdf", "ClientPORegister.pdf");

        }
        public ActionResult ClientPORegisterExcel(DateTime fromDate, DateTime toDate)
        {
           
            string title = "ClientPO Register";
            DataTable dt = GetClientPORegisterData(fromDate, toDate);
           
            byte[] excelBytes = GenerateExcel(fromDate, toDate, title, dt);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ClientPORegister.xlsx");

        }
        #endregion

        #region "StockStatement"
        public ActionResult StockStatement()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Brand = db.Brands.OrderBy(c => c.BrandName);
            StockReportParam model = (StockReportParam)Session["StockStatementReportParam"];// SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new StockReportParam
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    AsonDate = CommonFunctions.GetLastDayofMonth().Date, //.AddDays(-1);,
                    BrandID = 0,
                    ProductCategoryName = "",
                    EquipmentTypeID = 0,
                    ProductName = "",
                    Output = "PDF",
                    ReportType = "Ledger",
                    CustomerType = "CR"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommonFunctions.GetLastDayofMonth().Date;
            }

            Session["StockStatementReportParam"] = model;
            ViewBag.ReportName = "Stock Statement";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("StockStatement"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult StockStatement(StockReportParam picker)
        {

            StockReportParam model = new StockReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                BrandID = picker.BrandID,                
                EquipmentTypeID= picker.EquipmentTypeID,
                ProductName = picker.ProductName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate,
            };

            ViewBag.Token = model;
            Session["StockStatementReportParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            ReportsDAO.GenerateStockStatementReport();

            return RedirectToAction("StockStatement", "Reports");

        }
        #endregion
        #region "StockLedger"
        public ActionResult StockLedger()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Brand = db.Brands.OrderBy(c => c.BrandName);
            StockReportParam model = (StockReportParam)Session["StockLedgerReportParam"];// SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new StockReportParam
                {
                    FromDate = CommonFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommonFunctions.GetLastDayofMonth().Date,
                    AsonDate = CommonFunctions.GetLastDayofMonth().Date, //.AddDays(-1);,
                    BrandID = 0,
                    EquipmentTypeID =0,
                    ProductName = "",
                    Output = "PDF",
                    ReportType = "Ledger",
                    CustomerType = "CR"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommonFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommonFunctions.GetLastDayofMonth().Date;
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommonFunctions.GetLastDayofMonth().Date;
            }

            model.data = GetStockLedgerData(model.FromDate, model.ToDate);
            //model.AsonDate = AccountsDAO.CheckParamDate(model.AsonDate, yearid).Date;
            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;
            Session["StockLedgerReportParam"] = model;
            ViewBag.ReportName = "Stock Ledger";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("StockLedger"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }
        [HttpPost]
        public ActionResult StockLedger(StockReportParam picker)
        {

            StockReportParam model = new StockReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                BrandID = picker.BrandID,
                ProductCategoryName = picker.ProductCategoryName,
                EquipmentTypeID = picker.EquipmentTypeID, 
                ProductName = picker.ProductName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate,
            };

            ViewBag.Token = model;
            Session["StockLedgerReportParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
           model.data = GetStockLedgerData(model.FromDate, model.ToDate);
          // ReportsDAO.GenerateStockLedgerReport();

            return RedirectToAction("StockLedger", "Reports");

        }
        private DataTable GetStockLedgerData(DateTime fromDate, DateTime toDate)
        {         

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_StockLedger";
            //comd.Parameters.AddWithValue("@ProductCategoryID", reportparam.ProductCategoryID);
            comd.Parameters.AddWithValue("@EquipmentTypeID", 0);
            comd.Parameters.AddWithValue("@FromDate", fromDate);
            comd.Parameters.AddWithValue("@ToDate", toDate);
            //comd.Parameters.AddWithValue("@BranchID", branchid);
            //comd.Parameters.AddWithValue("@FYearID", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "StockLedger");
            DataTable dtResult = ds.Tables["StockLedger"];

            return dtResult;
        }

        public ActionResult StockLedgerPrint(DateTime fromDate, DateTime toDate)
        {

            string title = "Stock Ledger";
            DataTable dt = GetStockLedgerData(fromDate, toDate);

            byte[] pdfBytes = GeneratePdf(fromDate, toDate, title, dt);
            return File(pdfBytes, "application/pdf", "StockLedger.pdf");

        }
        public ActionResult StockLedgerExcel(DateTime fromDate, DateTime toDate)
        {

            string title = "Stock Ledger";
            DataTable dt = GetStockLedgerData(fromDate, toDate);

            byte[] excelBytes = GenerateExcel(fromDate, toDate, title, dt);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockLedger.xlsx");

        }

        #endregion


        #region generatepdfexcel
        private byte[] GeneratePdf(DateTime fromDate, DateTime toDate, string AccountHead, DataTable dataT)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string usertype = Session["UserType"].ToString();


            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);
            string accountHead = AccountHead;
            string period = $"From {fromDate:dd-MMM-yyyy} To {toDate:dd-MMM-yyyy}";
            string userdetail = "Printed by " + SourceMastersModel.GetUserFullName(userid, usertype)
                                + " on " + CommonFunctions.GetCurrentDateTime();
            DataTable dt = dataT;


            using (var ms = new MemoryStream())
            {

                Document doc = new Document(PageSize.A3, 30f, 30f, 40f, 30f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();


                Paragraph header = new Paragraph(companyname, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
                header.Alignment = Element.ALIGN_CENTER;
                doc.Add(header);

                Paragraph address = new Paragraph(companyaddress, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                address.Alignment = Element.ALIGN_CENTER;
                address.SpacingAfter = 10f;
                doc.Add(address);

                Paragraph reportTitle = new Paragraph(accountHead, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                reportTitle.Alignment = Element.ALIGN_CENTER;
                reportTitle.SpacingAfter = 5f;
                doc.Add(reportTitle);

                Paragraph periodPara = new Paragraph(period, FontFactory.GetFont(FontFactory.HELVETICA, 10));
                periodPara.Alignment = Element.ALIGN_CENTER;
                periodPara.SpacingAfter = 15f;
                doc.Add(periodPara);

                Paragraph userPara = new Paragraph(userdetail, FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.DARK_GRAY));
                userPara.Alignment = Element.ALIGN_RIGHT;
                userPara.SpacingAfter = 20f;
                doc.Add(userPara);


                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;


                foreach (DataColumn col in dt.Columns)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(col.ColumnName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(headerCell);
                }


                foreach (DataRow row in dt.Rows)
                {
                    foreach (var cell in row.ItemArray)
                    {
                        PdfPCell bodyCell = new PdfPCell(new Phrase(cell?.ToString() ?? "", FontFactory.GetFont(FontFactory.HELVETICA, 9)));
                        bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(bodyCell);
                    }
                }

                doc.Add(table);
                doc.Close();

                return ms.ToArray();
            }
        }


        private byte[] GenerateExcel(DateTime fromDate, DateTime toDate, string AccountHead, DataTable dataT)
        {



            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                wb.Worksheets.Add(dataT, AccountHead);
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }
        #endregion
    }
}