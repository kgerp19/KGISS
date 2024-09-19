using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class LandNLegalService : ILandNLegalService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<LandNLegalModel> GetLandNLegals(string searchText)
        {
            IQueryable<LandNLegal> landNLegals = null;
            landNLegals = context.LandNLegals.Where(x => x.CaseNo.Contains(searchText) || x.CaseStatus.Contains(searchText) || x.CaseType.Contains(searchText) || x.CourtName.Contains(searchText) || x.CaseStatus.Contains(searchText) || x.ResponsibleLayer.Contains(searchText)).OrderBy(x => x.OID);
            return ObjectConverter<LandNLegal, LandNLegalModel>.ConvertList(landNLegals.ToList()).ToList();
        }

        public LandNLegalModel GetLandNLegal(long id)
        {
            if (id == 0)
            {
                return new LandNLegalModel() { OID = id };
            }
            LandNLegal LandNLegal = context.LandNLegals.Include(x => x.FileAttachments).Where(x => x.OID == id).FirstOrDefault();
            return ObjectConverter<LandNLegal, LandNLegalModel>.Convert(LandNLegal);
        }

        public bool SaveLandNLegal(long id, LandNLegalModel model)
        {
            LandNLegal landNLegal = ObjectConverter<LandNLegalModel, LandNLegal>.Convert(model);
            bool result = false;
            if (id > 0)
            {
                landNLegal = context.LandNLegals.FirstOrDefault(x => x.OID == id);
                if (landNLegal != null)
                {
                    string caseHistory = string.Empty;

                    #region  //NextDate
                    if (landNLegal.NextDate == model.NextDate)
                    {
                        landNLegal.NextDate = model.NextDate;
                    }
                    else
                    {
                        DateTime? date1 = null;
                        DateTime? date2 = null;
                        if (landNLegal.NextDate != null)
                        {
                            date1 = (DateTime)landNLegal.NextDate;
                        }
                        if (model.PreviousDate1 != null)
                        {
                            date2 = (DateTime)model.PreviousDate1;
                        }
                        if (landNLegal.PreviousDate1 != null)
                        {
                            caseHistory = caseHistory + " * PreviousDate1 Changed from '" + landNLegal.PreviousDate1 + "' to '" + date1 + "'";
                            landNLegal.PreviousDate1 = date1;
                        }
                        else
                        {
                            if (date1 != null)
                            {
                                caseHistory = caseHistory + " * PreviousDate1 Changed to '" + date1 + "'";
                                landNLegal.PreviousDate1 = date1;
                            }
                        }
                        if (landNLegal.PreviousDate2 != null)
                        {
                            caseHistory = caseHistory + " * PreviousDate2 Changed from '" + landNLegal.PreviousDate2 + "' to '" + date2 + "'";
                            landNLegal.PreviousDate2 = date2;
                        }
                        else
                        {
                            if (date2 != null)
                            {
                                caseHistory = caseHistory + " * PreviousDate2 Changed to '" + date2 + "'";
                                landNLegal.PreviousDate2 = date2;
                            }
                        }

                        caseHistory = caseHistory + " * NextDate Changed from '" + landNLegal.NextDate + "' to '" + model.NextDate + "'";
                        landNLegal.NextDate = model.NextDate;
                    }

                    #endregion
                    #region//CaseSummary
                    if (landNLegal.CaseSummary == model.CaseSummary)
                    {
                        landNLegal.CaseSummary = model.CaseSummary;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.CaseSummary) && !string.IsNullOrEmpty(landNLegal.CaseSummary))
                        {
                            caseHistory = caseHistory + " * CaseSummary Delete from ' " + landNLegal.CaseSummary;
                        }
                        else if (!string.IsNullOrEmpty(model.CaseSummary) && string.IsNullOrEmpty(landNLegal.CaseSummary))
                        {
                            caseHistory = caseHistory + " * CaseSummary Added ' " + model.CaseSummary;
                        }
                        else
                        {
                            caseHistory = caseHistory + " * CaseSummary Changed from '" + landNLegal.CaseSummary + "' to '" + model.CaseSummary + "'";
                        }
                        landNLegal.CaseSummary = model.CaseSummary;
                    }
                    #endregion
                    #region//PlaintiffAppellant
                    if (landNLegal.PlaintiffAppellant == model.PlaintiffAppellant)
                    {
                        landNLegal.PlaintiffAppellant = model.PlaintiffAppellant;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * PlaintiffAppellant Changed from '" + landNLegal.PlaintiffAppellant + "' to '" + model.PlaintiffAppellant + "'";
                        landNLegal.PlaintiffAppellant = model.PlaintiffAppellant;
                    }
                    #endregion
                    #region//DefendantName
                    if (landNLegal.DefendantName == model.DefendantName)
                    {
                        landNLegal.DefendantName = model.DefendantName;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * DefendantName Changed from '" + landNLegal.DefendantName + "' to '" + model.DefendantName + "'";
                        landNLegal.DefendantName = model.DefendantName;
                    }
                    #endregion
                    #region//ResponsibleLayer
                    if (landNLegal.ResponsibleLayer == model.ResponsibleLayer)
                    {
                        landNLegal.ResponsibleLayer = model.ResponsibleLayer;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * ResponsibleLayer Changed from '" + landNLegal.ResponsibleLayer + "' to '" + model.ResponsibleLayer + "'";
                        landNLegal.ResponsibleLayer = model.ResponsibleLayer;
                    }
                    #endregion
                    #region//CaseStatus
                    if (landNLegal.CaseStatus == model.CaseStatus)
                    {
                        landNLegal.CaseStatus = model.CaseStatus;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * CaseStatus Changed from '" + landNLegal.CaseStatus + "' to '" + model.CaseStatus + "'";
                        landNLegal.CaseStatus = model.CaseStatus;
                    }
                    #endregion
                    #region//CourtName
                    if (landNLegal.CourtName == model.CourtName)
                    {
                        landNLegal.CourtName = model.CourtName;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * CourtName Changed from '" + landNLegal.CourtName + "' to '<b>" + model.CourtName + "</b>'";
                        landNLegal.CourtName = model.CourtName;
                    }
                    #endregion
                    #region//Remarks
                    if (landNLegal.Remarks == model.Remarks)
                    {
                        landNLegal.Remarks = model.Remarks;
                    }
                    else
                    {
                        caseHistory = caseHistory + " * Remarks Changed from '" + landNLegal.Remarks + "' to '<b>" + model.Remarks + "</b>'";
                        landNLegal.Remarks = model.Remarks;
                    }
                    #endregion
                    #region//AddCaseHistory 
                    if (!string.IsNullOrEmpty(caseHistory))
                    {
                        AddCaseHistory(caseHistory, landNLegal.OID, System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    #endregion
                    #region//AddCaseComments
                    if (!string.IsNullOrEmpty(model.CaseComments))
                    {
                        AddCaseComments(model.CaseComments, landNLegal.OID, System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    #endregion
                    #region //AddCaseAttachFile
                    //if (!string.IsNullOrEmpty(model.AttachFilePath))
                    //{
                    //    AddCaseAttachFile(model.AttachFilePath, landNLegal.OID, System.Web.HttpContext.Current.User.Identity.Name);
                    //}
                    #endregion
                }
                landNLegal.ModifiedDate = DateTime.Now;
                landNLegal.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                landNLegal.CaseNo = model.CaseNo;
                landNLegal.FilingDate = model.FilingDate;
                landNLegal.MobileNo = model.MobileNo;
                landNLegal.BusinessAddress = model.BusinessAddress;
                landNLegal.BusinessOrganizationName = model.BusinessOrganizationName;

                landNLegal.DefendantName = model.DefendantName;
                landNLegal.DefendantFatherName = model.DefendantFatherName;
                landNLegal.DefendantMotherName = model.DefendantMotherName;
                landNLegal.AdditionalDistrictCourt = model.AdditionalDistrictCourt;

                landNLegal.Division = model.Division;
                landNLegal.District = model.District;
                landNLegal.ThanaUpazila = model.ThanaUpazila;
                landNLegal.VillageMohalla = model.VillageMohalla;
                landNLegal.TradeLicenceNo = model.TradeLicenceNo;
                landNLegal.NIDnPassportNo = model.NIDnPassportNo;

                landNLegal.BankId = model.BankId;
                landNLegal.BankName = model.BankName;
                landNLegal.AccountName = model.AccountName;
                landNLegal.BranchName = model.BranchName;
                landNLegal.AccountNo = model.AccountNo;
                landNLegal.ChequeNumber = model.ChequeNumber;
                landNLegal.ChequeIssueDate = model.ChequeIssueDate;
                landNLegal.Amount = model.Amount;

                landNLegal.CaseType = model.CaseType;
                landNLegal.CompanyName = model.CompanyName;
            }
            else
            {
                landNLegal.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                landNLegal.CreatedDate = DateTime.Now;
                landNLegal.CaseNo = model.CaseNo;
                landNLegal.CourtName = model.CourtName;
                landNLegal.FilingDate = model.FilingDate;
                landNLegal.CaseStatus = model.CaseStatus;
                landNLegal.MobileNo = model.MobileNo;
                landNLegal.BusinessAddress = model.BusinessAddress;
                landNLegal.BusinessOrganizationName = model.BusinessOrganizationName;
                landNLegal.DefendantName = model.DefendantName;
                landNLegal.DefendantFatherName = model.DefendantFatherName;
                landNLegal.DefendantMotherName = model.DefendantMotherName;
                landNLegal.Division = model.Division;
                landNLegal.District = model.District;
                landNLegal.ThanaUpazila = model.ThanaUpazila;
                landNLegal.VillageMohalla = model.VillageMohalla;
                landNLegal.TradeLicenceNo = model.TradeLicenceNo;
                landNLegal.NIDnPassportNo = model.NIDnPassportNo;

                landNLegal.BankId = model.BankId;
                landNLegal.BankName = model.BankName;
                landNLegal.AccountName = model.AccountName;
                landNLegal.BranchName = model.BranchName;
                landNLegal.AccountNo = model.AccountNo;
                landNLegal.ChequeNumber = model.ChequeNumber;
                landNLegal.ChequeIssueDate = model.ChequeIssueDate;
                landNLegal.Amount = model.Amount;
                landNLegal.PlaintiffAppellant = model.PlaintiffAppellant;
                landNLegal.CaseType = model.CaseType;
                landNLegal.PreviousDate1 = model.PreviousDate1;
                landNLegal.PreviousDate2 = model.PreviousDate2;
                landNLegal.NextDate = model.NextDate;
                landNLegal.Remarks = model.Remarks;
                landNLegal.CaseSummary = model.CaseSummary;
                landNLegal.CompanyName = model.CompanyName;
                landNLegal.ResponsibleLayer = model.ResponsibleLayer;
            }
            context.Entry(landNLegal).State = landNLegal.OID == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                model.OID = landNLegal.OID;
                return result = true;
            }
            else
            {
                return result;
            }
        }

        public bool DeleteLandNLegal(long id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public List<SelectModel> GetLandNLegalEmployees()
        {
            //return context.Employees.Where(x => x.Active == true && x.DepartmentId == 6).ToList().Select(x => new SelectModel()
            string[] dealingofficer = { "KG0119", "KG0864", "KG0964", "KG3634", "KG0336", "KG0018", "KG0124", "KG1128" };
            return context.Employees.Where(x => dealingofficer.Contains(x.EmployeeId)).OrderBy(x => x.EmployeeOrder).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }

        public List<LandNLegalModel> GetLandNLegalEvent()
        {
            dynamic result = context.Database.SqlQuery<LandNLegalModel>("exec sp_GetUpcomingCaseEvent").OrderByDescending(x=>x.NextDate).ToList();
            return result;
        }

        public List<LandNLegalModel> GetCompanyBaseCaseList(int companyId)
        {
            dynamic result = context.Database.SqlQuery<LandNLegalModel>("exec lnl_GetCompanyBaseCaseList {0}", companyId).ToList();
            return result;
        }

        public List<LandNLegalModel> GetPrevious7DaysCaseSchedule()
        {
            dynamic result = context.Database.SqlQuery<LandNLegalModel>("exec sp_LnL_OneWeekPreviousCaseSchedule").ToList();
            return result;
        }

        private void AddCaseHistory(string ChangeHistory, long caseoId, string employeeId)
        {
            CaseHistory caseHistory = new CaseHistory();
            using (ERPEntities db = new ERPEntities())
            {
                caseHistory.ChangeHistory = ChangeHistory;
                caseHistory.CaseId = caseoId;
                caseHistory.CreatedBy = employeeId;
                caseHistory.CreatedDate = DateTime.Now;
                caseHistory.ModifiedBy = employeeId;
                caseHistory.ModifiedDate = DateTime.Now;
                db.CaseHistories.Add(caseHistory);
                db.SaveChanges();
            }
        }
        private void AddCaseComments(string caseCommonet, long caseoId, string employeeId)
        {
            CaseComment caseCommonets = new CaseComment();
            using (ERPEntities db = new ERPEntities())
            {
                caseCommonets.CaseComments = caseCommonet;
                caseCommonets.CaseId = caseoId;
                caseCommonets.CreatedBy = employeeId;
                caseCommonets.CreatedDate = DateTime.Now;
                caseCommonets.ModifiedBy = employeeId;
                caseCommonets.ModifiedDate = DateTime.Now;
                db.CaseComments.Add(caseCommonets);
                db.SaveChanges();
            }
        }
        //private void AddCaseAttachFile(string caseAttachFileName, long caseoId, string employeeId)
        //{
        //    CaseAttachment caseAttachment = new CaseAttachment();
        //    using (ERPEntities db = new ERPEntities())
        //    {
        //        caseAttachment.AttachFileName = caseAttachFileName;
        //        caseAttachment.CaseId = caseoId;
        //        caseAttachment.CreatedBy = employeeId;
        //        caseAttachment.CreatedDate = DateTime.Now;
        //        caseAttachment.ModifiedBy = employeeId;
        //        caseAttachment.ModifiedDate = DateTime.Now;
        //        db.CaseAttachments.Add(caseAttachment);
        //        db.SaveChanges();
        //    }
        //}

        public List<LandNLegalModel> GetKGCaseList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<LandNLegalModel>("exec sp_6GetKGCaseList {0} ", searchText).ToList();
            return result;
        }
    }
}
