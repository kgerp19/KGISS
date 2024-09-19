using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IVendorOfferService
    {
        List<VendorOfferModel> GetVendorOffers(int vendorId, string productType, string searchText);
        int InsertCustomerOffer(int companyId);
        VendorOfferModel GetVendorOffer(int id);
        bool SaveVendorOffer(long id, VendorOfferModel model);
    }
}
