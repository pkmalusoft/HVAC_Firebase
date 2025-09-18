
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public  class DocumentMasterVM  :DocumentMaster
    {
      
        
        public string DocumentTypeName { get; set; }       
        public string Filename { get; set; }
        public string FilePath { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }

        public string UserName { get; set; }
        public List<DocumentMasterVM> Details{ get; set; }
        
        
        
        
        
        
    }

    
}