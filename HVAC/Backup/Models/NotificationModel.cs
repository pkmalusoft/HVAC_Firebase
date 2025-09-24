using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }
        public long? TransactionId { get; set; }
        public string NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Priority { get; set; }
        public string DeliveryChannel { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}