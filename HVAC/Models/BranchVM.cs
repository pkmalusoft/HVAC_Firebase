using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
  public class BranchVM
  {
    public int BranchID { get; set; }

    public string BranchName { get; set; }

    public string Address1 { get; set; }

    public string Address2 { get; set; }

    public string Address3 { get; set; }

    public int CountryID { get; set; }

    public int CityID { get; set; }

    public int LocationID { get; set; }

    public string KeyPerson { get; set; }

    public int DesignationID { get; set; }

    public string Phone { get; set; }

    public string PhoneNo1 { get; set; }

    public string PhoneNo2 { get; set; }

    public string PhoneNo3 { get; set; }

    public string PhoneNo4 { get; set; }

    public string MobileNo1 { get; set; }

    public string MobileNo2 { get; set; }

    public string EMail { get; set; }

    public string Website { get; set; }

    public string BranchPrefix { get; set; }

    public int CurrencyID { get; set; }

    public int AcCompanyID { get; set; }

    public bool StatusAssociate { get; set; }
        public int InvoiceNoStart { get; set; }
    public string AWBFormat { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Location { get; set; }


        public string CountryName { get; set; }

        public string CityName { get; set; }

        public string LocationName { get; set; }

        public string Currency { get; set; }

        public string InvoicePrefix { get; set; }

        public string InvoiceFormat { get; set; }

        public string VATRegistrationNo { get; set; }
        public decimal VATPercent { get; set; }
        public int AcFinancialYearID { get; set; }
        public int VATAccountId { get; set; }
        public bool DRRProcess { get; set; }
        public decimal ImportVatThreshold { get; set; }
        public string TaxType { get; set; }
  }

     
   
    public class TName
    {
        public string TableName { get; set; }
    }
    public class DatePicker
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Create { get; set; }

        public int? StatusId { get; set; }
        public int? AgentId { get; set; }
        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }
        public string MovementId { get; set; }
        public int[] SelectedValues { get; set; }

        public int paymentId { get; set; }
        public int AcHeadId { get; set; }

    }
   

   
   
     
     
   

    

}
