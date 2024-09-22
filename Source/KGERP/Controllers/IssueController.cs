using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class IssueController : BaseController
    {
        private readonly IIssueService issue;
        private readonly IStockInfoService stockInfoService;
        public IssueController(IIssueService issue, IStockInfoService stockInfoService)
        {
            this.issue = issue;
            this.stockInfoService = stockInfoService;
        }

        public ActionResult CreateOrEdit()
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            IssueMasterInfoModel vm = new IssueMasterInfoModel();
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateOrEdit(IssueMasterInfoModel model)
        {
            issue.SaveIssueInformation(model);
            IssueMasterInfoModel vm = new IssueMasterInfoModel();
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            vm.StockInfos = stockInfoService.GetStockInfoSelectModels(companyId);
            return View(vm);
        }

        public JsonResult GetRmProducts(int productId, decimal qty)
        {
            var data = issue.GetRmProducts(productId, qty);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}