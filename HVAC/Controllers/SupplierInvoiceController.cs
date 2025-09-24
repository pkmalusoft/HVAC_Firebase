using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
using Newtonsoft.Json;
using System.Data;
using System.Data.Entity;
namespace HVAC.Controllers
{
    [SessionExpire]
    public class SupplierInvoiceController : Controller
    {
        HVACEntities db = new HVACEntities();
        public ActionResult Index()
        {

            SupplierInvoiceSearch obj = (SupplierInvoiceSearch)Session["SupplierInvoiceSearch"];
            SupplierInvoiceSearch model = new SupplierInvoiceSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            if (obj == null || obj.FromDate.ToString().Contains("0001"))
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0;
                pFromDate = CommonFunctions.GetFirstDayofMonth().Date; // DateTimeOffset.Now.Date;// CommonFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date; //.AddDays(-1) ; // FromDate = DateTime.Now;
                pToDate = CommonFunctions.GetLastDayofMonth().Date; // DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;

                obj = new SupplierInvoiceSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.InvoiceNo = "";
                obj.SupplierTypeId = 0;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.InvoiceNo = "";
                Session["SupplierInvoiceSearch"] = obj;

                model.Details = new List<SupplierInvoiceVM>();
            }
            else
            {
                model = obj;
                var data = ReceiptDAO.GetSupplierInvoiceList(obj.FromDate, obj.ToDate, yearid, obj.InvoiceNo);
                model.Details = data;
                Session["SupplierInvoiceSearch"] = model;
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SupplierInvoiceSearch obj)
        {
            Session["SupplierInvoiceSearch"] = obj;
            return RedirectToAction("Index");
        }
        public ActionResult Create(int id = 0)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            var suppliers = db.SupplierMasters.ToList();
            ViewBag.Supplier = suppliers;
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            ViewBag.ItemType = db.ItemTypes.ToList();
            ViewBag.Unit = db.ItemUnits.ToList();
            SupplierInvoiceVM _supinvoice = new SupplierInvoiceVM();
            ViewBag.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
            if (id > 0)
            {
                ViewBag.Title = "Supplier Invoice -Modify";
                var _invoice = db.SupplierInvoices.Find(id);
                _supinvoice.SupplierInvoiceID = _invoice.SupplierInvoiceID;
                _supinvoice.InvoiceDate = _invoice.InvoiceDate;
                _supinvoice.InvoiceNo = _invoice.InvoiceNo;
                //_supinvoice.SupplierID = _invoice.SupplierID;
                _supinvoice.Remarks = _invoice.Remarks;
                _supinvoice.ReferenceNo = _invoice.ReferenceNo;
                var supplier = suppliers.Where(d => d.SupplierID == _invoice.SupplierID).FirstOrDefault();
                if (supplier != null)
                {
                    _supinvoice.SupplierName = supplier.SupplierName;
                    _supinvoice.SupplierTypeId = Convert.ToInt32(supplier.SupplierTypeID);
                }

                //List<SupplierInvoiceDetail> _details = new List<SupplierInvoiceDetail>();
                List<SupplierInvoiceDetailVM> _details = new List<SupplierInvoiceDetailVM>();
                _details = (from c in db.SupplierInvoiceDetails
                            join a in db.AcHeads on c.AcHeadID equals a.AcHeadID
                            where c.SupplierInvoiceID == id
                            select new SupplierInvoiceDetailVM { SupplierInvoiceDetailID = c.SupplierInvoiceDetailID, SupplierInvoiceID = c.SupplierInvoiceID, AcHeadId = c.AcHeadID, AcHeadName = a.AcHead1, Particulars = c.Particulars, TaxPercentage = c.TaxPercentage, Amount = c.Amount, Rate = c.Rate, Quantity = c.Quantity, Value = c.Value }).ToList();



                _supinvoice.Details = _details;

                Session["SInvoiceListing"] = _details;


                //List<SupplierInvoiceAWBVM> AWBAllocationall = (from c in db.SupplierInvoiceAWBs  join d in db.InScanMasters on c.InScanID equals d.InScanID 
                //                                                       where c.SupplierInvoiceId == id select new SupplierInvoiceAWBVM { ID = c.ID, SupplierInvoiceId = c.SupplierInvoiceId, SupplierInvoiceDetailId = c.SupplierInvoiceDetailId,
                //                                                                    AcHeadId = c.AcHeadId, Amount = c.Amount, InScanID = c.InScanID, ConsignmentNo = d.AWBNo, ConsignmentDate = d.TransactionDate }).ToList();
                //Session["SIAWBAllocation"] = AWBAllocationall;
            }
            else
            {
                ViewBag.Title = "Supplier Invoice - Create";
                _supinvoice.SupplierTypeId = 1;
                var Maxnumber = db.SupplierInvoices.ToList().LastOrDefault();
                _supinvoice.InvoiceNo = ReceiptDAO.SP_GetMaxSINo(branchid, fyearid);

            }
            return View(_supinvoice);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetSupplierInvDetails(int acheadid, string achead, string invno, string Particulars, decimal Rate, int Qty, decimal amount, int currency, decimal Taxpercent, decimal netvalue)
        {
            Random rnd = new Random();
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6

            var invoice = new SupplierInvoiceDetailVM();
            invoice.AcHeadId = acheadid;
            invoice.AcHeadName = achead;
            invoice.InvNo = invno + "_" + dice;
            invoice.Particulars = Particulars;
            invoice.Rate = Rate;
            invoice.Quantity = Qty;
            invoice.CurrencyID = currency;
            var currencyMaster = db.CurrencyMasters.Find(currency);
            invoice.CurrencyAmount = Convert.ToDecimal(currencyMaster.ExchangeRate);
            invoice.Currency = currencyMaster.CurrencyName;
            //var amount = (Qty * Rate);
            //var value = amount + (amount * Taxpercent / 100);

            invoice.Amount = amount;
            invoice.Value = netvalue;
            invoice.TaxPercentage = Taxpercent;

            return Json(new { InvoiceDetails = invoice }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplierInvDetails(int Id)
        {
            Random rnd = new Random();
            // creates a number between 1 and 6

            var _invoice = db.SupplierInvoices.Find(Id);
            List<SupplierInvoiceDetailVM> _details = new List<SupplierInvoiceDetailVM>();
            List<SupplierInvoiceDetailVM> _details1 = new List<SupplierInvoiceDetailVM>();
            _details = (from c in db.SupplierInvoiceDetails
                        join a in db.AcHeads on c.AcHeadID equals a.AcHeadID
                        // join cu in db.CurrencyMasters on c.CurrencyID equals cu.CurrencyID
                        where c.ProjectID == Id
                        select new SupplierInvoiceDetailVM { SupplierInvoiceDetailID = c.SupplierInvoiceDetailID, SupplierInvoiceID = c.SupplierInvoiceID, AcHeadId = c.AcHeadID, AcHeadName = a.AcHead1, Particulars = c.Particulars, TaxPercentage = c.TaxPercentage, Amount = c.Amount, Rate = c.Rate, Quantity = c.Quantity, Value = c.Value }).ToList();



            foreach (var item in _details)
            {
                int dice = rnd.Next(1, 7);
                var invoice = new SupplierInvoiceDetailVM();
                invoice.SupplierInvoiceDetailID = item.SupplierInvoiceDetailID;
                invoice.AcHeadId = item.AcHeadId;
                invoice.AcHeadName = item.AcHeadName;
                invoice.InvNo = _invoice.InvoiceNo + "_" + dice;
                invoice.Particulars = item.Particulars;
                invoice.Rate = item.Rate;
                invoice.Quantity = item.Quantity;
                invoice.CurrencyID = item.CurrencyID;
                var currencyMaster = db.CurrencyMasters.Find(item.CurrencyID);
                //invoice.CurrencyAmount = Convert.ToDecimal(currencyMaster.ExchangeRate);
                //invoice.Currency = currencyMaster.CurrencyName;
                //decimal amount = (item.Quantity * item.Rate);
                //decimal value = amount + (amount * Convert.ToDecimal(item.TaxPercentage) / 100);
                invoice.Amount = item.Amount;
                invoice.Value = item.Value;
                invoice.TaxPercentage = item.TaxPercentage;

                _details1.Add(invoice);
            }

            return Json(new { InvoiceDetails = _details1 }, JsonRequestBehavior.AllowGet);
        }
        //SaveSupplierInvoice
        public JsonResult SaveSupplierInvoice(int Id, int SupplierID, string InvoiceDate, string InvoiceNo, string Remarks, string ReferenceNo, int SupplierTypeId, string Details)
        {
            try
            {
                int UserId = Convert.ToInt32(Session["UserID"]);
                var IDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceDetailVM>>(Details);


                var Supplierinvoice = (from d in db.SupplierInvoices where d.SupplierInvoiceID == Id select d).FirstOrDefault();
                if (Supplierinvoice == null)
                {
                    Supplierinvoice = new SupplierInvoice();
                }
                else
                {


                    //var stockdetails = (from d in db.SupplierInvoiceStocks where d.SupplierInvoiceID == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    //var stockdetails = (from d in db.ItemPurchases join c in db.SupplierInvoiceDetails on d.SupplierInvoiceDetailID equals c.SupplierInvoiceDetailID where c.SupplierInvoiceID == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    //db.ItemPurchases.RemoveRange(stockdetails);
                    //db.SaveChanges();

                    var details = (from d in db.SupplierInvoiceDetails where d.SupplierInvoiceID == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    db.SupplierInvoiceDetails.RemoveRange(details);
                    db.SaveChanges();

                }

                //Supplierinvoice.SupplierID = SupplierID;
                Supplierinvoice.InvoiceDate = Convert.ToDateTime(InvoiceDate);
                Supplierinvoice.InvoiceNo = InvoiceNo;
                Supplierinvoice.AccompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
                Supplierinvoice.BranchId = Convert.ToInt32(Session["CurrentBranchID"]);
                Supplierinvoice.FyearID = Convert.ToInt32(Session["fyearid"]);
                var amount = IDetails.Sum(d => d.Value);
                Supplierinvoice.InvoiceTotal = amount;
                Supplierinvoice.StatusClose = false;
                Supplierinvoice.IsDeleted = false;
                Supplierinvoice.Remarks = Remarks;
                Supplierinvoice.ReferenceNo = ReferenceNo;
                Supplierinvoice.SupplierTypeId = SupplierTypeId;

                Supplierinvoice.ModifiedDate = CommonFunctions.GetCurrentDateTime();
                Supplierinvoice.ModifiedBy = UserId;
                if (Supplierinvoice.SupplierInvoiceID == 0)
                {
                    Supplierinvoice.CreatedBy = UserId;
                    Supplierinvoice.CreatedDate = CommonFunctions.GetCurrentDateTime();
                    db.SupplierInvoices.Add(Supplierinvoice);
                }
                else
                {
                    db.Entry(Supplierinvoice).State = EntityState.Modified;
                }
                db.SaveChanges();
                foreach (var item in IDetails)
                {
                    var InvoiceDetail = new SupplierInvoiceDetail();
                    InvoiceDetail.SupplierInvoiceDetailID = Supplierinvoice.SupplierInvoiceID;
                    InvoiceDetail.AcHeadID = item.AcHeadId;

                    InvoiceDetail.Particulars = item.Particulars;
                    InvoiceDetail.Quantity = item.Quantity;
                    InvoiceDetail.Rate = item.Rate;

                    InvoiceDetail.Amount = item.Amount;
                    InvoiceDetail.TaxPercentage = item.TaxPercentage;
                    InvoiceDetail.Value = item.Value;

                    db.SupplierInvoiceDetails.Add(InvoiceDetail);
                    db.SaveChanges();





                }
                ReceiptDAO dao = new ReceiptDAO();
                dao.GenerateSupplierInvoicePosting(Supplierinvoice.SupplierInvoiceID);
                dao.GenerateStockMasterPosting(Supplierinvoice.SupplierInvoiceID, "Purchase Invoice");

                return Json(new { status = "ok", message = "Invoice Submitted Successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "failed", message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            if (id != 0)
            {
                DataTable dt = ReceiptDAO.DeleteSupplierInvoice(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        status = dt.Rows[0][0].ToString();
                        message = dt.Rows[0][1].ToString();
                        //TempData["ErrorMsg"] = "Transaction Exists. Deletion Restricted !";
                        return Json(new { status = status, message = message });

                    }

                }
                else
                {
                    return Json(new { status = "Failed", message = "Delete Failed!" });
                }
            }

            return Json(new { status = "Failed", message = "Delete Failed!" });

        }

        public ActionResult AccountHead(string term)
        {
            int branchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (!String.IsNullOrEmpty(term))
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList(); ;

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList =db.AcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = MM.AcHeadSelectAll(Common.ParseInt(Session["CurrentBranchID"].ToString()), term);

            }
            else
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID);
                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = db.AcHeadSelectAll(branchID).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
            }
        }


    }
}