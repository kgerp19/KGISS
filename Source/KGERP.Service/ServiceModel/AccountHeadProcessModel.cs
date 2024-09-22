using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class AccountHeadProcessModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Account Code")]
        public string AccCode { get; set; }
        [DisplayName("Account Name")]
        public string AccName { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> LayerNo { get; set; }


        public virtual ICollection<AccountHeadProcessModel> AccountHeadProcess1 { get; set; }
        public virtual AccountHeadProcessModel AccountHeadProcess2 { get; set; }

        //----------------Extendex Property-----------
        public string Remarks { get; set; }
        [DisplayName("Parent Head")]
        public string ParentAccountName { get; set; }
        public string Status { get; set; }
        public string ButtonName { get; set; }
        public bool IsActive { get; set; }

    }

    public class VMAccountHead
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string AccCode { get; set; }
        public string GLName { get; set; }
        public string Head5Name { get; set; }
        public string Head4Name { get; set; }
        public string Head3Name { get; set; }
        public string Head2Name { get; set; }
        public string Head1Name { get; set; }
        public int? OrderNo { get; set; }
        public int? LayerNo { get; set; }
        public string Remarks { get; set; }

        public string Status { get; set; }

        public virtual IEnumerable<VMAccountHead> DataList { get; set; }



    }


}
