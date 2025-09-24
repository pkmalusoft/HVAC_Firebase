using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HVAC.Models
{
    public class PortCountryVM
    {
        public string Port { get; set; }
        public string PortCode { get; set; }
        public int PortType { get; set; }
        public string PortTypeText { get; set; }
        public string CountryName { get; set; }
        public int PortID { get; set; }
        public int CountryID { get; set; }
        public int CityID { get; set; }

        public string OriginCity { get; set; }
    }

    public class PortVM : Port
    {
        public string OriginCity { get; set; }
        public string OriginCountry { get; set; }
    }
}