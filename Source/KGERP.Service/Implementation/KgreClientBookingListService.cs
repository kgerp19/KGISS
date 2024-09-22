using KGERP.Data.Models;
using KGERP.Service.Interface;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class KgreClientBookingListService : IKgreClientBookingList
    {
        public object LoadBookingListInfo()
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGRECustomers
                         where basic.StatusLevel == "Booking" && basic.CompanyId == 7
                         select new
                         {
                             basic.ClientId,
                             basic.Designation,
                             basic.FullName,
                             basic.DepartmentOrInstitution,
                             basic.PresentAddress,
                             basic.PermanentAddress,
                             basic.DateofBirth,
                             basic.NID,
                             basic.Nationality,
                             basic.CampaignName,
                             basic.MobileNo,
                             basic.MobileNo2,
                             basic.Email,
                             basic.Email1,
                             basic.Gender,
                             basic.SourceOfMedia,
                             basic.PromotionalOffer,
                             basic.Impression,
                             basic.StatusLevel,
                             basic.TypeOfInterest,
                             basic.Project,
                             basic.ChoieceOfArea,
                             basic.ReferredBy,
                             basic.ServicesDescription,
                             basic.DateOfContact,
                             basic.LastContactDate,
                             basic.NextScheduleDate,
                             basic.LastMeetingDate,
                             basic.NextFollowupdate,
                             basic.ResponsibleOfficer,
                             basic.KGRECompanyName,
                             basic.CompanyId,
                             basic.FinalStatus,
                             basic.FileNo,
                             basic.Division,
                             basic.District,
                             basic.ThanaUpazila,
                             basic.VillageMohalla,
                             basic.Remarks,
                             basic.CreatedBy,
                             basic.CreatedDate,
                             basic.ModifiedBy,
                             basic.ModifiedDate
                         }).ToList();

            return BasicInfo;

        }
    }
}
