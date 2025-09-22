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
    public class ReceiptDAO
    {
        public static string RevertInvoiceIdtoInscanMaster(int InvoiceId)
        {
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "Update InscanMaster set InvoiceID=null where Isnull(InvoiceId,0)=@InvoiceId";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }






        }

        public static string RevertInvoiceIdtoInboundShipment(int InvoiceId)
        {
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "Update InboundShipment set AgentInvoiceID=null where Isnull(AgentInvoiceId,0)=@InvoiceId";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }






        }

        //Delete tax invoice of importshipment 
        public static DataTable DeleteTaxInvoice(int ShipmentInvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteTaxInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipmentInvoiceId", @ShipmentInvoiceId);
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
                        cmd.CommandText = "SP_GetMaxEmployeeCode";
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


        public static DataTable DeleteEnquiries(int EnquiryId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteEnquiries";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EnquiryId", EnquiryId);
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



        public static void AddProductMaster(int Id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_SaveProjectProduct", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProjectId", Id);

                try
                {
                    conn.Open();


                    cmd.ExecuteNonQuery();


                    Console.WriteLine("ProductMaster added/updated successfully.");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);


                    throw new Exception("Error while adding/updating ProductMaster", ex);
                }
            }
        }


        public static List<SupplierInvoiceVM> GetSupplierInvoiceList(DateTime FromDate, DateTime ToDate, int FyearID, string InvoiceNo)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAllSupplierInvoice";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(FromDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@Todate", Convert.ToDateTime(ToDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FyearId", FyearID);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<SupplierInvoiceVM> objList = new List<SupplierInvoiceVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    SupplierInvoiceVM obj = new SupplierInvoiceVM();
                    obj.SupplierInvoiceID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["SupplierInvoiceID"].ToString());
                    obj.InvoiceDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["InvoiceDate"].ToString());
                    obj.InvoiceNo = ds.Tables[0].Rows[i]["InvoiceNo"].ToString();
                    obj.SupplierName = ds.Tables[0].Rows[i]["SupplierName"].ToString();
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    obj.SupplierType = ds.Tables[0].Rows[i]["SupplierType"].ToString();
                    obj.ReferenceNo = ds.Tables[0].Rows[i]["ReferenceNo"].ToString();
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static List<SupplierInvoiceDetailVM> GetPurchaseInvoiceList(int SupplierInvoiceId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetPurchaseInvoiceDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceId);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<SupplierInvoiceDetailVM> objList = new List<SupplierInvoiceDetailVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    SupplierInvoiceDetailVM obj = new SupplierInvoiceDetailVM();
                    obj.SupplierInvoiceID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["SupplierInvoiceID"].ToString());
                    obj.SupplierInvoiceDetailID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["SupplierInvoiceDetailID"].ToString());
                    obj.ProductID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ProductID"].ToString());
                    obj.ProductTypeID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ProductTypeID"].ToString());
                    obj.UnitID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.AcHeadId = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["AcHeadID"].ToString());
                    obj.UnitName = ds.Tables[0].Rows[i]["ItemUnit"].ToString();
                    obj.ProductName = ds.Tables[0].Rows[i]["ProductName"].ToString();
                    obj.Particulars = ds.Tables[0].Rows[i]["Particulars"].ToString();
                    obj.ProductType = ds.Tables[0].Rows[i]["ProductTypeName"].ToString();
                    obj.Quantity = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    //obj.CurrencyID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["CurrencyID"].ToString());
                    obj.Rate = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    obj.TaxPercentage = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TaxPercentage"].ToString());
                    obj.ProjectName = ds.Tables[0].Rows[i]["ProjectName"].ToString();
                    obj.ProjectID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["ProjectID"].ToString());
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Value"] == DBNull.Value)
                    {
                        obj.Value = 0;
                    }
                    else
                    {
                        obj.Value = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Value"].ToString());
                    }


                    objList.Add(obj);
                }
            }
            return objList;
        }










        public static DataTable DeleteFinancialYear(int YearID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteFinancialYear";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@YearID", YearID);
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
        public static DataTable DeleteProduct(int ProductID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteProduct";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", ProductID);
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

        public static DataTable DeleteProductCategory(int ProductCategoryID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteProductCategory";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductCategoryID", ProductCategoryID);
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
        public static DataTable DeleteProductGroup(int @ProductGroupID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteProductGroup";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGroupID", @ProductGroupID);
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
        public static DataTable DeleteInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCustomerInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerInvoiceId", InvoiceId);
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
        public static DataTable DeleteCODInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCODInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODInvoiceId", InvoiceId);
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
        public static DataTable DeleteCoLoaderInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCOLoaderInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AgentInvoiceId", InvoiceId);
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
        public static DataTable DeleteInscan(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteInscan";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InScanId", InscanId);
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
        public static DataTable DeleteCoLoaderShipment(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteInboundshipment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipmentId", InscanId);
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
        public static DataTable DeleteImportShipmentAWB(int ShipmentDetailId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteImportShipmentDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipmentDetailId", ShipmentDetailId);
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
        //Delete inter branch impor shipment
        public static DataTable DeleteInterBranchImport(int ImportID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteImportShipment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ImportId", ImportID);
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
        //Delete inter branch impor shipment
        public static string UpdateImportIDtoExport(int ImportID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_UpdateImportShipmentToExport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ImportId", ImportID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            return "ok";


        }

        //Delete import-Inscan items
        public static DataTable DeleteImportInscan(int QuickInscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteImportInScan";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuickInscanId", QuickInscanId);
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
        public static string DeleteDRSReconcDetail(int DRRID, int DRRDetailId)
        {

            SqlCommand cmd = new SqlCommand();
            try
            {

                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "SP_DeleteDRRDetail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DRRID", DRRID);
                cmd.Parameters.AddWithValue("@ID", DRRDetailId);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static DataTable DeleteDRSReconc(int DRRID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDRReconC";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRRID", DRRID);
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
        //Quick Inscan
        public static DataTable DeleteDepotInscan(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDepotInscan";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuickInScanId", InscanId);
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
        public static DataTable DeleteSupplierPayments(int RecPayID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplierPayments";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
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

        public static DataTable DeleteDomesticCOD(int ReceiptId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDomesticCODReciepts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReceiptID", ReceiptId);
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
        public static DataTable DeleteDRS(int DRSID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDRS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRSID", DRSID);
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

        public static DataTable DeleteSupplier(int SupplierId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplier";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
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

        public static DataTable DeleteForwardingAgent(int FAgentID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteForwardingAgent";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FAgentId", FAgentID);
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

        //Quick Customer Booking
        public static DataTable DeleteCustomerBooking(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCustomerBooking";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InScanId", InscanId);
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
        public static List<ReceiptAllocationDetailVM> GetAWBAllocation(List<ReceiptAllocationDetailVM> list, int InvoiceId, decimal Amount, int RecpayId)
        {
            try

            {

                if (list == null)
                    list = new List<ReceiptAllocationDetailVM>();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "SP_GetInvoiceAWBAllocation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                cmd.Parameters.AddWithValue("@ReceivedAmount", Amount);
                cmd.Parameters.AddWithValue("@RecPayId", RecpayId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow drrow = ds.Tables[0].Rows[i];
                        ReceiptAllocationDetailVM item = new ReceiptAllocationDetailVM();
                        item.ID = Convert.ToInt32(drrow["ID"].ToString());
                        item.CustomerInvoiceId = Convert.ToInt32(drrow["CustomerInvoiceId"].ToString());
                        item.CustomerInvoiceDetailID = Convert.ToInt32(drrow["CustomerInvoiceDetailID"].ToString());
                        item.InScanID = Convert.ToInt32(drrow["InScanId"].ToString());
                        item.RecPayID = Convert.ToInt32(drrow["RecPayID"].ToString());
                        item.RecPayDetailID = Convert.ToInt32(drrow["RecPayDetailID"].ToString());
                        item.CustomerInvoiceDetailID = Convert.ToInt32(drrow["CustomerInvoiceDetailID"].ToString());
                        item.AWBNo = drrow["AWBNo"].ToString();
                        item.AWBDate = Convert.ToDateTime(drrow["AWBDate"].ToString()).ToString("dd-MM-yyyy");
                        item.TotalAmount = Convert.ToDecimal(drrow["TotalAmount"].ToString());
                        item.ReceivedAmount = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                        item.PendingAmount = Convert.ToDecimal(drrow["PendingAmount"].ToString());
                        item.AllocatedAmount = Convert.ToDecimal(drrow["AllocatedAmount"].ToString());
                        item.Allocated = Convert.ToBoolean(drrow["Allocated"].ToString());

                        list.Add(item);

                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }

            return list;
        }



        #region "supplierInvoice"

        public static DataTable DeleteSupplierInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplierInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierInvoiceId", InvoiceId);
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

        public static DataTable DeleteProductionSetup(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteProductionSetup";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", ID);
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


        public static DataTable DeleteBatchProcess(int ID, int UserId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteBatchProcess";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BatchId", ID);
            cmd.Parameters.AddWithValue("@UserId", UserId);
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

        public static string GetMaxBathcNo(DateTime BatchDate, int BranchId, int FYearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxBatchNo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BatchDate", Convert.ToDateTime(BatchDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearID", FYearId);
            cmd.Parameters.AddWithValue("@BranchID", BranchId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static string SP_GetMaxSINo(int BranchId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxFowardingSupplierInvoiceNo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        #endregion




        #region customersupplieradvance
        public static decimal SP_GetCustomerAdvance(int CustomerId, int RecPayId, int FyearId, string EntryType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerAdvance";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@EntryType", EntryType);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public static decimal SP_GetCustomerInvoiceReceived(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoiceReceived";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }
        public static List<CustomerTradeReceiptVM> GetCustomerReceiptAllocated(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type, string RecPayType, string EntryType)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerReceiptAllocated";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            cmd.Parameters.AddWithValue("@ReceiptType", RecPayType);
            cmd.Parameters.AddWithValue("@EntryType", EntryType);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.RecPayDetailID = CommonFunctions.ParseInt(drrow["RecPayDetailID"].ToString());
                    item.SalesInvoiceID = CommonFunctions.ParseInt(drrow["InvoiceID"].ToString());
                    item.AcOPInvoiceDetailID = CommonFunctions.ParseInt(drrow["AcOPInvoiceDetailID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.date = Convert.ToDateTime(Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("yyyy-MM-dd h:mm tt"));
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceAmount = Convert.ToDecimal(drrow["InvoiceAmount"].ToString());
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());

                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());
                    item.AdjustmentAmount = Convert.ToDecimal(drrow["AdjustmentAmount"].ToString());
                    item.Allocated = true;
                    list.Add(item);

                }
            }

            return list;

        }

        public static List<CustomerTradeReceiptVM> SP_GetCustomerInvoicePending(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type, string RecPayType)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoicePending";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            cmd.Parameters.AddWithValue("@ReceiptType", RecPayType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.SalesInvoiceID = Convert.ToInt32(drrow["InvoiceID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.date = Convert.ToDateTime(drrow["InvoiceDate"].ToString());
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceAmount = Convert.ToDecimal(drrow["InvoiceAmount"].ToString());
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());
                    item.Allocated = false;
                    item.Amount = 0;
                    list.Add(item);

                }
            }

            return list;

        }
        public static List<CustomerTradeReceiptVM> SP_GetCustomerInvoicePendingDRS(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type, string RecPayType)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoicePendingDRS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            cmd.Parameters.AddWithValue("@ReceiptType", RecPayType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.SalesInvoiceID = Convert.ToInt32(drrow["InvoiceID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceAmount = Convert.ToDecimal(drrow["InvoiceAmount"].ToString());
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());

                    list.Add(item);

                }
            }

            return list;

        }
        public static List<CustomerTradeReceiptVM> SP_GetCustomerReceiptPending(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerReceiptPending";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.SalesInvoiceID = Convert.ToInt32(drrow["InvoiceID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.InvoiceAmount = Convert.ToDecimal(drrow["InvoiceAmount"].ToString());
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());

                    list.Add(item);

                }
            }

            return list;

        }

        public static List<CustomerTradeReceiptVM> SP_GetSupplierInvoicePending(int SupplierId, int RecPayId, int SupplierTypeId, string Type)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierInvoicePending";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);

            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);

            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            cmd.Parameters.AddWithValue("@SupplierTypeId", SupplierTypeId);
            cmd.Parameters.AddWithValue("@EntryType", Type);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.SalesInvoiceID = Convert.ToInt32(drrow["InvoiceID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.date = Convert.ToDateTime(drrow["InvoiceDate"].ToString());
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceAmount = Math.Abs(Convert.ToDecimal(drrow["InvoiceAmount"].ToString()));
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());
                    item.Amount = 0;
                    list.Add(item);

                }
            }

            return list;

        }

        public static List<CustomerTradeReceiptVM> SP_GetSupplierReceiptPending(int SupplierID, int InvoiceId, int RecPayId, int CreditNoteId, string Type)
        {
            List<CustomerTradeReceiptVM> list = new List<CustomerTradeReceiptVM>();
            CustomerTradeReceiptVM item = new CustomerTradeReceiptVM();
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierPaymentPending";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierID);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@DebitNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@FYearId", yearid);
            cmd.Parameters.AddWithValue("@BranchId", branchid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerTradeReceiptVM();
                    item.SalesInvoiceID = Convert.ToInt32(drrow["InvoiceID"].ToString());
                    item.InvoiceNo = drrow["InvoiceNo"].ToString();
                    item.InvoiceType = drrow["TransType"].ToString();
                    item.DateTime = Convert.ToDateTime(drrow["InvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    item.InvoiceAmount = Convert.ToDecimal(drrow["InvoiceAmount"].ToString());
                    item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                    item.Balance = Convert.ToDecimal(drrow["Balance"].ToString());

                    list.Add(item);

                }
            }

            return list;

        }


        public static decimal SP_GetSupplierAdvance(int SupplierId, int RecPayId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierAdvance";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public static decimal SP_GetSupplierInvoicePaid(int SupplierId, int InvoiceId, int RecPayId, int DebitNoteId, string Type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierInvoicePaid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@DebitNoteId", DebitNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }
        #endregion



        #region CodeGeneration

        public static void ReSaveForwardingAgentCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveForwardingAgentCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }
        public static void ReSaveSupplierCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveSupplierCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }
        public static void ReSaveEmployeeCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveEmployeeCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }

        public static void ReSaveCustomerCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveCustomerCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }

        public static string GetMaxCustomerCode(string CustomerName)
        {
            try
            {
                //SP_InsertJournalEntryForRecPay
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
                cmd.CommandText = "SP_GetMaxCustomerCode";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerName", CustomerName);
                cmd.Connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string cutomercode = ds.Tables[0].Rows[0][0].ToString();

                    return cutomercode;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
        public string GetMaxCustomerCode(int BranchId)
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
                        cmd.CommandText = "GetMaxCustomerNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);

                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }

        public static int CheckInvoiceReceipt(int InvoiceId, int CustomerId)
        {
            int ReceiptId = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_CheckInvoiceReceipt";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ReceiptId = Convert.ToInt32(ds.Tables[0].Rows[0]["RecPayID"]);
                }
            }

            return ReceiptId;

        }
        public static List<CustomerInvoiceVM> GetInvoiceList(DateTime FromDate, DateTime ToDate, string InvoiceNo, int FyearId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetInvoiceList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FyearId);

            if (InvoiceNo == null)
                InvoiceNo = "";
            cmd.Parameters.AddWithValue("@InvoiceNo", @InvoiceNo);

            cmd.Parameters.AddWithValue("@BranchID", branchid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<CustomerInvoiceVM> objList = new List<CustomerInvoiceVM>();
            CustomerInvoiceVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new CustomerInvoiceVM();
                    obj.CustomerInvoiceID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["CustomerInvoiceID"].ToString());
                    obj.CustomerInvoiceNo = ds.Tables[0].Rows[i]["CustomerInvoiceNo"].ToString();
                    obj.InvoiceDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["InvoiceDate"].ToString());
                    obj.CustomerName = ds.Tables[0].Rows[i]["CustomerName"].ToString();
                    obj.SalesMode = ds.Tables[0].Rows[i]["PaymentModeText"].ToString();
                    obj.CustomerID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                    obj.InvoiceTotal = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["InvoiceTotal"].ToString());
                    obj.InvoiceTotalFC = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["InvoiceTotalFC"].ToString());
                    obj.CurrencyName = ds.Tables[0].Rows[i]["CurrencyName"].ToString();
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }



        public string GetMaxInvoiceNo(int Companyid, int BranchId, int FYearId)
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
                        cmd.CommandText = "GetMaxInvoiceNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
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

        public string GetMaxSalesInvoiceNo(int Companyid, int BranchId, int FYearId)
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
                        cmd.CommandText = "GetMaxSalesInvoiceNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
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

        //Generate Invoice Posting
        public string GenerateInvoicePosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_GenerateInvoicePosting" + Id.ToString();
                        cmd.CommandType = CommandType.Text;
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


        public string GenerateSaleEntryPosting(int Id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_GenerateSaleEntryPosting", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@SalesID", Id);
                try
                {
                    conn.Open();

                    // Execute the stored procedure and return the result as an integer.
                    var result = cmd.ExecuteScalar();
                    return "OK";
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);

                    Console.WriteLine(ex.StackTrace);

                    return ex.Message;
                }
            }


        }
        public static string GenerateSaleEntryOpeningPosting(int Id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_ShiftEntryOpeningPosting", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CounterLoginID", Id);
                try
                {
                    conn.Open();

                    // Execute the stored procedure and return the result as an integer.
                    var result = cmd.ExecuteScalar();
                    return "OK";
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);

                    Console.WriteLine(ex.StackTrace);

                    return ex.Message;
                }
            }


        }
        public static string GenerateSaleEntryClosingPosting(int Id)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.GetConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_ShiftEntryClosingPosting", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CounterLoginID", Id);
                try
                {
                    conn.Open();

                    // Execute the stored procedure and return the result as an integer.
                    var result = cmd.ExecuteScalar();
                    return "OK";
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);

                    Console.WriteLine(ex.StackTrace);

                    return ex.Message;
                }
            }


        }


        public static CustomerInvoiceVM CustomerInvoiceDetail(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();
            CustomerInvoiceVM item = new CustomerInvoiceVM();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoiceDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", id);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerInvoice");

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow drrow = ds.Tables[0].Rows[i];
                    item = new CustomerInvoiceVM();
                    item.CustomerInvoiceID = Convert.ToInt32(drrow["CustomerInvoiceID"].ToString());
                    item.CustomerInvoiceNo = drrow["CustomerInvoiceNo"].ToString();
                    item.InvoiceDate = Convert.ToDateTime(drrow["InvoiceDate"].ToString());
                    item.InvoiceTotal = Convert.ToDecimal(drrow["InvoiceTotal"].ToString());
                    item.CustomerName = drrow["CustomerName"].ToString();
                    //item.CustomerCode = drrow["CustomerCode"].ToString();
                    //item.VATTRN = drrow["VATTRN"].ToString();
                    //item.CustomerCityName = drrow["CityName"].ToString();
                    //item.CustomerCountryName = drrow["CountryName"].ToString();                    
                    item.CustomerPhoneNo = drrow["CustomerPhoneNo"].ToString();
                    item.Address1 = drrow["CustomerAddress"].ToString();
                    //item.Pincode = drrow["Address3"].ToString();
                    item.InvoiceFooter1 = drrow["InvoiceFooter1"].ToString();
                    item.InvoiceFooter2 = drrow["InvoiceFooter2"].ToString();
                    item.InvoiceFooter3 = drrow["InvoiceFooter3"].ToString();
                    item.InvoiceFooter4 = drrow["InvoiceFooter4"].ToString();
                    item.InvoiceFooter5 = drrow["InvoiceFooter5"].ToString();
                    item.BankDetail1 = drrow["BankDetail1"].ToString();
                    item.BankDetail2 = drrow["BankDetail2"].ToString();
                    item.BankDetail3 = drrow["BankDetail3"].ToString();
                    item.BankDetail4 = drrow["BankDetail4"].ToString();
                    item.BranchTRN = drrow["BranchVATRegistrationNo"].ToString();
                }
            }

            return item;

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
        //Generate SupplierInvoice posting
        public string GenerateSupplierInvoicePosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_SupplierInvoicePosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
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
        public string SaveBatchRawProductDetail(int BatchID, int ProductID, int ProductCategoryID, int BatchActualQty)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_SaveBatchProcessDetail";
                        cmd.Parameters.AddWithValue("@BatchID", BatchID);
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);
                        cmd.Parameters.AddWithValue("@ProductCategoryID", ProductCategoryID);
                        cmd.Parameters.AddWithValue("@BatchActualQty", BatchActualQty);
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

        public string GenerateStockMasterPosting(int Id, string TransType)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_SaveStockMaster";
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
        public string GenerateBatchCompletePosting(int Id, int UserId)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_BatchCompletePosting";
                        cmd.Parameters.AddWithValue("@BatchID", Id);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
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
                        cmd.CommandText = "SaveRoleMenuAccessRights";
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


        public static DataTable DeleteItemUnit(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteItemUnit";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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
        //ProjectMilstone

        public static DataTable DeleteDocumentType(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDocumentType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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



        public static DataTable DeleteBondType(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteBondType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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


        public static DataTable DeleteWarranty(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteWarranty";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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


        public static DataTable DeleteQuotationStatus(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteQuotationStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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



        public static DataTable DeleteEquipmentTagType(int ID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteEquipmentTagType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", ID);
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


        public static DataTable DeletePurchaseOrder(int PurchaseOrderID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeletePurchaseOrder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);
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

        

        public static List<GRNVM> GRNList(DateTime FromDate, DateTime ToDate, int SupplierTypeId, string GRNNO)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_GRNList";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@SupplierTypeId", SupplierTypeId);
            cmd.Parameters.AddWithValue("@GRNNO", string.IsNullOrEmpty(GRNNO) ? (object)DBNull.Value : GRNNO);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<GRNVM> objList = new List<GRNVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    GRNVM obj = new GRNVM
                    {
                        GRNID = CommonFunctions.ParseInt(row["GRNID"].ToString()),
                        GRNDATE = Convert.ToDateTime(row["GRNDATE"]),
                        GRNNO = row["GRNNO"].ToString(),
                        Remarks = row["Remarks"].ToString(),
                        SupplierName = row["SupplierName"].ToString(),
                    };
                    objList.Add(obj);
                }
            }

            return objList;
        }


        public static List<PurchaseOrderVM> SupplierPurchaseOrderList(DateTime FromDate, DateTime ToDate, int SupplierTypeId, string PurchaseOrderNo)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "HVAC_SupplierPurchaseOrders";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@SupplierTypeId", SupplierTypeId);
            cmd.Parameters.AddWithValue("@PurchaseOrderNo", string.IsNullOrEmpty(PurchaseOrderNo) ? (object)DBNull.Value : PurchaseOrderNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<PurchaseOrderVM> objList = new List<PurchaseOrderVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    PurchaseOrderVM obj = new PurchaseOrderVM
                    {
                        PurchaseOrderID = CommonFunctions.ParseInt(row["PurchaseOrderID"].ToString()),
                        PurchaseOrderDate = Convert.ToDateTime(row["PurchaseOrderDate"]),
                        PurchaseOrderNo = row["PurchaseOrderNo"].ToString(),
                        Remarks = row["Remarks"].ToString(),
                        SupplierName = row["SupplierName"].ToString(),
                        SONoRef = row["SONoRef"].ToString(),
                        TotalAmount = CommonFunctions.ParseDecimal(row["TotalAmount"].ToString())
                    };
                    objList.Add(obj);
                }
            }

            return objList;
        }



        //public static List<PurchaseOrderVM> SupplierPurchaseOrderList(DateTime FromDate, DateTime ToDate, int SupplierTypeId)
        //{
        //    int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
        //    cmd.CommandText = "HVAC_SupplierPurchaseOrders";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(FromDate).ToString("MM/dd/yyyy"));
        //    cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(ToDate).ToString("MM/dd/yyyy"));
        //    cmd.Parameters.AddWithValue("@SupplierTypeId", SupplierTypeId);



        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);

        //    List<PurchaseOrderVM> objList = new List<PurchaseOrderVM>();

        //    if (ds != null && ds.Tables.Count > 0)
        //    {
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            PurchaseOrderVM obj = new PurchaseOrderVM();
        //            obj.PurchaseOrderID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["PurchaseOrderID"].ToString());
        //            obj.PurchaseOrderDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["PurchaseOrderDate"].ToString());
        //            obj.PurchaseOrderNo = ds.Tables[0].Rows[i]["PurchaseOrderNo"].ToString();
        //            obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
        //            obj.SupplierName = ds.Tables[0].Rows[i]["SupplierName"].ToString();
        //            obj.TotalAmount = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TotalAmount"].ToString());
        //            objList.Add(obj);
        //        }
        //    }
        //    return objList;
        //}


        public static List<PurchaseOrderDetailVM> GetPurchaseOrderList(int PurchaseOrderID)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetPurchaseOrderDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<PurchaseOrderDetailVM> objList = new List<PurchaseOrderDetailVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PurchaseOrderDetailVM obj = new PurchaseOrderDetailVM();
                    obj.PurchaseOrderID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["PurchaseOrderID"].ToString());
                    obj.PurchaseOrderDetailID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["PurchaseOrderDetailID"].ToString());
                    obj.EquipmentID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["EquipmentID"].ToString());
                    obj.ItemUnitID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["UnitID"].ToString());
                    obj.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                    obj.Quantity = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["Quantity"].ToString());
                    obj.Rate = CommonFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

    }


}