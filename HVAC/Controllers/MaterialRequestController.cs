using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.DAL;
using HVAC.Models;
using Newtonsoft.Json;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class MaterialRequestController : Controller
    {
        HVACEntities db = new HVACEntities();


        public ActionResult Index()
        {

            MaterialRequestSearch obj = (MaterialRequestSearch)Session["MaterialaRequestSearch"];
            MaterialRequestSearch model = new MaterialRequestSearch();
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
                obj = new MaterialRequestSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["MaterialRequestSearch"] = obj;
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

            List<MaterialRequestVM> lst = EnquiryDAO.MaterialRequestList(model.FromDate, model.ToDate, model.MRNo, model.ProjectNo, EmployeeId, branchid, yearid);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        public ActionResult Index(MaterialRequestSearch obj)
        {
            Session["MaterialaRequestSearch"] = obj;
            return RedirectToAction("Index");
        }
        public ActionResult Pending()
        {

            MaterialRequestSearch obj = (MaterialRequestSearch)Session["MaterialaRequestPending"];
            MaterialRequestSearch model = new MaterialRequestSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int RoleID = Convert.ToInt32(Session["UserRoleID"].ToString());
            int EmployeeId = 0;
            ViewBag.Approver = "False";
            ViewBag.POAccess= "False";
            HVAC.Models.SourceMastersModel obj1 = new HVAC.Models.SourceMastersModel();
            string path = "/PurchaseOrder/Index";
            var popage=obj1.GetAddpermission(Convert.ToInt32(Session["UserRoleID"]), path);
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();

            
                EmployeeId = useremployee.EmployeeID;

                if (useremployee.Approver==true)
                {
                    ViewBag.Approver = "True";
                }
                if (popage == true || RoleID==1)
                {
                    ViewBag.POPage = "True";
                }
                else
                {
                    ViewBag.POPage = "False";
                }

            

            if (obj == null)
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0;
                pFromDate = CommonFunctions.GetFirstDayofMonth().Date;
                pToDate = CommonFunctions.GetLastDayofMonth().Date;
                
                obj = new MaterialRequestSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.EnquiryNo = "";
                Session["MaterialaRequestPending"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.EnquiryNo = "";
            }
            else
            {
                model = obj;
                model.FromDate = obj.FromDate.Date;
                model.ToDate = obj.ToDate.Date;
                model.Status = obj.Status;
                Session["MaterialaRequestPending"] = obj;
            }
            
            List<MaterialRequestDetailVM> lst = EnquiryDAO.MaterialRequestPendingList(model.FromDate, model.ToDate, model.Status, branchid, yearid);
            model.EquipmentDetails = lst;

            return View(model);


        }
        [HttpPost]
        public ActionResult Pending(MaterialRequestSearch obj)
        {
            Session["MaterialaRequestPending"] = obj;
            return RedirectToAction("Pending");
        }

        public ActionResult Create(int id = 0)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            ViewBag.Employee = EnquiryDAO.GetDropdownData("Employee", "");
            ViewBag.BondType = db.BondTypes.OrderBy(cc => cc.BondType1).ToList();
            ViewBag.PaymentInstrument = db.PaymentInstruments.OrderBy(cc => cc.Instruments).ToList();
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            
            int JobID = 0;
             
           MaterialRequestVM vm = new MaterialRequestVM();
            
            

            ViewBag.UserRoleId = Convert.ToInt32(Session["UserRoleID"].ToString());

            ViewBag.EmployeeMaster = db.EmployeeMasters.ToList();

          
        
            if (id == 0) 
            {
                ViewBag.Title = "Create";

                vm.RequestedBy = useremployee.EmployeeID;
                vm.EmployeeName = useremployee.FirstName + " " + useremployee.LastName;
                vm.MRDate = CommonFunctions.GetCurrentDateTime();
                vm.MRNo = EnquiryDAO.GetMRMaxNo(BranchID, fyearid).QuotationNo;

                vm.Details = new List<MaterialRequestDetailVM>();
                
            }
            else
            {
                ViewBag.Title = "Modify";

                MaterialRequest model = db.MaterialRequests.Find(id);
                vm = CopyProperties<MaterialRequestVM>(model);

                if (model.JobHandOverID.HasValue)
                {
                    var _MRequest = db.JobHandovers.FirstOrDefault(emp => emp.JobHandOverID == model.JobHandOverID);
                    vm.ProjectNo = _MRequest.ProjectNumber;                    
                }

                if (model.JobPurchaseOrderDetailID > 0)
                {
                    var _MRequest = db.JobPurchaseOrderDetails.FirstOrDefault(emp => emp.ID == model.JobPurchaseOrderDetailID);
                    vm.PONO = _MRequest.PONumber;
                }

                // Load and map details

                vm.Details = new List<MaterialRequestDetailVM>();
                vm.Details = EnquiryDAO.MaterialRequestDetail(0, vm.MRequestID);
            }

            return View(vm);
        }
        [HttpPost]
        public JsonResult SaveMaterialRequest(MaterialRequestVM materialrequest, string Details)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string UserName = Session["UserName"].ToString();
            var IDetails = JsonConvert.DeserializeObject<List<MaterialRequestDetailVM>>(Details);

            MaterialRequest model;
            bool isNew = materialrequest.MRequestID == 0;

            if (isNew)
            {
                model = new MaterialRequest();
                model.MRNo = EnquiryDAO.GetMRMaxNo(branchId, fyearid).QuotationNo;
                model.RequestedBy = materialrequest.RequestedBy;
                model.MRDate = materialrequest.MRDate;
                model.StoreKeeperID = materialrequest.StoreKeeperID;
                model.JobHandOverID = materialrequest.JobHandOverID;
                model.JobPurchaseOrderDetailID = materialrequest.JobPurchaseOrderDetailID;
                model.Remarks = materialrequest.Remarks;
                model.CreatedBy = userid;
                model.CreatedDate = CommonFunctions.GetCurrentDateTime();
                db.MaterialRequests.Add(model);
            }
            else
            {
                model = db.MaterialRequests.Find(materialrequest.MRequestID);
                if (model == null)
                {
                    return Json(new { message = "Material Request not found!", status = "error" }, JsonRequestBehavior.AllowGet);
                }
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                model.StoreKeeperID = materialrequest.StoreKeeperID;
                model.Remarks = materialrequest.Remarks;
                model.MRDate = materialrequest.MRDate;
                model.JobHandOverID = materialrequest.JobHandOverID;
                model.JobPurchaseOrderDetailID = materialrequest.JobPurchaseOrderDetailID;
            }
         
            
            model.ModifiedBy = userid;
            model.ModifiedDate = CommonFunctions.GetCurrentDateTime();
       
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return Json(new { status = "error", message = exceptionMessage }, JsonRequestBehavior.AllowGet);
            }

            // Remove and re-add details only for update
            if (!isNew)
            {
                var qdetails = db.MaterialRequestDetails.Where(d => d.MRequestID == model.MRequestID).ToList();
                db.MaterialRequestDetails.RemoveRange(qdetails);
                db.SaveChanges();
            }

            foreach (MaterialRequestDetailVM detail in IDetails)
            {
                if (detail.Checked == true)
                {
                    var item = new MaterialRequestDetail
                    {
                        MRequestID = model.MRequestID,
                        JobHandOverID = model.JobHandOverID,
                        EquipmentID = detail.EquipmentID,
                        ItemUnitID = detail.ItemUnitID,
                        EquipmentTypeID = detail.EquipmentTypeID,
                        Quantity = detail.Quantity,
                        Model = detail.Model,
                        Description = detail.Description,
                        StockStatus = detail.StockStatus,
                        EstimationID = detail.EstimationID,
                        QuotationID = detail.QuotationID
                    };
                    db.MaterialRequestDetails.Add(item);
                    db.SaveChanges();
                }
            }

            if (isNew)
            {
                return Json(new { MRequestId = model.MRequestID, message = "Material Request Added Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { MRequestId = model.MRequestID, message = "Material Request Updated Succesfully!", status = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult AddEquipmentEntry(MaterialRequestDetailVM invoice, int index, string Details)
        //{
        //    var IDetails = JsonConvert.DeserializeObject<List<MaterialRequestDetailVM>>(Details);
        //    int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
        //    ViewBag.Unit = db.ItemUnits.ToList();
        //    List<MaterialRequestDetailVM> list = new List<MaterialRequestDetailVM>();
        //    MaterialRequestDetailVM item = new MaterialRequestDetailVM();

        //    if (IDetails.Count > 0 && Details != "[{}]")
        //        list = IDetails;
        //    else
        //        list = new List<MaterialRequestDetailVM>();

        //    item = new MaterialRequestDetailVM();
        //    item.ProjectNo = invoice.ProjectNo;
        //    item.ProjectName = invoice.ProjectName;
        //    item.EquipmentName = invoice.EquipmentName;
        //    item.EquipmentID = invoice.EquipmentID;
        //    item.JobHandOverID = invoice.JobHandOverID;
        //    item.Quantity = invoice.Quantity;
        //    item.Model = invoice.Model;
        //    item.Description = invoice.Description;
        //    item.UnitTypeStockStatus = invoice.UnitTypeStockStatus;
        //    item.EstimationID = invoice.EstimationID;
        //    item.QuotationID = invoice.QuotationID;
        //    item.Deleted = false;

        //    if (list == null)
        //    {
        //        list = new List<MaterialRequestDetailVM>();
        //    }

        //    if (index >= 0 && index < list.Count)
        //    {
        //        // Update the row at the given index
        //        list[index] = item;
        //    }
        //    else
        //    {
        //        // Add as new row
        //        list.Add(item);
        //    }

        //    Session["MRequestDetail"] = list;
        //    MaterialRequestVM vm = new MaterialRequestVM();
        //    vm.Details = list;
        //    return PartialView("RequestDetailList", vm);
        //}
        
        [HttpGet]
        public JsonResult GetClientPO(string term, int projectId)
        {
            var query = EnquiryDAO.GetJobClientPO(projectId);

            var requests = query.Select(m => new { ID = m.ID, TermsText = m.PONumber }).ToList();
            return Json(requests, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectNo(string term, int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var list = EnquiryDAO.GetEmployeeProject(EmployeeID, branchId, fyearid);
            if (term.Trim() != "")
            {
                list = list.Where(cc => cc.ProjectNumber.StartsWith(term)).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRequestEquipment(int ClientPOID)
        {
            var equipmentList = EnquiryDAO.MaterialRequestDetail(ClientPOID, 0);
            MaterialRequestVM vm = new MaterialRequestVM();
            vm.Details = equipmentList;
            return PartialView("DetailList", vm);

        }
        public JsonResult GetProjectEquipment(string term, int ProjectID, int EmployeeID)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var list = EnquiryDAO.GetEmployeeProjectEquipment(EmployeeID, ProjectID, branchId, fyearid);
            if (term.Trim() != "")
            {
                list = list.Where(cc => cc.EquipmentName.StartsWith(term)).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            if (id != 0)
            {
                var request = db.MaterialRequests.Find(id);
                if (request != null)
                {
                    // Remove all details first
                    var details = db.MaterialRequestDetails.Where(d => d.MRequestID == id).ToList();
                    db.MaterialRequestDetails.RemoveRange(details);

                    db.MaterialRequests.Remove(request);
                    db.SaveChanges();
                    return Json(new { status = "OK", message = "Material Request deleted successfully!" });
                }
                else
                {
                    return Json(new { status = "Failed", message = "Material Request not found!" });
                }
            }
            return Json(new { status = "Failed", message = "Invalid ID!" });
        }




        [HttpPost]
        public JsonResult ConfirmIssueStatus(string status, string Details)
        {
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int employeeId = 0;
            var useremployee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            var IDetails = JsonConvert.DeserializeObject<List<MaterialRequestDetailVM>>(Details);
            var _approval = new MaterialApproval();
            _approval.ApprovedBy = useremployee.EmployeeID;
            if (status == "IssueRequestPending")
            {
                _approval.ApprovedFor = "MaterialIssue";
            }
            else if (status == "PORequestPending")
            {
                _approval.ApprovedFor = "PORequest";
            }

            _approval.CreatedBy = userid;            
            _approval.ApprovedDate = CommonFunctions.GetBranchDateTime();
            _approval.CreatedDate = CommonFunctions.GetBranchDateTime();
            db.MaterialApprovals.Add(_approval);
            db.SaveChanges();

            foreach (MaterialRequestDetailVM detail in IDetails)
            {
                  
                    var item = new MaterialApprovalDetail
                    {
                        ApprovalID = _approval.ID,
                        MRequestDetailID = detail.MRequestDetailID,
                        MaterialRequestID = detail.MRequestID                        
                         
                    };
                    db.MaterialApprovalDetails.Add(item);
                    db.SaveChanges();
                 
            }




            return Json(new { status = "OK", message = "Material PO Approve Status Update successfully!" });
               
          
        }

        [HttpPost]
        public JsonResult UpdatePOStatus(int id,bool status)
        {
            if (id != 0)
            {
                var request = db.MaterialRequestDetails.Find(id);
                if (request != null)
                {
                    // Remove all details first
                    request.ReadytoPO = status;

                    db.Entry(request).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { status = "OK", message = "Material PO Approve Status Update successfully!" });
                }
                else
                {
                    return Json(new { status = "Failed", message = "Material PO Approve Status Update Failed!" });
                }
            }
            return Json(new { status = "Failed", message = "Invalid ID!" });
        }

        [HttpPost]
        public JsonResult UpdateIssueStatus(int id, bool status)
        {
            if (id != 0)
            {
                var request = db.MaterialRequestDetails.Find(id);
                if (request != null)
                {
                    // Remove all details first
                    request.ReadytoIssue = status;

                    db.Entry(request).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { status = "OK", message = "Material Issue Approve Status Update successfully!" });
                }
                else
                {
                    return Json(new { status = "Failed", message = "Material Issue Approve Status Update Failed!" });
                }
            }
            return Json(new { status = "Failed", message = "Invalid ID!" });
        }
        public ActionResult Revision(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());

            MaterialRequest old = db.MaterialRequests.Find(id);
            if (old == null)
                return HttpNotFound();

            // Copy all fields except the primary key
            MaterialRequestVM vm = CopyProperties<MaterialRequestVM>(old);
            vm.MRequestID = 0; // So a new record is created
            vm.MRNo = EnquiryDAO.GetMRMaxNo(BranchID, fyearid).QuotationNo; // New MR No
            vm.CreatedBy = userid;
            vm.CreatedDate = CommonFunctions.GetCurrentDateTime();
            vm.ModifiedBy = userid;
            vm.ModifiedDate = CommonFunctions.GetCurrentDateTime();

            // Copy details
            var oldDetails = db.MaterialRequestDetails.Where(d => d.MRequestID == id).ToList();
            vm.Details = new List<MaterialRequestDetailVM>();
            foreach (var detail in oldDetails)
            {
                var detailVM = CopyProperties<MaterialRequestDetailVM>(detail);
                detailVM.MRequestDetailID = 0; // New detail
                detailVM.MRequestID = 0;
                vm.Details.Add(detailVM);
            }

            ViewBag.Title = "Revision";
            return View("Create", vm);
        }
    }
}