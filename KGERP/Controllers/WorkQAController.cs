using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class WorkQAController : BaseController
    {
        private readonly IWorkQAService workQAService;
        public WorkQAController(IWorkQAService workQAService)
        {
            this.workQAService = workQAService;
        }
        [HttpGet]
        
        public ActionResult Index()
        {
            List<WorkQAModel> models = workQAService.GetWorkQAs();
            List<WorkQAModel> newModels = new List<WorkQAModel>() { };
            foreach (var item in models)
            {
                WorkQAModel workQAModel = new WorkQAModel
                {
                    WorkQAId = item.WorkQAId,
                    FromEmpId = item.FromEmpId,
                    FromKGID = item.FromKGID,
                    FromName = item.FromName,
                    FromEmpImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Picture/" + (string.IsNullOrEmpty(item.FromEmpImage) ? "default.png" : item.FromEmpImage),
                    ToEmpId = item.ToEmpId,
                    ToKGID = item.ToKGID,
                    ToName = item.ToName,
                    ToEmpImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Picture/" + (string.IsNullOrEmpty(item.FromEmpImage) ? "default.png" : item.ToEmpImage),
                    Conversation = item.Conversation,
                    WorkQAFiles = workQAService.GetWorkQAFiles(item.WorkQAId)
                };
                newModels.Add(workQAModel);
            }
            return View(newModels);
        }

        [HttpGet]
        
        public ActionResult QuestionCreate(int id)
        {
            WorkQAModel model = workQAService.GetQuestion(id);
            return View(model);
        }

        [HttpPost]
        
        public ActionResult QuestionCreate(WorkQAModel model)
        {

            List<WorkQAFileModel> workQAFiles = new List<WorkQAFileModel>();
            foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null)
                {
                    WorkQAFileModel workQAFile = new WorkQAFileModel();
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                    file.SaveAs(Path.Combine(Server.MapPath("~/TaskManagement/ConversationFile/"), fileName));

                    workQAFile.EmpId = workQAService.GetCurrentEmpId();
                    workQAFile.FileName = fileName;
                    workQAFiles.Add(workQAFile);
                }

            }
            model.WorkQAFiles = workQAFiles;
            bool result = workQAService.SaveWorkQA(0, model);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }


        [HttpGet]
        
        public ActionResult AnswerCreate(int id)
        {
            WorkQAModel model = workQAService.GetAnswer(id);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult AnswerCreate(WorkQAModel model)
        {
            bool result = workQAService.SaveWorkQA(model.WorkQAId, model);

            if (result)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public FileResult Download(long workQAFileId)
        {
            string fileName = workQAService.GetAttachFile(workQAFileId);
            string path = Server.MapPath("~/TaskManagement/ConversationFile/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

    }
}