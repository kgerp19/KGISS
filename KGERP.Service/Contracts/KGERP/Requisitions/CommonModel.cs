using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Contracts.KGERP.Requisitions
{
    public class CommonModel
    {
        public string CreatedBy { get; set; }= System.Web.HttpContext.Current.User.Identity.Name;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
