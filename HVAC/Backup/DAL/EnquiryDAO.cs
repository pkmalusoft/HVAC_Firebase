using HVAC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HVAC.DAL
{
    public class EnquiryDAO
    {
        public static List<EnquiryVM> EnquiryList(DateTime FromDate, DateTime ToDate, string EnquiryNo, int FyearId,int EmployeeId,int RoleID)
        {
            int branchid = HttpContext.Current?.Session?["CurrentBranchID"] != null ? Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString()) : 0;
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "HVAC_GetEnquiryList";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@FYearId", FyearId);
                            

                if (EnquiryNo == null)
                    EnquiryNo = "";
                cmd.Parameters.AddWithValue("@EnquiryNo", EnquiryNo);

                cmd.Parameters.AddWithValue("@BranchID", branchid);
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValue("@RoleID",RoleID);

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
                        obj.EnquiryID= Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
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
                        obj.ProjectPrefix = ds.Tables[0].Rows[i]["ProjectPrefix"].ToString();
                        obj.CountryName = ds.Tables[0].Rows[i]["CountryName"].ToString();
                        objList.Add(obj);
                    }
                }
                return objList;
            }
        }


        public static List<EstimationVM> EstimationList(DateTime FromDate, DateTime ToDate, string EnquiryNo, string EstimationNo, int EmployeeID, int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEstimationList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            if (EnquiryNo == null)
                EnquiryNo = "";


            cmd.Parameters.AddWithValue("@EnquiryNo", EnquiryNo);

            if (EstimationNo == null)
                @EstimationNo = "";

            cmd.Parameters.AddWithValue("@EstimationNo", EstimationNo);            
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EstimationVM> objList = new List<EstimationVM>();
            EstimationVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EstimationVM();
                    obj.EstimationID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.EstimationDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EstimationDate"].ToString());
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                    obj.VarNo = Convert.ToInt32(ds.Tables[0].Rows[i]["VarNo"].ToString());                    
                    obj.SellingValue = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["SellingValue"].ToString());
                    obj.TotalLandingCostOMR= CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TotalLandingCostOMR"].ToString());
                    
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();                    
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNo"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<EstimationDetailVM> EstimationEquipment(int EnquiryID, int EstimationID = 0)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEstimationDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@EstimationID", EstimationID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EstimationDetailVM> objList = new List<EstimationDetailVM>();
            EstimationDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EstimationDetailVM();
                    obj.EstimationDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationDetailID"].ToString());
                    obj.EstimationID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.EstimationCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationCategoryID"].ToString());
                    obj.EstimationMasterID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationMasterID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.CategoryName = ds.Tables[0].Rows[i]["CategoryName"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.UnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.CurrencyCode = ds.Tables[0].Rows[i]["CurrencyCode"].ToString();
                    obj.CurrencyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CurrencyID"].ToString());
                    obj.Qty = Convert.ToDecimal(ds.Tables[0].Rows[i]["Qty"].ToString());
                    obj.ExchangeRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["ExchangeRate"].ToString());
                    obj.Rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.FValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["FValue"].ToString());
                    obj.LValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["LValue"].ToString());
                    obj.Deleted = false;
                    obj.AutoCalc= Convert.ToBoolean(ds.Tables[0].Rows[i]["AutoCalc"].ToString());
                    obj.RowType = "false";
                    obj.Roworder = Convert.ToInt32(obj.EstimationCategoryID);
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<EstimationDetailVM> EstimationEquipmentSellingRate(int EnquiryID, int EstimationID = 0)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEstimationSellingDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@EstimationID", EstimationID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EstimationDetailVM> objList = new List<EstimationDetailVM>();
            EstimationDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EstimationDetailVM();
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.EstimationDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationDetailID"].ToString());
                    obj.EstimationID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.EstimationCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationCategoryID"].ToString());
                    obj.EstimationMasterID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationMasterID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.CategoryName = ds.Tables[0].Rows[i]["CategoryName"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.UnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.CurrencyCode = ds.Tables[0].Rows[i]["CurrencyCode"].ToString();
                    obj.CurrencyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CurrencyID"].ToString());
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Qty"].ToString());
                    obj.ExchangeRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["ExchangeRate"].ToString());
                    obj.UnitRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["SellingUnitRate"].ToString());
                    
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["LValue"].ToString());
                    obj.Deleted = false;
                    obj.AutoCalc = Convert.ToBoolean(ds.Tables[0].Rows[i]["AutoCalc"].ToString());
                    obj.RowType = "false";
                    obj.Roworder = Convert.ToInt32(obj.EstimationCategoryID);
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<EnquiryClientVM> EnquiryClient(int EnquiryID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryClientDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
             
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EnquiryClientVM> objList = new List<EnquiryClientVM>();
            EnquiryClientVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EnquiryClientVM();
                    obj.EnquiryClientID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryClientID"].ToString());
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.ClientID = Convert.ToInt32(ds.Tables[0].Rows[i]["ClientID"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.ClientType = ds.Tables[0].Rows[i]["ClientType"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<EnquiryEmployeeVM> EnquiryEmployee(int EnquiryID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryEmployeeDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EnquiryEmployeeVM> objList = new List<EnquiryEmployeeVM>();
            EnquiryEmployeeVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EnquiryEmployeeVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EmployeeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.EmployeeShortName = ds.Tables[0].Rows[i]["EmployeeShortName"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        //to bind equipment type autocomplete
        public static List<EnquiryEquipmentVM> GetEquipmentType(int BrandID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEquipmentType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BrandID", BrandID);
         
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EnquiryEquipmentVM> objList = new List<EnquiryEquipmentVM>();
            EnquiryEquipmentVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EnquiryEquipmentVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    
                    obj.ProductFamilyID = Convert.ToInt32(ds.Tables[0].Rows[i]["ProductFamilyID"].ToString());
                    
                    obj.EquipmentType = ds.Tables[0].Rows[i]["EquipmentType"].ToString();
                    obj.ProductFamilyName = ds.Tables[0].Rows[i]["ProductFamilyName"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<QuotationDetailVM> QuotationEquipment(int EnquiryID, int QuotationId = 0)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetQuotationEquipment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@QuotationId", QuotationId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationDetailVM> objList = new List<QuotationDetailVM>();
            QuotationDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationDetailVM();
                    obj.QuotationDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationDetailID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EstimationDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationDetailID"].ToString());
                    obj.EstimationID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    //obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EstimationCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationCategoryID"].ToString());
                    obj.EstimationMasterID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationMasterID"].ToString());
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.CategoryName = ds.Tables[0].Rows[i]["CategoryName"].ToString();
                    obj.UnitName= ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.UnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    if (ds.Tables[0].Rows[i]["UnitRate"] != DBNull.Value &&
                        !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[i]["UnitRate"].ToString()))
                    {
                        obj.UnitRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["UnitRate"]);
                    }
                    else
                    {
                        obj.UnitRate = 0; // or assign null if UnitRate is nullable decimal
                    }
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                   // obj.EquipmentStatus = ds.Tables[0].Rows[i]["EquipmentStatus"].ToString();
                    //obj.CreatedBy = ds.Tables[0].Rows[i]["CreatedBy"].ToString();
                    //obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    //obj.NominalCapacity = ds.Tables[0].Rows[i]["NominalCapacity"].ToString();
                    //obj.EfficientType = ds.Tables[0].Rows[i]["EfficientType"].ToString();
                    //obj.Refrigerant = Convert.ToBoolean(ds.Tables[0].Rows[i]["Refrigerant"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<EnquiryEquipmentVM> EnquiryEquipment(int EnquiryID,int QuotationId=0)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryEquipment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@QuotationId", QuotationId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EnquiryEquipmentVM> objList = new List<EnquiryEquipmentVM>();
            EnquiryEquipmentVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EnquiryEquipmentVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.ProductFamilyID = Convert.ToInt32(ds.Tables[0].Rows[i]["ProductFamilyID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.CategoryName = ds.Tables[0].Rows[i]["EquipmentType"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    
                    obj.Brand = ds.Tables[0].Rows[i]["Brand"].ToString();
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    
                    obj.Quantity = Convert.ToInt32(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.UnitRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["UnitRate"].ToString());
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obj.EquipmentStatus = ds.Tables[0].Rows[i]["EquipmentStatus"].ToString();
                    obj.CreatedBy = ds.Tables[0].Rows[i]["CreatedBy"].ToString();
                    obj.CreatedDate =Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    obj.NominalCapacity = ds.Tables[0].Rows[i]["NominalCapacity"].ToString();
                    obj.EfficientType = ds.Tables[0].Rows[i]["EfficientType"].ToString();
                    obj.Refrigerant =Convert.ToBoolean(ds.Tables[0].Rows[i]["Refrigerant"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<QuotationDetailVM> ClientPOEquipment(int QuotationId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetClientPoEstimationDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationId", QuotationId);
             
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationDetailVM> objList = new List<QuotationDetailVM>();
            QuotationDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationDetailVM();
                    obj.QuotationDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationDetailID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    //obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EstimationCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationCategoryID"].ToString());
                    obj.EstimationMasterID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationMasterID"].ToString());
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.CategoryName = ds.Tables[0].Rows[i]["CategoryName"].ToString();
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.UnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.UnitRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["UnitRate"].ToString());
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

      
        public static List<QuotationVM> EnquiryQuotation(int EnquiryID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryQuotation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationVM> objList = new List<QuotationVM>();
            QuotationVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationVM();
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EngineerID = Convert.ToInt32(ds.Tables[0].Rows[i]["EngineerID"].ToString());
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.QuotationNo = ds.Tables[0].Rows[i]["QuotationNo"].ToString();
                    obj.Version = Convert.ToInt32(ds.Tables[0].Rows[i]["Version"].ToString());
                    obj.QuotationDate =Convert.ToDateTime(ds.Tables[0].Rows[i]["QuotationDate"].ToString());
                    obj.QuotationValue = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["QuotationValue"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.QuotationStatus = ds.Tables[0].Rows[i]["Status"].ToString();                    
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    obj.QuoteClientLocation = "";
                    string _quotationclient = "";
                    for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                    {
                        if (ds.Tables[1].Rows[j][0].ToString() == obj.QuotationID.ToString())
                        {
                            if (_quotationclient == "")
                                _quotationclient = _quotationclient + ds.Tables[1].Rows[j][1].ToString();
                            else
                                _quotationclient = _quotationclient + "," + ds.Tables[1].Rows[j][1].ToString();

                        }
                         
                    }
                    obj.QuoteClientDetail = _quotationclient;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<AuditLogVM> EnquiryLog(string EnquiryNo)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryLog";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryNo", EnquiryNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AuditLogVM> objList = new List<AuditLogVM>();
            AuditLogVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AuditLogVM();
                    obj.AuditLogID = Convert.ToInt32(ds.Tables[0].Rows[i]["AuditLogID"].ToString());                    
                    obj.ReferenceNo = ds.Tables[0].Rows[i]["ReferenceNo"].ToString();
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    obj.UserName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.EmployeePrefix = ds.Tables[0].Rows[i]["EmployeePrefix"].ToString();
                    
                    obj.TransDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransDate"].ToString());                                       
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<DropdownVM> GetDropdownData(string MasterName,string term)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_MasterData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MasterName", MasterName);
            cmd.Parameters.AddWithValue("@term", term);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<DropdownVM> objList = new List<DropdownVM>();
            DataTable dt = ds.Tables[0];
            string json = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
            objList = JsonConvert.DeserializeObject<List<DropdownVM>>(json);

            return objList;

        }

        public static List<DocumentMasterVM> GetEnquiryDocument(int EnquiryId)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryId);
           
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
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.UserName = ds.Tables[0].Rows[i]["UserName"].ToString();
                    obj.DocumentLink = ds.Tables[0].Rows[i]["DocumentLink"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    obj.UserName = ds.Tables[0].Rows[i]["UserName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static EnquiryPrintVM GetEnquiryPrintData(int EnquiryId)
        {
            using (var conn = new SqlConnection(CommonFunctions.GetConnectionString))
            using (var cmd = new SqlCommand("GetEnquiryPrintData", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EnquiryId", EnquiryId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                var enquiry = new EnquiryPrintVM();

                // ✅ First Result set: Enquiry Details
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    enquiry.EnquiryNo = row["EnquiryNo"].ToString();
                    enquiry.EnquiryDate = row["EnquiryDate"] as DateTime?;
                    enquiry.DueDate = row["DueDate"] as DateTime?;
                    enquiry.DueDays = row["DueDays"] as int?;
                    enquiry.EnquiryStage = row["EnquiryStage"].ToString();
                    enquiry.Prefix = row["Prefix"].ToString();
                    enquiry.ProjectName = row["ProjectName"].ToString();
                    enquiry.ProjectDetails = row["ProjectDetails"].ToString();
                    enquiry.ProjectLocation = row["ProjectLocation"].ToString();
                }

                // ✅ Second Result set: Items
                enquiry.Items = new List<EnquiryItemVM>();
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        enquiry.Items.Add(new EnquiryItemVM
                        {
                            EquipmentType = row["EquipmentType"].ToString(),
                            EquipmentName = row["EquipmentName"].ToString(),
                            Model = row["Model"].ToString(),
                            Description = row["Description"].ToString(),
                            Qty = row["Qty"] as decimal?,
                            Unit = row["Unit"].ToString(),
                            UnitPrice = row["UnitPrice"] as decimal?
                        });
                    }
                }

                // ✅ Third Result set: Employees
                enquiry.AssignedToNames = new List<string>();
                if (ds.Tables.Count > 2)
                {
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        enquiry.AssignedToNames.Add(row["employeename"].ToString());
                    }
                }

                return enquiry;
            }
        }

        public static Quotation GetMaxEstimationNo(int BranchId, int FyearId, int EnquiryID, int EstimationID, int EmployeeID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMaxEstimationNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@EstimationId", EstimationID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();
                _obj.Version = Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";
                _obj.Version = 1;
                return _obj;
            }

        }

        public static Quotation GetMaxJobQuotationNo(int BranchId, int FyearId, int EnquiryID, int QuotationID,int EmployeeID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMaxQuotationNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            SqlDataAdapter da = new SqlDataAdapter(cmd); 
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();
                _obj.Version = Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";
                _obj.Version = 1;
                return _obj;
            }

        }

        public static Quotation GetJobMaxPONo(int JobID, int BranchId, int FyearId) 
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetJobMaxPONo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobID", JobID);
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();
                _obj.Version =Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";
                _obj.Version = 1;
                return _obj;
            }

        }

        public static Quotation GetSupplierMaxPONo(int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetSupplierMaxPONo";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();
                _obj.Version = Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";
                _obj.Version = 1;
                return _obj;
            }

        }
        public static Quotation GetMaxGRNCode(int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetGRNMaxNo"; // updated SP name
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0]["GRNNo"].ToString();
                _obj.Version = 1; // No versioning in GRN, setting default value
            }
            else
            {
                _obj.QuotationNo = "";
                _obj.Version = 1;
            }

            return _obj;
        }



        public static Quotation GetMRMaxNo(int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMaterialRequestNo";
            cmd.CommandType = CommandType.StoredProcedure;
           
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();
           
                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";
              
                return _obj;
            }

        }
        public static Quotation GetMaterialIssueMAxNo(int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMaterialIssueNo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Quotation _obj = new Quotation();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _obj.QuotationNo = ds.Tables[0].Rows[0][0].ToString();

                return _obj;
            }
            else
            {
                _obj.QuotationNo = "";

                return _obj;
            }

        }
        public static List<EmployeeVM> GetEmployeesList()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEmployeesList";
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            //cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EmployeeVM> objList = new List<EmployeeVM>();
            EmployeeVM _obj = new EmployeeVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EmployeeVM obj = new EmployeeVM();
                    obj.EmployeeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.EmployeePrefix = ds.Tables[0].Rows[i]["EmployeePrefix"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;

        }
        public static List<EmployeeVM> GetEnquiryAssigneEmployees(int EnquiryID,int EmployeeID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryAssignedEmployees";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EmployeeVM> objList = new List<EmployeeVM>();
            EmployeeVM _obj = new EmployeeVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EmployeeVM obj = new EmployeeVM();
                    obj.EmployeeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.EmployeePrefix = ds.Tables[0].Rows[i]["EmployeePrefix"].ToString();                    
                    objList.Add(obj);
                }
            }
            return objList;

        }
        public static List<EmployeeVM> GetPOEmployees(int PurchaseOrderID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetPOApproveEmployees";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EmployeeVM> objList = new List<EmployeeVM>();
            EmployeeVM _obj = new EmployeeVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EmployeeVM obj = new EmployeeVM();
                    obj.EmployeeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.EmployeePrefix = ds.Tables[0].Rows[i]["EmployeePrefix"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;

        }

        public static StatusModel SavePOApprover(POApproverVM obj)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_SavePOApprover";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderId", obj.PurchaseOrderID);
            cmd.Parameters.AddWithValue("@EmployeeID", obj.EmployeeID);
            cmd.Parameters.AddWithValue("@ApproveType", obj.Type);
            cmd.Parameters.AddWithValue("@ValidateText", obj.ValidateText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            StatusModel result = new StatusModel();
            if (ds != null && ds.Tables.Count > 0)

            {
                 if (ds.Tables[0].Rows.Count>0)
                {
                    result.Status = ds.Tables[0].Rows[0]["Status"].ToString();
                    result.Message= ds.Tables[0].Rows[0]["Message"].ToString();                 
                }
            }
            
            return result;

        }
        public static List<EnquiryVM> GetEmployeeEnquiry(int EmployeeID,int BranchId,int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEmployeeEnquiry";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearId", FyearId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EnquiryVM> objList = new List<EnquiryVM> ();
            EnquiryVM _obj = new EnquiryVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EnquiryVM obj = new EnquiryVM();
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();
                    
                    objList.Add(obj);
                }
            }
            return objList;

        }

        public static List<EnquiryVM> GetProjectNo(string term,int EmployeeID, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetProjectNo";
            cmd.CommandType = CommandType.StoredProcedure;
            if (term == null)
                term = "";
            cmd.Parameters.AddWithValue("@term", term);
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearId", FyearId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EnquiryVM> objList = new List<EnquiryVM>();
            EnquiryVM _obj = new EnquiryVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EnquiryVM obj = new EnquiryVM();
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.JobHandOverID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectTitle"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;

        }
        public static List<EnquiryVM> GetEstimationEnquiry(int EmployeeID, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEnquiryEstimation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearId", FyearId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EnquiryVM> objList = new List<EnquiryVM>();
            EnquiryVM _obj = new EnquiryVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EnquiryVM obj = new EnquiryVM();
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;

        }
        public static List<EnquiryVM> GetEmployeeProject(int EmployeeID, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetEmployeeProject";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearId", FyearId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EnquiryVM> objList = new List<EnquiryVM>();
            EnquiryVM _obj = new EnquiryVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EnquiryVM obj = new EnquiryVM();
                    obj.EnquiryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.JobHandOverID= CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    //obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();
                    obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectTitle"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;

        }

        public static List<EnquiryEquipmentVM> GetEmployeeProjectEquipment(int EmployeeID, int ProjectID, int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetProjectEquipment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearId", FyearId);
            cmd.Parameters.AddWithValue("@ProjectId", ProjectID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<EnquiryEquipmentVM> objList = new List<EnquiryEquipmentVM>();
            EnquiryEquipmentVM _obj = new EnquiryEquipmentVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EnquiryEquipmentVM obj = new EnquiryEquipmentVM();
                    obj.ID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    
                    
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.Brand = ds.Tables[0].Rows[i]["Brand"].ToString();
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;

        }

        public static List<QuotationVM> QuotationList(DateTime FromDate, DateTime ToDate, string EnquiryNo, string QuotationNo,int EmployeeID,int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetQuotationList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            if (EnquiryNo == null)
                EnquiryNo = "";
            

            cmd.Parameters.AddWithValue("@EnquiryNo",EnquiryNo);

            if (QuotationNo == null)
                QuotationNo = "";

            cmd.Parameters.AddWithValue("@QuotationNo", QuotationNo);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);
 
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationVM> objList = new List<QuotationVM>();
            QuotationVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationVM();
                    obj.EnquiryID = Convert.ToInt32(ds.Tables[0].Rows[i]["EnquiryID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.QuotationNo = ds.Tables[0].Rows[i]["QuotationNo"].ToString();
                    obj.QuotationDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["QuotationDate"].ToString());
                    obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();                    
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                    obj.Version = Convert.ToInt32(ds.Tables[0].Rows[i]["Version"].ToString());
                    obj.QuotationStatus = ds.Tables[0].Rows[i]["QuotationStatus"].ToString();
                    obj.QuotationValue = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["QuotationValue"].ToString());
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.CityName = ds.Tables[0].Rows[i]["City"].ToString();
                    obj.CountryName = ds.Tables[0].Rows[i]["CountryName"].ToString();
                    obj.PONo = ds.Tables[0].Rows[i]["PONo"].ToString();
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNo"].ToString();
                    obj.JobHandOverID =Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.PurchaseOrderDetailId = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderDetailId"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<JobHandOverVM> JobHandOverList(DateTime FromDate, DateTime ToDate, string EnquiryNo, string ProjectNo, int EmployeeID, int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_JobHandOverList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            if (ProjectNo == null)
                ProjectNo = "";

            cmd.Parameters.AddWithValue("@ProjectNo", ProjectNo);
            
            if (EnquiryNo == null)
                EnquiryNo = "";

            cmd.Parameters.AddWithValue("@EnquiryNo", EnquiryNo);

           
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobHandOverVM> objList = new List<JobHandOverVM>();
            JobHandOverVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new JobHandOverVM();
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.JobDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["JobDate"].ToString());
                    obj.EnquiryNo = ds.Tables[0].Rows[i]["EnquiryNo"].ToString();
                    obj.ProjectTitle = ds.Tables[0].Rows[i]["ProjectTitle"].ToString();
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.JobValue = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["JobValue"].ToString());
                    obj.Margin = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Margin"].ToString());
                    obj.JobCost = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["JobCost"].ToString());
                    obj.VatAmount = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["VatAmount"].ToString());
                    obj.TotalValue = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TotalValue"].ToString());
                    //obj.Job = Convert.ToDecimal(ds.Tables[0].Rows[i]["JobValue"].ToString());

                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<JobPurchaseOrderVM> JobInwardPOList(DateTime FromDate, DateTime ToDate, string EnquiryNo, string ProjectNo, int EmployeeID, int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_JobInwardPOList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            if (ProjectNo == null)
                ProjectNo = "";

            cmd.Parameters.AddWithValue("@ProjectNo", ProjectNo);

            if (EnquiryNo == null)
                EnquiryNo = "";

            cmd.Parameters.AddWithValue("@PONo", EnquiryNo);


            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobPurchaseOrderVM> objList = new List<JobPurchaseOrderVM>();
            JobPurchaseOrderVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new JobPurchaseOrderVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.VarNo =Convert.ToInt32(ds.Tables[0].Rows[i]["VarNo"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.PONumber = ds.Tables[0].Rows[i]["PONumber"].ToString();
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectTitle"].ToString();
                    obj.PODate = Convert.ToDateTime(ds.Tables[0].Rows[i]["PODate"].ToString());
                    obj.CreatedByName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.TotalValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalValue"].ToString());
                    obj.MRequestNo = ds.Tables[0].Rows[i]["MRequestNo"].ToString();
                    obj.MRequestID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["MRequestID"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        
        //for jobhandover print details
        public static List<JobPurchaseOrderVM> JobwisePOList(int JobId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_JobwiseClientPO";
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@JobID", JobId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobPurchaseOrderVM> objList = new List<JobPurchaseOrderVM>();
            JobPurchaseOrderVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new JobPurchaseOrderVM();
                    obj.SqNo = i + 1;
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ProjectNumber = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.VarNo = Convert.ToInt32(ds.Tables[0].Rows[i]["VarNo"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.PONumber = ds.Tables[0].Rows[i]["PONumber"].ToString();
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectTitle"].ToString();
                    obj.PODate = Convert.ToDateTime(ds.Tables[0].Rows[i]["PODate"].ToString());
                    obj.CreatedByName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.OrderValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["OrderValue"].ToString());
                    obj.TotalValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalValue"].ToString());
                    obj.VatAmount = Convert.ToDecimal(ds.Tables[0].Rows[i]["VatAmount"].ToString());
                    obj.VatPercent = Convert.ToDecimal(ds.Tables[0].Rows[i]["VatPercent"].ToString());
                    obj.MRequestNo = ds.Tables[0].Rows[i]["MRequestNo"].ToString();
                    obj.MRequestID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["MRequestID"].ToString());
                    obj.QuotationId = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["QuotationId"].ToString());
                    obj.QuotationNo = ds.Tables[0].Rows[i]["QuotationNo"].ToString();
                    obj.QuotationDate =Convert.ToDateTime(ds.Tables[0].Rows[i]["QuotationDate"].ToString());
                    obj.CreatedByName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<JobBondVM> GetJobInwardBondList(int POID,int JobHandoverID )
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetClientPoBond";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@POID", POID);
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandoverID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobBondVM> objList = new List<JobBondVM>();
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var obj = new JobBondVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.BondTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["BondTypeID"].ToString());
                    obj.BondName = ds.Tables[0].Rows[i]["BondType"].ToString();
                    obj.SalesValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["SalesValue"].ToString());
                    obj.Percentage= Convert.ToDecimal(ds.Tables[0].Rows[i]["Percentage"].ToString());
                    obj.BondValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["BondValue"].ToString());
                    obj.BondValidity = Convert.ToInt32(ds.Tables[0].Rows[i]["BondValidity"].ToString());
                    obj.BondExpiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BondExpiryDate"].ToString());
                    obj.BondIssueDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BondIssueDate"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        //job handover detail tabs
        public static List<JobPurchaseOrderVM> GetJobPoList(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetJOBPODetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobPurchaseOrderVM> objList = new List<JobPurchaseOrderVM>();
            JobPurchaseOrderVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new JobPurchaseOrderVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.PONumber = ds.Tables[0].Rows[i]["PONumber"].ToString();
                    obj.QuotationNo = ds.Tables[0].Rows[i]["QuotationNo"].ToString();
                    obj.QuotationId = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationId"].ToString());
                    obj.PODate = Convert.ToDateTime(ds.Tables[0].Rows[i]["PODate"].ToString());
                    obj.OrderValue =Convert.ToDecimal( ds.Tables[0].Rows[i]["OrderValue"].ToString());
                    obj.VatAmount = Convert.ToDecimal(ds.Tables[0].Rows[i]["VatAmount"].ToString());
                    obj.TotalValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalValue"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<JobBondVM> GetJobBondList(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetClientPoBond";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@POID", 0);
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobBondVM> objList = new List<JobBondVM>();
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var obj = new JobBondVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.BondTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["BondTypeID"].ToString());
                    obj.BondName = ds.Tables[0].Rows[i]["BondType"].ToString();
                    obj.SalesValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["SalesValue"].ToString());
                    obj.Percentage = Convert.ToDecimal(ds.Tables[0].Rows[i]["Percentage"].ToString());
                    obj.BondValue = Convert.ToDecimal(ds.Tables[0].Rows[i]["BondValue"].ToString());
                    obj.BondValidity = Convert.ToInt32(ds.Tables[0].Rows[i]["BondValidity"].ToString());
                    obj.BondExpiryDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BondExpiryDate"].ToString());
                    obj.BondIssueDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BondIssueDate"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<QuotationWarrantyVM> GetJobWarrantyList(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetJOBWarrantyDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationWarrantyVM> objList = new List<QuotationWarrantyVM>();
            QuotationWarrantyVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationWarrantyVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.WarrantyType = ds.Tables[0].Rows[i]["WarrantyType"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.Checked = true;
                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static List<JobPaymentVM> GetJobPaymentList(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetJOBPaymentDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobPaymentVM> objList = new List<JobPaymentVM>();
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var obj = new JobPaymentVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.PaymentID = Convert.ToInt32(ds.Tables[0].Rows[i]["PaymentID"].ToString());
                    obj.PaymentInstrument = ds.Tables[0].Rows[i]["PaymentInstrument"].ToString();
                    obj.Terms = ds.Tables[0].Rows[i]["Terms"].ToString();
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obj.DueDays = Convert.ToInt32(ds.Tables[0].Rows[i]["DueDays"].ToString());                    
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static string GetMaxEnquiryNo(int BranchId, int FyearId, int EnquiryId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetMaxEnquiryNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@JobId", 0);
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //return ds.Tables[0].Rows[0][0].ToString() + '-' + ds.Tables[0].Rows[0][1].ToString();
                return ds.Tables[0].Rows[0][0].ToString();

            }
            else
            {
                return "";
            }

        }

        public static StatusModel GenerateJobConfirm(int BranchId, int FyearId, int EnquiryId,int CreatedBy)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GenerateJobConfirm";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryID", EnquiryId);
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            StatusModel _model = new StatusModel();
            if (ds.Tables[0].Rows.Count > 0)
            {
                _model.Status = ds.Tables[0].Rows[0][0].ToString();
                _model.Message = ds.Tables[0].Rows[0][1].ToString();
                return _model;

            }
            else
            {
                _model.Status = "Failed";
                _model.Message = "Error";
                return _model;
            }

        }



        public static List<MaterialRequestVM> MaterialRequestList(DateTime FromDate, DateTime ToDate, string MRNo, string ProjectNo, int EmployeeID, int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_MaterialRequestList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            if (ProjectNo == null)
                ProjectNo = "";

            cmd.Parameters.AddWithValue("@ProjectNo", ProjectNo);

            if (MRNo == null)
                MRNo = "";

            cmd.Parameters.AddWithValue("@MRNo", MRNo);


            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialRequestVM> objList = new List<MaterialRequestVM>();
            MaterialRequestVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialRequestVM();
                    
                    // Safe conversion for MRequestID
                    string mRequestIDStr = ds.Tables[0].Rows[i]["MRequestID"]?.ToString();
                    obj.MRequestID = !string.IsNullOrEmpty(mRequestIDStr) ? Convert.ToInt32(mRequestIDStr) : 0;
                    
                    // Safe conversion for MRNo
                    obj.MRNo = ds.Tables[0].Rows[i]["MRNo"]?.ToString() ?? "";
                    
                    // Safe conversion for MRDate
                    string mrDateStr = ds.Tables[0].Rows[i]["MRDate"]?.ToString();
                    obj.MRDate = !string.IsNullOrEmpty(mrDateStr) ? Convert.ToDateTime(mrDateStr) : DateTime.MinValue;
                    

                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNumber"]?.ToString() ?? "";
                    obj.RequestedByName = ds.Tables[0].Rows[i]["RequestedByName"]?.ToString() ?? "";
                    obj.StoreKeeperName = ds.Tables[0].Rows[i]["StoreKeeperName"]?.ToString() ?? "";
                    // Handle missing EmployeeName column - set default
                    obj.EmployeeName = "N/A";
                    
                    //// Safe conversion for TotalValue
                    //string totalValueStr = ds.Tables[0].Rows[i]["TotalValue"]?.ToString();
                  //  obj.TotalValue = !string.IsNullOrEmpty(totalValueStr) ? Convert.ToDecimal(totalValueStr) : 0;

                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<MaterialRequestDetailVM> MaterialRequestPendingList(DateTime FromDate, DateTime ToDate, string Status,int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_MaterialRequestPendingList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialRequestDetailVM> objList = new List<MaterialRequestDetailVM>();
            MaterialRequestDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialRequestDetailVM();

                    // Safe conversion for MRequestID
                    obj.MRequestDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["MRequestDetailID"].ToString());
                    string mRequestIDStr = ds.Tables[0].Rows[i]["MRequestID"]?.ToString();
                    obj.MRequestID = !string.IsNullOrEmpty(mRequestIDStr) ? Convert.ToInt32(mRequestIDStr) : 0;

                    // Safe conversion for MRNo
                    obj.MRNo = ds.Tables[0].Rows[i]["MRNo"]?.ToString() ?? "";

                    // Safe conversion for MRDate
                    string mrDateStr = ds.Tables[0].Rows[i]["MRDate"]?.ToString();
                    obj.MRDate = !string.IsNullOrEmpty(mrDateStr) ? Convert.ToDateTime(mrDateStr) : DateTime.MinValue;


                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNumber"]?.ToString() ?? "";
                    obj.RequestedByName = ds.Tables[0].Rows[i]["RequestedByName"]?.ToString() ?? "";
                    obj.StoreKeeperName = ds.Tables[0].Rows[i]["StoreKeeperName"]?.ToString() ?? "";
                    obj.EquipmentType = ds.Tables[0].Rows[i]["EquipmentType"]?.ToString() ?? "";
                    obj.Model = ds.Tables[0].Rows[i]["Model"]?.ToString() ?? "";
                    string mQuantityStr = ds.Tables[0].Rows[i]["Quantity"]?.ToString();
                    obj.Quantity = !string.IsNullOrEmpty(mQuantityStr) ? Convert.ToDecimal(mQuantityStr) : 0;
                    string mQuantityStr1 = ds.Tables[0].Rows[i]["CurrentStock"]?.ToString();
                    obj.Stock = !string.IsNullOrEmpty(mQuantityStr1) ? Convert.ToDecimal(mQuantityStr1) : 0;
                    obj.StockStatus = ds.Tables[0].Rows[i]["StockStatus"]?.ToString() ?? "";
                    obj.ReadytoPO = Convert.ToBoolean(ds.Tables[0].Rows[i]["ReadytoPO"].ToString());
                    obj.ReadytoIssue = Convert.ToBoolean(ds.Tables[0].Rows[i]["ReadytoIssue"]?.ToString());
                    obj.ApprovedLog = ds.Tables[0].Rows[i]["ApprovedLog"]?.ToString() ?? "";
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<ClientMasterVM> ClientList(string ClientType)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_ClientMasterList";
            cmd.CommandType = CommandType.StoredProcedure;
            
            if (ClientType == null)
                ClientType = "All";


            cmd.Parameters.AddWithValue("@ClientType", ClientType);

         

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<ClientMasterVM> objList = new List<ClientMasterVM>();
            ClientMasterVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new ClientMasterVM();
                    obj.ClientID = Convert.ToInt32(ds.Tables[0].Rows[i]["ClientID"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.ContactName = ds.Tables[0].Rows[i]["ContactName"].ToString();
                    obj.ContactNo = ds.Tables[0].Rows[i]["ContactNo"].ToString();
                    obj.CountryName = ds.Tables[0].Rows[i]["CountryName"].ToString();
                    obj.CityName = ds.Tables[0].Rows[i]["City"].ToString();
                    obj.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                    obj.ClientType= ds.Tables[0].Rows[i]["ClientType"].ToString();                                        
                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static List<EquipmentTypeVM> EquipmentTypeList(int BrandID=0)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_EquipmentTypeList";
            cmd.CommandType = CommandType.StoredProcedure;

            //if (BrandID == null)
            //    BrandID = 0;


            cmd.Parameters.AddWithValue("@BrandID", BrandID);



            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<EquipmentTypeVM> objList = new List<EquipmentTypeVM>();
            EquipmentTypeVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new EquipmentTypeVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.EquipmentType1 = ds.Tables[0].Rows[i]["EquipmentType"].ToString();
                    obj.ProductFamilyName = ds.Tables[0].Rows[i]["ProductFamilyName"].ToString();
                    obj.BrandName = ds.Tables[0].Rows[i]["BrandName"].ToString();                    
                    objList.Add(obj);
                }
            }
            return objList;
        }


        //Save estimation profit
        public static string SaveEstimationProfit(int EstimationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_SaveEstimationProfit";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EstimationID", EstimationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        //Save estimation profit
        public static string SaveQuotationProfit(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_SaveQuotationProfit";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static List<PortCountryVM> GetPortList()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_PortList";
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<PortCountryVM> objList = new List<PortCountryVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PortCountryVM obj = new PortCountryVM();
                    obj.PortID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["PortID"].ToString());
                    obj.PortCode = Convert.ToString(ds.Tables[0].Rows[i]["PortCode"].ToString());
                    obj.Port = Convert.ToString(ds.Tables[0].Rows[i]["Port"].ToString());
                    obj.CountryName = Convert.ToString(ds.Tables[0].Rows[i]["CountryName"].ToString());
                    obj.OriginCity = Convert.ToString(ds.Tables[0].Rows[i]["CityName"].ToString());
                    obj.PortTypeText = Convert.ToString(ds.Tables[0].Rows[i]["PortTypeText"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<QuotationScopeofWorkVM> QuotationScopeofWork(int QuotationId,int EquipmentID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_QuotationScopeofWork";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationId);
            cmd.Parameters.AddWithValue("@EquipmentID", EquipmentID);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationScopeofWorkVM> objList = new List<QuotationScopeofWorkVM>();
            QuotationScopeofWorkVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationScopeofWorkVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.OrderNo = Convert.ToInt32(ds.Tables[0].Rows[i]["OrderNo"].ToString());
                    obj.Checked = true;
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<ScopeOfWorkGroupVM> GetQuotationScopeGroups(int quotationId)
        {
            string connStr = CommonFunctions.GetConnectionString;
            var groupsByKey = new Dictionary<string, ScopeOfWorkGroupVM>(StringComparer.OrdinalIgnoreCase);

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("HVAC_QuotationScopeofWorkPrint", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuotationID", quotationId);
                conn.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    // Result set 1: Parent groups (EquipmentName, Model, Qty)
                    while (rdr.Read())
                    {
                        var g = new ScopeOfWorkGroupVM
                        {
                            EquipmentName = rdr["EquipmentName"]== DBNull.Value ? "" : rdr["EquipmentName"].ToString(),
                            Model = rdr["Model"] == DBNull.Value ? "" : rdr["Model"].ToString(),
                            Qty = rdr["Qty"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["Qty"])
                        };
                        var key = $"{g.EquipmentName}||{g.Model}";
                        groupsByKey[key] = g;
                    }

                    // Move to Result set 2: Child rows (from #temptable = details)
                    if (rdr.NextResult())
                    {
                        while (rdr.Read())
                        {
                            var item = new ScopeOfWorkItemVM
                            {
                                ID = Convert.ToInt32(rdr["ID"]),
                                QuotationID = Convert.ToInt32(rdr["QuotationID"]),
                                EquipmentID = Convert.ToInt32(rdr["EquipmentID"]),
                                EquipmentName = rdr["EquipmentName"]?.ToString(),
                                Description = rdr["Description"]?.ToString(),
                                Model = rdr["Model"]?.ToString(),
                                OrderNo = Convert.ToInt32(rdr["OrderNo"])
                            };

                            var key = $"{item.EquipmentName}||{item.Model}";
                            if (!groupsByKey.TryGetValue(key, out var group))
                            {
                                // Safety: if summary didn’t include this (shouldn’t happen), create it
                                group = new ScopeOfWorkGroupVM
                                {
                                    EquipmentName = item.EquipmentName,
                                    Model = item.Model,
                                    Qty = 0
                                };
                                groupsByKey[key] = group;
                            }
                            group.Items.Add(item);
                        }
                    }
                }
            }

            // Optional ordering: by EquipmentName, then Model, then child OrderNo
            var result = groupsByKey.Values
                .OrderBy(g => g.EquipmentName)
                .ThenBy(g => g.Model)
                .ToList();

            foreach (var g in result)
                g.Items = g.Items.OrderBy(i => i.OrderNo).ToList();

            return result;
        }

        public static List<QuotationWarrantyVM> QuotationWarranty(int QuotationId, int EquipmentID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_QuotationWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationId);
            cmd.Parameters.AddWithValue("@EquipmentID", EquipmentID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationWarrantyVM> objList = new List<QuotationWarrantyVM>();
            QuotationWarrantyVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationWarrantyVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.WarrantyType = ds.Tables[0].Rows[i]["WarrantyType"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.Checked = true;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<QuotationExclusionVM> QuotationExclusions(int QuotationId, int EquipmentID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_QuotationExclusion";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationId);
            cmd.Parameters.AddWithValue("@EquipmentID", EquipmentID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationExclusionVM> objList = new List<QuotationExclusionVM>();
            QuotationExclusionVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationExclusionVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());

                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.Checked = true;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<QuotationTermsVM> QuotationTerms(int QuotationId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetQuotationTerms";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationId);            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationTermsVM> objList = new List<QuotationTermsVM>();
            QuotationTermsVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationTermsVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();                    
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<QuotationContactVM> GetQuotationContacts(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetQuotationContacts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<QuotationContactVM> objList = new List<QuotationContactVM>();
            QuotationContactVM _obj = new QuotationContactVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    QuotationContactVM obj = new QuotationContactVM();
                    obj.ID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ClientID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ClientID"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    obj.EmailID = ds.Tables[0].Rows[i]["EmailID"].ToString();
                    obj.ContactName = ds.Tables[0].Rows[i]["ContactName"].ToString();
                    obj.PhoneNo = ds.Tables[0].Rows[i]["PhoneNo"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;

        }
        public static List<ClientVM> GetQuotationClient(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetQuotationSendTo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);
          

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ClientVM> objList = new List<ClientVM>();
            ClientVM _obj = new ClientVM();
            if (ds != null && ds.Tables.Count > 0)

            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ClientVM obj = new ClientVM();
                    obj.ClientID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ClientID"].ToString());
                    obj.ClientName = ds.Tables[0].Rows[i]["ClientName"].ToString();
                    
                    objList.Add(obj);
                }
            }
            return objList;

        }


        //Load Quotation wise Scope of Work
        public static string UpdateQuotationScopeofWork(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_UpdateQuotationScope";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationId", QuotationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }
        //Auto update of  Quotation contacts
        public static string UpdateQuotationContact(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_SaveQuotationContacts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationID", QuotationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static string UpdateQuotationWarranty(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_UpdateQuotationWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationId", QuotationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static string UpdateQuotationExclusion(int QuotationID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_UpdateQuotationExclusion";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationId", QuotationID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static string UpdateClientPOWarranty(int QuotationID,int JobPurchaseOrderDetailID, int JobHandOverID )
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_UpdateClientPOWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuotationId", QuotationID);
            cmd.Parameters.AddWithValue("@JobPurchaseOrderDetailID", JobPurchaseOrderDetailID);
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static string UpdateJobcost(int JobHandOverID,int ClientPOID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_UpdateJobCost";
            cmd.CommandType = CommandType.StoredProcedure;
         
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);
            cmd.Parameters.AddWithValue("@ClientPOID", ClientPOID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }
        public static string DeleteClientPOWarranty(int JobPurchaseOrderDetailID, int UserId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_DeleteClientPOWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", JobPurchaseOrderDetailID);
            cmd.Parameters.AddWithValue("@UserID", UserId);
      
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            return "OK";
        }

        public static List<QuotationWarrantyVM> ClientPOWarranty(int Id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_ClientPOWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", Id);
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuotationWarrantyVM> objList = new List<QuotationWarrantyVM>();
            QuotationWarrantyVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuotationWarrantyVM();
                    obj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.WarrantyType = ds.Tables[0].Rows[i]["WarrantyType"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.EquipmentName = ds.Tables[0].Rows[i]["EquipmentName"].ToString();
                    obj.Checked = true;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<MaterialIssueVM> MaterialIssueList(DateTime FromDate, DateTime ToDate, string IssueNo, string RequestedBy, int EmployeeID, int BranchID, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_MaterialIssueList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));

            if (IssueNo == null)
                IssueNo = "";
            cmd.Parameters.AddWithValue("@IssueNo", IssueNo);

            if (RequestedBy == null)
                RequestedBy = "";
            cmd.Parameters.AddWithValue("@RequestedBy", RequestedBy);

            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@FYearID", FyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialIssueVM> objList = new List<MaterialIssueVM>();
            MaterialIssueVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialIssueVM();

                    // Safe conversion for MIssueID
                    string mIssueIDStr = ds.Tables[0].Rows[i]["MIssueID"]?.ToString();
                    obj.MIssueID = !string.IsNullOrEmpty(mIssueIDStr) ? Convert.ToInt32(mIssueIDStr) : 0;

                    // Safe conversion for IssueDate
                    string issueDateStr = ds.Tables[0].Rows[i]["IssueDate"]?.ToString();
                    obj.IssueDate = !string.IsNullOrEmpty(issueDateStr) ? Convert.ToDateTime(issueDateStr) : DateTime.MinValue;

                    // Safe conversion for RequestID
                    string requestIDStr = ds.Tables[0].Rows[i]["RequestID"]?.ToString();
                    obj.RequestID = !string.IsNullOrEmpty(requestIDStr) ? Convert.ToInt32(requestIDStr) : (int?)null;
                    obj.IssueNo = ds.Tables[0].Rows[i]["IssueNo"]?.ToString() ?? "";
                    // RequestedBy
                    obj.RequestedByName = ds.Tables[0].Rows[i]["RequestedByName"]?.ToString() ?? "";

                    // ProjectN o.
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNumber"]?.ToString() ?? "";
                    // Safe conversion for IssuedBy
                    string issuedByStr = ds.Tables[0].Rows[i]["IssuedBy"]?.ToString();
                    obj.IssuedBy = !string.IsNullOrEmpty(issuedByStr) ? Convert.ToInt32(issuedByStr) : (int?)null;

                    // Remarks
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"]?.ToString() ?? "";

                    // IssuedByName
                    obj.IssuedByName = ds.Tables[0].Rows[i]["IssuedByName"]?.ToString() ?? "";

                    // RequestedByName
                    obj.RequestedByName = ds.Tables[0].Rows[i]["RequestedByName"]?.ToString() ?? "";

                    // IssueNo - fix the mapping
                    //  obj.IssueNo = ds.Tables[0].Rows[i]["IssueNo"]?.ToString() ?? "";

                    objList.Add(obj);
                }
            }
            return objList;
        }


        //Project No. wise Equipment details
        public static List<PurchaseOrderDetailVM> JobEquipmentDetail(int JobHandOverID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GetJobEquipmentDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<PurchaseOrderDetailVM> objList = new List<PurchaseOrderDetailVM>();
            PurchaseOrderDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new PurchaseOrderDetailVM();
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();                    
                    obj.EstimationID = Convert.ToInt32(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.JobHandOverID = Convert.ToInt32(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNumber"].ToString();
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    //obj.CurrencyCode = ds.Tables[0].Rows[i]["CurrencyCode"].ToString();
                    //obj.CurrencyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CurrencyID"].ToString());
                    obj.Quantity = Convert.ToInt32(ds.Tables[0].Rows[i]["Qty"].ToString());
                    //obj.ExchangeRate = Convert.ToDecimal(ds.Tables[0].Rows[i]["ExchangeRate"].ToString());
                    obj.Rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());

                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["FValue"].ToString());
                    obj.Deleted = false;                    
                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<PurchaseOrderDetailVM> PurchaseOrderDetail(int PurchaseOrderID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_PODetailList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<PurchaseOrderDetailVM> objList = new List<PurchaseOrderDetailVM>();
            PurchaseOrderDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new PurchaseOrderDetailVM();
                    obj.PurchaseOrderDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderDetailID"].ToString());
                    obj.PurchaseOrderID = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.EstimationID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.JobHandOverID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.MRequestID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["MRequestID"].ToString());
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNo"].ToString();
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemUnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    // obj.EquipmentStatus = ds.Tables[0].Rows[i]["EquipmentStatus"].ToString();
                    //obj.CreatedBy = ds.Tables[0].Rows[i]["CreatedBy"].ToString();
                    //obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    //obj.NominalCapacity = ds.Tables[0].Rows[i]["NominalCapacity"].ToString();
                    //obj.EfficientType = ds.Tables[0].Rows[i]["EfficientType"].ToString();
                    //obj.Refrigerant = Convert.ToBoolean(ds.Tables[0].Rows[i]["Refrigerant"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<POApproverVM> PurchaseOrderApproveDetail(int PurchaseOrderID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_POApproveDetailList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<POApproverVM> objList = new List<POApproverVM>();
            POApproverVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new POApproverVM();
                    obj.POApprovalID = Convert.ToInt32(ds.Tables[0].Rows[i]["POApprovalID"].ToString());
                    obj.PurchaseOrderID = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderID"].ToString());
                    obj.EmployeeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeID"].ToString());
                    obj.Type = ds.Tables[0].Rows[i]["Type"].ToString();
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());                                        
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<PurchaseOrderDetailVM> PurchaseOrderPendingDetail()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_POPendingDetailList";
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<PurchaseOrderDetailVM> objList = new List<PurchaseOrderDetailVM>();
            PurchaseOrderDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new PurchaseOrderDetailVM();
                    obj.PurchaseOrderDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderDetailID"].ToString());
                    obj.PurchaseOrderID = Convert.ToInt32(ds.Tables[0].Rows[i]["PurchaseOrderID"].ToString());
                    obj.MRequestID = Convert.ToInt32(ds.Tables[0].Rows[i]["MRequestID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.QuotationID = Convert.ToInt32(ds.Tables[0].Rows[i]["QuotationID"].ToString());
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EstimationID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EstimationID"].ToString());
                    obj.JobHandOverID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNo"].ToString();
                    obj.EstimationNo = ds.Tables[0].Rows[i]["EstimationNo"].ToString();
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemUnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Rate = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.Amount = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    // obj.EquipmentStatus = ds.Tables[0].Rows[i]["EquipmentStatus"].ToString();
                    //obj.CreatedBy = ds.Tables[0].Rows[i]["CreatedBy"].ToString();
                    //obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString());
                    //obj.NominalCapacity = ds.Tables[0].Rows[i]["NominalCapacity"].ToString();
                    //obj.EfficientType = ds.Tables[0].Rows[i]["EfficientType"].ToString();
                    //obj.Refrigerant = Convert.ToBoolean(ds.Tables[0].Rows[i]["Refrigerant"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static StatusModel GenerateMaterialRequest(int JobHandOverID, int PurchaseOrderDetailId, int RequestedBy,int StoreKeeperID,int UserId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GenerateMaterialRequest";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobHandOverID);
            cmd.Parameters.AddWithValue("@PurchaseOrderDetailId", PurchaseOrderDetailId);
            cmd.Parameters.AddWithValue("@RequestedBy", RequestedBy);
            cmd.Parameters.AddWithValue("@StoreKeeperID", StoreKeeperID);
            cmd.Parameters.AddWithValue("@UserId", UserId);
        
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            StatusModel _model = new StatusModel();
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    _model.Status = ds.Tables[0].Rows[0][0].ToString();
                    _model.Message = ds.Tables[0].Rows[0][1].ToString();
                }
            }

            return _model;
        }

        public static List<GRNDetailVM> PurchaseOrderDetailforGRN(int PurchaseOrderID,int GRNID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_PODetailList_GRN";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);
            cmd.Parameters.AddWithValue("@GRNID", GRNID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<GRNDetailVM> objList = new List<GRNDetailVM>();
            GRNDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new GRNDetailVM();
                    obj.GRNDetailID = Convert.ToInt32(ds.Tables[0].Rows[i]["GRNDetailID"].ToString());
                    obj.GRNID = Convert.ToInt32(ds.Tables[0].Rows[i]["GRNID"].ToString());                    
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.JobHandOverID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["JobHandOverID"].ToString());
                    obj.ProjectNo = ds.Tables[0].Rows[i]["ProjectNo"].ToString();
                    
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();

                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemUnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    
                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static string StockMasterGRNPosting(int Id, string TransType)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "HVAC_SaveStockMaster";
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@ReferenceID", Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        public static List<StockOpeningVM> GetStockOpeningList(int BranchId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_StockOpeningList";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<StockOpeningVM> objList = new List<StockOpeningVM>();
            StockOpeningVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new StockOpeningVM();
                    obj.OpeningID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["OpeningID"].ToString());
                    obj.EquipmentTypeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());
                    obj.EquipmentType = ds.Tables[0].Rows[i]["EquipmentType"].ToString();
                    obj.ProductFamilyName = ds.Tables[0].Rows[i]["ProductFamilyName"].ToString();
                    obj.BrandName = ds.Tables[0].Rows[i]["BrandName"].ToString();
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Rate = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.Quantity = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Value = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Value"].ToString());                    
                    obj.ItemUnit = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static DataTable DeleteStockOpening(int OpeningID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_DeleteStockOpening";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OpeningID", OpeningID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }


        }


        public static List<MaterialIssueDetailVM> MaterialIssueDetail(int MRequestID, int MIssueID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_RequestDetailList_Issue";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MRequestID", MRequestID);
            cmd.Parameters.AddWithValue("@MIssueID", MIssueID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialIssueDetailVM> objList = new List<MaterialIssueDetailVM>();
            MaterialIssueDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialIssueDetailVM();
                    
                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());

                    obj.EquipmentType = ds.Tables[0].Rows[i]["Description"].ToString();

                 

                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemUnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Checked = Convert.ToBoolean(ds.Tables[0].Rows[i]["Checked"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }


        //in the material request page, clientpo wise equipment display
        public static List<MaterialRequestDetailVM> MaterialRequestDetail(int ClientPOID, int MRequestID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_EquipmentDetailList_Request";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ClientPOID", ClientPOID);
            cmd.Parameters.AddWithValue("@MRequestID", MRequestID);            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialRequestDetailVM> objList = new List<MaterialRequestDetailVM>();
            MaterialRequestDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialRequestDetailVM();

                    obj.EquipmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.EquipmentTypeID = Convert.ToInt32(ds.Tables[0].Rows[i]["EquipmentTypeID"].ToString());

                    obj.EquipmentType = ds.Tables[0].Rows[i]["Description"].ToString();



                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ItemUnitID = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemUnitID"].ToString());
                    obj.Model = ds.Tables[0].Rows[i]["Model"].ToString();
                    obj.Quantity = Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Checked = Convert.ToBoolean(ds.Tables[0].Rows[i]["Checked"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<MaterialRequestVM> GetJobMaterialRequest(int JobID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HAVC_GetProjectMaterialRequest";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobID);
          
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<MaterialRequestVM> objList = new List<MaterialRequestVM>();
            MaterialRequestVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new MaterialRequestVM();

                    obj.MRequestID = Convert.ToInt32(ds.Tables[0].Rows[i]["MRequestID"].ToString());
                    obj.MRNo = ds.Tables[0].Rows[i]["MRNo"].ToString();
                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<JobPurchaseOrderDetail> GetJobClientPO(int JobID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HAVC_GetProjectClientPO";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@JobHandOverID", JobID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<JobPurchaseOrderDetail> objList = new List<JobPurchaseOrderDetail>();
            JobPurchaseOrderDetail obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new JobPurchaseOrderDetail();

                    obj.ID= Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.PONumber = ds.Tables[0].Rows[i]["PONumber"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static string UpdateMaterialRequestStatus(int PurchaseOrderID,int IssueID)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "HVAC_MaterialRequestStatusUpdate";
                        cmd.Parameters.AddWithValue("@PurchaseorderID", PurchaseOrderID);
                        cmd.Parameters.AddWithValue("@IssueID", IssueID);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }
    }
}