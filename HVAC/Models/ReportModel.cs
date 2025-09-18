using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVAC.Models
{
    public class ReportParam1
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int EmployeeID  { get; set; }
        public string Output { get; set; } //printer ,pdf,word,excel
        public string ReportType { get; set; } //sumary details
        public string ReportFileName { get; set; }
    }

    public class EnquirySummaryModel
    {
        public string MonthName { get; set; }
        public int EnquiriesReceived { get; set; }
        public int EnquiriesQuoted { get; set; }

      
        
        

    }
    public class ChartDataModel
    {
        public string Name { get; set; }
        public decimal Y { get; set; }  // decimal for numeric values
        public decimal ProjectValue { get; set; }
    }
    public class SecuredJobModel
    {
        public string Category { get; set; }
        public string ProjectTitle { get; set; }
        public decimal ValueInOMR { get; set; }
    }
    public class SecuredJobDetailModel
    {
        public int SLNo { get; set; }
        public string JobNo { get; set; }
        public string POReference { get; set; }
        public DateTime PODate { get; set; }
        public decimal OrderValue { get; set; }
        public decimal VAT { get; set; }
        public decimal POValue { get; set; } //ordervalue + vat
        public decimal EstimatedCost { get; set; } //ordervalue + vat
        public decimal EstimateProfit { get; set; } //ordervalue + vat
        public decimal EstimateMargin { get; set; } //ordervalue + vat
        public string QuotedBy { get; set; }
        public string Category { get; set; }
        public string ProjectTitle { get; set; }
        
    }
    public class SecuredJobGroupModel
    {
        public string Category { get; set; }
        public List<SecuredJobModel> Jobs { get; set; }
        
        public decimal CategoryTotal { get; set; }
    }

    public class MasterDropdownData
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
    public class CurrentMonthSecuredJobsViewModel
    {
        public List<int> Years { get; set; }
        public List<SelectListItem> Months { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public string SelectedMonthName { get; set; }
        public string ReportClass { get; set; }
        public List<SecuredJobGroupModel> Groups { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal GrandOrderValue { get; set; }
        public decimal GrandVAT { get; set; }        
        public List<SecuredJobDetailModel> JobDetail { get; set; }
    }

    public class AnalysisReportModel
    {
        public List<int> Years { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<SelectListItem> Employees { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public List<EnquirySummaryModel> summaryList { get; set; }
        public List<ChartDataModel> chartdata { get; set; }
        public decimal ProjectValue { get; set; }
        public int[] EnqRcd { get; set; }
        public int[] EnqQtd { get; set; }
 
    }

    public class QuotationStatusViewModel
    {
        public string name { get; set; }
        public decimal y { get; set; }
        public string color { get; set; }
    }
    public class EnquiryRegisterReportModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int EmployeeID { get; set; }
        public string Output { get; set; } //printer ,pdf,word,excel
        public string ReportType { get; set; } //sumary details
        public string ReportFileName { get; set; }
        public List<EnquiryRegisterViewModel> Groups { get; set; }
        
    }


    public class EnquiryRegisterViewModel
    {
        public int SlNo { get; set; }
        public string EnquiryNo { get; set; }
        public string EnquiryDate { get; set; }
        public string ProjectClient { get; set; }
        public string ProjectLocation { get; set; }
        public string EnquiryType { get; set; }
        public string AssignedTo { get; set; }
        public string EnquiryStage { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class RegisterReportModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int EmployeeID { get; set; }
        public string Output { get; set; } //printer ,pdf,word,excel
        public string ReportType { get; set; } //sumary details
        public string ReportFileName { get; set; }
        public DataTable Groups { get; set; }

    }
}