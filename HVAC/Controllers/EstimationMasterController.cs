using Microsoft.Office.Interop.Excel;
using HVAC.DAL;
using HVAC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class EstimationMasterController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.EstimationMasterVM> Protypes = (from c in db.EstimationMasters
                                                        join d in db.EstimationCategories on c.CategoryID equals d.ID
                                                        join u in db.ItemUnits on c.UnitID equals u.ItemUnitID
                                                        join cu in db.CurrencyMasters on c.CurrencyID equals cu.CurrencyID                                                        
                                                        orderby d.CategoryName
                                                        select new EstimationMasterVM
                                                        {
                                                            ID = c.ID,
                                                            CategoryID = c.CategoryID,
                                                            CategoryName = d.CategoryName,
                                                            Unit = u.ItemUnit1,
                                                            Qty = c.Qty,
                                                            TypeName =c.TypeName,
                                                            CurrencyCode = cu.CurrencyCode
                                                        }).ToList();             
           
            return View(Protypes);
        }


        public ActionResult Create(int id = 0)
        {

            ViewBag.Currency = db.CurrencyMasters.ToList();
             
            Models.EstimationMaster obj = new Models.EstimationMaster();
            ViewBag.EstimationCategory = db.EstimationCategories.ToList();
            ViewBag.Unit= db.ItemUnits.ToList();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.ID = 0;

                ViewBag.Title = "Create";
            }
            else
            {
                ViewBag.Title = "Modify";
                Models.EstimationMaster type = (from c in db.EstimationMasters where c.ID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj = type;
                 

                }
            }
            return View(obj);
        }

    

        [HttpPost]
        public JsonResult SaveEstimationMaster(Models.EstimationMaster ProType)
        {

            Models.EstimationMaster Ptype = new Models.EstimationMaster();

            if (ProType.ID == 0)
            {

                var duplicate = (from c in db.EstimationMasters where c.TypeName==ProType.TypeName
                                 select c).FirstOrDefault();

                if (duplicate !=null)
                {
                    
                    return Json(new { status = "Failed", message = "Duplicate Type Not allowed!" });

                }
                Ptype.TypeName = ProType.TypeName;
                Ptype.CategoryID = ProType.CategoryID;
                Ptype.CurrencyID = ProType.CurrencyID;
                Ptype.UnitID = ProType.UnitID;
                Ptype.Remarks = ProType.Remarks;
                Ptype.OrderNo = ProType.OrderNo;
                db.EstimationMasters.Add(Ptype);
                db.SaveChanges();

                return Json(new { status = "OK", message = "Enquiry Stage Added Successfully!" });
            }
            else
            {
                var duplicate = (from c in db.EstimationMasters where c.TypeName == ProType.TypeName && 
                                    c.ID!= ProType.ID select c).FirstOrDefault();
                 
                if (duplicate != null)
                {
                    return Json(new { status = "Failed", message = "Estimation Type already Exist!" });

                }

                Ptype = db.EstimationMasters.Find(ProType.ID);
                Ptype.CategoryID = ProType.CategoryID;
                Ptype.TypeName = ProType.TypeName;
                Ptype.Qty = ProType.Qty;
                Ptype.UnitID = ProType.UnitID;
                Ptype.CurrencyID = ProType.CurrencyID;
                Ptype.Remarks = ProType.Remarks;
                Ptype.OrderNo = ProType.OrderNo;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "Estimation Type Updated Successfully!" });
            }







        }

        public JsonResult DeleteProjecType(int id)
        {

            Models.EnquiryStage protype = db.EnquiryStages.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var _enquiry = db.Enquiries.Where(cc => cc.EnquiryStageID == id).FirstOrDefault();
                if (_enquiry!=null)
                {
                    return Json(new { status = "Failed", message = "Enquiry Stage referenced in the Enquiry,Could not Delete!" });
                }
                var protypes = db.EnquiryStages.Where(cc => cc.EnqStageID == id).ToList();
                db.EnquiryStages.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "Enquiry Stage Deleted Successfully!" });

            }

        }

    }
}