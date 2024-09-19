using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Audit.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace KGERP.Service.Implementation.Audit
{
    public class AuditService : IAuditService
    {
        private readonly ERPEntities _context;
        public AuditService(ERPEntities eRPEntities)
        {
            _context = eRPEntities;
        }
        public bool AddAuditDocument(PreservingAuditDocumentVM modelVM)
        {

            var obj = _context.PreservingAuditDocuments.Where(x => x.Month == modelVM.Month && x.Year == modelVM.Year).OrderByDescending(s => s.ID).Select(y => y.DocumentNo).FirstOrDefault();

            if (obj != null)
            {
                int hyphenIndex = obj.IndexOf('-');
                string word = obj.Substring(hyphenIndex + 1);
                int number = int.Parse(word);
                int increae = number + 1;
                string numberString = increae.ToString();
                string finalnumber = "AIF-00" + numberString;
                modelVM.DocumentNo = finalnumber;
            }
            else
            {
                modelVM.DocumentNo = "AIF-001";
            }
            PreservingAuditDocument model = new PreservingAuditDocument()
            {
                CompanyId = modelVM.CompanyId,
                Type = (int)modelVM.Type,
                ProcedureeApplied = modelVM.ProcedureeApplied,
                AuditRecommendationPrimary = modelVM.AuditRecommendationPrimary,
                AuditRecommendationFinal = modelVM.AuditRecommendationFinal,
                ConcernedFeedback = modelVM.ConcernedFeedback,
                Observation = modelVM.Observation,
                IsActive = true,
                Year = modelVM.Year,
                Month = modelVM.Month,
                CreatedBy = Common.GetUserId(),
                CreatedDate = DateTime.Now,
                DocumentNo = modelVM.DocumentNo,
                Title=modelVM.Title
                
            };

            _context.PreservingAuditDocuments.Add(model);
            
            if (_context.SaveChanges() > 0)
            {
                modelVM.Id = model.ID;
                return true;

            }
            return false;
        }

        public bool DeleteAuditDocument(long id)
        {
            var exist = _context.PreservingAuditDocuments.FirstOrDefault(x => x.ID == id);
            if (exist != null)
            {
                exist.IsActive = false;
                exist.ModifiedBy = Common.GetUserId();
                exist.ModifiedDate = DateTime.Now;
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<PreservingAuditDocumentVM> GetAllAuditDocument(int?companyid,int? year, int? month, int? type, int companyId2 = 0)
        {
           
            
                DateTime currentDate = DateTime.Now;
                DateTime twoMonthsAgo = currentDate.AddMonths(-2);
                int monthTwoMonthsAgo = twoMonthsAgo.Month;
                //month = month == null ? monthTwoMonthsAgo : month;
                // companyId2 = companyId2 == 0? null: companyId2;
                type = type == 0 ? null : type;



                var  data = (from model in _context.PreservingAuditDocuments
                            join company in _context.Companies on model.CompanyId equals company.CompanyId
                            where model.IsActive
                             //&& (!fromDate.HasValue || model.CreatedDate >= fromDate)
                             //&& (!toDate.HasValue || model.CreatedDate <= toDate)
                             && (month==null?model.Month>= monthTwoMonthsAgo:model.Month==month)
                            && (!type.HasValue || model.Type == type)
                            && (!year.HasValue || model.Year == year)
                            && (companyId2 == 0 ? model.CompanyId > 0 : model.CompanyId == companyId2)
                            select new PreservingAuditDocumentVM()
                            {
                                Id = model.ID,
                                CompanyId = model.CompanyId,
                                CompanyName = company.Name,
                                Type = (PreservingAuditDocumentTypeEnum)model.Type,
                                ProcedureeApplied = model.ProcedureeApplied,
                                AuditRecommendationPrimary = model.AuditRecommendationPrimary,
                                AuditRecommendationFinal = model.AuditRecommendationFinal,
                                ConcernedFeedback = model.ConcernedFeedback,
                                Observation = model.Observation,
                                IsActive = true,
                                Year = model.Year,
                                Month = model.Month,
                                MonthName = ((MonthList)model.Month).ToString(),
                                Title=model.Title,
                            }).OrderByDescending(x => x.Id).AsNoTracking();
                return data;

      
        }

        public PreservingAuditDocumentVM GetAuditDocumentById(long id)
        {
            var data = (from model in _context.PreservingAuditDocuments
                        join company in _context.Companies on model.CompanyId equals company.CompanyId
                        where model.ID == id && model.IsActive
                        select new PreservingAuditDocumentVM()
                        {
                            Id = model.ID,
                            CompanyId = model.CompanyId,
                            CompanyName = company.Name,
                            Type = (PreservingAuditDocumentTypeEnum)model.Type,
                            ProcedureeApplied = model.ProcedureeApplied,
                            AuditRecommendationPrimary = model.AuditRecommendationPrimary,
                            AuditRecommendationFinal = model.AuditRecommendationFinal,
                            ConcernedFeedback = model.ConcernedFeedback,
                            Observation = model.Observation,
                            IsActive = true,
                            Year = model.Year,
                            Month = model.Month,
                            MonthName = ((MonthList)model.Month).ToString(),
                            Title=model.Title,
                        }).AsNoTracking().FirstOrDefault();
            return data;
        }

        public async Task<PreservingAuditDocumentVM> GetAuditDocumentFileAsync(long id)
        {

            PreservingAuditDocumentVM vm = new PreservingAuditDocumentVM();

            vm = (from model in _context.PreservingAuditDocuments
                        join company in _context.Companies on model.CompanyId equals company.CompanyId
                        where model.ID == id && model.IsActive
                        select new PreservingAuditDocumentVM()
                        {
                            Id = model.ID,
                            CompanyId = model.CompanyId,
                            CompanyName = company.Name,
                            Type = (PreservingAuditDocumentTypeEnum)model.Type,
                            ProcedureeApplied = model.ProcedureeApplied,
                            AuditRecommendationPrimary = model.AuditRecommendationPrimary,
                            AuditRecommendationFinal = model.AuditRecommendationFinal,
                            ConcernedFeedback = model.ConcernedFeedback,
                            Observation = model.Observation,
                            IsActive = true,
                            Year = model.Year,
                            Month = model.Month,
                            DocumentNo = model.DocumentNo,
                            CreatedBy=model.CreatedBy,
                            MonthName = ((MonthList)model.Month).ToString(),
                            Title=model.Title,
                        }).AsNoTracking().FirstOrDefault();





            vm.Attachments = await Task.Run(() => (from t1 in _context.FileArchives.Where(x => x.FileCatagoryId == 7)
                                                                 join t2 in _context.CustomerBookingFileMappings on t1.docid equals t2.DocId
                                                                 where t2.CGId == (long)id && t2.IsActive
                                                                 select new GLDLBookingAttachment
                                                                 {
                                                                     DocId = t2.DocId,
                                                                     Title = t2.FileTitel,
                                                                     Extension = t1.fileext

                                                                 }).ToListAsync());

            return vm;
        }





        public bool UpdateAuditDocument(PreservingAuditDocumentVM modelVM)
        {
            var exist = _context.PreservingAuditDocuments.FirstOrDefault(x => x.ID == modelVM.Id);
            if (exist != null)
            {
                exist.CompanyId = modelVM.CompanyId;
                 exist.Type = (int)modelVM.Type;
                 exist.ProcedureeApplied = modelVM.ProcedureeApplied;
                 exist.AuditRecommendationPrimary = modelVM.AuditRecommendationPrimary;
                 exist.AuditRecommendationFinal = modelVM.AuditRecommendationFinal;
                 exist.ConcernedFeedback = modelVM.ConcernedFeedback;
                 exist.Observation = modelVM.Observation;
                 exist.Year = modelVM.Year;
                 exist.Month = modelVM.Month;
                exist.ModifiedBy = Common.GetUserId();
                exist.CreatedDate = DateTime.Now;
                exist.Title = modelVM.Title;
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }           
            return false;
        }
    }
}
