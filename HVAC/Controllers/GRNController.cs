using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using HVAC.DAL;
using HVAC.Models;
using Newtonsoft.Json;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class GRNController : Controller
    {
        HVACEntities db = new HVACEntities();

        //public ActionResult Index(string SearchText = "")
        //{
        //    List<GRNVM> lst = (from c in db.GRNs
        //                       select new GRNVM { GRNID = c.GRNID, GRNDATE = c.GRNDATE, GRNNO = c.GRNNO }).ToList();
        //    return View(lst);

        //}

        public ActionResult Index()
        {

            GRNSearch obj = (GRNSearch)Session["GRNSearch"];
            GRNSearch model = new GRNSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = 1; // Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            if (obj == null || obj.FromDate.ToString().Contains("0001"))
            {
                DateTime pFromDate;
                DateTime pToDate;

                pFromDate = CommonFunctions.GetFirstDayofMonth().Date; // DateTimeOffset.Now.Date;// 
                pToDate = CommonFunctions.GetLastDayofMonth().Date; // DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;

                obj = new GRNSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.GRNNO = "";
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.GRNNO = "";
                Session["GRNSearch"] = obj;

                model.Details = new List<GRNVM>();
            }
            else
            {
                model = obj;
                var data = ReceiptDAO.GRNList(obj.FromDate, obj.ToDate, obj.SupplierTypeId, obj.GRNNO);
                model.Details = data;
                Session["GRNSearch"] = model;
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GRNSearch obj)
        {
            Session["GRNSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id = 0, int GRNID = 0)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int employeeId = 0;
            string employeeName = "";
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            if (useremployee != null)
            {
                employeeId = useremployee.EmployeeID;
                employeeName = useremployee.FirstName;
            }
            ViewBag.LoggedInEmployeeID = employeeId;
            ViewBag.LoggedInEmployeeName = employeeName;
            var suppliers = db.SupplierMasters.ToList();
            ViewBag.Supplier = suppliers;
            ViewBag.Unit = db.ItemUnits.ToList();
            ViewBag.EquipmentType = db.EquipmentTypes.ToList();
            //ViewBag.Projects = db.ProjectMasters.ToList();
            ViewBag.PurchaseOrder = db.PurchaseOrders.ToList();

            GRNSaveRequest _GO = new GRNSaveRequest();
            _GO.go = new GRN();

            ViewBag.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());

            if (GRNID > 0)
            {
                var _grn = db.GRNs.Find(GRNID);
                if (_grn != null)
                {
                    var Maxnumber = EnquiryDAO.GetMaxGRNCode(branchid, yearid).QuotationNo;

                    _GO.go.GRNID = _grn.GRNID;
                    _GO.go.GRNDATE = _grn.GRNDATE;
                    _GO.go.GRNNO = Maxnumber;
                    _GO.go.SUPPLIERID = _grn.SUPPLIERID;
                    _GO.go.PURCHASEORDERID = _grn.PURCHASEORDERID;
                    _GO.go.EmployeeID = _grn.EmployeeID;
                    _GO.go.Remarks = _grn.Remarks;
                }

                int? empId = int.TryParse(_grn.EmployeeID?.ToString(), out var bId) ? bId : (int?)null;
                int? poId = int.TryParse(_grn.PURCHASEORDERID?.ToString(), out var dId) ? dId : (int?)null;



                var result = new GRMTextVM
                {
                    EmployeeText = empId.HasValue ? db.EmployeeMasters.FirstOrDefault(x => x.EmployeeID == empId.Value)?.FirstName : null,
                    PurchaseOrderText = poId.HasValue ? db.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == poId.Value)?.PurchaseOrderID.ToString() : null,
                    SupplierText = db.SupplierMasters.FirstOrDefault(x => x.SupplierID == _grn.SUPPLIERID)?.SupplierName
                };

                _GO.masterDropdowns = result;

                // Load grn list
                _GO.gRNDetails = new List<GRNDetailVM>();

            }
            else if (id > 0)
            {
                ViewBag.Title = "GRN -Modify";
                var _grn = db.GRNs.Find(id);
                if (_grn != null)
                {

                    _GO.go.GRNID = _grn.GRNID;
                    _GO.go.GRNDATE = _grn.GRNDATE;
                    _GO.go.GRNNO = _grn.GRNNO;
                    _GO.go.SUPPLIERID = _grn.SUPPLIERID;
                    _GO.go.PURCHASEORDERID = _grn.PURCHASEORDERID;
                    _GO.go.EmployeeID = _grn.EmployeeID;
                    _GO.go.Remarks = _grn.Remarks;
                }

                int? empId = int.TryParse(_grn.EmployeeID?.ToString(), out var bId) ? bId : (int?)null;
                int? poId = int.TryParse(_grn.PURCHASEORDERID?.ToString(), out var dId) ? dId : (int?)null;



                var result = new GRMTextVM
                {
                    EmployeeText = empId.HasValue ? db.EmployeeMasters.FirstOrDefault(x => x.EmployeeID == empId.Value)?.FirstName : null,
                    PurchaseOrderText = poId.HasValue ? db.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == poId.Value)?.PurchaseOrderID.ToString() : null,
                    SupplierText = db.SupplierMasters.FirstOrDefault(x => x.SupplierID == _grn.SUPPLIERID)?.SupplierName
                };

                _GO.masterDropdowns = result;
                // Load equipment list
                _GO.gRNDetails = EnquiryDAO.PurchaseOrderDetailforGRN(0, id); ;//  db.GRNDetails.Where(d => d.GRNID == id).ToList();
            }
            else
            {
                ViewBag.Title = "Create";

                var grnString = EnquiryDAO.GetMaxGRNCode(branchid, yearid).QuotationNo;
                _GO.go.GRNNO = grnString; // Store full value like "2025-001"
                _GO.go.GRNDATE = CommonFunctions.GetCurrentDateTime();
                _GO.gRNDetails = new List<GRNDetailVM>();
            }
            return View(_GO);

        }


        public JsonResult GetEmployeeName(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                var empList = (from t in db.EmployeeMasters
                               where t.FirstName.ToLower().Contains(term.ToLower())
                               orderby t.FirstName ascending
                               select new { ID = t.EmployeeID, TermsText = t.FirstName }).ToList();

                return Json(empList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var empList = (from t in db.EmployeeMasters
                               orderby t.FirstName ascending
                               select new { ID = t.EmployeeID, TermsText = t.FirstName }).ToList();

                return Json(empList, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetPurchaseOrderID(string term, int? supplierId = null)
        {
            var query = db.PurchaseOrders.AsQueryable();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(t => t.PurchaseOrderNo.Contains(term));
            }
            if (supplierId.HasValue)
            {
                query = query.Where(t => t.SupplierID == supplierId.Value);
            }
            var poList = query.OrderBy(t => t.PurchaseOrderNo)
                              .Select(t => new {
                                  ID = t.PurchaseOrderID,
                                  TermsText = t.PurchaseOrderNo
                              }).ToList();
            return Json(poList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveGRN(GRNSaveRequest request)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                // Save PurchaseOrder 
                var GRN = (from d in db.GRNs where d.GRNID == request.go.GRNID select d).FirstOrDefault();
                if (GRN == null)
                {
                    db.GRNs.Add(request.go);
                    db.SaveChanges();
                }
                else
                {
                    // Update each property manually
                    GRN.GRNNO = request.go.GRNNO;
                    GRN.GRNDATE = request.go.GRNDATE;
                    GRN.SUPPLIERID = request.go.SUPPLIERID;
                    GRN.PURCHASEORDERID = request.go.PURCHASEORDERID;
                    GRN.EmployeeID = request.go.EmployeeID;
                    GRN.Remarks = request.go.Remarks;

                    db.SaveChanges();
                }

                // Save equipment row logic here
                var grnDetails = (from d in db.GRNDetails where d.GRNID == request.go.GRNID select d).ToList();
                db.GRNDetails.RemoveRange(grnDetails);
                db.SaveChanges();

                if (request.gRNDetails != null)
                {
                    foreach (var item in request.gRNDetails)
                    {

                        GRNDetail obj = new GRNDetail();
                        obj.EquipmentTypeID = item.EquipmentTypeID;
                        obj.ItemUnitID = item.ItemUnitID;
                        obj.EquipmentID = item.EquipmentID;
                        obj.Description = item.Description;
                        obj.Model = item.Model;
                        obj.JobHandOverID = item.JobHandOverID;
                        obj.ProjectNo = item.ProjectNo;
                        obj.Quantity = item.Quantity;
                        obj.Rate = item.Rate;
                        obj.Amount = item.Amount;
                        obj.GRNID = request.go.GRNID;
                        
                        db.GRNDetails.Add(obj);
                        db.SaveChanges();
                    }
                }

                EnquiryDAO.StockMasterGRNPosting(request.go.GRNID, "GRN");


                return Json(new { success = true, GRNID = request.go.GRNID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public JsonResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";

            try
            {
                // Call stored procedure to perform soft delete
                db.Database.ExecuteSqlCommand("EXEC HVAC_DeleteGRN @GRNID = {0}", id);

                status = "OK";
                message = "GRN marked as deleted successfully.";
            }
            catch (Exception ex)
            {
                status = "Failed";
                message = "Error occurred while deleting. " + ex.Message;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }



        //[HttpGet]
        //public JsonResult GetEquipment(string term, int ProductTypeID)
        //{
        //    int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());

        //    if (term.Trim() != "")
        //    {


        //        var productlist = (from c1 in db.EquipmentTypes
        //                           join c2 in db.ItemUnits on c1.ID equals c2.ItemUnitID
        //                           where c1.EquipmentType1.ToLower().StartsWith(term.ToLower())
        //                           && (c1.ID == ProductTypeID || ProductTypeID == 0)
        //                           orderby c1.EquipmentType1 ascending
        //                           select new { ProductID = c1.ID, ProductCode = c1.EquipmentType1, UnitName = c2.ItemUnit1 }).Take(25).ToList();

        //        return Json(productlist, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var productlist = (from c1 in db.EquipmentTypes
        //                           join c2 in db.ItemUnits on c1.ID equals c2.ItemUnitID
        //                           where
        //                           (c1.ID == ProductTypeID || ProductTypeID == 0)
        //                           orderby c1.EquipmentType1 ascending
        //                           select new { ProductID = c1.ID, ProductCode = c1.EquipmentType1, UnitName = c2.ItemUnit1 }).Take(25).ToList();

        //        return Json(productlist, JsonRequestBehavior.AllowGet);

        //    }




        //}


        //public JsonResult GetSupplierName(string term)
        //{

        //    if (term.Trim() != "")
        //    {
        //        var customerlist = (from c1 in db.SupplierMasters
        //                            where c1.SupplierName.ToLower().Contains(term.ToLower()) && (c1.SupplierID != -1)
        //                            orderby c1.SupplierName ascending
        //                            select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();

        //        return Json(customerlist, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var customerlist = (from c1 in db.SupplierMasters
        //                            where (c1.SupplierID != -1)
        //                            orderby c1.SupplierName ascending
        //                            select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();

        //        return Json(customerlist, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpPost]
        //public JsonResult SetGRNDetails(int DetailID, int GRNID, int GRNNo, int EquipmentID, decimal Rate, string ProjectNo, int Qty, decimal amount, decimal Tax, decimal netvalue)
        //{
        //    Random rnd = new Random();
        //    int dice = rnd.Next(1, 7);

        //    var _GRN = new GRNDetailVM();
        //    _GRN.DetailID = DetailID;
        //    _GRN.GRNID = GRNID;
        //    _GRN.GRNNumber = GRNNo + "_" + dice;
        //    _GRN.EquipmentID = EquipmentID;
        //    _GRN.Qty = Qty;
        //    _GRN.Rate = Rate;
        //    _GRN.ProjectNo = ProjectNo;


        //    //var amount = (Qty * Rate);
        //    //var value = amount + (amount * Taxpercent / 100);

        //    _GRN.Amount = amount;
        //    _GRN.Value = netvalue;
        //    _GRN.Tax = Tax;

        //    return Json(new { InvoiceDetails = _GRN }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetGRNDetails(int Id)
        //{

        //    var _Goods = db.GRNs.Find(Id);
        //    List<GRNDetailVM> _details = new List<GRNDetailVM>();
        //    List<GRNDetailVM> _details1 = new List<GRNDetailVM>();
        //    _details = ReceiptDAO.GetGRnList(Id);
        //    return Json(new { InvoiceDetails = _details }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult SaveGRN(int Id, int SupplierID, string GRNDate, string GRNNumber, string SupplierReference, string PONumber, string FASvalue, string Freight, string CFRSValue, int Legalcharge, string Details)
        //{
        //    try
        //    {
        //        int UserId = Convert.ToInt32(Session["UserID"]);
        //        var IDetails = JsonConvert.DeserializeObject<List<GRNDetailVM>>(Details);
        //        GRNMaster Gm = new GRNMaster();
        //        GRNVM gvm = new GRNVM();

        //        var GR = (from d in db.GRNs where d.GRNID == Id select d).FirstOrDefault();
        //        if (GR == null)
        //        {
        //            GR = new GRN();
        //        }
        //        else
        //        {

        //            var stockdetails = (from d in db.EquipmentTypes join c in db.GRNDetails on d.ID equals c.DetailID where c.GRNID == GR.GRNID select d).ToList();
        //            db.EquipmentTypes.RemoveRange(stockdetails);
        //            db.SaveChanges();

        //            var details = (from d in db.GRNDetails where d.GRNID == GR.GRNID select d).ToList();
        //            db.GRNDetails.RemoveRange(details);
        //            db.SaveChanges();

        //        }

        //        GR.GRNID = Id;
        //        GR.GRNDate = Convert.ToDateTime(GRNDate);

        //        GR.GRNNumber = GRNNumber;
        //        GR.PurchaseOrderNo = PONumber;
        //        GR.SupplierID = SupplierID;
        //        GR.SupplierReference = SupplierReference;
        //        Gm.FASValue = FASvalue;
        //        Gm.FREIGHT = Freight;
        //        Gm.CFRSOHARValue = CFRSValue;
        //        Gm.LegalisationCharge = Legalcharge;

        //        var amount = IDetails.Sum(d => d.Value);
        //        if (Id == 0)
        //        {
        //            GR.CreatedBy = UserId;
        //            GR.CreatedDate = CommonFunctions.GetCurrentDateTime();
        //            db.GRNs.Add(GR);
        //        }
        //        else
        //        {
        //            db.Entry(GR).State = EntityState.Modified;
        //        }
        //        db.SaveChanges();
        //        foreach (var item in IDetails)
        //        {
        //            var GRNDetail = new GRNDetail();
        //            GRNDetail.GRNID = GR.GRNID;
        //            var product = db.EquipmentTypes.Find(item.EquipmentID);

        //            if (product != null)
        //                GRNDetail.EquipmentID = product.ID;
        //            GRNDetail.Unit = item.Unit;
        //            GRNDetail.Qty = item.Qty;
        //            GRNDetail.Rate = item.Rate;
        //            GRNDetail.Tax = item.Tax;
        //            GRNDetail.ProjectID = item.ProjectID;
        //            db.GRNDetails.Add(GRNDetail);
        //            db.SaveChanges();
        //        }
        //        return Json(new { status = "ok", message = "GRN Submitted Successfully!" }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "failed", message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);

        //    }
        //}

        //public JsonResult DeleteConfirmed(int id)
        //{
        //    string status = "";
        //    string message = "";
        //    if (id != 0)
        //    {
        //        DataTable dt = ReceiptDAO.DeleteGRN(id);
        //        if (dt != null)
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                status = dt.Rows[0][0].ToString();
        //                message = dt.Rows[0][1].ToString();
        //                return Json(new { status = status, message = message });

        //            }

        //        }
        //        else
        //        {
        //            return Json(new { status = "Failed", message = "Delete Failed!" });
        //        }
        //    }

        //    return Json(new { status = "Failed", message = "Delete Failed!" });

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPurchaseOrderEquipment(int purchaseOrderId,int GRNID)
        {
            var equipmentList = EnquiryDAO.PurchaseOrderDetailforGRN(purchaseOrderId,GRNID);
            GRNSaveRequest vm = new GRNSaveRequest();
            vm.gRNDetails = equipmentList;            
            return PartialView("DetailList", vm);
            //return Json(equipmentList, JsonRequestBehavior.AllowGet);
        }

    }
}