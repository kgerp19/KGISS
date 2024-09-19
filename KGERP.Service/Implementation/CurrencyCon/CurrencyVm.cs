using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.CurrencyCon
{
    public class CurrencyVm
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public decimal? CurrencyRate { get; set; }
        public string Sign { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<CurrencyVm> DataList { get; set; }
        public CurrencyVm currencyVm { get; set; }

    }
}
