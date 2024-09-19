using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using KGERP.Data.Models;
using KGERP.Service.Implementation.LcInfoServices;
using KGERP.Service.Implementation.LcInfoServices.LcCommonService;
using KGERP.Service.Implementation.LcInfoServices.LCInformation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.LCInformation
{
    [CheckSession]
    public class ShereMemberController : Controller
    {
       
        private readonly IShereMember _ishereMember;
        public ShereMemberController(IShereMember ishereMember)
        {
            _ishereMember = ishereMember;
           
        }
       
    }
}