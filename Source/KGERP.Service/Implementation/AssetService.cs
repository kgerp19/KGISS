using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{

    public class AssetService : IAssetService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public AssetService(ERPEntities context)
        {
            this.context = context;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<AssetModel> Index()
        {
            IQueryable<Asset> queryable = context.Assets.Include(x => x.Company).Include(x => x.AssetType)
                .Include(x => x.AssetLocation)
                .Include(x => x.AssetSubLocation)
                .Include(x => x.AssetCategory)
                .Where(x => x.AssetCategoryId != 3);
            return ObjectConverter<Asset, AssetModel>.ConvertList(queryable.ToList()).ToList();
        }

        public async Task<AssetModelVm> GetOfficeAssets(int? companyId)
        {
            AssetModelVm model = new AssetModelVm();

            if(companyId!=null || companyId > 0)
            {
                model.DataList = await Task.Run(() => (from t1 in context.AssetTrackingFinals
                                                       join t2 in context.Companies on t1.CompanyId equals t2.CompanyId into t2_join
                                                       from t2 in t2_join.DefaultIfEmpty()
                                                       join t3 in context.AssetTypes on t1.AssetTypeId equals t3.AssetTypeId into t3_join
                                                       from t3 in t3_join.DefaultIfEmpty()
                                                       join t4 in context.AssetSubLocations on t1.AssetLocationId equals t4.LocationId into t4_join
                                                       from t4 in t4_join.DefaultIfEmpty()
                                                       join t5 in context.AssetSubLocations on t1.AssetSubLocationId equals t5.SubLocationId into t5_join
                                                       from t5 in t5_join.DefaultIfEmpty()
                                                       join t6 in context.AssetCategories on t1.AssetCategoryId equals t6.AssetCategoryId into t6_join
                                                       from t6 in t6_join.DefaultIfEmpty()
                                                       where t1.CompanyId == companyId                                            
                                                                    select new AssetModelVm
                                                                    {
                                                                        AssteId= t1.OID,
                                                                        CompanyId = t1.CompanyId ??0,
                                                                        AssetName = t6.Name,
                                                                        AssetLocation = t4.Name,
                                                                        CompanyName = t2.Name,
                                                                        SerialNo = t1.SerialNumber,
                                                                        StyleName = t1.Style,
                                                                        DepartmentName = t1.DepartmentName,
                                                                        UnitPrice =  t1.UnitPrice ??0,
                                                                        Quantity= t1.Quantity ?? 0
                                                                    }).OrderByDescending(o => o.AssetName).AsEnumerable());
            }
            else
            {
                model.DataList = await Task.Run(() =>( from t1 in context.AssetTrackingFinals
                                                      join t2 in context.Companies on t1.CompanyId equals t2.CompanyId into t2_join
                                                      from t2 in t2_join.DefaultIfEmpty()
                                                      join t3 in context.AssetTypes on t1.AssetTypeId equals t3.AssetTypeId into t3_join
                                                      from t3 in t3_join.DefaultIfEmpty()
                                                      join t4 in context.AssetSubLocations on t1.AssetLocationId equals t4.LocationId into t4_join
                                                      from t4 in t4_join.DefaultIfEmpty()
                                                      join t5 in context.AssetSubLocations on t1.AssetSubLocationId equals t5.SubLocationId into t5_join
                                                      from t5 in t5_join.DefaultIfEmpty()
                                                      join t6 in context.AssetCategories on t1.AssetCategoryId equals t6.AssetCategoryId into t6_join
                                                      from t6 in t6_join.DefaultIfEmpty()
                                                      select new AssetModelVm
                                                       {
                                                           CompanyId = t1.CompanyId ?? 0,
                                                           AssetName = t6.Name,
                                                           AssetLocation = t4.Name,
                                                           CompanyName = t2.Name,
                                                           SerialNo = t1.SerialNumber,
                                                           StyleName = t1.Style,
                                                           DepartmentName = t1.DepartmentName,
                                                           UnitPrice = t1.UnitPrice ?? 0,
                                                           Quantity = t1.Quantity ?? 0
                                                       }).OrderByDescending(o => o.AssetName).AsEnumerable());
            }

            model.CompanyList = new SelectList(CompaniesDropDownList(), "Value", "Text");
            return model;

        }
        public List<object> CompaniesDropDownList()
        {
            var list = new List<object>();
            var v = context.Companies.ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.CompanyId });
            }
            return list;
        }
        public AssetModel GetAsset(int id)
        {
            if (id == 0)
            {
                AssetModel asset = new AssetModel();
                return asset;
            }
            else
            {
                Asset data = context.Assets
                    .Include(f=>f.FileAttachments)
                    .Where(x => x.AssetId == id).FirstOrDefault();
                return ObjectConverter<Asset, AssetModel>.Convert(data);
            }

        }


        public List<SelectModel> Company()
        {
            return context.Companies.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> AssetLocation()
        {
            return context.AssetLocations.Where(x => x.LocationId != 3).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.LocationId
            }).ToList();
        }

        public List<SelectModel> AssetSubLocation(int locationId)
        {
            return context.AssetSubLocations.Where(x => x.LocationId == locationId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.SubLocationId
            }).ToList();
        }

        public List<SelectModel> AssetSubLocationByLocationId(int? locationId)
        {
            return context.AssetSubLocations.Where(x => x.LocationId == locationId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.SubLocationId
            }).ToList();
        }
        public List<SelectModel> AssetCategory()
        {
            return context.AssetCategories.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetCategoryId
            }).ToList();
        }

        public List<SelectModel> AssetType(int categoryId)
        {
            return context.AssetTypes.Where(x => x.AssetCategoryId == categoryId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetTypeId
            }).ToList();
        }
        public List<SelectModel> AssetTypeByCategoryId(int? categoryId)
        {
            return context.AssetTypes.Where(x => x.AssetCategoryId == categoryId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.AssetTypeId
            }).ToList();
        }

        public List<SelectModel> AssetStatus()
        {
            return context.AssetStatus.ToList().Select(x => new SelectModel()
            {
                Text = x.Status,
                Value = x.StatusId
            }).ToList();
        }
        public List<SelectModel> Project()
        {
            return context.Projects.ToList().Select(x => new SelectModel()
            {
                Text = x.ProjectName,
                Value = x.ProjectId
            }).ToList();
        }

        public List<SelectModel> DisputedList()
        {
            return context.DisputedLists.ToList().Select(x => new SelectModel()
            {
                Text = x.Description,
                Value = x.DisputedListId,
            }).ToList();
        }
        public List<SelectModel> Colour()
        {
            return context.Colours.ToList().Select(x => new SelectModel()
            {
                Text = x.ColourName,
                Value = x.ColourId
            }).ToList();
        }

        public bool SaveOrEdit(AssetModel model)
        {
            if (model == null)
            {
                throw new Exception("Asset not found!");
            }

            Asset asset = ObjectConverter<AssetModel, Asset>.Convert(model);

            string comSerNo = model.CompanyId.ToString().PadLeft(2, '0');
            string subLocSerNo = context.AssetSubLocations.Where(x => x.SubLocationId == model.AssetSubLocationId).Select(x => x.SerialNo).FirstOrDefault();
            string productSerNo = context.AssetTypes.Where(x => x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNo).FirstOrDefault();
            string assetSerNo = context.Assets.Where(x => x.CompanyId == model.CompanyId && x.AssetSubLocationId == model.AssetSubLocationId && x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNO).FirstOrDefault();
            assetSerNo = assetSerNo ?? "00-00000-00000-0000";

            var sno = Convert.ToInt32(assetSerNo.Substring(15, 4));

            if (asset.AssetId > 0)
            {
                asset = context.Assets.FirstOrDefault(x => x.AssetId == asset.AssetId);
                if (asset == null)
                {
                    throw new Exception("Asset not found!");
                }
                asset.ModifiedDate = DateTime.Now;
                asset.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocationId = model.AssetSubLocationId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.StatusId = model.StatusId;
                asset.ColorId = model.ColorId;
                asset.ModelNo = model.ModelNo;
                asset.Weight = model.Weight;
                asset.Quantity = 1;

                asset.UnitPrice = model.UnitPrice;
                asset.DistrictId = model.DistrictId;
                asset.UpazillaId = model.UpazillaId;
                asset.ReceiverNameEn = model.ReceiverNameEn;
                asset.DonerNameEn = model.DonerNameEn;
                asset.DeedNo = model.DeedNo;
                asset.DeedDate = model.DeedDate;
                asset.BiaDeedNoAndDateEn = model.BiaDeedNoAndDateEn;
                asset.AmountOfLandPurchasedEn = model.AmountOfLandPurchasedEn;
                asset.CS = model.CS;
                asset.SA = model.SA;
                asset.RS = model.RS;
                asset.BS = model.BS;
                asset.CSDag = model.CSDag;
                asset.SADag = model.SADag;
                asset.RSDag = model.RSDag;
                asset.BSDag = model.BSDag;
                asset.TotalLandOfSADag = model.TotalLandOfSADag;
                asset.TotalLandOfRSDag = model.TotalLandOfRSDag;
                asset.TotalLandOfBSDag = model.TotalLandOfBSDag;
                asset.PurchaseLandOfSADag = model.PurchaseLandOfSADag;
                asset.PurchaseLandOfRSDag = model.PurchaseLandOfRSDag;
                asset.PurchaseLandOfBSDag = model.PurchaseLandOfBSDag;
                asset.RemainingLandOfSADag = model.RemainingLandOfSADag;
                asset.RemainingLandOfRSDag = model.RemainingLandOfRSDag;
                asset.RemainingLandOfBSDag = model.RemainingLandOfBSDag;
                asset.JotNot = model.JotNot;
                asset.KhatianNo = model.KhatianNo;
                asset.DagNo = model.DagNo;
                asset.RemainingLand = model.RemainingLand;
                asset.ProjectId = model.ProjectId;
                asset.UserName = model.UserName;
                asset.SoldLand = model.SoldLand;
                asset.PresentLand = model.PresentLand;
                asset.MortgageLand = model.MortgageLand;
                asset.NonMortgageLand = model.NonMortgageLand;
                asset.MortgageInstitution = model.MortgageInstitution;
                asset.DisputedListId = model.DisputedListId;
                asset.OtherDisputed = model.OtherDisputed;
                asset.CityJoripKhatiyan = model.CityJoripKhatiyan;
                asset.CityJoripDag = model.CityJoripDag;
                asset.LandReceiverId = model.LandReceiverId;
                asset.LandUserId = model.LandUserId;

                asset.Remarks = model.Remarks;
                asset.SerialNO = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                context.SaveChanges();
            }

            else
            {
                asset.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CreatedDate = DateTime.Now;
                asset.Quantity = 1;
                asset.IsAssigned = 0;
                for (int i = 0; i < model.Quantity; i++)
                {
                    asset.SerialNO = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                    context.Assets.Add(asset);
                    context.SaveChanges();
                    sno = sno + 1;
                }
            }
            return true;
        }

        public List<AssetModel> LandIndex(string searchText)
        {
            List<Asset> data = context.Assets
                .Include(x => x.Company)
                .Include(x => x.District)
                .Include(x => x.Upazila)
                .Where(x => x.AssetTypeId == 3 
                && x.AssetCategoryId == 3 
                && (x.DeedNo.ToLower().Contains(searchText) || (x.Company.Name.ToLower().Contains(searchText))))
                .ToList();
            return ObjectConverter<Asset, AssetModel>.ConvertList(data).ToList();
        }
        public async Task<VMAsset> LandList(int companyId)
        {
            VMAsset vMAsset = new VMAsset();
            vMAsset.CompanyFK = companyId;
            int assetTypeId = 3;
            vMAsset.DataList = await Task.Run(() => (from t1 in context.Assets.Where(x => x.AssetTypeId == assetTypeId
                                                     && x.AssetCategoryId == 3 
                                                    )
                                                             join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                             join t3 in context.Districts on t1.DistrictId equals t3.DistrictId
                                                             join t4 in context.Upazilas on t1.UpazillaId equals t4.UpazilaId
                                                             select new VMAsset
                                                             {
                                                                 AssetId = t1.AssetId,
                                                                 CompanyName = t2.Name,
                                                                 DistrictName = t3.Name,
                                                                 UpazillaName = t4.Name,
                                                                 DeedNo= t1.DeedNo,
                                                                 DeedReceiverName = t1.ReceiverNameEn,
                                                                 SellerName = t1.DonerNameEn,
                                                                 AmountOfLandDecimal = t1.AmountOfLandPurchasedEn ?? 0
                                                               
                                                             }).OrderByDescending(x => x.AssetId).AsEnumerable());
            return vMAsset;
        }
        public async Task<AssetModel2> GetLandList(int companyId, int? landReceiverId, int? districtId, int? upazilaId, int? selectedCompanyId)
        {
            AssetModel2 assetmodel = new AssetModel2();

            assetmodel.DataList = await Task.Run(() => (from t1 in context.Assets
                                                        join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                        join t3 in context.Districts on t1.DistrictId equals t3.DistrictId into t3_Join
                                                        from t3 in t3_Join.DefaultIfEmpty()
                                                        join t4 in context.Upazilas on t1.UpazillaId equals t4.UpazilaId into t4_Join
                                                        from t4 in t4_Join.DefaultIfEmpty()
                                                        where  
                                                        ((landReceiverId != null && landReceiverId != 0)
                                                        ? t1.LandReceiverId == landReceiverId : (t1.AssetCategoryId == 3 && t1.AssetTypeId == 3))
                                                        &&
                                                        ((districtId != null && districtId != 0) ? t1.DistrictId == districtId : (t1.AssetCategoryId == 3 && t1.AssetTypeId == 3))
                                                         &&
                                                        ((upazilaId != null && upazilaId != 0) ? t1.UpazillaId == upazilaId : (t1.AssetCategoryId == 3 && t1.AssetTypeId == 3))
                                                         &&
                                                        ((selectedCompanyId != null && selectedCompanyId != 0) ? t1.CompanyId == selectedCompanyId : (t1.AssetCategoryId == 3 && t1.AssetTypeId == 3)
                                                        ) && (t1.AssetCategoryId == 3 && t1.AssetTypeId == 3)
                                                        select new AssetModel2
                                                        {
                                                            AssetId = t1.AssetId,
                                                            CompanyId = t1.CompanyId,
                                                            CompanyName = t2.Name,
                                                            ReceiverNameEn = t1.ReceiverNameEn,
                                                            DistrictId = t3.DistrictId,
                                                            DeedNo = t1.DeedNo,
                                                            DistrictName = t3.Name,
                                                            Mouja=t1.Mouja,
                                                            UpazillaName = t4.Name,
                                                            SellerName = t1.DonerNameEn,
                                                            AmountOfLandPurchasedEn = t1.AmountOfLandPurchasedEn,
                                                            JotNot=t1.JotNot,
                                                            NumberofFile = context.FileAttachments.Where(q => q.AssetId == t1.AssetId).Count()

                                                        }).AsEnumerable());
            assetmodel.LandReceiverList=await context.LandReceivers.Select(s => new SelectModelType
            {
                Value = s.LandReceiverId,
                Text = s.LandReceiverName
            }).OrderBy(x => x.Text).ToListAsync(); 
            assetmodel.DistrictList=await context.Districts.Select(s => new SelectModelType
            {
                Value = s.DistrictId,
                Text = s.Name
            }).OrderBy(x => x.Text).ToListAsync();

            assetmodel.UpzillaList=await context.Upazilas.Select(x => new SelectModelType
            {
                Text = x.Name,
                Value = x.DistrictId
            }).OrderBy(x => x.Text).ToListAsync();
            assetmodel.CompanyList =await context.Companies.Where(q=>q.IsActive).Select(s => new SelectModelType
            {
                Value = s.CompanyId,
                Text = s.Name
            }).OrderBy(x => x.Text).ToListAsync();

            return assetmodel;


        }


        public AssetModel LandDetails(int id)
        {
            Asset data = context.Assets.Include(x => x.Company).Include(x => x.District).Include(x => x.Upazila).Where(x => x.AssetId == id).FirstOrDefault();
            return ObjectConverter<Asset, AssetModel>.Convert(data);
        }

        public List<SelectModel> LandReceiver()
        {
            return context.LandReceivers.ToList().Select(x => new SelectModel()
            {
                Text = x.LandReceiverName,
                Value = x.LandReceiverId
            }).ToList();
        }

        public List<SelectModel> LandUser()
        {
            return context.LandUsers.ToList().Select(x => new SelectModel()
            {
                Text = x.LandUserName,
                Value = x.LandUserId
            }).ToList();
        }

        public bool SaveOrEditAsset(OfficeAssetModel model)
        {
            if (model == null)
            {
                throw new Exception("Asset not found!");
            }

            AssetTrackingFinal asset = ObjectConverter<OfficeAssetModel, AssetTrackingFinal>.Convert(model);

            string comSerNo = model.CompanyId.ToString().PadLeft(2, '0');
            string subLocSerNo = context.AssetSubLocations.Where(x => x.SubLocationId == model.AssetSubLocationId).Select(x => x.SerialNo).FirstOrDefault();
            string productSerNo = context.AssetTypes.Where(x => x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNo).FirstOrDefault();
            string assetSerNo = context.AssetTrackingFinals.Where(x => x.CompanyId == model.CompanyId && x.AssetSubLocationId == model.AssetSubLocationId && x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNumber).FirstOrDefault();
            assetSerNo = assetSerNo ?? "00-00000-00000-0000";
            int sno1 = assetSerNo.Length;

            var sno = Convert.ToInt32(assetSerNo.Substring(15, 4));

            if (asset.OID > 0)
            {
                asset = context.AssetTrackingFinals.FirstOrDefault(x => x.OID == asset.OID);
                if (asset == null)
                {
                    throw new Exception("Asset not found!");
                }
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocationId = model.AssetSubLocationId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.StatusId = model.StatusId;
                asset.ColorId = model.ColorId;

                asset.ModifiedDate = DateTime.Now.ToString();
                asset.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.ProductCode = model.ProductCode;
                asset.AssetsName = model.AssetsName;
                asset.Manufacurer = model.Manufacturer;
                asset.ProductDescriptionORProductType = model.ProductDescriptionORProductType;
                asset.Brand = model.Brand;
                asset.SerialNumber = model.SerialNumber;
                asset.ProductNSerial = model.ProductNSerial;
                asset.CompanyShortName = model.CompanyShortName;

                asset.ProductSerialCompany = model.ProductSerialCompany;
                asset.UserName = model.UserName;
                asset.KGID = model.KGID;
                asset.SupplierName = model.SupplierName;
                asset.Style = model.Style;
                asset.Color = model.Color;
                asset.Size = model.Size;
                asset.Weight = model.Weight;
                asset.Status = model.Status;
                asset.AssetLocation = model.AssetLocation;
                asset.CompanyName = model.CompanyName;
                asset.DepartmentName = model.DepartmentName;
                asset.DepartmentId = model.DepartmentId;
                asset.Floor = model.Floor;
                asset.Quantity = model.Quantity;
                asset.UnitPrice = model.UnitPrice;
                asset.TotalPrice = model.TotalPrice;
                asset.Remarks = model.Remarks;
                asset.SerialNumber = GenerateSLNo(comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0'));
                //asset.SerialNumber = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                context.SaveChanges();
            }
            else
            {
                asset.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CreatedDate = DateTime.Now.ToString();
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocationId = model.AssetSubLocationId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.StatusId = model.StatusId;
                asset.ColorId = model.ColorId;

                asset.ModifiedDate = DateTime.Now.ToString();
                asset.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.ProductCode = model.ProductCode;
                asset.AssetsName = model.AssetsName;
                asset.Manufacurer = model.Manufacturer;
                asset.ProductDescriptionORProductType = model.ProductDescriptionORProductType;
                asset.Brand = model.Brand;
                asset.SerialNumber = model.SerialNumber;
                asset.ProductNSerial = model.ProductNSerial;
                asset.CompanyShortName = model.CompanyShortName;

                asset.ProductSerialCompany = model.ProductSerialCompany;
                asset.UserName = model.UserName;
                asset.KGID = model.KGID;
                asset.SupplierName = model.SupplierName;
                asset.Style = model.Style;
                asset.Color = model.Color;
                asset.Size = model.Size;
                asset.Weight = model.Weight;
                asset.Status = model.Status;
                asset.AssetLocation = model.AssetLocation;
                asset.CompanyName = model.CompanyName;
                asset.DepartmentName = model.DepartmentName;
                asset.DepartmentId = model.DepartmentId;
                asset.Floor = model.Floor;
                asset.Quantity = model.Quantity;
                asset.UnitPrice = model.UnitPrice;
                asset.TotalPrice = model.TotalPrice;
                asset.Remarks = model.Remarks;
                asset.SerialNumber = GenerateSLNo(comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0'));

                //for (int i = 0; i < model.Quantity; i++)
                //{
                asset.SerialNumber = GenerateSLNo(comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0'));
                //asset.SerialNumber = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                context.AssetTrackingFinals.Add(asset);
                context.SaveChanges();
                //}
            }
            return true;
        }

        private string GenerateSLNo(string sno)
        {
            string kg = string.Empty;
            AssetTrackingFinal asset = context.AssetTrackingFinals.Where(x => x.SerialNumber == sno).FirstOrDefault();

            if (asset == null)
            {
                return sno;
            }
            else
            {
                AssetTrackingFinal asset2 = context.AssetTrackingFinals.Where(x => x.SerialNumber == sno).First();
                kg = asset2.SerialNumber.Substring(0, 15);
                string sNumber = asset2.SerialNumber.Substring(15);
                int num = 0;
                if (asset2.SerialNumber != string.Empty)
                {
                    num = Convert.ToInt32(sNumber);
                    ++num;
                }
                string newsNumber = num.ToString().PadLeft(4, '0');
                return kg + newsNumber;
            }
        }

        public OfficeAssetModel GetFinalAsset(int id)
        {
            if (id == 0)
            {
                OfficeAssetModel asset = new OfficeAssetModel();
                return asset;
            }
            else
            {
                AssetTrackingFinal data = context.AssetTrackingFinals.Where(x => x.OID == id).FirstOrDefault();
                return ObjectConverter<AssetTrackingFinal, OfficeAssetModel>.Convert(data);
            }
        }

        public List<OfficeAssetModel> FinalAssetList()
        {
            dynamic result = context.Database.SqlQuery<OfficeAssetModel>("exec sp_Asset_GetFinalAssetList").ToList();
            return result;
        }

        public OfficeAssetModel AssetDetails(int id)
        {
            AssetTrackingFinal data = context.AssetTrackingFinals.FirstOrDefault();
            return ObjectConverter<AssetTrackingFinal, OfficeAssetModel>.Convert(data);
        }


        //----------------------------


        public AssetModel GetKGAsset(int id)
        {
            if (id == 0)
            {
                AssetModel asset = new AssetModel();
                return asset;
            }
            else
            {
                Asset data = context.Assets.Include(x => x.AssetFileAttaches).Where(x => x.AssetId == id).FirstOrDefault();
                return ObjectConverter<Asset, AssetModel>.Convert(data);
            }

        }


        public List<AssetModel> KGLandList(string searchText)
        {
            List<Asset> data = context.Assets.Include(x => x.Company).Include(x => x.FileAttachments).Include(x => x.District).Include(x => x.Upazila).Where(x => x.AssetTypeId == 3 && x.AssetCategoryId == 3 && (x.DeedNo.ToLower().Contains(searchText) || (x.Company.Name.ToLower().Contains(searchText)))).ToList();
            return ObjectConverter<Asset, AssetModel>.ConvertList(data).ToList();
        }


        public bool SaveOrEditKGLandAsset(AssetModel model)
        {
            if (model == null)
            {
                throw new Exception("Asset not found!");
            }
            bool result = false;
            Asset asset = new Asset();

            string comSerNo = model.CompanyId.ToString().PadLeft(2, '0');
            string subLocSerNo = context.AssetSubLocations.Where(x => x.SubLocationId == model.AssetSubLocationId).Select(x => x.SerialNo).FirstOrDefault();
            string productSerNo = context.AssetTypes.Where(x => x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNo).FirstOrDefault();
            string assetSerNo = context.Assets.Where(x => x.CompanyId == model.CompanyId && x.AssetSubLocationId == model.AssetSubLocationId && x.AssetTypeId == model.AssetTypeId).Select(x => x.SerialNO).FirstOrDefault();
            assetSerNo = assetSerNo ?? "00-00000-00000-0000";

            var sno = Convert.ToInt32(assetSerNo.Substring(15, 4));

            if (model.AssetId > 0)
            {
                asset = context.Assets.FirstOrDefault(x => x.AssetId == model.AssetId);
                if (asset == null)
                {
                    throw new Exception("Asset not found!");
                }
                asset.ModifiedDate = DateTime.Now;
                asset.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocationId = model.AssetSubLocationId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.StatusId = model.StatusId;
                asset.ColorId = model.ColorId;
                asset.ModelNo = model.ModelNo;
                asset.Weight = model.Weight;
                asset.Quantity = 1;

                asset.UnitPrice = model.UnitPrice;
                asset.DistrictId = model.DistrictId;
                asset.UpazillaId = model.UpazillaId;
                asset.ReceiverNameEn = model.ReceiverNameEn;
                asset.DonerNameEn = model.DonerNameEn;
                asset.DeedNo = model.DeedNo;
                asset.DeedDate = model.DeedDate;
                asset.BiaDeedNoAndDateEn = model.BiaDeedNoAndDateEn;
                asset.AmountOfLandPurchasedEn = model.AmountOfLandPurchasedEn;
                asset.CS = model.CS;
                asset.SA = model.SA;
                asset.RS = model.RS;
                asset.BS = model.BS;
                asset.CSDag = model.CSDag;
                asset.SADag = model.SADag;
                asset.RSDag = model.RSDag;
                asset.BSDag = model.BSDag;
                asset.TotalLandOfSADag = model.TotalLandOfSADag;
                asset.TotalLandOfRSDag = model.TotalLandOfRSDag;
                asset.TotalLandOfBSDag = model.TotalLandOfBSDag;
                asset.PurchaseLandOfSADag = model.PurchaseLandOfSADag;
                asset.PurchaseLandOfRSDag = model.PurchaseLandOfRSDag;
                asset.PurchaseLandOfBSDag = model.PurchaseLandOfBSDag;
                asset.RemainingLandOfSADag = model.RemainingLandOfSADag;
                asset.RemainingLandOfRSDag = model.RemainingLandOfRSDag;
                asset.RemainingLandOfBSDag = model.RemainingLandOfBSDag;
                asset.JotNot = model.JotNot;
                asset.KhatianNo = model.KhatianNo;
                asset.DagNo = model.DagNo;
                asset.RemainingLand = model.RemainingLand;
                asset.ProjectId = model.ProjectId;
                asset.UserName = model.UserName;
                asset.SoldLand = model.SoldLand;
                asset.PresentLand = model.PresentLand;
                asset.MortgageLand = model.MortgageLand;
                asset.NonMortgageLand = model.NonMortgageLand;
                asset.MortgageInstitution = model.MortgageInstitution;
                asset.DisputedListId = model.DisputedListId;
                asset.OtherDisputed = model.OtherDisputed;
                asset.CityJoripKhatiyan = model.CityJoripKhatiyan;
                asset.CityJoripDag = model.CityJoripDag;
                asset.LandReceiverId = model.LandReceiverId;
                asset.LandUserId = model.LandUserId;
                asset.Eunion=model.Eunion;
                asset.Mouja=model.Mouja;

                asset.Remarks = model.Remarks;
                asset.SerialNO = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                //if (context.SaveChanges() > 0)
                //{
                //    result = true;
                //}
            }

            else
            {
                asset.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                asset.CreatedDate = DateTime.Now;
                asset.CompanyId = model.CompanyId;
                asset.AssetLocationId = model.AssetLocationId;
                asset.AssetSubLocationId = model.AssetSubLocationId;
                asset.AssetCategoryId = model.AssetCategoryId;
                asset.AssetTypeId = model.AssetTypeId;
                asset.StatusId = model.StatusId;
                asset.ColorId = model.ColorId;
                asset.ModelNo = model.ModelNo;
                asset.Weight = model.Weight;
                asset.Quantity = 1;

                asset.UnitPrice = model.UnitPrice;
                asset.DistrictId = model.DistrictId;
                asset.UpazillaId = model.UpazillaId;
                asset.ReceiverNameEn = model.ReceiverNameEn;
                asset.DonerNameEn = model.DonerNameEn;
                asset.DeedNo = model.DeedNo;
                asset.DeedDate = model.DeedDate;
                asset.BiaDeedNoAndDateEn = model.BiaDeedNoAndDateEn;
                asset.AmountOfLandPurchasedEn = model.AmountOfLandPurchasedEn;
                asset.CS = model.CS;
                asset.SA = model.SA;
                asset.RS = model.RS;
                asset.BS = model.BS;
                asset.CSDag = model.CSDag;
                asset.SADag = model.SADag;
                asset.RSDag = model.RSDag;
                asset.BSDag = model.BSDag;
                asset.TotalLandOfSADag = model.TotalLandOfSADag;
                asset.TotalLandOfRSDag = model.TotalLandOfRSDag;
                asset.TotalLandOfBSDag = model.TotalLandOfBSDag;
                asset.PurchaseLandOfSADag = model.PurchaseLandOfSADag;
                asset.PurchaseLandOfRSDag = model.PurchaseLandOfRSDag;
                asset.PurchaseLandOfBSDag = model.PurchaseLandOfBSDag;
                asset.RemainingLandOfSADag = model.RemainingLandOfSADag;
                asset.RemainingLandOfRSDag = model.RemainingLandOfRSDag;
                asset.RemainingLandOfBSDag = model.RemainingLandOfBSDag;
                asset.JotNot = model.JotNot;
                asset.KhatianNo = model.KhatianNo;
                asset.DagNo = model.DagNo;
                asset.RemainingLand = model.RemainingLand;
                asset.ProjectId = model.ProjectId;
                asset.UserName = model.UserName;
                asset.SoldLand = model.SoldLand;
                asset.PresentLand = model.PresentLand;
                asset.MortgageLand = model.MortgageLand;
                asset.NonMortgageLand = model.NonMortgageLand;
                asset.MortgageInstitution = model.MortgageInstitution;
                asset.DisputedListId = model.DisputedListId;
                asset.OtherDisputed = model.OtherDisputed;
                asset.CityJoripKhatiyan = model.CityJoripKhatiyan;
                asset.CityJoripDag = model.CityJoripDag;
                asset.LandReceiverId = model.LandReceiverId;
                asset.LandUserId = model.LandUserId;

                asset.Remarks = model.Remarks;
                asset.SerialNO = comSerNo + "-" + subLocSerNo + "-" + productSerNo + "-" + (sno + 1).ToString().PadLeft(4, '0');
                //if (context.SaveChanges() > 0)
                //{
                //    result = true;
                //}
            }
            context.Entry(asset).State = asset.AssetId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                model.AssetId = asset.AssetId;
                return result = true;
            }
            else
            {
                return result;
            }
        }


    }
}
