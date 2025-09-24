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
using Ganss.Xss;

namespace HVAC.Controllers
{

    [SessionExpireFilter]
    public class QuotationController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: Quotationroller
        [OutputCache(Duration = 120, VaryByParam = "none")]
        public ActionResult Index()
        {
            try
            {
                QuotationSearch obj = (QuotationSearch)Session["QuotationSearch"];
                QuotationSearch model = new QuotationSearch();
                int branchid = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
                int yearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
                int userid = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"].ToString()) : 0;
                int RoleID = Session["UserRoleID"] != null ? Convert.ToInt32(Session["UserRoleID"].ToString()) : 0;
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
                obj = new QuotationSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["QuotationSearch"] = obj;
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

                List<QuotationVM> lst = EnquiryDAO.QuotationList(model.FromDate, model.ToDate, model.EnquiryNo, model.QuotationNo, EmployeeId, branchid, yearid);
                model.Details = lst;

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging framework)
                ModelState.AddModelError("", "An error occurred while loading quotations. Please try again.");
                return View(new QuotationSearch());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(QuotationSearch obj)
        {
            if (ModelState.IsValid)
            {
                Session["QuotationSearch"] = obj;
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        public ActionResult Create(int id = 0, string EnquiryNo = "", int QuotationId = 0)
        {
            try
            {
                int EnquiryID = 0;
                if (EnquiryNo != "")
                {
                    var _Enquiry = db.Enquiries.Where(cc => cc.EnquiryNo == EnquiryNo).FirstOrDefault();
                    EnquiryID = _Enquiry?.EnquiryID ?? 0;
                }
                QuotationVM vm = new QuotationVM();
                int fyearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
                int branchId = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
                int userid = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"].ToString()) : 0;
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            ViewBag.EstimationCategory = db.EstimationCategories.Where(cc => cc.ID != 4).ToList();
            ViewBag.Unit = db.ItemUnits.ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.PaymentTerms = db.PaymentTerms.ToList();
            if (useremployee != null)
            {
                ViewBag.Enquiry = EnquiryDAO.GetEmployeeEnquiry(useremployee.EmployeeID, branchId, fyearid);
            }
            else
            {
                List<EnquiryVM> _enquirylist = new List<EnquiryVM>();
                ViewBag.Enquiry = _enquirylist;
            }

            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.QuotationStatus = db.QuotationStatus.ToList();

            //ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductFamilyName).ToList();
            
            

            vm.QuotationDetails = new List<QuotationDetailVM>();

            if (useremployee != null)
                vm.EngineerID = useremployee.EmployeeID;

            if (QuotationId > 0) //new revision
            {
                Quotation item = db.Quotations.Find(QuotationId);
                vm.QuotationID = 0;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.QuotationNo = item.QuotationNo;
                vm.Version = db.Quotations.Where(cc => cc.QuotationNo == vm.QuotationNo).Select(cc => cc.Version).Max() + 1;
                vm.QuotationDate = item.QuotationDate;
                vm.MobileNumber = item.MobileNumber;
                vm.ContactPerson = item.ContactPerson;
                vm.ClientID = item.ClientID;
                vm.ClientDetail = item.ClientDetail;

                vm.CurrencyId = item.CurrencyId;
                vm.Salutation = item.Salutation;
                vm.TermsandConditions = item.TermsandConditions;
                vm.PaymentTerms = item.PaymentTerms;
                vm.SubjectText = item.SubjectText;
                vm.QuotationValue = item.QuotationValue;
                vm.GrossAmount = Convert.ToDecimal(item.GrossAmount);
                vm.QuotationTo = item.QuotationTo;
                vm.EngineerID = item.EngineerID;
                vm.QuotationStatusID = item.QuotationStatusID;
                vm.DiscountAmount = item.DiscountAmount;
                vm.DiscountPercent = item.DiscountPercent;
                vm.VATPercent = item.VATPercent;
                vm.VATAmount = item.VATAmount;
                vm.DeliveryTerms = item.DeliveryTerms;
                vm.Validity = item.Validity;
                vm.ClientPOID = 0;

                if (item.QuotationStatusID > 0)
                {
                        var _status = db.QuotationStatus.Where(CC => CC.ID == vm.QuotationStatusID).FirstOrDefault();
                    vm.QuotationStatus = _status?.Status ?? "";
                }
                vm.QuotationDetails = EnquiryDAO.QuotationEquipment(item.EnquiryID, item.QuotationID);
                ViewBag.QuotationMode = "Revision";
                ViewBag.Title = "Revision";

                var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
                var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
                var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);
                var _detail4 = EnquiryDAO.QuotationTerms(id);
                var _detail5 = EnquiryDAO.GetQuotationContacts(id);
                if (_detail1 == null)
                {
                    _detail1 = new List<QuotationScopeofWorkVM>();
                }
                if (_detail2 == null)
                {
                    _detail2 = new List<QuotationWarrantyVM>();
                }
                if (_detail3 == null)
                {
                    _detail3 = new List<QuotationExclusionVM>();
                }

                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                if (_detail5 == null)
                {
                    _detail5 = new List<QuotationContactVM>();
                }

                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;

                var _EmployeeMaster = EnquiryDAO.GetEnquiryAssigneEmployees(vm.EnquiryID, useremployee.EmployeeID);

                if (_EmployeeMaster == null)
                {
                    _EmployeeMaster = new List<EmployeeVM>();
                    ViewBag.EmployeeMaster = _EmployeeMaster;
                }
                else
                {
                    ViewBag.EmployeeMaster = _EmployeeMaster;
                }
            }
            else if (id == 0 && EnquiryID == 0) //quotation id is 0 create mode
            {
                vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
                vm.CurrencyId = CommonFunctions.GetDefaultCurrencyId();
                vm.QuotationStatusID = 1;
                if (vm.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                ViewBag.Title = "Create";
                vm.ClientPOID = 0;
                var _detail1 = new List<QuotationScopeofWorkVM>();
                var _detail2 = new List<QuotationWarrantyVM>();
                var _detail3 = new List<QuotationExclusionVM>();
                var _detail5 = new List<QuotationContactVM>();
                var _detail4 = EnquiryDAO.QuotationTerms(0);
                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;
            }
            else if (id == 0 && EnquiryID > 0)
            {
                vm.EnquiryID = EnquiryID;
                var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName, ContactPerson = d.ContactName, Mobileno = d.ContactNo }).FirstOrDefault();

                if (_client != null)
                {
                    vm.ClientDetail = _client.ClientName;
                    vm.ClientID = _client.ClientID;
                    vm.ContactPerson = _client.ContactPerson;
                    vm.MobileNumber = _client.Mobileno;
                }
                var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0, Convert.ToInt32(vm.EngineerID));
                vm.QuotationNo = dta1.QuotationNo;
                vm.Version = dta1.Version;
                var defaultQuotationStatus = db.QuotationStatus.Where(cc => cc.DefaultStatus == true).FirstOrDefault();
                int defaultquotationstatusid = 0;
                if (defaultQuotationStatus != null)
                {
                    defaultquotationstatusid = defaultQuotationStatus.ID;
                }
                else
                {
                    defaultquotationstatusid = 1;
                }
                vm.QuotationStatusID = defaultquotationstatusid;
                vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
                if (vm.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                vm.CurrencyId = CommonFunctions.GetDefaultCurrencyId();
                vm.EnquiryID = EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.ProjectRef = vm.ProjectName;
                vm.VATPercent = 5;
                vm.ClientPOID = 0;
                vm.QuotationDetails = new List<QuotationDetailVM>();

                ViewBag.QuotationMode = "New";
                ViewBag.Title = "Create";

                var _detail1 = new List<QuotationScopeofWorkVM>();
                var _detail2 = new List<QuotationWarrantyVM>();
                var _detail3 = new List<QuotationExclusionVM>();
                var _detail5 = new List<QuotationContactVM>();
                var _detail4 = EnquiryDAO.QuotationTerms(0);
                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.TermsandConditions = "";
                vm.QuotationContacts = _detail5;


                //var _employee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
                
                vm.EngineerID = useremployee.EmployeeID;
                vm.EmployeeName = useremployee.FirstName + " " + useremployee.LastName;
                
                var _EmployeeMaster = EnquiryDAO.GetEnquiryAssigneEmployees(vm.EnquiryID, useremployee.EmployeeID);

                if (_EmployeeMaster == null)
                {
                    _EmployeeMaster = new List<EmployeeVM>();
                    ViewBag.EmployeeMaster = _EmployeeMaster;
                }
                else
                {
                    ViewBag.EmployeeMaster = _EmployeeMaster;
                }
            }
            else if (id > 0) //edit mode
            {

                 Quotation item = db.Quotations.Find(id);
                vm.QuotationID = item.QuotationID;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.QuotationNo = item.QuotationNo;
                vm.QuotationDate = item.QuotationDate;
                vm.MobileNumber = item.MobileNumber;
                vm.ContactPerson = item.ContactPerson;
                vm.ClientID = item.ClientID;
                vm.ClientDetail = item.ClientDetail;
                vm.Version = item.Version;
                vm.CurrencyId = item.CurrencyId;
                vm.Salutation = item.Salutation;
                vm.TermsandConditions = item.TermsandConditions;
                vm.PaymentTerms = item.PaymentTerms;
                vm.SubjectText = item.SubjectText;
                vm.QuotationValue = item.QuotationValue;
                vm.DiscountAmount = item.DiscountAmount;
                vm.DiscountPercent = item.DiscountPercent;
                vm.VATPercent = item.VATPercent;
                vm.VATAmount = item.VATAmount;
                vm.ClientPOID = item.ClientPOID;
                if (vm.ClientPOID>0)
                {
                    var poDetail = db.JobPurchaseOrderDetails.FirstOrDefault(p => p.ID == vm.ClientPOID);
                    vm.ClientPONO = poDetail?.PONumber ?? "";
                }
                vm.MarginPercent = Convert.ToDecimal(item.MarginPercent ?? 0); 
                vm.Margin = Convert.ToDecimal(item.Margin ?? 0);
                if (vm.VATAmount is null)
                    vm.VATAmount = 0;
                if (vm.DiscountAmount is null)
                    vm.DiscountAmount = 0;


                //vm.GrossAmount = Convert.ToDecimal(item.QuotationValue) - (Convert.ToDecimal(vm.VATAmount) + Convert.ToDecimal(vm.DiscountAmount));
                
                vm.GrossAmount = Convert.ToDecimal(item.GrossAmount ?? 0);
                vm.SellingValue = Convert.ToDecimal(item.SellingValue ?? 0);
                vm.EngineerID = item.EngineerID;
                vm.ProjectRef = item.ProjectRef;
                vm.QuotationTo = item.QuotationTo;
                vm.QuotationStatusID = item.QuotationStatusID;

                vm.DeliveryTerms = item.DeliveryTerms;
                vm.DirectQuotation = item.DirectQuotation;
                vm.Validity = item.Validity;
                if (item.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                vm.QuotationDetails = EnquiryDAO.QuotationEquipment(item.EnquiryID, item.QuotationID);

                var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
                //var _detail1 = EnquiryDAO.GetQuotationScopeGroups(id);
                var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
                var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);
                var _detail5 = EnquiryDAO.GetQuotationContacts(id);
              //  var _detail4 = EnquiryDAO.QuotationTerms(id);

                if (_detail1 == null)
                {
                    _detail1 = new List<QuotationScopeofWorkVM>();
                }
                if (_detail2 == null)
                {
                    _detail2 = new List<QuotationWarrantyVM>();
                }
                if (_detail3 == null)
                {
                    _detail3 = new List<QuotationExclusionVM>();
                }
                //if (_detail1 == null)
                //{
                //    _detail4 = new List<QuotationTermsVM>();
                //}
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                //vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;
                ViewBag.Title = "Modify";
                var _EmployeeMaster = db.EmployeeMasters.FirstOrDefault(e => e.EmployeeID == vm.EngineerID);
                vm.EmployeeName = _EmployeeMaster != null ? _EmployeeMaster.FirstName + " " + _EmployeeMaster.LastName : "";                

                //if (_EmployeeMaster == null)
                //{
                //    _EmployeeMaster = new List<EmployeeVM>();
                //    ViewBag.EmployeeMaster = _EmployeeMaster;
                //}
                //else
                //{
                //    ViewBag.EmployeeMaster = _EmployeeMaster;
                //}
            }

            return View(vm);
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging framework)
                ModelState.AddModelError("", "An error occurred while loading the quotation. Please try again.");
                return View(new QuotationVM());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveQuotation(Quotation quotation, List<QuotationDetailVM> Details)
        {
            int fyearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
            int branchId = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
            //ViewBag.Unit = db.ItemUnits.ToList();
            int userid = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"].ToString()) : 0;
            string UserName = Session["UserName"]?.ToString() ?? "";
            var IDetails = Details;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    QuotationDetail item = new QuotationDetail();
                    Quotation model = new Quotation();
                    if (quotation.QuotationID == 0)
                    {
                        model = new Quotation();
                        //var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, quotation.EnquiryID,0,Convert.ToInt32(quotation.EngineerID));
                        //model.QuotationNo = dta1.QuotationNo;
                        //model.Version = dta1.Version;
                        model.QuotationNo = quotation.QuotationNo;
                        model.Version = quotation.Version;
                        model.EngineerID = quotation.EngineerID;
                        model.ClientDetail = quotation.ClientDetail;
                        model.ClientID = quotation.ClientID;
                        model.EnquiryID = quotation.EnquiryID;

                    }
                    else
                    {
                        model = db.Quotations.Find(quotation.QuotationID);
                    }
                    model.QuotationStatusID = quotation.QuotationStatusID;
                    model.QuotationDate = quotation.QuotationDate;
                    model.PaymentTerms = quotation.PaymentTerms;
                    model.TermsandConditions = quotation.TermsandConditions;
                    model.Salutation = quotation.Salutation;
                    model.SubjectText = quotation.SubjectText;
                    model.QuotationTo = quotation.QuotationTo;
                    model.ProjectRef = quotation.ProjectRef;
                    model.Version = quotation.Version;
                    model.CurrencyId = quotation.CurrencyId;
                    //model.ContactPerson = quotation.ContactPerson;
                    //model.MobileNumber = quotation.MobileNumber;
                    model.QuotationValue = quotation.QuotationValue;
                    model.GrossAmount = quotation.GrossAmount;
                    model.MarginPercent = quotation.MarginPercent;
                    model.Margin = quotation.Margin;
                    model.DiscountPercent = quotation.DiscountPercent;
                    model.DiscountAmount = quotation.DiscountAmount;
                    model.VATPercent = quotation.VATPercent;
                    model.VATAmount = quotation.VATAmount;
                    model.PaymentTerms = quotation.PaymentTerms;
                    model.DeliveryTerms = quotation.DeliveryTerms;
                    model.Validity = quotation.Validity;
                    model.SellingValue = quotation.SellingValue;
                    model.DirectQuotation = quotation.DirectQuotation;
                    model.ModifiedBy = userid;
                    model.ModifiedDate = CommonFunctions.GetCurrentDateTime();

                    if (quotation.QuotationID == 0)
                    {
                        model.CreatedBy = userid;
                        model.CreatedDate = CommonFunctions.GetCurrentDateTime();

                        db.Quotations.Add(model);
                        db.SaveChanges();

                    }
                    else
                    {
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var qdetails = (from d in db.QuotationDetails where d.QuotationID == quotation.QuotationID select d).ToList();
                        db.QuotationDetails.RemoveRange(qdetails);
                        db.SaveChanges();
                    }


                    foreach (QuotationDetailVM detail in Details)
                    {
                        if (detail.Deleted == false && detail.Amount != null)
                        {

                            item = new QuotationDetail();
                            item.QuotationID = model.QuotationID;
                            //  item.EnquiryID = model.EnquiryID;
                            item.EquipmentID = detail.EquipmentID;
                            item.EstimationID = detail.EstimationID;
                            item.EstimationDetailID = detail.EstimationDetailID;
                            item.EstimationMasterID = detail.EstimationMasterID;
                            item.EstimationCategoryID = detail.EstimationCategoryID;
                            item.Model = detail.Model;
                            item.Description = detail.Description;
                            item.UnitID = detail.UnitID;
                            item.Quantity = detail.Quantity;
                            item.Amount = detail.Amount;
                            item.UnitRate = detail.UnitRate;
                            item.EquipmentStatusID = detail.EquipmentStatusID;
                            //item.Refrigerant = detail.Refrigerant;
                            //item.NominalCapacity = detail.NominalCapacity;
                            //item.EfficientType = detail.EfficientType;
                            //item.CreatedDate = CommonFunctions.GetBranchDateTime();
                            //item.ModifiedDate = CommonFunctions.GetBranchDateTime();
                            db.QuotationDetails.Add(item);
                        }

                    }

                    var _enquiry = db.Enquiries.AsNoTracking().FirstOrDefault(e => e.EnquiryID == model.EnquiryID);
                    if (_enquiry != null)
                    {
                        if (_enquiry.EnquiryStatusID == 5) //assigned
                        {
                            _enquiry.EnquiryStatusID = 3; //change to quoted
                            db.Entry(_enquiry).State = EntityState.Modified;
                            //  db.SaveChanges();
                        }
                    }
                    else
                    {
                        return Json(new { QuotationId = 0, message = "Error", status = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    if (quotation.QuotationID == 0)
                    {
                        EnquiryDAO.UpdateQuotationScopeofWork(model.QuotationID);
                        EnquiryDAO.UpdateQuotationContact(model.QuotationID);
                        EnquiryDAO.UpdateQuotationWarranty(model.QuotationID);
                        EnquiryDAO.UpdateQuotationExclusion(model.QuotationID);

                        GeneralDAO.SaveAuditLog("Quotation Added", _enquiry.EnquiryNo, "Enquiry Status");
                        
                    }
                    else
                    {
                        EnquiryDAO.UpdateQuotationContact(model.QuotationID);
                        GeneralDAO.SaveAuditLog("Quotation Updated", _enquiry.EnquiryNo, "Enquiry Status");
                        //return Json(new { QuotationId = model.QuotationID, message = "Quotation Updated Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    
                    if (quotation.DirectQuotation==true)
                       EnquiryDAO.SaveQuotationProfit(model.QuotationID);

                    return Json(new { QuotationId = model.QuotationID, message = "Quotation Saved Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
                }         
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
          }
            }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveQuotationold(Quotation quotation, List<QuotationDetailVM> Details)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //ViewBag.Unit = db.ItemUnits.ToList();
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string UserName = Session["UserName"].ToString();
            var IDetails = Details;
            //  var IDetails = JsonConvert.DeserializeObject<List<QuotationDetailVM>>(Details);
            //   var QuotationEditor = JsonConvert.DeserializeObject<QuotationEditorData>(quotationeditor);
            QuotationDetail item = new QuotationDetail();
            Quotation model = new Quotation();
            if (quotation.QuotationID == 0)
            {
                model = new Quotation();
                //var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, quotation.EnquiryID,0,Convert.ToInt32(quotation.EngineerID));
                //model.QuotationNo = dta1.QuotationNo;
                //model.Version = dta1.Version;
                model.QuotationNo = quotation.QuotationNo;
                model.Version = quotation.Version;
                model.EngineerID = quotation.EngineerID;
                model.ClientDetail = quotation.ClientDetail;
                model.ClientID = quotation.ClientID;
                model.EnquiryID = quotation.EnquiryID;

            }
            else
            {
                model = db.Quotations.Find(quotation.QuotationID);
            }
            model.QuotationStatusID = quotation.QuotationStatusID;
            model.QuotationDate = quotation.QuotationDate;
            model.PaymentTerms = quotation.PaymentTerms;
            model.TermsandConditions = quotation.TermsandConditions;
            model.Salutation = quotation.Salutation;
            model.SubjectText = quotation.SubjectText;
            model.QuotationTo = quotation.QuotationTo;
            model.ProjectRef = quotation.ProjectRef;
            model.Version = quotation.Version;
            model.CurrencyId = quotation.CurrencyId;
            //model.ContactPerson = quotation.ContactPerson;
            //model.MobileNumber = quotation.MobileNumber;
            model.QuotationValue = quotation.QuotationValue;
            model.DiscountPercent = quotation.DiscountPercent;
            model.DiscountAmount = quotation.DiscountAmount;
            model.VATPercent = quotation.VATPercent;
            model.VATAmount = quotation.VATAmount;
            model.PaymentTerms = quotation.PaymentTerms;
            model.DeliveryTerms = quotation.DeliveryTerms;
            model.Validity = quotation.Validity;

            //foreach (QuotationDetailVM detail in IDetails)
            //{
            //    if (detail.Deleted == false)
            //    {
            //        model.QuotationValue = model.QuotationValue + detail.Amount;
            //    }
            //}

            model.ModifiedBy = userid;
            model.ModifiedDate = CommonFunctions.GetCurrentDateTime();

            if (quotation.QuotationID == 0)
            {
                model.CreatedBy = userid;
                model.CreatedDate = CommonFunctions.GetCurrentDateTime();

                db.Quotations.Add(model);
                db.SaveChanges();

            }
            else
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var qdetails = (from d in db.QuotationDetails where d.QuotationID == quotation.QuotationID select d).ToList();
                db.QuotationDetails.RemoveRange(qdetails);
                db.SaveChanges();
            }


            foreach (QuotationDetailVM detail in IDetails)
            {
                if (detail.Deleted == false && detail.Amount != null)
                {

                    item = new QuotationDetail();
                    item.QuotationID = model.QuotationID;
                    //  item.EnquiryID = model.EnquiryID;
                    item.EquipmentID = detail.EquipmentID;
                    item.EstimationID = detail.EstimationID;
                    item.EstimationDetailID = detail.EstimationDetailID;
                    item.EstimationMasterID = detail.EstimationMasterID;
                    item.EstimationCategoryID = detail.EstimationCategoryID;
                    item.Model = detail.Model;
                    item.Description = detail.Description;
                    item.UnitID = detail.UnitID;
                    item.Quantity = detail.Quantity;
                    item.Amount = detail.Amount;
                    item.UnitRate = detail.UnitRate;
                    item.EquipmentStatusID = detail.EquipmentStatusID;
                    //item.Refrigerant = detail.Refrigerant;
                    //item.NominalCapacity = detail.NominalCapacity;
                    //item.EfficientType = detail.EfficientType;
                    //item.CreatedDate = CommonFunctions.GetBranchDateTime();
                    //item.ModifiedDate = CommonFunctions.GetBranchDateTime();
                    db.QuotationDetails.Add(item);
                    db.SaveChanges();

                    var _enquiry = db.Enquiries.Find(model.EnquiryID);
                    if (_enquiry != null)
                    {
                        if (_enquiry.EnquiryStatusID == 5) //assigned
                        {
                            _enquiry.EnquiryStatusID = 3; //change to quoted
                            db.Entry(_enquiry).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                }
            }
            var job = db.Enquiries.Find(quotation.EnquiryID);

            if (job == null)
            {
                return Json(new { QuotationId = 0, message = "Error", status = "Failed" }, JsonRequestBehavior.AllowGet);
            }
            //if (job.JobStatusId == 1) //Enquiry status
            //{
            //    job.StatusTypeId = 1;
            //    job.JobStatusId = 2; //--quotation status
            //    job.ModifiedBy = userid;
            //    job.ModifiedTime = CommonFunctions.GetCurrentDateTime();
            //    db.Entry(job).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //}
            if (quotation.QuotationID == 0)
            {
                EnquiryDAO.UpdateQuotationScopeofWork(model.QuotationID);
                EnquiryDAO.UpdateQuotationContact(model.QuotationID);
                EnquiryDAO.UpdateQuotationWarranty(model.QuotationID);
                EnquiryDAO.UpdateQuotationExclusion(model.QuotationID);
                var _enquiry = db.Enquiries.Find(model.EnquiryID);
                GeneralDAO.SaveAuditLog("Quotation Added", _enquiry.EnquiryNo, "Enquiry Status");
                return Json(new { QuotationId = model.QuotationID, message = "Quotation Added Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                EnquiryDAO.UpdateQuotationContact(model.QuotationID);
                var _enquiry = db.Enquiries.Find(model.EnquiryID);
                GeneralDAO.SaveAuditLog("Quotation Updated", _enquiry.EnquiryNo, "Enquiry Status");
                return Json(new { QuotationId = model.QuotationID, message = "Quotation Updated Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult QuotationDetail(int id = 0, string EnquiryNo = "", int QuotationId = 0)
        {
            int EnquiryID = 0;
            if (EnquiryNo != "")
            {
                var _Enquiry = db.Enquiries.Where(cc => cc.EnquiryNo == EnquiryNo).FirstOrDefault();
                EnquiryID = _Enquiry.EnquiryID;
            }
            QuotationVM vm = new QuotationVM();
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            ViewBag.EstimationCategory = db.EstimationCategories.Where(cc => cc.ID != 4).ToList();
            ViewBag.Unit = db.ItemUnits.ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.PaymentTerms = db.PaymentTerms.ToList();
            if (useremployee != null)
            {
                ViewBag.Enquiry = EnquiryDAO.GetEmployeeEnquiry(useremployee.EmployeeID, branchId, fyearid);
            }
            else
            {
                List<EnquiryVM> _enquirylist = new List<EnquiryVM>();
                ViewBag.Enquiry = _enquirylist;
            }

            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.QuotationStatus = db.QuotationStatus.ToList();


            var _EmployeeMaster = EnquiryDAO.GetEnquiryAssigneEmployees(EnquiryID, useremployee.EmployeeID);
            ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductFamilyName).ToList();
            if (_EmployeeMaster == null)
            {
                _EmployeeMaster = new List<EmployeeVM>();
                ViewBag.EmployeeMaster = _EmployeeMaster;
            }
            else
            {
                ViewBag.EmployeeMaster = _EmployeeMaster;
            }

            vm.QuotationDetails = new List<QuotationDetailVM>();

            if (useremployee != null)
                vm.EngineerID = useremployee.EmployeeID;

            if (QuotationId > 0) //new revision
            {
                Quotation item = db.Quotations.Find(QuotationId);
                vm.QuotationID = 0;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.QuotationNo = item.QuotationNo;
                vm.Version = db.Quotations.Where(cc => cc.QuotationNo == vm.QuotationNo).Select(cc => cc.Version).Max() + 1;
                vm.QuotationDate = item.QuotationDate;
                vm.MobileNumber = item.MobileNumber;
                vm.ContactPerson = item.ContactPerson;
                vm.ClientID = item.ClientID;
                vm.ClientDetail = item.ClientDetail;

                vm.CurrencyId = item.CurrencyId;
                vm.Salutation = item.Salutation;
                vm.TermsandConditions = item.TermsandConditions;
                vm.PaymentTerms = item.PaymentTerms;
                vm.SubjectText = item.SubjectText;
                vm.QuotationValue = item.QuotationValue;
                vm.QuotationTo = item.QuotationTo;

                vm.EngineerID = item.EngineerID;
                vm.QuotationStatusID = item.QuotationStatusID;
                vm.DiscountAmount = item.DiscountAmount;
                vm.DiscountPercent = item.DiscountPercent;
                vm.VATPercent = item.VATPercent;
                vm.VATAmount = item.VATAmount;
                vm.DeliveryTerms = item.DeliveryTerms;
                vm.Validity = item.Validity;

                if (item.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                vm.QuotationDetails = EnquiryDAO.QuotationEquipment(item.EnquiryID, item.QuotationID);
                ViewBag.QuotationMode = "Revision";
                ViewBag.Title = "Revision";

                var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
                var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
                var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);
                var _detail4 = EnquiryDAO.QuotationTerms(id);
                var _detail5 = EnquiryDAO.GetQuotationContacts(id);
                if (_detail1 == null)
                {
                    _detail1 = new List<QuotationScopeofWorkVM>();
                }
                if (_detail2 == null)
                {
                    _detail2 = new List<QuotationWarrantyVM>();
                }
                if (_detail3 == null)
                {
                    _detail3 = new List<QuotationExclusionVM>();
                }

                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                if (_detail5 == null)
                {
                    _detail5 = new List<QuotationContactVM>();
                }

                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;
            }
            else if (id == 0 && EnquiryID == 0) //quotation id is 0 create mode
            {
                vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
                vm.CurrencyId = CommonFunctions.GetDefaultCurrencyId();
                vm.QuotationStatusID = 1;
                if (vm.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                ViewBag.Title = "Create";

                var _detail1 = new List<QuotationScopeofWorkVM>();
                var _detail2 = new List<QuotationWarrantyVM>();
                var _detail3 = new List<QuotationExclusionVM>();
                var _detail5 = new List<QuotationContactVM>();
                var _detail4 = EnquiryDAO.QuotationTerms(0);
                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;
            }
            else if (id == 0 && EnquiryID > 0)
            {
                vm.EnquiryID = EnquiryID;
                var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName, ContactPerson = d.ContactName, Mobileno = d.ContactNo }).FirstOrDefault();

                if (_client != null)
                {
                    vm.ClientDetail = _client.ClientName;
                    vm.ClientID = _client.ClientID;
                    vm.ContactPerson = _client.ContactPerson;
                    vm.MobileNumber = _client.Mobileno;
                }
                var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0, Convert.ToInt32(vm.EngineerID));
                vm.QuotationNo = dta1.QuotationNo;
                vm.Version = dta1.Version;
                var defaultQuotationStatus = db.QuotationStatus.Where(cc => cc.DefaultStatus == true).FirstOrDefault();
                int defaultquotationstatusid = 0;
                if (defaultQuotationStatus != null)
                {
                    defaultquotationstatusid = defaultQuotationStatus.ID;
                }
                else
                {
                    defaultquotationstatusid = 1;
                }
                vm.QuotationStatusID = defaultquotationstatusid;
                vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
                if (vm.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                vm.CurrencyId = CommonFunctions.GetDefaultCurrencyId();
                vm.EnquiryID = EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.VATPercent = 5;
                vm.QuotationDetails = new List<QuotationDetailVM>();

                ViewBag.QuotationMode = "New";
                ViewBag.Title = "Create";

                var _detail1 = new List<QuotationScopeofWorkVM>();
                var _detail2 = new List<QuotationWarrantyVM>();
                var _detail3 = new List<QuotationExclusionVM>();
                var _detail5 = new List<QuotationContactVM>();
                var _detail4 = EnquiryDAO.QuotationTerms(0);
                if (_detail4 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.TermsandConditions = "";
                vm.QuotationContacts = _detail5;
            }
            else if (id > 0) //edit mode
            {

                Quotation item = db.Quotations.Find(id);
                vm.QuotationID = item.QuotationID;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.ProjectName = GetEnquiryProjectName(vm.EnquiryID);
                vm.QuotationNo = item.QuotationNo;
                vm.QuotationDate = item.QuotationDate;
                vm.MobileNumber = item.MobileNumber;
                vm.ContactPerson = item.ContactPerson;
                vm.ClientID = item.ClientID;
                vm.ClientDetail = item.ClientDetail;
                vm.Version = item.Version;
                vm.CurrencyId = item.CurrencyId;
                vm.Salutation = item.Salutation;
                vm.TermsandConditions = item.TermsandConditions;
                vm.PaymentTerms = item.PaymentTerms;
                vm.SubjectText = item.SubjectText;
                vm.QuotationValue = item.QuotationValue;
                vm.DiscountAmount = item.DiscountAmount;
                vm.DiscountPercent = item.DiscountPercent;
                vm.VATPercent = item.VATPercent;
                vm.VATAmount = item.VATAmount;
                if (vm.VATAmount is null)
                    vm.VATAmount = 0;
                if (vm.DiscountAmount is null)
                    vm.DiscountAmount = 0;
                vm.GrossAmount = Convert.ToDecimal(item.QuotationValue) - (Convert.ToDecimal(vm.VATAmount) + Convert.ToDecimal(vm.DiscountAmount));
                vm.EngineerID = item.EngineerID;
                vm.ProjectRef = item.ProjectRef;
                vm.QuotationTo = item.QuotationTo;
                vm.QuotationStatusID = item.QuotationStatusID;

                vm.DeliveryTerms = item.DeliveryTerms;
                vm.Validity = item.Validity;
                if (item.QuotationStatusID > 0)
                {
                    var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                    vm.QuotationStatus = _status;
                }
                vm.QuotationDetails = EnquiryDAO.QuotationEquipment(item.EnquiryID, item.QuotationID);

                var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
                //var _detail1 = EnquiryDAO.GetQuotationScopeGroups(id);
                var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
                var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);
                var _detail5 = EnquiryDAO.GetQuotationContacts(id);
                var _detail4 = EnquiryDAO.QuotationTerms(id);

                if (_detail1 == null)
                {
                    _detail1 = new List<QuotationScopeofWorkVM>();
                }
                if (_detail2 == null)
                {
                    _detail2 = new List<QuotationWarrantyVM>();
                }
                if (_detail3 == null)
                {
                    _detail3 = new List<QuotationExclusionVM>();
                }
                if (_detail1 == null)
                {
                    _detail4 = new List<QuotationTermsVM>();
                }
                vm.QuotationScopeDetails = _detail1;
                vm.QuotationWarrantyDetails = _detail2;
                vm.QuotationExclusionDetails = _detail3;
                vm.QuotationTerms = _detail4;
                vm.QuotationContacts = _detail5;
                ViewBag.Title = "Modify";
            }

            return View(vm);
        }
        //public JsonResult ShowQuotationEntry(int Id = 0, int EnquiryID = 0, int QuotationID = 0,int EmployeeID=0)
        //{
        //    int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
        //    int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
        //    ViewBag.Curency = db.CurrencyMasters.ToList();


        //    //ViewBag.Unit = db.ItemUnits.ToList();
        //    QuotationVM vm = new QuotationVM();
        //    if (Id == 0)
        //    {
        //        var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchid, fyearid, EnquiryID, 0,EmployeeID);
        //        vm.QuotationNo = dta1.QuotationNo;
        //        vm.Version = dta1.Version;
        //        vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
        //        vm.EnquiryID = EnquiryID;
        //        var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName }).FirstOrDefault();

        //        if (_client != null)
        //        {
        //            vm.ClientDetail = _client.ClientName;
        //            vm.ClientID = _client.ClientID;
        //        }
        //        var defaultcurrencyid = CommonFunctions.GetDefaultCurrencyId();
        //        vm.CurrencyId = defaultcurrencyid;
        //        //var job = db.JobGenerations.Find(vm.JobID);
        //        //if (job != null)
        //        //    vm.ClientDetail = job.Consignor + "\r" + job.OriginAddress;

        //        //if (job.StatusTypeId == null)
        //        //{
        //        //    vm.StatusTypeId = 1;
        //        //}
        //        //else
        //        //{
        //        //    vm.StatusTypeId = Convert.ToInt32(job.StatusTypeId);
        //        //}
        //    }
        //    else
        //    {
        //        Quotation item = db.Quotations.Find(Id);
        //        vm.QuotationID = item.QuotationID;
        //        vm.QuotationNo = item.QuotationNo;
        //        vm.QuotationDate = item.QuotationDate;
        //        vm.MobileNumber = item.MobileNumber;
        //        vm.ContactPerson = item.ContactPerson;
        //        vm.ClientID = item.ClientID;
        //        vm.ClientDetail = item.ClientDetail;
        //        vm.Version = item.Version;
        //        vm.CurrencyId = item.CurrencyId;
        //        vm.Salutation = item.Salutation;
        //        vm.TermsandConditions = item.TermsandConditions;
        //        vm.PaymentTerms = item.PaymentTerms;
        //        vm.SubjectText = item.SubjectText;
        //        vm.QuotationValue = item.QuotationValue;
        //        vm.EnquiryID = item.EnquiryID;


        //        vm.EngineerID = item.EngineerID;
        //        //vm.ClientDetail = item.ClientDetail;
        //        var job = db.Enquiries.Find(vm.EnquiryID);

        //        if (QuotationID > 0)
        //        { //for copy quotation{
        //            var dta1 = JobDAO.GetMaxJobQuotationNo(branchid, fyearid, EnquiryID, QuotationID);
        //            vm.QuotationNo = dta1.Split('-')[0];
        //            vm.Version = Convert.ToInt32(dta1.Split('-')[1]);
        //        }

        //    }
        //    return Json(new { QuotationId = Id, data = vm, message = "Data Found Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);

        //}

        public JsonResult AddEstimationItem(string Details)
        {
            int EstimationID = 0;
            EstimationVM vm = new EstimationVM();
            var IDetails = JsonConvert.DeserializeObject<List<EstimationDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EstimationDetailVM> list = new List<EstimationDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];


            foreach (var item in IDetails)
            {
                if (item.Checked == true)
                {
                    EstimationID = Convert.ToInt32(item.EstimationID);
                    if (list == null)
                    {
                        list = new List<EstimationDetailVM>();
                    }


                    list.Add(item);
                }
            }


            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }

            vm.Details = list;

            vm.Details = vm.Details.OrderBy(cc => cc.Roworder).ToList();
            return Json(new { EstimationID = EstimationID, data = list, status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddItem1(string Details, string Details1, int EstimationID)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationDetailVM>>(Details);
            var IDetails1 = JsonConvert.DeserializeObject<List<QuotationDetailVM>>(Details1);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<QuotationDetailVM> list = new List<QuotationDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationDetailVM item = new QuotationDetailVM();

            if (IDetails1.Count > 0 && Details != "[{}]")
                list = IDetails1;
            else
                list = new List<QuotationDetailVM>();

            list = list.Where(cc => cc.EstimationID != EstimationID).ToList();
            foreach (var item1 in IDetails)
            {

                item = new QuotationDetailVM();
                item.EstimationCategoryID = item1.EstimationCategoryID;
                item.EstimationMasterID = item1.EstimationMasterID;
                item.EstimationDetailID = item1.EstimationDetailID;
                item.EstimationID = item1.EstimationID;
                item.EstimationNo = item1.EstimationNo;
                item.CategoryName = item1.CategoryName;
                item.EquipmentID = item1.EquipmentID;
                item.Description = item1.Description;
                item.Model = item1.Model;
                item.UnitName = item1.UnitName;
                item.UnitID = item1.UnitID;
                item.Quantity = item1.Quantity;
                item.Amount = item1.Amount;

                item.UnitRate = item1.UnitRate;
                item.Deleted = item1.Deleted;

                if (list == null)
                {
                    list = new List<QuotationDetailVM>();
                }
                if (item.Deleted == false)
                    list.Add(item);
            }
            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }


            vm.QuotationDetails = list;
            return PartialView("QuotationDetailList", vm);
        }
        public ActionResult AddItem(QuotationDetailVM invoice, string Details, int EstimationID)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<QuotationDetailVM> list = new List<QuotationDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationDetailVM item = new QuotationDetailVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationDetailVM>();
            if (invoice != null)
            {
                item = new QuotationDetailVM();
                item.EstimationCategoryID = invoice.EstimationCategoryID;
                item.EstimationMasterID = invoice.EstimationMasterID;
                //item.Roworder = Convert.ToDecimal(item.EstimationCategoryID);
                if (item.EstimationCategoryID == 4)
                {
                    item.displayclass = "clsfinance";
                }
                item.CategoryName = invoice.CategoryName;
                item.EquipmentID = invoice.EquipmentID;
                item.Description = invoice.Description;
                item.Model = invoice.Model;
                item.UnitName = invoice.UnitName;
                item.UnitID = invoice.UnitID;
                item.Quantity = invoice.Quantity;
                item.Amount = invoice.Amount;

                item.UnitRate = invoice.UnitRate;
                item.Deleted = invoice.Deleted;
                //item.RowType = invoice.RowType;
                if (list == null)
                {
                    list = new List<QuotationDetailVM>();
                }
                if (item.Deleted == false)
                    list.Add(item);
            }
            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }


            vm.QuotationDetails = list;
            return PartialView("QuotationDetailList", vm);
        }
        public ActionResult ShowQuotationDetailList(int EnquiryID, int QuotationId)
        {
            ViewBag.Unit = db.ItemUnits.ToList();
            QuotationVM vm = new QuotationVM();

            vm.QuotationDetails = EnquiryDAO.QuotationEquipment(EnquiryID, QuotationId);

            return PartialView("QuotationDetailList", vm);
        }

        public ActionResult AddQuotationInventory(QuotationDetailVM invoice, string Details)
        {

            var IDetails = JsonConvert.DeserializeObject<List<QuotationDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<QuotationDetailVM> list = new List<QuotationDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationDetailVM item = new QuotationDetailVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationDetailVM>();

            item = new QuotationDetailVM();


            item.ProductFamilyID = invoice.ProductFamilyID;
            item.EquipmentTypeID = invoice.EquipmentTypeID;
            //item.EquipmentName = invoice.EquipmentName;
            item.Brand = invoice.Brand;
            item.Model = invoice.Model;

            item.Quantity = invoice.Quantity;
            item.Amount = invoice.Amount;
            item.UnitRate = invoice.UnitRate;            
            //item.Remarks = invoice.Remarks;


            if (list == null)
            {
                list = new List<QuotationDetailVM>();
            }

            list.Add(item);

            Session["JQuotationDetail"] = list;
            QuotationVM vm = new QuotationVM();
            vm.QuotationDetails = list;
            return PartialView("QuotationDetailList", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteQuotation(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int JobID = 0;
            Quotation obj = db.Quotations.Find(id);
            List<QuotationVM> list = new List<QuotationVM>();

            try
            {
                if (obj != null)
                {
                    if (obj.ClientPOID > 0)
                    {
                        return Json(new { status = "Failed", message = "Quotation could not be deleted as the Client PO has been raised.!" }, JsonRequestBehavior.AllowGet);
                    }
                    obj.IsDeleted = true;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Quotation Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed", data = list }, JsonRequestBehavior.AllowGet);
            }

        }
        public List<QuotationVM> BindQuotations(int EnquiryID)
        {
            List<QuotationVM> List = new List<QuotationVM>();

            List = (from c in db.Quotations join d in db.CurrencyMasters on c.CurrencyId equals d.CurrencyID where c.EnquiryID == EnquiryID select new QuotationVM { EnquiryID = c.EnquiryID, QuotationID = c.QuotationID, QuotationNo = c.QuotationNo, QuotationDate = c.QuotationDate, Version = c.Version, QuotationValue = c.QuotationValue, CurrencyName = d.CurrencyName }).ToList();

            return List;

        }

        public JsonResult GetQuotationNo(string term, int EnquiryID, int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            List<QuotationVM> objQuotation = new List<QuotationVM>();
            var _list = (from c in db.Quotations where c.EnquiryID == EnquiryID && c.EngineerID == EmployeeID select new QuotationVM { EnquiryID = c.EnquiryID, QuotationID = c.QuotationID, QuotationNo = c.QuotationNo, QuotationDate = c.QuotationDate, Version = c.Version }).ToList();

            //var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0,EmployeeID);
            //_list.Add(new QuotationVM { EnquiryID = EnquiryID,QuotationID=0,QuotationNo= dta1.QuotationNo  +"(New)",Version=dta1.Version });

            return Json(_list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetClientName(string term, int EnquiryID)
        {
            var _clientlist = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID select new { ClientID = c.ClientID, ClientName = d.ClientName, ContactPerson = d.ContactName, Mobileno = d.ContactNo }).ToList();
            return Json(_clientlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNewQuotationNo(int EnquiryID, int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0, EmployeeID);
            var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName, ContactName = d.ContactName, MobileNo = d.ContactNo }).FirstOrDefault();
            var _ClientDetail = "";
            var _ClientID = 0;
            var _ClientContactName = "";
            var _ClientContactNo = "";

            if (_client != null)
            {
                _ClientDetail = _client.ClientName;
                _ClientID = Convert.ToInt32(_client.ClientID);
            }
            return Json(new QuotationVM { EnquiryID = EnquiryID, QuotationID = 0, QuotationNo = dta1.QuotationNo, Version = dta1.Version, ClientID = _ClientID, ClientDetail = _ClientDetail, ContactPerson = _ClientContactName, MobileNumber = _ClientContactNo }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetDueDays(DateTime QuotationDate, DateTime DueDate)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            return Json("OK", JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReportFrame()
        {
            if (Session["ReportOutput"] != null)
                ViewBag.ReportOutput = Session["ReportOutput"].ToString();
            else
            {
                string reportpath = ReportsDAO.GenerateDefaultReport();
                ViewBag.ReportOutput = reportpath; // "~/Reports/DefaultReport.pdf";
                //ViewBag.ReportOutput = "~/Reports/DefaultReport.pdf";
            }
            return PartialView();
        }
        public ActionResult ReportPrint(int Id)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Quotation Printing";
            ViewBag.Client = EnquiryDAO.GetQuotationClient(Id);
            ReportsDAO.QuotationReport(Id, 0);
            QuotationVM vm = new QuotationVM();
            vm.QuotationID = Id;
            vm.ClientID = 0;
            return View(vm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportPrint(QuotationVM model)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Quotation Printing";
            ViewBag.Client = EnquiryDAO.GetQuotationClient(model.QuotationID);
            int clientid = 0;
            if (model.ClientID == null)
            {
                clientid = 0;
            }
            else
            {
                clientid = Convert.ToInt32(model.ClientID);
            }
            ReportsDAO.QuotationReport(model.QuotationID, clientid);

            return View(model);

        }


        [HttpGet]
        public JsonResult GetEstimation(string term, int EnquiryID)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            var list = (from c in db.Estimations where c.IsDeleted == false && c.EnquiryID == EnquiryID select new DropdownVM { ID = c.EstimationID, Text = c.EstimationNo }).ToList();
            if (term == null)
            { term = ""; }
            if (term.Trim() != "")
            {
                list.Where(cc => cc.Text.Contains(term.Trim())).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var list1 = (from c in db.Estimations where c.IsDeleted == false && c.EnquiryID == EnquiryID select new DropdownVM { ID = c.EstimationID, Text = c.EstimationNo }).ToList();


                return Json(list1, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public JsonResult GetEstimationTypeByText(string TypeName)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var _Estimationmaster = new EstimationMaster();
            _Estimationmaster = (from c in db.EstimationMasters where c.TypeName == TypeName select c).FirstOrDefault();

            return Json(_Estimationmaster, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetEstimationDetail(int EstimationID, int EnquiryID)
        {
            EstimationVM vm = new EstimationVM();
            vm.Details = EnquiryDAO.EstimationEquipmentSellingRate(EnquiryID, EstimationID);
            vm.Details = vm.Details.OrderBy(cc => cc.Roworder).ToList();
            Session["EstimationDetail"] = vm.Details.OrderBy(cc => cc.Roworder).ToList();
            return PartialView("EstimationDetailList", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetContactDetail(int QuotationID)
        {
            QuotationVM vm = new QuotationVM();
            vm.QuotationContacts = EnquiryDAO.GetQuotationContacts(QuotationID);


            return PartialView("ContactList", vm);
        }


        #region quotationdetails
        public ActionResult Details(int id = 0)
        {
            ViewBag.Brand = db.Brands.OrderBy(cc => cc.BrandName).ToList();
            ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductFamilyName).ToList();

            Models.QuotationVM _quotationvm = new Models.QuotationVM();
            if (id > 0)
            {


                Quotation item = db.Quotations.Find(id);
                _quotationvm.QuotationID = item.QuotationID;
                //quotationvm.e = item.EnquiryID;
                _quotationvm.QuotationNo = item.QuotationNo;
                _quotationvm.QuotationDate = item.QuotationDate;
                _quotationvm.EnquiryID = item.EnquiryID;

                var _enquiry = db.Enquiries.Where(cc => cc.EnquiryID == _quotationvm.EnquiryID).FirstOrDefault();
                if (_enquiry != null)
                {
                    _quotationvm.EnquiryNo = _enquiry.EnquiryNo;
                }

                var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == item.EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName, ContactPerson = d.ContactName, Mobileno = d.ContactNo }).FirstOrDefault();

                if (_client != null)
                {
                    _quotationvm.ClientDetail = _client.ClientName;
                }
                var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
                var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
                var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);

                if (_detail1 == null)
                {
                    _detail1 = new List<QuotationScopeofWorkVM>();
                }
                if (_detail2 == null)
                {
                    _detail2 = new List<QuotationWarrantyVM>();
                }
                if (_detail3 == null)
                {
                    _detail3 = new List<QuotationExclusionVM>();
                }
                _quotationvm.QuotationScopeDetails = _detail1;
                _quotationvm.QuotationWarrantyDetails = _detail2;
                _quotationvm.QuotationExclusionDetails = _detail3;

            }
            return View(_quotationvm);
        }

        public ActionResult AddScopeItem(QuotationScopeofWorkVM invoice, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationScopeofWorkVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationScopeofWorkVM> list = new List<QuotationScopeofWorkVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationScopeofWorkVM item = new QuotationScopeofWorkVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationScopeofWorkVM>();
            if (invoice != null)
            {
                item = new QuotationScopeofWorkVM();

                item.Description = invoice.Description;
                item.QuotationID = invoice.QuotationID;
                item.Model = invoice.Model;
                item.EquipmentID = invoice.EquipmentID;
                item.EquipmentName = invoice.EquipmentName;
                if (list == null)
                {
                    list = new List<QuotationScopeofWorkVM>();
                }

                list.Add(item);
            }



            vm.QuotationScopeDetails = list;
            return PartialView("ScopeDetailList", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteScopeEntry(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int JobID = 0;
            QuotationScopeofwork obj = db.QuotationScopeofworks.Find(id);

            try
            {
                if (obj != null)
                {
                    db.QuotationScopeofworks.Remove(obj);
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Quotation Scope work Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteWarrantyEntry(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int JobID = 0;
            QuotationWarranty obj = db.QuotationWarranties.Find(id);

            try
            {
                if (obj != null)
                {
                    db.QuotationWarranties.Remove(obj);
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Quotation Scope work Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteExclusionEntry(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int JobID = 0;
            QuotationExclusion obj = db.QuotationExclusions.Find(id);

            try
            {
                if (obj != null)
                {
                    db.QuotationExclusions.Remove(obj);
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Quotation Exlcusion Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult AddMasterScopeofWork(int QuotationId, int EquipmentId, string EquipmentName, string Model, string Details)
        {
            var _equipment = db.Equipments.Where(cc => cc.ID == EquipmentId).FirstOrDefault();
            var _equipmenttypeid = _equipment.EquipmentTypeID;

            var _masterdetail1 = (from c in db.EquipmentScopeofworks where c.EquipmentTypeID == _equipmenttypeid && c.Model == Model select new EquipmentScopeofworkVM { ID = c.ID, Model = c.Model, Description = c.Description, Deleted = false }).ToList();

            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationScopeofWorkVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationScopeofWorkVM> list = new List<QuotationScopeofWorkVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationScopeofWorkVM item = new QuotationScopeofWorkVM();

            if (IDetails.Count > 0 && Details != "[{}]")
            {
                list = IDetails;
                list = list.Where(cc => cc.EquipmentID != EquipmentId).ToList();
            }
            else
                list = new List<QuotationScopeofWorkVM>();
            if (_masterdetail1 != null)
            {
                foreach (var masteritem in _masterdetail1)
                {
                    item = new QuotationScopeofWorkVM();
                    item.Model = masteritem.Model;
                    item.QuotationID = QuotationId;
                    item.EquipmentID = EquipmentId;
                    item.EquipmentName = EquipmentName;
                    item.Description = masteritem.Description;
                    item.Checked = true;
                    if (list == null)
                    {
                        list = new List<QuotationScopeofWorkVM>();
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
            }


            vm.QuotationScopeDetails = list;
            return PartialView("ScopeDetailList", vm);

        }


        public ActionResult AddWarrantyItem(QuotationWarrantyVM invoice, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationWarrantyVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationWarrantyVM> list = new List<QuotationWarrantyVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationWarrantyVM item = new QuotationWarrantyVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationWarrantyVM>();
            if (invoice != null)
            {
                item = new QuotationWarrantyVM();

                item.Description = invoice.Description;
                item.QuotationID = invoice.QuotationID;
                item.EquipmentName = invoice.EquipmentName;
                item.WarrantyType = invoice.WarrantyType;
                item.EquipmentID = invoice.EquipmentID;
                item.Checked = invoice.Checked;

                if (list == null)
                {
                    list = new List<QuotationWarrantyVM>();
                }

                list.Add(item);
            }



            vm.QuotationWarrantyDetails = list;
            return PartialView("WarrantyDetailList", vm);
        }

        public ActionResult AddMasterWarrantyItem(int QuotationID, int EquipmentId, string Type, string Details)
        {

            var _equipment = db.Equipments.Where(cc => cc.ID == EquipmentId).FirstOrDefault();
            var _equipmenttypeid = _equipment.EquipmentTypeID;

            var _masterdetail1 = (from c in db.EquipmentWarranties where c.EquipmentTypeID == _equipmenttypeid && c.WarrantyType == Type select new QuotationWarrantyVM { ID = c.ID, WarrantyType = c.WarrantyType, Description = c.Description, Checked = true }).ToList();

            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationWarrantyVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationWarrantyVM> list = new List<QuotationWarrantyVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationWarrantyVM item = new QuotationWarrantyVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationWarrantyVM>();
            if (_masterdetail1 != null)
            {
                foreach (var masteritem in _masterdetail1)
                {
                    item = new QuotationWarrantyVM();

                    item.Description = masteritem.Description;
                    item.WarrantyType = masteritem.WarrantyType;
                    item.QuotationID = QuotationID;
                    item.EquipmentID = EquipmentId;
                    item.Checked = true;

                    if (list == null)
                    {
                        list = new List<QuotationWarrantyVM>();
                    }

                    list.Add(item);
                }
            }



            vm.QuotationWarrantyDetails = list;
            return PartialView("WarrantyDetailList", vm);
        }


        public ActionResult AddExclusionItem(QuotationExclusionVM invoice, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationExclusionVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationExclusionVM> list = new List<QuotationExclusionVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            QuotationExclusionVM item = new QuotationExclusionVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<QuotationExclusionVM>();
            if (invoice != null)
            {
                item = new QuotationExclusionVM();

                item.Description = invoice.Description;
                item.EquipmentName = invoice.EquipmentName;
                item.QuotationID = invoice.QuotationID;
                item.EquipmentID = invoice.EquipmentID;
                item.Checked = invoice.Checked;
                if (list == null)
                {
                    list = new List<QuotationExclusionVM>();
                }

                list.Add(item);
            }




            vm.QuotationExclusionDetails = list;
            return PartialView("ExclusionDetailList", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveScopeItem1(QuotationScopeofWorkVM request)
        {

            int QuotationID = Convert.ToInt32(request.QuotationID);
            string description = request.Description;
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("br");

            description = sanitizer.Sanitize(description ?? string.Empty);
            if (request.ID > 0)
            {
                QuotationScopeofwork _obj = db.QuotationScopeofworks.Find(request.ID);
                _obj.Description = request.Description;
                _obj.Model = request.Model;
                _obj.Airmech = request.Airmech;
                _obj.client = request.client;
                _obj.OrderNo = request.OrderNo;
                _obj.EquipmentID = request.EquipmentID;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                QuotationScopeofwork _obj = new QuotationScopeofwork();
                _obj.Description = request.Description;
                _obj.Model = request.Model;
                _obj.EquipmentID = request.EquipmentID;
                _obj.QuotationID = request.QuotationID;
                _obj.OrderNo = request.OrderNo;
                _obj.Airmech = request.Airmech;
                _obj.client = request.client;
                db.QuotationScopeofworks.Add(_obj);
                db.SaveChanges();
            }
            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Quotation Scope of Work Updated Successfully!" });

        }

        public JsonResult SaveWarrantyItem1(QuotationWarrantyVM request)
        {

            int QuotationID = Convert.ToInt32(request.QuotationID);
            string description = request.Description;
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("br");

            description = sanitizer.Sanitize(description ?? string.Empty);
            if (request.ID > 0)
            {
                QuotationWarranty _obj = db.QuotationWarranties.Find(request.ID);
                _obj.Description = request.Description;
                _obj.WarrantyType = request.WarrantyType;
                //_obj.OrderNo = request.OrderNo;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                QuotationWarranty _obj = new QuotationWarranty();
                _obj.Description = request.Description;
                _obj.WarrantyType = request.WarrantyType;
                //  _obj.Model = request.Model;
                _obj.EquipmentID = request.EquipmentID;
                _obj.QuotationID = request.QuotationID;
                //_obj.OrderNo = request.OrderNo;
                db.QuotationWarranties.Add(_obj);
                db.SaveChanges();
            }

            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Quotation Warranty Data Updated Successfully!" });

        }

        public JsonResult SaveExclusionItem1(QuotationExclusionVM request)
        {

            int QuotationID = Convert.ToInt32(request.QuotationID);
            string description = request.Description;
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("br");

            description = sanitizer.Sanitize(description ?? string.Empty);
            if (request.ID > 0)
            {
                QuotationExclusion _obj = db.QuotationExclusions.Find(request.ID);
                _obj.Description = request.Description;
                //_obj.WarrantyType = request.WarrantyType;
                //_obj.OrderNo = request.OrderNo;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                QuotationExclusion _obj = new QuotationExclusion();
                _obj.Description = request.Description;
                //_obj.WarrantyType = request.WarrantyType;
                //  _obj.Model = request.Model;
                _obj.EquipmentID = request.EquipmentID;
                _obj.QuotationID = request.QuotationID;
                //_obj.OrderNo = request.OrderNo;
                db.QuotationExclusions.Add(_obj);
                db.SaveChanges();
            }

            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Quotation Exclusion Data Updated Successfully!" });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // allows HTML in Details string
        public JsonResult SaveScopeItem(int QuotationID, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationScopeofWorkVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationScopeofWorkVM> list = new List<QuotationScopeofWorkVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<QuotationScopeofWorkVM>();
                var _details = db.QuotationScopeofworks.Where(cc => cc.QuotationID == QuotationID).ToList();

                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Checked == true)
                    {
                        QuotationScopeofwork _obj = db.QuotationScopeofworks.Find(item.ID);
                        _obj.Description = item.Description;
                        _obj.Model = item.Model;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Checked == false)
                    {
                        QuotationScopeofwork _obj = db.QuotationScopeofworks.Find(item.ID);
                        db.QuotationScopeofworks.Remove(_obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        QuotationScopeofwork _obj = new QuotationScopeofwork();
                        _obj.QuotationID = QuotationID;
                        _obj.Model = item.Model;
                        _obj.EquipmentID = item.EquipmentID;
                        _obj.Description = item.Description;
                        db.QuotationScopeofworks.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Quotation Scope of Work Updated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }


        public JsonResult SaveWarrantyItem(int QuotationID, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationWarrantyVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationWarrantyVM> list = new List<QuotationWarrantyVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<QuotationWarrantyVM>();


                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Checked == true)
                    {
                        QuotationWarranty _obj = db.QuotationWarranties.Find(item.ID);
                        _obj.Description = item.Description;
                        _obj.WarrantyType = item.WarrantyType;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Checked == false)
                    {
                        QuotationWarranty _obj = db.QuotationWarranties.Find(item.ID);
                        db.QuotationWarranties.Remove(_obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        QuotationWarranty _obj = new QuotationWarranty();
                        _obj.QuotationID = QuotationID;
                        _obj.WarrantyType = item.WarrantyType;
                        _obj.EquipmentID = item.EquipmentID;
                        _obj.Description = item.Description;
                        db.QuotationWarranties.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Quotation Warranty Updated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }

        public JsonResult SaveExclusion(int QuotationID, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationExclusionVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationExclusionVM> list = new List<QuotationExclusionVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<QuotationExclusionVM>();


                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Checked == true)
                    {
                        QuotationExclusion _obj = db.QuotationExclusions.Find(item.ID);
                        _obj.Description = item.Description;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Checked == false)
                    {
                        QuotationExclusion _obj = db.QuotationExclusions.Find(item.ID);
                        db.QuotationExclusions.Remove(_obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        QuotationExclusion _obj = new QuotationExclusion();
                        _obj.QuotationID = QuotationID;
                        _obj.EquipmentID = item.EquipmentID;
                        _obj.Description = item.Description;
                        db.QuotationExclusions.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Quotation Exclusions Saved Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }
        public JsonResult SaveContacts(int QuotationID, string Details)
        {
            QuotationVM vm = new QuotationVM();
            var IDetails = JsonConvert.DeserializeObject<List<QuotationContactVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<QuotationContactVM> list = new List<QuotationContactVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<QuotationContactVM>();


                foreach (var item in list)
                {
                    if (item.ID > 0)
                    {
                        QuotationContact _obj = db.QuotationContacts.Find(item.ID);
                        _obj.ContactName = item.ContactName;
                        _obj.EmailID = item.EmailID;
                        _obj.PhoneNo = item.PhoneNo;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }
                return Json(new { status = "OK", message = "Quotation Contact Saved Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }
        public ActionResult ShowScopeofWork(int QuotationID)
        {
            var _detail1 = EnquiryDAO.QuotationScopeofWork(QuotationID, 0);

            QuotationVM vm = new QuotationVM();
            vm.QuotationScopeDetails = _detail1;
            return PartialView("ScopeDetailList", vm);
        }

        public ActionResult ShowAddScopeofWork(int QuotationID, int EnquiryID, int ID)
        {

            var _equipments = GetEquipment(1, EnquiryID);
            ViewBag.Equipments = _equipments;

            QuotationScopeofWorkVM vm = new QuotationScopeofWorkVM();

            if (ID > 0)
            {
                var _detail1 = db.QuotationScopeofworks.Find(ID);
                vm.ID = _detail1.ID;
                vm.QuotationID = Convert.ToInt32(_detail1.QuotationID);

                vm.EquipmentID = Convert.ToInt32(_detail1.EquipmentID);
                vm.Model = _detail1.Model;
                vm.OrderNo = _detail1.OrderNo ?? 0;
                vm.Description = _detail1.Description;
            }
            return PartialView("AddScopeDetail", vm);
        }
        public ActionResult ShowAddWarranty(int QuotationID, int EnquiryID, int ID)
        {

            var _equipments = GetEquipment(1, EnquiryID);
            ViewBag.Equipments = _equipments;

            QuotationWarrantyVM vm = new QuotationWarrantyVM();

            if (ID > 0)
            {
                var _detail1 = db.QuotationWarranties.Find(ID);
                vm.ID = _detail1.ID;
                vm.QuotationID = Convert.ToInt32(_detail1.QuotationID);

                vm.EquipmentID = Convert.ToInt32(_detail1.EquipmentID);
                //vm.Model = _detail1.Model;
                //vm.OrderNo = _detail1.OrderNo ?? 0;
                vm.Description = _detail1.Description;
            }
            return PartialView("AddWarrantyDetail", vm);
        }
        public ActionResult ShowAddExclusion(int QuotationID, int EnquiryID, int ID)
        {

            var _equipments = GetEquipment(1, EnquiryID);
            ViewBag.Equipments = _equipments;

            QuotationExclusionVM vm = new QuotationExclusionVM();

            if (ID > 0)
            {
                var _detail1 = db.QuotationExclusions.Find(ID);
                vm.ID = _detail1.ID;
                vm.QuotationID = Convert.ToInt32(_detail1.QuotationID);
                vm.EquipmentID = Convert.ToInt32(_detail1.EquipmentID);

                //vm.Model = _detail1.Model;
                //vm.OrderNo = _detail1.OrderNo ?? 0;

                vm.Description = _detail1.Description;
            }

            return PartialView("AddExclusionDetail", vm);

        }
        public List<DropdownVM> GetEquipment(int CategoryID, int EnquiryID)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (CategoryID == 1)
            {
                var list = (from c in db.Equipments where c.EnquiryID == EnquiryID select new DropdownVM { ID = c.ID, Text = c.EquipmentName }).ToList();


                return list;

            }
            else
            {
                var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();


                return list1;

            }
        }

        public ActionResult ShowWarranty(int QuotationID)
        {
            var _detail1 = EnquiryDAO.QuotationWarranty(QuotationID, 0);

            QuotationVM vm = new QuotationVM();
            vm.QuotationWarrantyDetails = _detail1;
            return PartialView("WarrantyDetailList", vm);
        }

        public ActionResult ShowExclusion(int QuotationID)
        {
            var _detail1 = EnquiryDAO.QuotationExclusions(QuotationID, 0);
            QuotationVM vm = new QuotationVM();
            vm.QuotationExclusionDetails = _detail1;
            return PartialView("ExclusionDetailList", vm);
        }

        [HttpGet]
        public JsonResult GetWarrantyItem(string term, int EquipmentId, string Type)
        {

            var _equipment = db.Equipments.Where(cc => cc.ID == EquipmentId).FirstOrDefault();
            var _equipmenttypeid = _equipment.EquipmentTypeID;

            var _masterdetail1 = (from c in db.EquipmentWarranties where c.EquipmentTypeID == _equipmenttypeid && c.WarrantyType == Type select new QuotationWarrantyVM { ID = c.ID, WarrantyType = c.WarrantyType, Description = c.Description, Checked = true }).ToList();

            if (term.Trim() == "")
            {
                return Json(_masterdetail1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _list = _masterdetail1.Where(cc => cc.Description.Contains(term)).ToList();

                return Json(_list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetExclusionItem(string term, int EquipmentId)
        {

            var _equipment = db.Equipments.Where(cc => cc.ID == EquipmentId).FirstOrDefault();
            var _equipmenttypeid = _equipment.EquipmentTypeID;

            var _masterdetail1 = (from c in db.EquipmentExclusions where c.EquipmentTypeID == _equipmenttypeid select new QuotationWarrantyVM { ID = c.ID, Description = c.Description, Checked = true }).ToList();

            if (term.Trim() == "")
            {
                return Json(_masterdetail1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _list = _masterdetail1.Where(cc => cc.Description.Contains(term)).ToList();

                return Json(_list, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetEnquiryNo(int EnquiryID)
        {
            if (EnquiryID > 0)
            {
                var _enquiry = db.Enquiries.Where(cc => cc.EnquiryID == EnquiryID).FirstOrDefault();
                return _enquiry.EnquiryNo;
            }
            else
            {
                return "";
            }

        }
        public string GetEnquiryProjectName(int EnquiryID)
        {
            if (EnquiryID > 0)
            {
                var _enquiry = db.Enquiries.Where(cc => cc.EnquiryID == EnquiryID).FirstOrDefault();
                return _enquiry.ProjectName;
            }
            else
            {
                return "";
            }

        }
        #endregion

        #region QuotationPrint
      
        [HttpGet]
        public ActionResult Print(int Id)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Quotation Printing";

            var Clients = EnquiryDAO.GetQuotationClient(Id);
            ViewBag.Client = Clients;
            //ReportsDAO.QuotationReport(Id, 0);
            QuotationVM vm = new QuotationVM();
            vm.ReportClass = "";
            vm = GetQuotationPrintData(Id, Clients[0].ClientID);
            vm.ReportClass = "";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Print(int Id, int ClientID)
        {

            ViewBag.ReportName = "Quotation Printing";
            ViewBag.Client = EnquiryDAO.GetQuotationClient(Id);
            //ReportsDAO.QuotationReport(Id, ClientID);
            QuotationVM vm = new QuotationVM();
            vm = GetQuotationPrintData(Id, ClientID);
            vm.ReportClass = "";
            return View(vm);
        }
        public ActionResult PrintPDF(int Id, int ClientID)
        {
            // Use the absolute server path to the header/footer files
            string headerUrl = Server.MapPath("~/Content/HeadersAndFooters/QuotationHeader.html");
            string footerUrl = Server.MapPath("~/Content/HeadersAndFooters/QuotationFooter.html");
            ViewBag.Client = EnquiryDAO.GetQuotationClient(Id);
            string customSwitches = $"--header-html {headerUrl} " +
                                  $"--header-spacing 3 " +
                                  $"--footer-html {footerUrl} " +
                                  $"--footer-spacing 0";
            //ReportsDAO.QuotationReport(Id, ClientID);
            QuotationVM vm = new QuotationVM();
            vm = GetQuotationPrintData(Id, ClientID);
            vm.ReportClass = "pdfenable";
            return new Rotativa.ViewAsPdf("Print", vm)
            {
                FileName = "Quotation_" + vm.QuotationNo + ".pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(50, 20, 50, 30),
                CustomSwitches = customSwitches

            };
        }
        public QuotationVM GetQuotationPrintData(int id, int ClientID)
        {
            QuotationVM vm = new QuotationVM();

            Quotation item = db.Quotations.Find(id);
            vm.QuotationID = item.QuotationID;
            vm.EnquiryID = item.EnquiryID;
            vm.QuotationNo = item.QuotationNo;
            vm.QuotationDate = item.QuotationDate;
            vm.MobileNumber = item.MobileNumber;
            vm.ContactPerson = item.ContactPerson;

            var Enquiry = db.Enquiries.Find(item.EnquiryID);
            vm.ProjectName = Enquiry.ProjectName;
            vm.EnquiryNo = Enquiry.EnquiryNo;
            var _client = db.ClientMasters.Find(ClientID);
            vm.ClientID = _client.ClientID;
            vm.ClientDetail = _client.ClientName;
            var quotecontact = db.QuotationContacts.Where(cc => cc.ClientID == ClientID).FirstOrDefault();
            if (quotecontact != null)
            {
                
                vm.ContactPerson = quotecontact.ContactName;
                vm.QuoteClientEmailId = quotecontact.EmailID;

                string clientaddress = "";
                var _clientcity = db.CityMasters.Find(_client.CityID).City;
                if (_clientcity == "Unknown")
                    _clientcity = "";


                var _clientcountry = db.CountryMasters.Find(_client.CountryID).CountryName;
                if (_clientcountry == "Unknown")
                    _clientcountry = "";

                clientaddress = _clientcity + "," + _clientcountry;
                vm.QuoteClientAddress = _client.Address1;
                vm.QuoteClientLocation = clientaddress; 
                
            }
            vm.Version = item.Version;
            vm.CurrencyId = item.CurrencyId;
            vm.Salutation = item.Salutation;
            vm.TermsandConditions = item.TermsandConditions;
            vm.PaymentTerms = item.PaymentTerms;
            vm.SubjectText = item.SubjectText;
            vm.Validity = item.Validity;
            vm.QuotationValue = item.QuotationValue;
            vm.DeliveryTerms = item.DeliveryTerms;
            vm.VATPercent = item.VATPercent;
            vm.VATAmount = item.VATAmount;
            if (vm.VATAmount is null)
                vm.VATAmount = 0;
            if (vm.DiscountAmount is null)
                vm.DiscountAmount = 0;
            vm.GrossAmount = Convert.ToDecimal(item.QuotationValue) - (Convert.ToDecimal(vm.VATAmount) + Convert.ToDecimal(vm.DiscountAmount));
            vm.QuotationValueInWords = OmaniCurrencyWords.ToOmaniCurrencyWords(Convert.ToDecimal(vm.QuotationValue));

            vm.EngineerID = item.EngineerID;
            var _employee = db.EmployeeMasters.Find(vm.EngineerID);

            vm.EmployeeName = _employee.FirstName + " " + _employee.LastName;
            vm.QuotationStatusID = item.QuotationStatusID;
            if (item.QuotationStatusID > 0)
            {
                var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                vm.QuotationStatus = _status;
            }

            vm.QuotationDetails = EnquiryDAO.QuotationEquipment(item.EnquiryID, item.QuotationID);

            //var _detail1 = EnquiryDAO.QuotationScopeofWork(id, 0);
            var _detail2 = EnquiryDAO.QuotationWarranty(id, 0);
            var _detail3 = EnquiryDAO.QuotationExclusions(id, 0);
            var _detail5 = EnquiryDAO.GetQuotationContacts(id);
            //var _detail4 = EnquiryDAO.QuotationTerms(id);

            //if (_detail1 == null)
            //{
            //    _detail1 = new List<QuotationScopeofWorkVM>();
            //}
            if (_detail2 == null)
            {
                _detail2 = new List<QuotationWarrantyVM>();
            }
            if (_detail3 == null)
            {
                _detail3 = new List<QuotationExclusionVM>();
            }
            //if (_detail4 == null)
            //{
            //    _detail4 = new List<QuotationTermsVM>();
            //}
            //vm.QuotationScopeDetails = _detail1;
            vm.QuotationScopeofWork = EnquiryDAO.GetQuotationScopeGroups(id);
            vm.QuotationWarrantyDetails = _detail2;
            vm.QuotationExclusionDetails = _detail3;
            //vm.QuotationTerms = _detail4;
            vm.QuotationContacts = _detail5;

            return vm;

        }

        #endregion
    }
}