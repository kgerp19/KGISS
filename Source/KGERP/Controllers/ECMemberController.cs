using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ECMemberController : Controller
    {
        private readonly IECMemberService ecMemberService;
        public ECMemberController(IECMemberService ecMemberService)
        {
            this.ecMemberService = ecMemberService;
        }

        
        [HttpGet]
        public ActionResult Index(string searchText)
        {
            List<ECMemberModel> members = ecMemberService.GetECMembers(searchText);
            List<ECMemberModel> membersWithImage = members.Select(x => new ECMemberModel
            {
                ECMemberId = x.ECMemberId,
                MemberName = x.MemberName,
                MemberOrder = x.MemberOrder,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Phone = x.Phone,
                Email = x.Email,               
                MemberImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/ECMember/" + (string.IsNullOrEmpty(x.MemberImage) ? "default.png" : x.MemberImage),
            }).ToList();
            return View(membersWithImage);
        }

        [HttpGet]
        
        public ActionResult CreateOrEdit(int id)
        {
            ECMemberModel model = ecMemberService.GetECMember(id);
            model.MemberImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/ECMember/" + (string.IsNullOrEmpty(model.MemberImage) ? "default.png" : model.MemberImage);
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ECMemberModel model)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;

            if (model.MemberImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(model.MemberImageUpload.FileName);
                string extension = Path.GetExtension(model.MemberImageUpload.FileName);
                fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                model.MemberImage = fileName;
                model.MemberImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/ECMember/"), fileName));
            }

            if (model.ECMemberId <= 0)
            {
                result = ecMemberService.SaveECMember(0, model);
            }
            else
            {
                result = ecMemberService.SaveECMember(model.ECMemberId, model);
            }
            if (result)
            {
                TempData["message"] = "EC Member Saved Successfully !";
            }
            else
            {
                TempData["message"] = "EC Member not Saved !";
            }
            return RedirectToAction("Index");
        }

    }

}