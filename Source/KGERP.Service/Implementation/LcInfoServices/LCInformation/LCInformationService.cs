using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.ServiceModel;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.LcInfoServices.LCInformation
{
    public class LCInformationService : ILCInformationInterface
    {
        private readonly ERPEntities context;
        AccountingService _accountingService;
        public LCInformationService(ERPEntities context, AccountingService accountingService)
        {
            this.context = context;
            _accountingService = accountingService;
        }
        public long AddLC(LcInfoViewModel model)
        {
            LCInfo lCInfo = new LCInfo();
            lCInfo.CreatedDate = DateTime.Now;
            lCInfo.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            lCInfo.LCNo = model.LCNo;
            lCInfo.LCDate = model.LCDate;
            lCInfo.PINo = model.PINo;
            lCInfo.PIDate = model.PIDate;
            lCInfo.CompanyBankBranchId = model.CompanyBankBranchId ?? 0;
            lCInfo.AdvanceOrLoanGLId = model.AdvanceOrLoanGLId ?? 0;

            lCInfo.SupplierId = model.SupplierId;
            lCInfo.SupplierBankBranchId = model.SupplierBankBranchId;
            lCInfo.CountryofOriginId = model.CountryofOriginId;
            lCInfo.PortOfLoadingId = model.PortOfLoadingId;
            lCInfo.CountryOfDestinationId = model.CountryOfDestinationId;
            lCInfo.PortOfDischargeId = model.PortOfDischargeId;
            lCInfo.VesselId = (int)model.Vessal;
            lCInfo.LCType = (int)model.Lc;

            lCInfo.LCValue = model.LCValue;
            lCInfo.LCValueInBDT = model.LCValueInBDT;


            lCInfo.AmendmentIncrase = model.AmendmentIncrase;
            lCInfo.AmendmentDiccrase = model.AmendmentDiccrase;

            lCInfo.CashMarginPercent = model.CashMarginPercent;
            lCInfo.CashMarginAmount = model.CashMarginAmount;

            lCInfo.CurrenceyId = model.CurrenceyId;
            lCInfo.CurrenceyRate = model.CurrenceyRate;
            lCInfo.PaymentTerms = model.PaymentTerms;
            lCInfo.ShipmentDate = model.ShipmentDate;
            lCInfo.ShippingMark = model.ShippingMark;
            lCInfo.ExparyDate = model.ExparyDate;
            lCInfo.NotefyParty = model.NotefyParty;
            lCInfo.CompanyId = model.CompanyId;

            lCInfo.ProductOriginId = 0;
            lCInfo.IsActive = true;
            lCInfo.Status = (int)LCStatus.OnGoing;

            lCInfo.FreightCharges = model.FreightCharges;
            lCInfo.OtherCharge = model.OtherCharge;
            lCInfo.BankCharge = model.BankCharge;
            lCInfo.InsuranceValue = model.InsuranceValue;
            lCInfo.CommissionValue = model.CommissionValue;

            context.LCInfoes.Add(lCInfo);
            var currency = context.Currencies.FirstOrDefault(d => d.CurrencyId == model.CurrenceyId);
            var res = context.SaveChanges();
            if (res > 0)
            {
                if (lCInfo.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
                {
                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = lCInfo.LCNo + " Date: " + lCInfo.LCDate + " Currencey: " + currency.Name + " CurrencyRate: " + currency.CurrencyRate,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 50602917, //L/C Advance KFMAL
                        IsActive = true,

                        CompanyFK = lCInfo.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                    int headglId = LCAdvanceHeadPush(integration);

                    LCInfo cInfo = context.LCInfoes.Find(lCInfo.LCId);
                    cInfo.AccountingHeadId = headglId;
                }
                else if (lCInfo.CompanyId == (int)CompanyName.KrishibidPackagingLimited)
                {
                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = lCInfo.LCNo + " Date: " + lCInfo.LCDate + " Currencey: " + currency.Name + " CurrencyRate: " + currency.CurrencyRate,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = 40478, //L/C Advance Packaging
                        IsActive = true,

                        CompanyFK = lCInfo.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                    int headglId = LCAdvanceHeadPush(integration);

                    LCInfo cInfo = context.LCInfoes.Find(lCInfo.LCId);
                    cInfo.AccountingHeadId = headglId;
                }


                context.SaveChanges();


                return lCInfo.LCId;
            }

            else
            {
                return 0;
            }
        }
        public int LCAdvanceHeadPush(VMHeadIntegration vmHeadIntegration)
        {
            int result = -1;
            #region Account Code
            string newAccountCode = "";
            int orderNo = 0;
            Head5 parentHead = context.Head5.Where(x => x.Id == vmHeadIntegration.ParentId).FirstOrDefault();
            IQueryable<HeadGL> childHeads = context.HeadGLs.Where(x => x.ParentId == vmHeadIntegration.ParentId);
            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }
            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }
            #endregion

            HeadGL headGL = new HeadGL
            {
                Id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = orderNo,
                Remarks = vmHeadIntegration.Remarks,
                IsActive = true

            };
            context.HeadGLs.Add(headGL);
            if (context.SaveChanges() > 0)
            {
                result = headGL.Id;
            }
            return result;
        }
        public LcInfoViewModel LCDetails(long id)
        {
            LcInfoViewModel model = new LcInfoViewModel();
            model = (from t1 in context.LCInfoes
                     join t2 in context.Vendors on t1.SupplierId equals t2.VendorId

                     join t3 in context.BankBranches on t1.SupplierBankBranchId equals t3.BankBranchId
                     join t3_1 in context.Banks on t3.BankId equals t3_1.BankId

                     join t0 in context.BankBranches on t1.CompanyBankBranchId equals t0.BankBranchId into t0_Join
                     from t0 in t0_Join.DefaultIfEmpty()
                     join t0_1 in context.Banks on t0.BankId equals t0_1.BankId into t0_1_Join
                     from t0_1 in t0_1_Join.DefaultIfEmpty()

                     join t10 in context.HeadGLs on t1.AdvanceOrLoanGLId equals t10.Id into t10_Join
                     from t10 in t10_Join.DefaultIfEmpty()

                     join t4 in context.Companies on t1.CompanyId equals t4.CompanyId
                     join t5 in context.Countries on t1.CountryofOriginId equals t5.CountryId
                     join t6 in context.PortOfCountries on t1.PortOfLoadingId equals t6.PortOfCountryId
                     join t7 in context.Countries on t1.CountryOfDestinationId equals t7.CountryId
                     join t8 in context.PortOfCountries on t1.PortOfDischargeId equals t8.PortOfCountryId
                     join t9 in context.Currencies on t1.CurrenceyId equals t9.CurrencyId




                     where t1.LCId == id && t1.IsActive == true
                     select new LcInfoViewModel
                     {
                         AccountingHeadId = t1.AccountingHeadId,
                         AccountingBankHeadId = t0 != null ? t0.AccountHeadId : t1.AdvanceOrLoanGLId,
                         // t10
                         LCId = t1.LCId,
                         LCNo = t1.LCNo,
                         LCDate = t1.LCDate,
                         LCType = t1.LCType,
                         PINo = t1.PINo,
                         PIDate = t1.PIDate,
                         SupplierBankBranchId = t1.SupplierBankBranchId,
                         SupplierName = t2.Name,
                         SupplierId = t1.SupplierId,
                         SupplierAddress = t2.Address,
                         SupplierPhone = t2.Phone,
                         SupplierEmail = t2.Email,
                         CompanyBankBranchId = t1.CompanyBankBranchId,
                         LCValueInBDT = t1.LCValueInBDT,
                         SupplierBankName = t3_1.Name,
                         CompanyId = t1.CompanyId,
                         CompanyNmae = t4.Name,
                         CompanyBankNmae = t0 != null ? t0_1.Name : t10.AccCode + "-" + t10.AccName,
                         CountryofOriginName = t5.CountryName,
                         CountryofOriginId = t1.CountryofOriginId,
                         CountryOfDestinationId = t1.CountryOfDestinationId,
                         PortOfDischargeId = t1.PortOfDischargeId,
                         PortOfLoadingId = t1.PortOfLoadingId,
                         PortOfLoadingName = t6.PortName,
                         CurrenceyId = t1.CurrenceyId,
                         CountryOfDestinationName = t7.CountryName,
                         PortOfDischargeName = t8.PortName,
                         CurrenceyName = t9.Name,
                         CurrenceyRate = t1.CurrenceyRate,
                         VesselId = t1.VesselId,
                         Vessal = (VesselTypeEnum)t1.VesselId,
                         LCValue = t1.LCValue,
                         Lc = (LcTypeEnum)t1.LCType,
                         AmendmentIncrase = (decimal)t1.AmendmentIncrase,
                         AmendmentDiccrase = (decimal)t1.AmendmentDiccrase,
                         CashMarginPercent = (decimal)t1.CashMarginPercent,
                         CashMarginAmount = (decimal)t1.CashMarginAmount,
                         BankCharge = (decimal)t1.BankCharge,
                         InsuranceValue = (decimal)t1.InsuranceValue,
                         CommissionValue = (decimal)t1.CommissionValue,
                         FreightCharges = (decimal)t1.FreightCharges,
                         OtherCharge = (decimal)t1.OtherCharge,
                         PaymentTerms = t1.PaymentTerms,
                         ExparyDate = t1.ExparyDate,
                         ShipmentDate = t1.ShipmentDate,
                         ShippingMark = t1.ShippingMark,
                         NotefyParty = t1.NotefyParty,
                         CreatedBy = t1.CreatedBy,
                         CreatedDate = t1.CreatedDate,
                         Status = t1.Status,
                         CurrencySign = t9.Sign,
                         LCDateAmmendMend = DateTime.Now,
                         AmendmentDate = DateTime.Now,

                         TotalLandedValue = ((decimal)t1.CommissionValue + (decimal)t1.InsuranceValue + (decimal)t1.BankCharge + (decimal)t1.OtherCharge)

                     }
                ).FirstOrDefault();
            model.PurchaseorderList = (from t1 in context.PurchaseOrders
                                       join t2 in context.Vendors on t1.SupplierId equals t2.VendorId
                                       join t3 in context.PurchaseOrderDetails on t1.PurchaseOrderId equals t3.PurchaseOrderId
                                       join t4 in context.Products on t3.ProductId equals t4.ProductId
                                       where t1.LCId == id && t1.IsActive == true
                                       group new { t1, t2, t3, t4 } by t1.PurchaseOrderNo into grouped
                                       select new VMPurchaseOrderSlave
                                       {
                                           OrderNo = grouped.Key,
                                           SupplierName = grouped.FirstOrDefault().t2.Name,
                                           PurchaseOrderDetails = grouped.Select(item => new VMPurchaseOrderDetail
                                           {
                                               ProductName = item.t4.ProductName,
                                               PurchaseQuantity = item.t3.PurchaseQty,
                                               PurchasingPrice = item.t3.PurchaseAmount,
                                               PurchaseAmount = item.t3.PurchaseRate,


                                           }).ToList(),
                                           OrderDate = grouped.FirstOrDefault().t1.PurchaseDate,
                                           DeliveryAddress = grouped.FirstOrDefault().t1.DeliveryAddress
                                       }).ToList();

            model.AmendmentList = (from t1 in context.LCAmendments
                                   join t2 in context.LCInfoes on t1.LCId equals t2.LCId
                                   where t1.LCId == id && t1.IsActive

                                   select new VMLCAmendment
                                   {
                                       IsSubmit = t1.IsSubmit,
                                       LCAmendmentId = t1.LCAmendmentId,
                                       LCId = t1.LCId,
                                       AmendmentDate = t1.AmendmentDate,
                                       AmendmentValue = t1.AmendmentValue,
                                       IsIncrase = t1.IsIncrase,
                                       CreatedBy = t1.CreatedBy,
                                       Remarks = t1.Remark
                                   }).ToList();
            model.LcValueAmendmentList = (from t1 in context.LCValueAmendments
                                          join t2 in context.LCInfoes on t1.LCId equals t2.LCId
                                          where t1.LCId == id && t1.IsActive

                                          select new LcValueAmmendmentvm
                                          {
                                              LCValueAmendmentId = t1.LCValueAmendmentId,
                                              LCId = t1.LCId,
                                              AmendmentDate = t1.AmendmentDate,
                                              LCValue = t1.LCValue,
                                              LCValueInBDT = t1.LCValueInBDT,
                                              CashMarginAmount = t1.CashMarginAmount,
                                              CashMarginPercent = t1.CashMarginPercent,
                                              BankCharge = t1.BankCharge,
                                              InsuranceValue = t1.InsuranceValue,
                                              CommissionValue = t1.CommissionValue,
                                              FreightCharges = t1.FreightCharges,
                                              OtherCharge = t1.OtherCharge,
                                              Status = t1.Status,
                                              CreatedBy = t1.CreatedBy,
                                          }).ToList();


            return model;

        }


        public VMLCAmendment AmendmentDetails(long LCAmendmentId)
        {
            VMLCAmendment model = new VMLCAmendment();

            model = (from t1 in context.LCAmendments
                     join t2 in context.LCInfoes on t1.LCId equals t2.LCId
                     join t0 in context.BankBranches on t2.CompanyBankBranchId equals t0.BankBranchId into t0_Join
                     from t0 in t0_Join.DefaultIfEmpty()
                     join t0_1 in context.Banks on t0.BankId equals t0_1.BankId into t0_1_Join
                     from t0_1 in t0_1_Join.DefaultIfEmpty()

                     join t10 in context.HeadGLs on t2.AdvanceOrLoanGLId equals t10.Id into t10_Join
                     from t10 in t10_Join.DefaultIfEmpty()
                     where t1.LCAmendmentId == LCAmendmentId && t1.IsActive

                     select new VMLCAmendment
                     {
                         AccountingHeadId = t2.AccountingHeadId,
                         AccountingBankHeadId = t0 != null ? t0.AccountHeadId : t2.AdvanceOrLoanGLId,

                         LCAmendmentId = t1.LCAmendmentId,
                         LCId = t1.LCId,
                         LCNO = t2.LCNo,

                         AmendmentDate = t1.AmendmentDate,
                         AmendmentValue = t1.AmendmentValue,
                         IsIncrase = t1.IsIncrase,
                         CreatedBy = t1.CreatedBy,
                         Remarks = t1.Remark
                     }).FirstOrDefault();


            return model;

        }

        public LcInfoViewModel LCList(int companyId)
        {
            LcInfoViewModel model = new LcInfoViewModel();
            model.listdata = (from t1 in context.LCInfoes
                              join t2 in context.Vendors on t1.SupplierId equals t2.VendorId

                              //join t3 in context.BankBranches on t1.SupplierBankBranchId equals t3.BankBranchId
                              //join t3_1 in context.Banks on t3.BankId equals t3_1.BankId

                              //join t0 in context.BankBranches on t1.CompanyBankBranchId equals t0.BankBranchId
                              //join t0_1 in context.Banks on t0.BankId equals t0_1.BankId

                              //join t4 in context.Companies on t1.CompanyId equals t4.CompanyId
                              //join t5 in context.Countries on t1.CountryofOriginId equals t5.CountryId
                              //join t6 in context.PortOfCountries on t1.PortOfLoadingId equals t6.PortOfCountryId
                              //join t7 in context.Countries on t1.CountryOfDestinationId equals t7.CountryId
                              //join t8 in context.PortOfCountries on t1.PortOfDischargeId equals t8.PortOfCountryId
                              //join t9 in context.Currencies on t1.CurrenceyId equals t9.CurrencyId
                              where t1.CompanyId == companyId && t1.IsActive == true
                              select new LcInfoViewModel
                              {
                                  LCId = t1.LCId,
                                  LCNo = t1.LCNo,
                                  LCDate = t1.LCDate,
                                  LCType = t1.LCType,
                                  PINo = t1.PINo,
                                  PIDate = t1.PIDate,
                                  SupplierName = t2.Name,
                                  //SupplierAddress = t2.Address,
                                  //SupplierPhone = t2.Phone,
                                  //SupplierEmail = t2.Email,
                                  //SupplierBankId = t3.BankId,
                                  //SupplierBankName = t4.Name,
                                  //CompanyId = t1.CompanyId,
                                  //CompanyNmae = t4.Name,
                                  //CompanyBankNmae = t10.Name,
                                  //CountryofOriginName = t5.CountryName,
                                  //PortOfLoadingName = t6.PortName,
                                  //CountryOfDestinationName = t7.CountryName,
                                  //PortOfDischargeName = t8.PortName,
                                  //CurrenceyName = t9.Name,
                                  //CurrenceyRate = t1.CurrenceyRate,
                                  //VesselId = t1.VesselId,
                                  LCValue = t1.LCValue,
                                  //AmendmentIncrase = t1.AmendmentIncrase,
                                  //AmendmentDiccrase = t1.AmendmentDiccrase,
                                  //CashMarginPercent = t1.CashMarginPercent,
                                  //CashMarginAmount = t1.CashMarginAmount,
                                  //BankCharge = t1.BankCharge,
                                  //InsuranceValue = t1.InsuranceValue,
                                  //CommissionValue = t1.CommissionValue,
                                  //PaymentTerms = t1.PaymentTerms,
                                  //ExparyDate = t1.ExparyDate,
                                  //ShipmentDate = t1.ShipmentDate,
                                  //ShippingMark = t1.ShippingMark,
                                  //NotefyParty = t1.NotefyParty,
                                  // CreatedBy = t1.CreatedBy,
                                  Status = t1.Status,
                              }
                ).OrderByDescending(x => x.LCId).ToList();
            return model;
        }

        public int LCDelete(LcInfoViewModel model)
        {
            var res = context.LCInfoes.FirstOrDefault(f => f.LCId == model.LCId);
            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            res.ModifiedDate = DateTime.Now;
            res.IsActive = false;
            context.Entry(res).State = EntityState.Modified;
            context.SaveChanges();
            return res.CompanyId;

        }
        public async Task<int> LCAmendmentSubmit(LcInfoViewModel model)
        {
            var ammandment = AmendmentDetails(model.LCAmendmentId);
            ammandment.CompanyId = model.CompanyId;
            if (ammandment.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                await _accountingService.AccountingLCAmendmentPushKFMAL(ammandment.CompanyId, ammandment, (int)KfmalJournalEnum.LCAdvanceVoucher);

            }

            LCAmendment info = context.LCAmendments.Find(ammandment.LCAmendmentId);
            info.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            info.ModifiedDate = DateTime.Now;
            info.IsSubmit = true;
            context.Entry(info).State = EntityState.Modified;
            context.SaveChanges();
            return ammandment.CompanyId;

        }

        public async Task<int> LCSubmit(LcInfoViewModel model)
        {
            var res = LCDetails(model.LCId);
            if (res.CompanyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                await _accountingService.AccountingLCAdvancePushKFMAL(model.CompanyId, res, (int)KfmalJournalEnum.LCAdvanceVoucher);

            }
            if (res.CompanyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                await _accountingService.AccountingLCAdvancePushPackaging(model.CompanyId, res, (int)PackagingJournalEnum.LCAdvanceVoucher);

            }


            LCInfo info = context.LCInfoes.Find(res.LCId);
            info.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            info.ModifiedDate = DateTime.Now;
            info.Status = (int)LCStatus.Sumitted;
            context.Entry(info).State = EntityState.Modified;
            context.SaveChanges();
            return res.CompanyId;

        }


        public async Task<int> LCValueAmendmentSubmit(LcInfoViewModel model)
        {
            var res = GetLcAmmendmentValue(model.LCValueAmendmentId);
            if (res.CompanyFK == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                await _accountingService.AccLCValueAmendmentAdvancePushKFMAL(res.CompanyFK.Value, res, (int)KfmalJournalEnum.LCAdvanceVoucher);

            }
            //if (res.CompanyId == (int)CompanyName.KrishibidPackagingLimited)
            //{
            //    await _accountingService.AccountingLCAdvancePushPackaging(model.CompanyId, res, (int)PackagingJournalEnum.LCAdvanceVoucher);

            //}


            LCValueAmendment info = context.LCValueAmendments.Find(res.LCValueAmendmentId);
            info.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            info.ModifiedDate = DateTime.Now;
            info.Status = (int)LCStatus.Sumitted;
            context.Entry(info).State = EntityState.Modified;
            context.SaveChanges();
            return res.CompanyFK.Value;

        }




        public async Task<long> LCAmendmentSave(LcInfoViewModel model)
        {
            if(model.LCAmendmentId == 0)
            {
                LCAmendment amendment = new LCAmendment();
                amendment.LCId = model.LCId;
                amendment.AmendmentDate = model.AmendmentDate;
                amendment.AmendmentValue = model.AmendmentValue;
                amendment.IsIncrase = model.IsIncrase;
                amendment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                amendment.CreatedDate = DateTime.Now;
                amendment.IsActive = true;
                amendment.Remark = model.Remark;
                context.LCAmendments.Add(amendment);
                context.SaveChanges();
            }
            else
            {
                var obj = context.LCAmendments.Where(x => x.LCAmendmentId == model.LCAmendmentId).FirstOrDefault();
                if(obj == null)
                {
                    return model.LCId;
                }
                else
                {
                    obj.AmendmentValue = model.AmendmentValue;
                    obj.IsIncrase = model.IsIncrase;
                    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    obj.ModifiedDate = DateTime.Now;
                    obj.Remark = model.Remark;
                    context.SaveChanges();
                }
            }
            return model.LCId;
        }


        public async Task<long> LCValueAmendmentSave(LcInfoViewModel model)
        {
            if (model.LCValueAmendmentId == 0)
            {
                LCValueAmendment amendment = new LCValueAmendment();
                amendment.LCId = model.LCId;
                amendment.AmendmentDate = model.LCDateAmmendMend;
                amendment.LCValue = model.LCValueAmendMendt;
                amendment.LCValueInBDT = model.LCValueInBDTAmendMendt;
                amendment.CashMarginAmount = model.CashMarginAmountAmendMendt;
                amendment.CashMarginPercent = model.CashMarginPercentAmendMendt;
                amendment.FreightCharges = model.FreightChargesAmendMendt;
                amendment.BankCharge = model.BankChargeAmendMendt;
                amendment.InsuranceValue = model.InsuranceValueAmendMendt;
                amendment.CommissionValue = model.CommissionValueAmendMendt;
                amendment.OtherCharge = model.OtherChargeAmendMendt;
                amendment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                amendment.CreatedDate = DateTime.Now;
                amendment.IsActive = true;
                context.LCValueAmendments.Add(amendment);
                context.SaveChanges();

            }
            else
            {
                var obj = context.LCValueAmendments.Where(x => x.LCValueAmendmentId == model.LCValueAmendmentId).FirstOrDefault();
                if (obj == null)
                {
                    return model.LCId;
                }
                else
                {
                    obj.LCValue = model.LCValueAmendMendt;
                    obj.AmendmentDate = model.LCDateAmmendMend;
                    obj.LCValueInBDT = model.LCValueInBDTAmendMendt;
                    obj.CashMarginPercent = model.CashMarginPercentAmendMendt;
                    obj.OtherCharge = model.OtherChargeAmendMendt;
                    obj.BankCharge = model.BankChargeAmendMendt;
                    obj.CashMarginAmount = model.CashMarginAmountAmendMendt;
                    obj.InsuranceValue = model.InsuranceValueAmendMendt;
                    obj.CommissionValue = model.CommissionValueAmendMendt;
                    obj.FreightCharges = model.FreightChargesAmendMendt;
                    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    obj.ModifiedDate = DateTime.Now;
                    context.SaveChanges();
                }
            }
            return model.LCId;
        }




        public long UpdateLC(LcInfoViewModel model)
        {
            LCInfo lCInfo = context.LCInfoes.FirstOrDefault(f => f.LCId == model.LCId);
            lCInfo.ModifiedDate = DateTime.Now;
            lCInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            lCInfo.LCNo = model.LCNo;
            lCInfo.LCDate = model.LCDate;
            lCInfo.PINo = model.PINo;
            lCInfo.PIDate = model.PIDate;
            lCInfo.CompanyBankBranchId = model.CompanyBankBranchId ?? 0;
            lCInfo.AdvanceOrLoanGLId = model.AdvanceOrLoanGLId ?? 0;
            lCInfo.SupplierId = model.SupplierId;
            lCInfo.SupplierBankBranchId = model.SupplierBankBranchId;
            lCInfo.CountryofOriginId = model.CountryofOriginId;
            lCInfo.PortOfLoadingId = model.PortOfLoadingId;
            lCInfo.CountryOfDestinationId = model.CountryOfDestinationId;
            lCInfo.PortOfDischargeId = model.PortOfDischargeId;
            lCInfo.VesselId = (int)model.Vessal;
            lCInfo.LCType = (int)model.Lc;
            lCInfo.LCValue = model.LCValue;
            lCInfo.LCValueInBDT = model.LCValueInBDT;
            lCInfo.AmendmentIncrase = model.AmendmentIncrase;
            lCInfo.AmendmentDiccrase = model.AmendmentDiccrase;

            lCInfo.CashMarginPercent = model.CashMarginPercent;
            lCInfo.CashMarginAmount = model.CashMarginAmount;

            lCInfo.FreightCharges = model.FreightCharges;
            lCInfo.BankCharge = model.BankCharge;
            lCInfo.InsuranceValue = model.InsuranceValue;
            lCInfo.CommissionValue = model.CommissionValue;
            lCInfo.OtherCharge = model.OtherCharge;

            lCInfo.CurrenceyId = model.CurrenceyId;
            lCInfo.CurrenceyRate = model.CurrenceyRate;
            lCInfo.PaymentTerms = model.PaymentTerms;
            lCInfo.ShipmentDate = model.ShipmentDate;
            lCInfo.ShippingMark = model.ShippingMark;
            lCInfo.ExparyDate = model.ExparyDate;
            lCInfo.NotefyParty = model.NotefyParty;
            lCInfo.CompanyId = model.CompanyId;
            context.Entry(lCInfo).State = EntityState.Modified;
            var res = context.SaveChanges();
            if (res > 0)
            {
                return lCInfo.LCId;
            }
            else
            {
                return 0;
            }
        }



        public LcValueAmmendmentvm GetLcAmmendmentValue(long amendmentId)
        {
            LcValueAmmendmentvm model = new LcValueAmmendmentvm();
            model = (from t1 in context.LCValueAmendments
                     join t2 in context.LCInfoes on t1.LCId equals t2.LCId


                     join t0 in context.BankBranches on t2.CompanyBankBranchId equals t0.BankBranchId into t0_Join
                     from t0 in t0_Join.DefaultIfEmpty()
                     join t0_1 in context.Banks on t0.BankId equals t0_1.BankId into t0_1_Join
                     from t0_1 in t0_1_Join.DefaultIfEmpty()

                     where t1.LCValueAmendmentId == amendmentId && t1.IsActive

                     select new LcValueAmmendmentvm
                     {
                         AccountingHeadId = t2.AccountingHeadId,
                         AccountingBankHeadId = t0 != null ? t0.AccountHeadId : t2.AdvanceOrLoanGLId,

                         LCNo = t2.LCNo,
                         CompanyFK = t2.CompanyId,
                         LCValueAmendmentId = t1.LCValueAmendmentId,
                         LCId = t1.LCId,
                         AmendmentDate = t1.AmendmentDate,
                         LCValue = t1.LCValue,
                         LCValueInBDT = t1.LCValueInBDT,
                         CashMarginAmount = t1.CashMarginAmount,
                         CashMarginPercent = t1.CashMarginPercent,
                         BankCharge = t1.BankCharge,
                         InsuranceValue = t1.InsuranceValue,
                         CommissionValue = t1.CommissionValue,
                         FreightCharges = t1.FreightCharges,
                         OtherCharge = t1.OtherCharge,
                         Status = t1.Status,
                         CreatedBy = t1.CreatedBy,
                     }).FirstOrDefault();

            return model;
        }

        public bool DeleteAmmendMendValue(int ammendmendid)
        {
            var obj = context.LCValueAmendments.Where(x => x.LCValueAmendmentId == ammendmendid).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public LCAmendment GetLCAmendment(long amendmentId)
        {
            LCAmendment obj = context.LCAmendments.Where(x => x.LCAmendmentId == amendmentId).FirstOrDefault();           
            return obj;
        }
        public bool DeleteLCAmendment(int amendmentID)
        {
            var obj = context.LCAmendments.Where(x => x.LCAmendmentId == amendmentID).FirstOrDefault();
            if(obj != null)
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
