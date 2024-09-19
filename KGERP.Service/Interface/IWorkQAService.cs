using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IWorkQAService
    {
        List<WorkQAModel> GetWorkQAs();
        //WorkQAModel GetWorkQA(int id);
        bool SaveWorkQA(long id, WorkQAModel model);
        WorkQAModel GetQuestion(int id);
        WorkQAModel GetAnswer(int id);
        long? GetCurrentEmpId();
        ICollection<WorkQAFileModel> GetWorkQAFiles(long workQAId);
        string GetAttachFile(long workQAFileId);
    }
}
