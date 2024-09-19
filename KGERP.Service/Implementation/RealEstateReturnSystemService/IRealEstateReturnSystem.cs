using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.RealEstateReturnSystemService
{
    public interface IRealEstateReturnSystem { 
    Task<RealEstateReturnSystemVM> GetFileInformation(long CgId);
    Task<int> AddReturn(RealEstateReturnSystemVM vM);
    Task<RealEstateReturnListVM> ReturnListn(int companyid);
    Task<RealEstateReturnListVM> KGEREReturnListn(int companyid);
    Task<int> RemoveReturn(long Id);
    Task<long> AccConfirmReturn(RealEstateReturnListVM vM, RealEstateReturnSystemVM systemVM);
    Task<long> AccTransfarFileConfirmReturn(RealEstateReturnListVM vM, RealEstateReturnSystemVM bookingVM);
    }

}
