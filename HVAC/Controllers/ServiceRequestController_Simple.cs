using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;

namespace HVAC.Controllers
{
    public class ServiceRequestController : Controller
    {
        // GET: ServiceRequest
        public ActionResult Index()
        {
            try
            {
                if (Session["UserID"] == null || Session["fyearid"] == null || 
                    Session["CurrentBranchID"] == null || Session["UserRoleID"] == null)
                {
                    return RedirectToAction("Home", "Home");
                }

                // Return empty list for now - will be populated when database is connected
                var serviceRequests = new List<ServiceRequest>();
                return View(serviceRequests);
            }
            catch (Exception ex)
            {
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

                // Create empty ViewBag items for dropdowns
                ViewBag.CustomerID = new SelectList(new List<object>(), "Value", "Text");
                ViewBag.EquipmentID = new SelectList(new List<object>(), "Value", "Text");
                ViewBag.PriorityID = new SelectList(new List<object>(), "Value", "Text");
                ViewBag.ServiceStatusID = new SelectList(new List<object>(), "Value", "Text");
                ViewBag.ServiceTypeID = new SelectList(new List<object>(), "Value", "Text");
                
                return View();
            }
            catch (Exception ex)
            {
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
                    // For now, just redirect back to index with success message
                    TempData["SuccessMsg"] = "Service Request created successfully! (Database integration pending)";
                    return RedirectToAction("Index");
                }

                // Re-populate ViewBag items
                ViewBag.CustomerID = new SelectList(new List<object>(), "Value", "Text", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(new List<object>(), "Value", "Text", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(new List<object>(), "Value", "Text", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(new List<object>(), "Value", "Text", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(new List<object>(), "Value", "Text", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
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

                // Create a sample model for testing
                var model = new ServiceRequestVM
                {
                    ServiceRequestID = id,
                    ServiceRequestNo = "SR2024001",
                    CustomerID = 1,
                    EquipmentID = 1,
                    PriorityID = 1,
                    ServiceStatusID = 1,
                    ServiceTypeID = 1,
                    ServiceDescription = "Sample service request for testing",
                    ContactPerson = "John Doe",
                    ContactPhone = "1234567890",
                    ContactEmail = "john@example.com",
                    Location = "Main Office",
                    Remarks = "Test remarks"
                };

                ViewBag.CustomerID = new SelectList(new List<object>(), "Value", "Text", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(new List<object>(), "Value", "Text", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(new List<object>(), "Value", "Text", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(new List<object>(), "Value", "Text", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(new List<object>(), "Value", "Text", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
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
                    TempData["SuccessMsg"] = "Service Request updated successfully! (Database integration pending)";
                    return RedirectToAction("Index");
                }

                ViewBag.CustomerID = new SelectList(new List<object>(), "Value", "Text", model.CustomerID);
                ViewBag.EquipmentID = new SelectList(new List<object>(), "Value", "Text", model.EquipmentID);
                ViewBag.PriorityID = new SelectList(new List<object>(), "Value", "Text", model.PriorityID);
                ViewBag.ServiceStatusID = new SelectList(new List<object>(), "Value", "Text", model.ServiceStatusID);
                ViewBag.ServiceTypeID = new SelectList(new List<object>(), "Value", "Text", model.ServiceTypeID);
                
                return View(model);
            }
            catch (Exception ex)
            {
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

                // Create a sample model for testing
                var serviceRequest = new ServiceRequest
                {
                    ServiceRequestID = id,
                    ServiceRequestNo = "SR2024001",
                    ServiceDescription = "Sample service request for testing",
                    CreationDate = DateTime.Now,
                    ContactPerson = "John Doe",
                    ContactPhone = "1234567890",
                    ContactEmail = "john@example.com",
                    Location = "Main Office",
                    Remarks = "Test remarks"
                };

                return View(serviceRequest);
            }
            catch (Exception ex)
            {
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
                // For now, just return success
                return Json(new { status = "OK", message = "Service Request deleted successfully! (Database integration pending)" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Error", message = "Error deleting Service Request: " + ex.Message });
            }
        }

        // GET: ServiceRequest/GetEquipmentByCustomer
        [HttpGet]
        public JsonResult GetEquipmentByCustomer(int customerId)
        {
            try
            {
                // Return sample data for now
                var equipment = new List<object>
                {
                    new { ID = 1, EquipmentName = "HVAC Unit 1 (Brand A - Model X)" },
                    new { ID = 2, EquipmentName = "HVAC Unit 2 (Brand B - Model Y)" }
                };

                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
