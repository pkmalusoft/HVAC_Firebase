using ClosedXML.Excel;
using HVAC.DAL;
using HVAC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HVAC.Controllers
{
    [SessionExpireFilter]
    public class ProductOpeningController : Controller
    {
        HVACEntities db = new HVACEntities();


        public ActionResult Index(string brand, string family)
        {
            List<StockOpeningVM> lst = new List<StockOpeningVM>();
            int BranchId = CommonFunctions.ParseInt(Session["CurrentBranchID"].ToString());           
          
            lst = EnquiryDAO.GetStockOpeningList(BranchId);
            if (!string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(family))
            {
               
                lst = lst.Where(x => x.BrandName == brand && x.ProductFamilyName == family).ToList();
            }
            else if (!string.IsNullOrEmpty(brand))
            {
               
                lst = lst.Where(x => x.BrandName == brand).ToList();
            }
            else if (!string.IsNullOrEmpty(family))
            {
                
                lst = lst.Where(x => x.ProductFamilyName == family).ToList();
            }

            return View(lst);
        }

       

        public ActionResult Create(int id=0)
        {
            StockOpeningVM v = new StockOpeningVM();
            StockOpening a = new StockOpening();
            int BranchId = CommonFunctions.ParseInt(Session["CurrentBranchID"].ToString());
            var branches = (from c in db.BranchMasters where c.BranchID == BranchId select new { BranchID = c.BranchID, BranchName = c.BranchName }).ToList();
            branches.Add(new { BranchID = -1, BranchName = "All Branch" });
            ViewBag.Branches = branches;
            //ViewBag.ProductFamily = db.ProductFamilies.OrderBy(cc => cc.ProductCategoryName).ToList();
            
            ViewBag.ItemUnit = db.ItemUnits.ToList();
            if (id>0)
            {
                ViewBag.Title = "Modify";
                a = (from c in db.StockOpenings where c.OpeningID == id select c).FirstOrDefault();
                v.EquipmentTypeID = a.EquipmentTypeID;

                v.OpeningID = a.OpeningID;
                
                v.ItemUnitID = 1;
                v.Quantity = a.Quantity;
                v.Rate = a.Rate;
                v.Value = a.Value;
                v.AsonDate = a.AsonDate;
                v.Model = a.Model;
                var _equipment = db.EquipmentTypes.Find(v.EquipmentTypeID);
                v.EquipmentType = _equipment.EquipmentType1;
                var _productfamily = db.ProductFamilies.Find(_equipment.ProductFamilyID);
                if (_productfamily != null)
                    v.ProductFamilyName = _productfamily.ProductFamilyName;
                var _brand = db.Brands.Find(_equipment.BrandID);
                    v.BrandName = _brand.BrandName;
                
            }
            else
            {
                ViewBag.Title = "Create";
                
                v.OpeningID = 0;
                v.AsonDate = CommonFunctions.GetBranchDateTime();

            }

            return View(v);
        }

   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveProductOpening(StockOpeningVM v)
        {
            int BranchId = CommonFunctions.ParseInt(Session["CurrentBranchID"].ToString());
            int UserID = CommonFunctions.ParseInt(Session["UserID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            StockOpening   a = new StockOpening();
            try
            {


                if (v.OpeningID == 0)
                {
                    var _fyear = db.AcFinancialYears.Find(yearid);
                    //duplicate checking
                    var duplicate = db.StockOpenings.Where(cc => cc.EquipmentTypeID == v.EquipmentTypeID && cc.Model==v.Model).FirstOrDefault();
                    if (duplicate != null)
                    {
                        return Json(new { status = "Failed", ProductID = 0, message = "Product Opening Exists to this Equipment with same Model!" });

                    }
                    a.EquipmentTypeID = v.EquipmentTypeID;
                    a.Model = v.Model;
                    a.ItemUnitID = v.ItemUnitID;
                    a.Quantity = v.Quantity;
                    a.Rate = v.Rate;
                    a.Value = v.Value;                    
                    a.CreatedBy = UserID;
                    a.CreatedDate = CommonFunctions.GetBranchDateTime();
                    a.AsonDate = v.AsonDate;

                    db.StockOpenings.Add(a);
                    db.SaveChanges();
                    EnquiryDAO.StockMasterGRNPosting(a.OpeningID, "Opening");
                    return Json(new { status = "OK", ProductID = 0, message = "Equipment Stock Opening Added Succesfully!" });
                }
                else
                {
                    var duplicate = db.StockOpenings.Where(cc => cc.OpeningID != v.OpeningID && cc.EquipmentTypeID==v.EquipmentTypeID && cc.Model==v.Model).FirstOrDefault();
                    if (duplicate != null)
                    {
                        return Json(new { status = "Failed", ProductID = 0, message = "Stock Opening already existing to this Equipment with same Model!" });

                    }
                    a = db.StockOpenings.Find(v.OpeningID);
                    a.EquipmentTypeID = v.EquipmentTypeID;
                    a.ItemUnitID = v.ItemUnitID;
                    a.Quantity = v.Quantity;
                    a.Rate = v.Rate;
                    a.Model = v.Model;
                    a.Value= v.Value;
                    
                    a.ModifiedBy = UserID;
                    a.ModifiedDate= CommonFunctions.GetBranchDateTime();

                    db.Entry(a).State = EntityState.Modified;
                    db.SaveChanges();

                    EnquiryDAO.StockMasterGRNPosting(a.OpeningID, "Opening");
                    return Json(new { status = "OK", OpeningID = a.OpeningID, message = "Product Opening Updated Succesfully!" });
                }

                

            }
            catch(Exception ex)
            {
                return Json(new { status = "Failed", ProductTypeID = 0, message = ex.Message });
            }
        }

       
        public JsonResult DeleteConfirmed(int id)
        {
            string status = "";
            string message = "";
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = EnquiryDAO.DeleteStockOpening(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            status = dt.Rows[0][0].ToString();
                            message = dt.Rows[0][1].ToString();
                         
                            return Json(new { status = status, message = message });
                        }

                    }
                    else
                    {
                        
                        return Json(new { status = "Failed", message = "Delete Failed!" });
                    }
                }
                else
                {
                 
                    return Json(new { status = "Failed", message = "Delete Failed!" });
                }
            }

            return Json(new { status = "Failed", message = "Delete Failed!" });
        }

        public ActionResult ProductOpeningExcel(string brand, string family)
        {

            string title = "Product Opening List";
            List<StockOpeningVM> lst = new List<StockOpeningVM>();
            int BranchId = CommonFunctions.ParseInt(Session["CurrentBranchID"].ToString());

            lst = EnquiryDAO.GetStockOpeningList(BranchId);
            if (!string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(family))
            {

                lst = lst.Where(x => x.BrandName == brand && x.ProductFamilyName == family).ToList();
            }
            else if (!string.IsNullOrEmpty(brand))
            {

                lst = lst.Where(x => x.BrandName == brand).ToList();
            }
            else if (!string.IsNullOrEmpty(family))
            {

                lst = lst.Where(x => x.ProductFamilyName == family).ToList();
            }
           
            byte[] excelBytes = GenerateExcel(title, lst);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProductOpening.xlsx");

        }
        private byte[] GenerateExcel(string sheetName, List<StockOpeningVM> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add(string.IsNullOrEmpty(sheetName) ? "Sheet1" : sheetName);

                if (data == null || !data.Any())
                {
                    ws.Cell(1, 1).Value = "No Data Available";
                }
                else
                {
                    // 🔹 Define headers exactly like in table
                    string[] headers = { "Brand", "Product Name", "Model", "Unit", "Product Family", "Quantity", "Rate", "Value" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(1, i + 1).Value = headers[i];
                        ws.Cell(1, i + 1).Style.Font.Bold = true;
                        ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    // 🔹 Fill rows
                    int row = 2;
                    foreach (var item in data)
                    {
                        ws.Cell(row, 1).Value = item.BrandName;
                        ws.Cell(row, 2).Value = item.EquipmentType;
                        ws.Cell(row, 3).Value = item.Model;
                        ws.Cell(row, 4).Value = item.ItemUnit;
                        ws.Cell(row, 5).Value = item.ProductFamilyName;

                        ws.Cell(row, 6).Value = item.Quantity;
                        ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        ws.Cell(row, 7).Value = item.Rate;
                        ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        ws.Cell(row, 8).Value = item.Value;
                        ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        row++;
                    }

                    ws.Columns().AdjustToContents(); // Auto-fit
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


    }
}