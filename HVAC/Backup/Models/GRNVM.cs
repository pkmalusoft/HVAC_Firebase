using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class GRNVM : GRN
    {
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }

        public decimal Amount { get; set; }
        public int ItemTypeId { get; set; }
        public int ItemTypeId1 { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string MovementId { get; set; }
        public int[] SelectedValues { get; set; }

        public List<GRNDetailVM> Details { get; set; }
        //public List<GRNMaster> GrnMaster { get; set; }
    }
    //public class GRNDetailVM
    //{
    //    public int DetailID { get; set; }
    //    public Nullable<int> GRNID { get; set; }
    //    public int EquipmentID { get; set; }
    //    public string Unit { get; set; }
    //    public int Qty { get; set; }
    //    public decimal Rate { get; set; }
    //    public decimal Tax { get; set; }
    //    public Nullable<int> ProjectID { get; set; }
    //    public Nullable<decimal> Amount { get; set; }
    //    public Nullable<decimal> Value { get; set; }
    //    public string ProjectNo { get; set; }
    //    public string GRNNumber { get; set; }
    //    public System.DateTime GRNDate { get; set; }

    //    public virtual GRN GRN { get; set; }
    //}
    public class GRNSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int SupplierTypeId { get; set; }

        public int[] SelectedValues { get; set; }

        public string GRNNO { get; set; }
        public List<GRNVM> Details { get; set; }
    }

    public class GRNSaveRequest
    {
        public GRN go { get; set; }
        public List<GRNDetailVM> gRNDetails { get; set; }
        public GRMTextVM masterDropdowns { get; set; }

    }
    public class GRNDetailVM :GRNDetail
    {
        public bool Checked { get; set; }
        public string UnitName { get; set; }
        public int OrderQty { get; set; }

        public int ReceivedQty { get; set; }
    }

    public class GRMTextVM
    {
        public string SupplierText { get; set; }
        public string PurchaseOrderText { get; set; }
        public string EmployeeText { get; set; }

    }




}
