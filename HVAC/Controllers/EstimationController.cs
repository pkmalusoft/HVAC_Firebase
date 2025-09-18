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
using System.Runtime.InteropServices.ComTypes;
using System.Data.Entity.ModelConfiguration.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Web.UI.WebControls;
namespace HVAC.Views
{

    [SessionExpireFilter]
    public class EstimationController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: Quotationroller
        public ActionResult Index()
        {

            EstimationSearch obj = (EstimationSearch)Session["EstimationSearch"];
            EstimationSearch model = new EstimationSearch();
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
                obj = new EstimationSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["EstimationSearch"] = obj;
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

            List<EstimationVM> lst = EnquiryDAO.EstimationList(model.FromDate, model.ToDate, model.EnquiryNo,model.EstimationNo, EmployeeId, branchid,yearid);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        public ActionResult Index(EstimationSearch obj)
        {
            Session["EstimationSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id=0,string  EnquiryNo = "",int EstimationId=0)
        {
            int EnquiryID = 0;

            if  (EnquiryNo!="")
            {
                var _EnquiryNo = db.Enquiries.Where(c => c.EnquiryNo == EnquiryNo).FirstOrDefault();
                if (_EnquiryNo!=null)
                {
                    EnquiryID = _EnquiryNo.EnquiryID;
                }
            }
            var _currency = db.CurrencyMasters.Where(cc => cc.CurrencyCode == "USD").FirstOrDefault();
            decimal _exchangerate = Convert.ToDecimal(_currency.ExchangeRate);
            EstimationVM vm = new EstimationVM();
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            var  useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();

            //ViewBag.Quotation=DBCo
            ViewBag.Enquiry = EnquiryDAO.GetEstimationEnquiry(useremployee.EmployeeID, branchId, fyearid);
             
           // ViewBag.Enquiry = db.Enquiries.ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.QuotationStatus = db.QuotationStatus.ToList();
            ViewBag.Unit = db.ItemUnits.ToList();
             
            List<EmployeeVM> _EmployeeMaster = EnquiryDAO.GetEmployeesList();
            ViewBag.EmployeeMaster = _EmployeeMaster;
            ViewBag.EstimationCategory = db.EstimationCategories.Where(cc=>cc.ID!=4).ToList();
            ViewBag.EstimationMaster = db.EstimationMasters.ToList();

            vm.Details = new List<EstimationDetailVM>();

            if (useremployee != null)
                vm.EmployeeID = useremployee.EmployeeID;

            if (EstimationId > 0) //new revision
            {
                Estimation item = db.Estimations.Find(EstimationId);
                vm.EstimationID = 0;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                vm.EstimationNo = item.EstimationNo;
                vm.EstimationDate = CommonFunctions.GetCurrentDateTime();
                vm.VarNo = db.Estimations.Where(cc => cc.EstimationNo == vm.EstimationNo).Select(cc => cc.VarNo).Max() + 1;
                vm.CurrencyID = item.CurrencyID;
                vm.ExchangeRate = item.ExchangeRate;
                vm.CurrencyCode=_currency.CurrencyCode + ":" + vm.ExchangeRate.ToString();
                vm.TotalFCValue = item.TotalFCValue;
                vm.TotalLCValue = item.TotalLCValue;
                vm.EmployeeID  = item.EmployeeID;
                vm.Notes = item.Notes;
                vm.EquipmentsTotal = item.EquipmentsTotal;
                vm.AccessoriesTotal = item.AccessoriesTotal;
                vm.FreightTotal = item.FreightTotal;                
                vm.LocalChargesTotal = item.LocalChargesTotal;
                vm.OtherChargesTotal = item.OtherChargesTotal;
                vm.PaymentDays = item.PaymentDays;
                vm.FreeServiceDays = item.FreeServiceDays;
                vm.FinCharge = item.FinCharge;
                vm.FinanceChargesTotal = item.FinanceChargesTotal;
                vm.FinChargePercent = item.FinChargePercent;
                vm.FinPerMonth = item.FinPerMonth;
                vm.ChargeableMonth = item.ChargeableMonth;
                vm.Margin = item.Margin;
                vm.MarginPercent = item.MarginPercent;
                vm.SellingValue = item.SellingValue;
                vm.SoharValue = item.SoharValue;
                vm.SoharValueOMR = item.SoharValueOMR;
                vm.LandingCostOMR = item.LandingCostOMR;
                vm.TotalLandingCostOMR = item.TotalLandingCostOMR;
                vm.EmployeeID = useremployee.EmployeeID;
                var list = (from c in db.Equipments where c.EnquiryID == item.EnquiryID select new DropdownVM { ID = c.ID, Text = c.EquipmentName }).ToList();
                ViewBag.Equipment = list;
                //vm.PaymentDays = 180;
                //vm.FreeServiceDays = 60;
                vm.Details = EnquiryDAO.EstimationEquipment(item.EnquiryID, item.EstimationID);
                vm = GetFinanceCharge1(vm.Details, vm, Convert.ToInt32(vm.PaymentDays));
                vm.Details = vm.Details.OrderBy(cc => cc.Roworder).ToList();
                ViewBag.QuotationMode = "Revision";
                ViewBag.Title = "Revision";
            }
            else if (id == 0 && EnquiryID==0) //quotation id is 0 create mode
            {
                vm.EstimationDate = CommonFunctions.GetCurrentDateTime();
                vm.CurrencyID = CommonFunctions.GetUSDCurrencyId();
                vm.ExchangeRate = CommonFunctions.GetUSDExRate(vm.CurrencyID);
                var dta1 = EnquiryDAO.GetMaxEstimationNo(branchId, fyearid, EnquiryID, 0, 0);
                vm.EstimationNo = dta1.QuotationNo;
                vm.VarNo = dta1.Version;
                vm.CurrencyCode = _currency.CurrencyCode + ":" + _exchangerate.ToString();
                ViewBag.Title = "Create";
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);

            }
            else if (id==0 && EnquiryID > 0)
            {
                vm.EnquiryID = EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(vm.EnquiryID);
                //var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName,ContactPerson=d.ContactName,Mobileno=d.ContactNo }).FirstOrDefault();
                var list = (from c in db.Equipments where c.EnquiryID == EnquiryID select new DropdownVM { ID = c.ID, Text = c.EquipmentName }).ToList();
                ViewBag.Equipment = list;
                var dta1 = EnquiryDAO.GetMaxEstimationNo(branchId, fyearid, EnquiryID, 0, 0);
                vm.EstimationNo = dta1.QuotationNo;
                vm.VarNo = dta1.Version;
                
                vm.EstimationDate = CommonFunctions.GetCurrentDateTime();

                vm.CurrencyID = CommonFunctions.GetUSDCurrencyId();
                vm.ExchangeRate = CommonFunctions.GetUSDExRate(vm.CurrencyID);
                vm.EmployeeID = useremployee.EmployeeID;
                
                vm.ExchangeRate = Convert.ToDecimal(_currency.ExchangeRate);
                vm.CurrencyCode = _currency.CurrencyCode + ":" + _exchangerate.ToString();
                ViewBag.QuotationMode = "New";
                ViewBag.Title = "Create";
                vm.PaymentDays = 180;
                vm.FreeServiceDays = 60;                
                vm.Details = new List<EstimationDetailVM>();
            }
            else if (id > 0) //edit mode
            {
                
                Estimation item = db.Estimations.Find(id);
                vm.EstimationID = item.EstimationID;
                vm.EnquiryID = item.EnquiryID;
                vm.EnquiryNo = GetEnquiryNo(item.EnquiryID);
                vm.EstimationNo = item.EstimationNo;
                vm.EstimationDate = item.EstimationDate;                
                vm.VarNo = item.VarNo;
                vm.CurrencyID = item.CurrencyID;
                vm.ExchangeRate = item.ExchangeRate;
                vm.CurrencyCode=_currency.CurrencyCode + ":" + vm.ExchangeRate.ToString();
                vm.TotalFCValue = item.TotalFCValue;
                vm.TotalLCValue = item.TotalLCValue;
                vm.EmployeeID  = item.EmployeeID;
                vm.Notes = item.Notes;
                vm.EquipmentsTotal = item.EquipmentsTotal;
                vm.AccessoriesTotal = item.AccessoriesTotal;
                vm.FreightTotal = item.FreightTotal;
                vm.OtherChargesTotal = item.OtherChargesTotal;
                vm.LocalChargesTotal = item.LocalChargesTotal;
                vm.PaymentDays = item.PaymentDays;
                vm.FreeServiceDays = item.FreeServiceDays;
                vm.FinCharge = item.FinCharge;
                vm.FinanceChargesTotal = item.FinanceChargesTotal;
                vm.FinChargePercent = item.FinChargePercent;
                vm.FinPerMonthPercent = item.FinPerMonthPercent;
                vm.FinPerMonth = item.FinPerMonth;
                vm.ChargeableMonth = item.ChargeableMonth;
                vm.Margin = item.Margin;
                vm.MarginPercent = item.MarginPercent;
                vm.SellingValue = item.SellingValue;
                vm.LandingCost = item.LandingCost;
                vm.SoharValue = item.SoharValue;
                vm.SoharValueOMR = item.SoharValueOMR;
                vm.LandingCostOMR = item.LandingCostOMR;
                vm.TotalLandingCostOMR = item.TotalLandingCostOMR;
                var list = (from c in db.Equipments where c.EnquiryID == item.EnquiryID select new DropdownVM { ID = c.ID, Text = c.EquipmentName }).ToList();
                ViewBag.Equipment = list;
                vm.Details = EnquiryDAO.EstimationEquipment(item.EnquiryID, item.EstimationID);
                vm  = GetFinanceCharge1(vm.Details, vm,Convert.ToInt32(vm.PaymentDays));
                vm.Details = vm.Details.OrderBy(cc => cc.Roworder).ToList();
                Session["EstimationDetail"] = vm.Details.OrderBy(cc => cc.Roworder).ToList();
                vm.QuotationDetails = GetQuotationRef(vm.EstimationID);
                
                ViewBag.Title = "Modify";
            }
            vm.DefaultCurrencyID = CommonFunctions.GetDefaultCurrencyId();
            return View(vm);
        }
        [HttpPost]
        public JsonResult SaveEstimation(Estimation estimation, string Details)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());

            var IDetails = JsonConvert.DeserializeObject<List<EstimationDetailVM>>(Details);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Estimation model;

                    if (estimation.EstimationID == 0)
                    {
                        model = new Estimation
                        {
                            EstimationNo = estimation.EstimationNo,
                            EmployeeID = estimation.EmployeeID,
                            EnquiryID = estimation.EnquiryID,
                            VarNo = estimation.VarNo,
                            CreatedBy = userid,
                            CreatedDate = CommonFunctions.GetCurrentDateTime()
                        };

                        db.Estimations.Add(model);
                        
                    }
                    else
                    {
                        model = db.Estimations.Find(estimation.EstimationID);
                        if (model == null)
                        {
                            return Json(new { message = "Estimation not found!", status = "error" });
                        }

                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        // remove existing details in one go
                        var qdetails = db.EstimationDetails
                                         .Where(d => d.EstimationID == estimation.EstimationID);
                        db.EstimationDetails.RemoveRange(qdetails);
                    }

                    // common fields
                    model.EstimationDate = estimation.EstimationDate;
                    model.Notes = estimation.Notes;
                    model.TotalFCValue = estimation.TotalFCValue;
                    model.TotalLCValue = estimation.TotalLCValue;
                    model.CurrencyID = estimation.CurrencyID;
                    model.ExchangeRate = estimation.ExchangeRate;
                    model.EquipmentsTotal = estimation.EquipmentsTotal;
                    model.AccessoriesTotal = estimation.AccessoriesTotal;
                    model.FreightTotal = estimation.FreightTotal;
                    model.FinanceChargesTotal = estimation.FinanceChargesTotal;
                    model.LocalChargesTotal = estimation.LocalChargesTotal;
                    model.OtherChargesTotal = estimation.OtherChargesTotal;
                    model.ChargeableMonth = estimation.ChargeableMonth;
                    model.FinChargePercent = estimation.FinChargePercent;
                    model.PaymentDays = estimation.PaymentDays;
                    model.FreeServiceDays = estimation.FreeServiceDays;
                    model.FinCharge = estimation.FinCharge;
                    model.FinPerMonthPercent = estimation.FinPerMonthPercent;
                    model.FinPerMonth = estimation.FinPerMonth;
                    model.MarginPercent = estimation.MarginPercent;
                    model.Margin = estimation.Margin;
                    model.SellingValue = estimation.SellingValue;
                    model.LandingCost = estimation.LandingCost;
                    model.LandingCostOMR = estimation.LandingCostOMR;
                    model.TotalLandingCostOMR = estimation.TotalLandingCostOMR;
                    model.SoharValue = estimation.SoharValue;
                    model.SoharValueOMR = estimation.SoharValueOMR;
                    model.FYearID = fyearid;
                    model.BranchID = branchId;
                    model.ModifiedBy = userid;
                    model.ModifiedDate = CommonFunctions.GetCurrentDateTime();
                    
                    if (estimation.EstimationID == 0)
                    {
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    // build new details
                    var newDetails = IDetails
                        .Where(d => !d.Deleted)
                        .Select(detail => new EstimationDetail
                        {
                            EstimationID = model.EstimationID, // will be set after save
                    EstimationCategoryID = detail.EstimationCategoryID,
                            EstimationMasterID = detail.EstimationMasterID??0,
                            EquipmentID = detail.EquipmentID,
                            Description = detail.Description,
                            Model = detail.Model,
                            UnitID = detail.UnitID,
                            Qty = detail.Qty,
                            Rate = detail.Rate,
                            FValue = detail.FValue,
                            LValue = detail.LValue,
                            CurrencyID = detail.CurrencyID,
                            ExchangeRate = detail.ExchangeRate
                        }).ToList();

                    db.EstimationDetails.AddRange(newDetails);

                    // save once
                    db.SaveChanges();
                    // commit transaction
                    transaction.Commit();
                    // recalc profit
                    EnquiryDAO.SaveEstimationProfit(model.EstimationID);

                    

                    return Json(new
                    {
                        EstimationID = model.EstimationID,
                        message = estimation.EstimationID == 0
                                  ? "Estimation Added Successfully!"
                                  : "Estimation Updated Successfully!",
                        status = "ok"
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    // rollback transaction
                    transaction.Rollback();

                    return Json(new
                    {
                        message = "Error while saving estimation: " + ex.Message,
                        status = "error"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult ShowQuotationEntry(int Id = 0, int EnquiryID = 0, int QuotationID = 0,int EmployeeID=0)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ViewBag.Curency = db.CurrencyMasters.ToList();
          

            //ViewBag.Unit = db.ItemUnits.ToList();
            QuotationVM vm = new QuotationVM();
            if (Id == 0)
            {
                var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchid, fyearid, EnquiryID, 0,EmployeeID);
                vm.QuotationNo = dta1.QuotationNo;
                vm.Version = dta1.Version;
                vm.QuotationDate = CommonFunctions.GetCurrentDateTime();
                vm.EnquiryID = EnquiryID;
                var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName }).FirstOrDefault();

                if (_client != null)
                {
                    vm.ClientDetail = _client.ClientName;
                    vm.ClientID = _client.ClientID;
                }
                var defaultcurrencyid = CommonFunctions.GetDefaultCurrencyId();
                vm.CurrencyId = defaultcurrencyid;
                //var job = db.JobGenerations.Find(vm.JobID);
                //if (job != null)
                //    vm.ClientDetail = job.Consignor + "\r" + job.OriginAddress;

                //if (job.StatusTypeId == null)
                //{
                //    vm.StatusTypeId = 1;
                //}
                //else
                //{
                //    vm.StatusTypeId = Convert.ToInt32(job.StatusTypeId);
                //}
            }
            else
            {
                Quotation item = db.Quotations.Find(Id);
                vm.QuotationID = item.QuotationID;
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
                vm.EnquiryID = item.EnquiryID;
           
                
                vm.EngineerID = item.EngineerID;
                //vm.ClientDetail = item.ClientDetail;
                var job = db.Enquiries.Find(vm.EnquiryID);
                
                if (QuotationID > 0)
                { //for copy quotation{
                    var dta1 = JobDAO.GetMaxJobQuotationNo(branchid, fyearid, EnquiryID, QuotationID);
                    vm.QuotationNo = dta1.Split('-')[0];
                    vm.Version = Convert.ToInt32(dta1.Split('-')[1]);
                }

            }
            return Json(new { QuotationId = Id, data = vm, message = "Data Found Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReportPrint(int Id)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Estimation Printing";
            ReportsDAO.EstimationReport(Id);

            return View();

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
        //public ActionResult QuotationDetailList()
        //{
        //    ViewBag.Unit = db.ItemUnits.ToList();
        //    List<QuotationDetailVM> vm = new List<QuotationDetailVM>();
        //    return View(vm);
        //}
        public ActionResult ShowQuotationDetailList(int EnquiryID,int QuotationId)
        {
            ViewBag.Unit = db.ItemUnits.ToList();
            QuotationVM vm = new QuotationVM();

            vm.QuotationDetails = EnquiryDAO.QuotationEquipment(EnquiryID,QuotationId);

            return PartialView("QuotationDetailList", vm);
        }
        public EstimationVM GetFinanceCharge1(List<EstimationDetailVM> lst, EstimationVM vm, int _WorkingDays)
        {



            decimal equipmenttotal = vm.EquipmentsTotal ?? 0; //  lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 1).Sum(cc => cc.FValue) ?? 0;
            decimal accessoriestotal = vm.AccessoriesTotal ?? 0;// lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 2).Sum(cc => cc.FValue) ?? 0;
            decimal freighttotal = vm.FreightTotal ?? 0;// lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 3).Sum(cc => cc.FValue) ?? 0;
            decimal localcharges = vm.LocalChargesTotal ?? 0;// lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 5).Sum(cc => cc.LValue) ?? 0;
            decimal othercharges = lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID > 5).Sum(cc => cc.LValue) ?? 0;

            decimal grosstotal = equipmenttotal + accessoriestotal + freighttotal;
            var _currency = db.CurrencyMasters.Where(cc => cc.CurrencyCode == "USD").FirstOrDefault();
            decimal _exchangerate = Convert.ToDecimal(_currency.ExchangeRate);
            if (_currency != null)
            {
                _exchangerate = Convert.ToDecimal(_currency.ExchangeRate);
            }

            _exchangerate = vm.ExchangeRate;
            int workingdays = _WorkingDays;
            int freedays = Convert.ToInt32(vm.FreeServiceDays);
            int workingmonth = (int)Math.Round((decimal)(workingdays - freedays) / 30, 0);

            decimal FinChargePercent = Convert.ToDecimal(vm.FinChargePercent);// 1.75m;
            decimal financecharge = Convert.ToDecimal(vm.FinCharge);// grosstotal * (FinChargePercent / 100);
            decimal financepermonth = Convert.ToDecimal(vm.FinPerMonth);
            decimal FinCharge = Convert.ToDecimal(vm.FinCharge);// Math.Round(financecharge, 2);
            decimal FinPerMonthPercent =Convert.ToDecimal(vm.FinPerMonthPercent); ;// Math.Round(FinChargePercent / 100 / workingmonth, 6);
            decimal FinPerMonth = financepermonth;
            decimal _landingcost = 0;
            decimal _landingcostOMR = 0;
            decimal _SoharValue = 0;
            decimal _SoharValueOMR = 0;
            vm.PaymentDays = workingdays;
            vm.FreeServiceDays = freedays;
            vm.ChargeableMonth = workingmonth;
            vm.EquipmentsTotal = equipmenttotal;
            vm.AccessoriesTotal = accessoriestotal;
            vm.FreightTotal = freighttotal;
            vm.FinCharge = FinCharge;
            vm.FinPerMonth = FinPerMonth;
            vm.FinChargePercent = FinChargePercent;
            vm.FinPerMonthPercent = FinPerMonthPercent;
            vm.FinanceChargesTotal = grosstotal * FinChargePercent;

            EstimationDetailVM item = new EstimationDetailVM
            {
                EstimationCategoryID = 4,
                CategoryName = "Finance Charges",
                Description = "Finance Charge",
                Qty = workingmonth,
                Rate = FinPerMonth,
                FValue = workingmonth * FinPerMonth,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode = "USD",
                ExchangeRate = _exchangerate,
                displayclass = "clsfinance",
                Roworder = 4,
                EstimationMasterID = 6
            };


            EstimationDetailVM checkfinace = lst.Where(cc => cc.EstimationCategoryID == 4).FirstOrDefault();
            if (checkfinace != null)
                lst.Remove(checkfinace);
            lst.Add(item);

            EstimationDetailVM _totalex = lst.Where(cc => cc.Roworder == 1.99M).FirstOrDefault();
            if (_totalex != null)
                lst.Remove(_totalex);
            //var exworkvalue
            EstimationDetailVM exwitem = new EstimationDetailVM
            {
                Roworder = 1.99M,
                EstimationCategoryID = 1,
                CategoryName = "",
                Description = "Total - Ex-works Value",
                Qty = 0,
                Rate = 0,
                FValue = equipmenttotal,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode = "USD",
                ExchangeRate = _exchangerate,
                RowType = "Total",
                displayclass = "clsexworks"
            };

            lst.Add(exwitem);


            EstimationDetailVM _totalgross = lst.Where(cc => cc.Roworder == 3.99M).FirstOrDefault();
            if (_totalgross != null)
                lst.Remove(_totalgross);

            //var equipment accessories freigh total
            EstimationDetailVM grossitem = new EstimationDetailVM
            {
                Roworder = 3.99M,
                EstimationCategoryID = 3,
                CategoryName = "",
                Description = "Gross Total",
                Qty = 0,
                Rate = 0,
                FValue = grosstotal,
                UnitID = 1,
                CurrencyCode = "USD",
                CurrencyID = 3,
                ExchangeRate = _exchangerate,
                RowType = "Total",
                displayclass = "clsgross"
            };

            lst.Add(grossitem);

            EstimationDetailVM _totalsoharitem = lst.Where(cc => cc.Roworder == 4.2M).FirstOrDefault();
            if (_totalsoharitem != null)
                lst.Remove(_totalsoharitem);

            //var landingcost
            _SoharValue = grosstotal + financecharge;

            EstimationDetailVM ShoarItem = new EstimationDetailVM
            {
                Roworder = 4.2M,
                EstimationCategoryID = 4,
                CategoryName = "",
                Description = "Total - Till Port Sohar Value",
                Qty = 0,
                Rate = 0,
                FValue = _SoharValue,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode = "USD",
                ExchangeRate = _exchangerate,
                RowType = "Total",
                displayclass = "clsSohar"
            };


            lst.Add(ShoarItem);
            vm.SoharValue = _SoharValue;


            _SoharValueOMR = vm.SoharValueOMR ?? 0; // (_SoharValue) * _exchangerate;
            EstimationDetailVM _totalshoaromritem = lst.Where(cc => cc.Roworder == 4.3M).FirstOrDefault();
            if (_totalshoaromritem != null)
                lst.Remove(_totalshoaromritem);

            //var equipment accessories freigh total
            EstimationDetailVM Soharomr = new EstimationDetailVM
            {
                Roworder = 4.3M,
                EstimationCategoryID = 4,
                CategoryName = "",
                Description = "Total - Till Port Sohar Value - OMR",
                Qty = 0,
                Rate = 0,
                LValue = _SoharValueOMR,
                UnitID = 1,
                CurrencyID = 1,
                CurrencyCode = "OMR",
                ExchangeRate = 1,
                RowType = "Total",
                displayclass = "clsSoharomr"
            };
            lst.Add(Soharomr);

            vm.SoharValueOMR = _SoharValueOMR;

            EstimationDetailVM _landingcostomr = lst.Where(cc => cc.Roworder == 5.1M).FirstOrDefault();
            if (_landingcostomr != null)
                lst.Remove(_landingcostomr);

            vm.LandingCostOMR = vm.SoharValueOMR + localcharges;
            //var equipment accessories freigh total
            EstimationDetailVM _landingcostomr1 = new EstimationDetailVM
            {
                Roworder = 5.1M,
                EstimationCategoryID = 8,
                CategoryName = "",
                Description = "Landing Cost - OMR",
                Qty = 0,
                Rate = 0,
                LValue = vm.LandingCostOMR,
                UnitID = 1,
                CurrencyID = 1,
                CurrencyCode = "OMR",
                ExchangeRate = 1,
                RowType = "Total",
                displayclass = "clslandingomr"
            };
            lst.Add(_landingcostomr1);

            EstimationDetailVM _totallandingcostomr = lst.Where(cc => cc.Roworder == 8.1M).FirstOrDefault();
            if (_totallandingcostomr != null)
                lst.Remove(_totallandingcostomr);

            //vm.TotalLandingCostOMR = vm.LandingCostOMR + othercharges;
            //var equipment accessories freigh total
            EstimationDetailVM _totallandingcostomr1 = new EstimationDetailVM
            {
                Roworder = 8.1M,
                EstimationCategoryID = 8,
                CategoryName = "",
                Description = "Total - Landing Cost - OMR",
                Qty = 0,
                Rate = 0,
                LValue = vm.TotalLandingCostOMR,
                UnitID = 1,
                CurrencyID = 1,
                CurrencyCode = "OMR",
                ExchangeRate = 1,
                RowType = "Total",
                displayclass = "clslandingomr"
            };
            lst.Add(_totallandingcostomr1);


            vm.Details = lst;
            return vm;
        }
        public EstimationVM GetFinanceCharge(List<EstimationDetailVM> lst,EstimationVM vm,int _WorkingDays)
        {
             
            

            decimal equipmenttotal = lst.Where(cc=>cc.RowType!="Total" && cc.EstimationCategoryID == 1).Sum(cc => cc.FValue) ?? 0;
            decimal accessoriestotal = lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 2).Sum(cc => cc.FValue) ?? 0;
            decimal freighttotal = lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 3).Sum(cc => cc.FValue) ?? 0;
            decimal localcharges = lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID == 5).Sum(cc => cc.LValue) ?? 0;
            decimal othercharges = lst.Where(cc => cc.RowType != "Total" && cc.EstimationCategoryID >5).Sum(cc => cc.LValue) ?? 0;
            
            decimal grosstotal = equipmenttotal + accessoriestotal + freighttotal;
            var _currency = db.CurrencyMasters.Where(cc => cc.CurrencyCode == "USD").FirstOrDefault();
            decimal _exchangerate = Convert.ToDecimal(_currency.ExchangeRate);
            if (_currency != null)
            {
                _exchangerate = Convert.ToDecimal(_currency.ExchangeRate);
            }

            
            int workingdays = _WorkingDays;
            int freedays = 60;
            int workingmonth = (int)Math.Round((decimal)(workingdays - freedays) / 30, 0);

            decimal FinChargePercent = 0.4375m;// 1.75m/workingmonth;// 0.4375m;// 1.75m;
            decimal financecharge = grosstotal * (FinChargePercent/100) * workingmonth;

            decimal financepermonth = 0M;
            decimal FinCharge = 0M;
            decimal FinPerMonthPercent = 0M;
            decimal FinPerMonth = 0M;

            if (workingmonth > 0)
            {
                //FinChargePercent = 0.4375m;  // (1.75/100/workingmonth);// 0.4375m;// 1.75m;
                financepermonth = Math.Round(financecharge / workingmonth, 2);
                FinCharge = Math.Round(financecharge, 2);
                FinPerMonthPercent =  Math.Round(FinChargePercent / 100 / workingmonth, 6);
                FinPerMonth = financepermonth;
            }
            else
            {
                financecharge = 0;
            }

            decimal _landingcost = 0;
            decimal _landingcostOMR = 0;
            decimal _SoharValue = 0;
            decimal _SoharValueOMR = 0;
            decimal _totalLandingcostomr = 0;
            vm.PaymentDays = workingdays;
            vm.FreeServiceDays = freedays;
            vm.ChargeableMonth = workingmonth;
            vm.EquipmentsTotal = equipmenttotal;
            vm.AccessoriesTotal = accessoriestotal;
            vm.FreightTotal = freighttotal;
            vm.FinCharge = FinCharge;
            vm.FinPerMonth = FinPerMonth;
            vm.FinChargePercent = FinChargePercent;
            vm.FinPerMonthPercent = FinPerMonthPercent;
            vm.FinanceChargesTotal = grosstotal * FinChargePercent;
            vm.LocalChargesTotal = localcharges;
            vm.OtherChargesTotal = othercharges;
            EstimationDetailVM item = new EstimationDetailVM
            {    
                EstimationCategoryID = 4,
                CategoryName= "Finance Charges",
                Description = "Finance Charge",
                Qty = workingmonth,
                Rate = FinPerMonth,
                FValue = FinCharge , //workingmonth * FinPerMonth,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode="USD",
                ExchangeRate = _exchangerate,
                displayclass="clsfinance",
                Roworder=4,
                EstimationMasterID=6
            };
             

            EstimationDetailVM checkfinace = lst.Where(cc => cc.EstimationCategoryID == 4).FirstOrDefault();
            if (checkfinace != null)
                lst.Remove(checkfinace);
            lst.Add(item);

            EstimationDetailVM _totalex = lst.Where(cc => cc.Roworder == 1.99M).FirstOrDefault();
            if (_totalex != null)
                lst.Remove(_totalex);
            //var exworkvalue
            EstimationDetailVM exwitem = new EstimationDetailVM
            {
                Roworder = 1.99M,
                EstimationCategoryID = 1,
                CategoryName = "",
                Description = "Total - Ex-works Value",
                Qty = 0,
                Rate = 0,
                FValue = equipmenttotal,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode ="USD",
                ExchangeRate = _exchangerate,
                RowType = "Total",
                displayclass = "clsexworks"
            };

            lst.Add(exwitem);


            EstimationDetailVM _totalgross = lst.Where(cc => cc.Roworder == 3.99M).FirstOrDefault();
            if (_totalgross != null)
                lst.Remove(_totalgross);

            //var equipment accessories freigh total
            EstimationDetailVM grossitem = new EstimationDetailVM
            {
                Roworder = 3.99M,
                EstimationCategoryID = 3,
                CategoryName = "",
                Description = "Gross Total",
                Qty = 0,
                Rate = 0,
                FValue = grosstotal,
                UnitID = 1,
                CurrencyCode = "USD",
                CurrencyID = 3,
                ExchangeRate= _exchangerate,
                RowType = "Total",
                displayclass = "clsgross"
            };

            lst.Add(grossitem);

            EstimationDetailVM _totalsoharitem = lst.Where(cc => cc.Roworder == 4.2M).FirstOrDefault();
            if (_totalsoharitem != null)
                lst.Remove(_totalsoharitem);

            //var landingcost
            _SoharValue = grosstotal + financecharge;

            EstimationDetailVM ShoarItem = new EstimationDetailVM
            {
                Roworder = 4.2M,
                EstimationCategoryID = 4,
                CategoryName = "",
                Description = "Total - Till Port Sohar Value",
                Qty = 0,
                Rate = 0,
                FValue = _SoharValue,
                UnitID = 1,
                CurrencyID = 3,
                CurrencyCode="USD",
                ExchangeRate= _exchangerate,
                RowType = "Total",
                displayclass = "clsSohar"
            };
                      
            
            lst.Add(ShoarItem);
            vm.SoharValue = _SoharValue;


            _SoharValueOMR = (_SoharValue) * _exchangerate;
            EstimationDetailVM _totalshoaromritem = lst.Where(cc => cc.Roworder == 4.3M).FirstOrDefault();
            if (_totalshoaromritem != null)
                lst.Remove(_totalshoaromritem);

            //var equipment accessories freigh total
            EstimationDetailVM Soharomr = new EstimationDetailVM
            {
                Roworder = 4.3M,
                EstimationCategoryID = 4,
                CategoryName = "",
                Description = "Total - Till Port Sohar Value - OMR",
                Qty = 0,
                Rate = 0,
                LValue = Math.Round(_SoharValueOMR, 0),
            UnitID = 1,
                CurrencyID = 1,
                CurrencyCode="OMR",
                ExchangeRate=1,
                RowType = "Total",
                displayclass = "clsSoharomr"
            };
            lst.Add(Soharomr);

            vm.SoharValueOMR = Math.Round(_SoharValueOMR,0);

            EstimationDetailVM _landingcostomr = lst.Where(cc => cc.Roworder == 5.1M).FirstOrDefault();
            if (_landingcostomr != null)
                lst.Remove(_landingcostomr);

            vm.LandingCostOMR = vm.SoharValueOMR + localcharges;
            //var equipment accessories freigh total
            EstimationDetailVM _landingcostomr1 = new EstimationDetailVM
            {
                Roworder = 5.1M,
                EstimationCategoryID = 8,
                CategoryName = "",
                Description = "Landing Cost - OMR",
                Qty = 0,
                Rate = 0,
                LValue = vm.LandingCostOMR,
                UnitID = 1,
                CurrencyID = 1,
                CurrencyCode = "OMR",
                ExchangeRate = 1,
                RowType = "Total",
                displayclass = "clslandingomr"
            };
            lst.Add(_landingcostomr1);

            EstimationDetailVM _totallandingcostomr = lst.Where(cc => cc.Roworder == 8.1M).FirstOrDefault();
            if (_totallandingcostomr != null)
                lst.Remove(_totallandingcostomr);

            vm.TotalLandingCostOMR = Math.Round(Convert.ToDecimal(vm.LandingCostOMR + othercharges),0);
            //var equipment accessories freigh total
            EstimationDetailVM _totallandingcostomr1 = new EstimationDetailVM
            {
                Roworder = 8.1M,
                EstimationCategoryID = 8,
                CategoryName = "",
                Description = "Total - Landing Cost - OMR",
                Qty = 0,
                Rate = 0,
                LValue = vm.TotalLandingCostOMR,
                UnitID = 1,
                CurrencyID = 1,
                CurrencyCode = "OMR",
                ExchangeRate = 1,
                RowType = "Total",
                displayclass= "clslandingomr"
            };
            lst.Add(_totallandingcostomr1);
             

            vm.Details = lst;
            return vm;
        }
        
        public List<QuotationVM> GetQuotationRef(int EstimationId)
        {
            List<QuotationVM> _quotelist = new List<QuotationVM>();

            string QuotationNos = "";
            var _Quotations = (from c in db.QuotationDetails join d in db.Quotations on c.QuotationID equals d.QuotationID where c.EstimationID == EstimationId && d.IsDeleted == false select d).ToList();
            if (_Quotations != null)
            {
                if (_Quotations.Count > 0)
                {
                    foreach(var item in _Quotations)
                    {
                        QuotationVM _quote = new QuotationVM();
                        _quote.QuotationID = item.QuotationID;
                        _quote.QuotationNo = item.QuotationNo;
                        //QuotationNos = QuotationNos + "," + item.QuotationNo;
                        _quotelist.Add(_quote);
                    }                
                }
            }
            return _quotelist;
        }
        
        public ActionResult AddItem(EstimationDetailVM invoice, string Details,int WorkingDays)
        {
            EstimationVM vm = new EstimationVM();
            var IDetails = JsonConvert.DeserializeObject<List<EstimationDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<EstimationDetailVM> list = new List<EstimationDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            EstimationDetailVM item = new EstimationDetailVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<EstimationDetailVM>();
            if (invoice != null)
            {
                item = new EstimationDetailVM();
                item.EstimationCategoryID = invoice.EstimationCategoryID;
                item.EstimationMasterID = invoice.EstimationMasterID;
                item.Roworder = Convert.ToDecimal(item.EstimationCategoryID);
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
                item.CurrencyID = invoice.CurrencyID;
                item.CurrencyCode = invoice.CurrencyCode;
                item.ExchangeRate = invoice.ExchangeRate;
                item.Qty = invoice.Qty;
                item.LValue = invoice.LValue;
                item.FValue = invoice.FValue;
                item.Rate = invoice.Rate;
                item.Deleted = invoice.Deleted;
                item.RowType = invoice.RowType;
                if (list == null)
                {
                    list = new List<EstimationDetailVM>();
                }
                if (item.Deleted == false)
                    list.Add(item);
            }
            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }
            //list=list.OrderBy(cc => cc.EstimationCategoryID).ToList();
           
           
            vm.Details = list;
            vm = GetFinanceCharge(list, vm,WorkingDays);
            vm.Details =vm.Details.OrderBy(cc => cc.Roworder).ToList();
            Session["EstimationDetail"] =vm.Details.OrderBy(cc => cc.Roworder).ToList();
            return PartialView("DetailList", vm);
        }

        [HttpPost]
        public JsonResult DeleteEstimation(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            var _Quotations = (from c in db.QuotationDetails join d in db.Quotations on c.QuotationID equals d.QuotationID where c.EstimationID == id && d.IsDeleted == false select c).ToList();
            if (_Quotations!=null)
            {
                if (_Quotations.Count>0)
                {
                    return Json(new { status = "Failed", message = "Estimation could not delete,Quotation Added!" }, JsonRequestBehavior.AllowGet);
                }

            }
            Estimation obj = db.Estimations.Find(id);
            
            try
            {
                if (obj != null)
                {
                    obj.IsDeleted = true;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                }                
                return Json(new {status="OK", message = "Estimation Deleted Succesfully!"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed"  }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetEquipmentType(string term, int EnquiryID,int CategoryID)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (CategoryID == 1)
            {
                var list = (from c in db.Equipments where c.EnquiryID == EnquiryID select  new DropdownVM { ID = c.ID, Text = c.EquipmentName }).ToList();
                if (term==null)
                { term = ""; }
                if (term.Trim() != "")
                {
                    list=list.Where(cc => cc.Text.ToLower().StartsWith(term.ToLower().Trim())).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();


                    return Json(list, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {

                if (term == null)
                    term = "";

           

                if (term.Trim() != "")
                {
                    var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID && c.TypeName.ToLower().Contains(term) orderby c.TypeName
                                    select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();
                    return Json(list1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID orderby c.TypeName select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();
                    return Json(list1, JsonRequestBehavior.AllowGet);
                }
                    

                
                
            }




        }


        [HttpGet]
        public JsonResult GetEstimationEquipmentType(string term, int EnquiryID, int CategoryID)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (CategoryID == 1)
            {
                var list = (from c in db.EstimationDetails join d in db.Estimations on c.EstimationID equals d.EstimationID where c.EstimationCategoryID==1 &&  d.EnquiryID == EnquiryID select new EquipmentTypeVM { ID = c.EquipmentID.Value, EquipmentType1 = c.Description + " - " + c.Model, BrandName=c.Model }).ToList();
                if (term == null)
                { term = ""; }
                if (term.Trim() != "")
                {
                    list.Where(cc => cc.EquipmentType1.StartsWith(term.Trim())).ToList();

                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();


                    return Json(list, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {

                if (term == null)
                    term = "";



                if (term.Trim() != "")
                {
                    var list1 = (from c in db.EstimationMasters
                                 where c.CategoryID == CategoryID && c.TypeName.ToLower().Contains(term)
                                 orderby c.TypeName
                                 select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();
                    return Json(list1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list1 = (from c in db.EstimationMasters where c.CategoryID == CategoryID orderby c.TypeName select new DropdownVM { ID = c.ID, Text = c.TypeName }).ToList();
                    return Json(list1, JsonRequestBehavior.AllowGet);
                }




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
        public List<QuotationVM> BindQuotations(int EnquiryID)
        {
            List<QuotationVM> List = new List<QuotationVM>();

            List = (from c in db.Quotations join d in db.CurrencyMasters on c.CurrencyId equals d.CurrencyID where c.EnquiryID == EnquiryID select new QuotationVM { EnquiryID = c.EnquiryID, QuotationID = c.QuotationID, QuotationNo = c.QuotationNo, QuotationDate = c.QuotationDate, Version = c.Version, QuotationValue = c.QuotationValue, CurrencyName = d.CurrencyName }).ToList();

            return List;

        }
        [HttpGet]
        public JsonResult GetEstimationCategory()
        {

            var items = db.EstimationCategories.ToList();

            return Json(items, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetQuotationNo(string term,int EnquiryID,int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            List<QuotationVM> objQuotation = new List<QuotationVM>();
            var _list = (from c in db.Quotations where c.EnquiryID == EnquiryID && c.EngineerID == EmployeeID select new QuotationVM { EnquiryID = c.EnquiryID, QuotationID = c.QuotationID, QuotationNo = c.QuotationNo, QuotationDate = c.QuotationDate ,Version=c.Version }).ToList();
            
            //var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0,EmployeeID);
            //_list.Add(new QuotationVM { EnquiryID = EnquiryID,QuotationID=0,QuotationNo= dta1.QuotationNo  +"(New)",Version=dta1.Version });

            return Json(_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewQuotationNo(int EnquiryID, int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());                        
            var dta1 = EnquiryDAO.GetMaxJobQuotationNo(branchId, fyearid, EnquiryID, 0,EmployeeID);
            var _client = (from c in db.EnquiryClients join d in db.ClientMasters on c.ClientID equals d.ClientID where c.EnquiryID == EnquiryID && d.ClientType == "Client" select new { ClientID = c.ClientID, ClientName = d.ClientName,ContactName=d.ContactName,MobileNo=d.ContactNo }).FirstOrDefault();
            var _ClientDetail = "";
            var _ClientID = 0;
            var _ClientContactName = "";
            var _ClientContactNo = "";

            if (_client != null)
            {
                _ClientDetail = _client.ClientName;
                _ClientID = Convert.ToInt32(_client.ClientID);
            }
            return Json(new QuotationVM { EnquiryID = EnquiryID, QuotationID = 0, QuotationNo = dta1.QuotationNo, Version = dta1.Version,ClientID=_ClientID,ClientDetail=_ClientDetail,ContactPerson=_ClientContactName,MobileNumber = _ClientContactNo }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDueDays(DateTime  QuotationDate, DateTime DueDate)
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
        

        public ActionResult Print(int id)
        {
            QuotationVM vm = new QuotationVM();

            Quotation item = db.Quotations.Find(id);
            vm.QuotationID = item.QuotationID;
            vm.EnquiryID = item.EnquiryID;
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

            
            vm.EngineerID = item.EngineerID;
            vm.QuotationStatusID = item.QuotationStatusID;
            if (item.QuotationStatusID > 0)
            {
                var _status = db.QuotationStatus.Find(vm.QuotationStatusID).Status;
                vm.QuotationStatus = _status;
            }
           // vm.QuotationDetails = EnquiryDAO.EnquiryEquipment(item.EnquiryID, item.QuotationID);



            return View(vm);
            //  }
        }


        public string CheckModelName(string Model)
        {

            var _Model = db.Models.Where(cc => cc.ModelName.ToLower().Trim() == Model.Trim()).FirstOrDefault();
            if (_Model != null)
            {
                Model _Model1 = new Model();
                _Model1.ModelName = Model;
                db.Models.Add(_Model    );
                db.SaveChanges(); ;
                return Model;
            }
            else
            {
                return Model;
            }

        }

    }
}