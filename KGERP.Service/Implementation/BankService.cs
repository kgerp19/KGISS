using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class PaymentModeService : IPaymentModeService
    {
        private readonly ERPEntities context = new ERPEntities();
        //public PaymentModeService(ERPEntities context)
        //{
        //    this.context = context;
        //}
        public List<PaymentMode> GetPaymentModes()
        {
            return context.PaymentModes.ToList();
        }

        public List<SelectModel> GetPaymentModeSelectModels()
        {
            return context.PaymentModes.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PaymentModeId
            }).ToList();
        }

        public List<SelectModel> GetPaymentReceiveSelectModels()
        {
            return context.PaymentModes.Where(x => x.PaymentModeId == 2 || x.PaymentModeId == 4 || x.PaymentModeId == 8 || x.PaymentModeId == 9).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PaymentModeId
            }).ToList();
        }

        public List<SelectModel> PaymentModes()
        {
            return context.PaymentModes.Where(x => x.PaymentModeId == 2 || x.PaymentModeId == 4 || x.PaymentModeId == 10).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PaymentModeId
            }).ToList();
        }
    }
}
