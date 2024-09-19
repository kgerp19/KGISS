using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class KgReCrmService : IKgReCrmService
    {
        private bool disposed = false;
        ERPEntities context = new ERPEntities();
        public async Task<KgReCrmModel> GetCommonClient(int companyId)
        {
            KgReCrmModel vmCommonSupplier = new KgReCrmModel();
            vmCommonSupplier.CompanyId = companyId;
            vmCommonSupplier.DataList = await Task.Run(() => (
            (from t in context.KGRECustomers
             join e in context.Employees on t.CreatedBy equals e.EmployeeId
             join b in context.KGREPlotBookings on t.ClientId equals b.ClientId
             where t.CompanyId == companyId
             select new KgReCrmModel
             {
                 ClientId = t.ClientId,
                 DealingOfficer = e.Name,
                 FullName = t.FullName,
                 PresentAddress = t.PresentAddress,
                 Designation = t.Designation,
                 DepartmentOrInstitution = t.DepartmentOrInstitution,
                 PermanentAddress = t.PermanentAddress,
                 DateofBirth = t.DateofBirth,
                 NID = t.NID,
                 Nationality = t.Nationality,
                 CampaignName = t.CampaignName,
                 MobileNo = t.MobileNo,
                 MobileNo2 = t.MobileNo2,
                 Email = t.Email,
                 Email1 = t.Email1,
                 Gender = t.Gender,
                 SourceOfMedia = t.SourceOfMedia,
                 PromotionalOffer = t.PromotionalOffer,
                 StatusLevel = t.StatusLevel,
                 TypeOfInterest = t.TypeOfInterest,
                 Project = t.Project,
                 ChoieceOfArea = t.ChoieceOfArea,
                 ReferredBy = t.ReferredBy,
                 ServicesDescription = t.ServicesDescription,
                 DateOfContact = t.DateOfContact,
                 LastContactDate = t.LastContactDate,
                 NextScheduleDate = t.NextScheduleDate,
                 LastMeetingDate = t.LastMeetingDate,
                 NextFollowupdate = t.NextFollowupdate,
                 ResponsibleOfficer = t.ResponsibleOfficer,
                 CompanyId = t.CompanyId.Value,
                 FileNo = t.FileNo,
                 Remarks = t.Remarks,
                 CreatedBy = t.CreatedBy,
                 CreatedDate = t.CreatedDate,
                 ModifiedBy = t.ModifiedBy,
                 ModifiedDate = t.ModifiedDate

             }).OrderByDescending(x => x.ClientId).AsEnumerable()));
            return vmCommonSupplier;
        }

        public async Task<bool> IsExistingMobileNo(string mobileNo,string mobileNo2, int companyId)
        {
            var data = new KGRECustomer();

            if (!string.IsNullOrEmpty(mobileNo) && !string.IsNullOrEmpty(mobileNo2))
            {
                data = context.KGRECustomers
                       .FirstOrDefault(x => ((
                       x.MobileNo == mobileNo.Trim()
                       || x.MobileNo2 == mobileNo.Trim()
                        ) || (x.MobileNo == mobileNo2.Trim()
                       || x.MobileNo2 == mobileNo2.Trim()
                       ))
                       && x.CompanyId == companyId);

            }
            else if (!string.IsNullOrEmpty(mobileNo) && string.IsNullOrEmpty(mobileNo2))
            {
                data = context.KGRECustomers
                       .FirstOrDefault(x => (
                       x.MobileNo == mobileNo.Trim()
                       || x.MobileNo2 == mobileNo.Trim()
                       ) && x.CompanyId == companyId);
            }
            else
            {
                data = null;
            }
            if (data == null)
                return false;

            return true;
        }

        public async Task<KgReCrmModel> CreateNewClient(KgReCrmModel model)
        {
            var objectToSave = await context.KGRECustomers
                .SingleOrDefaultAsync(s=>s.ClientId == model.ClientId
                && s.CompanyId == model.CompanyId);
            if(objectToSave == null)
            {
                objectToSave = new KGRECustomer()
                {
                    CompanyId= model.CompanyId,
                    BanglaName= model.BanglaName,
                    FullName= model.FullName,
                    Gender= model.Gender,
                    ReligionId = model.ReligionId,
                    MobileNo= model.MobileNo,
                    MobileNo2= model.MobileNo2,
                    Email= model.Email,
                    Email1= model.Email1,
                    DateofBirth=model.DateofBirth,
                    Designation= model.Designation,
                    DepartmentOrInstitution=model.DepartmentOrInstitution,
                    PresentAddress=model.PresentAddress,
                    PermanentAddress=model.PermanentAddress,
                    ResponsibleOfficer=model.ResponsibleOfficer,
                    ProjectId= model.ProjectId,
                    StatusLevel=model.StatusLevel,
                    TypeOfInterest=model.TypeOfInterest,
                    DateOfContact= model.DateOfContact,
                    NextScheduleDate= model.NextScheduleDate,
                    NextFollowupdate= model.NextFollowupdate,
                    ServicesDescription= model.ServicesDescription,
                    Remarks= model.Remarks,
                    SourceOfMedia= model.SourceOfMedia,
                    CampaignName=model.CampaignName,
                    PromotionalOffer= model.PromotionalOffer,
                    ChoieceOfArea= model.ChoieceOfArea,
                    ReferredBy= model.ReferredBy
               };   
            }
            else
            {

            }

            context.KGRECustomers.Add(objectToSave);

            await context.SaveChangesAsync();
            return model;
        }
        public List<KgReCrmModel> GetKGRELeadList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec sp_KGRE_LeadList {0} ", searchText).ToList();
            return result;

            //var firmalar = (from t in context.KGRECustomers
            //                join e in context.Employees on t.CreatedBy equals e.EmployeeId
            //                select new KgReCrmModel
            //                {
            //                    ClientId = t.ClientId,
            //                    DealingOfficer=e.Name,
            //                    FullName = t.FullName,
            //                    PresentAddress = t.PresentAddress,
            //                    Designation = t.Designation,
            //                    DepartmentOrInstitution = t.DepartmentOrInstitution,
            //                    PermanentAddress = t.PermanentAddress,
            //                    DateofBirth = t.DateofBirth,
            //                    NID = t.NID,
            //                    Nationality = t.Nationality,
            //                    CampaignName = t.CampaignName,
            //                    MobileNo = t.MobileNo,
            //                    MobileNo2 = t.MobileNo2,
            //                    Email = t.Email,
            //                    Email1 = t.Email1,
            //                    Gender = t.Gender,
            //                    SourceOfMedia = t.SourceOfMedia,
            //                    PromotionalOffer = t.PromotionalOffer,
            //                    StatusLevel = t.StatusLevel,
            //                    TypeOfInterest = t.TypeOfInterest,
            //                    Project = t.Project,
            //                    ChoieceOfArea = t.ChoieceOfArea,
            //                    ReferredBy = t.ReferredBy,
            //                    ServicesDescription = t.ServicesDescription,
            //                    DateOfContact = t.DateOfContact,
            //                    LastContactDate = t.LastContactDate,
            //                    NextScheduleDate = t.NextScheduleDate,
            //                    LastMeetingDate = t.LastMeetingDate,
            //                    NextFollowupdate = t.NextFollowupdate,
            //                    ResponsibleOfficer = t.ResponsibleOfficer,
            //                    CompanyId = t.CompanyId,
            //                    FileNo = t.FileNo,
            //                    Remarks = t.Remarks,
            //                    CreatedBy = t.CreatedBy,
            //                    CreatedDate = t.CreatedDate,
            //                    ModifiedBy = t.ModifiedBy,
            //                    ModifiedDate = t.ModifiedDate

            //                }).ToList();
            //return firmalar;

        }
        public object LoadBookingListPaymentInfo()
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGRECustomers
                         join b in db.KGREPlotBookings on basic.ClientId equals b.ClientId
                         join p in db.KGREPlots on b.PlotId equals p.PlotId
                         join d in db.DropDownItems on p.PlotStatus equals d.DropDownItemId
                         join pid in db.KGREProjects on basic.ProjectId equals pid.ProjectId
                         where p.PlotStatus == 471 && basic.CompanyId == 7
                         && b.LandValue != 420 && b.NoOfInstallment > 1
                         select new
                         {
                             p.PlotNo,
                             p.PlotSize,
                             p.PlotStatus,
                             d.Name,
                             p.PlotFace,
                             b.GrandTotal,
                             b.NoOfInstallment,
                             b.BookingDate,
                             b.FileNo,
                             b.RegistrationDate,
                             b.LandValue,
                             b.LandValueR,
                             b.Additional25Percent,
                             b.Additional25PercentR,
                             b.Additional10Percent,
                             b.Additional10PercentR,
                             b.Additional15Percent,
                             b.Additional15PercentR,
                             b.Discount,
                             b.DiscountR,
                             b.ServiceCharge4or10Per,
                             b.ServiceCharge4or10PerR,
                             b.BookingMoney,
                             b.UtilityCost,
                             b.InstallmentAmount,
                             b.GrandTotalR,
                             b.NetReceivedR,
                             b.Due,
                             pid.ProjectName,
                             basic.ClientId,
                             basic.Designation,
                             basic.FullName,
                             basic.DepartmentOrInstitution,
                             basic.PresentAddress,
                             basic.PermanentAddress,
                             basic.DateofBirth,
                             basic.NID,
                             basic.Nationality,
                             basic.MobileNo,
                             basic.MobileNo2,
                             basic.Email,
                             basic.Email1,
                             basic.Gender,
                             basic.StatusLevel,
                             basic.ReferredBy,
                             basic.ResponsibleOfficer,
                             basic.CompanyId,
                             basic.Remarks
                         }).ToList();

            return BasicInfo;
        }

        public object LoadBookingListInfo()
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGRECustomers
                         join b in db.KGREPlotBookings on basic.ClientId equals b.ClientId
                         join p in db.KGREPlots on b.PlotId equals p.PlotId
                         join d in db.DropDownItems on p.PlotStatus equals d.DropDownItemId
                         join pid in db.KGREProjects on basic.ProjectId equals pid.ProjectId
                         where p.PlotStatus == 471 || p.PlotStatus == 472 || p.PlotStatus == 473 && basic.CompanyId == 7
                         && b.LandValue != 420
                         select new
                         {
                             p.PlotNo,
                             p.PlotSize,
                             p.PlotStatus,
                             d.Name,
                             p.PlotFace,
                             b.GrandTotal,
                             b.NoOfInstallment,
                             b.BookingDate,
                             b.FileNo,
                             b.RegistrationDate,
                             b.LandValue,
                             b.LandValueR,
                             b.Additional25Percent,
                             b.Additional25PercentR,
                             b.Additional10Percent,
                             b.Additional10PercentR,
                             b.Additional15Percent,
                             b.Additional15PercentR,
                             b.Discount,
                             b.DiscountR,
                             b.ServiceCharge4or10Per,
                             b.ServiceCharge4or10PerR,
                             b.BookingMoney,
                             b.UtilityCost,
                             b.InstallmentAmount,
                             b.GrandTotalR,
                             b.NetReceivedR,
                             b.Due,
                             pid.ProjectName,
                             basic.ClientId,
                             basic.Designation,
                             basic.FullName,
                             basic.DepartmentOrInstitution,
                             basic.PresentAddress,
                             basic.PermanentAddress,
                             basic.DateofBirth,
                             basic.NID,
                             basic.Nationality,
                             basic.MobileNo,
                             basic.MobileNo2,
                             basic.Email,
                             basic.Email1,
                             basic.Gender,
                             basic.StatusLevel,
                             basic.ReferredBy,
                             basic.ResponsibleOfficer,
                             basic.CompanyId,
                             basic.Remarks
                         }).ToList();

            return BasicInfo;

        }

        public List<KgReCrmModel> GetKGRENewLeadList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec sp_KGRE_NewLeadList", searchText).ToList();
            return result;
        }

        public List<KgReCrmModel> GetKGREClient(string searchText)
        {
            IQueryable<KGRECustomer> kGRECustomers = null;
            kGRECustomers = context.KGRECustomers.Where(x => x.FullName.Contains(searchText) || x.StatusLevel.Contains(searchText)
            || x.ResponsibleOfficer.Contains(searchText) || x.PromotionalOffer.Contains(searchText) || x.Project.Contains(searchText)
            || x.TypeOfInterest.Contains(searchText) || x.Email.Contains(searchText) || x.Project.Contains(searchText) || x.MobileNo.Contains(searchText) || x.SourceOfMedia.Contains(searchText)).OrderBy(x => x.ClientId);
            return ObjectConverter<KGRECustomer, KgReCrmModel>.ConvertList(kGRECustomers.ToList()).ToList();
        }

        public KgReCrmModel GetKGRClientById(int? id)
        {
            if (id == 0)
            {
                return new KgReCrmModel() { ClientId = (int)id };
            }
            KGRECustomer KGRECustomer = context.KGRECustomers.Include(x => x.FileAttachments).Where(x => x.ClientId == id).FirstOrDefault();
            return ObjectConverter<KGRECustomer, KgReCrmModel>.Convert(KGRECustomer);
        }

        public VendorModel GetVendorById(int? id)
        {
            VendorModel vendorModel = new VendorModel();
            if (id > 0)
            {
                Vendor vendors = context.Vendors.FirstOrDefault(x => x.VendorId == id);
                vendorModel.VendorId = vendors.VendorId;
                vendorModel.Name = vendors.Name;
                vendorModel.Email = vendors.Email;
                vendorModel.Phone = vendors.Phone;
                vendorModel.Address = vendors.Address;
            }           
            return vendorModel;
        }
        public bool SaveKGREClient(int id, KgReCrmModel model)
        {
            KGRECustomer kGRECustomer = ObjectConverter<KgReCrmModel, KGRECustomer>.Convert(model);
            bool result = false;
            if (id > 0)
            {
                kGRECustomer = context.KGRECustomers.FirstOrDefault(x => x.ClientId == id);
                if (kGRECustomer != null)
                {
                    string clientHistory = string.Empty;
                    #region //NextScheduleDate
                    if (kGRECustomer.NextScheduleDate == model.NextScheduleDate)
                    {
                        kGRECustomer.NextScheduleDate = model.NextScheduleDate;
                    }
                    else
                    {
                        DateTime? date1 = null;
                        DateTime? date2 = null;
                        DateTime? date4 = null;
                        if (kGRECustomer.NextScheduleDate != null)
                        {
                            date1 = (DateTime)kGRECustomer.NextScheduleDate;
                        }

                        if (kGRECustomer.LastMeetingDate != null)
                        {
                            clientHistory = clientHistory + " * PreviousDate2 Changed from '" + kGRECustomer.LastMeetingDate + "' to '" + date2 + "'";
                            kGRECustomer.LastMeetingDate = date2;
                        }
                        else
                        {
                            if (date2 != null)
                            {
                                clientHistory = clientHistory + " * PreviousDate2 Changed to '" + date2 + "'";
                                kGRECustomer.LastMeetingDate = date2;
                            }
                        }

                        clientHistory = clientHistory + " * NextDate Changed from '" + kGRECustomer.NextScheduleDate + "' to '" + model.NextScheduleDate + "'";
                        kGRECustomer.NextScheduleDate = model.NextScheduleDate;
                    }
                    #endregion


                    #region //LastContactDate
                    if (kGRECustomer.LastContactDate == model.LastContactDate)
                    {
                        kGRECustomer.LastContactDate = model.LastContactDate;
                    }
                    else
                    {
                        DateTime? date3 = null;
                        if (kGRECustomer.LastContactDate != null)
                        {
                            date3 = (DateTime)kGRECustomer.LastContactDate;
                        }
                        if (date3 != null)
                        {
                            clientHistory = clientHistory + " * LastContactDate Changed to '" + date3 + "'";
                            kGRECustomer.LastContactDate = date3;
                        }
                    }
                    #endregion


                    #region //NextFollowupdate
                    if (kGRECustomer.NextFollowupdate == model.NextFollowupdate)
                    {
                        kGRECustomer.NextFollowupdate = model.NextFollowupdate;
                    }
                    else
                    {
                        DateTime? date3 = null;
                        if (model.NextFollowupdate != null)
                        {
                            date3 = (DateTime)model.NextFollowupdate;
                        }
                        if (date3 != null)
                        {
                            clientHistory = clientHistory + " * NextFollowupdate Changed to '" + date3 + "'";
                            kGRECustomer.NextFollowupdate = date3;
                        }
                    }
                    #endregion

                    #region//ServicesDescription
                    if (kGRECustomer.ServicesDescription == model.ServicesDescription)
                    {
                        kGRECustomer.ServicesDescription = model.ServicesDescription;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.ServicesDescription) && !string.IsNullOrEmpty(kGRECustomer.ServicesDescription))
                        {
                            clientHistory = clientHistory + " * ServicesDescription Delete from ' " + kGRECustomer.ServicesDescription + "' to '" + model.ServicesDescription + "'"; ;
                        }
                        else if (!string.IsNullOrEmpty(model.ServicesDescription) && string.IsNullOrEmpty(kGRECustomer.ServicesDescription))
                        {
                            clientHistory = clientHistory + " * ServicesDescription Added ' " + model.ServicesDescription;
                        }
                        else
                        {
                            clientHistory = clientHistory + " * ServicesDescription Changed from '" + kGRECustomer.ServicesDescription + "' to '" + model.ServicesDescription + "'";
                        }
                        kGRECustomer.ServicesDescription = model.ServicesDescription;
                    }
                    #endregion
                    #region//ResponsibleOfficer
                    if (kGRECustomer.ResponsibleOfficer == model.ResponsibleOfficer.Trim())
                    {
                        kGRECustomer.ResponsibleOfficer = model.ResponsibleOfficer.Trim();
                    }
                    else
                    {
                        clientHistory = clientHistory + " * ResponsibleOfficer Changed from '" + kGRECustomer.ResponsibleOfficer + "' to '" + model.ResponsibleOfficer.Trim() + "'";
                        kGRECustomer.ResponsibleOfficer = model.ResponsibleOfficer.Trim();
                    }
                    #endregion
                    #region //Final Status
                    if (kGRECustomer.StatusLevel == model.StatusLevel)
                    {
                        kGRECustomer.StatusLevel = model.StatusLevel;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * StatusLevel Changed from '" + kGRECustomer.StatusLevel + "' to '" + model.StatusLevel + "'";
                        kGRECustomer.StatusLevel = model.StatusLevel;
                    }
                    #endregion
                    #region //Promotional Offer
                    if (kGRECustomer.PromotionalOffer == model.PromotionalOffer)
                    {
                        kGRECustomer.PromotionalOffer = model.PromotionalOffer;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * PromotionalOffer Changed from '" + kGRECustomer.PromotionalOffer + "' to '" + model.PromotionalOffer + "'";
                        kGRECustomer.PromotionalOffer = model.PromotionalOffer;
                    }
                    #endregion
                    #region //SourceOfMedia
                    if (kGRECustomer.SourceOfMedia == model.SourceOfMedia)
                    {
                        kGRECustomer.SourceOfMedia = model.SourceOfMedia;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * SourceOfMedia Changed from '" + kGRECustomer.SourceOfMedia + "' to '" + model.SourceOfMedia + "'";
                        kGRECustomer.SourceOfMedia = model.SourceOfMedia;
                    }
                    #endregion
                    #region //Remarks
                    if (kGRECustomer.Remarks == model.Remarks)
                    {
                        kGRECustomer.Remarks = model.Remarks;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Remarks Changed from '" + kGRECustomer.Remarks + "' to '<b>" + model.Remarks + "</b>'";
                        kGRECustomer.Remarks = model.Remarks;
                    }
                    #endregion
                    #region//AddCaseHistory 
                    if (!string.IsNullOrEmpty(clientHistory))
                    {
                        AddClientHistory(clientHistory, kGRECustomer.ClientId, System.Web.HttpContext.Current.User.Identity.Name, model.CompanyId > 0 ? (int)model.CompanyId : 0);
                    }
                    #endregion
                    #region //AddCaseComments
                    if (!string.IsNullOrEmpty(model.ServicesDescription))
                    {
                        AddClientComments(model.ServicesDescription, kGRECustomer.ClientId, System.Web.HttpContext.Current.User.Identity.Name, model.CompanyId > 0 ? (int)model.CompanyId : 0);
                    }
                    #endregion
                }
                kGRECustomer.CompanyId = model.CompanyId;
                kGRECustomer.ModifiedDate = DateTime.Now;
                kGRECustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kGRECustomer.FullName = model.FullName;
                kGRECustomer.DateofBirth = model.DateofBirth;
                kGRECustomer.MobileNo = model.MobileNo;
                kGRECustomer.MobileNo2 = model.MobileNo2;
                kGRECustomer.CampaignName = model.CampaignName;
                kGRECustomer.PresentAddress = model.PresentAddress;
                kGRECustomer.PermanentAddress = model.PermanentAddress;
                kGRECustomer.Email = model.Email;
                kGRECustomer.Email1 = model.Email1;
                kGRECustomer.Nationality = model.Nationality;
                kGRECustomer.ReligionId = model.ReligionId;
                kGRECustomer.NID = model.NID;
                kGRECustomer.ChoieceOfArea = model.ChoieceOfArea;
                kGRECustomer.ReferredBy = model.ReferredBy;
                kGRECustomer.SourceOfMedia = model.SourceOfMedia;
                kGRECustomer.Gender = model.Gender;
                kGRECustomer.TypeOfInterest = model.TypeOfInterest;
                kGRECustomer.PromotionalOffer = model.PromotionalOffer;
                kGRECustomer.Project = model.ProjectName;
                kGRECustomer.ProjectId = model.ProjectId;
                kGRECustomer.Designation = model.Designation;
                kGRECustomer.DateOfContact = model.DateOfContact;
                kGRECustomer.DepartmentOrInstitution = model.DepartmentOrInstitution;
            }
            else
            {
                kGRECustomer.CompanyId = model.CompanyId;
                kGRECustomer.NID = model.NID;
                kGRECustomer.DepartmentOrInstitution = model.DepartmentOrInstitution;
                kGRECustomer.Email = model.Email;
                kGRECustomer.ResponsibleOfficer = model.ResponsibleOfficer.Trim();
                kGRECustomer.Email1 = model.Email1;
                kGRECustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kGRECustomer.CreatedDate = DateTime.Now;
                kGRECustomer.FullName = model.FullName;
                kGRECustomer.DateofBirth = model.DateofBirth;
                kGRECustomer.DateOfContact = model.DateOfContact;
                kGRECustomer.MobileNo = model.MobileNo;
                kGRECustomer.MobileNo2 = model.MobileNo2;
                kGRECustomer.CampaignName = model.CampaignName;
                kGRECustomer.PresentAddress = model.PresentAddress;
                kGRECustomer.PermanentAddress = model.PermanentAddress;
                kGRECustomer.Nationality = model.Nationality;
                if (model.StatusLevel != null)
                {
                    kGRECustomer.StatusLevel = model.StatusLevel;
                }
                else
                {
                    kGRECustomer.StatusLevel = "New";
                }
                kGRECustomer.ChoieceOfArea = model.ChoieceOfArea;
                kGRECustomer.ReferredBy = model.ReferredBy;
                kGRECustomer.SourceOfMedia = model.SourceOfMedia;
                kGRECustomer.Gender = model.Gender;
                kGRECustomer.TypeOfInterest = model.TypeOfInterest;
                kGRECustomer.PromotionalOffer = model.PromotionalOffer;
                kGRECustomer.Remarks = model.Remarks;
                kGRECustomer.ReligionId = model.ReligionId;
                kGRECustomer.ServicesDescription = model.ServicesDescription;
                kGRECustomer.Project = model.ProjectName;
                kGRECustomer.ProjectId = model.ProjectId;
                kGRECustomer.Designation = model.Designation;
                kGRECustomer.NextScheduleDate = model.NextScheduleDate;
                kGRECustomer.NextFollowupdate = model.NextFollowupdate;
                kGRECustomer.LastContactDate = model.LastContactDate;
                kGRECustomer.UploadDateTime = DateTime.Now;
            }
            context.Entry(kGRECustomer).State = kGRECustomer.ClientId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                return result = true;
            }
            else
            {
                return result;
            }
        }
        private string GetFileNoId(string fileNo, int projectId)
        {
            string kg = string.Empty;
            string kgNumber = string.Empty;
            if (projectId == 32 && fileNo.StartsWith("C"))
            {
                kg = fileNo.Substring(7, 8);
                kgNumber = fileNo.Substring(7);
            }
            if (projectId == 32 && fileNo.StartsWith("AB"))
            {
                kg = fileNo.Substring(7, 8);
                kgNumber = fileNo.Substring(7);
            }
            else
            {
                kg = fileNo.Substring(8, 8);
                kgNumber = fileNo.Substring(8);
            }
            int num = 0;
            if (fileNo != string.Empty)
            {
                num = Convert.ToInt32(kgNumber);
                ++num;
            }
            string newKgNumber = num.ToString().PadLeft(4, '0');
            return kg + newKgNumber;
        }
        public KgReCrmModel GetFileNo(int id)
        {
            if (id <= 0)
            {
                KGRECustomer lastCustomer = context.KGRECustomers.Where(x => x.ProjectId == id).OrderByDescending(x => x.ClientId).FirstOrDefault();

                if (lastCustomer == null)
                {
                    return new KgReCrmModel() { FileNo = "KGRE1" };
                }
                return new KgReCrmModel()
                {
                    FileNo = GetFileNoId(lastCustomer.FileNo, id)
                };
            }
            KGRECustomer Customer = context.KGRECustomers.Where(x => x.ProjectId == id).OrderByDescending(x => x.ClientId).FirstOrDefault();
            return ObjectConverter<KGRECustomer, KgReCrmModel>.Convert(Customer);
        }
        public bool DeleteKGREClient(int id)
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

        public async Task<List<SelectModel>> GetKGREClient(int companyId)
        {
            var data = await Task.Run(()=> (from t1 in context.TeamInfoes
                        join t2 in context.Employees on t1.EmployeeId equals t2.Id
                        where t1.IsActive
                        && t1.CompanyId == companyId
                        && t1.IsLeader == false
                        select new SelectModel()
                        {
                            Text = t2.Name + "(" + t1.Name + ")",
                            Value = t1.Id.ToString()
                        }).ToListAsync());

            return data;
            //return context.Employees.Where(x => x.FaxNo == "KGRE")
            //    .OrderBy(x => x.EmployeeOrder).ToList().Select(x => new SelectModel()
            //    {
            //        Text = x.Name.ToString(),
            //        Value = x.EmployeeId.ToString()
            //    }).ToList();
        }

        public List<SelectModel> GetKGREClient()
        {
            //string[] dealingofficer = { "KG0117", "KG0952", "KG3358", "KG0193", "KG0198", "KG0042", "KG0199", "KG0341", "KG0184", "KG0190", "KG0118", "KG0458", "KG3570" };
            //return context.Employees.Where(x => dealingofficer.Contains(x.EmployeeId)).OrderBy(x => x.EmployeeOrder).ToList().Select(x => new SelectModel()
            return context.Employees.Where(x => x.FaxNo == "KGRE")
                .OrderBy(x => x.EmployeeOrder).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name.ToString(),
                    Value = x.EmployeeId.ToString()
                }).ToList();

            //dynamic result = context.Database.SqlQuery<SelectModel>("exec kgre_GetDealingOfficer").ToList();
            //return result;
        }

        public List<SelectModel> GetProjects(int? companyId)
        {
            return context.KGREProjects.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.ProjectName,
                Value = x.ProjectId
            }).ToList();

        }
        public List<KgReCrmModel> GetKGREClientEvent()
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec KGRE_GetUpcomingClientEvent").ToList();
            return result;
        }
        public List<KgReCrmModel> GetKGREClientFollowup()
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec KGRE_GetClientFollowup").ToList();
            return result;
        }
        public List<KgReCrmModel> GetKGREClientList()
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("Select * from v_kgreClientList").ToList();
            return result;
        }

        public List<KgReCrmModel> GetPrevious7DaysClientSchedule()
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec sp_KGREE_Get1WeekPreviousClientSchedule").ToList();
            return result;
        }

        private void AddClientHistory(string ChangeHistory, long KGREId, string employeeId, int companyId)
        {
            KGREHistory kGREHistory = new KGREHistory();
            using (ERPEntities db = new ERPEntities())
            {
                kGREHistory.ChangeHistory = ChangeHistory;
                kGREHistory.KGREId = KGREId;
                kGREHistory.CreatedBy = employeeId;
                kGREHistory.CompanyId = companyId;
                kGREHistory.CreatedDate = DateTime.Now;
                kGREHistory.ModifiedBy = employeeId;
                kGREHistory.ModifiedDate = DateTime.Now;
                db.KGREHistories.Add(kGREHistory);
                db.SaveChanges();
            }
        }
        private void AddClientComments(string commonet, long KGREId, string employeeId, int companyId)
        {
            KGREComment kGREComment = new KGREComment();
            using (ERPEntities db = new ERPEntities())
            {
                kGREComment.KGREComments = commonet;
                kGREComment.KGREId = KGREId;
                kGREComment.CompanyId = companyId;
                kGREComment.CreatedBy = employeeId;
                kGREComment.CreatedDate = DateTime.Now;
                kGREComment.ModifiedBy = employeeId;
                kGREComment.ModifiedDate = DateTime.Now;
                db.KGREComments.Add(kGREComment);
                db.SaveChanges();
            }
        }

        public List<KgReCrmModel> GetKGClientList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec sp_KGREE_GetKGREClientList {0} ", searchText).ToList();
            return result;
        }

        //public List<KgReCrmModel> DailyDealingOfficerActivity(DateTime fromDate, DateTime toDate, string emoloyeeId)
        public List<KgReCrmModel> DailyDealingOfficerActivity(string fromDate, string toDate, string emoloyeeId)
        {
            if (!string.IsNullOrEmpty(emoloyeeId) && (fromDate != null) && (toDate != null))
            {
                dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec KGRE_DailyDealingOfficerActivity {0}, {1}, {2} ", fromDate, toDate, emoloyeeId).ToList();
                return result;
            }
            else
            {
                dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec KGRE_DailyDealingOfficerActivity2").ToList();
                return result;
            }
        }
        public List<KgReCrmModel> GetKGREExistingLeadList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<KgReCrmModel>("exec KGRE_ExistingLeadList {0} ", searchText).ToList();
            return result;
        }

        public bool SaveKGREClientBooking(int id, KgReCrmModel model)
        {
            KGRECustomer kGRECustomer = new KGRECustomer();
            bool result = false;
            if (id > 0)
            {
                kGRECustomer = context.KGRECustomers.FirstOrDefault(x => x.ClientId == id);
                model.SaveStatus = false;
                kGRECustomer.ModifiedDate = DateTime.Now;
                kGRECustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kGRECustomer.FileNo = model.FileNo;
                kGRECustomer.FullName = model.FullName;
                kGRECustomer.RegistrationName = model.RegistrationName;
                kGRECustomer.BanglaName = model.BanglaName;
                kGRECustomer.Spouse = model.Spouse;
                kGRECustomer.FatherName = model.FatherName;
                kGRECustomer.MotherName = model.MotherName;
                kGRECustomer.Gender = model.Gender;
                kGRECustomer.ReligionId = model.ReligionId;
                kGRECustomer.Designation = model.Designation;
                kGRECustomer.DepartmentOrInstitution = model.DepartmentOrInstitution;
                kGRECustomer.PresentAddress = model.PresentAddress;
                kGRECustomer.PermanentAddress = model.PermanentAddress;
                kGRECustomer.OfficeAddress = model.OfficeAddress;
                kGRECustomer.DateofBirth = model.DateofBirth;
                kGRECustomer.Nationality = model.Nationality;
                kGRECustomer.NID = model.NID;
                kGRECustomer.TIN = model.TIN;
                kGRECustomer.Fax = model.Fax;
                kGRECustomer.Email = model.Email;
                kGRECustomer.Email1 = model.Email1;
                kGRECustomer.TelephoneOffice = model.TelephoneOffice;
                kGRECustomer.TelephoneRes = model.TelephoneRes;
                kGRECustomer.PassportNo = model.PassportNo;
                kGRECustomer.MobileNo = model.MobileNo;
                kGRECustomer.MobileNo2 = model.MobileNo2;
                kGRECustomer.ResponsibleOfficer = model.DealingOfficer;
                kGRECustomer.CompanyId = model.CompanyId;
                kGRECustomer.ProjectId = model.ProjectId;
                kGRECustomer.NomineeFullName = model.NomineeFullName;
                kGRECustomer.NomineeFathersName = model.NomineeFathersName;
                kGRECustomer.NomineeMothersName = model.NomineeMothersName;
                kGRECustomer.NomineeNationlaty = model.NomineeNationlaty;
                kGRECustomer.NomineeNID = model.NomineeNID;
                kGRECustomer.NomineePerAdderss = model.NomineePerAdderss;
                kGRECustomer.NomineeTELMobile = model.NomineeTELMobile;
                kGRECustomer.NomineeTELMobile = model.NomineeTELMobile;
                kGRECustomer.NomineeEmail = model.NomineeEmail;
                kGRECustomer.BookingDate = model.BookingDate;
                kGRECustomer.NomineeNote = model.NomineeNote;
                kGRECustomer.ReletionwithApplicant = model.ReletionwithApplicant;
                //kGRECustomer.StatusLevel = "Sold";
                kGRECustomer.StatusLevel = model.StatusLevel;
                kGRECustomer.ResponsibleOfficer = model.ResponsibleOfficer;
            }
            else
            {
                model.SaveStatus = true;
                kGRECustomer.CompanyId = model.CompanyId;
                kGRECustomer.NID = model.NID;
                kGRECustomer.DepartmentOrInstitution = model.DepartmentOrInstitution;
                kGRECustomer.Email = model.Email;
                kGRECustomer.ResponsibleOfficer = model.ResponsibleOfficer.Trim();
                kGRECustomer.Email1 = model.Email1;
                kGRECustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kGRECustomer.CreatedDate = DateTime.Now;
                kGRECustomer.FullName = model.FullName;
                kGRECustomer.DateofBirth = model.DateofBirth;
                kGRECustomer.DateOfContact = model.DateOfContact;
                kGRECustomer.MobileNo = model.MobileNo;
                kGRECustomer.MobileNo2 = model.MobileNo2;
                kGRECustomer.CampaignName = model.CampaignName;
                kGRECustomer.PresentAddress = model.PresentAddress;
                kGRECustomer.PermanentAddress = model.PermanentAddress;
                kGRECustomer.Nationality = model.Nationality;
                if (model.StatusLevel != null)
                {
                    kGRECustomer.StatusLevel = model.StatusLevel;
                }
                else
                {
                    kGRECustomer.StatusLevel = "Booking";
                }
                kGRECustomer.ChoieceOfArea = model.ChoieceOfArea;
                kGRECustomer.ReferredBy = model.ReferredBy;
                kGRECustomer.SourceOfMedia = model.SourceOfMedia;
                kGRECustomer.Gender = model.Gender;
                kGRECustomer.ReligionId = model.ReligionId;
                kGRECustomer.TypeOfInterest = model.TypeOfInterest;
                kGRECustomer.PromotionalOffer = model.PromotionalOffer;
                kGRECustomer.Remarks = model.Remarks;
                kGRECustomer.ServicesDescription = model.ServicesDescription;
                kGRECustomer.Project = model.ProjectName;
                kGRECustomer.ProjectId = model.ProjectId;
                kGRECustomer.Designation = model.Designation;
                kGRECustomer.NextScheduleDate = model.NextScheduleDate;
                kGRECustomer.NextFollowupdate = model.NextFollowupdate;
                kGRECustomer.LastContactDate = model.LastContactDate;
            }
            context.Entry(kGRECustomer).State = kGRECustomer.ClientId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                return result = true;
            }
            else
            {
                return result;
            }
        }

        public object GetCustomerAutoComplete(string prefix, int companyId)
        {
            return context.KGRECustomers.Where(x => x.CompanyId == companyId && x.FullName.Contains(prefix)).Select(x => new
            {
                label = x.FullName,
                val = x.ClientId
            }).OrderBy(x => x.label).Take(20).ToList();
        }


        public Product Getbyproduct(int productId)
        {
            Product res = context.Products.AsNoTracking().Include(e => e.Unit).FirstOrDefault(f => f.ProductId == productId);
            res.Unit.Products = new List<Product>();
            Product product = new Product();
            product.UnitPrice = res.UnitPrice;
            product.PackSize = res.PackSize;
            product.ProductName = res.ProductName;
            product.ProductCode = res.ProductCode;
            product.Unit = res.Unit;
            product.Status = res.Status;
            product.ProductId = res.ProductId;
            product.CompanyId = res.CompanyId;
            return product;
        }

        public ProductInfoVm GetbyproductForGldl(int productId)
        {
            Product res = context.Products.AsNoTracking().Include(e => e.Unit).FirstOrDefault(f => f.ProductId == productId);
            res.Unit.Products = new List<Product>();
            ProductInfoVm product = new ProductInfoVm();
            product.UnitPrice = res.UnitPrice;
            product.PackSize = res.PackSize;
            product.ProductName = res.ProductName;
            product.ProductCode = res.ProductCode;
            product.UnitName = res.Unit.Name;
            product.ProductModel = res.Model;
            product.HorsePower = res.HorsePower;
            product.CostingPrice = res.CostingPrice;
            return product;
        }

        public object AutoCompleteByFileNo(string prefix, int companyId)
        {
            return context.KGREPlotBookings.Where(x => x.FileNo != null && x.LandValue != 420 && x.LandValue > 0 && x.FileNo.Contains(prefix)).Select(x => new
            {
                label = x.FileNo,
                val = x.ClientId
            }).OrderBy(x => x.label).Take(20).ToList();
        }
        public KGREPlotBookingModel GetClientPaymentStatus(int id)
        {
            KGREPlotBooking customer = context.KGREPlotBookings.Where(x => x.ClientId == id).FirstOrDefault();
            if (customer == null)
            {
                return new KGREPlotBookingModel();
            }
            customer.Due = context.Database.SqlQuery<double>("KGRE_GetDuePaymentByFileNo {0}", customer.FileNo).ToList().FirstOrDefault();
            //   customer.Due = context.Database.SqlQuery<double>(@"select Sum(IsNull(GrandTotal,0))-Sum(IsNull(GrandTotalR,0)) AS Due from KGREPlotBooking where ClientId={0}", customer.ClientId).ToList().FirstOrDefault();
            return ObjectConverter<KGREPlotBooking, KGREPlotBookingModel>.Convert(customer);
        }

        public VMCommonSupplier GetCustomer(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.DataList = (from t1 in context.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == (int)Provider.Customer && x.CompanyId == companyId)
                                         join t2 in context.ProductCategories on t1.ZoneId equals t2.ProductCategoryId
                                         select new VMCommonSupplier
                                         {
                                             ID = t1.VendorId,
                                             Name = t1.Name,
                                             Code = t1.Code,
                                             ZoneName = t2.Name,
                                             HeadGLId = t1.HeadGLId,

                                             Email = t1.Email,
                                             ContactPerson = t1.ContactName,
                                             Address = t1.Address,
                                             CreatedBy = t1.CreatedBy,
                                             Remarks = t1.Remarks,
                                             CompanyFK = t1.CompanyId,
                                             Phone = t1.Phone,
                                             VendorTypeId = t1.VendorTypeId
                                         }).OrderByDescending(x => x.ID).AsEnumerable();

            return vmCommonCustomer;
        }

        public List<SelectModel> GetMappingCustomer(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.KGRECustomers.Where(x => x.CompanyId == companyId && (x.HeadGLId == null || x.HeadGLId == 0)).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.FullName + " -" +x.MobileNo + " -" +x.Project,
                    Value = x.ClientId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }


        public List<object> IncomeHeadGLList(int companyId)
        {
            var List = new List<object>();
            List<int> v1 = (from t1 in context.KGRECustomers
                      join t2 in context.HeadGLs on t1.HeadGLId equals t2.Id
                      select t1.HeadGLId.Value ).ToList();

            var v = (from t1 in context.HeadGLs
                     join t2 in context.Head5 on t1.ParentId equals t2.Id
                     join t3 in context.Head4 on t2.ParentId equals t3.Id   
                    
                     where t3.AccCode == "1301003" && !v1.Contains(t1.Id)
                     && t1.CompanyId == companyId
                     select new
                     {
                         Value = t1.Id,
                         Text = t2.AccCode + " -" + t2.AccName + " " + t1.AccCode + " -" + t1.AccName
                     }).ToList();
            foreach (var item in v)
            {
                //var customer = context.KGRECustomers.FirstOrDefault(f => f.HeadGLId == item.Value);
                //if (customer == null)
                //{ }
                List.Add(new { Text = item.Text, Value = item.Value });
                              
            }
            return List;
        }

        public int CustomerHeadUpdate(VMCommonSupplier vM)
        {
            try
            {
                var customer = context.KGRECustomers.FirstOrDefault(f => f.ClientId == vM.ID);
                customer.HeadGLId = (int?)vM.HeadGLId;
                context.Entry(customer).State = EntityState.Modified;
                context.SaveChanges();
                return (int)vM.CompanyFK;
            }
            catch (Exception ex)
            {
                return (int)vM.CompanyFK;
            }



        }
    }
}
