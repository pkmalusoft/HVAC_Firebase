using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class StockOpeningVM : StockOpening
    {
        public string EquipmentType { get; set; }
        public string ProductFamilyName { get; set; }
        public string BrandName { get; set; }
        public string ItemUnit { get; set; }
    }

    public class StockReportParam
    {
        public int BrandID { get; set; }
        public int EquipmentTypeID { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string EquipmentType { get; set; }
        public DateTime AsonDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Output { get; set; } //printer ,pdf,word,excel
        public string ReportType { get; set; } //sumary details
        public string ReportFileName { get; set; }
        public string Filters { get; set; }
        public string CustomerType { get; set; }
        public bool ForeignCurrency { get; set; }
        public DataTable data { get; set; }

    }
}