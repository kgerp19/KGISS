using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace KGERP.Service.Implementation
{
    public class StoreDetailService : IStoreDetailService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public StoreDetailService(ERPEntities context)
        {
            this.context = context;
        }
        public IEnumerable<StoreDetailModel> GetStoreDetails(long storeDetailId)
        {
            IQueryable<StoreDetail> storeDetails = context.StoreDetails.Include(x => x.Product).Where(x => x.StoreId == storeDetailId).OrderByDescending(x => x.StoreDetailId);
            return ObjectConverter<StoreDetail, StoreDetailModel>.ConvertList(storeDetails.ToList()).ToList();
        }

        private string GenerateSequenceNumber(long lastReceivedNo)
        {
            string input = string.Empty;
            long num = ++lastReceivedNo;
            input = num.ToString();
            if (input != string.Empty)
            {
                num = Convert.ToInt32(input);
            }
            return num.ToString().PadLeft(9, '0');
        }


        public StoreDetailModel GetStoreDetail(long id, long storeId)
        {
            string batchId = string.Empty;
            StoreDetail storeDetail = context.StoreDetails.Where(x => x.StoreDetailId == id).FirstOrDefault();
            if (storeDetail == null)
            {
                IQueryable<StoreDetail> storeDetails = context.StoreDetails;
                int count = storeDetails.Count();
                if (count == 0)
                {
                    return new StoreDetailModel()
                    {
                        BatchId = GenerateSequenceNumber(0),
                        StoreId = storeId,
                        //BatchDate = DateTime.Now
                    };
                }
                storeDetails = storeDetails.OrderByDescending(x => x.StoreDetailId).Take(1);
                batchId = storeDetails.ToList().FirstOrDefault().BatchId;
                long batchNo = Convert.ToInt64(batchId);
                batchId = GenerateSequenceNumber(batchNo);
                return new StoreDetailModel()
                {
                    BatchId = batchId,
                    StoreId = storeId,
                    //BatchDate = DateTime.Now
                };
            }
            StoreDetailModel model = ObjectConverter<StoreDetail, StoreDetailModel>.Convert(storeDetail);
            return model;
        }



        //public StoreDetailModel GetStoreDetail(long id, long storeId)
        //{
        //    if (id <= 0)
        //    {
        //        return new StoreDetailModel() { StoreId = storeId };
        //    }
        //    return ObjectConverter<StoreDetail, StoreDetailModel>.Convert(context.StoreDetails.Where(x => x.StoreDetailId == id).FirstOrDefault());
        //}


        public bool SaveStoreDetail(long id, StoreDetailModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Store Detail data missing!");
            }

            StoreDetail storeDetail = ObjectConverter<StoreDetailModel, StoreDetail>.Convert(model);
            if (id > 0)
            {
                storeDetail = context.StoreDetails.Where(x => x.StoreDetailId == id).FirstOrDefault();
                if (storeDetail == null)
                {
                    throw new Exception("Store data not found!");
                }
                //storeDetail.ModifiedDate = DateTime.Now;
                //storeDetail.ModifiedBy = "";
            }

            else
            {
                //storeDetail.IsActive = true;
                //storeDetail.CreatedDate = DateTime.Now;
                //storeDetail.CreatedBy = "";
            }

            storeDetail.StoreId = model.StoreId;
            storeDetail.ProductId = model.ProductId;

            //storeDetail.UnitPrice = model.UnitPrice;
            storeDetail.Qty = model.Qty;
            //storeDetail.TotalAmount = model.TotalAmount;
            storeDetail.BatchId = model.BatchId;
            //storeDetail.BatchDate = model.BatchDate;
            //storeDetail.Remarks = model.Remarks;
            try
            {
                context.Entry(storeDetail).State = storeDetail.StoreDetailId == 0 ? EntityState.Added : EntityState.Modified;
                return context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                message = "Something went wrong when trying to save Store Detail !";
                return false;
            }

        }


        public bool DeleteStoreDetail(long id)
        {
            StoreDetail storeDetail = context.StoreDetails.Where(x => x.StoreDetailId == id).FirstOrDefault();
            context.StoreDetails.Remove(storeDetail);
            return context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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


    }
}
