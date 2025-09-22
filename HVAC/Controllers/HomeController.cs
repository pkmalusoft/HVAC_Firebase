using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Configuration;
using System.Threading.Tasks;
 

namespace HVAC.Controllers
{
    public class HomeController : Controller
    {
     
        HVACEntities db=new HVACEntities();

        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest1;
        private static readonly string wasabiurl = ConfigurationManager.AppSettings["wasabiurl"];

        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static readonly string wasabiurl1 = ConfigurationManager.AppSettings["wasabiurl1"];

        /// <summary>
        /// Displays the home page with company information and session status
        /// </summary>
        /// <returns>Home view with company details and session information</returns>
        public ActionResult Home()
        {
            var compdetail = db.AcCompanies.FirstOrDefault();
            ViewBag.CompanyName = compdetail.AcCompany1;
            ViewBag.ContactPerson = compdetail.KeyPerson;
            ViewBag.ContactTelephone = compdetail.Phone;
            ViewBag.Email = compdetail.EMail;
            ViewBag.BranchCount = db.BranchMasters.Count();

            if (compdetail.AcceptSystem == true)
                ViewBag.AcceptSystem = "YES";
            else
                ViewBag.AcceptSystem = "NO";

            if (Session["LoginStatus"]!=null)
            {
                TempData["LoginErrorMsg"] = Session["StatusMessage"].ToString();
                TempData["Modal"] = "Login";
            }
            else if (Session["ForgotStatus"] != null)
            {
                TempData["ForgotErrorMsg"] = Session["StatusMessage"].ToString();
                TempData["Modal"] = "Forgot";
            }
            else if (Session["ResetStatus"] != null)
            {
                TempData["ResetErrorMsg"] = Session["StatusMessage"].ToString();
                TempData["Modal"] = "Reset";
            }
            else
            {
                TempData["LoginErrorMsg"] = null;
                TempData["Modal"] = null;
                TempData["ResetErrorMsg"] = null;
                TempData["LoginErrorMsg"] = null;
            }

            Session["LoginStatus"] = null;
            Session["StatusMessage"] = null;
            Session["ResetStatus"] = null;
            Session["ForgotStatus"] = null;
            return View();
            
           
        }

        /// <summary>
        /// Handles file upload to local server with validation
        /// </summary>
        /// <returns>JSON result with upload status and file information</returns>
        [HttpPost]
        public ActionResult UploadFileswork()
        {
            string fname = "";
            string filename = "";
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        
                        // Validate file
                        if (file == null || file.ContentLength == 0)
                            continue;
                            
                        // Check file size (10MB limit)
                        if (file.ContentLength > 10 * 1024 * 1024)
                        {
                            return Json(new { status = "Failed", FileName = "", message = "File size exceeds 10MB limit." });
                        }
                        
                        // Validate file extension
                        string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };
                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            return Json(new { status = "Failed", FileName = "", message = "File type not allowed." });
                        }


                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            filename = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;
                            filename = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/UploadDocuments/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(new { status = "ok", FileName = filename, message = "File Uploaded Successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = "Failed", FileName = "", message = ex.Message });
                    //return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json(new { status = "Failed", FileName = "", message = "No files selected." });
            }
        }
        private static IAmazonS3 _s3Client;

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public async Task<string> UploadFile(string objName, string pathAndFileName)
        {
            try
            {
                // 1. this is necessary for the endpoint
                //var config = new AmazonS3Config { ServiceURL = "https://s3.us-west-1.wasabisys.com/truebook/courier" };
                var config = new AmazonS3Config { ServiceURL = wasabiurl };
                // this will allow you to call whatever profile you have
                //var credentials = new StoredProfileAWSCredentials("truebook");
                var credentials = new BasicAWSCredentials(accesskey, secretkey);
                _s3Client = new AmazonS3Client(credentials, config);
                var obj = $"{objName}";
                var path = $"{pathAndFileName}";
                var putRequest = new PutObjectRequest();
                putRequest.BucketName = bucketName;
                putRequest.Key = objName;
                putRequest.FilePath = pathAndFileName;

                putRequest.Metadata.Add("x-amz-meta-title", "someTitle");
                await _s3Client.PutObjectAsync(putRequest);
                return "OK";
            }
            catch (AmazonS3Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Handles file upload to cloud storage (Wasabi S3) with validation
        /// </summary>
        /// <returns>JSON result with upload status and file information</returns>
        [HttpPost]
        public async Task<ActionResult> UploadFiles()
        {
            string fname = "";
            string filename = "";
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        
                        // Validate file
                        if (file == null || file.ContentLength == 0)
                            continue;
                            
                        // Check file size (10MB limit)
                        if (file.ContentLength > 10 * 1024 * 1024)
                        {
                            return Json(new { status = "Failed", FileName = "", message = "File size exceeds 10MB limit." });
                        }
                        
                        // Validate file extension
                        string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };
                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            return Json(new { status = "Failed", FileName = "", message = "File type not allowed." });
                        }


                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                            filename = file.FileName;
                        }
                        else
                        {
                            fname = file.FileName;
                            filename = file.FileName;
                        }
                        Random rnd = new Random();
                        int num = rnd.Next();
                        fname = fname.Replace(" ", "_");
                        fname = "f" + num.ToString() + "_" + fname;
                        filename = fname;
                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/UploadDocuments/"), fname);
                        file.SaveAs(fname);
                        var result = await UploadFile(filename, fname);
                        if (result == "OK")
                            return Json(new { status = "ok", FileName = filename, message = "File Uploaded Successfully!" });
                        else
                            return Json(new { status = "Failed", FileName = "", message = "Upload Failed." });
                    }
                    return Json(new { status = "Failed", FileName = "", message = "No files selected." });
                }
                catch (Exception ex)
                {
                    return Json(new { status = "Failed", FileName = "", message = ex.Message });
                    //return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json(new { status = "Failed", FileName = "", message = "No files selected." });
            }
        }

        public async Task<string> DownloadFile1(string objName)
        {
            try
            {
                var obj = $"{objName}";
                var getRequest = new GetObjectRequest();
                getRequest.BucketName = bucketName;
                getRequest.Key = objName;
                var config = new AmazonS3Config { ServiceURL = wasabiurl };
                var credentials = new BasicAWSCredentials(accesskey, secretkey);
                _s3Client = new AmazonS3Client(credentials, config);
                //putRequest.Metadata.Add("x-amz-meta-title", "someTitle");
                var response = await _s3Client.GetObjectAsync(getRequest);
                string reportpath = Path.Combine(Server.MapPath("~/ReportsPDF/"), objName);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.WriteResponseStreamToFile(reportpath);
                    return "OK";
                }
                else
                {
                    return "Failed";
                }
            }
            catch (AmazonS3Exception ex)
            {
                return "Failed:" + ex.Message;
            }
        }
        public FileResult DownloadFile(string filename)
        {
            string filepath = Path.Combine(Server.MapPath("~/ReportsPDF/"), filename);
            //string filename = "AcOpeningRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx"; // Server.MapPath("~" + filePath);
            //string filepath =filename;

            byte[] fileBytes = GetFile(filepath);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }
        public ActionResult TrialExpireIndex()
        {
            var compdetail = db.AcCompanies.FirstOrDefault();
            ViewBag.CompanyName = compdetail.AcCompany1;
            ViewBag.ContactPerson = compdetail.KeyPerson;
            ViewBag.ContactTelephone = compdetail.Phone;
            ViewBag.Email = compdetail.EMail;
            ViewBag.BranchCount = db.BranchMasters.Count();
            return View();
        }
        [HttpPost]
        public ActionResult TrialExpire(CompanyVM u)
        {
            var accomp = u;
            if (u.Accept == true)
            {
                var accompany = db.AcCompanies.FirstOrDefault();
                
                accompany.AcceptSystem = true;
                db.Entry(accompany).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["TrialSuccessMessage"] = "Thank you for Accept our System!";
            }
            else
            {
                TempData["TrialMessage"] = "Sorry to see you go. Hope to be of service in future. Thank you. !!";
            }
                
            return RedirectToAction("Home", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public ActionResult UploadFile(FormCollection formCollection, string Command)
        {
            if (Request != null)
            {
                Session["fname"] = formCollection["fname"].ToString();

                string x = Session["fname"].ToString();
                HttpPostedFileBase filebase = Request.Files["UploadedFile"];
                if (!string.IsNullOrEmpty(filebase.FileName))
                {
                    if ((filebase.FileName.Contains(".xlsx") || filebase.FileName.Contains(".xls")))
                    {
                        MappingManager manager = new MappingManager();
                        TempData["SelectedFile"] = filebase;
                        return RedirectToAction("Mapping", "Mapping");
                    }
                    else
                    {
                        TempData["Message"] = "Please select excel file only.";
                    }
                }
                else
                {
                    TempData["Message"] = "Please select file.";
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UnderDevelopment()
        {
            return View();
        }


        [HttpPost]
        public JsonResult ChangeBranch(int id)
        {
            try
            {
                var branch = db.BranchMasters.Find(id);
              

                //int? branchid = (from u2 in db.tblDepots where u2.ID == id select u2.BranchID).FirstOrDefault();
                Session["CurrentBranchID"] = branch.BranchID;                                
                var finacialyear = db.AcFinancialYears.Where(cc => cc.CurrentFinancialYear == true).FirstOrDefault();
                Session["fyearid"] = finacialyear.AcFinancialYearID;
                Session["FyearFrom"] = finacialyear.AcFYearFrom;
                Session["FyearTo"] = finacialyear.AcFYearTo;

                int startyearid = Convert.ToInt32(db.BranchMasters.Find(1).AcFinancialYearID);
                DateTime branchstartdate = Convert.ToDateTime(db.AcFinancialYears.Find(startyearid).AcFYearFrom);
                Session["CurrentYear"]= (finacialyear.AcFYearFrom.Date.ToString("dd MMM yyyy") + " - " + finacialyear.AcFYearTo.Date.ToString("dd MMM yyyy"));
                var allyear = (from c in db.AcFinancialYears where c.BranchID == branch.BranchID  select c).OrderByDescending(cc=>cc.AcFYearFrom).ToList();
                Session["FYear"] = allyear;
                int currencyid = branch.CurrencyID.Value; //  (from c in db.AcCompanies where c.AcCompanyID == accid select c.CurrencyID).FirstOrDefault().Value;
                var currency = (from c in db.CurrencyMasters where c.CurrencyID == currencyid select c).FirstOrDefault();
                short? noofdecimals = currency.NoOfDecimals;
                string monetaryunit = currency.MonetaryUnit;

                Session["Decimal"] = noofdecimals;
                Session["MonetaryUnit"] = monetaryunit;
                if (currency.NumberFormat == 1)
                    Session["NumberFormat"] = "Lakhs";
                else
                    Session["NumberFormat"] = "Millions";
                Session["ReportOutput"] = null;
                Session["YearEndProcessSearch"] = null;
                return Json(new { status = "ok", depotname = branch.BranchName, message = "Active Depot Selection changed to "  }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = "Failed", depotname = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult ChangeYear(int id)
        {
            try
            {
                int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                AcFinancialYear finacialyear = db.AcFinancialYears.Find(id);
                if (finacialyear != null)
                {
                    Session["fyearid"] = finacialyear.AcFinancialYearID;
                    Session["CurrentYear"] = (finacialyear.AcFYearFrom.Date.ToString("dd MMM yyyy") + " - " + finacialyear.AcFYearTo.Date.ToString("dd MMM yyyy"));
                    Session["FyearFrom"] = finacialyear.AcFYearFrom;
                    Session["FyearTo"] = finacialyear.AcFYearTo;
                    Session["YearEndProcessSearch"] = null;
                }

                return Json(new { status = "ok", yearname = finacialyear.ReferenceName, message = "Financial Year Selected Changed to " + finacialyear.ReferenceName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed",  message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
