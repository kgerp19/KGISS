using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
  public  class ErpLogViewModel
    {
        public long LogId { get; set; }
        public int CompanyId { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public string IntegretFrom { get; set; }


    }
}
