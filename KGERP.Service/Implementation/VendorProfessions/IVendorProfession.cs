using KGERP.Data.Models;
using KGERP.Service.Implementation.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.VendorProfessions
{
    public interface IVendorProfession
    {
        VendorProfessionVm GetVendorProfessionVm();
        bool AddNewName(VendorProfession model);
        bool Editsave(VendorProfession model);
        bool DeletVenodorByiD(int idl);
        VendorProfession GetNameById(int  id);
        List<object> getfordropdown();
    }
}
