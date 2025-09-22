using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HVAC.Models;
//using System.Data.SqlClient;
using System.Data; 
using System.Data.SqlClient;
using System.Text;
using System.Configuration;


namespace HVAC.DAL
{
    public class JobDAO
    {
        HVACEntities Context1 = new HVACEntities();

        //DateTimeZoneConversionModel DTZC = new DateTimeZoneConversionModel();

     
        public string ConvertDateTimeZone(DateTime Userdate)
        {
            DateTime serverDateTime = Userdate;

            DateTime dbDateTime = serverDateTime.ToUniversalTime();

            DateTimeOffset dbDateTimeOffset = new DateTimeOffset(dbDateTime, TimeSpan.Zero);

            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");

            DateTimeOffset userDateTimeOffset = TimeZoneInfo.ConvertTime(dbDateTimeOffset, userTimeZone);

            string userDateTimeString = userDateTimeOffset.ToString("dd-MM-yyyy HH:mm:ss");

            return userDateTimeString;

        }
       
        public static void SaveAuditLog(string Remarks,int JobId)
        {
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "SP_SaveAuditLog";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransDate", CommonFunctions.GetCurrentDateTime());
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@LoginID", userid);
                cmd.Parameters.AddWithValue("@JobID", JobId);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
             
        }

        public static string GetMaxJobEnquiryNo(DateTime EnquiryDate, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxJOBEnquiryNo";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@EnquiryDate", Convert.ToDateTime(EnquiryDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static List<string> GetRevenueGroupList()
        {
             
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
         
            cmd.CommandText = "select distinct rtrim(Isnull(RevenueGroup,''))  from RevenueType  where Isnull(RevenueGroup,'')<>''";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<string> objList = new List<string>();

            if (ds != null && ds.Tables.Count > 0)
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objList.Add(ds.Tables[0].Rows[i][0].ToString());
                }

            }
            return objList;
        }


        //used in the packing master list 
        public static List<string> GetPackingItemGroupList()
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);

            cmd.CommandText = "select distinct rtrim(Isnull(GroupName,''))  from PackingListMaster  where Isnull(GroupName,'')<>''";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<string> objList = new List<string>();

            if (ds != null && ds.Tables.Count > 0)
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objList.Add(ds.Tables[0].Rows[i][0].ToString());
                }

            }
            return objList;
        }
        public static string GetMaxJobQuotationNo(int BranchId, int FyearId, int JobId,int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxQuotationNo";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@JobId", JobId);
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString() + '-' + ds.Tables[0].Rows[0][1].ToString();
            }
            else
            {
                return "";
            }

        }



        public static List<EntityType> GetEntityType(int EnquiryID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetEntityType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            //cmd.Parameters.AddWithValue("@AllOption", AllOption);
            //if (FromDate == null)
            //    cmd.Parameters.AddWithValue("@FromDate", "");
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(FromDate).ToString("MM/dd/yyyy"));
            //if (ToDate == null)
            //    cmd.Parameters.AddWithValue("@ToDate", "");
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(ToDate).ToString("MM/dd/yyyy"));

            //cmd.Parameters.AddWithValue("@FYearId", FYearId);
            //cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
            //cmd.Parameters.AddWithValue("@JobIDs", JobIDs);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EntityType> objList = new List<EntityType>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EntityType obj = new EntityType();
                    obj.EntityTypeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EntityTypeID"].ToString());

                    obj.EntityTypeName =(ds.Tables[0].Rows[i]["EntityTypeName"].ToString());
                    //obj.JOBDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["JobDate"].ToString()).ToString("dd-MM-yyyy");
                    //obj.JOBCode = ds.Tables[0].Rows[i]["JobCode"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;

        }


    

        public static string GetMaxJobNo(int JobTypeId,DateTime JobDate,int BranchId,int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxJOBNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobTypeId", JobTypeId);
            cmd.Parameters.AddWithValue("@JobDate", Convert.ToDateTime(JobDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);            
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static string GetMaxJobTDNNo(int LoadPortID, int DestinationPortID, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetJobMAXTDN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LoadPortID", LoadPortID);
            cmd.Parameters.AddWithValue("@DestinationPortID", DestinationPortID);
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static bool CheckCustomerNameExist(string CustomerName, int CustomerId = 0)
        {
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                if (CustomerId > 0)
                {
                    cmd.CommandText = "select CustomerName from CustomerMaster where lower(rtrim(Isnull(CustomerName,''))) =@CustomerName and CustomerId<>@CustomerId";
                    cmd.Parameters.AddWithValue("@CustomerName", CustomerName.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                }
                else
                {
                    cmd.CommandText = "select CustomerName from CustomerMaster where lower(rtrim(Isnull(CustomerName,''))) =@CustomerName";
                    cmd.Parameters.AddWithValue("@CustomerName", CustomerName.Trim().ToLower());
                }
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                List<CountryMasterVM> objList = new List<CountryMasterVM>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static List<DocumentMasterVM> GetMCDocument(int JobId)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMCDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MCID", JobId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<DocumentMasterVM> objList = new List<DocumentMasterVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocumentMasterVM obj = new DocumentMasterVM();
                    obj.DocumentID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["DocumentID"].ToString());
                    obj.DocumentTitle = ds.Tables[0].Rows[i]["DocumentTitle"].ToString();
                    obj.DocumentTypeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["DocumentTypeID"].ToString());
                    //obj.MCID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["MCID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<DocumentMasterVM> GetJOBDocument(int EnquiryId,int JobId)
        {
            string url=ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetJOBDocument";
            cmd.CommandType = CommandType.StoredProcedure;
           // cmd.Parameters.AddWithValue("@EnquiryId", EnquiryId);
            cmd.Parameters.AddWithValue("@JobID", JobId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<DocumentMasterVM> objList = new List<DocumentMasterVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocumentMasterVM obj = new DocumentMasterVM();
                    obj.DocumentID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["DocumentID"].ToString());
                    obj.DocumentTitle = ds.Tables[0].Rows[i]["DocumentTitle"].ToString();
                    obj.DocumentTypeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["DocumentTypeID"].ToString());
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url +  obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }
        
    

        #region "Password generate"
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

        #endregion

       


      

   


    
    }
}