﻿// Decompiled with JetBrains decompiler
// Type: HVAC.Models.SourceMastersModel
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\Net4Courier.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HVAC.Models
{
  public class SourceMastersModel
  {
        HVACEntities db = new HVACEntities();
        public bool IsAccessibleMenu(int menuID, List<int> RoleIDs)
        {      
          List<MenuAccessLevel> list = db.MenuAccessLevels.Join((IEnumerable<Menu>) db.Menus, (Expression<Func<MenuAccessLevel, int?>>) (t => t.MenuID), (Expression<Func<Menu, int?>>) (role => (int?) role.MenuID), (t, role) => new{ t = t, role = role }).Where(data => data.t.MenuID == (int?) menuID && RoleIDs.Contains(data.t.RoleID.Value)).Select(data => data.t).ToList<MenuAccessLevel>();
          return list != null && list.Count > 0;
        }

        public static string GetUserFullName(int userid,string usertype)
        {
            HVACEntities db = new HVACEntities();
            string userfullname="";
            if (usertype=="Employee")
            {
                var emp = db.EmployeeMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
                userfullname = emp.FirstName;
            }
            else if(usertype=="Customer")
            {
                var customer = db.CustomerMasters.Where(cc => cc.UserID == userid).FirstOrDefault();
                userfullname = customer.CustomerName;
            }            
            else
            {
                userfullname = "User";
            }
            return userfullname;


        }
        #region MenuAccesslevel

        public bool GetAddpermission(int RoleId, string href)
        {
            var menus = db.Menus;
            var menu = menus.Where(d => d.Link.Contains(href)).FirstOrDefault();
            if (menu != null)
            {
                var menuid = menu.MenuID;
                var permission = db.MenuAccessLevels.Where(d => d.RoleID == RoleId && d.MenuID == menuid && d.IsAdd == true).FirstOrDefault();
                if (permission != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetModifypermission(int RoleId, string href)
        {
            var menus = db.Menus;
            var menu = menus.Where(d => d.Link.Contains(href)).FirstOrDefault();
            if (menu != null)
            {
                var menuid = menu.MenuID;
                var permission = db.MenuAccessLevels.Where(d => d.RoleID == RoleId && d.MenuID == menuid && d.IsModify == true).FirstOrDefault();
                if (permission != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetDeletepermission(int RoleId, string href)
        {
            var menus = db.Menus;
            var menu = menus.Where(d => d.Link.Contains(href)).FirstOrDefault();
            if (menu != null)
            {
                var menuid = menu.MenuID;
                var permission = db.MenuAccessLevels.Where(d => d.RoleID == RoleId && d.MenuID == menuid && d.IsDelete == true).FirstOrDefault();
                if (permission != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetPrintpermission(int RoleId, string href)
        {
            var menus = db.Menus;
            var menu = menus.Where(d => d.Link.Contains(href)).FirstOrDefault();
            if (menu != null)
            {
                var menuid = menu.MenuID;
                var permission = db.MenuAccessLevels.Where(d => d.RoleID == RoleId && d.MenuID == menuid && d.Isprint == true).FirstOrDefault();
                if (permission != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetViewpermission(int RoleId, string href)
        {
            var menus = db.Menus;
            var menuid = menus.Where(d => d.Link.Contains(href)).FirstOrDefault().MenuID;
            var permission = db.MenuAccessLevels.Where(d => d.RoleID == RoleId && d.MenuID == menuid && d.IsView == true).FirstOrDefault();
            if (permission != null)
            {
                return true;
            }
            return false;
        }
        #endregion
 

        
        
     

        #region
        public List<CurrencyMaster> GetCurrency()
        {
            var query = db.CurrencyMasters.OrderBy(x => x.CurrencyName).ToList();

            return query;
        }

        public List<CurrencyMaster> GetCurrency(string Term)
        {
            var query = db.CurrencyMasters.Where(c => c.CurrencyName.ToLower().Contains(Term.ToLower())).OrderBy(x => x.CurrencyName).ToList();

            return query;
        }
        public List<CurrencyMaster> GetCurrencyById(int Id)
        {
            var query = db.CurrencyMasters.Where(c => c.CurrencyID.Equals(Id)).ToList();
            return query;
        }
        #endregion


        #region reportheading
        public static string GetCompanyname(int branchId)
        {
            HVACEntities db = new HVACEntities();
            string reportheader = "";
            
            reportheader = db.BranchMasters.Find(branchId).BranchName;
            

            return reportheader;


        }
        public static string GetCompanyLocation(int branchId)
        {
            HVACEntities db = new HVACEntities();
            string reportheader = "";

            reportheader = db.BranchMasters.Find(branchId).LocationName;


            return reportheader;


        }
        public static string GetCompanyAddress(int branchId)
        {
            HVACEntities db = new HVACEntities();
            string reportheader = "";

            var branch = db.BranchMasters.Find(branchId);
            reportheader = branch.Address1 + "," + branch.Address2 + "," + branch.Address3 + "\n" + branch.CityName + "," +  branch.CountryName; ;
            return reportheader;


        }
        public static string GetReportHeader1(int branchId)
        {
            HVACEntities db = new HVACEntities();
            string reportheader = "";
            var  setuptype= db.GeneralSetupTypes.Where(cc => cc.TypeName == "ReportHeader1").FirstOrDefault();
            if (setuptype==null)
            {
                reportheader = db.BranchMasters.Find(branchId).BranchName;
            }
            else
            {
                var setup = db.GeneralSetups.Where(cc => cc.BranchId == branchId && cc.SetupTypeID==setuptype.ID).FirstOrDefault();
                if (setup!=null)
                {
                    reportheader = setup.Text1;
                }
                else
                {
                    reportheader = db.BranchMasters.Find(branchId).BranchName;
                }
            }
            
            return reportheader;


        }

        public static string GetReportHeader2(int branchId)
        {
            HVACEntities db = new HVACEntities();
            string reportheader = "";
            var branch = db.BranchMasters.Find(branchId);
            var setuptype = db.GeneralSetupTypes.Where(cc => cc.TypeName == "ReportHeader2").FirstOrDefault();
            if (setuptype == null)
            {
                reportheader = branch.CityName + " , " + branch.CountryName;
            }
            else
            {
                var setup = db.GeneralSetups.Where(cc => cc.BranchId == branchId && cc.SetupTypeID == setuptype.ID).FirstOrDefault();
                if (setup != null)
                {
                    reportheader = setup.Text1;
                }
                else
                {

                    reportheader = branch.CityName + " ," +  branch.CountryName;
                }
            }

            return reportheader;


        }

        public static int GetCompanyCurrencyID (int companyid)
        {
            HVACEntities db = new HVACEntities();
            var company = db.AcCompanies.Find(companyid);
            if (company != null)
            {
                if (company.CurrencyID != null)
                    return Convert.ToInt32(company.CurrencyID);
                else
                    return 0;
            }
            else
                return 0;
        }
        #endregion
        
        public List<Menu> GetMenuSidebar(int RoleId)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());

            if (RoleId == 1)
            {
                var Query = (from t in db.Menus where t.IsAccountMenu.Value == false && t.RoleID == null && t.MenuID!=3 orderby t.MenuOrder select t).ToList();
                
                return (Query);
            }
            else
            {
                //List<Menu> Query2 = new List<Menu>();
                var Query = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.MenuID where t1.RoleID == RoleId && t.IsAccountMenu.Value == false  orderby t.MenuOrder select t).ToList();

                var Query1 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == RoleId && t.ParentID == 0 && t.IsAccountMenu.Value == false  orderby t.MenuOrder select t).ToList();

                var Query2 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == RoleId && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

                if (Query2 != null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query2.Add(q);
                    }
                }

                if (Query1 != null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query.Add(q);
                    }
                }

                return Query;
            }

        }
    }
}
