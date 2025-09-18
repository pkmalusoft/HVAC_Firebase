using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.IO;
using System.Web.UI;
using ClosedXML.Excel;
using System.Xml;
using Newtonsoft.Json;
using System.Configuration;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Configuration;
using System.Threading.Tasks;
using Rotativa;

namespace HVAC.Views
{
    [SessionExpireFilter]
    public class JobHandoverController : Controller
    {

        HVACEntities db = new HVACEntities();
        public ActionResult Index()
        {

            JobHandOverSearch obj = (JobHandOverSearch)Session["JobHandOverSearch"];
            JobHandOverSearch model = new JobHandOverSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int RoleID = Convert.ToInt32(Session["UserRoleID"].ToString());
            int EmployeeId = 0;
            if (RoleID != 1)
            {
                var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
                EmployeeId = useremployee.EmployeeID;
            }

            if (obj == null)
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0;
                pFromDate = CommonFunctions.GetFirstDayofMonth().Date;
                pToDate = CommonFunctions.GetLastDayofMonth().Date;
                pFromDate = GeneralDAO.CheckParamDate(pFromDate, yearid).Date;
                pToDate = GeneralDAO.CheckParamDate(pToDate, yearid).Date;
                obj = new JobHandOverSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["JobHandOverSearch"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.EnquiryNo = "";
            }
            else
            {
                model = obj;
                model.FromDate = GeneralDAO.CheckParamDate(obj.FromDate, yearid).Date;
                model.ToDate = GeneralDAO.CheckParamDate(obj.ToDate, yearid).Date;
            }

            List<JobHandOverVM> lst = EnquiryDAO.JobHandOverList(model.FromDate, model.ToDate, model.EnquiryNo, model.ProjectNo, EmployeeId, branchid, yearid);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        public ActionResult Index(JobHandOverSearch obj)
        {
            Session["JobHandOverSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id=0,int EnquiryID=0)
        {

                   int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ViewBag.UserRoleId = Convert.ToInt32(Session["UserRoleID"].ToString());

            ViewBag.PaymentInstruments = db.PaymentInstruments.ToList();
            ViewBag.BondType = db.BondTypes.ToList();
            ViewBag.Warranty = db.Warranties.ToList();
            ViewBag.Enquiries = db.Enquiries.ToList();
            
            ViewBag.employee = db.EmployeeMasters.ToList();
             
            JobHandOverVM obj = new JobHandOverVM();
            ViewBag.DueDate = CommonFunctions.GetCurrentDateTime();
            if (id == 0)
            {
                ViewBag.Title = "Create";
               
                obj.JobDate = CommonFunctions.GetCurrentDateTime();
            
               
                
                obj.OrderDetails = new List<JobPurchaseOrderVM>();
                obj.BondDetails = new List<JobBondVM>();
                obj.WarrantyDetails = new List<QuotationWarrantyVM>();
                obj.PaymentDetails = new List<JobPaymentVM>();
            }
            else
            {
                ViewBag.Title = "Modify";

                JobHandover model = db.JobHandovers.Find(id);

                // Usage:
                 obj= CopyProperties<JobHandOverVM>(model);// Usage:


                var _enquiry = db.Enquiries.Find(obj.EnquiryID);

                obj.EnquiryNo = _enquiry.EnquiryNo;
                var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == obj.EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName }).FirstOrDefault();

                if (_client != null)
                {
                    obj.ClientName= _client.ClientName;
                    var _city = db.CityMasters.Find(_enquiry.CityID);
                    obj.DeliveryLocation = _city.City;
                }
                
                obj.OrderDetails = EnquiryDAO.GetJobPoList(obj.JobHandOverID);
                obj.WarrantyDetails = EnquiryDAO.GetJobWarrantyList(obj.JobHandOverID);
                obj.BondDetails = EnquiryDAO.GetJobBondList(obj.JobHandOverID);
                obj.PaymentDetails = EnquiryDAO.GetJobPaymentList(obj.JobHandOverID);

            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult ShowWarranty()
        {
            Warranty vm = new Warranty();
            return PartialView("WarrantyEntry", vm);
        }


        public static T CopyProperties<T>(object source) where T : new()
        {
            T target = new T();
            if (source == null) return target;

            var sourceProps = source.GetType().GetProperties();
            var targetProps = typeof(T).GetProperties();

            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(sp => sp.Name == targetProp.Name && sp.PropertyType == targetProp.PropertyType);
                if (sourceProp != null && sourceProp.CanRead && targetProp.CanWrite)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }

            return target;
        }

        // Render header partial as raw HTML
        public ActionResult PdfHeader(string id)
        {
            return View("~/Views/Shared/_PdfHeader.cshtml", id);
        }

        public ActionResult PrintPreview(int Id)
        {
            //string headerUrl = Url.Action("PdfHeader", "Jobs", new { id = vm.Job.JobNo }, Request.Url.Scheme);
            //var model = ReportsDAO.GetPurchaseOrderFromProcedure(Id);
            var model = GetJobDetail(Id);
            return View("Print", model);
        }

        // Export to PDF using Rotativa
        public ActionResult PrintPdf(int Id)
        {
            var model = GetJobDetail(Id);
            return new ViewAsPdf("Print", model)
            {
                FileName = "Job_" + model.Job.JobNo + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };
        }
        // Demo data. Replace with your repo/db fetch by JobNo/Id
        private JobDetailsViewModel GetJobDetail(int id)
        {
            return new JobDetailsViewModel
            {
                Job = new JobInfo
                {
                    JobNo =  "JOB-2025-001",
                    Title = "HVAC Installation – Block A",
                    ProjectSite = "Muscat, Oman",
                    Status = "In Progress",
                    StartDate = new DateTime(2025, 8, 1),
                    EndDate = null,
                    ProjectManager = "Ravi K",
                    Notes = "Phase-1 ducting in progress. Safety induction completed."
                },
                Customer = new CustomerInfo
                {
                    Name = "Airmech Oman LLC",
                    Code = "AIRM001",
                    ContactPerson = "Mr. Faisal",
                    Phone = "+968-24xxxxx",
                    Email = "faisal@airmech.om",
                    BillingAddress = "P.O. Box 123, Muscat, Oman",
                    ShippingAddress = "Project Site – Block A, Muscat",
                    TRN_VAT = "OM123456789"
                },
                PurchaseOrders = new List<POInfo>
                {
                    new POInfo { PONo="PO-1001", PODate=new DateTime(2025,8,5), Vendor="Al Jazeera Metals", Currency="OMR", Amount=3250.000m, Status="Delivered" },
                    new POInfo { PONo="PO-1002", PODate=new DateTime(2025,8,12), Vendor="Gulf Fasteners", Currency="OMR", Amount=1140.500m, Status="Partially Delivered" },
                    new POInfo { PONo="PO-1003", PODate=new DateTime(2025,8,18), Vendor="Desert Ducting Co.", Currency="OMR", Amount=5600.000m, Status="Approved" }
                },
                Bonds = new List<BondInfo>
                {
                    new BondInfo { BondType="Performance", BondNo="PB-7890", ValidFrom=new DateTime(2025,8,1), ValidTo=new DateTime(2026,7,31), Amount=10000m, Issuer="Bank Muscat", Remarks="10% of contract value" },
                    new BondInfo { BondType="Advance", BondNo="AB-4521", ValidFrom=new DateTime(2025,8,1), ValidTo=new DateTime(2026,1,31), Amount=7500m, Issuer="Bank Muscat", Remarks="Will be released after 50% progress" }
                },
                WarrantyTerms = new List<string>
                {
                    "12 months from commissioning date or 18 months from delivery, whichever is earlier.",
                    "Manufacturer defects covered; wear-and-tear excluded.",
                    "Response time within 24 hours for critical failures."
                },
                PaymentTerms = new List<string>
                {
                    "20% Advance against Advance Bank Guarantee.",
                    "50% on material delivery to site.",
                    "20% on installation completion.",
                    "10% on final handover (retention)."
                },
                Costing = new CostingInfo
                {
                    Materials = 6200m,
                    Labor = 2800m,
                    Subcontract = 1400m,
                    Overheads = 950m,
                    Contingency = 350m,
                    Taxes = 420m,
                    Discount = 200m,
                    QuotedPrice = 15000m
                }
            };
        }

        // View page
        public ActionResult Details(int id)
        {
            var vm = GetJobDetail(id);
            return View(vm);
        }

        public ActionResult DetailsPdf(int id)
        {
            var vm = GetJobDetail(id);

            string headerUrl = Url.Action("PdfHeader", "Jobs", new { id = vm.Job.JobNo }, Request.Url.Scheme);

            return new Rotativa.ViewAsPdf("Details", vm)
            {
                FileName = $"Job_{vm.Job.JobNo}.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(20, 10, 20, 10),
                CustomSwitches =
                    $"--print-media-type " +
                    $"--disable-smart-shrinking " +
                    $"--header-html {headerUrl} " +
                    $"--header-spacing 4 " +
                    $"--footer-center \"Page [page] of [topage]\" " +
                    $"--footer-font-size 8 --footer-spacing 4"
            };
        }        

    }
}