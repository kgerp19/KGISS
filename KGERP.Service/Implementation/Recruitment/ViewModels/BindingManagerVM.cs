using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Recruitment.ViewModels
{
    public class BindingManagerVM
    {
       public SignatoryStatusEnum? SignatoryStatus { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set;}


    }
}
