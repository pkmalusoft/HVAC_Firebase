using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.DAL;
using HVAC.Models;

namespace HVAC.Controllers
{
    public class DocumentTypeController : Controller
    {
         HVACEntities db = new HVACEntities();

      
        public ActionResult Index()
        {
            List<DocumentType> lst = new List<DocumentType>();
            var data = db.DocumentTypes.OrderBy(cc=>cc.DocumentTypeName).ToList();

            return View(data);
            
        }

      
        //
        // GET: /TypeOfGood/Create

        public ActionResult Create(int id=0)
        {
            DocumentType model = new DocumentType();
            if (id==0)
            {
                return View(model);
            }
            else
            {
                model=db.DocumentTypes.Find(id);
                return View(model);
            }
            
        }

        //
        // POST: /TypeOfGood/Create

        [HttpPost]
        public ActionResult Create(DocumentType v)
        {

            DocumentType t = new DocumentType();

            
            if (v.DocumentTypeID ==0)
            {
                db.DocumentTypes.Add(v);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Document Type";
                return RedirectToAction("Index");
            }
            else
            {
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Document Type";
                return RedirectToAction("Index");
            }
                 
        }
 
        
    
  
        public ActionResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = DocumentDAO.DeleteDocumentType(id);
                
              
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
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
                        //TempData["SuccessMsg"] = "You have successfully Deleted Cost !!";
                        return Json(new { status = "Failed", message = "Delete Failed!" });
                    }
                }
                else
                {
                    //TempData["SuccessMsg"] = "You have successfully Deleted Cost !!";
                    return Json(new { status = "Failed", message = "Delete Failed!" });
                }
            }

            return Json(new { status = "Failed", message = "Delete Failed!" });
          
          
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}