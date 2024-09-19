using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.Short__CreditService
{
    public class ShortCreditViewModel: BaseVM
    {
        public int VendorId {get; set; } 
        public int CompanyId {get; set; } 
        public string VendorName {get; set; } 
        public decimal Amount {get; set; } 

        public decimal Due {get; set; }
        public decimal BankCharge { get; set; }
        public long ShortCreditCollectionId { get; set; }      
        public decimal CollectedAmount { get; set; }
        public DateTime CollectionDate { get; set; }
        public int BankHeadId { get; set; }
        public int BankOrCashParantId { get; set; }      
        public DateTime CreateDate { get; set; }       
        public bool IsSubmitted { get; set; }
        public long VoucherId { get; set; }
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public int? HeadGLId { get;  set; }
        public string BankName { get;  set; }
        public string Message { get;  set; }
    }
}
