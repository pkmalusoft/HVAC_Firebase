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
    public class InwardPOController : Controller
    {

        HVACEntities db = new HVACEntities();
        private  NotificationDAO _notificationDAO;
        public ActionResult Index()
        {

            JobPOSearch obj = (JobPOSearch)Session["JobPOSearch"];
            JobPOSearch model = new JobPOSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int RoleID = Convert.ToInt32(Session["UserRoleID"].ToString());
            int EmployeeId = 0;
            ViewBag.EmployeeMaster = db.EmployeeMasters.OrderBy(cc => cc.FirstName).ToList();
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
                obj = new JobPOSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["JobPOSearch"] = obj;
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

            List<JobPurchaseOrderVM> lst = EnquiryDAO.JobInwardPOList(model.FromDate, model.ToDate, model.PONo, model.ProjectNo, EmployeeId, branchid, yearid);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(JobPOSearch obj)
        {
            Session["JobPOSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id = 0, int QuotationID = 0)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            ViewBag.BondType = db.BondTypes.OrderBy(cc => cc.BondType1).ToList();
            ViewBag.PaymentInstrument = db.PaymentInstruments.OrderBy(cc => cc.Instruments).ToList();
            ViewBag.Warranty = db.Warranties.OrderBy(cc => cc.WarrantyType).ToList();
            ViewBag.EmployeeMaster = EnquiryDAO.GetDropdownData("Employee", "");
            int JobID = 0;

            Enquiry enquiry = new Enquiry();
            int EnquiryID = 0;
            string EnquiryNo = "";
            string QuotationNo = "";
            decimal Ordervalue = 0;


               
            decimal discount = 0;
            decimal VatPercent = 0;
            decimal TotalValue = 0;
            decimal Vatamount = 0;
            decimal DiscountPercent = 0;
            decimal DiscountAmount = 0;
            if (QuotationID > 0 && id == 0)
            {
                var _quotation = db.Quotations.Find(QuotationID);
                if (_quotation != null)
                {
                    EnquiryID = _quotation.EnquiryID;
                    QuotationNo = _quotation.QuotationNo;
                    if (_quotation.DiscountAmount != null)
                        discount = Convert.ToDecimal(_quotation.DiscountAmount);

                    
                    if (_quotation.VATPercent != null)
                    {
                        VatPercent = Convert.ToDecimal(_quotation.VATPercent);
                        Vatamount = Convert.ToDecimal(_quotation.VATAmount);
                    }

                    if (_quotation.QuotationValue != null)
                    {
                        Ordervalue = Convert.ToDecimal(_quotation.QuotationValue) - (discount + Vatamount);
                        TotalValue = Convert.ToDecimal(_quotation.QuotationValue);
                    }
                    if (_quotation.DiscountPercent != null)
                    {
                        DiscountPercent = Convert.ToDecimal(_quotation.DiscountPercent);

                    }
                    if (_quotation.DiscountAmount != null)
                    {
                        DiscountAmount = Convert.ToDecimal(_quotation.DiscountAmount);
                    }
                }
            }

            Quotation _pomax = new Quotation();
            if (EnquiryID > 0)
            {
                enquiry = db.Enquiries.Find(EnquiryID);
                if (enquiry != null)
                {
                    if (enquiry.JobHandOverID != null)
                    {
                        JobID = Convert.ToInt32(enquiry.JobHandOverID);
                        EnquiryNo = enquiry.EnquiryNo;

                        //_pomax = EnquiryDAO.GetJobMaxPONo(JobID, BranchID, fyearid);

                    }
                }
            }


            ViewBag.UserRoleId = Convert.ToInt32(Session["UserRoleID"].ToString());

            ViewBag.employee = db.EmployeeMasters.ToList();

            JobPurchaseOrderVM vm = new JobPurchaseOrderVM();
            ViewBag.DueDate = CommonFunctions.GetCurrentDateTime();
            if (id == 0 & JobID > 0)
            {
                ViewBag.Title = "Create";

                var _job = db.JobHandovers.Find(JobID);

                vm.JobDate = _job.JobDate;
                vm.ProjectNumber = _job.ProjectNumber;
                vm.ProjectName = _job.ProjectTitle;
                vm.Site = _job.Site;
                vm.QuotationId = QuotationID;
                vm.QuotationNo = QuotationNo;
                vm.EnquiryNo = EnquiryNo;
                vm.OrderValue = Ordervalue;
                vm.VatPercent = VatPercent;
                vm.VatAmount = Vatamount;
                vm.TotalValue = TotalValue;
                vm.OrderValue = Ordervalue;
                vm.DiscountPercent = DiscountPercent;
                vm.DiscountAmount = DiscountAmount;


                if (enquiry != null)
                    vm.JobHandOverID = Convert.ToInt32(enquiry.JobHandOverID);

                var _client = (from c in db.ClientMasters where c.ClientID == _job.ClientID select new { ClientID = c.ClientID, ClientName = c.ClientName }).FirstOrDefault();

                if (_client != null)
                {
                    vm.ClientName = _client.ClientName;
                }
                vm.PONumber = "";// _pomax.QuotationNo; //this max pono.
                var maxVarNo = db.JobPurchaseOrderDetails
                .Where(cc => cc.JobHandOverID == vm.JobHandOverID)
                .Select(cc => cc.VarNo)
                .DefaultIfEmpty(-1)
                .Max();

                vm.VarNo = maxVarNo+1;
                vm.PODate = CommonFunctions.GetCurrentDateTime();
                if (vm.QuotationId != null)
                {
                    vm.Details = EnquiryDAO.ClientPOEquipment(Convert.ToInt32(vm.QuotationId));
                }
                else
                {
                    vm.Details = new List<QuotationDetailVM>();
                }
                vm.BondDetails = new List<JobBondVM>();
                vm.WarrantyDetails = new List<QuotationWarrantyVM>();
            }
            else if (id > 0)
            {
                ViewBag.Title = "Modify";

                JobPurchaseOrderDetail model = db.JobPurchaseOrderDetails.Find(id);

                // Usage:
                vm = CopyProperties<JobPurchaseOrderVM>(model);// Usage:
                var _quotation = db.Quotations.Find(model.QuotationId);
                vm.MRequestNo = "";
                if (model.MRequestID != null)
                {
                    var _request = db.MaterialRequests.Find(Convert.ToInt32(model.MRequestID));
                    if (_request != null)
                    {
                        vm.MRequestNo = _request.MRNo;

                    }
                }
                
                 

                if (_quotation != null)
                {
                    EnquiryID = _quotation.EnquiryID;
                    if (EnquiryID > 0)
                    {
                        enquiry = db.Enquiries.Find(EnquiryID);
                        EnquiryNo = enquiry.EnquiryNo;                        
                    }

                    QuotationNo = _quotation.QuotationNo;
                    vm.EnquiryNo = EnquiryNo;
                    vm.QuotationNo = QuotationNo;
                    if (_quotation.DiscountAmount != null)
                        discount = Convert.ToDecimal(_quotation.DiscountAmount);

                    decimal vatamount = 0;
                    if (_quotation.VATAmount != null)
                        vatamount = Convert.ToDecimal(_quotation.VATAmount);

                    if (_quotation.QuotationValue != null)
                    {
                        Ordervalue = Convert.ToDecimal(_quotation.QuotationValue) - discount- vatamount;
                        TotalValue = Convert.ToDecimal(_quotation.QuotationValue);
                    }
                    if (_quotation.VATPercent != null)
                    {
                        VatPercent = Convert.ToDecimal(_quotation.VATPercent);
                        Vatamount = Convert.ToDecimal(_quotation.VATAmount);
                    }
                }

                EnquiryID = _quotation.EnquiryID;

                var _job = db.JobHandovers.Find(model.JobHandOverID);
                if (_job != null)
                {
                    vm.JobDate = _job.JobDate;
                    vm.ProjectNumber = _job.ProjectNumber;
                    vm.ProjectName = _job.ProjectTitle;
                    vm.Site = _job.Site;
                    vm.JobDate = _job.JobDate;

                    var _client = (from c in db.ClientMasters where c.ClientID == _job.ClientID select new { ClientID = c.ClientID, ClientName = c.ClientName }).FirstOrDefault();

                    if (_client != null)
                    {
                        //vm.ClientID = _job.ClientID;
                        vm.ClientName = _client.ClientName;
                    }

                }
                if (vm.QuotationId != null)
                {
                    vm.Details = EnquiryDAO.ClientPOEquipment(Convert.ToInt32(vm.QuotationId));
                    vm.BondDetails = EnquiryDAO.GetJobInwardBondList(vm.ID,0);
                    vm.WarrantyDetails = EnquiryDAO.ClientPOWarranty(vm.ID);
                    if (vm.WarrantyDetails == null)
                    {
                        vm.WarrantyDetails = new List<QuotationWarrantyVM>();
                    }
                }
                else
                {
                    vm.Details = new List<QuotationDetailVM>();
                    vm.BondDetails = EnquiryDAO.GetJobInwardBondList(vm.ID, 0);
                    vm.WarrantyDetails = EnquiryDAO.ClientPOWarranty(vm.ID);
                    if (vm.WarrantyDetails == null)
                    {
                        vm.WarrantyDetails = new List<QuotationWarrantyVM>();
                    }
                }

            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePO(JobPurchaseOrderVM obj)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string UserName = Session["UserName"].ToString();

            JobPurchaseOrderDetail model = new JobPurchaseOrderDetail();
            //JobPurchaseOrderVM model = new JobPurchaseOrderVM();
            if (obj.ID == 0)
            {
                model = new JobPurchaseOrderDetail();
                model.PONumber = obj.PONumber;
                model.VarNo = obj.VarNo;
                model.QuotationId = obj.QuotationId;
                model.JobHandOverID = obj.JobHandOverID; 
            }

            model.PODate = obj.PODate;
            model.OrderValue = obj.OrderValue;
            model.TotalValue = obj.TotalValue;
            model.VatPercent = obj.VatPercent;
            model.VatAmount = obj.VatAmount;
            model.VarNo = obj.VarNo;


            if (obj.ID == 0)
            {
                model.CreatedBy = userid;
                model.CreatedDate = CommonFunctions.GetCurrentDateTime();
                db.JobPurchaseOrderDetails.Add(model);
                db.SaveChanges();

                EnquiryDAO.UpdateClientPOWarranty(Convert.ToInt32(model.QuotationId),model.ID,model.JobHandOverID);
                EnquiryDAO.UpdateJobcost(model.JobHandOverID,model.ID);
            }
            else
            {
                model = db.JobPurchaseOrderDetails.Find(obj.ID);
                model.PONumber = obj.PONumber;
                model.PODate = obj.PODate;
                model.ModifiedBy = userid;
                model.ModifiedDate = CommonFunctions.GetBranchDateTime();
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                EnquiryDAO.UpdateJobcost(model.JobHandOverID,model.ID);

            }




            if (obj.ID == 0)
            {
                return Json(new { Id = model.ID, message = "PO Added Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Id = model.ID, message = "PO Updated Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveBond(JobBondVM obj)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string UserName = Session["UserName"].ToString();

            JobBondDetail model = new JobBondDetail();
            try
            {
                if (obj.ID == 0)
                {
                    model = new JobBondDetail();
                    model.BondTypeID = obj.BondTypeID;
                    model.SalesValue = obj.SalesValue;
                    model.Percentage = obj.Percentage;
                    model.JobHandOverID = obj.JobHandOverID;
                    model.BondValue = obj.BondValue;
                    model.BondIssueDate = obj.BondIssueDate;
                    model.BondExpiryDate = obj.BondExpiryDate;
                    model.BondValidity = obj.BondValidity;
                    model.JobPurchaseOrderDetailID = obj.JobPurchaseOrderDetailID;
                    model.CurrencyID = 1;
                }



                if (obj.ID == 0)
                {
                    model.CreatedBy = userid;
                    model.CreatedDate = CommonFunctions.GetBranchDateTime();
                    db.JobBondDetails.Add(model);
                    db.SaveChanges();
                }
                else
                {

                    model = db.JobBondDetails.Find(obj.ID);
                    model.ID = obj.ID;
                    model.BondTypeID = obj.BondTypeID;
                    model.SalesValue = obj.SalesValue;
                    model.Percentage = obj.Percentage;
                    model.JobHandOverID = obj.JobHandOverID;
                    model.BondValue = obj.BondValue;
                    model.BondIssueDate = obj.BondIssueDate;
                    model.BondExpiryDate = obj.BondExpiryDate;
                    model.BondValidity = obj.BondValidity;
                    model.ModifiedBy = userid;
                    model.ModifiedDate = CommonFunctions.GetBranchDateTime();
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }

            }catch(Exception ex)
            {
                return Json(new { Id = 0, message = "Fill the missing Details to save", status = "Failed" }, JsonRequestBehavior.AllowGet);
            }


            if (obj.ID == 0)
            {
                return Json(new { Id = model.ID, message = "Bond Added Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Id = model.ID, message = "Bond Updated Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ShowBondList(int id)
        {

            JobPurchaseOrderVM vm = new JobPurchaseOrderVM();

            vm.BondDetails = EnquiryDAO.GetJobInwardBondList(id, 0);

            return PartialView("BondList", vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBond(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            
            var _bond = db.JobBondDetails.Find(id);
            var poid = _bond.JobPurchaseOrderDetailID;
            _bond.IsDeleted = true;
            _bond.DeletedBy = userid;
            _bond.DeletedDate = CommonFunctions.GetBranchDateTime();
            db.Entry(_bond).State = EntityState.Modified;
            db.SaveChanges();

            JobPurchaseOrderVM vm = new JobPurchaseOrderVM();

            vm.BondDetails = EnquiryDAO.GetJobInwardBondList(Convert.ToInt32(poid), 0);

            return PartialView("BondList", vm);
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


        public JsonResult DeleteConfirmed(int id)
        {
            int userid = Convert.ToInt32(Session["UserID"].ToString());

            JobPurchaseOrderDetail protype = db.JobPurchaseOrderDetails.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                EnquiryDAO.DeleteClientPOWarranty(id, userid);
                return Json(new { status = "OK", message = "Client PO Deleted Successfully!" });

            }

        }

        public ActionResult ReportPrint(int Id)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Quotation Printing";
            var QuotationID = db.JobPurchaseOrderDetails.Find(Id).QuotationId;

            ViewBag.Client = EnquiryDAO.GetQuotationClient(Convert.ToInt32(QuotationID));
            ReportsDAO.InwardPOReport(Id, 0);
            QuotationVM vm = new QuotationVM();
            vm.QuotationID = Id;
            vm.ClientID = 0;
            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GenerateMRequest(int ClientPOID,int Storekeeperid)
        {
            int userid = Convert.ToInt32(Session["UserID"].ToString());

            JobPurchaseOrderDetail protype = db.JobPurchaseOrderDetails.Find(ClientPOID);
            var employeeId = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault().EmployeeID;

            StatusModel _model=EnquiryDAO.GenerateMaterialRequest(protype.JobHandOverID, ClientPOID, employeeId, Storekeeperid, userid);

            JobHandover v = db.JobHandovers.Find(protype.JobHandOverID);
            
            int transactionId = 555;
            string JobNo = v.ProjectNumber;
            string ProjectTitle = v.ProjectTitle;
            string type = "Material Request";
            string title = "Approval Pending!";
            string msg = "Material Request is done on on Job No. : " + JobNo + " Project :" + ProjectTitle;
            _notificationDAO = new NotificationDAO();
            int notificationId = _notificationDAO.SaveNotification(userid, transactionId, type, title, msg);

            if (_model == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });
            }
            else
            {
                return Json(new { status = _model.Status, message = _model.Message });
            }
        }


        #region printoption
        public ActionResult PrintPreview(int Id)
        {
            //var model = ReportsDAO.GetPurchaseOrderFromProcedure(Id);
            var model = GetSamplePO(Id);
            return View("Print", model);
        }
        
        // Export to PDF using Rotativa
        public ActionResult PrintPdf(int Id)
        {
            var model = GetSamplePO(Id);
            return new ViewAsPdf("Print", model)
            {
                FileName = "ClientPO_" + model.JobNo + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };
        }
        public ProjectHandoverModel GetSamplePO(int Id)
        {
            var inwardpo = db.JobPurchaseOrderDetails.Find(Id);
            var job = db.JobHandovers.Find(inwardpo.JobHandOverID);
            int clientid = Convert.ToInt32(job.ClientID);
            var client = db.ClientMasters.Find(clientid);
            var Quotation = db.Quotations.Find(inwardpo.QuotationId);
            var emp = db.EmployeeMasters.Where(cc => cc.EmployeeID == Quotation.EngineerID).FirstOrDefault();
            var handledby = emp.FirstName + " " + emp.LastName;

            List<QuotationDetailVM> Detail = new List<QuotationDetailVM>();

            if (inwardpo.QuotationId != null)
            {
                Detail = EnquiryDAO.ClientPOEquipment(Convert.ToInt32(inwardpo.QuotationId));            
            }
            
            var model = new ProjectHandoverModel
            {
                ReportDate = DateTime.Now,
                ProjectTitle = job.ProjectTitle,
                JobNo = job.ProjectNumber,
                Variation = 0,
                Contractor = client.ClientName,

                PONo = inwardpo.PONumber,
                PODate = inwardpo.PODate,
                OrderValue = inwardpo.OrderValue,
                VAT =  Convert.ToDecimal(inwardpo.VatAmount), 
                TotalValue = inwardpo.TotalValue,

                JobValue = CommonFunctions.ParseDecimal(job.JobValue.ToString()),
                TotalCost = CommonFunctions.ParseDecimal(job.JobCost.ToString()),
                Margin = CommonFunctions.ParseDecimal(job.Margin.ToString()),
                MarginPercent = Convert.ToDecimal(job.MarginPercent),

                PaymentTerms = Quotation.PaymentTerms , // "LC 120 Days",
                HandledBy = handledby, // "Manjunatha Malleshappa",

                MainUnitWarranty = "1 Yr",
                CompressorWarranty = "5 Yr",
                CondenserWarranty = "1 Yr",
                Refrigerant = "R410a",
                WarrantyStart = "Date of Commissioning",

                DeliveryLocation = "Site",
                DeliveryPlace = "Ibri",
                DeliveryPeriod = "16 Weeks",

                AdvanceBond = "No",
                PerformanceBond = "No",
                CorporateBond = "No",
                AnyBond = "No",

                CostingList = new List<CostingItem>
                {
                    new CostingItem { SlNo = 1, Item = "AHU", UnitModelNo = "CLCP011", Qty = 2, Unit = "Nos", UnitRate = 3543.00m, TotalRate = 7086.000m, Remarks = "" },
                    new CostingItem { SlNo = 2, Item = "ACCU", UnitModelNo = "RAUJC60", Qty = 2, Unit = "Nos", UnitRate = 13625.00m, TotalRate = 27250.000m, Remarks = "" },
                    new CostingItem { SlNo = 3, Item = "Coating", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 426.80m, TotalRate = 426.800m, Remarks = "" },
                    new CostingItem { SlNo = 4, Item = "Freight", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 2114.60m, TotalRate = 2114.600m, Remarks = "" },
                    new CostingItem { SlNo = 5, Item = "Warranty", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 776.00m, TotalRate = 776.000m, Remarks = "" },
                    new CostingItem { SlNo = 6, Item = "Factory Visit", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 2250.00m, TotalRate = 2250.000m, Remarks = "" },
                    new CostingItem { SlNo = 7, Item = "Business Promotion", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 920.00m, TotalRate = 920.000m, Remarks = "" }
                }
            };

            model.CostingDetails = Detail;
            //var model = new ProjectHandoverModel
            //{
            //    ReportDate = DateTime.Now,
            //    ProjectTitle = "Daleel - Utility Building",
            //    JobNo = "TVA0160",
            //    Variation = 0,
            //    Contractor = "Mectron International LLC",

            //    PONo = "MI-PO-1298-2025",
            //    PODate = new DateTime(2025, 5, 27),
            //    OrderValue = 46000.000m,
            //    VAT = 2300.000m,
            //    TotalValue = 48300.000m,

            //    JobValue = 46000.000m,
            //    TotalCost = 40823.400m,
            //    Margin = 5176.600m,
            //    MarginPercent = 11.25m,

            //    PaymentTerms = "LC 120 Days",
            //    HandledBy = "Manjunatha Malleshappa",

            //    MainUnitWarranty = "1 Yr",
            //    CompressorWarranty = "5 Yr",
            //    CondenserWarranty = "1 Yr",
            //    Refrigerant = "R410a",
            //    WarrantyStart = "Date of Commissioning",

            //    DeliveryLocation = "Site",
            //    DeliveryPlace = "Ibri",
            //    DeliveryPeriod = "16 Weeks",

            //    AdvanceBond = "No",
            //    PerformanceBond = "No",
            //    CorporateBond = "No",
            //    AnyBond = "No",

            //    CostingList = new List<CostingItem>
            //    {
            //        new CostingItem { SlNo = 1, Item = "AHU", UnitModelNo = "CLCP011", Qty = 2, Unit = "Nos", UnitRate = 3543.00m, TotalRate = 7086.000m, Remarks = "" },
            //        new CostingItem { SlNo = 2, Item = "ACCU", UnitModelNo = "RAUJC60", Qty = 2, Unit = "Nos", UnitRate = 13625.00m, TotalRate = 27250.000m, Remarks = "" },
            //        new CostingItem { SlNo = 3, Item = "Coating", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 426.80m, TotalRate = 426.800m, Remarks = "" },
            //        new CostingItem { SlNo = 4, Item = "Freight", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 2114.60m, TotalRate = 2114.600m, Remarks = "" },
            //        new CostingItem { SlNo = 5, Item = "Warranty", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 776.00m, TotalRate = 776.000m, Remarks = "" },
            //        new CostingItem { SlNo = 6, Item = "Factory Visit", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 2250.00m, TotalRate = 2250.000m, Remarks = "" },
            //        new CostingItem { SlNo = 7, Item = "Business Promotion", UnitModelNo = "", Qty = 1, Unit = "Lot", UnitRate = 920.00m, TotalRate = 920.000m, Remarks = "" }
            //    }
            //};
            return model;
        }

        #endregion
    }
}