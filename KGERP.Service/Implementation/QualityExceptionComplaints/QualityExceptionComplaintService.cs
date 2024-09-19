using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace KGERP.Service.Implementation.QualityExceptionComplaints
{
    public class QualityExceptionComplaintService : IQualityExceptionComplaintService
    {
        private readonly ERPEntities _db;

        public QualityExceptionComplaintService(ERPEntities db)
        {
            _db = db;
        }

        public async Task<RResult> AddQualityExceptionItem(int DropDownTypeId, string ExceptionText, int? CompanyId)
        {
            RResult rResult = new RResult();
            var UserCode = Common.GetUserId();
            if (!string.IsNullOrEmpty(UserCode))
            {
                DropDownItem dropDownItem = new DropDownItem
                {
                    DropDownTypeId = DropDownTypeId,
                    Name = ExceptionText,
                    OrderNo = 0,
                    Remarks = ExceptionText,
                    IsActive = true,
                    CreatedBy = UserCode,
                    CreatedDate = DateTime.Now,
                    ItemValue = 0,
                    CompanyId = CompanyId
                };

                _db.DropDownItems.Add(dropDownItem);
                if (await _db.SaveChangesAsync() > 0)
                {
                    rResult.result = 1;
                    rResult.message = "Add New Item Successfully!";
                }
                else
                {
                    rResult.result = 0;
                    rResult.message = "Data can't Save!";
                }
            }
            else
            {
                rResult.result = 0;
                rResult.message = "Data can't Save!";
            }


            return rResult;

        }

        public async Task<QualityExceptionComplaintDetailVM> GetAllDataOfQualityExceptionComplaint(long QualityExceptionComplaintId = 0, long QualityExceptionComplaintDetailId = 0, CancellationToken cancellationToken = default)
        {

            QualityExceptionComplaintDetailVM dataModel = new QualityExceptionComplaintDetailVM();
            try
            {


                if (QualityExceptionComplaintId > 0 || QualityExceptionComplaintDetailId > 0)
                {
                    dataModel = await (from t1 in _db.QualityExceptionComplaints
                                       join t2 in _db.QualityExceptionComplaintDetails on t1.QualityExceptionComplaintId equals t2.QualityExceptionComplaintId
                                       join t3 in _db.OrderDeliverDetails on t2.OrderDeliverDetailId equals t3.OrderDeliverDetailId
                                       join t4 in _db.OrderDelivers on t3.OrderDeliverId equals t4.OrderDeliverId
                                       join t5 in _db.OrderMasters on t4.OrderMasterId equals t5.OrderMasterId
                                       join t6 in _db.Vendors on t5.CustomerId equals t6.VendorId
                                       where t1.IsActive && t2.IsActive
                                       && t1.QualityExceptionComplaintId == QualityExceptionComplaintId
                                       && t2.QualityExceptionComplaintDetailId == QualityExceptionComplaintDetailId
                                       select new QualityExceptionComplaintDetailVM
                                       {
                                           QualityExceptionComplaintId = t1.QualityExceptionComplaintId,
                                           IsSubmited = t1.IsSubmited,
                                           QualityExceptionComplaintDetailId = t2.QualityExceptionComplaintDetailId,
                                           IsSubmitedDetails = t2.IsSubmited,
                                           DescriptionQualityException = t2.DescriptionQualityException,
                                           OtherRemarks = t2.OtherRemarks,
                                           DeliveredAffectedQty = t2.DeliveredAffectedQty,
                                           DeliveredAffectedWeight = t2.DeliveredAffectedWeight,
                                           DeliveredQty = t3.DeliveredQty,
                                           NetWeight = t3.NetWeight,
                                           CustomerId = t6.VendorId,
                                           OrderMasterId = t5.OrderMasterId,
                                           OrderDeliverId = t4.OrderDeliverId,
                                           OrderDeliverDetailId = t3.OrderDeliverDetailId,
                                           QualityException = t2.QualityException,
                                           ExceptionExplanation = t2.ExceptionExplanation,
                                           StepsTakenToPreventRecurrence = t2.StepsTakenToPreventRecurrence,
                                           PersonsResponsibleForQualityException = t2.PersonsResponsibleForQualityException

                                       }).FirstOrDefaultAsync(cancellationToken);

                    dataModel.DropDownItemForQualityExceptionList = await (from t1 in _db.DropDownItems
                                                                           join t2 in _db.QualityExceptionComplaintMaps
                                                                           on t1.DropDownItemId equals t2.DesQualityExceptionDropDownItemId into gj
                                                                           from t2g in gj.DefaultIfEmpty()
                                                                           where t1.IsActive && t1.DropDownTypeId == 75
                                                                           && t2g.QualityExceptionComplaintDetailId == QualityExceptionComplaintDetailId
                                                                           select new DropDownItemForQualityException
                                                                           {
                                                                               Text = t1.Name.ToString(),
                                                                               Value = t1.DropDownItemId.ToString(),
                                                                               QualityExceptionValue = t2g.DescriptionOfQualityExceptionValue
                                                                           }).ToListAsync(cancellationToken);

                }
                else
                {
                    dataModel.DropDownItemForQualityExceptionList = await _db.DropDownItems.Where(x =>
                                                                                            x.IsActive
                                                                                            && x.DropDownTypeId == 75
                                                                                            ).Select(x => new DropDownItemForQualityException
                                                                                            {
                                                                                                Text = x.Name.ToString(),
                                                                                                Value = x.DropDownItemId.ToString(),
                                                                                                QualityExceptionValue = false
                                                                                            }).ToListAsync(cancellationToken);
                }

                return dataModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<QualityExceptionComplaintDetailVM> GetDataOfOrderDeliveryDetailsByOrderDeliveryDetailId(long OrderDeliveryDetailsId, CancellationToken cancellationToken = default)
        {
            var datas = await _db.OrderDeliverDetails.FirstOrDefaultAsync(x => x.IsActive && x.OrderDeliverDetailId == OrderDeliveryDetailsId);
            QualityExceptionComplaintDetailVM detailVM = new QualityExceptionComplaintDetailVM();
            if (datas != null)
            {

                detailVM.DeliveredQty = datas.DeliveredQty;
                detailVM.NetWeight = datas.NetWeight;

            }
            return detailVM;
        }

        public async Task<QualityExceptionComplaintRM> ProductionInChargeSubmition(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default)
        {
            QualityExceptionComplaintRM complaintRM = new QualityExceptionComplaintRM();
            long QuaExcComplaintUpdateBy = Common.GetIntUserId();
            string QuaExcComplaintUpdate = Common.GetUserId();
            if (QuaExcComplaintUpdateBy <= 0 && string.IsNullOrEmpty(QuaExcComplaintUpdate))
            {
                return complaintRM;
            }
            if (detailVM.QualityExceptionComplaintDetailId > 0)
            {
                var resultData = await _db.QualityExceptionComplaintDetails.FirstOrDefaultAsync(x => x.IsActive && x.QualityExceptionComplaintDetailId == detailVM.QualityExceptionComplaintDetailId);
                if (resultData != null)
                {
                    resultData.QualityException = detailVM.QualityException;
                    resultData.PersonsResponsibleForQualityException = detailVM.PersonsResponsibleForQualityException;
                    resultData.ExceptionExplanation = detailVM.ExceptionExplanation;
                    resultData.StepsTakenToPreventRecurrence = detailVM.StepsTakenToPreventRecurrence;
                    resultData.SignatureOfProductionInCharge = QuaExcComplaintUpdateBy;
                    resultData.ModifiedBy = QuaExcComplaintUpdate;
                    resultData.ModifiedDate = DateTime.Now;
                    _db.Entry(resultData).State = EntityState.Modified;
                    if (await _db.SaveChangesAsync(cancellationToken) > 0)
                    {
                        complaintRM.QualityExceptionComplaintId = resultData.QualityExceptionComplaintId;
                        complaintRM.QualityExceptionComplaintDetailId = resultData.QualityExceptionComplaintDetailId;
                    }
                }
            }
            return complaintRM;
        }

        public async Task<QualityExceptionComplaintRM> QualityExceptionComplaintDetailsSubmit(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default)
        {
            QualityExceptionComplaintRM complaintRM = new QualityExceptionComplaintRM();
            string QuaExcComplaintCreateBy = Common.GetUserId();
            if (string.IsNullOrEmpty(QuaExcComplaintCreateBy))
            {
                return complaintRM;
            }

            if (detailVM.QualityException > 0 && detailVM.IsSubmited == false)
            {
                complaintRM = await SubmitQualityExceptionComplain(detailVM, complaintRM, QuaExcComplaintCreateBy, cancellationToken);
            }
            else
            {
                complaintRM = await SubmitQualityExceptionComplainDetails(detailVM, complaintRM, QuaExcComplaintCreateBy, cancellationToken);
            }

            return complaintRM;
        }

        private async Task<QualityExceptionComplaintRM> SubmitQualityExceptionComplain(QualityExceptionComplaintDetailVM detailVM, QualityExceptionComplaintRM complaintRM, string QuaExcComplaintCreateBy, CancellationToken cancellationToken)
        {
            if (detailVM != null && detailVM.QualityExceptionComplaintId > 0)
            {
                var QualityExceptionComplaintDbData = await _db.QualityExceptionComplaints.FirstOrDefaultAsync(x => x.QualityExceptionComplaintId == detailVM.QualityExceptionComplaintId, cancellationToken);
                if (QualityExceptionComplaintDbData == null)
                {
                    return complaintRM;
                }
                QualityExceptionComplaintDbData.IsSubmited = true;
                QualityExceptionComplaintDbData.ModifiedBy = QuaExcComplaintCreateBy;
                QualityExceptionComplaintDbData.ModifiedDate = DateTime.Now;
                _db.Entry(QualityExceptionComplaintDbData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync(cancellationToken) > 0)
                {
                    complaintRM.QualityExceptionComplaintDetailId = detailVM.QualityExceptionComplaintDetailId;
                    complaintRM.QualityExceptionComplaintId = QualityExceptionComplaintDbData.QualityExceptionComplaintId;

                    return complaintRM;
                }
            }
            return complaintRM;
        }

        private async Task<QualityExceptionComplaintRM> SubmitQualityExceptionComplainDetails(QualityExceptionComplaintDetailVM detailVM, QualityExceptionComplaintRM complaintRM, string QuaExcComplaintCreateBy, CancellationToken cancellationToken)
        {
            if (detailVM != null && detailVM.QualityExceptionComplaintDetailId > 0)
            {
                var QualityExceptionComplaintDetailDbData = await _db.QualityExceptionComplaintDetails.FirstOrDefaultAsync(x => x.QualityExceptionComplaintDetailId == detailVM.QualityExceptionComplaintDetailId, cancellationToken);
                if (QualityExceptionComplaintDetailDbData == null)
                {
                    return complaintRM;
                }
                QualityExceptionComplaintDetailDbData.IsSubmited = true;
                QualityExceptionComplaintDetailDbData.ModifiedBy = QuaExcComplaintCreateBy;
                QualityExceptionComplaintDetailDbData.ModifiedDate = DateTime.Now;
                _db.Entry(QualityExceptionComplaintDetailDbData).State = EntityState.Modified;
                if (await _db.SaveChangesAsync(cancellationToken) > 0)
                {
                    complaintRM.QualityExceptionComplaintDetailId = QualityExceptionComplaintDetailDbData.QualityExceptionComplaintDetailId;
                    complaintRM.QualityExceptionComplaintId = QualityExceptionComplaintDetailDbData.QualityExceptionComplaintId;

                    return complaintRM;
                }

            }
            return complaintRM;
        }

        public async Task<QualityExceptionComplaintRM> SaveAndUpdateDataQualityExceptionMasterChildAndMapTable(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default)
        {
            long createdBy = Common.GetIntUserId();
            string QuaExcComplaintCreateBy = Common.GetUserId();
            QualityExceptionComplaintRM responseModel = new QualityExceptionComplaintRM();
            responseModel.CompanyId = detailVM.CompanyId;
            if (createdBy <= 0 || string.IsNullOrEmpty(QuaExcComplaintCreateBy))
            {
                return responseModel;
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                if (detailVM.QualityExceptionComplaintId > 0 && detailVM.QualityExceptionComplaintDetailId > 0)
                {
                    responseModel = await UpdateDataQualityExceptionMasterChildAndMap(detailVM, createdBy, QuaExcComplaintCreateBy, responseModel, transaction, cancellationToken);
                }
                else
                {
                    responseModel = await SaveDataQualityExceptionMasterChildAndMap(detailVM, createdBy, QuaExcComplaintCreateBy, responseModel, transaction, cancellationToken);
                }

            }
            return responseModel;
        }

        private async Task<QualityExceptionComplaintRM> SaveDataQualityExceptionMasterChildAndMap(QualityExceptionComplaintDetailVM detailVM, long createdBy, string QuaExcComplaintCreateBy, QualityExceptionComplaintRM responseModel, DbContextTransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                var qualityExceptionComplaint = new QualityExceptionComplaint
                {
                    VendorId = detailVM.CustomerId,
                    ReportingPerson = createdBy,
                    CreatedBy = QuaExcComplaintCreateBy,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _db.QualityExceptionComplaints.Add(qualityExceptionComplaint);

                if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                {
                    return responseModel;
                }
                responseModel.QualityExceptionComplaintId = qualityExceptionComplaint.QualityExceptionComplaintId;
                var complaintDetail = new QualityExceptionComplaintDetail
                {
                    QualityExceptionComplaintId = qualityExceptionComplaint.QualityExceptionComplaintId,
                    OrderDeliverDetailId = detailVM.OrderDeliverDetailId,
                    DeliveredAffectedWeight = detailVM.DeliveredAffectedWeight,
                    DeliveredAffectedQty = detailVM.DeliveredAffectedQty,
                    DescriptionQualityException = detailVM.DescriptionQualityException,
                    OtherRemarks = detailVM.OtherRemarks,
                    CreatedBy = QuaExcComplaintCreateBy,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _db.QualityExceptionComplaintDetails.Add(complaintDetail);

                if (await _db.SaveChangesAsync(cancellationToken) <= 0)
                {
                    return responseModel;
                }
                responseModel.QualityExceptionComplaintDetailId = complaintDetail.QualityExceptionComplaintDetailId;
                var qualityExceptionMaps = detailVM.DropDownItemForQualityExceptionList
                    .Select(item => new QualityExceptionComplaintMap
                    {
                        QualityExceptionComplaintDetailId = complaintDetail.QualityExceptionComplaintDetailId,
                        DesQualityExceptionDropDownItemId = Convert.ToInt32(item.Value),
                        DescriptionOfQualityExceptionValue = item.QualityExceptionValue,
                        CreatedBy = QuaExcComplaintCreateBy,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    }).ToList();

                _db.QualityExceptionComplaintMaps.AddRange(qualityExceptionMaps);

                if (await _db.SaveChangesAsync(cancellationToken) > 0)
                {
                    transaction.Commit();
                    return responseModel;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return responseModel;
        }

        private async Task<QualityExceptionComplaintRM> UpdateDataQualityExceptionMasterChildAndMap(QualityExceptionComplaintDetailVM detailVM, long createdBy, string createdByString, QualityExceptionComplaintRM responseModel, DbContextTransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                // Update QualityExceptionComplaint
                var complaint = await _db.QualityExceptionComplaints
                    .FirstOrDefaultAsync(x => x.QualityExceptionComplaintId == detailVM.QualityExceptionComplaintId, cancellationToken);
                if (complaint == null) return responseModel;

                complaint.VendorId = detailVM.CustomerId;
                complaint.ReportingPerson = createdBy;
                complaint.ModifiedBy = createdByString;
                complaint.ModifiedDate = DateTime.Now;
                _db.Entry(complaint).State = EntityState.Modified;
                if (await _db.SaveChangesAsync(cancellationToken) <= 0) return responseModel;
                responseModel.QualityExceptionComplaintId = complaint.QualityExceptionComplaintId;

                // Update QualityExceptionComplaintDetail
                var complaintDetail = await _db.QualityExceptionComplaintDetails
                    .FirstOrDefaultAsync(x => x.QualityExceptionComplaintDetailId == detailVM.QualityExceptionComplaintDetailId, cancellationToken);
                if (complaintDetail == null) return responseModel;

                complaintDetail.OrderDeliverDetailId = detailVM.OrderDeliverDetailId;
                complaintDetail.DeliveredAffectedWeight = detailVM.DeliveredAffectedWeight;
                complaintDetail.DeliveredAffectedQty = detailVM.DeliveredAffectedQty;
                complaintDetail.DescriptionQualityException = detailVM.DescriptionQualityException;
                complaintDetail.OtherRemarks = detailVM.OtherRemarks;
                complaintDetail.ModifiedBy = createdByString;
                complaintDetail.ModifiedDate = DateTime.Now;
                _db.Entry(complaintDetail).State = EntityState.Modified;
                if (await _db.SaveChangesAsync(cancellationToken) <= 0) return responseModel;
                responseModel.QualityExceptionComplaintDetailId = complaintDetail.QualityExceptionComplaintDetailId;

                // Update QualityExceptionComplaintMaps
                var existingMaps = await _db.QualityExceptionComplaintMaps
                    .Where(x => x.QualityExceptionComplaintDetailId == detailVM.QualityExceptionComplaintDetailId)
                    .ToListAsync(cancellationToken);
                var updatedMaps = detailVM.DropDownItemForQualityExceptionList
                    .Select(item => new QualityExceptionComplaintMap
                    {
                        QualityExceptionComplaintDetailId = complaintDetail.QualityExceptionComplaintDetailId,
                        DesQualityExceptionDropDownItemId = Convert.ToInt32(item.Value),
                        DescriptionOfQualityExceptionValue = item.QualityExceptionValue,
                        ModifiedBy = createdByString,
                        ModifiedDate = DateTime.Now
                    }).ToList();

                foreach (var map in existingMaps)
                {
                    var updatedMap = updatedMaps.FirstOrDefault(x => x.DesQualityExceptionDropDownItemId == map.DesQualityExceptionDropDownItemId);
                    if (updatedMap != null)
                    {
                        map.DescriptionOfQualityExceptionValue = updatedMap.DescriptionOfQualityExceptionValue;
                        map.ModifiedBy = createdByString;
                        map.ModifiedDate = DateTime.Now;
                        _db.Entry(map).State = EntityState.Modified;
                    }
                }
                if (await _db.SaveChangesAsync(cancellationToken) > 0)
                {
                    transaction.Commit();
                    return responseModel;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return responseModel;
        }

        public async Task<QualityExceptionComplaintDetailVM> GetQualityExceptionComplaintList(DateTime? fromDate, DateTime? endDate)
        {
            QualityExceptionComplaintDetailVM model = new QualityExceptionComplaintDetailVM();
            model.DataList = await Task.Run(() => (from t1 in _db.QualityExceptionComplaints
                                                   join t2 in _db.QualityExceptionComplaintDetails on t1.QualityExceptionComplaintId equals t2.QualityExceptionComplaintId
                                                   join t3 in _db.Employees on t1.ReportingPerson equals t3.Id
                                                   join t4 in _db.Vendors on t1.VendorId equals t4.VendorId
                                                   join t5 in _db.OrderDeliverDetails on t2.OrderDeliverDetailId equals t5.OrderDeliverDetailId
                                                   join t6 in _db.Products on t5.ProductId equals t6.ProductId
                                                   join t8 in _db.OrderDelivers on t5.OrderDeliverId equals t8.OrderDeliverId
                                                   join t7 in _db.Employees on t2.PersonsResponsibleForQualityException equals t7.Id into ps
                                                   from t7 in ps.DefaultIfEmpty()
                                                   where t1.IsActive
                                                   && t2.IsActive
                                                   && t1.CreatedDate >= fromDate
                                                   && t1.CreatedDate <= endDate
                                                   select new QualityExceptionComplaintDetailVM
                                                   {
                                                       QualityExceptionComplaintId = t1.QualityExceptionComplaintId,
                                                       VendorName = t4.Name,
                                                       ReportingPersonName = t3.Name,
                                                       QualityExceptionComplaintDetailId = t2.QualityExceptionComplaintDetailId,
                                                       ProductName = t6.ProductType + " " + t6.ProductName,
                                                       DeliveredAffectedWeight = t2.DeliveredAffectedWeight,
                                                       DeliveredAffectedQty = t2.DeliveredAffectedQty,
                                                       PersonsResponsible = t7.Name,
                                                       DeliveryDate = t8.DeliveryDate
                                                   }).OrderByDescending(x => x.QualityExceptionComplaintId).AsEnumerable());

            return model;

        }

        public async Task<RResult> QualityExceptionComplaintDelete(long qualityExceptionComplaintId, CancellationToken cancellationToken = default)
        {
            RResult rResult = new RResult();
            string CurrentUser = Common.GetUserId();
            if (string.IsNullOrEmpty(CurrentUser))
            {
                rResult.result = 0;
                rResult.message = "Data can't Deleted!";
                return rResult;
            }
            var data = await _db.QualityExceptionComplaints.FirstOrDefaultAsync(x => x.QualityExceptionComplaintId == qualityExceptionComplaintId);
            if (data == null)
            {
                rResult.result = 0;
                rResult.message = "Data can't Deleted!";
                return rResult;
            }
            data.IsActive = false;
            data.ModifiedBy = CurrentUser;
            data.ModifiedDate = DateTime.Now;
            _db.Entry(data).State = EntityState.Modified;
            if (await _db.SaveChangesAsync(cancellationToken) > 0)
            {
                rResult.result = 1;
                rResult.message = "Data Deleted Successfully!";
            }
            return rResult;
        }
    }
}
