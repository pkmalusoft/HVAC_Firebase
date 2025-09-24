// Decompiled with JetBrains decompiler
// Type: HVAC.Models.CurrencyVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\Net4Courier.dll

namespace HVAC.Models
{
  public class CurrencyVM
  {
    public int CurrencyID { get; set; }

    public string CurrencyName { get; set; }

    public string Symbol { get; set; }

    public short NoOfDecimals { get; set; }

    public string MonetaryUnit { get; set; }

    public int CountryID { get; set; }

    public bool StatusBaseCurrency { get; set; }
        public string CountryName { get; set; }
        public decimal ExchangeRate { get; set; }
        public int NumberFormat { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class CurrencyRateVM :CurrencyRate { 
    
        public string BaseCurrency { get; set; }
        public string TransactionCurrency { get; set; }
        
    }

}
