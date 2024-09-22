using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KGERP.Utility;
using System.Threading.Tasks;
using KGERP.Service.ServiceModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel.FTP_Models;

namespace KGERP.Service.Implementation.Realestate
{
    public interface IGLDLCustomerService
    {
        Task<GLDLBookingViewModel> CustomerBokking(GLDLBookingViewModel model);
        Task<GLDLBookingViewModel> SPCustomerBokking(GLDLBookingViewModel model);
        Task<GLDLBookingViewModel> PductChangingStatus(GLDLBookingViewModel model);
        Task<GLDLBookingViewModel> UpdateCustomerBokking(GLDLBookingViewModel model);
        Task<List<SelectModelType>> GetbyEmployee(int companyId);
        Task<CustomerNominee> AddCustomerNominee(CustomerNominee nominee);
        Task<CustomerNominee> UpdateNominee(CustomerNominee nominee);
        Task<CustomerNominee> DeleteNominee(CustomerNominee nominee);
        Task<int> GetByclient(long clientId);
        Task<CustomerNominee> GetByNominee(long id);
        Task<NomineeFile> FileUpdateNominee(NomineeFile nomineeFile, long ImageDocId, long NIDDocId);
        Task<bool> UpdateNomineeImageDociId(long id,long docId);
        Task<bool> UpdateNomineeNIDDociId(long id,long docId);
        Task<bool> DeleteCGFile(long docId, long CGId);
        Task<bool> FileMapping(List<FileItem> itemlist, long CGId);
        object GetCustomerAutoComplete(string prefix, int companyId);
        GLDLBookingViewModel bookingFilecheck(int companyId,string prefix);
        Task<List<SelectModelType>> GetbyMember(int companyId);
        

    }
}
