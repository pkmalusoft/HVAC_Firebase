using HVAC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace HealthCareApp
{
  public class MappingManager
  {
    private SqlConnection con;

    private void connection()
    {
      this.con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ToString());
    }

 
    public List<TName> GetTableList()
    {
      string appSetting = ConfigurationManager.AppSettings["DatabaseName"];
      List<TName> tnameList = new List<TName>();
      this.connection();
      SqlCommand sqlCommand = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + appSetting + "'", this.con);
      this.con.Open();
      SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
      while (sqlDataReader.Read())
        tnameList.Add(new TName()
        {
          TableName = sqlDataReader[0].ToString()
        });
      this.con.Close();
      return tnameList;
    }
  }
}
