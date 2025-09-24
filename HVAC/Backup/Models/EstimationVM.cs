using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class EstimationVM : Estimation
    {
        public string EnquiryNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public int DefaultCurrencyID { get; set; }
        public decimal EquipmentAccessTotal { get; set; }
        public string CurrencyCode { get; set; }
        public List<QuotationVM> QuotationDetails { get; set; }
        public List<EstimationDetailVM> Details { get; set; }
        

    }

    public class EstimationDetailVM : EstimationDetail
    {
        public string CategoryName { get; set; }
        public string UnitName { get; set; }
        public string CurrencyCode { get; set; }
        public bool Deleted { get; set; }
        public bool AutoCalc { get; set; }
        public string displayclass { get; set; }
        public string RowType { get; set; }
        public decimal Roworder { get; set; }
        public bool Checked {get;set;}
        public int EstimationDetailID { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string EstimationNo { get; set; }
        public string ProjectNo { get; set; }

        public int JobHandOverID { get; set; }
    }

    public class EstimationSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public string EstimationNo { get; set; }
        public List<EstimationVM> Details { get; set; }
    }

    public class EstimationMasterVM :EstimationMaster
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public string CurrencyCode { get; set; }
        
    }
}