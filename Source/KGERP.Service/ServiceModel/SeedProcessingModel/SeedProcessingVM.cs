﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel.SeedProcessingModel
{
    public class SeedProcessingVM : DefaultTableProperties
    {
        public long SeedProcessingId { get; set; }
        public string SeedProcessNo { get; set; }
        public DateTime? SeedProcessDate { get; set; }
        public string SeedProcessBy { get; set; }
        public int CompanyFK { get; set; }
        public bool IsSumitted { get; set; }
    }

    public class SeedProcessingDetailsVM : SeedProcessingVM
    {
        public long SeedProcessingDetailsId { get; set; }
        public int ProductId { get; set; }
        public long MaterialReceiveDetailId { get; set; }
        public decimal Amount { get; set; }
        public string MaterialReceiveNo { get; set; }
        public virtual string ProductName { get; set; }
        public virtual decimal ReceiveQty { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal? StockInQty { get; set; }


        public List<SeedProcessingDetailsVM> DataList { get; set; }

    }

    public partial class MaterialReceiveDetailsWithProductVM : DefaultTableProperties
    {
        public long SeedProcessingDetailsId { get; set; }
        public int ProductId { get; set; }
        public long MaterialReceiveDetailId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual decimal ReceiveQty { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal? StockInQty { get; set; }
        public decimal Amount { get; set; }

        public List<MaterialReceiveDetailsWithProductVM> DataListPro { get; set; }
    }
}