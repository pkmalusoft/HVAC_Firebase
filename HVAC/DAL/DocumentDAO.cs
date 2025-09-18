using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using AttributeRouting.Helpers;
using HVAC.Models;

namespace HVAC.DAL
{
    public class DocumentDAO
    {

        #region Document
        public static List<DocumentMasterVM> GetCashBankDocument(int Id)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCashBankDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
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
                    obj.AcJournalID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<DocumentMasterVM> GetCompanyDocument(int Id)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCompanyDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
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
                    //obj.CompanyID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<DocumentMasterVM> GetBranchDocument(int Id)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetBranchDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
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
                    //obj.AcJournalID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<DocumentMasterVM> GetEmployeeDocument(int Id)
        {
            string url = ConfigurationManager.AppSettings["wasabiurl1"];
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommonFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetEmployeeDocument";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
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
                    // obj.AcJournalID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    obj.DocumentTypeName = ds.Tables[0].Rows[i]["DocumentTypeName"].ToString();
                    obj.Filename = ds.Tables[0].Rows[i]["FileName"].ToString();
                    obj.FilePath = url + obj.Filename;
                    objList.Add(obj);
                }
            }
            return objList;
        }


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
        #endregion
    }
}