using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Data.CustomModel
{
    public class ReportCustomModel
    {
        public string Title { get; set; }
        public int? PayTypeId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        [DisplayName("A/C No")]
        public int Id { get; set; }
        public long CGId { get; set; }
        public long? TeamsId { get; set; }
        public long KgMonthlySaleTargetId { get; set; }
        public long KgSaleAchivementId { get; set; }
        public long? SalesPersonId { get; set; }
        public long MoneyReceiptId { get; set; }
        public int? Head2Id { get; set; }
        public int? Head3Id { get; set; }
        public int? Head4Id { get; set; }
        public int? Head5Id { get; set; }
        public int? HeadGLId { get; set; }
        public int? LayerNo { get; set; }
        public int? EmployeeStatus { get; set; }
        public string AccCode { get; set; }
        [DisplayName("Voucher No")]
        public string VoucherNo { get; set; }
        public string ReceivedBy { get; set; }
        public int? VmVoucherTypeId { get; set; }
        public string AccName { get; set; }
        public int officeid { get; set; }
        public long Csid { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("From Date")]
        public Nullable<System.DateTime> FromDate { get; set; }
        [DisplayName("To Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ToDate { get; set; }
        public string ReportType { get; set; }
        public List<SelectModel> PayType { get; set; }
        public string SelectedLotNumber { get; set; }
        public List<SelectModelSaleTitle> SaleTitle { get; set; }
        public long SaleTitleId { get; set; }
        public string ReportName { get; set; }
        public string NoteReportName { get; set; }
        public string StockTransferDelivery { get; set; }
        public string StockTransferReceive { get; set; }
        public string StockTransferStock { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }
        public string Month { get; set; }
        [DisplayName("From")]
        public string FromMonth { get; set; }
        [DisplayName("To")]
        public string ToMonth { get; set; }
        public string Year { get; set; }
        public int RMonths { get; set; }
        public int RYears { get; set; }
        [DisplayName("Cost Center")]
        public Nullable<int> CostCenterId { get; set; }
        public Nullable<int> StockId { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public int? SubZoneId { get; set; }
        public List<SelectModel> Years { get; set; }
        public List<string> LotNumber { get; set; }
        public List<SelectModel> Employees { get; set; }
        public List<SelectModel> Vendors { get; set; }
        public List<SelectModelType> VendorsList { get; set; }
        public SelectList VoucherTypesList { get; set; } = new SelectList(new List<object>());
        public Nullable<long> ManagerId { get; set; }
        public Nullable<long> WorkLebelId { get; set; }
        public Nullable<long> WorkStatusId { get; set; }
        public string Manager { get; set; }
        public List<SelectModel> Months
        {
            get
            {
                return new List<SelectModel> {
                    new SelectModel { Text="January",Value=1},
                    new SelectModel { Text="February",Value=2},
                    new SelectModel { Text="March",Value=3},
                    new SelectModel { Text="April",Value=4},
                    new SelectModel { Text="May",Value=5},
                    new SelectModel { Text="June",Value=6},
                    new SelectModel { Text="July",Value=7},
                    new SelectModel { Text="August",Value=8},
                    new SelectModel { Text="September",Value=9},
                    new SelectModel { Text="October",Value=10},
                    new SelectModel { Text="November",Value=11},
                     new SelectModel { Text="December",Value=12},
                };
            }
            set { }
        }



        public List<SelectModel> Stocks { get; set; }
        public List<SelectModel> LandUser { get; set; }
        public List<SelectModel> LandReceiver { get; set; }
        public List<SelectModel> ProjectList2 { get; set; }
        public List<SelectModel> DisputedLisr { get; set; }
        public List<SelectModel> Companies { get; set; }
        public long? LandUserId { get; set; }    
        public long? LandReceiverId { get; set; }    
        public long? Project2Id { get; set; }    
        public long? DisputedId { get; set; }    
        public long? CompaniesId { get; set; }
        public string DagNo { get; set; }
        public int? ProjectId { get; set; }
        public int? VoucherTypeId { get; set; }
        public List<SelectModel> VoucherTypes { get; set; }

        public string Customer { get; set; }
        public string Supplier { get; set; }
        public string EmployeeKGId { get; set; }
        public int? VendorId { get; set; }
        public int Accounting_BankOrCashParantId { get; set; }
        public string AsOnDate { get; set; }
        public long EmployeeId { get; set; }
        public long? EmplId { get; set; }
        public string StrFromDate { get; set; }
        public string BookingDate { get; set; }
        public string CreateDate { get; set; }
        public string StrToDate { get; set; }
        //public string BookingDate { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedToDate { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? ProductSubCategoryId { get; set; }
        public int? ProductId { get; set; }
        public List<SelectModel> CostCenters { get; set; }

        public int? Common_UpazilasFk { get; set; }
        public int? Common_DistrictsFk { get; set; }
        public int? Common_DivisionFk { get; set; }
        public int ReligionId { get; set; }
        public int BloodGroupId { get; set; }
        public int? StartGradeId { get; set; } = 0;
        public int? EndGradeId { get; set; } = 0;
        public int OfficeTypeId { get; set; }
        public int ServiceTypeId { get; set; }
        public int VendorTypeId { get; set; }
        public int JobStatusId { get; set; }
        public int? GenderId { get; set; }
        public int MaritalTypeId { get; set; }
        public List<SelectModel> BloodGroups { get; set; }
        public List<SelectModel> Genders { get; set; }
        public List<SelectModel> MaritalTypes { get; set; }
        public List<SelectModel> Religions { get; set; }
        public List<SelectModel> JobStatus { get; set; }
        public List<SelectModel> JobTypes { get; set; }
        public List<SelectModel> OfficeTypes { get; set; }
        public SelectList DistrictList { get; set; } = new SelectList(new List<object>());
        public SelectList DivisionList { get; set; } = new SelectList(new List<object>());
        public SelectList UpazilasList { get; set; } = new SelectList(new List<object>());
        public SelectList Head3List { get; set; } = new SelectList(new List<object>());
        public List<SelectModelType> ProjectList { get; set; }
        public SelectList Head4List { get; set; } = new SelectList(new List<object>());
        public SelectList Head5List { get; set; } = new SelectList(new List<object>());
        public SelectList HeadGLList { get; set; } = new SelectList(new List<object>());
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public List<SelectModel> YearsList { get; set; }
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public List<SelectModel> ProductCategoryList { get; set; }
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public SelectList CostCenterList { get; set; } = new SelectList(new List<object>());
        public SelectList VoucherTypeList { get; set; } = new SelectList(new List<object>());
        public List<SelectWorkState> SelectWorkState { get; set; }

        public List<SelectWorkLebel> selectWorkLebel { get; set; }
        public SelectList GroupList { get; set; } = new SelectList(new List<object>());
        public SelectList SupplierList { get; set; } = new SelectList(new List<object>());
        public SelectList Stocklist { get; set; } = new SelectList(new List<object>());
        public List<SelectModelType> ZoneList { get; set; } = new List<SelectModelType> { };
        public SelectList SubZoneList { get; set; } = new SelectList(new List<object>());
        public SelectList CustomerList { get; set; } = new SelectList(new List<object>());
        public SelectList TeamLedarList { get; set; } = new SelectList(new List<object>());
        public SelectList SalesPersonList { get; set; } = new SelectList(new List<object>());
        public SelectList CostHeadsList { get; set; } = new SelectList(new List<SelectDDLModel>());
        public int? CustomerId { get; set; }
        public long? CostHeadsId { get; set; }
        public int SalaryTag { get; set; } = 0;
        public string AttendanceStatusvalue { get; set; } = "";
    
        public List<SelectModel> AttendanceStatus
        {
            get
            {
                return new List<SelectModel> {
                    new SelectModel { Text="Present",Value="OK"},
                    new SelectModel { Text="Absent",Value="Absent"},
                    new SelectModel { Text="Late In",Value="Late In"},
                    new SelectModel { Text="Early Out",Value="Early Out"},
                    new SelectModel { Text="Late In & Early Out",Value="Late In & Early Out"},
                    new SelectModel { Text="On Leave",Value="On Leave"},
                    new SelectModel { Text="Tour",Value="Tour"},
                    new SelectModel { Text="On Field",Value="On Field"},
                    new SelectModel { Text="Top Management",Value="Top Management"},
                    new SelectModel { Text="Holiday",Value="Holiday"},
                    new SelectModel { Text="Off Day",Value="Off Day"},

                };
            }
            set { }
        }
        public string SupplierName { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }

        public int? SubZoneFk { get; set; }

        public List<SelectModel> Departments { get; set; }
        public List<SelectModel> Designations { get; set; }
        public List<SelectModelForGrade> Grades { get; set; }
        public List<SelectModel> EmployeeLogTypes { get; set; }
        public int? DepartmentId { get; set; } = 0;
        public int? DesignationId { get; set; } = 0;
        public SelectList HoursList { get; set; } = new SelectList(new List<SelectMarketingModel>());
        public SelectList Minutes { get; set; } = new SelectList(new List<object>());
       
        public int FromHour { get; set; }
        public string FromMode { get; set; }
        public int FromMinut { get; set; }
        public int ToHour { get; set; }
        public string ToMode { get; set; }
        public int ToMinut { get; set; }
       

        public BusinessTypeEnum BusinessType { get; set; }
        [Display(Name = "Status")]
        public int? RealEstateStatusId { get; set; }
        public SelectList RealEstateReturn { get { return new SelectList(BaseFunctionalities.GetEnumList<RealStateReturnsStatus>(), "Value", "Text"); } }
    }
    public class ReportCustomerModel
    {
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public List<SelectModel> ZoneList { get; set; } = new List<SelectModel> { };
        public int? ZoneFk { get; set; }
        public SelectList SubZoneList { get; set; } = new SelectList(new List<object>());
        public int? SubZoneFk { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }

    }
}