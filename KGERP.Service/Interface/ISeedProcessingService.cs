using KGERP.Service.ServiceModel.SeedProcessingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface ISeedProcessingService
    {
        Task<long> CreateSeepProcessing(SeedProcessingDetailsVM modelSeed, MaterialReceiveDetailsWithProductVM materialReceiveDetailsWithProductVM);
        Task<SeedProcessingDetailsVM> GetDataSeedProcessingAndDetailsBySeedProcessingId(long seedProcessingId);
        Task<SeedProcessingDetailsVM> GetDataSeedProcessingList(int companyId);
        Task<long> DeleteSeedProcessingDetails(long SeedProcessingDetailsId);
        Task<long> DeleteSeedProcessing(long SeedProcessingId);
        Task<long> SeedProcessingDetailsUpdate(SeedProcessingDetailsVM detailsVM);
        Task<long> SeedProcessingUpdate(SeedProcessingDetailsVM detailsVM);
        Task<long> SeedProcessingSubmitted(SeedProcessingDetailsVM detailsVM);
    }
}
