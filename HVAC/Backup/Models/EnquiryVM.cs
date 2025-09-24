using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Models
{
    public class EnquirySearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public List<EnquiryVM> Details { get; set; }
    }

    public class EnquiryVM :Enquiry
    {

        public string EnqStageName { get; set; }
        public string EnquiryStatus { get; set; }
        public string EnquiryStatushtml { get; set; }
        public string PriorityName { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string ClientName { get; set; }
        public int ClientID { get; set; }
        public string ProjectNumber { get; set; }
        public string Status { get; set; }
        public int SecuredQuoteCount { get; set; }
        public string AssignedEmps { get; set; }
        public List<EnquiryClientVM> ClientDetails { get; set; }
        public List<EnquiryEmployeeVM> EmployeeDetails { get; set; }
        public List<EnquiryEquipmentVM> EquipmentDetails { get; set; }
        public List<QuotationVM> QuotationDetails { get; set; }
        public List<AuditLogVM> AuditLogDetails { get; set; }
        public List<DocumentMasterVM> DocumentDetails { get; set; }

    }
    public class EnquiryEmployeeVM : EnquiryEmployee
    {
        public string EmployeeName { get; set; }
        public string EnquiryNo { get; set; }
        public string ImageURL { get; set; }
        public string EMPShortName { get; set; }
        public string DateString { get; set; }
    }
    public class ClientVM : ClientMaster
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public int EnquiryID { get; set; }
        public string EnquiryNo { get; set; }
    }
    public class DropdownVM
    {

        public int ID { get; set; }
        public string Text { get; set; }
    }

    public class EnquiryClientVM : EnquiryClient
    {
        
        public string ClientName { get; set; }
        public string EnquiryNo { get; set; }
        public bool Deleted { get; set; }
    }
    public class AuditLogVM : AuditLog
    {
        public string EmployeePrefix { get; set; }
        public string UserName { get; set; }
    }
    public class EnquiryEquipmentVM : Equipment
    {
        public string EquipmentType { get; set; }
        public string CategoryName { get; set; }
        public string ProductFamilyName { get; set; }
        public string EmployeeName { get; set; }
        public string Unit { get; set; }
        public string EquipmentStatus { get; set; }
        public bool Deleted { get; set; }

        public string EnquiryNo { get; set; }

    }

    public class EquipmentTagVM      
    {
        public int sno { get; set; }
        public int tagname { get; set; }
    }
    public class EnquiryPrintVM
    {
        // Company Info
        public string CompanyLogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddressLine1 { get; set; }
        public string CompanyAddressLine2 { get; set; }

        // Generated Info
        public string GeneratedBy { get; set; }

        // Enquiry Info
        public Enquiry Enquiry { get; set; }
        public List<EnquiryEquipmentVM> EnquiryEquipmentVMs { get; set; }
        public string EnquiryNo { get; set; }
        public DateTime? EnquiryDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DueDays { get; set; }
        public string EnquiryStage { get; set; }
        public string Prefix { get; set; }

        // Project Info
        public string ProjectName { get; set; }
        public string ProjectDetails { get; set; }
        public List<string> EntityTypes { get; set; }
        public string ProjectLocation { get; set; }
        public List<string> AssignedToNames { get; set; }

        // Items
        public List<EnquiryItemVM> Items { get; set; }
        public decimal? EstimatedTotal { get; set; }
        public string ItemNotes { get; set; }

        // Notes
        public string CommercialNotes { get; set; }
        public string TechnicalNotes { get; set; }

        // Attachments
        public List<AttachmentVM> Attachments { get; set; }

        // Approvals
        public string PreparedByName { get; set; }
        public DateTime? PreparedOn { get; set; }
        public string ReviewedByName { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedOn { get; set; }
    }

    public class EnquiryItemVM
    {
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string Unit { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class AttachmentVM
    {
        public string FileName { get; set; }
        public string Remarks { get; set; }
    }
    public class QuotationVM : Quotation
    {
        public string ReportClass { get; set; }
        public int Id { get; set; }
        public string EnquiryNo { get; set; }
        
        public DateTime? EnquiryDate { get; set; }

        public string TermsofShipments { get; set; }

        public string ProjectName { get; set; }
        public string EmployeeName { get; set; }
        public string QuotationStatus { get; set; }
        public string CurrencyName { get; set; }
        public int SelectedQuotaionId { get; set; }
        public string NewVersion { get; set; }

        public string CityName { get; set; }
        public string CountryName { get; set; }

        public string CurrentPage { get; set; }
        public bool OrderConfirmed { get; set; }
        public string ContainerType { get; set; }
        public int PurchaseOrderDetailId { get; set; }
        public string PONo { get; set; }
        public string ProjectNo { get; set; }
        public int JobHandOverID { get; set; }
        public decimal GrossAmount { get; set; }
        public string QuoteClientLocation { get; set; }
        public string QuoteClientAddress { get; set; }
        public string QuoteClientEmailId { get; set; }
        public string QuoteClientDetail { get; set; }
        public string ClientPONO { get; set; }
        public string QuotationValueInWords { get; set; }
        public List<QuotationDetailVM> QuotationDetails { get; set; }
        public List<QuotationContactVM> QuotationContacts { get; set; }
        public List<QuotationTermsVM> QuotationTerms { get; set; }
        public List<QuotationScopeofWorkVM> QuotationScopeDetails { get; set; }
        public List<ScopeOfWorkGroupVM> QuotationScopeofWork { get; set; }
        
        public List<QuotationWarrantyVM> QuotationWarrantyDetails { get; set; }
        public List<QuotationExclusionVM> QuotationExclusionDetails { get; set; }

    }
    public class QuotationContactVM : QuotationContact
    {
        public string ClientName { get; set; }
    }
    public class QuotationDetailVM : QuotationDetail
    {
        public string displayclass { get; set; }
        public int RowOrder { get; set; }
       public int ProductFamilyID { get; set; }
        public int EquipmentTypeID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int EnquiryID { get; set; }
        public string EquipmentType { get; set; }
            public string CategoryName { get; set; }
            public string ProductFamilyName { get; set; }
            public string EmployeeName { get; set; }
            public string UnitName { get; set; }
       
            public string EquipmentStatus { get; set; }
            public bool Deleted { get; set; }

        public string EquipmentName { get; set; }
        public string EstimationNo { get; set; }

    }

    public class QuotationTermsVM : TermsCondition
    {
        public int QuotationID { get; set; }
    }
    public class QuotationScopeofWorkVM : QuotationScopeofwork
    {
        public string EquipmentName { get; set; }

        [AllowHtml]
        public string Description1 { get; set; }
        public bool Checked { get; set; }
    }
    public class ScopeOfWorkItemVM
    {
        public int ID { get; set; }
        public int QuotationID { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public int OrderNo { get; set; }
    }
    public class ScopeOfWorkGroupVM
    {
        public string EquipmentName { get; set; }
        public string Model { get; set; }
        public int Qty { get; set; }
        public List<ScopeOfWorkItemVM> Items { get; set; } = new List<ScopeOfWorkItemVM>();
    }
    public class QuotationScopeofWorkVM1
    {
        public int ID { get; set; }
        public int QuotationID { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string Model { get; set; }

        public int OrderNo { get; set; }

        [System.Web.Mvc.AllowHtml] // allow <b>, <i>, etc.
        public string Description { get; set; }

        public bool Checked { get; set; }
    }
    public class QuotationWarrantyVM : QuotationWarranty
    {
        public bool Checked { get; set; }
        public string EquipmentName { get; set; }
    }
    public class QuotationExclusionVM : QuotationExclusion
    {
        public bool Checked { get; set; }
        public string EquipmentName { get; set; }
    }
    public class QuotationEditorData
    {
        public string ScopeofWork { get; set; }
        public string Warranty { get; set; }
        public string Exclusions { get; set; }

    }

    public class QuotationSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public string QuotationNo { get; set; }
        public List<QuotationVM> Details { get; set; }
    }

    public class JobHandOverVM : JobHandover
    {
        public string EnquiryNo { get; set; }
        public string ClientName { get; set; }
        public int BondyTypeId { get; set; }
        public int WarrantyId { get; set; }
        public int PaymentInstrumentId { get; set; }
        public List<JobPurchaseOrderVM> OrderDetails { get; set; }
        public List<JobBondVM> BondDetails { get; set; }
        public List<JobPaymentVM> PaymentDetails { get; set; }
        public List<QuotationWarrantyVM> WarrantyDetails { get; set; }

    }
    public class JobPurchaseOrderVM : JobPurchaseOrderDetail
    {
        public int SqNo { get; set; }
        public DateTime JobDate { get; set; }
        public string EnquiryNo { get; set; }
        public string QuotationNo { get; set; }
        public string ProjectNumber { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        
        public string CityName { get; set; }
        public string Site { get; set; }
        public string CountryName { get; set; }
        public string CreatedByName { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public string MRequestNo { get; set; }
        public DateTime QuotationDate { get; set; }
        public List<QuotationDetailVM> Details { get; set; }
        public List<JobBondVM> BondDetails { get; set; }
        public List<QuotationWarrantyVM> WarrantyDetails { get; set; }

    }
    public class JobBondVM : JobBondDetail
    {
        public string BondName { get; set; }

    }
    public class JobWarrantyVM : JobWarranty
    {

    }

    public class JobPaymentVM : JobPaymentTerm
    {
        public string PaymentInstrument { get; set; }

    }
    public class JobHandOverSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public string ProjectNo { get; set; }
        public List<JobHandOverVM> Details { get; set; }
    }

    public class JobPOSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EnquiryNo { get; set; }
        public string ProjectNo { get; set; }
        public string PONo { get; set; }
        public List<JobPurchaseOrderVM> Details { get; set; }
    }

    public class EquipmentTypeVM : EquipmentType
    {
        public string ProductFamilyName { get; set; }
        public string BrandName { get; set; }
        public List<EquipmentScopeofworkVM> ScopeofWorkDetails { get; set; }
        public List<EquipmentWarrantyVM> WarrantyDetail { get; set; }
        public List<EquipmentExclusionVM> Exclusions { get; set; }
    }
    public class EquipmentScopeofworkVM : EquipmentScopeofwork
    {
        public bool Deleted { get; set; }
        public string deletedclass { get;set;}
    }
    public class EquipmentWarrantyVM : EquipmentWarranty
    {
        public bool Deleted { get; set; }
        public string deletedclass { get; set; }

    }
    public class EquipmentExclusionVM : EquipmentExclusion
    {
        public bool Deleted { get; set; }
        public string deletedclass { get; set; }
    }
}