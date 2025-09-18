using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.DAL;
using HVAC.Models;

namespace HVAC.Controllers
{
    public class NotificationController : Controller
    {
        private  NotificationDAO _notificationDAO;

        public NotificationController()
        {
            
            _notificationDAO = new NotificationDAO();
        }
        // GET: Notification

        public ActionResult Index()
        {
            return View();
        }

        // GET: Notification/Unread
        public ActionResult Unread()
        {
            int userid = Convert.ToInt32(Session["UserID"].ToString());
           

          
            List<NotificationModel> unreadList = _notificationDAO.GetUnreadNotifications(userid);
            return View(unreadList);
        }

        
        public JsonResult GetUnreadCount()
        {
            if (Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(Session["UserID"].ToString());
                List<NotificationModel> unreadList = _notificationDAO.GetUnreadNotifications(userid);
                int count = unreadList.Count;
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int count = 0;
                return Json(count, JsonRequestBehavior.AllowGet);
            }

            

        }
        public ActionResult MarkAsRead(int id)
        {
            _notificationDAO.MarkNotificationAsRead(id); // We can implement this DAO next
            return RedirectToAction("Unread");
        }

        public ActionResult TopUnreadPartial()
        {
            int loggedInUserId = Convert.ToInt32(Session["UserID"].ToString());

            var topUnread = _notificationDAO.GetTopUnreadNotifications(loggedInUserId, 5);

            return PartialView("_TopUnreadNotifications", topUnread);
        }

       

    }
}