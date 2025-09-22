using System;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.Web;
//using System.Data.Objects;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Linq;
using HVAC.DAL;

namespace HVAC.Models
{
  public class CommonFunctions
  {
        public static string GetConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            }
        }

        public static double GetGMTHours
        {
            get
            {
                return Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());
            }
        }
        public static DateTime ParseDate(string str, string Format = "dd-MMM-yyyy")
        {
            DateTime dt = DateTime.MinValue;
            if (DateTime.TryParseExact(str, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            return dt;
        }
        public static int ParseInt(string str)
        {
            int k = 0;
            if (Int32.TryParse(str, out k))
            {
                return k;
            }
            return 0;
        }
        public static Decimal ParseDecimal(string str)
        {
            Decimal k = 0;
            if (Decimal.TryParse(str, out k))
            {
                return k;
            }
            return 0;
        }
        public static bool ParseBoolean(string str)
        {
            bool k = false;
            if (bool.TryParse(str, out k))
            {
                return k;
            }
            return k;
        }
        public static string GetMinFinancialDate()
        {
            HVACEntities db = new HVACEntities();
            
            if (HttpContext.Current?.Session?["fyearid"] == null)
                return DateTime.Now.ToString("yyyy/MM/dd");
                
            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());

            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);

            string ss = "";
            if (startdate != null)
                ss = startdate.Year + "/" + startdate.Month + "/" + startdate.Day; // string.Format("{0:YYYY MM dd}", (object)startdate.ToString());

            return ss;
        }
        public static string GetMaxFinancialDate()
        {
            HVACEntities db = new HVACEntities();

            if (HttpContext.Current?.Session?["fyearid"] == null)
                return DateTime.Now.ToString("yyyy/MM/dd");
                
            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());

            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            string ss = "";
            if (startdate != null)
                ss = startdate.Year + "/" + startdate.Month + "/" + startdate.Day; // string.Format("{0:YYYY MM dd}", (object)startdate.ToString());

            return ss;
        }

        public static string GetShortDateFormat(object iInputDate)
    {
      if (iInputDate != null && iInputDate!="")
        return string.Format("{0:dd/MM/yyyy}", (object) Convert.ToDateTime(iInputDate));
      return "";
    }

        public static string GetShortDateFormat1(object iInputDate)
        {
            if (iInputDate != null)
                return string.Format("{0:dd-MM-yyyy}", (object)Convert.ToDateTime(iInputDate));
            return "";
        }
        public static bool CheckCreateEntryValid()
        {
            //HVACEntities db = new HVACEntities();
            //int currentfyearid = db.AcFinancialYears.Where(cc => cc.CurrentFinancialYear == true).FirstOrDefault().AcFinancialYearID;
            //int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            //if (currentfyearid != fyearid)
            //    return false;
            return true;
        }
        public static DateTime GetFirstDayofYear()
        {
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);                                

            return Convert.ToDateTime(startdate);

            

        }
        public static DateTime GetFirstDayofMonth()
        {
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);

            string vdate = "01" + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            DateTime todaydate = DateTime.Now.Date;

            StatusModel statu= GeneralDAO.CheckDateValidate(vdate, fyearid);
            vdate = statu.ValidDate;

            return Convert.ToDateTime(vdate);

            //if (todaydate>=startdate && todaydate <=enddate ) //current date between current financial year
            //    return Convert.ToDateTime(vdate);
            //else
            //{
            //    vdate = "01" + "-" + enddate.Month.ToString() + "-" + enddate.Year.ToString();
            //    return Convert.ToDateTime(vdate);
            //}

        }
        public static DateTime GetFirstDayofWeek()
        {
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);

            string vdate = "01" + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            todaydate = todaydate.AddDays(-7);
            StatusModel statu = GeneralDAO.CheckDateValidate(todaydate.ToString(), fyearid);
            vdate = statu.ValidDate;
            return Convert.ToDateTime(todaydate);

            //if (todaydate>=startdate && todaydate <=enddate ) //current date between current financial year
            //    return Convert.ToDateTime(vdate);
            //else
            //{
            //    vdate = "01" + "-" + enddate.Month.ToString() + "-" + enddate.Year.ToString();
            //    return Convert.ToDateTime(vdate);
            //}

        }
        public static DateTime GetLastDayofMonth()
        {
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());

            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            StatusModel statu = GeneralDAO.CheckDateValidate(todaydate.ToString(), fyearid);
            string vdate = statu.ValidDate;
            return Convert.ToDateTime(vdate);

            //DateTime todaydate = DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            //return todaydate;
            //if (todaydate >= startdate && todaydate <= enddate) //current date between current financial year
            //    return todaydate;
            //else
            //{                
            //    return enddate;
            //}

        }

        public static DateTime GetCurrentDateTime()
        {
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());

            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            StatusModel statu = GeneralDAO.CheckDateValidate(todaydate.ToString(), fyearid);
            string vdate = statu.ValidDate;
            return  Convert.ToDateTime(vdate);
            
        }
        public static DateTime GetCurrentDateTime1()
        {
            HVACEntities db = new HVACEntities();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());

            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            StatusModel statu = GeneralDAO.CheckDateValidate(todaydate.ToString(), fyearid);
            string vdate = statu.ValidDate;
            return Convert.ToDateTime(vdate);

        }
        public static DateTime GetBranchDateTime(DateTime? dateTime = null)
        {

            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());
            if (dateTime == null)
                dateTime = DateTime.UtcNow.AddHours(hours);
            else
            {
                dateTime = Convert.ToDateTime(dateTime).AddHours(hours);
            }

            DateTime todaydate = Convert.ToDateTime(dateTime);
            return todaydate;

        }

        public static int GetBranchTaxAccountId(int branchId)
        {

            HVACEntities db = new HVACEntities();

            var branch = db.BranchMasters.Find(branchId);
            var taxaccountid = 0;
            if (branch != null)
                taxaccountid = Convert.ToInt32(branch.VATAccountId);

            return taxaccountid;
        }
        public static string GetLongDateFormat(object iInputDate)
        {
            if (iInputDate != null)
                return string.Format("{0:dd MMM yyyy hh:mm}", (object)Convert.ToDateTime(iInputDate));
            return "";
        }

        public static string GetDecimalFormat(object iInputValue, string Decimals)
        {
            if (Decimals == "2")
            {
                if (iInputValue != null)
                    return  String.Format("{0:0.00}", (object)Convert.ToDecimal(iInputValue));
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
        }
        public static string GetDecimalFormat1(object iInputValue, string Decimals="")
        {
            if (Decimals == "")
                Decimals = HttpContext.Current.Session["Decimal"].ToString();

            if (Convert.ToString(iInputValue) == "")
                return "";
            
            if (Decimals == "2")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.00}", (object)Convert.ToDecimal(iInputValue));
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
        }

        public static string GetCurrencyId(int CurrencyId)
        {
            HVACEntities db = new HVACEntities();
            try
            {
                

                string currencyname = db.CurrencyMasters.Find(CurrencyId).CurrencyName;

                return currencyname;
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        public static int GetDefaultCurrencyId()
        {
            HVACEntities db = new HVACEntities();
            try
            {
                if (HttpContext.Current?.Session?["CurrentBranchID"] == null)
                    return 0;
                int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
                int CurrencyId = 0;
                
                var branch=db.BranchMasters.Find(branchid);//.CurrencyID;
                if (branch.CurrencyID == null)
                {
                   CurrencyId=db.CurrencyMasters.Where(cc => cc.StatusBaseCurrency == true).FirstOrDefault().CurrencyID;

                }
                else
                {
                    CurrencyId = Convert.ToInt32(branch.CurrencyID);
                }

                return CurrencyId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public static int GetUSDCurrencyId()
        {
            HVACEntities db = new HVACEntities();
            try
            {
                if (HttpContext.Current?.Session?["CurrentBranchID"] == null)
                    return 0;
                int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
                int CurrencyId = 0;

                var branch = db.BranchMasters.Find(branchid);//.CurrencyID;
                if (branch.CurrencyID == null)
                {
                    CurrencyId = db.CurrencyMasters.Where(cc => cc.CurrencyCode == "USD").FirstOrDefault().CurrencyID;

                }
                else
                {
                    CurrencyId = Convert.ToInt32(branch.CurrencyID);
                }

                return CurrencyId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal GetUSDExRate(int CurrencyID)
        {
            HVACEntities db = new HVACEntities();
            try
            {
                CurrencyMaster Exrate = db.CurrencyMasters.Where(cc => cc.CurrencyID == CurrencyID).FirstOrDefault();
                if (Exrate == null)
                {
                    return 1;
                }
                return Convert.ToDecimal(Exrate.ExchangeRate);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static decimal GetCurrencyExRate(int CurrencyID)
        {
            HVACEntities db = new HVACEntities();
            try
            {
                CurrencyMaster Exrate = db.CurrencyMasters.Where(cc => cc.CurrencyID == CurrencyID).FirstOrDefault();
               if (Exrate ==null)
                {
                    return 1;
                }
                return  Convert.ToDecimal(Exrate.ExchangeRate);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string GetFormatNumber(object iInputValue, string Decimals="")
        {
            if (Decimals == "")
                Decimals = HttpContext.Current?.Session?["Decimal"]?.ToString() ?? "2";
            string NumberFormat = HttpContext.Current?.Session?["NumberFormat"]?.ToString() ?? "Lakhs";
            if (Decimals == "2")
            {
                                
                    if (iInputValue != null)
                    {
                     decimal v=0;
                      v = Decimal.Parse(((object)Convert.ToDecimal(iInputValue)).ToString());
                    if (v != 0)
                    {
                        if (NumberFormat == "Lakhs") //Lakhs
                        {
                            var c = CultureInfo.GetCultureInfo("en-IN");
                            decimal n = Convert.ToDecimal(iInputValue);
                            string s = n.ToString("#,0.00", c);
                            return s;

                        }
                            else
                                {
                                    var c = CultureInfo.GetCultureInfo("ar-AE");
                                    decimal n = Convert.ToDecimal(iInputValue);
                                    string s = n.ToString("#,0.00", c);
                                    return s;
                                }

                        //return String.Format("{0:#,0.00}", (object)Convert.ToDecimal(iInputValue));


                    }
                    else
                        return "";
                    }
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:#,0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            else if (Decimals == "0")
            {
                if (iInputValue != null)
                    return String.Format("{0:#,0}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
            
        }
        public static string GetBranchFormatNumber(object iInputValue, string Decimals = "")
        {
            if (Decimals == "")
                Decimals = HttpContext.Current?.Session?["Decimal"]?.ToString() ?? "2";
            string formatnumber = HttpContext.Current?.Session?["NumberFormat"]?.ToString() ?? "Lakhs";
            if (Decimals == "2")
            {

                if (iInputValue != null && iInputValue!="")
                {
                    decimal v = 0;
                    v = Decimal.Parse(((object)Convert.ToDecimal(iInputValue)).ToString());
                    if (v > 0)
                    {
                        if (formatnumber == "Lakhs")
                        { return String.Format("{0:#,0.00}", (object)Convert.ToDecimal(iInputValue)); }
                        else
                        {
                            string result = Convert.ToDecimal(iInputValue).ToString("#,#.00", CultureInfo.InvariantCulture);
                            return result;// String.Format("{0:###,###,###,000.00}", (object)Convert.ToDecimal(iInputValue));
                        }
                    }
                    else
                        return "";
                }
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:#,0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";

        }
        

        public  static string GetLoggedEmployeeName()
        {
            HVACEntities db = new HVACEntities();
            if (HttpContext.Current?.Session?["UserID"] == null)
                return "";
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            var employee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            if (employee != null)
            {
                string employeename = employee.FirstName + " " + employee.LastName;
                return employeename;
            }
            else
            {
                return "";
            }
        }
        public static int GetLoggedEmployeID()
        {
            HVACEntities db = new HVACEntities();
            if (HttpContext.Current?.Session?["UserID"] == null)
                return 0;
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            var employee = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
            if (employee != null)
            {
                
                return employee.EmployeeID;
            }
            else
            {
                return 0;
            }
        }


        public static string GetEmployeeName(int? EmpID)
        {
            HVACEntities db = new HVACEntities();
            if (EmpID == null)
            {
                return "";
            }
            if (HttpContext.Current?.Session?["UserID"] == null)
                return "";
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            var employee = db.EmployeeMasters.Where(cc => cc.EmployeeID == EmpID).FirstOrDefault();
            if (employee != null)
            {
                string employeename = employee.FirstName + " " + employee.LastName;
                return employeename;
            }
            else
            {
                return "";
            }
        }
    }

    public class StatusModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ValidDate  { get; set; }
    }
}
