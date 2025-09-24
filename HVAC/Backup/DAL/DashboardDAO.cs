using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections;
using HVAC.Models;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Web.Hosting;
using System.IO;
namespace HVAC.DAL
{
    public class DashboardDAO
    {
        public static List<EnquiryVM> GetDashboardEnquiryList(int BranchId, int FYearId,int EmployeeId,int RoleID)
        {
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "HVAC_DashboardEnquiryList";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FYearId", FYearId);
                cmd.Parameters.AddWithValue("@BranchID", BranchId);            
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValue("@RoleID", RoleID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                List<EnquiryVM> objList = new List<EnquiryVM>();
                EnquiryVM obj;
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        obj = new EnquiryVM();
                            obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                        obj.EnquiryDate = CommonFunctions.ParseDate(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();
                        obj.EnquiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EnquiryDate"].ToString());
                        obj.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                        obj.ProjectDescription = ds.Tables[0].Rows[i]["ProjectDescription"].ToString();
                        obj.EnqStageName = ds.Tables[0].Rows[i]["EnqStageName"].ToString();
                        obj.EnquiryStatus = ds.Tables[0].Rows[i]["EnqStatusName"].ToString();
                        obj.PriorityName = ds.Tables[0].Rows[i]["PriorityName"].ToString();
                        obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                        obj.CityName = ds.Tables[0].Rows[i]["City"].ToString();
                        obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                        obj.ProjectPrefix = ds.Tables[0].Rows[i]["ProjectPrefix"].ToString();
                        obj.CountryName = ds.Tables[0].Rows[i]["CountryName"].ToString();
                        obj.AssignedEmps = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                        objList.Add(obj);
                    }
                }
                return objList;
            }
        }

        public static DashboardViewModel GetDashboardSummary(int BranchId, int FYearId, int EmployeeId, int RoleID)
        {
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            //cmd.CommandText = "HVAC_GetDashboardStats";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@FYearId", FYearId);
            //cmd.Parameters.AddWithValue("@BranchID", BranchId);
            //cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            //cmd.Parameters.AddWithValue("@RoleID", RoleID);
            var stats = new DashboardViewModel();
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand("HVAC_GetDashboardStats", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FYearId", FYearId);
                cmd.Parameters.AddWithValue("@BranchID", BranchId);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValue("@RoleID", RoleID);
                cmd.Parameters.AddWithValue("@UserID",1);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats.TotalEnquiry = reader["TotalEnquiries"] == DBNull.Value ? 0 : (int)Convert.ToInt32(reader["TotalEnquiries"]); 
                        stats.TotalOrders = reader["TotalJob"] == DBNull.Value ? 0 : (int)Convert.ToInt32(reader["TotalJob"]);
                        stats.TotalJobvalue = reader["TotalJobvalue"] == DBNull.Value ? 0 : (decimal)Convert.ToDecimal(reader["TotalJobvalue"]);
                        stats.TotalMargin = reader["TotalMargin"] == DBNull.Value ? 0 : (decimal)Convert.ToDecimal(reader["TotalMargin"]);
                        stats.TotalJobCost = reader["TotalJobCost"] == DBNull.Value ? 0 : (decimal)Convert.ToDecimal(reader["TotalJobCost"]);
                        stats.NewMessages = reader["NewMessages"] == DBNull.Value ? 0 : (int)Convert.ToDecimal(reader["NewMessages"]);
                        
                    }
                }
            }

            return stats;
        }
        public static string  DashboardReprocess(int BranchId, int FyearID,int UserId)
        {

            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "SP_DashboardChart";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BranchId", BranchId); 
                cmd.Parameters.AddWithValue("@FYearId", FyearID);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@ProcessDate", CommonFunctions.GetCurrentDateTime());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                 

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count>0)
                    {
                        return "OK";
                    }
                }
                return "OK";

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}