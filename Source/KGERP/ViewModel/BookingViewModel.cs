using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGERP.ViewModel
{
    public class BookingViewModel
    {
        public ProductInfoVm product { get; set; }
        public List<BookingHeadServiceModel> LstPurchaseCostHeads { get; set; }
    }
}