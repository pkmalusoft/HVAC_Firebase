using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.Data;
using HVAC.DAL;
using System.Data.Entity;
 

namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ClientMasterController : Controller
    {
        HVACEntities db = new HVACEntities();

        public ActionResult Index()
        {

            ClientMasterSearch obj = (ClientMasterSearch)Session["ClientMasterSearch"];
            ClientMasterSearch model = new ClientMasterSearch();
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
                
                obj = new ClientMasterSearch();
                obj.ClientType = "All";
                Session["ClientMasterSearch"] = obj;
                
            }
            else
            {
                model = obj;                
            }

            List<ClientMasterVM> lst = EnquiryDAO.ClientList(model.ClientType);
            model.Details = lst;

            return View(model);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ClientMasterSearch obj)
        {
            Session["ClientMasterSearch"] = obj;
            return RedirectToAction("Index");
        }
        //public ActionResult Index()
        //{

        //    List<ClientMasterVM> lst = (from c in db.ClientMasters orderby c.ClientName
        //    select new ClientMasterVM { ClientID=c.ClientID,ClientName = c.ClientName,CountryName=c.CountryID, Email = c.Email, ContactNo = c.ContactNo,ClientType=c.ClientType }).ToList();
        //    return View(lst);
        //}

        public ActionResult Create(int id = 0)
        {

            ViewBag.City = db.CityMasters.ToList();
            ClientMasterVM _client = new ClientMasterVM();
            if (id == 0)
            {
                ViewBag.Title = "Create";
                _client.ClientID = 0;
            }
            else
            {
                ViewBag.Title = "Modify";
                ClientMaster model = db.ClientMasters.Find(id);

                _client.ClientID = model.ClientID;
                _client.ClientName = model.ClientName;
                    _client.Address1 = model.Address1;
                    _client.Address2 = model.Address2;
                    _client.Address3 = model.Address3;
                    _client.LocationName = model.LocationName;
                    _client.CountryID = model.CountryID;
                    _client.CityID = model.CityID;
                    if (_client.CountryID > 0)
                    {
                    var Country = db.CountryMasters.Find(_client.CountryID);
                       if (Country!=null)                        
                        _client.CountryName = Country.CountryName;
                    }
                    if (_client.CityID > 0)
                    {
                    var cityname = db.CityMasters.Find(_client.CityID);
                    if (cityname!=null)                         
                        _client.CityName = cityname.City;
                    }

                    _client.ContactNo = model.ContactNo;
                    _client.Email = model.Email;
                    _client.ContactName = model.ContactName;
                    _client.ClientType = model.ClientType;
                    _client.ClientPrefix = model.ClientPrefix;
                    _client.VATNo = model.VATNo;
                }
         
            return View(_client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientMasterVM v)
        {
           
            ClientMaster clientobj = new ClientMaster();           
            
            if (v.ClientID>0)
            {
                clientobj = db.ClientMasters.Find(clientobj.ClientID);
               
            }
            clientobj.ContactName = v.ContactName;
            clientobj.Address1 = v.Address1;
            clientobj.Address2 = v.Address2;
            clientobj.Address3 = v.Address3;
            clientobj.LocationName = v.LocationName;
            clientobj.CountryID = v.CountryID;
            clientobj.CityID = v.CityID;            
            clientobj.Email = v.Email;
            clientobj.ContactNo = v.ContactNo;
            clientobj.ClientType = v.ClientType;
            db.ClientMasters.Add(clientobj);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully added Client.";
            return RedirectToAction("Index");


        }

        public JsonResult DeleteConfirmed(int id)
        {
            ClientMaster a = (from c in db.ClientMasters where c.ClientID == id select c).FirstOrDefault();
            if (a == null)
            {
                return Json(new { status = "Failed", message = "Client Not Found!" });
            }
            else
            {
                try
                {
                    if (a != null)
                    {
                        db.ClientMasters.Remove(a);
                        db.SaveChanges();
                    }
           
                    db.SaveChanges();
                    TempData["SuccessMsg"] = "You have successfully Deleted Client.";
                    return Json(new { status = "OK", message = "Client  Deleted Successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = "Failed", message = ex.Message });
                }
            }

        }

        [HttpGet]
        public JsonResult GetClientName()
        {
            var employeelist = (from c1 in db.ClientMasters  select c1.ClientName).ToList();

            return Json(new { data = employeelist }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveClient(ClientMasterVM v)
        {

            ClientMaster _clientobj = new ClientMaster();
            if (v.ClientID == 0)
            {
                _clientobj = new ClientMaster();
                var _checkdupliate = db.ClientMasters.Where(cc => cc.ClientName == v.ClientName).FirstOrDefault();
                if (_checkdupliate!=null)
                    return Json(new { status = "Failed", ClientID = v.ClientID, message = "Client Name already Exist!" });
            }
            else
            {
                var _checkdupliate = db.ClientMasters.Where(cc => cc.ClientName == v.ClientName && cc.ClientID!=v.ClientID).FirstOrDefault();
                if (_checkdupliate != null)
                    return Json(new { status = "Failed", ClientID = v.ClientID, message = "Client Name already Exist!" });

                _clientobj = db.ClientMasters.Find(v.ClientID);
            }

            _clientobj.ClientName = v.ClientName;
            _clientobj.ClientPrefix = v.ClientPrefix;
            _clientobj.ContactName = v.ContactName;
            _clientobj.Address1 = v.Address1;
            _clientobj.Address2 = v.Address2;
            _clientobj.Address3 = v.Address3;
            _clientobj.LocationName = v.LocationName;
            _clientobj.CountryID = v.CountryID;
            _clientobj.CityID = v.CityID;
            _clientobj.Email = v.Email;
            _clientobj.ContactNo = v.ContactNo;
            _clientobj.ClientType = v.ClientType;
            _clientobj.VATNo = v.VATNo;            

            if (v.ClientID == 0)
            {
                db.ClientMasters.Add(_clientobj);
                db.SaveChanges();
            }
            else
            {
                db.Entry(_clientobj).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { status = "OK", ClientID = v.ClientID, message = "Client Updated Succesfully!" });
        }

    }

}
