using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace KGERP.ViewModel
{

    public class KgreCrmBulkVM
    {
        public List<KgreCrmBulk> ResponseList { get; set; } = new List<KgreCrmBulk>();
        public string  Status { get; set; }
        
        [DisplayName("Upload File")]
        public string FilePath { get; set; }

        [Required]
        public int CompanyId { get; set; }

        //public SelectList Companies
        //{
        //    get
        //    {
        //        return new SelectList {

        //            new SelectItemList { Text="GCCL",Value=7},
        //            new SelectItemList { Text="KPL",Value=9},
        //        };
        //    }
        //    set { }
        //}
        public HttpPostedFileBase ExcelFile { get; set; }
    }
    public class KgreCrmBulk
    {
        public int ClientId { get; set; }
        public string FileNo { get; set; }
        public string FullName { get; set; }
        public string BanglaName { get; set; }
        public string RegistrationName { get; set; }
        public string Spouse { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string Profession { get; set; }
        public string DepartmentOrInstitution { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string OfficeAddress { get; set; }
        public int? ReligionId { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Nationality { get; set; }
        public string NID { get; set; }
        public string Fax { get; set; }
        public string TIN { get; set; }
        public string CampaignName { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string TelephoneRes { get; set; }
        public string TelephoneOffice { get; set; }
        public string PassportNo { get; set; }
        public string Email { get; set; }
        public string Email1 { get; set; }
        public string SourceOfMedia { get; set; }
        public string PromotionalOffer { get; set; }
        public string Impression { get; set; }
        public string StatusLevel { get; set; }
        public string TypeOfInterest { get; set; }
        public string Project { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ChoieceOfArea { get; set; }
        public string ReferredBy { get; set; }
        public string ServicesDescription { get; set; }
        public DateTime DateOfContact { get; set; }
       
        public string ResponsibleOfficer { get; set; }
        public string KGRECompanyName { get; set; }
        public int? CompanyId { get; set; }
        public string FinalStatus { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string ThanaUpazila { get; set; }
        public string VillageMohalla { get; set; }
        public string Remarks { get; set; }
        public string NomineeFullName { get; set; }
        public string NomineeFathersName { get; set; }
        public string NomineeMothersName { get; set; }
        public string NomineePerAdderss { get; set; }
        public string ReletionwithApplicant { get; set; }
        public string NomineeNationlaty { get; set; }
        public string NomineeNID { get; set; }
        public string NomineeTELMobile { get; set; }
        public string NomineeEmail { get; set; }
        public string ClientImageLink { get; set; }
        public string NomineeImageLink { get; set; }
        public string NomineeNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string EntryStatus { get; set; } = "0";
        public string ExistName { get; set; }
    }
    public class KgReCrmViewModel
    {
        // internal KgReCrmModel lgReCrmModel;

        public KgReCrmModel _KgReCrmModel { get; set; }
        public KGREProjectModel kGREPlotModel { get; set; }
        public KGREPaymentModel Payment { get; set; }
        public List<SelectModel> VoucherTypes { get; set; }
        public List<SelectModel> CostCenters { get; set; }
        public List<SelectModel> Customers { get; set; }
        public List<SelectModel> PaymentModes { get; set; }
        public List<SelectModel> PaymentFors { get; set; }
        public List<SelectModel> Banks { get; set; }
        public KGREPlotBookingModel kGREPlotBookingModel { get; set; }
        public KgReCrmViewModel()
        {
            KgReCrmModel lgReCrmModel = new KgReCrmModel();
        }
        public IEnumerable<KGRECustomer> KGRECustomers { get; set; }
        public List<SelectModel> SourceOfMedias { get; set; }
        public List<SelectModel> PromotionalOffers { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }

        public List<SelectModel> PlotFlats { get; set; }
        public List<SelectModel> Impressions { get; set; }
        public List<SelectModel> StatusLevels { get; set; }
        public List<SelectModel> Genders { get; set; }
        public List<SelectModel> Religions { get; set; }
        public List<SelectModel> KGREProjects { get; set; }
        public List<SelectModel> Plots { get; set; } = new List<SelectModel>();
        public List<SelectModel> KGREChoiceAreas { get; set; }

        public List<SelectModel> SalesTypes { get; set; }
        [DisplayName("Upload File")]
        public string FilePath { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}