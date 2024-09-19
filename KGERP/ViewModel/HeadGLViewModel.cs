using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{

    public class HeadGLViewModel
    {
        public string CompanyName { get; set; }
        public AccountHeadProcessModel AccountHeadProcess { get; set; }
        public List<SelectModel> ParentHeads { get; set; }
        public List<AccountHeadProcessModel> AccountHeads { get; set; }
        public List<Head1> Head1s { get; set; }
    }
}