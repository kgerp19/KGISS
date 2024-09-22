using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.ServiceModel
{
    public class StockInfoModel
    {
        public int StockInfoId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string StockType { get; set; }
        public bool IsActive { get; set; }
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;
        public virtual CompanyModel Company { get; set; }
        public IEnumerable<StockInfoModel> DataList { get; set; }
        public long DepotInchargeEmpId { get; set; }
        public string DepotInchargeName {get; set; }
        public string EmployeeId { get; set; }
    }
}
