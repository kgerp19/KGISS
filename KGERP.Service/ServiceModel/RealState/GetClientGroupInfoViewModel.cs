using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel.RealState
{
  public  class GetClientGroupInfoViewModel
    {
        public string ClientName { get; set; }
        public long CGId { get; set; }
        public long BookingId { get; set; }
        public int? ClientId { get; set; }
        public string BookingNo { get; set; }
        public string ProjectName { get; set; }
        public string PlotName { get; set; }
        public string PlotNo { get; set; }
        public string FileNo { get; set; }
        public string BlockName { get; set; }
        public decimal BookingMoney { get; set; }
        public decimal DownPayment { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string ProductCatagoryName { get; set; }
        public string ProductSubCatagoryName { get; set; }

        public List<InstallmentScheduleShortModel> Schedule { get; set; }
        public List<SelectModelType> costHead { get; set; }

    }
}
