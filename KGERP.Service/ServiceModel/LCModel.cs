using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class LCModel
    {
        public string PINo { get; set; }
        public int? HeadId { get; set; }
        public int OriginCountry { get; set; }
        public int ProductCountry { get; set; }
        public long LCId { get; set; }
        public string  LCNo { get; set; }
        public DateTime LCDate { get; set; }
        public string LCType { get; set; }
        public string Supplier  { get; set; }
        public int CompanyId { get; set; }
        public decimal LCValue { get; set; }
        public decimal FreighterCharge { get; set; }
        public decimal OtherCharge { get; set; }
        public bool IsSubmit { get; set; }
        public bool POCreated { get; set; }
        public string InsuranceNo { get; set; }
        public Nullable<decimal> PremiumValue { get; set; }
    }
    public class LCViewModel
    {
        public List<LCModel> DataList { get; set; }
        public int CompanyId { get; set; }
        public string StrToDate { get; set; }
        public string StrFromDate { get; set; }
        public Nullable<int> LCType { get; set; }
        public List<SelectModel> POtypeLst { get; set; }
    }
    public class LCCreateModel
    {
        public List<SelectModel> POtypeLst { get; set; }
        public List<LCSelectModel> LstSupplier { get; set; }
        public List<SelectModel> Countries { get; set; }
        public LCInfo LC { get; set; } = new LCInfo() {LCDate=DateTime.Now };

        public int CompanyId { get; set; }
    }
    public class LCSelectModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
