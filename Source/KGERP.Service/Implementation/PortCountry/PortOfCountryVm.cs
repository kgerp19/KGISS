using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.PortCountry
{
    public class PortOfCountryVm
    {
        public int PortOfCountryId { get; set; }
        public string PortName { get; set; }
        public string countryName { get; set; }
        public int CountryId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<PortOfCountryVm> DataList { get; set; }
        public List<Country> Countrylist { get; set; } 

        public virtual Country Country { get; set; }
    }
}
