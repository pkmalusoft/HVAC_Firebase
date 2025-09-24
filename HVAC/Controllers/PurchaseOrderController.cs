using AttributeRouting.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using HVAC.DAL;
using HVAC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rotativa; // Rotativa.MVC for PDF export
namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class PurchaseOrderController : Controller
    {
        HVACEntities db = new HVACEntities();


        public ActionResult Index()
        {
            try
            {
                PurchaseOrderSearch obj = (PurchaseOrderSearch)Session["PurchaseOrderSearch"];
                PurchaseOrderSearch model = new PurchaseOrderSearch();
                int branchid = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
                int depotId = 1; // Convert.ToInt32(Session["CurrentDepotID"].ToString());
                int yearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            if (obj == null || obj.FromDate.ToString().Contains("0001"))
            {
                DateTime pFromDate;
                DateTime pToDate;

                pFromDate = CommonFunctions.GetFirstDayofMonth().Date; // DateTimeOffset.Now.Date;// 
                pToDate = CommonFunctions.GetLastDayofMonth().Date; // DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;

                obj = new PurchaseOrderSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.PurchaseOrderNo = "";
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.PurchaseOrderNo = "";
                Session["PurchaseOrderSearch"] = obj;

                model.Details = new List<PurchaseOrderVM>();
            }
            else
            {
                model = obj;
                var data = ReceiptDAO.SupplierPurchaseOrderList(obj.FromDate, obj.ToDate, obj.SupplierTypeId, obj.PurchaseOrderNo);

                model.Details = data;
                Session["PurchaseOrderSearch"] = model;
            }

            return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging framework)
                ModelState.AddModelError("", "An error occurred while loading the purchase orders. Please try again.");
                return View(new PurchaseOrderSearch());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PurchaseOrderSearch obj)
        {
            if (ModelState.IsValid)
            {
                Session["PurchaseOrderSearch"] = obj;
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public ActionResult Create(int id = 0, int PurchaseOrderID = 0)
        {
            int branchid = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
            int fyearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
            var suppliers = db.SupplierMasters.ToList();
            ViewBag.Supplier = suppliers;
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.ProductType = db.ProductTypes.Where(cc => cc.ProductTypeID != 4).ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.Unit = db.ItemUnits.ToList();
            ViewBag.Port = db.Ports.ToList();
            ViewBag.Bank = db.POBanks.OrderBy(cc => cc.BankName).ToList();
            ViewBag.Supplier = db.SupplierMasters.OrderBy(cc => cc.SupplierName).ToList();
            ViewBag.Country = db.CountryMasters.OrderBy(cc => cc.CountryName).ToList();
            ViewBag.EmployeeMaster = EnquiryDAO.GetPOEmployees(id);
            
            PurchaseOrderSaveRequest _PO = new PurchaseOrderSaveRequest();
            _PO.po = new PurchaseOrder();


            ViewBag.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());

            if (PurchaseOrderID > 0) //new revision
            {
                var _invoice = db.PurchaseOrders.Find(PurchaseOrderID);
                if (_invoice != null)
                {
                    // New PO No
                  //  var Maxnumber = EnquiryDAO.GetSupplierMaxPONo(branchid, fyearid).QuotationNo;

                    _PO.po.PurchaseOrderNo = _invoice.PurchaseOrderNo;
                    _PO.po.PurchaseOrderDate = CommonFunctions.GetCurrentDateTime();
                    _PO.po.SupplierID = _invoice.SupplierID;
                    _PO.po.SONoRef = _invoice.SONoRef;
                    _PO.po.PaymentTerms = _invoice.PaymentTerms;
                    _PO.po.DeliveryTerms = _invoice.DeliveryTerms;
                    _PO.po.INCOTerms = _invoice.INCOTerms;
                    _PO.po.Bank = _invoice.Bank;
                    _PO.po.POValue = _invoice.POValue;
                    _PO.po.TotalAmount = _invoice.TotalAmount;
                    _PO.po.VATAmount = _invoice.VATAmount;
                    _PO.po.VATPercent = _invoice.VATPercent;
                    _PO.po.Remarks = _invoice.Remarks;
                    _PO.po.Refrigerant = _invoice.Refrigerant;
                    _PO.po.CompressorWarranty = _invoice.CompressorWarranty;
                    _PO.po.CurrencyID = _invoice.CurrencyID;
                    _PO.po.UnitWarrantyID = _invoice.UnitWarrantyID;
                    _PO.po.OriginCountryID = _invoice.OriginCountryID;
                    _PO.po.PortID = _invoice.PortID;
                    _PO.po.Revision = _invoice.Revision+1;
                }

                int? bankId = Convert.ToInt32(_invoice.Bank);// int.TryParse(_invoice.Bank, out var bId) ? bId : (int?)null;
                int? dtId = int.TryParse(_invoice.DeliveryTerms, out var dId) ? dId : (int?)null;
                int? itId = int.TryParse(_invoice.INCOTerms, out var iId) ? iId : (int?)null;
                int? ptId = int.TryParse(_invoice.PaymentTerms, out var pId) ? pId : (int?)null;

                var result = new PurchaseOrderTextVM
                {
                    BankText = bankId.HasValue ? db.POBanks.FirstOrDefault(x => x.ID == bankId.Value)?.BankName : null,
                    DeliveryTermsText = dtId.HasValue ? db.PODeliveryTerms.FirstOrDefault(x => x.ID == dtId.Value)?.TermsText : null,
                    INCOTermsText = itId.HasValue ? db.POINCOTerms.FirstOrDefault(x => x.ID == itId.Value)?.TermsText : null,
                    PaymentTermsText = ptId.HasValue ? db.POPaymentTerms.FirstOrDefault(x => x.ID == ptId.Value)?.TermsText : null,
                    SupplierText = db.SupplierMasters.FirstOrDefault(x => x.SupplierID == _invoice.SupplierID)?.SupplierName,
                    RegrigerantText ="",// _invoice.Refrigerant.HasValue ? db.PORegrigerants.Where(x => x.ID == _invoice.Refrigerant.Value).Select(x => x.Regrigerant).FirstOrDefault() : null,
                    CompressorWarrantyText = _invoice.CompressorWarranty.HasValue ? db.POCompressorWarranties.Where(x => x.ID == _invoice.CompressorWarranty.Value).Select(x => x.CompressorWarranty).FirstOrDefault() : null
                };

                _PO.masterDropdowns = result;

            
                //// Other Charges
                //_PO.othercharges = db.PurchaseOrderOtherCharges
                //    .Where(c => c.PurchaseOrderID == PurchaseOrderID)
                //    .ToList();


                // Comments (Do not copy comments)
                _PO.comment = new List<PurchaseOrderUserComment>();
                _PO.Notes = new List<PurchaseOrderNote>();

                // Other Details
                _PO.orderdetails = db.PurchaseOrderOtherDetails
                                      .Where(x => x.PurchaseOrderID == id)
                                      .ToList();
                _PO.Details = EnquiryDAO.PurchaseOrderDetail(PurchaseOrderID);
                _PO.Notes = new List<PurchaseOrderNote>();
                _PO.ApproveDetails= new List<POApproverVM>();
                _PO.po.PreviousValue = _invoice.POValue;
            }
            else if (id > 0)
            {
                ViewBag.Title = "Modify";
                var _invoice = db.PurchaseOrders.Find(id);
                if (_invoice != null)
                {

                    _PO.po.PurchaseOrderID = _invoice.PurchaseOrderID;
                    _PO.po.PurchaseOrderDate = _invoice.PurchaseOrderDate;
                    _PO.po.PurchaseOrderNo = _invoice.PurchaseOrderNo;
                    _PO.po.SupplierID = _invoice.SupplierID;
                    _PO.po.SONoRef= _invoice.SONoRef;
                    _PO.po.PaymentTerms = _invoice.PaymentTerms;
                    _PO.po.DeliveryTerms = _invoice.DeliveryTerms;
                    _PO.po.INCOTerms = _invoice.INCOTerms;
                    _PO.po.Bank = _invoice.Bank;
                    _PO.po.POValue = _invoice.POValue;                    
                    _PO.po.TotalAmount = _invoice.TotalAmount;
                    _PO.po.VATAmount = _invoice.VATAmount;
                    _PO.po.VATPercent = _invoice.VATPercent;
                    _PO.po.Remarks = _invoice.Remarks;
                    _PO.po.Refrigerant = _invoice.Refrigerant;
                    _PO.po.CompressorWarranty = _invoice.CompressorWarranty;
                    _PO.po.FreightCharges = _invoice.FreightCharges;
                    _PO.po.OriginCharges = _invoice.OriginCharges;
                    _PO.po.FinanceCharges = _invoice.FinanceCharges;
                    _PO.po.FinancePercent = _invoice.FinancePercent;
                    _PO.po.CurrencyID = _invoice.CurrencyID;
                    _PO.po.UnitWarrantyID = _invoice.UnitWarrantyID;
                    _PO.po.OriginCountryID = _invoice.OriginCountryID;
                    _PO.po.PortID = _invoice.PortID;
                    _PO.po.Revision = _invoice.Revision;
                }

                int? bankId = Convert.ToInt32(_invoice.Bank);// int.TryParse(_invoice.Bank, out var bId) ? bId : (int?)null;
                int? dtId = int.TryParse(_invoice.DeliveryTerms, out var dId) ? dId : (int?)null;
                int? itId = int.TryParse(_invoice.INCOTerms, out var iId) ? iId : (int?)null;
                int? ptId = int.TryParse(_invoice.PaymentTerms, out var pId) ? pId : (int?)null;

                var result = new PurchaseOrderTextVM
                {
                    BankText = bankId.HasValue ? db.POBanks.FirstOrDefault(x => x.ID == bankId.Value)?.BankName : null,
                    DeliveryTermsText = dtId.HasValue ? db.PODeliveryTerms.FirstOrDefault(x => x.ID == dtId.Value)?.TermsText : null,
                    INCOTermsText = itId.HasValue ? db.POINCOTerms.FirstOrDefault(x => x.ID == itId.Value)?.TermsText : null,
                    PaymentTermsText = ptId.HasValue ? db.POPaymentTerms.FirstOrDefault(x => x.ID == ptId.Value)?.TermsText : null,
                    SupplierText = db.SupplierMasters.FirstOrDefault(x => x.SupplierID == _invoice.SupplierID)?.SupplierName,
                    RegrigerantText = "",// _invoice.Refrigerant.HasValue ? db.PORegrigerants.Where(x => x.ID == _invoice.Refrigerant.Value).Select(x => x.Regrigerant).FirstOrDefault() : null,
                    CompressorWarrantyText = _invoice.CompressorWarranty.HasValue ? db.POCompressorWarranties.Where(x => x.ID == _invoice.CompressorWarranty.Value).Select(x => x.CompressorWarranty).FirstOrDefault() : null,
                    UnitWarrantyText = _invoice.UnitWarrantyID.HasValue ? db.POCompressorWarranties.Where(x => x.ID == _invoice.UnitWarrantyID.Value).Select(x => x.CompressorWarranty).FirstOrDefault() : null
                };

                _PO.masterDropdowns = result;

                _PO.Details = EnquiryDAO.PurchaseOrderDetail(id);

                // Load comments
                _PO.comment = db.PurchaseOrderUserComments
                                .Where(x => x.PurchaseOrderID == id)
                                .OrderByDescending(x => x.EntryDate)
                                .ToList();

                _PO.Notes = db.PurchaseOrderNotes
                                .Where(x => x.PurchaseOrderID == id)                                
                                .ToList();

                // Load other details
                _PO.orderdetails = db.PurchaseOrderOtherDetails
                                     .Where(x => x.PurchaseOrderID == id)
                                     .ToList();


                _PO.ApproveDetails = EnquiryDAO.PurchaseOrderApproveDetail(id);
            }
            else
            {
                ViewBag.Title = "Create";
                var Maxnumber = EnquiryDAO.GetSupplierMaxPONo(branchid, fyearid).QuotationNo;
                _PO.po.PurchaseOrderNo = Maxnumber;
                _PO.po.PurchaseOrderID = 0;
                _PO.po.PurchaseOrderDate = CommonFunctions.GetCurrentDateTime();
                _PO.Details = new List<PurchaseOrderDetailVM>();
                // Set default Finance Charges
                _PO.po.FinancePercent = 0.4375M;
                _PO.po.Revision = 0;
                //_PO.po.VATPercent = 5;
                _PO.Details = EnquiryDAO.PurchaseOrderPendingDetail();
                _PO.Notes = new List<PurchaseOrderNote>();
                _PO.ApproveDetails = new List<POApproverVM>();

            }
            return View(_PO);

        }


        public JsonResult GetSupplierName(string term, int SupplierTypeId)
        {

            if (term.Trim() != "")
            {
                var customerlist = (from c1 in db.SupplierMasters
                                    where c1.SupplierName.ToLower().Contains(term.ToLower()) && (c1.SupplierID == -1 || c1.SupplierTypeID == SupplierTypeId)
                                    orderby c1.SupplierName ascending
                                    select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();

                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customerlist = (from c1 in db.SupplierMasters
                                    where (c1.SupplierID == -1 || c1.SupplierTypeID == SupplierTypeId)
                                    orderby c1.SupplierName ascending
                                    select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();

                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPaymentTerm(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                var termList = (from t in db.POPaymentTerms
                                where t.TermsText.ToLower().Contains(term.ToLower())
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var termList = (from t in db.POPaymentTerms
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetDeliveryTerm(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                var termList = (from t in db.PODeliveryTerms
                                where t.TermsText.ToLower().Contains(term.ToLower())
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var termList = (from t in db.PODeliveryTerms
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetIncoTerm(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                var termList = (from t in db.POINCOTerms
                                where t.TermsText.ToLower().Contains(term.ToLower())
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var termList = (from t in db.POINCOTerms
                                orderby t.TermsText ascending
                                select new { ID = t.ID, TermsText = t.TermsText }).ToList();

                return Json(termList, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetBankName(string term)
        {
            var bankList = string.IsNullOrWhiteSpace(term)
                ? db.POBanks
                    .OrderBy(b => b.BankName)
                    .Select(b => new { b.ID, b.BankName })
                    .ToList()
                : db.POBanks
                    .Where(b => b.BankName.ToLower().Contains(term.ToLower()))
                    .OrderBy(b => b.BankName)
                    .Select(b => new { b.ID, b.BankName })
                    .ToList();

            return Json(bankList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEquipmentType(string term)
        {
            var list = db.EquipmentTypes
                .Where(e => e.EquipmentType1.Contains(term))
                .OrderBy(e => e.EquipmentType1)
                .Select(e => new
                {
                    ID = e.ID,
                    EquipmentName = e.EquipmentType1
                }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegrigerant(string term)
        {
            var list = db.PORegrigerants
                         .Where(x => string.IsNullOrEmpty(term) || x.Regrigerant.Contains(term))
                         .OrderBy(x => x.Regrigerant)
                         .Select(x => new
                         {
                             x.ID,
                             x.Regrigerant
                         })
                         .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompressorWarranty(string term)
        {
            var list = db.POCompressorWarranties
                         .Where(x => x.Type == "Compressor")
                         .OrderBy(x => x.CompressorWarranty)
                         .Select(x => new
                         {
                             x.ID,
                             x.CompressorWarranty
                         })
                         .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitWarranty(string term)
        {
            var list = db.POCompressorWarranties
                         .Where(x=>x.Type == "Unit")
                         .OrderBy(x => x.CompressorWarranty)
                         .Select(x => new
                         {
                             x.ID,
                             x.CompressorWarranty
                         })
                         .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesOrderNo(string JobIDs)
        {
            var _jobs = JobIDs.Split(',');
            
            var _clientpods = "";
            foreach(var item in _jobs)
            {
                if (item != "")
                {
                    int _jobid = Convert.ToInt32(item);
                    var clientpo = db.JobPurchaseOrderDetails.Where(cc => cc.JobHandOverID == _jobid).FirstOrDefault();
                    if (clientpo != null)
                    {
                        if (_clientpods == "")
                            _clientpods = clientpo.PONumber;
                        else
                            if (!_clientpods.Contains(clientpo.PONumber))
                            _clientpods = _clientpods + clientpo.PONumber;
                    }
                }
            }
            
            return Json(_clientpods, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult SetPurchaseOrderDetails(string OrderNo, string Description, decimal Rate, int Qty)
        {
            Random rnd = new Random();
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6

            var invoice = new PurchaseOrderDetailVM();

            //invoice.PurchaseOrderID = OrderNo + "_" + dice;
            invoice.Description = Description;
            invoice.Rate = Rate;
            invoice.Quantity = Qty;

            return Json(new { InvoiceDetails = invoice }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPurchaseOrderDetails(int Id)
        {
            Random rnd = new Random();
            // creates a number between 1 and 6

            var _invoice = db.PurchaseOrders.Find(Id);
            List<PurchaseOrderDetailVM> _details = new List<PurchaseOrderDetailVM>();
            List<PurchaseOrderDetailVM> _details1 = new List<PurchaseOrderDetailVM>();
            _details = ReceiptDAO.GetPurchaseOrderList(Id);


            return Json(new { InvoiceDetails = _details }, JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";

            try
            {
                // Call stored procedure to perform soft delete
                db.Database.ExecuteSqlCommand("EXEC HVAC_DeletePurchaseOrder @PurchaseOrderID = {0}", id);

                status = "OK";
                message = "Purchase Order marked as deleted successfully.";
            }
            catch (Exception ex)
            {
                status = "Failed";
                message = "Error occurred while deleting. " + ex.Message;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }


        

        [HttpPost]
        public ActionResult GetJobEquipmentDetail(int JobID)
        {
            PurchaseOrderVM vm = new PurchaseOrderVM();
            vm.Details = EnquiryDAO.JobEquipmentDetail(JobID);

            Session["POItemDetail"] = vm.Details;
            return PartialView("JobEquipmentDetailList", vm);
        }

        [HttpPost]
        public JsonResult SaveEquipmentRow(PurchaseOrderSaveRequest request)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                var empid = db.EmployeeMasters.Where(cc => cc.UserID == userId).FirstOrDefault().EmployeeID;
                // Save PurchaseOrder 
                var PurchaseOrder = (from d in db.PurchaseOrders where d.PurchaseOrderID == request.po.PurchaseOrderID select d).FirstOrDefault();
                if (PurchaseOrder == null)
                {
                    // Ensure Regrigerant and CompressorWarranty are set for new PO
                    PurchaseOrder po = request.po;
                    po.Refrigerant = request.po.Refrigerant;
                    po.EmployeeID = empid;
                    po.CompressorWarranty = request.po.CompressorWarranty;
                    po.UnitWarrantyID = request.po.UnitWarrantyID;
                    po.CurrencyID = request.po.CurrencyID;
                    po.RevisionRemarks = request.po.RevisionRemarks;
                    po.OriginCountryID = request.po.OriginCountryID;
                    po.PortID = request.po.PortID;
                    po.IsDeleted = false;
                    db.PurchaseOrders.Add(po);
                    db.SaveChanges();
                }
                else
                {
                    // Update each property manually
                    PurchaseOrder.PurchaseOrderNo = request.po.PurchaseOrderNo;
                    PurchaseOrder.PurchaseOrderDate = request.po.PurchaseOrderDate;
                    PurchaseOrder.SupplierID = request.po.SupplierID;
                    PurchaseOrder.CurrencyID = request.po.CurrencyID;
                    PurchaseOrder.PortID = request.po.PortID;
                    PurchaseOrder.OriginCountryID = request.po.OriginCountryID;
                    PurchaseOrder.SONoRef = request.po.SONoRef;
                    PurchaseOrder.Revision = request.po.Revision;
                    PurchaseOrder.RevisionRemarks = request.po.RevisionRemarks;
                    PurchaseOrder.PaymentTerms = request.po.PaymentTerms;
                    PurchaseOrder.DeliveryTerms = request.po.DeliveryTerms;
                    PurchaseOrder.INCOTerms = request.po.INCOTerms;
                    PurchaseOrder.Bank = request.po.Bank;
                    PurchaseOrder.Remarks = request.po.Remarks;
                    PurchaseOrder.POValue = request.po.POValue;                    
                    PurchaseOrder.TotalAmount = request.po.TotalAmount;
                    PurchaseOrder.VATPercent = request.po.VATPercent;
                    PurchaseOrder.VATAmount = request.po.VATAmount;
                    // Ensure Regrigerant and CompressorWarranty are updated for existing PO
                    PurchaseOrder.Refrigerant = request.po.Refrigerant;
                    PurchaseOrder.CompressorWarranty = request.po.CompressorWarranty;
                    PurchaseOrder.FinancePercent = request.po.FinancePercent;
                    PurchaseOrder.FinanceCharges = request.po.FinanceCharges;
                    PurchaseOrder.FreightCharges= request.po.FreightCharges;
                    PurchaseOrder.OriginCharges = request.po.OriginCharges;
                    db.Entry(PurchaseOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Save equipment row logic here
                var equipments = (from d in db.PurchaseOrderDetails where d.PurchaseOrderID == request.po.PurchaseOrderID select d).ToList();
                db.PurchaseOrderDetails.RemoveRange(equipments);
                db.SaveChanges();

                if (request.Details != null)
                {
                    foreach (var item in request.Details)
                    {

                        PurchaseOrderDetail obj = new PurchaseOrderDetail();
                        obj.PurchaseOrderID = request.po.PurchaseOrderID;
                        obj.JobHandOverID = item.JobHandOverID;
                        obj.EquipmentID = item.EquipmentID;
                        obj.EstimationID = item.EstimationID;
                        obj.EquipmentTypeID = item.EquipmentTypeID;
                        obj.QuotationID = item.QuotationID;
                        obj.Model = item.Model;
                        obj.Description = item.Description;
                        obj.Quantity = item.Quantity;
                        obj.ItemUnitID= item.ItemUnitID;
                        obj.Rate = item.Rate;
                        obj.Amount   = item.Amount;
                        obj.ProjectNo = item.ProjectNo;

                        obj.MRequestID = item.MRequestID;
                        db.PurchaseOrderDetails.Add(obj);
                        db.SaveChanges();

                        
                    }
                }

               

                //// Save userComments row logic here
                //var userComments = (from d in db.PurchaseOrderUserComments where d.PurchaseOrderID == request.po.PurchaseOrderID select d).ToList();
                //db.PurchaseOrderUserComments.RemoveRange(userComments);
                //db.SaveChanges();
                //if (request.comment != null)
                //{
                //    foreach (var item in request.comment)
                //    {
                //        item.PurchaseOrderID = request.po.PurchaseOrderID;
                //        item.UserID = userId;
                //        item.EntryDate = DateTime.Now;
                //        db.PurchaseOrderUserComments.Add(item);
                //        db.SaveChanges();
                //    }
                //}

                // Save userComments row logic here
                var userComments = (from d in db.PurchaseOrderNotes where d.PurchaseOrderID == request.po.PurchaseOrderID select d).ToList();
                db.PurchaseOrderNotes.RemoveRange(userComments);
                db.SaveChanges();
                if (request.Notes != null)
                {
                    foreach (var item in request.Notes)
                    {
                        item.PurchaseOrderID = request.po.PurchaseOrderID;                                                
                        db.PurchaseOrderNotes.Add(item);
                        db.SaveChanges();
                    }
                }
                //// Save orderdetails row logic here
                var orderdetails = (from d in db.PurchaseOrderOtherDetails where d.PurchaseOrderID == request.po.PurchaseOrderID select d).ToList();
                db.PurchaseOrderOtherDetails.RemoveRange(orderdetails);
                db.SaveChanges();

                if (request.orderdetails != null)
                {
                    foreach (var item in request.orderdetails)
                    {
                        item.PurchaseOrderID = request.po.PurchaseOrderID;
                        db.PurchaseOrderOtherDetails.Add(item);
                        db.SaveChanges();
                    }
                }

                EnquiryDAO.UpdateMaterialRequestStatus(request.po.PurchaseOrderID,0);

                return Json(new { success = true, PurchaseOrderID = request.po.PurchaseOrderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public JsonResult AddEstimationItem(string Details)
        {
            int JobHandOverID = 0;
            PurchaseOrderVM vm = new PurchaseOrderVM();
            var IDetails = JsonConvert.DeserializeObject<List<PurchaseOrderDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<PurchaseOrderDetailVM> list = new List<PurchaseOrderDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];


            foreach (var item in IDetails)
            {
                if (item.Checked == true)
                {
                    JobHandOverID = Convert.ToInt32(item.JobHandOverID);
                    if (list == null)
                    {
                        list = new List<PurchaseOrderDetailVM>();
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

         
            return Json(new { JobHandOverID = JobHandOverID, data = list, status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddItem(PurchaseOrderDetailVM invoice, string Details)
        {
            PurchaseOrderSaveRequest vm = new PurchaseOrderSaveRequest();
            var IDetails = JsonConvert.DeserializeObject<List<PurchaseOrderDetailVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<PurchaseOrderDetailVM> list = new List<PurchaseOrderDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            PurchaseOrderDetailVM item = new PurchaseOrderDetailVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<PurchaseOrderDetailVM>();
            if (invoice != null)
            {
                item = new PurchaseOrderDetailVM();
                item.JobHandOverID = invoice.JobHandOverID;
                item.ProjectNo = invoice.ProjectNo;
                item.EquipmentTypeID = invoice.EquipmentTypeID;
                item.EquipmentID = invoice.EquipmentID;
                item.Description = invoice.Description;
                item.Model = invoice.Model;
                item.UnitName = invoice.UnitName;
                item.ItemUnitID = invoice.ItemUnitID;
                item.Quantity = invoice.Quantity;
                item.Amount = invoice.Amount;

                item.Rate = invoice.Rate;
                item.Deleted = invoice.Deleted;
                //item.RowType = invoice.RowType;
                if (list == null)
                {
                    list = new List<PurchaseOrderDetailVM>();
                }
                if (item.Deleted == false)
                    list.Add(item);
            }
            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }


            vm.Details = list;
            return PartialView("DetailList", vm);
        }
        public ActionResult AddItem1(string Details, string Details1, int JobID)
        {
            PurchaseOrderSaveRequest vm = new PurchaseOrderSaveRequest();
            var IDetails = JsonConvert.DeserializeObject<List<PurchaseOrderDetailVM>>(Details);
            var IDetails1 = JsonConvert.DeserializeObject<List<PurchaseOrderDetailVM>>(Details1);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Unit = db.ItemUnits.ToList();
            List<PurchaseOrderDetailVM> list = new List<PurchaseOrderDetailVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            PurchaseOrderDetailVM item = new PurchaseOrderDetailVM();

            if (IDetails1.Count > 0 && Details != "[{}]")
                list = IDetails1;
            else
                list = new List<PurchaseOrderDetailVM>();

            list = list.Where(cc => cc.JobHandOverID != JobID).ToList();
            foreach (var item1 in IDetails)
            {

                item = new PurchaseOrderDetailVM();


                item.QuotationID = item1.QuotationID;
                item.EstimationID = item1.EstimationID;
                item.EstimationNo = item1.EstimationNo;
                item.EquipmentTypeID = item1.EquipmentTypeID;
                item.EquipmentID = item1.EquipmentID;                
                item.Description = item1.Description;
                item.Model = item1.Model;
                item.UnitName = item1.UnitName;
                item.ItemUnitID = item1.ItemUnitID;
                item.Quantity = item1.Quantity;
                item.Amount = item1.Amount;
                item.ProjectNo = item1.ProjectNo;
                item.Rate = item1.Rate;
                item.JobHandOverID = item1.JobHandOverID;
         
                item.Deleted = item1.Deleted;

                if (list == null)
                {
                    list = new List<PurchaseOrderDetailVM>();
                }
                if (item.Deleted == false)
                    list.Add(item);
            }
            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }


            vm.Details = list;
            return PartialView("DetailList", vm);
        }

        [HttpPost]
        public JsonResult SaveApprover(POApproverVM obj)
        {
            StatusModel model = EnquiryDAO.SavePOApprover(obj);
            
            return Json(new { Status=model.Status, Message = model.Message }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult GetPOApproverDetail(int PurchaseOrderID)
        {
            PurchaseOrderVM vm = new PurchaseOrderVM();
            vm.ApproveDetails = EnquiryDAO.PurchaseOrderApproveDetail(PurchaseOrderID);
                        
            return PartialView("JobEquipmentDetailList", vm);
        }
        public ActionResult ReportPrint(int Id)
        {

            //ViewBag.JobId = JobId;
            ViewBag.ReportName = "Purchase Order Printing";
            ReportsDAO.PurchaseOrderReport(Id);

            return View();

        }
        public ActionResult PrintPreview(int Id)
        {
            var model = ReportsDAO.GetPurchaseOrderFromProcedure(Id);
            return View("Print", model);
        }
        //public ActionResult Print(int Id)
        //{

        //    var model = GetSamplePurchaseOrder();
        //    return View("PurchaseOrderPrint", model);

        //}
        // Export to PDF using Rotativa
        public ActionResult PrintPdf(int Id)
        {
            var model = ReportsDAO.GetPurchaseOrderFromProcedure(Id);

          //  var model = GetSamplePurchaseOrder();
            return new ViewAsPdf("Print", model)
            {
                FileName = "PurchaseOrder_" + model.OrderNo + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10,10, 10,10)
            };
        }

        private PurchaseOrderViewModel GetSamplePurchaseOrder()
        {
            return new PurchaseOrderViewModel
            {
                OrderNo = "TVU-0645-ABM-RTU-25-009",
                Revision = 1,
                ProjectNo = "TVU0645",
                ProjectName = "Audit Building at MAM",
                SONo = "83V624",
                ProductFamily = "Unitary Light Commercial",
                Product = "Rooftop Package Unit",
                OrderDate = new DateTime(2025, 3, 19),
                PortOfImport = "Sohar, Oman",
                PaymentTerms = "180 days Avalized Draft from BL",
                CountryOfOrigin = "China",
                IncoTerms = "CFR Basis",
                DeliveryWeeks = "8 Weeks",
                Currency = "USD",

                Items = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem { SqNo = 1, Description = "MTZH360DD00300A", Quantity = 6, JobNo = "TVH0039", UnitRate = 9879, TotalPrice = 59274 },
                    new PurchaseOrderItem { SqNo = 2, Description = "MTZH150DC00300A", Quantity = 3, JobNo = "TVU0645", UnitRate = 4127, TotalPrice = 12381 },
                    new PurchaseOrderItem { SqNo = 3, Description = "MTZH210DC00300A", Quantity = 2, JobNo = "TVU0645", UnitRate = 5720, TotalPrice = 11440 },
                    new PurchaseOrderItem { SqNo = 4, Description = "MTZH360DC00300A", Quantity = 2, JobNo = "TVU0645", UnitRate = 8945, TotalPrice = 17890 }
                },

                ImportantNotes = new List<string>
                {
                    "Quality Documents to be provided along with the BL.",
                    "Invoice to mention that the units CONTAIN REFRIGERANT R-410a",
                    "Invoice to be sent in TRIPLICATE",
                    "Invoice to show the HS Code of the Material",
                    "Complete Unit Warranty - 18 months from date of shipment or 12 months from date of commissioning whichever is earlier.",
                    "Unit Compressor Warranty - 66 months from date of shipment or 60 months from date of commissioning whichever is earlier."
                },

                TotalValue = 100985.00m,
                FreightCharges = 7227.00m,
                OriginCharges = 0.00m,
                SubTotal = 108212.00m,
                FinanceCharges = 1894.00m,
                TotalPOValue = 110106.00m,

                ProjectBreakup = new List<ProjectBreakupItem>
                {
                    new ProjectBreakupItem { ProjectCode = "TVH0039", Amount = 64628.00m },
                    new ProjectBreakupItem { ProjectCode = "TVU0645", Amount = 45478.00m }
                },

                ConsigneeName = "AIRMEC OMAN LLC",
                ConsigneeAddress = "P.O. Box-2033, Postal Code - 112, Sultanate of Oman",
                BankName = "Oman Arab Bank SAOC, P.O. Box 2010, Postal Code 112, Ruwi, Sultanate of Oman",
                BankAccount = "A/C No. 3101-137222-300",
                SwiftCode = "OMABOMRUX",

                ChangeOrderDetails = "CFR rates for 2*40HC & 1*20 STD added",
                PreviousPOValue = 102752.00m
            };
        }
    }
}