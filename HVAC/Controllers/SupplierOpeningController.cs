using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Dynamic;
using System.Data;
using HVAC.DAL;
using Newtonsoft.Json;
using System.Data.Entity;

namespace HVAC.Controllers
{
    [SessionExpire]
    public class SupplierOpeningController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: CustomerOpening
        public ActionResult Index(SupplierOpeningSearch obj)
        {
            ViewBag.SupplierType = db.SupplierTypes.ToList();

            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<SupplierOpeningVM> lst = CustomerOpeningDAO.SupplierOpeningList(fyearid, branchid, obj.SupplierID,obj.SupplierTypeID);
            obj.Details = lst;

            return View(obj);
        }

        public ActionResult Create(int id=0)
        {
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            AcInvoiceOpeningVM vm = new AcInvoiceOpeningVM();
            vm = GetPostingHead();
            
            if (id==0)
            {
                vm.AcOPInvoiceDetailId = 0;
                vm.InvoiceDate = CommonFunctions.GetCurrentDateTime().Date;
                return View(vm);
            }
            else
            {
                AcOPInvoiceDetail model = db.AcOPInvoiceDetails.Find(id);
                vm.AcOPInvoiceDetailId = model.AcOPInvoiceDetailID;
                AcOPInvoiceMaster master = db.AcOPInvoiceMasters.Where(cc => cc.AcOPInvoiceMasterID == model.AcOPInvoiceMasterID).FirstOrDefault();
                vm.AcOPInvoiceMasterID =Convert.ToInt32(model.AcOPInvoiceMasterID);
                vm.PartyID = Convert.ToInt32(master.PartyID);
                vm.InvoiceNo = model.InvoiceNo;
                if (model.InvoiceDate == null)
                {
                    
                }
                else
                {
                    vm.InvoiceDate =Convert.ToDateTime(model.InvoiceDate);
                }
                var customer = db.CustomerMasters.Find(vm.PartyID);
                vm.PartyName = customer.CustomerName;
                if (model.Amount > 0)
                    vm.Debit = Convert.ToDecimal(model.Amount);
                else if (model.Amount < 0)
                    vm.Credit = Convert.ToDecimal(model.Amount) * -1;

                return View(vm);
            }

        }

        public AcInvoiceOpeningVM GetPostingHead()
        {
            AcInvoiceOpeningVM vm = new AcInvoiceOpeningVM();
            var acsetup = db.AccountSetups.Where(cc => cc.PageName == "SupplierAcOpening").FirstOrDefault();
            if (acsetup != null)
            {
                if (acsetup.DebitAccountId != null)
                {
                    vm.AcHeadID = Convert.ToInt32(acsetup.DebitAccountId);
                    var head = db.AcHeads.Find(acsetup.DebitAccountId);
                    if (head != null)
                    {
                        vm.AccountName = head.AcHead1;                        
                    }
                    else
                    {
                        vm.AccountName = "";
                        vm.AcHeadID = 0;
                    }
                }
                else
                {
                    vm.AcHeadID = 0;
                    vm.AccountName = "";
                }
            }
            else
            {
                vm.AcHeadID = 0; //Customer control account
                vm.AccountName = "";
            }
            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveOpeningInvoice(CustomerInvoiceOpeningVM model)
        {
            try
            {
                int userid = Convert.ToInt32(Session["UserID"]);
                int yearid = Convert.ToInt32(Session["fyearid"].ToString());
                int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
                AcOPInvoiceMaster invoice = new AcOPInvoiceMaster();
                if (model.SupplierTypeID >0)
                {
                    var suppliertype = db.SupplierTypes.Find(model.SupplierTypeID);
                    if (suppliertype!=null)
                    {
                        var achead = db.AcHeads.Find(suppliertype.AcHeadID);
                        if (achead!=null)
                            model.AcHeadID = achead.AcHeadID;
                        else
                            return Json(new { status = "failed", message = "Account Setup in the Supplier Type Not found!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "failed", message = "Supplier Type Not found!" }, JsonRequestBehavior.AllowGet);
                    }

                }



                if (model.AcHeadID == 0)
                {
                    return Json(new { status = "failed", message = "Account Setup not found!" }, JsonRequestBehavior.AllowGet);
                }
                if (model.PartyID>0)
                {
                    invoice = db.AcOPInvoiceMasters.Where(cc => cc.StatusSDSC == "S" && cc.PartyID == model.PartyID).FirstOrDefault(); 
                    if (invoice==null)
                    {
                        model.AcOPInvoiceMasterID = 0;
                    }
                    else
                    {
                        model.AcOPInvoiceMasterID = invoice.AcOPInvoiceMasterID;
                    }
                }                

                if (model.AcOPInvoiceMasterID == 0) // new entry
                {
                    invoice = new AcOPInvoiceMaster();
                    invoice.PartyID = model.PartyID;
                    invoice.AcFinancialYearID = yearid;
                    invoice.StatusSDSC = "S";
                    invoice.SupplierTypeID = model.SupplierTypeID;
                    invoice.AcHeadID = model.AcHeadID;
                    invoice.BranchID = branchid;
                    invoice.OPDate = Convert.ToDateTime(Session["FyearFrom"]);

                    db.AcOPInvoiceMasters.Add(invoice);
                    db.SaveChanges();
                }
              
                 
                    var InvoiceDetail = new AcOPInvoiceDetail();
                
                    if (model.AcOPInvoiceDetailId > 0)
                    {
                        InvoiceDetail = db.AcOPInvoiceDetails.Find(model.AcOPInvoiceDetailId);                        

                    }
                    else
                    {
                        InvoiceDetail.AcOPInvoiceMasterID = invoice.AcOPInvoiceMasterID;
                    }

                    InvoiceDetail.InvoiceDate = model.InvoiceDate;
                    InvoiceDetail.InvoiceNo = model.InvoiceNo;

                    if (model.Debit > 0)
                        InvoiceDetail.Amount = model.Debit;
                    else
                        InvoiceDetail.Amount = model.Credit * -1;

                    
                    if (model.AcOPInvoiceDetailId == 0)
                {
                    InvoiceDetail.CreatedBy = userid;
                    InvoiceDetail.CreatedDate = CommonFunctions.GetCurrentDateTime();
                    InvoiceDetail.ModifiedDate = CommonFunctions.GetCurrentDateTime();
                    InvoiceDetail.ModifiedBy = userid;
                    db.AcOPInvoiceDetails.Add(InvoiceDetail);
                        db.SaveChanges();
                    }
                    else
                {
                   
                    InvoiceDetail.ModifiedDate = CommonFunctions.GetCurrentDateTime();
                    InvoiceDetail.ModifiedBy = userid;
                    db.Entry(InvoiceDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                
                string result=AccountsDAO.SupplierInvoiceOpeningPosting( yearid, branchid);
                if (result!="OK")
                {
                    return Json(new { status = "failed", message = result }, JsonRequestBehavior.AllowGet);
                }
                if (model.AcOPInvoiceDetailId == 0)
                {
                    return Json(new { status = "ok", message = "Opening Added Successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "ok", message = "Opening Updated Successfully!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = "failed", message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }


        //Index page as child item by mc no.
        public ActionResult ShowOpeningDetails(int AcOpInvoiceMasterID = 0)
        {            
            List<AcInvoiceOpeningDetailVM> VM = new List<AcInvoiceOpeningDetailVM>();
            
            if (AcOpInvoiceMasterID > 0)            {               

                VM = CustomerOpeningDAO.CustomerOpeningDetail(AcOpInvoiceMasterID);
                return PartialView("OpeningDetails", VM);
            }

            return PartialView("OpeningDetails", VM);

        }

        //DeleteDetailOpening
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOpeningDetail(int id)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);

            //int k = 0;
            if (id != 0)
            {
                AcOPInvoiceDetail detail = db.AcOPInvoiceDetails.Find(id);
                if (detail != null)
                {
                    int masterid = Convert.ToInt32(detail.AcOPInvoiceMasterID);
                    db.AcOPInvoiceDetails.Remove(detail);

                    db.SaveChanges();

                    AccountsDAO.SupplierInvoiceOpeningPosting(yearid, branchid);

                    return Json(new { status = "ok", message = "Deleted Successfully!" }, JsonRequestBehavior.AllowGet);



                }
            }
                
            return Json(new { status = "Failed", message = "Contact Admin!" });

        }


        //DeleteDetailOpening
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOpeningMaster(int id)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int AcJournalID = 0;
            //int k = 0;
            if (id != 0)
            {
                AcOPInvoiceMaster Master = db.AcOPInvoiceMasters.Find(id);
                AcJournalID = Convert.ToInt32(Master.AcJournalID);

                if (AcJournalID >0)
                {
                    var acjournaldetails = db.AcJournalDetails.Where(cc => cc.AcJournalID == AcJournalID).ToList();
                    db.AcJournalDetails.RemoveRange(acjournaldetails);
                    db.SaveChanges();

                    var acjournal = db.AcJournalMasters.Find(AcJournalID);
                    db.AcJournalMasters.Remove(acjournal);
                    db.SaveChanges();
                }
                if (Master != null)
                {
                    List<AcOPInvoiceDetail> detail = db.AcOPInvoiceDetails.Where(cc=>cc.AcOPInvoiceMasterID==id).ToList();
                    if (detail != null && detail.Count > 0)
                    {
                        db.AcOPInvoiceDetails.RemoveRange(detail);
                        db.SaveChanges();
                    }

                    db.AcOPInvoiceMasters.Remove(Master);
                    db.SaveChanges();                    
                    
                    return Json(new { status = "OK", message = "Supplier Opening Deleted Successfully!" }, JsonRequestBehavior.AllowGet);
                    
                }
            }

            return Json(new { status = "Failed", message = "Contact Admin!" });

        }
    }
}