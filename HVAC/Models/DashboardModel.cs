using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Models
{
    public class DashboardViewModel
    {
        // Summary Widgets
        public int TotalEnquiry { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalJobvalue { get; set; }
        public decimal TotalMargin { get; set; }
        public decimal TotalJobCost { get; set; }
        public int NewMessages { get; set; }

        // Sales / Revenue over time (e.g., for chart)
        public List<ChartData> RevenueChartData { get; set; }

        // Recent Orders or Activities
        public List<EnquiryVM> RecentOrders { get; set; }

        // Notifications
        public List<Notification> Notifications { get; set; }

        // Tasks or Progress tracking
        public List<TaskItem> TaskList { get; set; }
        public string RevenueSeriesA { get; set; }
        public string RevenueSeriesB { get; set; }
        public string RevenueSeriesC { get; set; }
        public string RevenueSeriesD { get; set; }
        public string CountSeriesA { get; set; }
        public string CountSeriesB { get; set; }
        public string CountSeriesC { get; set; }
        public string CountSeriesD { get; set; }
        public FinancialsChartViewModel FinancialsChartModel { get; set; }

        public DashboardViewModel()
        {
            RevenueChartData = new List<ChartData>();
            RecentOrders = new List<EnquiryVM>();
            Notifications = new List<Notification>();
            TaskList = new List<TaskItem>();
            FinancialsChartModel = new FinancialsChartViewModel();
        }
    }
    public class ChartData
    {
        public string Label { get; set; }         // e.g., "Jan", "Feb"
        public decimal Value { get; set; }        // e.g., 15000.50
    }
    public class FinancialsChartViewModel
    {
        public List<SelectListItem> Months { get; set; }
        public List<SelectListItem> Years { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public string SelectedMonthName =>
            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonth);
    }
    public class RecentOrder
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }        // e.g., "Pending", "Completed"
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
    }

    //public class Notification
    //{
    //    public string Title { get; set; }
    //    public string Message { get; set; }
    //    public DateTime Time { get; set; }
    //    public bool IsRead { get; set; }
    //}

    public class TaskItem
    {
        public string Description { get; set; }
        public int ProgressPercent { get; set; }  // 0 to 100
        public bool IsCompleted { get; set; }
    }
}