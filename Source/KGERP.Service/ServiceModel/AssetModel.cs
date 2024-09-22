using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class VMAsset : BaseVM
    {
        public int AssetId { get; set; }
        public int AssetLocationId { get; set; }
        public int AssetSubLocationId { get; set; }
        public int AssetCategoryId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetCode { get; set; }
        public string SerialNO { get; set; }
        public string ModelNo { get; set; }
        public Nullable<int> ColorId { get; set; }
        public Nullable<decimal> Weight { get; set; }
        [DisplayName("Status")]
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> Quantity { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        
        public string LandButton { get; set; }
       
        public Nullable<int> IsAssigned { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public string DistrictName { get; set; }
        public Nullable<int> UpazillaId { get; set; }
        public string UpazillaName { get; set; }
        public string Eunion { get; set; }
        public string Mouja { get; set; }
        public string ReceiverNameBn { get; set; }
        public string ReceiverNameEn { get; set; }
        public string DeedReceiverName { get; set; }
        public string DonerNameBn { get; set; }
        public string DonerNameEn { get; set; }
        public string SellerName { get; set; }
        public string DeedNo { get; set; }
        public Nullable<System.DateTime> DeedDate { get; set; }
        public Nullable<decimal> AmountOfLandPurchasedEn { get; set; }
        public decimal AmountOfLandDecimal { get; set; }
        public string AmountOfLandPurchasedBn { get; set; }
        public string BiaDeedNoAndDateEn { get; set; }
        public string BiaDeedNoAndDateBn { get; set; }
        public string CS { get; set; }
        public string CSDag { get; set; }
        public string SA { get; set; } 
        public string SADag { get; set; }
        public string RS { get; set; }
        public string RSDag { get; set; }
        public string BS { get; set; }
        public string BSDag { get; set; }
        public Nullable<decimal> TotalLandOfSADag { get; set; }
        public Nullable<decimal> PurchaseLandOfSADag { get; set; }
        public Nullable<decimal> RemainingLandOfSADag { get; set; }
        public Nullable<decimal> TotalLandOfRSDag { get; set; }
        public Nullable<decimal> PurchaseLandOfRSDag { get; set; }
        public Nullable<decimal> RemainingLandOfRSDag { get; set; }
        public Nullable<decimal> TotalLandOfBSDag { get; set; }
        public Nullable<decimal> PurchaseLandOfBSDag { get; set; }
        public Nullable<decimal> RemainingLandOfBSDag { get; set; }
        public string JotNot { get; set; }
        public string KhatianNo { get; set; }
        public string DagNo { get; set; }
        public Nullable<decimal> AmountOfRegisteredLand { get; set; }
        public Nullable<decimal> RemainingLand { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<decimal> PurchasedLand { get; set; }
        public Nullable<decimal> SoldLand { get; set; }
        public Nullable<decimal> PresentLand { get; set; }
        public Nullable<decimal> MortgageLand { get; set; }
        public Nullable<decimal> NonMortgageLand { get; set; }
        public string MortgageInstitution { get; set; }
        public Nullable<int> DisputedListId { get; set; }
        public string OtherDisputed { get; set; }
        public string CityJoripKhatiyan { get; set; }
        public string CityJoripDag { get; set; }
        public string UserName { get; set; }
        public Nullable<int> LandReceiverId { get; set; }
        public Nullable<int> LandUserId { get; set; }
        public virtual ICollection<AssetAssignModel> AssetAssigns { get; set; }
        public virtual ICollection<AssetFileAttach> AssetFileAttaches { get; set; }
        public virtual ICollection<FileAttachment> FileAttachments { get; set; }
        public IEnumerable<VMAsset> DataList { get; set; }
        public SelectList AssetCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList AssetLocationList { get; set; } = new SelectList(new List<object>());
        public SelectList AssetStatusList { get; set; } = new SelectList(new List<object>());
        public SelectList AssetSubLocationList { get; set; } = new SelectList(new List<object>());
        public SelectList AssetTypeList { get; set; } = new SelectList(new List<object>());
        public SelectList ColourList{ get; set; } = new SelectList(new List<object>());
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public SelectList DistrictList { get; set; } = new SelectList(new List<object>());
        public SelectList UpazilaList { get; set; } = new SelectList(new List<object>());
        public SelectList LandOwnerList { get; set; } = new SelectList(new List<object>());
        public SelectList LandUserList { get; set; } = new SelectList(new List<object>());

    }
    public class AssetModel2
    {
        public int AssetId { get; set; }
        [DisplayName("Owner Company")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [DisplayName("District")]
        public Nullable<int> DistrictId { get; set; }
        public string DistrictName { get; set; }
        [DisplayName("Upazilla")]
        public Nullable<int> UpazillaId { get; set; }
        public string UpazillaName { get; set; }
        public string MoujaName { get; set; }
        public string Mouja { get; set; }
        [DisplayName("Receiver Name")]
        public string ReceiverNameBn { get; set; }
        [DisplayName("Deed Receiver")]
        public string SellerName { get; set; }
        public string ReceiverNameEn { get; set; }
        [DisplayName("Name of the Seller")]
        public string DonerNameBn { get; set; }
        [DisplayName("Name of the Seller")]
        public string DonerNameEn { get; set; }
        [Required]
        [DisplayName("Deed No")]
        public string DeedNo { get; set; }
        public string JotNot { get; set; }
        [DisplayName("Purchased Land (Decimal)")]
        public Nullable<decimal> AmountOfLandPurchasedEn { get; set; }
        public string AmountOfLandPurchasedBn { get; set; }
        public string DeedReceiver { get; set; }
        [DisplayName("Deed Receiver")]
        public Nullable<int> LandReceiverId { get; set; }
        public int NumberofFile { get; set; }
        public IEnumerable<AssetModel2> DataList { get; set; }
        public List<SelectModelType> LandReceiverList { get; set; } = new List<SelectModelType>();
        public List<SelectModelType> DistrictList { get; set; } = new List<SelectModelType>();
        public List<SelectModelType> UpzillaList { get; set; } = new List<SelectModelType>();
        public List<SelectModelType> CompanyList { get; set; } = new List<SelectModelType>();

        public int? SelectedLandReceiverId { get; set; }
        public int? SelectedDistrictId { get; set; }
        public int? SelectedUpzillaId { get; set; }
        public int? SelectedCompanyId { get; set; }



    }
    public class AssetModel
    {
        public int AssetId { get; set; }
        public int SelectedCompanyId { get; set; }
        [DisplayName("Owner Company")]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        [DisplayName("Location")]
        public int AssetLocationId { get; set; }
        [DisplayName("Sub-Location")]
        public int AssetSubLocationId { get; set; }
        [DisplayName("Asset Category")]
        public int AssetCategoryId { get; set; }
        [DisplayName("Product")]
        public int AssetTypeId { get; set; }
        [DisplayName("Asset Code")]
        public string AssetCode { get; set; }
        [DisplayName("Serial No")]
        public string SerialNO { get; set; }
        [DisplayName("Model No")]
        public string ModelNo { get; set; }
        [DisplayName("Colour")]
        public Nullable<int> ColorId { get; set; }
        public Nullable<decimal> Weight { get; set; }
        [DisplayName("Status")]
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> Quantity { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        public string Remarks { get; set; }
        public string LandButton { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> IsAssigned { get; set; }
        [DisplayName("District")]
        public Nullable<int> DistrictId { get; set; }
        [DisplayName("Upazilla")]
        public Nullable<int> UpazillaId { get; set; }
        [DisplayName("Unions")]
        public string Eunion { get; set; }
        public string MoujaName { get; set; }
        public string Mouja { get; set; }

        [DisplayName("Receiver Name")]
        public string ReceiverNameBn { get; set; }
        [DisplayName("Deed Receiver")]
        public string SellerName { get; set; }
        public string ReceiverNameEn { get; set; }
        [DisplayName("Name of the Seller")]
        public string DonerNameBn { get; set; }
        [DisplayName("Name of the Seller")]
        public string DonerNameEn { get; set; }
        [Required]
        [DisplayName("Deed No")]
        public string DeedNo { get; set; }
        [DisplayName("Deed Date")]
        public Nullable<System.DateTime> DeedDate { get; set; }
        //[DisplayName("Amount of Land Purchased (Decimal)")]
        [DisplayName("Purchased Land (Decimal)")]
        public Nullable<decimal> AmountOfLandPurchasedEn { get; set; }
        public string AmountOfLandPurchasedBn { get; set; }
        [DisplayName("Bia Deed No and Date ")]
        public string BiaDeedNoAndDateEn { get; set; }
        public string BiaDeedNoAndDateBn { get; set; }
        [DisplayName("CS Khatian")]
        public string CS { get; set; }
        [DisplayName("CS Dag")]
        public string CSDag { get; set; }
        [DisplayName("SA Khatian")]
        public string SA { get; set; }
        [DisplayName("SA Dag")]
        public string SADag { get; set; }
        [DisplayName("RS Khatian")]
        public string RS { get; set; }
        [DisplayName("RS Dag")]
        public string RSDag { get; set; }
        [DisplayName("BS Khatian")]
        public string BS { get; set; }
        [DisplayName("BS Dag")]
        public string BSDag { get; set; }
        [DisplayName("Total Land of SA Dag(Decimal) ")]
        public Nullable<decimal> TotalLandOfSADag { get; set; }
        [DisplayName("Purchase Land of SA Dag (Decimal)")]
        public Nullable<decimal> PurchaseLandOfSADag { get; set; }
        [DisplayName("Remaining Land of SA Dag (Decimal)")]
        public Nullable<decimal> RemainingLandOfSADag { get; set; }
        [DisplayName("Total Land of RS Dag (Decimal)")]
        public Nullable<decimal> TotalLandOfRSDag { get; set; }
        [DisplayName("Purchase Land of RS Dag (Decimal)")]
        public Nullable<decimal> PurchaseLandOfRSDag { get; set; }
        [DisplayName("Remaining Land of RS Dag (Decimal)")]
        public Nullable<decimal> RemainingLandOfRSDag { get; set; }
        [DisplayName("Total Land of BS Dag (Decimal)")]
        public Nullable<decimal> TotalLandOfBSDag { get; set; }
        [DisplayName("Purchase Land of BS Dag (Decimal)")]
        public Nullable<decimal> PurchaseLandOfBSDag { get; set; }
        [DisplayName("Remaining Land of BS Dag (Decimal)")]
        public Nullable<decimal> RemainingLandOfBSDag { get; set; }
        [DisplayName("Jot No")]
        public string JotNot { get; set; }
        [DisplayName("Khatian No")]
        public string KhatianNo { get; set; }
        [DisplayName("Dag No")]
        public string DagNo { get; set; }
        [DisplayName("Amount of Muted Land (Decimal)")]
        public Nullable<decimal> AmountOfRegisteredLand { get; set; }
        [DisplayName("Remaining Land (Decimal)")]
        public Nullable<decimal> RemainingLand { get; set; }
        [DisplayName("Block/Project")]
        public Nullable<int> ProjectId { get; set; }
        [DisplayName("Parchase Land (Decimal) ")]
        public Nullable<decimal> PurchasedLand { get; set; }
        [DisplayName("Sold Land (Decimal)")]
        public Nullable<decimal> SoldLand { get; set; }
        [DisplayName("Present Land (Decimal)")]
        public Nullable<decimal> PresentLand { get; set; }
        [DisplayName("Mortgage Land (Decimal)")]
        public Nullable<decimal> MortgageLand { get; set; }
        [DisplayName("Non Mortgage Land (Decimal)")]
        public Nullable<decimal> NonMortgageLand { get; set; }
        [DisplayName("Mortgage Institution")]
        public string MortgageInstitution { get; set; }
        [DisplayName("Type of Dispute (if any)")]
        public Nullable<int> DisputedListId { get; set; }
        [DisplayName("Other reason of dispute")]
        public string OtherDisputed { get; set; }
        [DisplayName("City Khatian")]
        public string CityJoripKhatiyan { get; set; }
        [DisplayName("City Dag")]
        public string CityJoripDag { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public string DeedReceiver { get; set; }
        [DisplayName("Deed Receiver")]
        public Nullable<int> LandReceiverId { get; set; }
        [DisplayName("Land User")]
        public Nullable<int> LandUserId { get; set; }

        public virtual AssetCategoryModel AssetCategory { get; set; }
        public virtual AssetLocationModel AssetLocation { get; set; }
        public virtual AssetStatuModel AssetStatu { get; set; }
        public virtual AssetSubLocationModel AssetSubLocation { get; set; }
        public virtual AssetTypeModel AssetType { get; set; }
        public virtual ColourModel Colour { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual DistrictModel District { get; set; }
        public virtual UpazilaModel Upazila { get; set; }
        public virtual LandOwnerModel LandOwner { get; set; }
        public virtual LandUserModel LandUser { get; set; }

        public virtual ICollection<AssetAssignModel> AssetAssigns { get; set; }
        public virtual ICollection<AssetFileAttach> AssetFileAttaches { get; set; }
        public virtual ICollection<FileAttachment> FileAttachments { get; set; }

      
    }
    public class AssetModelVm
    {
        public string AssetName { get; set; }
        public string AssetLocation { get; set; }
        public string CompanyName { get; set; }
        public string SerialNo{ get; set; }
        public string StyleName { get; set; }
        public string DepartmentName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public int AssteId { get; set; }
        public int CompanyId { get; set; }

        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<AssetModelVm> DataList { get; set; }
    }
}
