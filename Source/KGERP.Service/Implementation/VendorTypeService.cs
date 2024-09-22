using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class VendorTypeService : IVendorTypeService
    {
        private readonly ERPEntities context;
        public VendorTypeService(ERPEntities context)
        {
            this.context = context;
        }
        public List<VendorTypeModel> GetVendorTypes()
        {
            IQueryable<Data.Models.VendorType> vendors = context.VendorTypes.AsQueryable();
            return ObjectConverter<VendorType, VendorTypeModel>.ConvertList(vendors.ToList()).ToList();

        }

        public List<SelectModel> GetVendorTypeSelectModels()
        {
            return context.VendorTypes.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VendorTypeId
            }).ToList();
        }
    }
}
