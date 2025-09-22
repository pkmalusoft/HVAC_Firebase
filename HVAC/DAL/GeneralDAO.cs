using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using AttributeRouting.Helpers;
using HVAC.Models;

namespace HVAC.DAL
{
    public class GeneralDAO
    {
        public static void SaveAuditLog(string Remarks, string ReferenceNo,string PageName)
        {
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "HVAC_SaveAuditLog";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransDate", CommonFunctions.GetCurrentDateTime());
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@LoginID", userid);
                cmd.Parameters.AddWithValue("@PageName", PageName);
                cmd.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented
                throw;
            }

        }

        public static DateTime CheckParamDate(DateTime EntryDate, int yearid)
        {
            //DateTime pFromDate = Convert.ToDateTime(EntryDate);
            //StatusModel obj = new StatusModel();
            //obj.Status = "OK";
            //obj.Message = "OK";
            //obj.ValidDate = EntryDate.ToString();
            return EntryDate;
            //try
            //{
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            //    cmd.CommandText = "HVAC_CheckDateValiate";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@EntryDate", pFromDate.ToString("MM/dd/yyyy"));
            //    cmd.Parameters.AddWithValue("@FYearId", yearid);

            //    SqlDataAdapter da = new SqlDataAdapter(cmd);
            //    DataSet ds = new DataSet();
            //    da.Fill(ds);

            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        obj.Status = ds.Tables[0].Rows[0][0].ToString();
            //        obj.Message = ds.Tables[0].Rows[0][1].ToString();
            //        obj.ValidDate = Convert.ToDateTime(ds.Tables[0].Rows[0][2].ToString()).ToString("dd-MM-yyyy");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    obj.Status = "Failed";
            //    obj.Message = ex.Message;

            //}

            //if (obj.Status != "OK")
            //{
            //    return Convert.ToDateTime(obj.ValidDate);
            //}
            //else
            //{
            //    return EntryDate;
            //}
        }
        public static StatusModel CheckDateValidate(string EntryDate, int FyearId)
        {
            StatusModel obj = new StatusModel();
            if (EntryDate != null)
            {
                DateTime pFromDate = Convert.ToDateTime(EntryDate);

                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                    cmd.CommandText = "HVAC_CheckDateValiate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EntryDate", pFromDate.ToString("MM/dd/yyyy HH:mm"));
                    cmd.Parameters.AddWithValue("@FYearId", FyearId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.Status = ds.Tables[0].Rows[0][0].ToString();
                        obj.Message = ds.Tables[0].Rows[0][1].ToString();
                        obj.ValidDate = Convert.ToDateTime(ds.Tables[0].Rows[0][2].ToString()).ToString("dd-MM-yyyy HH:mm");
                    }
                }
                catch (Exception ex)
                {
                    obj.Status = "Failed";
                    obj.Message = ex.Message;

                }
            }
            return obj;
        }
        public string SaveMenuAccess(int RoleId, string Menus)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "HVAC_SaveRoleMenuAccessRights";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@RoleId", RoleId);
                        cmd.Parameters.AddWithValue("@MenusList", Menus);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        //SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        //SqlDA.Fill(dt);
                        //if (dt.Rows.Count > 0)
                        //    MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;
        }

        public static string GetMaxEmployeeCode()
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "HVAC_GetMaxEmployeeCode";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return MaxPickUpNo;

        }

        public static void ReSaveEmployeeCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_ReSaveEmployeeCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }

        // Generate a random password of a given length (optional)  
        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        // Generate a random string with a given size and case.   
        // If second parameter is true, the return string is lowercase  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

    }
}