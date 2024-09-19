using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IEducationService
    {
        List<EducationModel> GetEducations(long id);
        EducationModel GetEducation(long id, int educationId);
        bool SaveEducation(int id, EducationModel education, out string message);
        bool DeleteEducation(long id, int educationId);
        string GetCertificateName(int educationId);
    }
}
