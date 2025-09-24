using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class MaterialRequestVM : MaterialRequest
    {
        public string EmployeeName { get; set; }
        public List<MaterialRequestDetailVM> Details { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string StoreKeeperName { get; set; }
        public string RequestedByName { get; set; }
        public string PONO{ get; set; }
        // String property to handle form input for MRDate
        public string MRDateString { get; set; }
        
        // Additional properties for form fields
        public decimal? POValue { get; set; }
    }

    public class MaterialRequestDetailVM : MaterialRequestDetail
    {
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string EquipmentType { get; set; }
        public string UnitName { get; set; }
        public bool Checked{ get; set; }
        public string MRNo { get; set; }
        public DateTime MRDate { get; set; }
        public string RequestedByName { get; set; }
        public string StoreKeeperName { get; set; }
        public string IssueNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public decimal Stock { get; set; }
        public string ApprovedLog { get; set; }
        // The following are already in base: Model, Description, EstimationID, QuotationID
    }

    public class MaterialRequestSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string MRNo { get; set; }
        public string Status { get; set; }
        public List<MaterialRequestVM> Details { get; set; }
        public List<MaterialRequestDetailVM> EquipmentDetails { get; set; }
    }
}