using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
 
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;
using CrystalDecisions.ReportAppServer.CommonObjectModel;
using System.Text;
using HVAC.Models;
namespace HVAC.DAL
{
    public class ReportsDAO
    {
        public static string GenerateDefaultReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();
            
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "DefaultReport.rpt"));
            
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);
            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue("Default Report");
            string period = "Report Period as on Date"; 
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
           
            string reportname = "DefaultReport.pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);

            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            reportpath = "~/ReportsPDF/" + reportname;
            return reportpath;           
        }
        public static string QuotationReport(int QuotationID,int ClientId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_QuotationPrint";
        
            comd.Parameters.AddWithValue("@QuotationID", QuotationID);
            comd.Parameters.AddWithValue("@ClientId", ClientId);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "QuotationPrint");

            DataSet dt1 = ds.Tables[0].DataSet;
            DataSet dt2 = ds.Tables[1].DataSet;
            DataSet dt3 = ds.Tables[2].DataSet;
            DataSet dt4 = ds.Tables[3].DataSet;
            //DataSet dt5 = ds.Tables[4].DataSet;
            //generate XSD to design report            
            //System.IO.StreamWriter writer1 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationPrint.xsd"));
            //dt1.WriteXmlSchema(writer1);
            //writer1.Close();

            //generate XSD to design report            
            //System.IO.StreamWriter writer2 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationPrint_Scope.xsd"));
            //dt2.WriteXmlSchema(writer2);
            //writer2.Close();

            //System.IO.StreamWriter writer3 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationPrint_Warrant.xsd"));
            //dt3.WriteXmlSchema(writer3);
            //writer3.Close();

            //System.IO.StreamWriter writer4 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationPrint_Exclusions.xsd"));
            //dt4.WriteXmlSchema(writer4);
            //writer4.Close();

            //System.IO.StreamWriter writer5 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationPrint_Terms.xsd"));
            //dt5.WriteXmlSchema(writer5);
            //writer5.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "QuotationPrint.rpt"));

            rd.SetDataSource(dt1);
            rd.Subreports[0].SetDataSource(dt2);
            rd.Subreports[1].SetDataSource(dt3);
            rd.Subreports[2].SetDataSource(dt4);
            //rd.Subreports[3].SetDataSource(dt5);

            //Set Paramerter Field Values -General
            #region "param"            
            string companyname = ""; //SourceMastersModel.GetCompanyname(branchid);
            string companylocation = "";// SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            //string warranty = ds.Tables[0].Rows[0]["ScopeofWork"].ToString();
            //warranty= Uri.UnescapeDataString(warranty);
            //rd.ParameterFields["QuoteWarranty"].CurrentValues.AddValue(warranty);

            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue("");
            //   rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Quotation");

            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue("");

            string userdetail = "";// "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "QuotationPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            //reportparam.ReportFileName = reportname;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);

            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;


        }

        public static string EstimationReport(int EstimationID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_EstimationPrint";

            comd.Parameters.AddWithValue("@EstimationID", EstimationID);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "EstimationPrint");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "EstimationPrint.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "EstimationPrint.rpt"));

            rd.SetDataSource(ds);


            //Set Paramerter Field Values -General
            #region "param"            
            string companyname = ""; //SourceMastersModel.GetCompanyname(branchid);
            string companylocation = "";// SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            //string warranty = ds.Tables[0].Rows[0]["ScopeofWork"].ToString();
            //warranty= Uri.UnescapeDataString(warranty);
            //rd.ParameterFields["QuoteWarranty"].CurrentValues.AddValue(warranty);

            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue("");
            //   rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Estimation Print");

            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue("");

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "EstimationPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            //reportparam.ReportFileName = reportname;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);

            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;


        }
        public static string EnquiryRegister()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            EnquiryRegisterReportModel reportparam = (EnquiryRegisterReportModel)(HttpContext.Current.Session["EnquiryRegister"]);
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_EnquiryRegister";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@BranchId", branchid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "EnquiryRegister");




            //enerate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "EnquiryRegister.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "EnquiryRegister.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
          //  rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);


            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Enquiry Register Report");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "EnquiryRegisterReport_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "EnquiryRegisterReport_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "EnquiryRegisterReport_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;
 
        }


        public static string QuotationRegister()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            ReportParam1 reportparam = (ReportParam1)(HttpContext.Current.Session["QuotationRegister"]);
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_QuotationRegister";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@EnquiryNo", "");
            comd.Parameters.AddWithValue("@QuotationNo", "");
            comd.Parameters.AddWithValue("@EmployeeID", "0");
            comd.Parameters.AddWithValue("@BranchID", branchid);
            comd.Parameters.AddWithValue("@FyearID", yearid);


            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "QuotationRegister");




            //enerate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "QuotationRegister.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "QuotationRegister.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            //rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);


            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Quotation Register");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "QuotationRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "QuotationRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "QuotationRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

        }


        public static string JobHandOverReport(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_JobHandOverPrint";

            comd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "JobHandOverPrint");

            //generate XSD to design report            
            System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "JobHandOverPrint.xsd"));
            ds.WriteXmlSchema(writer);
            writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "JobHandOverPrint.rpt"));

            rd.SetDataSource(ds);


            //Set Paramerter Field Values -General
            #region "param"            
            string companyname = ""; //SourceMastersModel.GetCompanyname(branchid);
            string companylocation = "";// SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            //string warranty = ds.Tables[0].Rows[0]["ScopeofWork"].ToString();
            //warranty= Uri.UnescapeDataString(warranty);
            //rd.ParameterFields["QuoteWarranty"].CurrentValues.AddValue(warranty);

            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue("");
            //   rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Quotation");

            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue("");

            string userdetail = "";// "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "QuotationPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            //reportparam.ReportFileName = reportname;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);

            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;


        }


        public static string InwardPOReport(int ID,int ClientId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_InwardPOPrint";

            comd.Parameters.AddWithValue("@ID", ID);
            comd.Parameters.AddWithValue("@ClientId", ClientId);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "InWardPOPrint");

            DataSet dt1 = ds.Tables[0].DataSet;
            DataSet dt2 = ds.Tables[1].DataSet;
            DataSet dt3 = ds.Tables[2].DataSet;
            DataSet dt4 = ds.Tables[3].DataSet;
            //DataSet dt5 = ds.Tables[4].DataSet;
            //generate XSD to design report            
            //System.IO.StreamWriter writer1 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "InwardPOPrint.xsd"));
            //dt1.WriteXmlSchema(writer1);
            //writer1.Close();

            //generate XSD to design report            
            //System.IO.StreamWriter writer2 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "InwardPOCostingItem.xsd"));
            //dt2.WriteXmlSchema(writer2);
            //writer2.Close();

            //System.IO.StreamWriter writer3 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "InwardPO_Warranty.xsd"));
            //dt3.WriteXmlSchema(writer3);
            //writer3.Close();

            //System.IO.StreamWriter writer4 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "InwardPO_Bond.xsd"));
            //dt4.WriteXmlSchema(writer4);
            //writer4.Close();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "InwardPOPrint.rpt"));

            rd.SetDataSource(dt1);
            rd.Subreports[0].SetDataSource(dt2);
            rd.Subreports[1].SetDataSource(dt3);
            rd.Subreports[2].SetDataSource(dt4);
            //rd.Subreports[3].SetDataSource(dt5);

            //Set Paramerter Field Values -General
            #region "param"            
            string companyname = ""; //SourceMastersModel.GetCompanyname(branchid);
            string companylocation = "";// SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            //string warranty = ds.Tables[0].Rows[0]["ScopeofWork"].ToString();
            //warranty= Uri.UnescapeDataString(warranty);
            //rd.ParameterFields["QuoteWarranty"].CurrentValues.AddValue(warranty);

            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue("");
            //   rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Quotation");

            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue("");

            string userdetail = "";// "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "QuotationPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            //reportparam.ReportFileName = reportname;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);

            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;


        }


        public static string GenerateStockStatementReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            StockReportParam reportparam = (StockReportParam)HttpContext.Current.Session["StockStatementReportParam"];// SessionDataModel.GetCustomerLedgerReportParam();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_StockStatement";
            //comd.Parameters.AddWithValue("@ProductCategoryID", reportparam.ProductCategoryID);
            comd.Parameters.AddWithValue("@EquipmentTypeID", 0);
            comd.Parameters.AddWithValue("@AsonDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@BranchID", branchid);
            //comd.Parameters.AddWithValue("@FYearID", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "StockStatement"); //stomerLedgerDetail

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "StockStatement.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "StockStatement.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Stock Statement");
            string period = " As on " + reportparam.FromDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetBranchDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "StockStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "StockStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "StockStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateStockLedgerReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            StockReportParam reportparam = (StockReportParam)HttpContext.Current.Session["StockLedgerReportParam"];// SessionDataModel.GetCustomerLedgerReportParam();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_StockLedger";
            //comd.Parameters.AddWithValue("@ProductCategoryID", reportparam.ProductCategoryID);
            comd.Parameters.AddWithValue("@EquipmentTypeID", 0);
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@BranchID", branchid);
            //comd.Parameters.AddWithValue("@FYearID", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "StockLedger"); //stomerLedgerDetail

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "StockLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "StockLedger.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Stock Ledger");
            string period = " From Date " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "StockLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "StockLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "StockLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }


        #region PagePrint
        public static string PurchaseOrderReport(int PurchaseOrderID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "HVAC_PurchaseOrderPrint";

            comd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "PurchaseOrderPrint");

            DataSet dt1 = ds.Tables[0].DataSet;
            DataSet dt2 = ds.Tables[1].DataSet;
            DataSet dt3 = ds.Tables[2].DataSet;
            DataSet dt4 = ds.Tables[3].DataSet;
            DataSet dt5 = ds.Tables[4].DataSet;

            //generate XSD to design report            
            //System.IO.StreamWriter writer1 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "PurchaseOrderPrint.xsd"));
            //dt1.WriteXmlSchema(writer1);
            //writer1.Close();

            ////generate XSD to design report            
            //System.IO.StreamWriter writer2 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "PurchaseOrderPrint_Item.xsd"));
            //dt2.WriteXmlSchema(writer2);
            //writer2.Close();

            //System.IO.StreamWriter writer3 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "PurchaseOrderPrint_Terms.xsd"));
            //dt3.WriteXmlSchema(writer3);
            //writer3.Close();

            //System.IO.StreamWriter writer4 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "PurchaseOrderPrint_Revision.xsd"));
            //dt4.WriteXmlSchema(writer4);
            //writer4.Close();

            //System.IO.StreamWriter writer5 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "PurchaseOrderPrint_Bank.xsd"));
            //dt5.WriteXmlSchema(writer5);
            //writer5.Close();


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "PurchaseOrderPrint.rpt"));

            rd.SetDataSource(dt1);
            rd.Subreports[0].SetDataSource(dt2);
            rd.Subreports[1].SetDataSource(dt3);
            rd.Subreports[2].SetDataSource(dt4);
            rd.Subreports[3].SetDataSource(dt5);

            //Set Paramerter Field Values -General
            #region "param"            
            string companyname = ""; //SourceMastersModel.GetCompanyname(branchid);
            string companylocation = "";// SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            //string warranty = ds.Tables[0].Rows[0]["ScopeofWork"].ToString();
            //warranty= Uri.UnescapeDataString(warranty);
            //rd.ParameterFields["QuoteWarranty"].CurrentValues.AddValue(warranty);

            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue("");
            //   rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Estimation Print");

            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue("");

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + CommonFunctions.GetCurrentDateTime();
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "PurchaseOrderPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            //reportparam.ReportFileName = reportname;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);

            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;


        }

        public static PurchaseOrderViewModel GetPurchaseOrderFromProcedure(int ID)
        {
            PurchaseOrderViewModel model = null;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("HVAC_PurchaseOrderPrint", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PurchaseOrderID", ID);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Read Main Purchase Order Info (First Result Set)
                        if (reader.Read())
                        {
                            model = new PurchaseOrderViewModel
                            {
                                OrderNo = reader["OrderNo"].ToString(),
                                Revision = Convert.ToInt32(reader["Revision"]),
                                //ProjectNo = reader["ProjectNo"].ToString(),
                                //ProjectName = reader["ProjectName"].ToString(),
                                SONo = reader["SONo"].ToString(),
                                ProductFamily = reader["ProductFamily"].ToString(),
                                Product = reader["Product"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                PortOfImport = reader["PortOfImport"].ToString(),
                                PaymentTerms = reader["PaymentTerms"].ToString(),
                                CountryOfOrigin = reader["CountryOfOrigin"].ToString(),
                                IncoTerms = reader["IncoTerms"].ToString(),
                                DeliveryWeeks = reader["DeliveryWeeks"].ToString(),
                                Currency = reader["Currency"].ToString(),
                                TotalValue = Convert.ToDecimal(reader["TotalValue"]),
                                FreightCharges = Convert.ToDecimal(reader["FreightCharges"]),
                                VATAmount = Convert.ToDecimal(reader["VATAmount"]),
                                OriginCharges = Convert.ToDecimal(reader["OriginCharges"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                FinanceCharges = Convert.ToDecimal(reader["FinanceCharges"]),
                                TotalPOValue = Convert.ToDecimal(reader["TotalPOValue"]),
                                ConsigneeName = reader["ConsigneeName"].ToString(),
                                ConsigneeAddress = reader["ConsigneeAddress"].ToString(),
                                BankName = reader["BankName"].ToString(),
                                BankAccount = reader["BankAccount"].ToString(),
                                SwiftCode = reader["SwiftCode"].ToString(),
                                ChangeOrderDetails = reader["ChangeOrderDetails"].ToString(),
                                PreviousPOValue = Convert.ToDecimal(reader["PreviousPOValue"]),
                                SupplierName = reader["SupplierName"].ToString(),
                                SupplierAddress = reader["SupplierAddress1"].ToString(),
                                SupplierContactPerson = reader["SupplierContactPerson"].ToString()
                            };
                        }
                        model.Items = new List<PurchaseOrderItem>();

                        // Read Next Result Set – Line Items
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.Items.Add(new PurchaseOrderItem
                                {
                                    SqNo = Convert.ToInt32(reader["SqNo"]),
                                    Description = reader["Description"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    JobNo = reader["JobNo"].ToString(),
                                    UnitRate = Convert.ToDecimal(reader["UnitRate"]),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                                });
                            }
                        }
                        model.ProjectBreakup = new List<ProjectBreakupItem>();
                        // Read Next Result Set – Project Breakup
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.ProjectBreakup.Add(new ProjectBreakupItem
                                {
                                    ProjectCode = reader["ProjectCode"].ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"])
                                });
                            }
                        }
                        model.ImportantNotes = new List<string>();
                        // Read Next Result Set – Important Notes
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.ImportantNotes.Add(reader["Note"].ToString());
                            }
                        }

                        // Read Next Result Set – Supplier
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                model.Supplier=(new SupplierVM
                                {
                                    SupplierName = reader["SupplierName"].ToString(),
                                    Address1 = reader["Address1"].ToString(),
                                    Address2 = reader["Address2"].ToString(),
                                    CityName = reader["Address2"].ToString(),

                                });
                            }
                        }


                    }
                }
            }

            return model;
        }

        #endregion

        #region RegisterReport
        public static DataTable GetEnquiryRegisterData(DateTime fromDate, DateTime toDate,int BranchId,int FYearId)
        {
            
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtResult = new DataTable();

            // Define only required columns
          
            dtResult.Columns.Add("Enquiry No.", typeof(string));
            dtResult.Columns.Add("Enquiry Date", typeof(string));
            dtResult.Columns.Add("Project Title", typeof(string));
            dtResult.Columns.Add("ClientName", typeof(string));          
            dtResult.Columns.Add("Stage", typeof(string));
            dtResult.Columns.Add("Due Date", typeof(string));
           

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_EnquiryRegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId",FYearId);
                cmd.Parameters.AddWithValue("@BranchId", BranchId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            int slno = 1;
            foreach (DataRow dr in dtFromDb.Rows)
            {
                DataRow newRow = dtResult.NewRow();
              
                newRow["Enquiry No."] = dr["EnquiryNo"]?.ToString();
                newRow["Enquiry Date"] = dr["EnquiryDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["EnquiryDate"]).ToString("dd-MM-yyyy")
                    : "";
                newRow["Project Title"] = dr["ProjectName"]?.ToString();
                
                newRow["ClientName"] = dr["ClientName"]?.ToString();
                newRow["Stage"] = dr["EnqStageName"]?.ToString();
                newRow["Due Date"] = dr["DueDate"] != DBNull.Value
                    ? Convert.ToDateTime(dr["DueDate"]).ToString("dd-MM-yyyy")
                    : "";
               

                dtResult.Rows.Add(newRow);
            }

            return dtResult;
        }

        public static List<EnquiryRegisterViewModel> GetEnquiryRegisterList(DateTime fromDate, DateTime toDate, int BranchId, int FYearId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            DataTable dtFromDb = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("HVAC_EnquiryRegister", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", toDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId", FYearId);
                cmd.Parameters.AddWithValue("@BranchId", BranchId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFromDb);
                }
            }

            var result = new List<EnquiryRegisterViewModel>();
            int slno = 1;

            foreach (DataRow dr in dtFromDb.Rows)
            {
                var item = new EnquiryRegisterViewModel
                {
                    SlNo = slno++,
                    EnquiryNo = dr["EnquiryNo"]?.ToString(),
                    EnquiryDate = dr["EnquiryDate"] != DBNull.Value
                        ? Convert.ToDateTime(dr["EnquiryDate"]).ToString("dd-MM-yyyy")
                        : string.Empty,
                    ProjectClient = dr["ProjectName"]?.ToString(),
                    ProjectLocation = dr["City"]?.ToString(),
                    EnquiryType = dr["ClientType"]?.ToString(),
                    AssignedTo = dr["ClientName"]?.ToString(),
                    EnquiryStage = dr["EnqStageName"]?.ToString(),
                    DueDate = dr["DueDate"] != DBNull.Value
                        ? Convert.ToDateTime(dr["DueDate"]).ToString("dd-MM-yyyy")
                        : string.Empty,
                    Status = dr["EnqStatusName"]?.ToString(),
                    Remarks = string.Empty
                };

                result.Add(item);
            }

            return result;
        }

        #endregion
    }
}