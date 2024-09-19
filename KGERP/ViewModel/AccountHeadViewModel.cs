using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{

    public class AccountHeadViewModel
    {
        public AccountHeadModel AccountHead { get; set; }
        public List<SelectModel> ParentHeads { get; set; }
        public List<AccountHeadProcessModel> AccountHeads { get; set; }
    }
}