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

namespace HVAC.Controllers
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        public ActionResult PdfFooter()
        {
            // You can pass a model if needed, but for a static footer, it's not required.
            return View("_PdfFooter");
        }
        public ActionResult PdfHeader()
        {
            // You can pass a model if needed, but for a static footer, it's not required.
            return View("_PdfHeader");
        }
        // Export to PDF using Rotativa
        public ActionResult PrintPdf(int Id)
        {
            // Use the absolute server path to the header/footer files
            string headerUrl = Server.MapPath("~/Content/HeadersAndFooters/Header.html");
            string footerUrl = Server.MapPath("~/Content/HeadersAndFooters/Footer.html");
            // Dynamically get the full URL for the header and footer actions.
            //string headerUrl = Url.Action("PdfHeader", "JobHandover", null, Request.Url.Scheme);
            //string footerUrl = Url.Action("PdfFooter", "JobHandover", null, Request.Url.Scheme);
            var model = GetJobDetail(Id);
            // Construct a single string for all custom switches.
            //$"--print-media-type " +
            string customSwitches =  $"--header-html {headerUrl} " +
                                    $"--header-spacing 4 " +
                                    $"--footer-html {footerUrl} " +
                                    $"--footer-spacing 30";

            return new ViewAsPdf("Print", model)
            {
                FileName = "Job_" + model.Job.JobNo + ".pdf",
                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(30, 10, 50, 10),
                CustomSwitches = customSwitches
            };

            //var FooterHtml = Url.Action("PdfFooter", "JobHandover", null, Request.Url.Scheme);
           
            //string headerUrl = Url.Action("PdfHeader", "JobHandover", null, Request.Url.Scheme);
       
          
            //return new ViewAsPdf("Print", model)
            //{
            //    FileName = "Job_" + model.Job.JobNo + ".pdf",
            //    PageSize = Rotativa.Options.Size.A3,
            //    PageOrientation = Rotativa.Options.Orientation.Portrait,
            //    PageMargins = new Rotativa.Options.Margins(10, 10,40, 10),                  
            //    CustomSwitches =
            //        //$"--print-media-type " +
            //        //$"--disable-smart-shrinking " +
            //        $"--header-html {headerUrl} " +
            //        $"--header-spacing 4 " +
            //        $"--footer-html {FooterHtml} " +
            //        $"--footer-spacing 30 " +
            //        $"--footer-center \"page [page] of [topage]\" " +
            //        $"--footer-font-size 8 --footer-spacing 30"
            //};
        }
        // Demo data. Replace with your repo/db fetch by JobNo/Id
        private JobDetailsViewModel GetJobDetail(int id)
        {
            var _job = db.JobHandovers.Find(id);
            var _enquiry = db.Enquiries.Find(_job.EnquiryID);
             var _customer = db.ClientMasters.Find(_job.ClientID);
            var _polist = EnquiryDAO.JobwisePOList(id);
            var _quotationid = 0;
            if (_polist.Count > 0)
                _quotationid = Convert.ToInt32(_polist[0].QuotationId);

            if (_job != null)
            {
                return new JobDetailsViewModel
                {
                    Job = new JobInfo
                    {
                        JobNo = _job.ProjectNumber,
                        Title = _job.ProjectTitle,
                        ProjectSite = _job.DeliveryLocation,
                        Status = "Status",
                        StartDate = _enquiry.EnquiryDate,
                        EndDate = null,
                        ProjectManager = "Ravi K",
                        Notes = _enquiry.ProjectDescription,
                        JobValue =_job.JobValue,
                        Cost = _job.JobCost,
                        Vat = _job.VatAmount,
                        Margin = _job.Margin,
                        TotalValue = _job.TotalValue
                    },
                    Customer = new CustomerInfo
                    {
                        Name = _customer.ClientName,
                        Code = _customer.ClientPrefix,
                        ContactPerson = _customer.ContactName,
                        Phone = _customer.ContactNo,
                        Email = _customer.Email,
                        BillingAddress = _customer.Address1 + "," + _customer.Address2,
                        //ShippingAddress = "Project Site – Block A, Muscat",
                        //TRN_VAT = "OM123456789"
                    },
                    PurchaseOrders = _polist,
                    BondDetails  = EnquiryDAO.GetJobBondList(id),
                    WarrantyDetails = EnquiryDAO.GetJobWarrantyList(id),
                    Costing = EnquiryDAO.ClientPOEquipment(_quotationid)
            };
            }
            else
            {
                return new JobDetailsViewModel(); 
            }
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