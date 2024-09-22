using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class MessageController : BaseController
    {
        private readonly IMessageService messageService;
        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        
        public ActionResult Message(int companyId)
        {
            MessageModel model = new MessageModel { CompanyId = companyId };
            return View(model);
        }

        
        public ActionResult SendMessage(MessageModel model)
        {
            int noOfMessageSent = messageService.SendMessage(model);
            return RedirectToAction("Message", new { model.CompanyId });
        }


    }
}