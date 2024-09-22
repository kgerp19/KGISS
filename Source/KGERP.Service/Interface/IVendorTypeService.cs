using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IVendorTypeService
    {
        List<VendorTypeModel> GetVendorTypes();
        List<SelectModel> GetVendorTypeSelectModels();
    }
}
