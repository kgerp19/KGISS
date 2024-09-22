using System;

namespace KGERP.Service.ServiceModel
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string ISOCountryCodes { get; set; }
        public Nullable<double> CountryCode { get; set; }
        //public virtual ICollection<State> States { get; set; }
        //public virtual ICollection<VendorModel> Vendors { get; set; }
    }
}
