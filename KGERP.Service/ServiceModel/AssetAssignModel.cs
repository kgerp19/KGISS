using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class AssetAssignModel
    {
        public int AssignId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        [DisplayName("Asset Location")]
        public int AssetLocationId { get; set; }
        [DisplayName("Asset Sub-Location")]
        public int AssetSubLocId { get; set; }
        [DisplayName("Asset Category")]
        public int AssetCategoryId { get; set; }
        [DisplayName("Product")]
        public int AssetTypeId { get; set; }
        [DisplayName("Serial No")]
        public int AssetId { get; set; }
        public string AssetSerialNo { get; set; }
        [DisplayName("Assign To")]
        public long AssignTo { get; set; }
        [DisplayName("Start Date")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("End Date")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Remarks { get; set; }

        //public virtual AssetModel Asset { get; set; }
        public virtual OfficeAssetModel AssetTrackingFinal { get; set; }
        public virtual AssetCategoryModel AssetCategory { get; set; }
        public virtual AssetLocationModel AssetLocation { get; set; }
        public virtual AssetSubLocationModel AssetSubLocation { get; set; }
        public virtual AssetTypeModel AssetType { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual EmployeeModel Employee { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string SubLocation { get; set; }
        public string Category { get; set; }
        public string Asset { get; set; }

        [DisplayName("Assigned Person")]
        public string AssignedPerson { get; set; }

        [DisplayName("Serial No")]
        public string SerialNumber { get; set; }


        //----------------Extra--------------
        public string AssignedPersonName { get; set; }
    }
}
