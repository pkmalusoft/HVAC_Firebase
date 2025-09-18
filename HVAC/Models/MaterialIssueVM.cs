using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class MaterialIssueVM : MaterialIssue
    {
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }
        public string EmployeeName { get; set; }
        public string RequestedByName { get; set; }
        public string IssuedByName { get; set; }
        public decimal Amount { get; set; }
        public int ItemTypeId { get; set; }
        public int ItemTypeId1 { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string MovementId { get; set; }
        public string IssueNo { get; set; }
        public string ProjectNo{ get; set; }
        public int[] SelectedValues { get; set; }
        public List<MaterialIssueDetailVM> Details { get; set; }
    }

    

    public class MaterialIssueSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string IssueNo { get; set; }
        public List<MaterialIssueVM> Details { get; set; }
    }

    public class MaterialIssueRequest
    {
        public MaterialIssue mi { get; set; }
        public string ApprovedByName { get; set; }
        public string IssuedByName { get; set; }
        public string RequestedByName { get; set; }
        public List<MaterialIssueDetail> miDetails { get; set; }
    }

    public class MaterialIssueSaveRequest
    {
        public MaterialIssue mi { get; set; }
        public string IssuedByname { get; set; }
        public string RequestedByName { get; set; }
        public string ApprovedByName { get; set; }

        public string RequestNo { get;set; }
        public string ProjectNo { get; set; }
        public List<MaterialIssueDetailVM> miDetails { get; set; }
    }

   public class MaterialIssueDetailVM : MaterialIssueDetail
    {
        public string EquipmentType { get; set; }
        public string UnitName { get; set; }
        public bool Checked { get; set; }
    }
}
