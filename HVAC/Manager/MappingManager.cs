// Decompiled with JetBrains decompiler
// Type: HealthCareApp.MappingManager
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\Net4Courier.dll

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
