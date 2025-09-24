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
    public class LocationDAO
    {
        public static List<LocationVM> GetLocation(string term)
        {
            using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "SP_QryGetLocation";
                cmd.CommandType = CommandType.StoredProcedure;
                if (term == null)
                {
                    term = "";
                }
                cmd.Parameters.AddWithValue("@term", term);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                List<LocationVM> objList = new List<LocationVM>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        LocationVM obj = new LocationVM();
                        obj.LocationID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["LocationID"].ToString());
                        obj.CityID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["CityID"].ToString());
                        obj.CountryID = CommonFunctions.ParseInt(ds.Tables[0].Rows[i]["CountryID"].ToString());
                        obj.Location = ds.Tables[0].Rows[i]["Location"].ToString();
                        obj.CityName = ds.Tables[0].Rows[i]["City"].ToString();
                        obj.CountryName = ds.Tables[0].Rows[i]["CountryName"].ToString();
                        objList.Add(obj);
                    }
                }
                return objList;
            }
        }
    }
}