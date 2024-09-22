using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class StockInfosController : Controller
    {
        private readonly IStockInfoService _stockInfoService;
        public StockInfosController(IStockInfoService stockInfoService)
        {
            _stockInfoService = stockInfoService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId) 
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            StockInfoModel model = new StockInfoModel();
            

            model = await _stockInfoService.GetStockInfos(companyId);
           
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(StockInfoModel model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _stockInfoService.StockInfoAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _stockInfoService.StockInfoEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _stockInfoService.StockInfoDelete(model.StockInfoId);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId });
        }


    }
}