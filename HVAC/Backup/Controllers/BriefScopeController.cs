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
    public class BriefScopeController : Controller
    {
        HVACEntities db = new HVACEntities();
        // GET: ProjectType
        public ActionResult Index(string SearchText = "")
        {
            List<Models.BriefScope> Protypes = db.BriefScopes.Where(temp => temp.BriefScope1.Contains(SearchText)).ToList();
            return View(Protypes);

        }


        public ActionResult Create(int id = 0)
        {


            Models.BriefScope obj = new Models.BriefScope();
            if (id == 0)
            {
                //ViewBag.Title = "EntityType - CREATE";
                obj.BriefScopeID = 0;


            }
            else
            {
             //   ViewBag.Title = "BriefScope - Modify";
                Models.BriefScope type = (from c in db.BriefScopes where c.BriefScopeID == id select c).FirstOrDefault();
                if (type == null)
                {
                    return HttpNotFound();
                }
                else
                {

                    obj.BriefScopeID = type.BriefScopeID;
                    obj.BriefScope1 = type.BriefScope1;
                 

                }
            }
            return View(obj);
        }

        public JsonResult BriefScopeExist(string PType, int ID)
        {
            string status = "true";

            if (ID == 0)
            {
                var existtype = (from b in db.BriefScopes where b.BriefScope1 == PType select b).FirstOrDefault();
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
                var existtypebyid = (from b in db.BriefScopes where b.BriefScope1 == PType && b.BriefScopeID != ID select b).FirstOrDefault();
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
        public JsonResult SaveBriefScope(Models.BriefScope ProType)
        {

            Models.BriefScope Ptype = new Models.BriefScope();

            if (ProType.BriefScopeID == 0)
            {

                int max = (from c in db.BriefScopes orderby c.BriefScopeID descending select c.BriefScopeID).FirstOrDefault();

                if (max == null)
                {
                    Ptype.BriefScopeID = 1;
                    Ptype.BriefScope1 = ProType.BriefScope1;
                  


                }
                else
                {
                    Ptype.BriefScopeID = max + 1;
                    Ptype.BriefScope1 = ProType.BriefScope1;
                   


                }
                db.BriefScopes.Add(Ptype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "BriefScope Addded Succesfully!" });
            }
            else
            {
                var item = db.BriefScopes.Where(cc => cc.BriefScopeID == ProType.BriefScopeID && cc.BriefScopeID != ProType.BriefScopeID).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { status = "Failed", message = "BriefScope already Exist!" });

                }

                Ptype = db.BriefScopes.Find(ProType.BriefScopeID);
                Ptype.BriefScope1 = ProType.BriefScope1;
                db.Entry(Ptype).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "OK", message = "BriefScope Updated Succesfully!" });
            }







        }

        public JsonResult DeleteBriefScope(int id)
        {

            Models.BriefScope protype = db.BriefScopes.Find(id);
            if (protype == null)
            {
                return Json(new { status = "Failed", message = "Contact Admin!" });

            }
            else
            {
                var protypes = db.BriefScopes.Where(cc => cc.BriefScopeID == id).ToList();
                db.BriefScopes.Remove(protype);
                db.SaveChanges();
                return Json(new { status = "OK", message = "BriefScope Deleted Successfully!" });

            }

        }

    }
}