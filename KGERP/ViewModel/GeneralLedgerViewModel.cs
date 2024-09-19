using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class VoucherViewModel
    {
        public VoucherModel Voucher { get; set; }
        public List<VoucherModel> Vouchers { get; set; }
        public List<VoucherDetailModel> VoucherDetails { get; set; }
        public List<SelectModel> VoucherTypes { get; set; }
        public List<SelectModel> CostCenters { get; set; }

    }
}