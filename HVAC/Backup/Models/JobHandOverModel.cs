using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    // --- Model Class ---

    public class ProjectHandoverModel
    {
        public DateTime ReportDate { get; set; }
        public string ProjectTitle { get; set; }
        public string JobNo { get; set; }
        public int Variation { get; set; }
        public string Contractor { get; set; }

        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public decimal OrderValue { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalValue { get; set; }

        public decimal JobValue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Margin { get; set; }
        public decimal MarginPercent { get; set; }

        public string PaymentTerms { get; set; }
        public string HandledBy { get; set; }

        public string MainUnitWarranty { get; set; }
        public string CompressorWarranty { get; set; }
        public string CondenserWarranty { get; set; }
        public string Refrigerant { get; set; }
        public string WarrantyStart { get; set; }

        public string DeliveryLocation { get; set; }
        public string DeliveryPlace { get; set; }
        public string DeliveryPeriod { get; set; }

        public string AdvanceBond { get; set; }
        public string PerformanceBond { get; set; }
        public string CorporateBond { get; set; }
        public string AnyBond { get; set; }

        public List<CostingItem> CostingList { get; set; }
        public List<QuotationDetailVM> CostingDetails { get; set; }

    }
    public class CostingItem
    {
        public int SlNo { get; set; }
        public string Item { get; set; }
        public string UnitModelNo { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalRate { get; set; }
        public string Remarks { get; set; }
    }




    public class JobDetailsViewModel
    {
        public JobInfo Job { get; set; }
        public CustomerInfo Customer { get; set; }
        public List<JobPurchaseOrderVM> PurchaseOrders { get; set; }
        public List<JobBondVM> BondDetails { get; set; }
        public List<QuotationWarrantyVM> WarrantyDetails { get; set; }
        public string PaymentTerms { get; set; }
        public List<QuotationDetailVM> Costing { get; set; }

        public JobDetailsViewModel()
        {
            PurchaseOrders = new List<JobPurchaseOrderVM>();
            BondDetails = new List<JobBondVM>();
            WarrantyDetails = new List<QuotationWarrantyVM>();

            Costing = new List<QuotationDetailVM>();
            Job = new JobInfo();
            Customer = new CustomerInfo();
        }
    }

    public class JobInfo
    {
        public string JobNo { get; set; }
        public string Title { get; set; }
        public string ProjectSite { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectManager { get; set; }
        public string Notes { get; set; }

        public decimal? JobValue { get; set; } = 0m;
        public decimal? Cost { get; set; } = 0m;
        public decimal? Margin { get; set; } = 0m;
        public decimal? Vat { get; set; } = 0m;
        public decimal? TotalValue { get; set; } = 0m;


    }

    public class CustomerInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string TRN_VAT { get; set; }
    }

    public class POInfo
    {
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public string Vendor { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class BondInfo
    {
        public string BondType { get; set; }   // e.g., Performance, Advance, Retention
        public string BondNo { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal Amount { get; set; }
        public string Issuer { get; set; }
        public string Remarks { get; set; }
    }

    public class CostingInfo
    {
        public decimal Materials { get; set; }
        public decimal Labor { get; set; }
        public decimal Subcontract { get; set; }
        public decimal Overheads { get; set; }
        public decimal Contingency { get; set; }
        public decimal Taxes { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalCost => Materials + Labor + Subcontract + Overheads + Contingency + Taxes - Discount;
        public decimal QuotedPrice { get; set; }
        public decimal Margin => QuotedPrice - TotalCost;
        public decimal MarginPct => QuotedPrice == 0 ? 0 : (Margin / QuotedPrice) * 100m;
    }
}



