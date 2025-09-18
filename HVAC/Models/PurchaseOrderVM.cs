using HVAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class PurchaseOrderVM:PurchaseOrder
    {
         
        public string EnquiryNo { get; set; }
        public string PortoFImport { get; set; }
        public Nullable<int> CountryOfOrigin { get; set; }
        public Nullable<int> Currency { get; set; }
        public string Delivery { get; set; }
        public string SONumber { get; set; }
        public string SupplierName { get; set; }
        

        public string MovementId { get; set; }
        public int[] SelectedValues { get; set; }
        public List<PurchaseOrderDetailVM> Details { get; set; }
        //public List<PurchaseOrderOtherCharge> othercharges { get; set; }
        public List<PurchaseOrderUserComment> comment { get; set; }
        public List<PurchaseOrderOtherDetail> orderdetails { get; set; }
        public List<POApproverVM> ApproveDetails { get; set; }
    }

    public class PurchaseOrderDetailVM :PurchaseOrderDetail
    {
         
        
        public string EstimationNo { get; set; }
                
       public bool Checked { get; set; }
        public string Remarks { get; set; }
        public bool Deleted { get; set; }
        public string UnitName { get; set; }
        
    }



    public class PurchaseOrderSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int SupplierTypeId { get; set; }
      
        public int[] SelectedValues { get; set; }
      
        public string PurchaseOrderNo { get; set; }
        public List<PurchaseOrderVM> Details { get; set; }
    }

    

    public class PurchaseOrderSaveRequest
    {
        public PurchaseOrder po { get; set; }
        public decimal SubTotal { get; set; } //total amount + freight + origin
        public decimal SubTotal1 { get; set; } //total amount + freight + origin + finance
        public int EmployeeID { get; set; }
        public List<PurchaseOrderDetailVM> Details { get; set; }
        //public List<PurchaseOrderEquipmentVM> equipment { get; set; }
        //public List<PurchaseOrderOtherCharge> othercharges { get; set; }
        public List<PurchaseOrderUserComment> comment { get; set; }
        public List<PurchaseOrderNote> Notes { get; set; }
        public List<PurchaseOrderOtherDetail> orderdetails { get; set; }
        public List<PORegrigerant> refrigerants { get; set; }
        public PurchaseOrderTextVM masterDropdowns { get; set; }
        public List<POApproverVM> ApproveDetails { get; set; }


    }
    
    public class PurchaseOrderTextVM 
    {
        public string SupplierText { get; set; }
        public string PaymentTermsText { get; set; }
        public string INCOTermsText { get; set; }
        public string DeliveryTermsText { get; set; }
        public string BankText { get; set; }
        public string RegrigerantText { get; set; }
        public string CompressorWarrantyText { get; set; }
        public string UnitWarrantyText { get; set; }
    }

    public class POApproverVM: PurchaseOrderApproval
    {
        public string EmployeeName { get; set; }
        public string ValidateText { get; set; }
    }



    #region  "reportprint"
    public class PurchaseOrderViewModel
    {
        public string OrderNo { get; set; }
        public int Revision { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string SONo { get; set; }
        public string ProductFamily { get; set; }
        public string Product { get; set; }
        public DateTime OrderDate { get; set; }
        public string PortOfImport { get; set; }
        public string PaymentTerms { get; set; }
        public string CountryOfOrigin { get; set; }
        public string IncoTerms { get; set; }
        public string DeliveryWeeks { get; set; }
        public string Currency { get; set; }

        public List<PurchaseOrderItem> Items { get; set; }
        public List<string> ImportantNotes { get; set; }
        public decimal TotalValue { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal OriginCharges { get; set; }
        public decimal SubTotal { get; set; }
        public decimal FinanceCharges { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TotalPOValue { get; set; }

        public List<ProjectBreakupItem> ProjectBreakup { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string SwiftCode { get; set; }

        public string ChangeOrderDetails { get; set; }
        public decimal PreviousPOValue { get; set; }

        public SupplierVM Supplier { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierContactPerson { get; set; }
    }
    public class SupplierVM:SupplierMaster
    {

    }
    public class PurchaseOrderItem
    {
        public int SqNo { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string JobNo { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProjectBreakupItem
    {
        public string ProjectCode { get; set; }
        public decimal Amount { get; set; }
    }

    #endregion
}
