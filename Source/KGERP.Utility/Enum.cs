using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Utility
{


    public enum StatusEnum
    {
        Inactive = 0,
        Active = 1,
        Deleted = 2
    }
    public enum LeaveStatusNewEnum
    {
        Pending ,
        Approved,
        Denied
    }

    //public enum AnnualPerformanceCategory
    //{
    //    Manager = 1,
    //    MDCO,
    //    GMD,
    //    HRAdmin
    //}
    public enum TransportTypeEnum
    {
        Courier = 1,
        Truck
    }

    public enum LeaveStatusEnum
    {
        Pending = 1,
        Approved = 2,
        Denied = 3
    }


    public enum CalculationTypeEnum
    {
        Addition = 1,
        Deduction = 2,
        NotDefined = 3,
        Modification = 4,


    }


    public enum LeaveTypeEnum
    {
        CasualLeave = 1,
        EarnLeave = 2,
        SickLeave = 4,
        MaternityLeave = 6,
        HajjLeave = 8,
        LeaveWithoutPay = 9

    }

    public enum MaritalStatusEnum
    {
        Single = 4,
        Married = 3,
        Divorced = 6,
        Widowed = 5,
        Separated = 7
    }

    public enum ApprovarTypeEnum
    {
        Manager = 1,
        HRAdmin = 2
    }
    public enum AttendanceStatusEnum
    {
        OK = 1,
        Absent = 2,
        Off_Day = 3,
        Holiday = 4,
        On_Field = 5,
        On_Leave = 6,
        Tour = 7,
        Late_In,
        Early_Out,
        Late_In_Early_Out,
    }

    public enum RealStateReturnsStatus
    {
        FileCancel = 2,
        FileTransfer = 3,
        Resale = 4,
    }
    public enum RealStateMessagesStatus
    {
        Pending = 1,
        Failed = 99,
        Success = 100,
    }
    public enum RealStateMessageStatus
    {
        FileBooking = 1,
        MoneyReceipt = 2,
    }
    public enum GeneralRequisitionStatusEnum
    {
        Draft = 0,
        Submitted = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }
    public enum ApprovalStatusEnum
    {
        Draft = 0,
        Submitted = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }
    public enum EFileSignatoryStatusEnum
    {
        Pending = 0,
        Forwarded = 1,
        Return = 2,
        Approved = 3,
        Rejected = 4
    }
    public enum GenderTypeEnum
    {
        Male = 1,
        Female = 2,
        Others = 3
    }

    public enum RecruitmentStatusEnum
    {
        Draft = 0,
        Open = 1,
        Progress = 2,
        Approved = 3,
        Rejected = 4
    }
    public enum JobTypeEnum
    {
        Full_Time = 1,
        Part_Time = 2,
        Contractual = 3,
        Freelance = 4,
        Internship = 5,
        Temporary = 6
    }
    public enum EmploymentStatusEnum
    {
        Probition = 41,
        Permanent = 40
    }
    public enum JobNatureEnum
    {
        HQ = 1,
        Field = 2,
        Factory = 3,
        HomeOffice = 4
    }
    public enum VacancyTypeEnum
    {
        New_Vacancy = 1,
        Repleacement_Vacancy = 2,
        Others
    }
    public enum SignatoryStatusEnum
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum EducationLevelEnum
    {
        None = 0,
        PrimarySchool = 1,
        JuniorSecondarySchool = 2,
        SecondarySchool = 3,
        HigherSecondarySchool = 4,
        BachelorDegree = 5,
        MasterDegree = 6,
        DoctorateDegree = 7
    }
    public enum PreservingAuditDocumentTypeEnum
    {
        Inventory = 1,
        Bill_Voucher = 2,
        Cash_Verification = 3,
        Special_Investigation = 4
    }

    public enum DayTypeEnum
    {
        Saturday = 1,
        Sunday = 2,
        Monday = 3,
        Tuesday = 4,
        Wednesday = 5,
        Thursday = 6,
        Friday = 7
    }

    public enum BankAccountTypeEnum
    {
        Current_Account = 1,
        Checking_Account = 2,
        Savings_Account = 3,
        Salary_Account = 4
    }
    public enum BusinessTypeEnum
    {
        Company = 1,
        Division = 2
    }
    public enum RequisitionAssetTypeEnum
    {
        OperationalExpances = 0,
        Asset = 1
    }

    public enum FLatCompletionStatus
    {
        OnGoing = 1,
        FUllReady = 2
    }

    public enum LCStatus
    {
        OnGoing = 1,
        Sumitted = 2
    }
    public enum IndicatorEnum
    {
        BookingMoney = 1,
        Installment,
        CostHead,
        DownPayment

    }
    public enum EnumProductBookingSteps
    {
        Entry = 1,//Dealing officer entry
        Checking = 2,// for Head of sales checking
        Approval = 3,//for companyy DMD approval
        FInalApproval = 4 //for company  MD approval
    }
    public enum EnumProductApprovalStatus
    {
        Draft = 1,
        Recheck = 2,
        Approve = 3,
        FinalApprove = 4,
        Reject = 5
    }
    public enum EnumReqStatus
    {
        Draft,
        Submitted,
        Closed
    }
    public enum EnumIssueStatus
    {
        Draft,
        Submitted,
        Closed
    }
    public enum EnumPOStatus
    {
        Draft,
        Submitted,
        Closed
    }
    public enum EnumFeedSalesStatus
    {
        Draft,
        Submit,
        GMApproval,
        MDApproval,
        AccountsApproval,
        PartialDelivered,
        Delivered,
        StoreAcceptance

    }
    public enum EnumStockTransferStatus
    {
        Draft,
        Submitted,
        Reveived,
        Closed
    }
    public enum EnumSmSStatus
    {

        Draft = 1,
        Pending = 2,
        Failed = 3,
        Cencel = 4,

        Success = 9,
        All = 99,
    }

    public enum EnumPOCompletionStatus
    {
        Incomplete = 1,
        Partially_Complete = 2,
        Complete = 3
    }


    public enum TaskType
    {
        ERP = 1,
        IT = 2,
        Admin = 3,
        Accounts = 4,
        Engineering = 5,

    }
    public enum EnumRequisitionType
    {
        PurchaseRequisition = 1,
        StoreRequisition,
        PackgingRequisition

    }
    public enum EnumDepartment
    {
        AccountsSection = 3, // Data set as on Department table
        SalesMarketingDivision,
        Purchase,
        Store,
        Production,
        FinishStore

    }


    public enum JournalEnum
    {
        BankPayment = 1,
        BankReceive,
        CashPayment,
        CashReceive,
        BillPayment,
        BillReceive,
        ContraVoucher,
        JournalVoucer,
        SalesVoucher,
        PurchaseVoucher,
        ReverseEntry
    }

    public enum GCCLJournalEnum
    {
        SalesVoucher = 9,
        PurchaseVoucher,
        ReverseEntry,
        JournalVoucher,
        ContraVoucher,
        CreditVoucher,
        DebitVoucher,
        CashVoucher,
        ProductionVoucher = 26,
        SalesReturnVoucher = 98,
        PurchaseReturnVoucher = 162

    }
    public enum SeedJournalEnum
    {

        JournalVoucher = 17,
        ContraVoucher = 18,
        CreditVoucher = 19,
        DebitVoucher = 20,
        CashVoucher = 21,
        SalesVoucher = 109,
        PurchaseVoucher = 110,
        AdjustmentEntry = 111,
        ProductionVoucher = 112,
        SalesReturnVoucher = 113,
        PurchaseReturnVoucher = 156
    }
    public enum FeedJournalEnum
    {

        JournalVoucher = 117,
        ContraVoucher = 116,
        CreditVoucher = 115,
        DebitVoucher = 114,
        SalesVoucher = 149, // 109,
        PurchaseVoucher = 110,
        RMAdjustmentEntry = 151, // 111,
        ProductionVoucher = 148,// 112,
        SalesReturnVoucher = 153, // 113,
        ProductConvertVoucher = 152,
        PurchaseReturnVoucher = 154
    }



    public enum KfmalJournalEnum
    {
        JournalVoucher = 132,
        ContraVoucher,
        CreditVoucher,
        DebitVoucher,
        CashVoucher,
        Adjustment = 168,
        SalesVoucher,
        PurchaseVoucher,
        SalesReturnVoucher,
        PurchaseReturnVoucher,
        LCAdvanceVoucher
    }

    public enum KPLJournalEnum
    {
        JournalVoucher = 42,
        ContraVoucher,
        CreditVoucher,
        DebitVoucher,
        AdjustmentCollection,
        CashVoucher = 155,
        SalesReturnVoucher = 161,
        SalesVoucher = 174
    }
    public enum PackagingJournalEnum
    {
        JournalVoucher = 27,
        ContraCashVoucher,
        CreditVoucher,
        DebitVoucher,
        LCAdvanceVoucher = 220,
        PurchaseVoucher,
        SalesVoucher,
        ProductionVoucher = 228,
        RMIssuedVoucher
    }


    public enum GLDLJournalEnum
    {
        JournalVoucher = 88,
        ContraVoucher,
        CreditVoucher,
        DebitVoucher,
        CashVoucher,
        SalesVoucher = 147,
        SalesReturnVoucher = 160
    }
    public enum ActionEnum
    {
        Add = 1,
        Edit,
        Delete,
        Detech,
        Attech,
        Approve,
        Close,
        UnApprove,
        ReOpen,
        Finalize,
        Acknowledgement
    }
    public enum Provider
    {
        Supplier = 1,
        Customer,
        RentCompany,
        CustomerAssociates

    }

    public enum PromotionTypeEnum
    {
        FreeProduct = 1,
        PromoAmount = 2
    }
    public enum CustomerType
    {
        Dealer = 1,
        Retail = 2,
        Corporate = 3

    }
    public enum KFMALCustomerType
    {
        TractorRotavator = 1,
        PowerTillerDieselEngine,
        LubricantOil,
        WaterPump,
        GovtSubsidySales,
        OthersMachineries,


    }


    public enum VendorsPaymentMethodEnum
    {
        Cash = 1,
        Credit,
        LC
    }


    public enum PaymentMethod
    {
        Cash = 1,
        Bank = 2,
        Adjustment = 3,
        Debit = 10,
    }
    public enum RealStatePaymentMethod
    {
        Cash = 1,
        Bank = 2,
        RemoteDiposit = 3,
        InternalTransfer = 4,

    }

    public enum KgRePaymentMethod
    {
        Cash = 1,
        Bank = 2,
        OnlineBEFTN = 3,
        Mobile = 4,
    }

    public enum CustomerStatusEnum
    {
        Unique = 1,
        Regular,
        Block,
        LegalAction

    }
    public enum HrAdmin
    {
        Id = 42146,

    }
    public enum TicketingStatus
    {
        ToDo = 1,
        InProgress = 2,
        Done = 3,
        Cencel = 4
    }

    public enum ProductStatusEnumGLDL
    {
        Booked = 471,
        Sold,
        Registered,
        UnSold,
        BookingCancelled = 481,
    }
    public enum ProductStatusEnumKPL
    {
        Booked = 1520,
        Sold,
        Registered,
        VacantFlat,
        BookingCancelled,
        LandOwner
    }

    public enum CompanyName
    {
        KrishibidGroup = 1,
        KrishibidFirmLimited = 4,
        KrishibidMultipurposeCo_operativeSoceityLimited = 5,
        KrishibidPoultryLimited = 6,
        GloriousLandsAndDevelopmentsLimited = 7,
        KrishibidFeedLimited = 8,
        KrishibidPropertiesLimited = 9,
        KrishibidFarmMachineryAndAutomobilesLimited = 10,
        KrishibidSaltLimited = 11,
        KrishibidStockAndSecuritiesLimited = 12,
        KrishiFoundation = 13,
        GloriousOverseasLimited = 14,
        KrishibidBazaarLimited = 16,
        KrishibidSecurityAndServicesLimited = 17,
        KrishibidToursTravelsLimited = 18,
        KrishibidPrintingAndPublicationLimited = 19,
        KrishibidPackagingLimited = 20,
        KrishibidSeedLimited = 21,
        KrishibidFoodAndBeverageLimited = 22,
        KrishibidTradingLimited = 23,
        GloriousCropCareLimited = 24,
        KrishibidFisheriesLimited = 25,
        HumanResourceManagementSystem = 26,
        System = 27,
        KGECOM = 28,
        MymensinghHatcheryAndFeedsLtd = 29,
        AssetManagementSystem = 30,
        TaskManagementSystem = 31,
        GloriousInternationalSchoolAndCollege = 32,
        LandAndLegalDivision = 227,
        KrishibidFillingStationLtd = 308,
        KrishibidMediaCorporationLimited = 309,
        KGBGlobalImpExLtd = 310,
        KGBTradingLimited = 311,
        KrishibidHospitalLtd = 312,
        SonaliOrganicDairyLimited = 650,
        OrganicPoultryLimited = 651,
        NaturalFishFarmingLimited = 652,
        KrishibidSafeFood = 648
    }
    public enum MonthList
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public enum VesselTypeEnum
    {
        Sea = 1,
        Air = 2,
        Cargo = 3
    }
    public enum LcTypeEnum
    {
        ForeignLc = 1,
        LocalLc = 2
    }

    public enum ColorEnum
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yelow = 3,
        White = 4
    }


    public enum DecisionEnum
    {
        Yes = 1,
        No = 2
    }




    public enum SalaryReportTypeEnum
    {
        BankAdviceSheet = 1,
        PaySlipReport = 2,
        SalarySheetReport = 3,
        PaymentDone = 4,
        BankAdviceAndPaymentDone = 5
    }

}
