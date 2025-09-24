// Decompiled with JetBrains decompiler
// Type: HVAC.Models.EmployeeVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\Net4Courier.dll

using System;
using System.Collections.Generic;

namespace HVAC.Models
{
    public class ClientMasterVM
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string LocationName { get; set; }
        public int CityID { get; set; }
        public int CountryID { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string ClientType { get; set; }
        public string ClientPrefix { get; set; }

        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string VATNo { get; set; }

    }

    public class ClientMasterSearch
    {
        public string ClientType { get; set; }
        public List<ClientMasterVM> Details { get; set; }
    }

    public class EquipmentTypeSearch
    {
        public int BrandID { get; set; }
        public List<EquipmentTypeVM> Details { get; set; }
    }
}
