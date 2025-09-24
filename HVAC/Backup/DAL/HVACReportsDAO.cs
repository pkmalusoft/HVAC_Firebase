using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using HVAC.Models;

namespace HVAC.DAL
{
    public class HVACReportsDAO
    {

        public static List<int> GetAvailableYears()
        {
            List<int> years = new List<int>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("HVAC_GetAvailableEnquiryYears", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        years.Add(Convert.ToInt32(reader["YearValue"]));
                    }
                }
            }

            return years;
        }
        public static List<EnquirySummaryModel> GetEnquirySummaryList(int ID)
        {
            List<EnquirySummaryModel> summaryList = new List<EnquirySummaryModel>();

            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "HVAC_GetMonthlyEnquirySummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", ID);
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        summaryList.Add(new EnquirySummaryModel
                        {
                            MonthName = reader["MonthName"].ToString(),
                            EnquiriesReceived = Convert.ToInt32(reader["EnquiriesReceived"]),
                            EnquiriesQuoted = Convert.ToInt32(reader["EnquiriesQuoted"])
                        });
                    }
                }
            }

            return summaryList;

        }

        //consolidated
        public static List<SecuredJobModel> GetCurrentMonthSecuredJobs(int yearIndex,int MonthIndex)
        {
            List<SecuredJobModel> summaryList = new List<SecuredJobModel>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMonthlySecuredJob";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MonthIndex", MonthIndex);
            cmd.Parameters.AddWithValue("@Year", yearIndex);
            cmd.Connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    summaryList.Add(new SecuredJobModel
                    {
                        Category = reader["Category"].ToString(),
                        ProjectTitle =reader["ProjectTitle"].ToString(),
                        ValueInOMR = Convert.ToDecimal(reader["QuotationValue"])
                    });
                }
            }


            return summaryList;

        }

        //secured jobs details
        public static List<SecuredJobDetailModel> GetSecuredJobsDetail(int yearIndex, int MonthIndex)
        {
            List<SecuredJobDetailModel> summaryList = new List<SecuredJobDetailModel>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetSecuredJobDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MonthIndex", MonthIndex);
            cmd.Parameters.AddWithValue("@Year", yearIndex);
            cmd.Connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    summaryList.Add(new SecuredJobDetailModel {                                 
                        SLNo = CommonFunctions.ParseInt(reader["SlNo"].ToString()),
                       JobNo = reader["ProjectNumber"].ToString(), 
                       ProjectTitle = reader["ProjectTitle"].ToString(),
                       POReference = reader["PONumber"].ToString(),
                       OrderValue = CommonFunctions.ParseDecimal(reader["OrderValue"].ToString()),
                       VAT = CommonFunctions.ParseDecimal(reader["VatAmount"].ToString()),
                       PODate = Convert.ToDateTime(reader["PODate"].ToString()),
                        POValue = CommonFunctions.ParseDecimal(reader["TotalValue"].ToString()),
                       EstimatedCost = CommonFunctions.ParseDecimal(reader["EstimatedCost"].ToString()),
                       EstimateProfit = CommonFunctions.ParseDecimal(reader["JobValue"].ToString()),
                       EstimateMargin = CommonFunctions.ParseDecimal(reader["Margin"].ToString()),
                      QuotedBy = reader["QuotedBy"].ToString()
                    });
                }
            }            

            return summaryList;

        }
        //Engineer Booking
        public static List<ChartDataModel> GetEnquiryEngineerBooking(int year,int Month)
        {
            List<ChartDataModel> summaryList = new List<ChartDataModel>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEngineerBooking";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MonthIndex", Month);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.Connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    summaryList.Add(new ChartDataModel
                    {
                        Name = reader["EmployeeName"].ToString(),
                        Y  = Convert.ToInt32(reader["ProjectValue1"]),
                        ProjectValue = Convert.ToDecimal(reader["ProjectValue1"])
                    });
                }
            }


            return summaryList;

        }


        //Sales Funner Chart by Quotation value
        public static List<QuotationStatusViewModel> GetQuotationStatusData()
        {
            var result = new List<QuotationStatusViewModel>();

            using (var conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                using (var cmd = new SqlCommand("HVAC_GetQuotationStatusSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new QuotationStatusViewModel
                            {
                                name = reader["Name"].ToString(),
                                y = Convert.ToDecimal(reader["Y"]),
                                color = reader["Color"].ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }


        //Sales Funner Chart by EquipmentValue
        public static List<QuotationStatusViewModel> GetEquipmentStatusData()
        {
            var result = new List<QuotationStatusViewModel>();

            using (var conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                using (var cmd = new SqlCommand("HVAC_GetSalesEquipmentSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new QuotationStatusViewModel
                            {
                                name = reader["Name"].ToString(),
                                y = Convert.ToDecimal(reader["Y"]),
                                color = reader["Color"].ToString()
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}