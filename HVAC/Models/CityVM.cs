﻿// Decompiled with JetBrains decompiler
// Type: HVAC.Models.CityVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\Net4Courier.dll

namespace HVAC.Models
{
    public class CityVM
    {
        public int CityID { get; set; }

        public string CityCode { get; set; }

        public string City { get; set; }

        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public bool IsHub { get; set; }

        public string Country { get; set; }
        public int Id { get; set; } //for city id
        public string CountryCode { get; set; }
        public string CityName { get; set; }
        public bool Deleted { get; set; }
        public string ProvinceName { get; set; }
    }
}
