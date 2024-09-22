using KGERP.Data.Models;
using KGERP.Service.Implementation.Audit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Audit
{
    public interface IAuditService
    {
        IEnumerable<PreservingAuditDocumentVM> GetAllAuditDocument(int?companyId,int? year, int? month, int? type, int companyId2 = 0);
        PreservingAuditDocumentVM GetAuditDocumentById(long id);
        Task<PreservingAuditDocumentVM> GetAuditDocumentFileAsync(long id);
        bool AddAuditDocument(PreservingAuditDocumentVM modelVM);

        bool UpdateAuditDocument(PreservingAuditDocumentVM modelVM);
        bool DeleteAuditDocument(long id);

    }
}
