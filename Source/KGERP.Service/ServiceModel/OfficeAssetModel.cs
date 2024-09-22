using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class OfficeAssetModel
    {
        public int OID { get; set; }

        [DisplayName("Product Code")]
        public string ProductCode { get; set; }
        [DisplayName("Asset Name")]
        public string AssetsName { get; set; }
        public string Manufacturer { get; set; }

        [DisplayName("Asset Description")]
        public string ProductDescriptionORProductType { get; set; }
        public string Brand { get; set; }

        [DisplayName("Model No")]
        public string ModelNo { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Product & Serial")]
        public string ProductNSerial { get; set; }

        [DisplayName("Short Name")]
        public string CompanyShortName { get; set; }

        [DisplayName("Serial Number")]
        public string ProductSerialCompany { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("Employee ID")]
        public string KGID { get; set; }
        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Status { get; set; }
        [DisplayName("Asset Location")]
        public string AssetLocation { get; set; }
        [DisplayName("Sub Location")]
        public string AssetSubLocation { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Department Name")]
        public string DepartmentName { get; set; }

        [DisplayName("Department")]
        public Nullable<int> DepartmentId { get; set; }
        public string Floor { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        [DisplayName("Total Price")]
        public Nullable<decimal> TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Location")]
        public Nullable<int> AssetLocationId { get; set; }
        [DisplayName("Sub Location")]
        public Nullable<int> AssetSubLocationId { get; set; }

        [DisplayName("Category")]
        public Nullable<int> AssetCategoryId { get; set; }

        [DisplayName("Category")]
        public string AssetCategory { get; set; }

        [DisplayName("Asset Name")]
        public Nullable<int> AssetTypeId { get; set; }
        [DisplayName("Status")]
        public Nullable<int> StatusId { get; set; }
        [DisplayName("Color")]
        public Nullable<int> ColorId { get; set; }

    }
}
