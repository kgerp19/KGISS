using AutoMapper;
using KGERP.Data.Models;
using KGERP.Service.Contracts.KGERP.Requisitions.Command.RequestResponseModel;
using KGERP.Service.Implementation.Recruitment.ViewModels;
using KGERP.Service.ServiceModel;

namespace KGERP.Models
{
    public partial class ModelMapper
    {
        public static void SetUp()
        {
            Mapper.Initialize(cfg =>
            {
               
                //***************** Entity to Model*********************
                cfg.CreateMap<Company, CompanyModel>();
                cfg.CreateMap<CompanyMenu, CompanyMenuModel>();
                cfg.CreateMap<CompanySubMenu, CompanySubMenuModel>();
                cfg.CreateMap<CompanyUserMenu, CompanyUserMenuModel>();
                cfg.CreateMap<District, DistrictModel>();
                cfg.CreateMap<Zone, ZoneModel>();
                cfg.CreateMap<LandNLegal, LandNLegalModel>();
                cfg.CreateMap<Upazila, UpazilaModel>();
                cfg.CreateMap<KttlCustomer, KttlCustomerModel>();
                cfg.CreateMap<KttlService, KttlServiceModel>();
                cfg.CreateMap<DropDownType, DropDownTypeModel>();
                cfg.CreateMap<DropDownItem, DropDownItemModel>();
                cfg.CreateMap<Department, DepartmentModel>();
                cfg.CreateMap<Designation, DesignationModel>();
                cfg.CreateMap<User, UserModel>();
                cfg.CreateMap<Employee, EmployeeModel>();
                cfg.CreateMap<LeaveApplication, LeaveApplicationModel>();
                cfg.CreateMap<LeaveCategory, LeaveCategoryModel>();
                cfg.CreateMap<AttendenceApproveApplication, AttendenceApproveApplicationModel>();
                cfg.CreateMap<Education, EducationModel>();
                cfg.CreateMap<YearlyHoliday, YearlyHolidayModel>();
                cfg.CreateMap<ProductCategory, ProductCategoryModel>();
                cfg.CreateMap<ProductSubCategory, ProductSubCategoryModel>();
                cfg.CreateMap<Product, ProductModel>();
                cfg.CreateMap<ConvertedProduct, ConvertedProductModel>();
                cfg.CreateMap<StockInfo, StockInfoModel>();
                cfg.CreateMap<VendorType, VendorTypeModel>();
                cfg.CreateMap<Vendor, VendorModel>();
                cfg.CreateMap<VendorOffer, VendorOfferModel>();
                cfg.CreateMap<Store, StoreModel>();
                cfg.CreateMap<StoreDetail, StoreDetailModel>();
                cfg.CreateMap<OrderMaster, OrderMasterModel>()
                .ForMember(d => d.OrderDetails, opt => opt.MapFrom(s => s.OrderDetails));

                cfg.CreateMap<OrderDetail, OrderDetailModel>()
                .ForMember(d => d.OrderMaster, opt => opt.MapFrom(s => s.OrderMaster))
                .ForMember(d => d.Product, opt => opt.MapFrom(s => s.Product));

                cfg.CreateMap<OrderDeliver, OrderDeliverModel>();
                cfg.CreateMap<OrderDeliverDetail, OrderDeliverDetailModel>()
                .ForMember(d => d.OrderDeliver, opt => opt.MapFrom(s => s.OrderDeliver));
                cfg.CreateMap<Payment, PaymentModel>();
                cfg.CreateMap<ProductFormula, ProductFormulaModel>();
                cfg.CreateMap<Demand, DemandModel>();
                cfg.CreateMap<DemandItem, DemandItemModel>();
                cfg.CreateMap<DemandItemDetail, DemandItemDetailModel>();
                cfg.CreateMap<UpazilaAssign, UpazilaAssignModel>();
                cfg.CreateMap<Requisition, RequisitionModel>();
                cfg.CreateMap<Voucher, VoucherModel>();
                cfg.CreateMap<PFormulaDetail, PFormulaDetailModel>();
                cfg.CreateMap<PurchaseOrder, PurchaseOrderModel>().ForMember(d => d.PurchaseOrderDetails, opt => opt.MapFrom(s => s.PurchaseOrderDetails));
                cfg.CreateMap<PurchaseOrderDetail, PurchaseOrderDetailModel>();
                cfg.CreateMap<Requisition, RequisitionModel>();
                cfg.CreateMap<StockTransfer, StockTransferModel>();
                cfg.CreateMap<Head1, Head1Model>();
                cfg.CreateMap<StockTransferDetail, StockTransferDetailModel>();
                cfg.CreateMap<SaleReturn, SaleReturnModel>()
                  .ForMember(d => d.SaleReturnDetails, opt => opt.MapFrom(s => s.SaleReturnDetails));

                cfg.CreateMap<SaleReturnDetail, SaleReturnDetailModel>();
                cfg.CreateMap<ProductPrice, ProductPriceModel>();
                cfg.CreateMap<EMI, EMIModel>();
                cfg.CreateMap<EmiDetail, EmiDetailModel>();
                cfg.CreateMap<KGRECustomer, KgReCrmModel>();
                cfg.CreateMap<AssetLocation, AssetLocationModel>();
                cfg.CreateMap<AssetSubLocation, AssetSubLocationModel>();
                cfg.CreateMap<AssetCategory, AssetCategoryModel>();
                cfg.CreateMap<Work, WorkModel>();
               
                cfg.CreateMap<WorkState, WorkStateModel>();
              

                cfg.CreateMap<Asset, AssetModel>();
                cfg.CreateMap<AssetStatu, AssetStatuModel>();
                cfg.CreateMap<Colour, ColourModel>();
                cfg.CreateMap<AssetAssign, AssetAssignModel>();
                cfg.CreateMap<AssetType, AssetTypeModel>();
                cfg.CreateMap<WorkMember, WorkMemberModel>();
                cfg.CreateMap<EmployeeOperation, EmployeeOperationModel>();
                cfg.CreateMap<StockAdjust, StockAdjustModel>().
                ForMember(d => d.StockAdjustDetails, opt => opt.MapFrom(s => s.StockAdjustDetails));

                cfg.CreateMap<Requisition, RequisitionModel>();
                cfg.CreateMap<RequisitionItem, RequisitionItemModel>();
                cfg.CreateMap<RequisitionItemDetail, RequisitionItemDetailModel>();
                cfg.CreateMap<RequisitionItemDetail, RequisitionItemDetailRM>();

                cfg.CreateMap<ProductDetail, ProductDetailModel>();

                cfg.CreateMap<OfficerAssign, OfficerAssignModel>();

                cfg.CreateMap<MaterialReceive, MaterialReceiveModel>();
                cfg.CreateMap<MaterialReceiveDetail, MaterialReceiveDetailModel>();
                cfg.CreateMap<ECMember, ECMemberModel>();
                cfg.CreateMap<BoardOfDirector, BoardOfDirectorModel>();
                cfg.CreateMap<ShareHolder, ShareHolderModel>();
                cfg.CreateMap<LandReceiver, LandReceiverModel>();
                cfg.CreateMap<LandUser, LandUserModel>();

                cfg.CreateMap<IngredientStandard, IngredientStandardModel>();
                cfg.CreateMap<IngredientStandardDetail, IngredientStandardDetailModel>();
                cfg.CreateMap<ComplainManagement, ComplainManagementModel>();
                cfg.CreateMap<ComplainType, ComplainTypeModel>();
                cfg.CreateMap<Farmer, FarmerModel>();
                cfg.CreateMap<CreditRecover, CreditRecoverModel>();
                cfg.CreateMap<CreditRecoverDetail, CreditRecoverDetailModel>();
                cfg.CreateMap<MonthlyTarget, MonthlyTargetModel>();
                cfg.CreateMap<SubZone, SubZoneModel>();
                cfg.CreateMap<ConvertedProductDetail, ConvertedProductDetailModel>();
                cfg.CreateMap<Bag, BagModel>();
                cfg.CreateMap<KGREProject, KGREProjectModel>();
                cfg.CreateMap<PfData, MemberBasePFSummaryModel>();
                cfg.CreateMap<EmployeeLoan, EmployeeLoanModel>();
                cfg.CreateMap<AssetTrackingFinal, OfficeAssetModel>();
                cfg.CreateMap<KGREPlot, KGREProjectModel>();
                cfg.CreateMap<KGREPayment, KGREPaymentModel>();
                cfg.CreateMap<KGREPlotBooking, KGREPlotBookingModel>();
                cfg.CreateMap<PurchaseReturn, PurchaseReturnModel>()
                 .ForMember(d => d.PurchaseReturnDetails, opt => opt.MapFrom(s => s.PurchaseReturnDetails));
                cfg.CreateMap<PurchaseReturnDetail, PurchaseReturnDetailModel>();
                cfg.CreateMap<RentProduction, RentProductionModel>()
               .ForMember(d => d.RentProductionDetails, opt => opt.MapFrom(s => s.RentProductionDetails));
                cfg.CreateMap<RentProductionDetail, RentProductionDetailModel>();


                //*****************Model to Entity*********************
                cfg.CreateMap<CompanyModel, Company>();
                cfg.CreateMap<CompanyMenuModel, CompanyMenu>()
                  .ForMember(d => d.Company, opt => opt.MapFrom(s => s.Company));
                cfg.CreateMap<CompanySubMenuModel, CompanySubMenu>();
                cfg.CreateMap<CompanyUserMenuModel, CompanyUserMenu>();
                cfg.CreateMap<LandNLegalModel, LandNLegal>();
                cfg.CreateMap<VoucherModel, Voucher>();
                cfg.CreateMap<DistrictModel, District>();
                cfg.CreateMap<ZoneModel, Zone>();
                cfg.CreateMap<UpazilaModel, Upazila>();
                cfg.CreateMap<KttlCustomerModel, KttlCustomer>();
                cfg.CreateMap<KttlServiceModel, KttlService>();
                cfg.CreateMap<DropDownTypeModel, DropDownType>();
                cfg.CreateMap<DropDownItemModel, DropDownItem>();
                cfg.CreateMap<DepartmentModel, Department>();
                cfg.CreateMap<DesignationModel, Designation>();
                cfg.CreateMap<EmployeeModel, Employee>();
                cfg.CreateMap<UserModel, User>();
                cfg.CreateMap<LeaveApplicationModel, LeaveApplication>();
                cfg.CreateMap<LeaveCategoryModel, LeaveCategory>();
                cfg.CreateMap<AttendenceApproveApplicationModel, AttendenceApproveApplication>();
                cfg.CreateMap<EducationModel, Education>();
                cfg.CreateMap<YearlyHolidayModel, YearlyHoliday>();
                cfg.CreateMap<ProductCategoryModel, ProductCategory>();
                cfg.CreateMap<ProductSubCategoryModel, ProductSubCategory>();
                cfg.CreateMap<ProductModel, Product>();
                cfg.CreateMap<ConvertedProductModel, ConvertedProduct>();
                cfg.CreateMap<StockInfoModel, StockInfo>();
                cfg.CreateMap<VendorTypeModel, VendorType>();
                cfg.CreateMap<VendorModel, Vendor>();
                cfg.CreateMap<VendorOfferModel, VendorOffer>();
                cfg.CreateMap<StoreModel, Store>();
                cfg.CreateMap<StoreDetailModel, StoreDetail>()
                 .ForMember(d => d.Product, opt => opt.MapFrom(s => s.Product));

                cfg.CreateMap<OrderMasterModel, OrderMaster>()
               .ForMember(d => d.OrderDetails, opt => opt.MapFrom(s => s.OrderDetails));

                cfg.CreateMap<OrderDetailModel, OrderDetail>()
                .ForMember(d => d.OrderMaster, opt => opt.MapFrom(s => s.OrderMaster))
                .ForMember(d => d.Product, opt => opt.MapFrom(s => s.Product));

                cfg.CreateMap<OrderDeliverModel, OrderDeliver>();
                cfg.CreateMap<OrderDeliverDetailModel, OrderDeliverDetail>()
                .ForMember(d => d.OrderDeliver, opt => opt.MapFrom(s => s.OrderDeliver));

                cfg.CreateMap<PaymentModel, Payment>();
                cfg.CreateMap<ProductFormulaModel, ProductFormula>();
                cfg.CreateMap<DemandModel, Demand>();
                cfg.CreateMap<DemandItemModel, DemandItem>();
                cfg.CreateMap<DemandItemDetailModel, DemandItemDetail>();
                cfg.CreateMap<UpazilaAssignModel, UpazilaAssign>();
                cfg.CreateMap<RequisitionModel, Requisition>();
                cfg.CreateMap<PFormulaDetailModel, PFormulaDetail>();
                cfg.CreateMap<PurchaseOrderModel, PurchaseOrder>()
                .ForMember(d => d.PurchaseOrderDetails, opt => opt.MapFrom(s => s.PurchaseOrderDetails));
                cfg.CreateMap<PurchaseOrderDetailModel, PurchaseOrderDetail>();
                cfg.CreateMap<RequisitionModel, Requisition>();
                cfg.CreateMap<StockTransferModel, StockTransfer>();
                cfg.CreateMap<Head1Model, Head1>();
                cfg.CreateMap<StockTransferDetailModel, StockTransferDetail>();
                cfg.CreateMap<SaleReturnModel, SaleReturn>()
                 .ForMember(d => d.SaleReturnDetails, opt => opt.MapFrom(s => s.SaleReturnDetails));

                cfg.CreateMap<SaleReturnDetailModel, SaleReturnDetail>();
                cfg.CreateMap<ProductPriceModel, ProductPrice>();
                cfg.CreateMap<EMIModel, EMI>();
                cfg.CreateMap<EmiDetailModel, EmiDetail>();
                cfg.CreateMap<KgReCrmModel, KGRECustomer>();
                cfg.CreateMap<AssetLocationModel, AssetLocation>();
                cfg.CreateMap<AssetSubLocationModel, AssetSubLocation>();
                cfg.CreateMap<AssetCategoryModel, AssetCategory>();
                cfg.CreateMap<WorkModel, Work>();
               
                cfg.CreateMap<WorkStateModel, WorkState>();
               
                cfg.CreateMap<AssetModel, Asset>();
                cfg.CreateMap<AssetStatuModel, AssetStatu>();
                cfg.CreateMap<ColourModel, Colour>();
                cfg.CreateMap<AssetAssignModel, AssetAssign>();
                cfg.CreateMap<AssetTypeModel, AssetType>();
                cfg.CreateMap<WorkMemberModel, WorkMember>();
                cfg.CreateMap<EmployeeOperationModel, EmployeeOperation>();
                cfg.CreateMap<StockAdjustModel, StockAdjust>().
                ForMember(d => d.StockAdjustDetails, opt => opt.MapFrom(s => s.StockAdjustDetails));
                cfg.CreateMap<RequisitionModel, Requisition>();
                cfg.CreateMap<RequisitionItemModel, RequisitionItem>();
                cfg.CreateMap<RequisitionItemDetailModel, RequisitionItemDetail>();
                cfg.CreateMap<ProductDetailModel, ProductDetail>();
                cfg.CreateMap<OfficerAssignModel, OfficerAssign>();
                cfg.CreateMap<MaterialReceiveModel, MaterialReceive>();
                cfg.CreateMap<MaterialReceiveDetailModel, MaterialReceiveDetail>();
                cfg.CreateMap<ECMemberModel, ECMember>();
                cfg.CreateMap<BoardOfDirectorModel, BoardOfDirector>();
                cfg.CreateMap<ShareHolderModel, ShareHolder>();
                cfg.CreateMap<LandReceiverModel, LandReceiver>();
                cfg.CreateMap<LandUserModel, LandUser>();
                cfg.CreateMap<IngredientStandardModel, IngredientStandard>();
                cfg.CreateMap<IngredientStandardDetailModel, IngredientStandardDetail>();
                cfg.CreateMap<ComplainManagementModel, ComplainManagement>();
                cfg.CreateMap<ComplainTypeModel, ComplainType>();
                cfg.CreateMap<FarmerModel, Farmer>();
                cfg.CreateMap<CreditRecoverModel, CreditRecover>();
                cfg.CreateMap<CreditRecoverDetailModel, CreditRecoverDetail>();
                cfg.CreateMap<MonthlyTargetModel, MonthlyTarget>();
                cfg.CreateMap<SubZoneModel, SubZone>();
                cfg.CreateMap<ConvertedProductDetailModel, ConvertedProductDetail>();
                cfg.CreateMap<BagModel, Bag>();
                cfg.CreateMap<KGREProjectModel, KGREProject>();
                cfg.CreateMap<MemberBasePFSummaryModel, PfData>();
                cfg.CreateMap<EmployeeLoanModel, EmployeeLoan>();
                cfg.CreateMap<OfficeAssetModel, AssetTrackingFinal>();
                cfg.CreateMap<KGREProjectModel, KGREPlot>();
                cfg.CreateMap<KGREPaymentModel, KGREPayment>();
                cfg.CreateMap<KGREPlotBookingModel, KGREPlotBooking>();
                cfg.CreateMap<PurchaseReturnModel, PurchaseReturn>()
                .ForMember(d => d.PurchaseReturnDetails, opt => opt.MapFrom(s => s.PurchaseReturnDetails));
                cfg.CreateMap<PurchaseReturnDetailModel, PurchaseReturnDetail>();

                cfg.CreateMap<RentProductionModel, RentProduction>()
               .ForMember(d => d.RentProductionDetails, opt => opt.MapFrom(s => s.RentProductionDetails));
                cfg.CreateMap<RentProductionDetailModel, RentProductionDetail>();
                cfg.CreateMap<RecruitmentRequisition, RecruitmentVM>().ReverseMap();
            });
        }
    }
}


