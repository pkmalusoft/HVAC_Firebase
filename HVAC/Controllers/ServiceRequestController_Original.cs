using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.DAL;
using HVAC.Common;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ServiceRequestController : Controller
    {
        private HVACEntities db = new HVACEntities();

        // GET: ServiceRequest
        [OutputCache(Duration = 120, VaryByParam = "none")]
        public ActionResult Index()
        {
            try
            {
                if (Session["UserID"] == null || Session["fyearid"] == null || 
                    Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
                {
                    return RedirectToAction("Home", "Home");
                }

                int branchid = Session["CurrentBranchID"] != null ? Convert.ToInt32(Session["CurrentBranchID"].ToString()) : 0;
                int yearid = Session["fyearid"] != null ? Convert.ToInt32(Session["fyearid"].ToString()) : 0;
                int userid = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"].ToString()) : 0;
                int RoleID = Session["UserRoleID"] != null ? Convert.ToInt32(Session["UserRoleID"].ToString()) : 0;

                var serviceRequests = db.ServiceRequests
                    .Include(s => s.CustomerMaster)
                    .Include(s => s.Equipment)
                    .Include(s => s.Priority)
                    .Include(s => s.ServiceStatus)
                    .Include(s => s.ServiceType)
                    .Where(s => !s.IsDeleted && s.BranchID == branchid && s.AcFinancialYearID == yearid)
                    .OrderByDescending(s => s.CreationDate)
                    .ToList();

                return View(serviceRequests);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Index", "ServiceRequestController", "Index", ex);
                ModelState.AddModelError("", "An error occurred while loading service requests. Please try again.");
                return View(new List<ServiceRequest>());
            }
        }

        // GET: ServiceRequest/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["UserID"] == null || Session["fyearid"] == null || 
                    Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
                {
                    return RedirectToAction("Home", "Home");
                }

                ViewBag.CustomerID = new SelectList(db.CustomerMasters.Where(c => c.StatusActive == true), "CustomerID", "CustomerName");
                ViewBag.EquipmentID = new SelectList(db.Equipments, "ID", "EquipmentName");
                ViewBag.PriorityID = new SelectList(db.Priorities.Where(p => p.IsActive), "PriorityID", "PriorityName");
                ViewBag.ServiceStatusID = new SelectList(db.ServiceStatuses.Where(s => s.IsActive), "ServiceStatusID", "ServiceStatusName");
                ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes.Where(s => s.IsActive), "ServiceTypeID", "ServiceTypeName");
                
                return View();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Create GET", "ServiceRequestController", "Create", ex);
                TempData["ErrorMsg"] = "An error occurred while loading the create form. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceRequestVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceRequest = new ServiceRequest
                    {
                        CustomerID = model.CustomerID,
                        EquipmentID = model.EquipmentID,
                        PriorityID = model.PriorityID,
                        ServiceStatusID = model.ServiceStatusID,
                        ServiceTypeID = model.ServiceTypeID,
                        ServiceDescription = model.ServiceDescription,
                        ServiceRequestNo = GenerateServiceRequestNo(),
                        CreationDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(Session["UserID"]),
                        ContactPerson = model.ContactPerson,
                        ContactPhone = model.ContactPhone,
                        ContactEmail = model.ContactEmail,
                        Location = model.Location,
                        ScheduledDate = model.ScheduledDate,
                        AssignedTo = model.AssignedTo,
                        EstimatedCost = model.EstimatedCost,
                        ActualCost = model.ActualCost,
                        ResolutionNotes = model.ResolutionNotes,
                        Remarks = model.Remarks,
                        IsDeleted = false,
                        BranchID = Convert.ToInt32(Session["CurrentBranchID"]),
                        AcFinancialYearID = Convert.ToInt32(Session["fyearid"])
                    };

                    db.ServiceRequests.Add(serviceRequest);
                    db.SaveChanges();
                    
                    TempData["SuccessMsg"] = "Service Request created successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.CustomerID = new SelectList(db.CustomerMasters.Where(c => c.StatusActive == true), "CustomerID", "CustomerName", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(db.Equipments, "ID", "EquipmentName", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(db.Priorities.Where(p => p.IsActive), "PriorityID", "PriorityName", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(db.ServiceStatuses.Where(s => s.IsActive), "ServiceStatusID", "ServiceStatusName", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes.Where(s => s.IsActive), "ServiceTypeID", "ServiceTypeName", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Create POST", "ServiceRequestController", "Create", ex);
                TempData["ErrorMsg"] = "An error occurred while creating the service request. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // GET: ServiceRequest/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["UserID"] == null || Session["fyearid"] == null || 
                    Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
                {
                    return RedirectToAction("Home", "Home");
                }

                var serviceRequest = db.ServiceRequests.Find(id);
                if (serviceRequest == null)
                {
                    TempData["ErrorMsg"] = "Service Request not found!";
                    return RedirectToAction("Index");
                }

                var model = new ServiceRequestVM
                {
                    ServiceRequestID = serviceRequest.ServiceRequestID,
                    CustomerID = serviceRequest.CustomerID,
                    EquipmentID = serviceRequest.EquipmentID,
                    PriorityID = serviceRequest.PriorityID,
                    ServiceStatusID = serviceRequest.ServiceStatusID,
                    ServiceTypeID = serviceRequest.ServiceTypeID,
                    ServiceDescription = serviceRequest.ServiceDescription,
                    ServiceRequestNo = serviceRequest.ServiceRequestNo,
                    ScheduledDate = serviceRequest.ScheduledDate,
                    AssignedTo = serviceRequest.AssignedTo,
                    ResolutionNotes = serviceRequest.ResolutionNotes,
                    EstimatedCost = serviceRequest.EstimatedCost,
                    ActualCost = serviceRequest.ActualCost,
                    ContactPerson = serviceRequest.ContactPerson,
                    ContactPhone = serviceRequest.ContactPhone,
                    ContactEmail = serviceRequest.ContactEmail,
                    Location = serviceRequest.Location,
                    Remarks = serviceRequest.Remarks
                };

                ViewBag.CustomerID = new SelectList(db.CustomerMasters.Where(c => c.StatusActive == true), "CustomerID", "CustomerName", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(db.Equipments, "ID", "EquipmentName", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(db.Priorities.Where(p => p.IsActive), "PriorityID", "PriorityName", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(db.ServiceStatuses.Where(s => s.IsActive), "ServiceStatusID", "ServiceStatusName", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes.Where(s => s.IsActive), "ServiceTypeID", "ServiceTypeName", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Edit GET", "ServiceRequestController", "Edit", ex);
                TempData["ErrorMsg"] = "An error occurred while loading the service request. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: ServiceRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceRequestVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceRequest = db.ServiceRequests.Find(model.ServiceRequestID);
                    if (serviceRequest != null)
                    {
                        serviceRequest.CustomerID = model.CustomerID;
                        serviceRequest.EquipmentID = model.EquipmentID;
                        serviceRequest.PriorityID = model.PriorityID;
                        serviceRequest.ServiceStatusID = model.ServiceStatusID;
                        serviceRequest.ServiceTypeID = model.ServiceTypeID;
                        serviceRequest.ServiceDescription = model.ServiceDescription;
                        serviceRequest.ScheduledDate = model.ScheduledDate;
                        serviceRequest.AssignedTo = model.AssignedTo;
                        serviceRequest.ResolutionNotes = model.ResolutionNotes;
                        serviceRequest.EstimatedCost = model.EstimatedCost;
                        serviceRequest.ActualCost = model.ActualCost;
                        serviceRequest.ContactPerson = model.ContactPerson;
                        serviceRequest.ContactPhone = model.ContactPhone;
                        serviceRequest.ContactEmail = model.ContactEmail;
                        serviceRequest.Location = model.Location;
                        serviceRequest.Remarks = model.Remarks;
                        serviceRequest.UpdatedBy = Convert.ToInt32(Session["UserID"]);
                        serviceRequest.UpdatedDate = DateTime.Now;

                        db.SaveChanges();
                        
                        TempData["SuccessMsg"] = "Service Request updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Service Request not found!";
                        return RedirectToAction("Index");
                    }
                }

                ViewBag.CustomerID = new SelectList(db.CustomerMasters.Where(c => c.StatusActive == true), "CustomerID", "CustomerName", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(db.Equipments, "ID", "EquipmentName", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(db.Priorities.Where(p => p.IsActive), "PriorityID", "PriorityName", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(db.ServiceStatuses.Where(s => s.IsActive), "ServiceStatusID", "ServiceStatusName", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes.Where(s => s.IsActive), "ServiceTypeID", "ServiceTypeName", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Edit POST", "ServiceRequestController", "Edit", ex);
                TempData["ErrorMsg"] = "An error occurred while updating the service request. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // GET: ServiceRequest/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                if (Session["UserID"] == null || Session["fyearid"] == null || 
                    Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
                {
                    return RedirectToAction("Home", "Home");
                }

                var serviceRequest = db.ServiceRequests
                    .Include(s => s.CustomerMaster)
                    .Include(s => s.Equipment)
                    .Include(s => s.Priority)
                    .Include(s => s.ServiceStatus)
                    .Include(s => s.ServiceType)
                    .FirstOrDefault(s => s.ServiceRequestID == id);

                if (serviceRequest == null)
                {
                    TempData["ErrorMsg"] = "Service Request not found!";
                    return RedirectToAction("Index");
                }

                return View(serviceRequest);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Details", "ServiceRequestController", "Details", ex);
                TempData["ErrorMsg"] = "An error occurred while loading the service request details. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: ServiceRequest/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            try
            {
                var serviceRequest = db.ServiceRequests.Find(id);
                if (serviceRequest != null)
                {
                    serviceRequest.IsDeleted = true;
                    serviceRequest.DeletedBy = Convert.ToInt32(Session["UserID"]);
                    serviceRequest.DeletedDate = DateTime.Now;
                    db.SaveChanges();
                    
                    return Json(new { status = "OK", message = "Service Request deleted successfully!" });
                }
                return Json(new { status = "Error", message = "Service Request not found!" });
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in ServiceRequest Delete", "ServiceRequestController", "Delete", ex);
                return Json(new { status = "Error", message = "Error deleting Service Request: " + ex.Message });
            }
        }

        // GET: ServiceRequest/GetEquipmentByCustomer
        [HttpGet]
        public JsonResult GetEquipmentByCustomer(int customerId)
        {
            try
            {
                var equipment = db.Equipments
                    .Where(e => e.Enquiry.CustomerID == customerId)
                    .Select(e => new { 
                        ID = e.ID, 
                        EquipmentName = e.EquipmentName + " (" + e.Brand + " - " + e.Model + ")" 
                    })
                    .ToList();

                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError("Error in GetEquipmentByCustomer", "ServiceRequestController", "GetEquipmentByCustomer", ex);
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        private string GenerateServiceRequestNo()
        {
            var year = DateTime.Now.Year;
            var count = db.ServiceRequests.Count(s => s.CreationDate.Year == year) + 1;
            return $"SR{year}{count:D4}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
