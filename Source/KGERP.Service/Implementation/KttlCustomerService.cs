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
    public class KttlCustomerService : IKttlCustomerService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<KttlCustomerModel> GetKttlCustomers(string searchText)
        {
            IQueryable<KttlCustomer> kttlCustomers = context.KttlCustomers.Where(x => x.SurName.Contains(searchText) || x.FullName.Contains(searchText)
            || x.GivenName.Contains(searchText) || x.Services.Contains(searchText) || x.PassportNo.Contains(searchText) || x.MobileNo.Contains(searchText)
            || x.MobileNo2.Contains(searchText) || x.NationalID.Contains(searchText) || x.BirthID.Contains(searchText)
            || x.Organization.Contains(searchText) || x.ResponsiblePerson.Contains(searchText) || x.Email.Contains(searchText)).OrderBy(x => x.ClientId);

            var data = kttlCustomers.Select(w => new KttlCustomerModel()
            {
                ClientId = w.ClientId,
                NationalID = w.NationalID ?? w.BirthID,
                FullName = w.FullName,
                PassportNo = w.PassportNo,
                PresentAddress = w.PresentAddress,
                Services = w.Services,
                Organization = w.Organization,
                MobileNo = w.MobileNo ?? w.MobileNo2,
                ResponsiblePerson = w.ResponsiblePerson
            }).ToList();
            return data;
            //return ObjectConverter<KttlCustomer, KttlCustomerModel>.ConvertList(kttlCustomers.ToList()).ToList();
        }

        public KttlCustomerModel GetKttlCustomer(int id)
        {
            if (id == 0)
            {
                return new KttlCustomerModel() { ClientId = id };
            }
            KttlCustomer kttlCustomer = context.KttlCustomers.Find(id);
            return ObjectConverter<KttlCustomer, KttlCustomerModel>.Convert(kttlCustomer);
        }

        public bool DeleteKttlCustomer(int id)
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

        public List<SelectModel> GetKTTLEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.EmployeeId != "KG0372" && x.EmployeeId != "KG0027"
            && x.CompanyId == 18).OrderBy(x=>x.EmployeeOrder).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }
        public List<int> GetServeiceYear()
        {
            IList<int> years = Enumerable.Range((DateTime.Now.Year - 10), 30).ToList();
            return years.ToList();
        }

        public List<KttlCustomerModel> GetKttlCustomerSchedule()
        {
            dynamic result = context.Database.SqlQuery<KttlCustomerModel>("exec sp_KTTLCRM_GetUpcomingClientEvent").ToList();
            return result;
        }
        public List<KttlCustomerModel> Previous7DaysClientSchedule()
        {
            dynamic result = context.Database.SqlQuery<KttlCustomerModel>("exec sp_KTTLCRM_Get1WeekPreviousClientSchedule").ToList();
            return result;
        }

        public bool SaveKTTLCustomerData(int id, KttlCustomerModel model)
        {
            KttlCustomer kttlCustomer = ObjectConverter<KttlCustomerModel, KttlCustomer>.Convert(model);
            bool result = false;
            if (id > 0)
            {
                kttlCustomer = context.KttlCustomers.FirstOrDefault(x => x.ClientId == id);
                if (kttlCustomer != null)
                {
                    string clientHistory = string.Empty;

                    #region  //NextScheduleDate
                    if (kttlCustomer.NextScheduleDate == model.NextScheduleDate)
                    {
                        kttlCustomer.NextScheduleDate = model.NextScheduleDate;
                    }
                    else
                    {
                        DateTime? date1 = null;
                        DateTime? date2 = null;

                        if (kttlCustomer.NextScheduleDate != null)
                        {
                            date1 = (DateTime)kttlCustomer.NextScheduleDate;
                        }

                        if (model.LastMeetingDate1 != null)
                        {
                            date2 = (DateTime)model.LastMeetingDate1;
                        }

                        if (kttlCustomer.LastMeetingDate1 != null)
                        {
                            clientHistory = clientHistory + " * LastMeetingDate1 Changed from '" + kttlCustomer.LastMeetingDate1 + "' to '" + date1 + "'";
                            kttlCustomer.LastMeetingDate1 = date1;
                        }
                        else
                        {
                            if (date1 != null)
                            {
                                clientHistory = clientHistory + " * LastMeetingDate1 Changed to '" + date1 + "'";
                                kttlCustomer.LastMeetingDate1 = date1;
                            }
                        }
                        if (kttlCustomer.LastMeetingDate2 != null)
                        {
                            clientHistory = clientHistory + " * LastMeetingDate2 Changed from '" + kttlCustomer.LastMeetingDate2 + "' to '" + date2 + "'";
                            kttlCustomer.LastMeetingDate2 = date2;
                        }
                        else
                        {
                            if (date2 != null)
                            {
                                clientHistory = clientHistory + " * LastMeetingDate2 Changed to '" + date2 + "'";
                                kttlCustomer.LastMeetingDate2 = date2;
                            }
                        }

                        clientHistory = clientHistory + " * NextDate Changed from '" + kttlCustomer.NextScheduleDate + "' to '" + model.NextScheduleDate + "'";
                        kttlCustomer.NextScheduleDate = model.NextScheduleDate;
                    }
                    #endregion
                    #region //ServicesDescription
                    if (kttlCustomer.ServicesDescription == model.ServicesDescription)
                    {
                        kttlCustomer.ServicesDescription = model.ServicesDescription;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.ServicesDescription) && !string.IsNullOrEmpty(kttlCustomer.ServicesDescription))
                        {
                            clientHistory = clientHistory + " * ServicesDescription Delete from ' " + kttlCustomer.ServicesDescription;
                        }
                        else if (!string.IsNullOrEmpty(model.ServicesDescription) && string.IsNullOrEmpty(kttlCustomer.ServicesDescription))
                        {
                            clientHistory = clientHistory + " * ServicesDescription Added ' " + model.ServicesDescription;
                        }
                        else
                        {
                            clientHistory = clientHistory + " * ServicesDescription Changed from '" + kttlCustomer.ServicesDescription + "' to '" + model.ServicesDescription + "'";
                        }
                        kttlCustomer.ServicesDescription = model.ServicesDescription;
                    }
                    #endregion
                    #region //ResponsiblePerson
                    if (kttlCustomer.ResponsiblePerson == model.ResponsiblePerson)
                    {
                        kttlCustomer.ResponsiblePerson = model.ResponsiblePerson;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * ResponsiblePerson Changed from '" + kttlCustomer.ResponsiblePerson + "' to '" + model.ResponsiblePerson + "'";
                        kttlCustomer.ResponsiblePerson = model.ResponsiblePerson;
                    }
                    #endregion
                    #region //PassportNo
                    if (kttlCustomer.PassportNo == model.PassportNo)
                    {
                        kttlCustomer.PassportNo = model.PassportNo;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * PassportNo Changed from '" + kttlCustomer.PassportNo + "' to '<b>" + model.PassportNo + "</b>'";
                        kttlCustomer.PassportNo = model.PassportNo;
                    }
                    #endregion
                    #region //PromotionalOffer
                    if (kttlCustomer.PromotionalOffer == model.PromotionalOffer)
                    {
                        kttlCustomer.PromotionalOffer = model.PromotionalOffer;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * PromotionalOffer Changed from '" + kttlCustomer.PromotionalOffer + "' to '<b>" + model.PromotionalOffer + "</b>'";
                        kttlCustomer.PromotionalOffer = model.PromotionalOffer;
                    }
                    #endregion
                    #region //ClientStatus
                    if (kttlCustomer.ClientStatus == model.ClientStatus)
                    {
                        kttlCustomer.ClientStatus = model.ClientStatus;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * ClientStatus Changed from '" + kttlCustomer.ClientStatus + "' to '<b>" + model.ClientStatus + "</b>'";
                        kttlCustomer.ClientStatus = model.ClientStatus;
                    }
                    #endregion
                    #region //SourceOfMedia
                    if (kttlCustomer.SourceOfMedia == model.SourceOfMedia)
                    {
                        kttlCustomer.SourceOfMedia = model.SourceOfMedia;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * SourceOfMedia Changed from '" + kttlCustomer.SourceOfMedia + "' to '<b>" + model.SourceOfMedia + "</b>'";
                        kttlCustomer.SourceOfMedia = model.SourceOfMedia;
                    }
                    #endregion
                    #region //PassportNo
                    if (kttlCustomer.PassportNo == model.PassportNo)
                    {
                        kttlCustomer.PassportNo = model.PassportNo;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * PassportNo Changed from '" + kttlCustomer.PassportNo + "' to '<b>" + model.PassportNo + "</b>'";
                        kttlCustomer.PassportNo = model.PassportNo;
                    }
                    #endregion
                    #region //Email
                    if (kttlCustomer.Email == model.Email)
                    {
                        kttlCustomer.Email = model.Email;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Email Changed from '" + kttlCustomer.Email + "' to '<b>" + model.Email + "</b>'";
                        kttlCustomer.Email = model.Email;
                    }
                    #endregion
                    #region //CTitle
                    if (kttlCustomer.CTitle == model.CTitle)
                    {
                        kttlCustomer.CTitle = model.CTitle;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Client Title Changed from '" + kttlCustomer.CTitle + "' to '<b>" + model.CTitle + "</b>'";
                        kttlCustomer.CTitle = model.CTitle;
                    }
                    #endregion
                    #region //Service Year
                    if (kttlCustomer.ServiceYear == model.ServiceYear)
                    {
                        kttlCustomer.ServiceYear = model.ServiceYear;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * ServiceYear Title Changed from '" + kttlCustomer.ServiceYear + "' to '<b>" + model.ServiceYear + "</b>'";
                        kttlCustomer.ServiceYear = model.ServiceYear;
                    }
                    #endregion
                    #region //Services
                    if (kttlCustomer.Services == model.Services)
                    {
                        kttlCustomer.Services = model.Services;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Services Title Changed from '" + kttlCustomer.Services + "' to '<b>" + model.Services + "</b>'";
                        kttlCustomer.Services = model.Services;
                    }
                    #endregion
                    #region //Organization
                    if (kttlCustomer.Organization == model.Organization)
                    {
                        kttlCustomer.Organization = model.Organization;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Organization Title Changed from '" + kttlCustomer.Organization + "' to '<b>" + model.Organization + "</b>'";
                        kttlCustomer.Organization = model.Organization;
                    }
                    #endregion
                    #region //NationalID
                    if (kttlCustomer.NationalID == model.NationalID)
                    {
                        kttlCustomer.NationalID = model.NationalID;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * NationalID Title Changed from '" + kttlCustomer.NationalID + "' to '<b>" + model.NationalID + "</b>'";
                        kttlCustomer.NationalID = model.NationalID;
                    }
                    #endregion
                    #region //BirthID
                    if (kttlCustomer.BirthID == model.BirthID)
                    {
                        kttlCustomer.BirthID = model.BirthID;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * BirthID Title Changed from '" + kttlCustomer.BirthID + "' to '<b>" + model.BirthID + "</b>'";
                        kttlCustomer.BirthID = model.BirthID;
                    }
                    #endregion
                    #region //PurposeOfMeeting
                    if (kttlCustomer.PurposeOfMeeting == model.PurposeOfMeeting)
                    {
                        kttlCustomer.PurposeOfMeeting = model.PurposeOfMeeting;
                    }
                    else
                    {
                        clientHistory = clientHistory + " * Purpose Of Meeting Title Changed from '" + kttlCustomer.PurposeOfMeeting + "' to '<b>" + model.PurposeOfMeeting + "</b>'";
                        kttlCustomer.PurposeOfMeeting = model.PurposeOfMeeting;
                    }
                    #endregion
                    //AddCaseHistory 
                    if (!string.IsNullOrEmpty(clientHistory))
                    {
                        AddClientHistory(clientHistory, kttlCustomer.ClientId, System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    //AddCaseComments
                    if (!string.IsNullOrEmpty(model.Remarks))
                    {
                        AddClientComments(model.Remarks, kttlCustomer.ClientId, System.Web.HttpContext.Current.User.Identity.Name);
                    }
                }

                kttlCustomer.ModifiedDate = DateTime.Now;
                kttlCustomer.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

                //Personal Info
                // kttlCustomer.CTitle = model.CTitle;
                kttlCustomer.Gender = model.Gender;
                kttlCustomer.SurName = model.SurName;
                kttlCustomer.GivenName = model.GivenName;
                kttlCustomer.FullName = model.GivenName + " " + model.SurName;
                kttlCustomer.FatherName = model.FatherName;
                kttlCustomer.MotherName = model.MotherName;
                kttlCustomer.MaritalStatus = model.MaritalStatus;
                kttlCustomer.Spouse = model.Spouse;
                kttlCustomer.Religion = model.Religion;
                kttlCustomer.BloodGroup = model.BloodGroup;
                kttlCustomer.NationalID = model.NationalID;
                kttlCustomer.BirthID = model.BirthID;

                //Concat Address
                kttlCustomer.MobileNo = model.MobileNo;
                kttlCustomer.MobileNo2 = model.MobileNo2;
                kttlCustomer.Email = model.Email;
                kttlCustomer.Division = model.Division;
                kttlCustomer.District = model.District;
                kttlCustomer.ThanaUpazila = model.ThanaUpazila;
                kttlCustomer.VillageMohalla = model.VillageMohalla;
                //Passport
                // kttlCustomer.PassportNo = model.PassportNo;
                kttlCustomer.Nationality = model.Nationality;
                kttlCustomer.DateOfExpire = model.DateOfExpire;
                kttlCustomer.DateofBirth = model.DateOfBirth;
                kttlCustomer.DateOfIssue = model.DateOfIssue;

                kttlCustomer.OfficeAddress = model.OfficeAddress;
                kttlCustomer.BloodGroupId = model.BloodGroupId;
                kttlCustomer.SocialId = model.SocialId;
                kttlCustomer.PassportValidityId = model.PassportValidityId;
                kttlCustomer.PlacesOfBirthId = model.PlacesOfBirthId;
                kttlCustomer.PhoneHome = model.PhoneHome;
                kttlCustomer.PhoneOffice = model.PhoneOffice;
                kttlCustomer.Organization = model.Organization;

                kttlCustomer.PostOffice = model.PostOffice;
                kttlCustomer.Sector = model.Sector;
                kttlCustomer.Block = model.Block;
                kttlCustomer.PresentAddress = model.PresentAddress;
                kttlCustomer.OfficeAddress = model.OfficeAddress;

                kttlCustomer.ContactName = model.ContactName;
                kttlCustomer.ContactAddress = model.ContactAddress;
                kttlCustomer.ContactCellNo = model.ContactCellNo;
                kttlCustomer.ContactRelation = model.ContactRelation;
                kttlCustomer.ContactNotes = model.ContactNotes;
                kttlCustomer.ContactEmail = model.ContactEmail;
                kttlCustomer.TypeOfClientId = model.TypeOfClientId;
            }
            else
            {
                kttlCustomer.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kttlCustomer.CreatedDate = DateTime.Now;
                //Personal Info
                kttlCustomer.CTitle = model.CTitle;
                kttlCustomer.Gender = model.Gender;
                kttlCustomer.SurName = model.SurName;
                kttlCustomer.GivenName = model.GivenName;
                kttlCustomer.FullName = model.GivenName + " " + model.SurName;
                kttlCustomer.FatherName = model.FatherName;
                kttlCustomer.MotherName = model.MotherName;
                kttlCustomer.MaritalStatus = model.MaritalStatus;
                kttlCustomer.Spouse = model.Spouse;
                kttlCustomer.Religion = model.Religion;
                kttlCustomer.BloodGroup = model.BloodGroup;
                kttlCustomer.NationalID = model.NationalID;
                kttlCustomer.BirthID = model.BirthID;
                //Contact Address
                kttlCustomer.MobileNo = model.MobileNo;
                kttlCustomer.MobileNo2 = model.MobileNo2;
                kttlCustomer.Email = model.Email;
                kttlCustomer.Email1 = model.Email1;
                kttlCustomer.Division = model.Division;
                kttlCustomer.District = model.District;
                kttlCustomer.ThanaUpazila = model.ThanaUpazila;
                kttlCustomer.VillageMohalla = model.VillageMohalla;
                kttlCustomer.Division1 = model.Division1;
                kttlCustomer.District1 = model.District1;
                kttlCustomer.ThanaUpazila1 = model.ThanaUpazila1;
                kttlCustomer.VillageMohalla1 = model.VillageMohalla1;
                //Passport
                kttlCustomer.PassportNo = model.PassportNo;
                kttlCustomer.Nationality = model.Nationality;
                kttlCustomer.DateOfExpire = model.DateOfExpire;
                kttlCustomer.DateofBirth = model.DateOfBirth;
                kttlCustomer.DateOfIssue = model.DateOfIssue;
                //Service Info
                kttlCustomer.Services = model.Services;
                kttlCustomer.ServiceYear = model.ServiceYear;
                kttlCustomer.ResponsiblePerson = model.ResponsiblePerson;
                kttlCustomer.Organization = model.Organization;
                kttlCustomer.ClientStatus = model.ClientStatus;
                kttlCustomer.SourceOfMedia = model.SourceOfMedia;
                kttlCustomer.PromotionalOffer = model.PromotionalOffer;
                kttlCustomer.ServicesDescription = model.ServicesDescription;
                //Meeting
                kttlCustomer.NextScheduleDate = model.NextScheduleDate;
                kttlCustomer.PurposeOfMeeting = model.PurposeOfMeeting;
                kttlCustomer.Remarks = model.Remarks;

                //Change Request by KTTL Incharge Shahid vi Develop by Ashraf-20211118
                kttlCustomer.OfficeAddress = model.OfficeAddress;
                kttlCustomer.BloodGroupId = model.BloodGroupId;
                kttlCustomer.SocialId = model.SocialId;
                kttlCustomer.PassportValidityId = model.PassportValidityId;
                kttlCustomer.PlacesOfBirthId = model.PlacesOfBirthId;
                kttlCustomer.PhoneHome = model.PhoneHome;
                kttlCustomer.PhoneOffice = model.PhoneOffice;
                kttlCustomer.Organization = model.Organization;

                kttlCustomer.PostOffice = model.PostOffice;
                kttlCustomer.Sector = model.Sector;
                kttlCustomer.Block = model.Block;
                kttlCustomer.PresentAddress = model.PresentAddress;
                kttlCustomer.OfficeAddress = model.OfficeAddress;

                kttlCustomer.ContactName = model.ContactName;
                kttlCustomer.ContactAddress = model.ContactAddress;
                kttlCustomer.ContactCellNo = model.ContactCellNo;
                kttlCustomer.ContactRelation = model.ContactRelation;
                kttlCustomer.ContactNotes = model.ContactNotes;
                kttlCustomer.ContactEmail = model.ContactEmail;
                kttlCustomer.TypeOfClientId = model.TypeOfClientId;

            }
            context.Entry(kttlCustomer).State = kttlCustomer.ClientId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                return result = true;
            }
            else
            {
                return result;
            }
        }

        private void AddClientHistory(string ChangeHistory, long KTTLClientId, string employeeId)
        {
            KTTLHistory kTTLHistory = new KTTLHistory();
            using (ERPEntities db = new ERPEntities())
            {
                kTTLHistory.ChangeHistory = ChangeHistory;
                kTTLHistory.KTTLClientId = KTTLClientId;
                kTTLHistory.CreatedBy = employeeId;
                kTTLHistory.CreatedDate = DateTime.Now;
                kTTLHistory.ModifiedBy = employeeId;
                kTTLHistory.ModifiedDate = DateTime.Now;
                db.KTTLHistories.Add(kTTLHistory);
                db.SaveChanges();
            }
        }
        private void AddClientComments(string commonet, long KGREId, string employeeId)
        {
            KTTLComment kTTLComment = new KTTLComment();
            using (ERPEntities db = new ERPEntities())
            {
                kTTLComment.KTTLComments = commonet;
                kTTLComment.KTTLClientId = KGREId;
                kTTLComment.CreatedBy = employeeId;
                kTTLComment.CreatedDate = DateTime.Now;
                kTTLComment.ModifiedBy = employeeId;
                kTTLComment.ModifiedDate = DateTime.Now;
                db.KTTLComments.Add(kTTLComment);
                db.SaveChanges();
            }
        }

        #region Change Request
        public object LoadCustomerDataList()
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from c in db.KttlCustomers
                         join e in db.Employees on c.CreatedBy equals e.EmployeeId into emp
                         from employee in emp.DefaultIfEmpty()
                         join r in db.DropDownItems on c.ReligionId equals r.DropDownItemId into reli
                         from religion in reli.DefaultIfEmpty()
                         join ser in db.DropDownItems on c.ServicesId equals ser.DropDownItemId into serv
                         from service in reli.DefaultIfEmpty()

                             //join ro in db.Employees on c.ResponsiblePerson equals ro.EmployeeId
                             //join r in  db.DropDownItems on c.ReligionId equals r.DropDownItemId
                             //join cl in db.DropDownItems on c.TypeOfClientId equals cl.DropDownItemId
                             //join bl in db.DropDownItems on c.BloodGroupId equals bl.DropDownItemId
                             //join pv in db.DropDownItems on c.PassportValidityId equals pv.DropDownItemId
                             //join pf in db.DropDownItems on c.ProfessionId equals pf.DropDownItemId
                             //join ms in db.DropDownItems on c.MaritalStatusId equals ms.DropDownItemId
                             //join ser in db.DropDownItems on c.ServicesId equals ser.DropDownItemId
                             //join g in db.DropDownItems on c.GenderId equals g.DropDownItemId
                             //join t in db.DropDownItems on c.TitlesId equals t.DropDownItemId
                             //join s in db.DropDownItems on c.SourceOfMediasId equals s.DropDownItemId
                             //join cs in db.DropDownItems on c.ClientStatusId equals cs.DropDownItemId
                             //join prf in db.DropDownItems on c.ProfessionId equals prf.DropDownItemId
                             //join tf in db.DropDownItems on c.TypeOfClientId equals tf.DropDownItemId
                             //join bp in db.DropDownItems on c.PlacesOfBirthId equals bp.DropDownItemId
                             //join d in db.Divisions on c.Division equals d.DivisionId
                             //join dst in db.Districts on c.District equals dst.DistrictId
                             //join ps in db.Upazilas on c.ThanaUpazila equals ps.UpazilaId
                             //join dpl in db.Districts on c.PlacesOfBirthId equals dpl.DistrictId
                         select new
                         {
                             ClientId = c.ClientId,
                             CompanyId = c.CompanyId,
                             CTitle = c.CTitle,
                             FullName = c.FullName,
                             ResponsiblePerson = employee.Name,
                             Religion = religion.Name,
                             SurName = c.SurName,
                             GivenName = c.GivenName,
                             PresentAddress = c.PresentAddress,
                             PermanentAddress = c.PermanentAddress,
                             DateofBirth = c.DateofBirth,
                             PassportNo = c.PassportNo,
                             DateOfIssue = c.DateOfIssue,
                             DateOfExpire = c.DateOfExpire,
                             FatherName = c.FatherName,
                             MobileNo = c.MobileNo,
                             Services = service.Name,
                             //                          c.MotherName
                             //,
                             //                          c.MaritalStatus
                             //,
                             //                          c.Spouse
                             //,
                             //                          c.NoOfChild
                             //,
                             //                          c.Religion
                             //,
                             //                          r.Name
                             //,
                             //                          c.BloodGroup
                             //,
                             //                          c.Nationality
                             //,
                             //                          c.NationalID
                             //,
                             //                          c.BirthID
                             //,
                             //                          c.MobileNo
                             //,
                             //                          c.MobileNo2
                             //,
                             //                          c.Division
                             //,
                             //                          c.District
                             //,
                             //                          c.ThanaUpazila
                             //,
                             //                          c.VillageMohalla
                             //,
                             //                          c.Email1
                             //,
                             //                          c.Email
                             //,
                             //                          c.Services
                             //,
                             //                          c.Organization
                             //,
                             //                          c.ServicesDescription
                             //,
                             //                          c.ResponsiblePerson
                             //,
                             //                          c.SourceOfMedia
                             //,
                             //                          c.PromotionalOffer
                             //,
                             //                          c.ServiceYear
                             //,
                             //                          c.ClientStatus
                             //,
                             //                          c.NextScheduleDate
                             //,
                             //                          c.PurposeOfMeeting
                             //,
                             //                          c.LastMeetingDate1
                             //,
                             //                          c.LastMeetingDate2
                             //,
                             //                          c.NIDorBirthID
                             //,
                             //                          c.Telephone
                             //,
                             //                          c.Remarks
                             //,
                             //                          c.CreatedBy
                             //,
                             //                          c.CreatedDate
                             //,
                             //                          c.ModifiedBy
                             //,
                             //                          c.ModifiedDate
                             //,
                             //                          c.ContactName
                             //,
                             //                          c.ContactAddress
                             //,
                             //                          c.ContactCellNo
                             //,
                             //                          c.ContactEmail
                             //,
                             //                          c.ContactRelation
                             //,
                             //                          c.ContactNotes
                             //,
                             //                          c.TypeOfClientId
                             //,
                             //                          c.BloodGroupId
                             //,
                             //                          c.PassportValidityId
                             //,
                             //                          c.Block
                             //,
                             //                          c.Sector
                             //,
                             //                          c.PostOffice
                             //,
                             //                          c.OfficeAddress
                             //,
                             //                          c.PlacesOfBirthId
                             //,
                             //                          c.PhoneHome
                             //,
                             //                          c.PhoneOffice
                             //,
                             SocialId = c.SocialId,
                             //                          c.MaritalStatusId
                             //,
                             //                          c.ServicesId
                             //,
                             //                          c.GenderId
                             //,
                             //                          c.TitlesId
                             //,
                             //                          c.SourceOfMediasId
                             //,
                             //                          c.ClientStatusId


                         }).ToList();

            return BasicInfo;

        }

        public IQueryable<KttlCustomerModel> GetCustomers(int companyId, string searchValue, out int count)
        {
            count = context.Database.SqlQuery<int>("select count(ClientId) from KttlCustomer where CompanyId=" + companyId + "").First();
            return context.Database.SqlQuery<KttlCustomerModel>(@"exec spGetCustomerSearch {0},{1}", companyId, searchValue).AsQueryable();
        }

        #endregion

        #region For GOL
        public List<SelectModel> GetGOLEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.CompanyId == 14).OrderBy(x => x.EmployeeOrder).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }
        #endregion
    }
}
