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
    public class MaterialIssueController : Controller
    {
        HVACEntities db = new HVACEntities();

        public ActionResult Index()
        {
            MaterialIssueSearch obj = (MaterialIssueSearch)Session["MaterialIssueSearch"];
            MaterialIssueSearch model = new MaterialIssueSearch();
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

            if (obj == null || obj.FromDate.ToString().Contains("0001"))
            {
                DateTime pFromDate;
                DateTime pToDate;

                pFromDate = CommonFunctions.GetFirstDayofMonth().Date;
                pToDate = CommonFunctions.GetLastDayofMonth().Date;
                pFromDate = GeneralDAO.CheckParamDate(pFromDate, yearid).Date;
                pToDate = GeneralDAO.CheckParamDate(pToDate, yearid).Date;

                obj = new MaterialIssueSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.IssueNo = "";
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.IssueNo = "";
                Session["MaterialIssueSearch"] = obj;

                model.Details = new List<MaterialIssueVM>();
            }
            else
            {
                model = obj;
                model.FromDate = GeneralDAO.CheckParamDate(obj.FromDate, yearid).Date;
                model.ToDate = GeneralDAO.CheckParamDate(obj.ToDate, yearid).Date;
            }

            List<MaterialIssueVM> lst = EnquiryDAO.MaterialIssueList(model.FromDate, model.ToDate, model.IssueNo, "", EmployeeId, branchid, yearid);
            model.Details = lst;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MaterialIssueSearch obj)
        {
            Session["MaterialIssueSearch"] = obj;
            return RedirectToAction("Index");
        }

        public ActionResult Create(int id = 0)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            int employeeId = 0;
            string employeeName = "";
            
            //ViewBag.Unit = db.ItemUnits.ToList();
            //ViewBag.EquipmentType = db.EquipmentTypes.ToList();
            //ViewBag.MaterialRequest = db.MaterialRequests.ToList();

            MaterialIssueSaveRequest _GO = new MaterialIssueSaveRequest();
            _GO.mi = new MaterialIssue();

            if (id > 0)
            {
                ViewBag.Title = "Modify";
                var _materialIssue = db.MaterialIssues.Find(id);
                if (_materialIssue != null)
                {
                    _GO.mi.MIssueID = _materialIssue.MIssueID;
                    _GO.mi.IssueDate = _materialIssue.IssueDate;
                     _GO.mi.IssueNo = _materialIssue.IssueNo;
                    _GO.mi.RequestID = _materialIssue.RequestID;
                    _GO.mi.RequestedBy = _materialIssue.RequestedBy;
                    _GO.mi.IssuedBy = _materialIssue.IssuedBy;
                    _GO.mi.ApprovedBy = _materialIssue.ApprovedBy;
                    _GO.mi.JobHandOverID = _materialIssue.JobHandOverID;
                    _GO.mi.Remarks = _materialIssue.Remarks;

                    _GO.RequestedByName = CommonFunctions.GetEmployeeName(_GO.mi.RequestedBy);
                    _GO.ApprovedByName = CommonFunctions.GetEmployeeName(_GO.mi.ApprovedBy);
                    _GO.IssuedByname = CommonFunctions.GetEmployeeName(_GO.mi.IssuedBy);
                    

                    if (_materialIssue.RequestID.HasValue)
                    {
                        var _MRequest = db.MaterialRequests.FirstOrDefault(emp => emp.MRequestID == _materialIssue.RequestID);
                        _GO.RequestNo = _MRequest.MRNo;
                    }

                    if (_materialIssue.JobHandOverID>0)
                    {
                        var _MRequest = db.JobHandovers.FirstOrDefault(emp => emp.JobHandOverID == _materialIssue.JobHandOverID);
                        _GO.ProjectNo = _MRequest.ProjectNumber;
                    }

                }

                // Load material issue details
                _GO.miDetails = EnquiryDAO.MaterialIssueDetail(0, _GO.mi.MIssueID);
            }
            else
            {
                ViewBag.Title = "Create";
                _GO.mi.IssueNo = EnquiryDAO.GetMaterialIssueMAxNo(branchid, yearid).QuotationNo;
                _GO.miDetails = new List<MaterialIssueDetailVM>();
                _GO.mi.IssueDate = CommonFunctions.GetCurrentDateTime();

                _GO.mi.IssuedBy = CommonFunctions.GetLoggedEmployeID();
                _GO.IssuedByname = CommonFunctions.GetLoggedEmployeeName();

            }
            return View(_GO);
        }

        //public string GetEmployeename (int empid)
        //{
        //    var useremployee = db.EmployeeMasters.Find(empid);
        //    if (useremployee != null)
        //    {
               
        //       string employeeName = useremployee.FirstName;
        //        return employeeName;
        //    }

        //    return "";
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveMaterialIssue(MaterialIssueSaveRequest request)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);

                // Save MaterialIssue 
                var materialIssue = (from d in db.MaterialIssues where d.MIssueID == request.mi.MIssueID select d).FirstOrDefault();
                if (materialIssue == null)
                {
                    request.mi.CreatedBy = userId;
                    request.mi.CreatedDate = CommonFunctions.GetBranchDateTime();
                    db.MaterialIssues.Add(request.mi);
                    db.SaveChanges();
                }
                else
                {
                    // Update each property manually
                    materialIssue.IssueDate = request.mi.IssueDate;
                    materialIssue.RequestID = request.mi.RequestID;
                    materialIssue.RequestedBy = request.mi.RequestedBy;
                    materialIssue.ApprovedBy = request.mi.ApprovedBy;
                    materialIssue.IssuedBy = request.mi.IssuedBy;
                    materialIssue.JobHandOverID = request.mi.JobHandOverID;
                    materialIssue.Remarks = request.mi.Remarks;
                    materialIssue.ModifiedBy = userId;
                    materialIssue.ModifiedDate = CommonFunctions.GetBranchDateTime();
                    db.Entry(materialIssue).State = EntityState.Modified;

                    db.SaveChanges();
                }

                // Save material issue details
                var materialIssueDetails = (from d in db.MaterialIssueDetails where d.MIssueID == request.mi.MIssueID select d).ToList();
                db.MaterialIssueDetails.RemoveRange(materialIssueDetails);
                db.SaveChanges();

                if (request.miDetails != null)
                {
                    foreach (var item in request.miDetails)
                    {
                        MaterialIssueDetail obj = new MaterialIssueDetail();
                        obj.EquipmentID = item.EquipmentID;
                        obj.EquipmentTypeID = item.EquipmentTypeID;
                        obj.Model = item.Model;
                        obj.ItemUnitID = item.ItemUnitID;
                        obj.Qty = item.Qty;
                        obj.MIssueID = request.mi.MIssueID;
                        db.MaterialIssueDetails.Add(obj);
                        db.SaveChanges();
                    }
                }
                EnquiryDAO.StockMasterGRNPosting(request.mi.MIssueID, "Material Issue");
                EnquiryDAO.UpdateMaterialRequestStatus( 0, request.mi.MIssueID);

                return Json(new { success = true, MIssueID = request.mi.MIssueID });
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
                db.Database.ExecuteSqlCommand("EXEC HVAC_DeleteMeterialIsuue @MIssueID = {0}", id);

                status = "OK";
                message = "Material Issue marked as deleted successfully.";
            }
            catch (Exception ex)
            {
                status = "Failed";
                message = "Error occurred while deleting. " + ex.Message;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaterialRequestedBy(int RequestId)
        {
            var _MaterialRequest = db.MaterialRequests.Find(RequestId);
            string EmployeeName = "";
            int EmpiD = 0;
            if (_MaterialRequest != null)
            {
                var emp = db.EmployeeMasters.Find(_MaterialRequest.RequestedBy);
                if (emp != null)
                    EmployeeName = emp.FirstName + " " + emp.LastName;
                EmpiD = emp.EmployeeID;
            }
            return Json(new { EmployeeID = EmpiD, EmployeeName = EmployeeName } , JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaterialRequest(string term, int projectId)
        {
            var query = EnquiryDAO.GetJobMaterialRequest(projectId);
            
            
            var requests = query.Select(m => new { ID = m.MRequestID, TermsText = m.MRNo }).ToList();
            return Json(requests, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRequestEquipment(int MRequestId)
        {
            var equipmentList = EnquiryDAO.MaterialIssueDetail(MRequestId, 0);
            MaterialIssueSaveRequest vm = new MaterialIssueSaveRequest();
            vm.miDetails = equipmentList;
            return PartialView("DetailList", vm);
            
        }

        //[HttpGet]
        //public JsonResult GetEquipmentType(string term)
        //{
        //    if (string.IsNullOrEmpty(term))
        //        return Json(new List<object>(), JsonRequestBehavior.AllowGet);

        //    var equipment = db.EquipmentTypes
        //        .Where(e => e.EquipmentType1.ToLower().Contains(term.ToLower()))
        //        .Select(e => new { ID = e.ID, EquipmentName = e.EquipmentType1 })
        //        .Take(10)
        //        .ToList();

        //    return Json(equipment, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetMaterialRequestDetails(int id)
        //{
        //    var mr = db.MaterialRequests.Include("MaterialRequestDetails").FirstOrDefault(m => m.MRequestID == id);
        //    if (mr == null)
        //        return Content("No details found.");
        //    return PartialView("_MaterialRequestDetailsModal", mr);
        //}

        //public JsonResult GetMaterialRequestDetailsTable(int id)
        //{
        //    var details = db.MaterialRequestDetails
        //        .Where(d => d.MRequestID == id)
        //        .Select(d => new {
        //            d.EquipmentID,
        //            EquipmentName = db.EquipmentTypes.FirstOrDefault(e => e.ID == d.EquipmentID).EquipmentType1,
        //            Model = d.Description,
        //            d.Qty,
                    
        //        })
        //        .ToList();
        //    return Json(details, JsonRequestBehavior.AllowGet);
        //}
    }
}