using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IMemberBasePFSummaryService
    {

        bool SaveEmployeePF(int id, MemberBasePFSummaryModel model);
        List<MemberBasePFSummaryModel> GetMemberBasePFSummaryByEmployeeId(string EmployeeId);
        List<MemberBasePFSummaryModel> GetPFDetialsByEmployeeId(string EmployeeId);
        MemberBasePFSummaryModel GetPFLastMonthUpdatedByEmployeeId(string EmployeeId);
        List<MemberBasePFSummaryModel> ExportPFDetialsByEmployeeId(string EmployeeId);
    }
}
