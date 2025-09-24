using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Data;
using HVAC.DAL;
using System.Data.Entity;
using Newtonsoft.Json;
using Ganss.Xss;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ProductTypeController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index()
        {
            ViewBag.Brand = db.Brands.OrderBy(cc => cc.BrandName).ToList();
            EquipmentTypeSearch obj = (EquipmentTypeSearch)Session["EquipmentTypeSearch"];
            EquipmentTypeSearch model = new EquipmentTypeSearch();
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

                obj = new EquipmentTypeSearch();
                obj.BrandID = 0;
                Session["EquipmentTypeSearch"] = obj;

            }
            else
            {
                model = obj;
            }

            List<EquipmentTypeVM> lst = EnquiryDAO.EquipmentTypeList(model.BrandID);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EquipmentTypeSearch obj)
        {
            Session["EquipmentTypeSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id = 0)
        {
            ViewBag.Brand = db.Brands.OrderBy(cc => cc.BrandName).ToList();
            ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductFamilyName).ToList();

            Models.EquipmentTypeVM equipmentType = new Models.EquipmentTypeVM();
            if (id == 0)
            {
                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.EquipmentType type = (from c in db.EquipmentTypes where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    equipmentType.ID= type.ID;
                    equipmentType.EquipmentType1 = type.EquipmentType1;
                    equipmentType.ProductFamilyID  = type.ProductFamilyID;
                    equipmentType.BrandID = type.BrandID;                    

                }
            }
            return View(equipmentType);
        }

        public JsonResult ProductTypeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.EquipmentTypes where b.EquipmentType1 == PType select b).FirstOrDefault();
                if (existtype != null)
                {
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    status = "false";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var existtypebyid = (from b in db.EquipmentTypes where b.EquipmentType1 == PType && b.ID != ID select b).FirstOrDefault();
                if (existtypebyid != null)
                {
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    status = "false";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveProductType(Models.EquipmentType ProType)
        {

            Models.EquipmentType Ptype = new Models.EquipmentType();

            if (ProType.ID == 0)
            {

                var duplicate = (from c in db.EquipmentTypes where c.EquipmentType1 == ProType.EquipmentType1 select c).FirstOrDefault();

                if (duplicate !=null)
                {
                   
                    return Json(new { status = "Failed", message = "Duplicate Product Type Name Not allowed!" });

                }
                Ptype.EquipmentType1 = ProType.EquipmentType1;                
                db.EquipmentTypes.Add(Ptype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Product Type Added Successfully!" });
            }
            else
            {
                var duplicate = (from c in db.EquipmentTypes where c.EquipmentType1 == ProType.EquipmentType1 && c.ID != ProType.ID select c).FirstOrDefault();
                 
                if (duplicate != null)
                {
                    return Json(new { status = "Failed", message = "Product Type already Exist!" });

                }

                Ptype = db.EquipmentTypes.Find(ProType.ID);
                Ptype.EquipmentType1 = ProType.EquipmentType1;
                Ptype.ProductFamilyID = ProType.ProductFamilyID;
                Ptype.BrandID = ProType.BrandID;

                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Product Type Updated Successfully!" });
            }
        }

        public JsonResult DeleteProductType(int id)
        {

            Models.EquipmentType protype = db.EquipmentTypes.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var _enquiry = db.Equipments.Where(cc => cc.EquipmentTypeID == id).FirstOrDefault();
                if (_enquiry!=null)
                {
                    return Json(new { status = "Failed", message = "Product Type referenced in the Equipment,Could not Delete!" });
                }
                var protypes = db.EquipmentTypes.Where(cc => cc.ID == id).ToList();
                db.EquipmentTypes.Remove(protype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Product Type Deleted Successfully!" });

            }

        }

        public ActionResult Details(int id =0)
        {
            ViewBag.Brand = db.Brands.OrderBy(cc => cc.BrandName).ToList();
            ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductFamilyName).ToList();

            Models.EquipmentTypeVM equipmentType = new Models.EquipmentTypeVM();
            if (id == 0)
            {
                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.EquipmentType type = (from c in db.EquipmentTypes where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    equipmentType.ID = type.ID;
                    equipmentType.EquipmentType1 = type.EquipmentType1;
                    equipmentType.ProductFamilyID = type.ProductFamilyID;
                    equipmentType.BrandID = type.BrandID;
                    if (type.BrandID > 0)
                    {
                        var _brand = db.Brands.Find(type.BrandID);
                        equipmentType.BrandName = _brand.BrandName;
                    }
                    else
                        equipmentType.BrandName = "N/A";

                    var _pfamily = db.ProductFamilies.Find(type.ProductFamilyID);
                    if (_pfamily != null)
                        equipmentType.ProductFamilyName = _pfamily.ProductFamilyName;
                    else
                        equipmentType.ProductFamilyName = "";

                    var _detail1 = (from c in db.EquipmentScopeofworks where c.EquipmentTypeID == type.ID select new EquipmentScopeofworkVM { ID = c.ID, Model=c.Model, Description = c.Description, Deleted = false,OrderNo=c.OrderNo }).ToList();
                    var _detail2 = (from c in db.EquipmentWarranties where c.EquipmentTypeID == type.ID select new EquipmentWarrantyVM { ID = c.ID, WarrantyType=c.WarrantyType, Description = c.Description, Deleted = false }).ToList(); 
                    var _detail3 = (from c in db.EquipmentExclusions where c.EquipmentTypeID == type.ID select new EquipmentExclusionVM { ID = c.ID, Description = c.Description, Deleted = false }).ToList();

                    if (_detail1 == null)
                    {
                        _detail1 = new List<EquipmentScopeofworkVM>();
                    }
                    if (_detail2 == null)
                    {
                        _detail2 = new List<EquipmentWarrantyVM>();
                    }
                    if (_detail3 == null)
                    {
                        _detail3 = new List<EquipmentExclusionVM>();
                    }
                    equipmentType.ScopeofWorkDetails = _detail1;
                    equipmentType.WarrantyDetail = _detail2;
                    equipmentType.Exclusions = _detail3;
                }
            }
            return View(equipmentType);
        }


        public ActionResult AddScopeItem(EquipmentScopeofworkVM invoice, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentScopeofworkVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            
            List<EquipmentScopeofworkVM> list = new List<EquipmentScopeofworkVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            EquipmentScopeofworkVM item = new EquipmentScopeofworkVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<EquipmentScopeofworkVM>();
            if (invoice != null)
            {
                item = new EquipmentScopeofworkVM();
                item.Model = invoice.Model;
                item.Description = invoice.Description;
                 
                item.Deleted = invoice.Deleted;
                 
                if (list == null)
                {
                    list = new List<EquipmentScopeofworkVM>();
                }
                if (item.Deleted == false)
                {
                    item.deletedclass = "";
                    list.Add(item);
                }
                else
                {
                    item.deletedclass = "hide";
                    list.Add(item);
                }
            }
            //if (list != null)
            //{
            //    if (list.Count > 0)
            //        list = list.Where(cc => cc.Deleted == false).ToList();
            //}


            vm.ScopeofWorkDetails = list;
            return PartialView("ScopeDetailList", vm);
        }


        public ActionResult AddWarrantyItem(EquipmentWarrantyVM invoice, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentWarrantyVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EquipmentWarrantyVM> list = new List<EquipmentWarrantyVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            EquipmentWarrantyVM item = new EquipmentWarrantyVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<EquipmentWarrantyVM>();
            if (invoice != null)
            {
                item = new EquipmentWarrantyVM();

                item.Description = invoice.Description;
                item.WarrantyType = invoice.WarrantyType;
                item.Deleted = invoice.Deleted;

                if (list == null)
                {
                    list = new List<EquipmentWarrantyVM>();
                }
                if (item.Deleted == false)
                {
                    item.deletedclass = "";
                    list.Add(item);
                }
                else
                {
                    item.deletedclass = "hide";
                    list.Add(item);
                }
            }

            if (list != null)
            {
                if (list.Count > 0)
                    list = list.Where(cc => cc.Deleted == false).ToList();
            }


            vm.WarrantyDetail = list;
            return PartialView("WarrantyDetailList", vm);
        }


        public ActionResult AddExclusionItem(EquipmentExclusionVM invoice, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentExclusionVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EquipmentExclusionVM> list = new List<EquipmentExclusionVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            EquipmentExclusionVM item = new EquipmentExclusionVM();

            if (IDetails.Count > 0 && Details != "[{}]")
                list = IDetails;
            else
                list = new List<EquipmentExclusionVM>();
            if (invoice != null)
            {
                item = new EquipmentExclusionVM();

                item.Description = invoice.Description;

                item.Deleted = invoice.Deleted;

                if (list == null)
                {
                    list = new List<EquipmentExclusionVM>();
                }
                if (item.Deleted == false)
                {
                    item.deletedclass = "";
                    list.Add(item);
                }
                else
                {
                    item.deletedclass = "hide";
                    list.Add(item);
                }
            }
            //if (list != null)
            //{
            //    if (list.Count > 0)
            //        list = list.Where(cc => cc.Deleted == false).ToList();
            //}



            vm.Exclusions = list;
            return PartialView("ExclusionDetailList", vm);
        }


        public JsonResult SaveScopeItem(int EquipmentTypeID, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentScopeofworkVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EquipmentScopeofworkVM> list = new List<EquipmentScopeofworkVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<EquipmentScopeofworkVM>();
                var _details = db.EquipmentScopeofworks.Where(cc => cc.EquipmentTypeID == EquipmentTypeID).ToList();

                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Deleted == false)
                    {
                        EquipmentScopeofwork _obj = db.EquipmentScopeofworks.Find(item.ID);
                        _obj.Description = item.Description;

                        _obj.Model = item.Model;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Deleted == true)
                    {
                        EquipmentScopeofwork _obj = db.EquipmentScopeofworks.Find(item.ID);
                        db.EquipmentScopeofworks.Remove(_obj);
                        db.SaveChanges();
                    }
                    else if (item.Deleted == false)
                    {
                        EquipmentScopeofwork _obj = new EquipmentScopeofwork();
                        _obj.EquipmentTypeID = EquipmentTypeID;
                        _obj.Model = item.Model;
                        _obj.Description = item.Description;
                        db.EquipmentScopeofworks.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Scope of Work Updated Successfully!" });
            }
            catch(Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }
            
        }


        public JsonResult SaveWarrantyItem(int EquipmentTypeID, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentWarrantyVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EquipmentWarrantyVM> list = new List<EquipmentWarrantyVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<EquipmentWarrantyVM>();
                var _details = db.EquipmentWarranties.Where(cc => cc.EquipmentTypeID == EquipmentTypeID).ToList();

                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Deleted == false)
                    {
                        EquipmentWarranty _obj = db.EquipmentWarranties.Find(item.ID);
                        _obj.Description = item.Description;
                        _obj.WarrantyType = item.WarrantyType;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Deleted == true)
                    {
                        EquipmentWarranty _obj = db.EquipmentWarranties.Find(item.ID);
                        db.EquipmentWarranties.Remove(_obj);
                        db.SaveChanges();
                    }
                    else if (item.Deleted == false)
                    {
                        EquipmentWarranty _obj = new EquipmentWarranty();
                        _obj.EquipmentTypeID = EquipmentTypeID;
                        _obj.WarrantyType = item.WarrantyType;
                        _obj.Description = item.Description;
                        db.EquipmentWarranties.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Warranty Items Updated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }

        public JsonResult SaveExclusion(int EquipmentTypeID, string Details)
        {
            EquipmentTypeVM vm = new EquipmentTypeVM();
            var IDetails = JsonConvert.DeserializeObject<List<EquipmentExclusionVM>>(Details);
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            List<EquipmentExclusionVM> list = new List<EquipmentExclusionVM>(); //(List<JobQuotationDetailVM>)Session["JQuotationDetail"];
            try
            {

                if (IDetails.Count > 0 && Details != "[{}]")
                    list = IDetails;
                else
                    list = new List<EquipmentExclusionVM>();
                var _details = db.EquipmentWarranties.Where(cc => cc.EquipmentTypeID == EquipmentTypeID).ToList();

                foreach (var item in list)
                {
                    if (item.ID > 0 && item.Deleted == false)
                    {
                        EquipmentExclusion _obj = db.EquipmentExclusions.Find(item.ID);
                        _obj.Description = item.Description;
                        db.Entry(_obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (item.ID > 0 && item.Deleted == true)
                    {
                        EquipmentExclusion _obj = db.EquipmentExclusions.Find(item.ID);
                        db.EquipmentExclusions.Remove(_obj);
                        db.SaveChanges();
                    }
                    else if (item.Deleted == false)
                    {
                        EquipmentExclusion _obj = new EquipmentExclusion();
                        _obj.EquipmentTypeID = EquipmentTypeID;
                        _obj.Description = item.Description;
                        db.EquipmentExclusions.Add(_obj);
                        db.SaveChanges();
                    }

                }
                return Json(new { status = "OK", message = "Scope of Work Updated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message });
            }

        }

        public ActionResult ShowScopeofWork(int EquipmentTypeID)
        {
            var _detail1 = (from c in db.EquipmentScopeofworks where c.EquipmentTypeID == EquipmentTypeID select new EquipmentScopeofworkVM { ID = c.ID, Model=c.Model, Description = c.Description, Deleted = false ,OrderNo=c.OrderNo}).ToList();
            EquipmentTypeVM vm = new EquipmentTypeVM();
            vm.ScopeofWorkDetails = _detail1;
            return PartialView("ScopeDetailList", vm);
        }

        public ActionResult ShowWarranty(int EquipmentTypeID)
        {
            var _detail1 = (from c in db.EquipmentWarranties where c.EquipmentTypeID == EquipmentTypeID select new EquipmentWarrantyVM { ID = c.ID,WarrantyType=c.WarrantyType, Description = c.Description, Deleted = false }).ToList();
            EquipmentTypeVM vm = new EquipmentTypeVM();
            vm.WarrantyDetail = _detail1;
            return PartialView("WarrantyDetailList", vm);
        }

        public ActionResult ShowExclusion(int EquipmentTypeID)
        {
            var _detail1 = (from c in db.EquipmentExclusions where c.EquipmentTypeID == EquipmentTypeID select new EquipmentExclusionVM { ID = c.ID, Description = c.Description, Deleted = false }).ToList();
            EquipmentTypeVM vm = new EquipmentTypeVM();
            vm.Exclusions = _detail1;
            return PartialView("ExclusionDetailList", vm);
        }
        public ActionResult ShowAddScopeofWork(int ID,int EquipmentTypeID)
        {

           
            EquipmentScopeofworkVM vm = new EquipmentScopeofworkVM();

            if (ID > 0)
            {
                var _detail1 = db.EquipmentScopeofworks.Find(ID);
                vm.ID = _detail1.ID;                
                vm.EquipmentTypeID = Convert.ToInt32(_detail1.EquipmentTypeID);
                vm.Model = _detail1.Model;
                vm.OrderNo = _detail1.OrderNo ?? 0;
                vm.Description = _detail1.Description;
            }
            else
            {
                vm.EquipmentTypeID = EquipmentTypeID;
            }
            return PartialView("AddScopeDetail", vm);
        }
        public ActionResult ShowAddWarranty(int ID,int EquipmentTypeID)
        {

          
            EquipmentWarrantyVM vm = new EquipmentWarrantyVM();

            if (ID > 0)
            {
                var _detail1 = db.EquipmentWarranties.Find(ID);
                vm.ID = _detail1.ID;
                vm.EquipmentTypeID = Convert.ToInt32(_detail1.EquipmentTypeID);
                
                vm.Description = _detail1.Description;
            }
            else
            {
                vm.EquipmentTypeID = EquipmentTypeID;
            }
            return PartialView("AddWarrantyDetail", vm);
        }
        public ActionResult ShowAddExclusion(int ID, int EquipmentTypeID)
        {

          

            EquipmentExclusionVM vm = new EquipmentExclusionVM();

            if (ID > 0)
            {
                var _detail1 = db.EquipmentExclusions.Find(ID);
                vm.ID = _detail1.ID;                
                vm.EquipmentTypeID = Convert.ToInt32(_detail1.EquipmentTypeID);
                vm.Description = _detail1.Description;
            }
            else
            {
                vm.EquipmentTypeID = EquipmentTypeID;
            }

            return PartialView("AddExclusionDetail", vm);

        }
        [HttpPost]
        //[ValidateAntiForgeryToken] // if you use it
        public JsonResult SaveScopeItem1(EquipmentScopeofworkVM request)
        {

            
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
                EquipmentScopeofwork _obj = db.EquipmentScopeofworks.Find(request.ID);
                _obj.Description = request.Description;
                _obj.Model = request.Model;
                //_obj.Airmech = request.Airmech;
                //_obj.client = request.client;
                _obj.OrderNo = request.OrderNo;
                //_obj.EquipmentTypeID = request.EquipmentTypeID;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                EquipmentScopeofwork _obj = new EquipmentScopeofwork();
                _obj.Description = request.Description;
                _obj.Model = request.Model;
                _obj.EquipmentTypeID= request.EquipmentTypeID;
                
                _obj.OrderNo = request.OrderNo;
                //_obj.Airmech = request.Airmech;
                //_obj.client = request.client;
                db.EquipmentScopeofworks.Add(_obj);
                db.SaveChanges();
            }
            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Scope of Work Updated Successfully!" });

        }

        public JsonResult SaveWarrantyItem1(EquipmentWarrantyVM request)
        {

            
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
                EquipmentWarranty _obj = db.EquipmentWarranties.Find(request.ID);
                _obj.Description = request.Description;
                _obj.WarrantyType = request.WarrantyType;
                //_obj.OrderNo = request.OrderNo;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                EquipmentWarranty _obj = new EquipmentWarranty();
                _obj.Description = request.Description;
                _obj.WarrantyType = request.WarrantyType;
                //  _obj.Model = request.Model;
                _obj.EquipmentTypeID = request.EquipmentTypeID;
                
                
                db.EquipmentWarranties.Add(_obj);
                db.SaveChanges();
            }

            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Warranty Data Updated Successfully!" });

        }

        public JsonResult SaveExclusionItem1(EquipmentExclusionVM request)
        {

             
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
                EquipmentExclusion _obj = db.EquipmentExclusions.Find(request.ID);
                _obj.Description = request.Description;
                //_obj.WarrantyType = request.WarrantyType;
                //_obj.OrderNo = request.OrderNo;
                db.Entry(_obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                EquipmentExclusion _obj = new EquipmentExclusion();
                _obj.Description = request.Description;
                //_obj.WarrantyType = request.WarrantyType;
                //  _obj.Model = request.Model;
                _obj.EquipmentTypeID = request.EquipmentTypeID;
                //_obj.QuotationID = request.QuotationID;
                //_obj.OrderNo = request.OrderNo;
                db.EquipmentExclusions.Add(_obj);
                db.SaveChanges();
            }

            // ... your existing save logic using list and QuotationID ...
            return Json(new { status = "OK", message = "Exclusion Data Updated Successfully!" });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteScopeEntry(int id)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int JobID = 0;
            EquipmentScopeofwork obj = db.EquipmentScopeofworks.Find(id);

            try
            {
                if (obj != null)
                {
                    db.EquipmentScopeofworks.Remove(obj);
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
            EquipmentWarranty obj = db.EquipmentWarranties.Find(id);

            try
            {
                if (obj != null)
                {
                    db.EquipmentWarranties.Remove(obj);
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Warranty Item Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
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
            EquipmentExclusion obj = db.EquipmentExclusions.Find(id);

            try
            {
                if (obj != null)
                {
                    db.EquipmentExclusions.Remove(obj);
                    db.SaveChanges();
                }
                return Json(new { status = "OK", message = "Exlcusion Item Deleted Succesfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}