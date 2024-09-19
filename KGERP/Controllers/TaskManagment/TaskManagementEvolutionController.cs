using DocumentFormat.OpenXml.Wordprocessing;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.TaskManagment;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using Microsoft.AspNetCore.Mvc;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;
using Newtonsoft.Json;

namespace KGERP.Controllers.TaskManagment
{
    [CheckSession]
    public class TaskManagementEvolutionController : Controller
    {
        public readonly ITaskManagementEvolutionService taskManagementEvolutionService;
        public TaskManagementEvolutionController(ITaskManagementEvolutionService taskManagementEvolutionService)
        {
            this.taskManagementEvolutionService = taskManagementEvolutionService;
        }
        #region task
        public async Task<ActionResult> TaskCreate(int? id)
        {
            TaskManagementEvolutionVM model = new TaskManagementEvolutionVM();
            model.ExpectedEndDate = DateTime.Now;
            if (id != null)
            {
                model = await taskManagementEvolutionService.GetTaskForUpdate(id);
                return View(model);
            }
            return View(model);
        }

        public async Task<ActionResult> TaskCreateWithWoorkstate(int Workstate,int WorkbordId,int WorkSpaceId)
        {
            TaskManagementEvolutionVM model = new TaskManagementEvolutionVM();
            model= await taskManagementEvolutionService.GetWorkstate(WorkbordId);
            model.ExpectedEndDate = DateTime.Now;
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.TaskStatusID = Workstate;
            model.WorkboardId = WorkbordId;
            model.workspaceid = WorkSpaceId;
            return View(model);
        }

        [HttpPost]
        public ActionResult TaskCreateWithWoorkstate(TaskManagementEvolutionVM model)
        {
            if (ModelState.IsValid)
            {
                var result = taskManagementEvolutionService.TaskAdd(model);

                var feedbackMessage = $"<label style=\"color: {(result.isOK == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.FeedbackMessage}</label>";
                TempData["FeedbackMessage"] = feedbackMessage;
            }
            return RedirectToAction("AllTaskList", new { WorkbordId = model.WorkboardId, WorkSpaceId=model.workspaceid });

        }
        [HttpPost]
        public ActionResult TaskCreateOrUpdate(TaskManagementEvolutionVM model)
        {
            if (ModelState.IsValid)
            {
                var result = taskManagementEvolutionService.TaskAdd(model);

                var feedbackMessage = $"<label style=\"color: {(result.isOK == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.FeedbackMessage}</label>";
                TempData["FeedbackMessage"] = feedbackMessage;
            }
            return RedirectToAction("AllTask");
        }
        [HttpGet]
        public async Task<ActionResult> AllTask()
        {
            TaskManagementEvolutionVM task = new TaskManagementEvolutionVM();
            long EmployeeId = Common.GetIntUserId();
            task = await Task.Run(() => taskManagementEvolutionService.GetAllTask(EmployeeId));
            ViewBag.EmployeeId = EmployeeId;
            return View(task);
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkSpace()
        {
            WorkSpaceVm workspase = new WorkSpaceVm();
            long EmployeeId = Common.GetIntUserId();
            workspase = await Task.Run(() => taskManagementEvolutionService.GetAllWorkspase(EmployeeId));
            ViewBag.EmployeeId = EmployeeId;
            return View(workspase);
        }
        [HttpPost]
        public async Task<ActionResult> CreateWorkspace(WorkSpaceVm Model)
        {
            if (Model.Title == null)
            {
                return RedirectToAction("GetWorkSpace");
            }
            else
            {
                var obj = await taskManagementEvolutionService.AddWorkspace(Model);
                return RedirectToAction("GetWorkSpace");
            }
          
          
        }
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var isDeleted = await taskManagementEvolutionService.DeleteWorkSpacee(id);
            if (isDeleted)
            {
                // Data deletion successful
                return Json(isDeleted);
            }
            else
            {
                // Data deletion failed
                return Json(isDeleted);
            }
        }
        [HttpGet]
        public async Task<ActionResult> AllTaskList(int? WorkbordId,int?WorkSpaceId)
        {
            TaskManagementEvolutionVM task = new TaskManagementEvolutionVM();
            long EmployeeId = Common.GetIntUserId();
            task = await Task.Run(() => taskManagementEvolutionService.GetAllTaskBywork(EmployeeId, WorkbordId));
            ViewBag.EmployeeId = EmployeeId;
            task.WorkboardId = WorkbordId;
            task.workspaceid = WorkSpaceId;
            return View(task);
        }

        [HttpPost]
        public ActionResult DeleteTask(int id)
        {
            int result = taskManagementEvolutionService.TaskDelete(id);
            if (result > 0)
            {
                TempData["FeedbackMessage"] = "Deleted Successfully";
                return Json(new { success = true });
            }
            else
            {
                TempData["FeedbackMessage"] = "Try again!";
                return Json(new { success = false });
            }
        }

        #endregion

        [HttpPost]
        public JsonResult TitleUpdate(string newTitle,long buttonValue)
        {
            var res= taskManagementEvolutionService.editModal(newTitle, buttonValue);

            return Json("res");
        }
        [HttpPost]
        public  JsonResult WatchButton( long WorkId)
        {
            var res = taskManagementEvolutionService.WatchModel(WorkId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task< JsonResult> GetByTaskAjax(long WorkId,int? workboardid)
        {
            var res = await taskManagementEvolutionService.GetAllTaskBYajax(WorkId, workboardid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WatchButtonSeenUnseen(long WorkId)
        {
            var res = taskManagementEvolutionService.WatchModelSeennUnseen(WorkId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult WorkAssign(long[] selectedInstallments,long wokkid)
        {
            var res = taskManagementEvolutionService.WorkAssignSer(selectedInstallments, wokkid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RemoveWorkAssignEmployee(int workMemberId)
        {
            var res = taskManagementEvolutionService.RemoveWorkAssign(workMemberId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangeState(int selectedValue,long workid)
        {
            var res = taskManagementEvolutionService.ChangeStaeValue(selectedValue, workid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangeWorkLabel(int selectedValue1, long workid)
        {
            var res = taskManagementEvolutionService.ChangeWorkLabelValue(selectedValue1, workid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdtaeDescription(string description, long workid)
        {
            var res = taskManagementEvolutionService.ChangeDescription(description, workid);
            return Json( JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CommentSave(string commen, long workid)
        {
            var res = taskManagementEvolutionService.CommentSave(commen, workid);
            return Json(res,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateComent(string newText, long workQAId)
        {
            var res = taskManagementEvolutionService.CommentUpdate(newText, workQAId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteComent( long workQAId)
        {
            var res = taskManagementEvolutionService.DeleteComent(workQAId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LikeUpdate(long WorkQid)
        {
            var res = taskManagementEvolutionService.LikeGive(WorkQid);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> LikePeople(long workQAId)
        {
            var res = await taskManagementEvolutionService.LikePeople(workQAId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> WorkBoard(long WorkSpaceID)
        { WorkBoardVm workBoardVm= new WorkBoardVm();
            workBoardVm = await taskManagementEvolutionService.GetWorkBoard(WorkSpaceID);
            workBoardVm.WorkSpacesId= WorkSpaceID;
            return View(workBoardVm);
        }
        [HttpPost]
        public async Task<ActionResult> CreateBoard(WorkBoard Model)
        {
            if (Model.BoardName == null)
            {
                return RedirectToAction("WorkBoard", new { WorkSpaceID = Model.WorkSpacesId });
            }
            else
            {
                var obj = await taskManagementEvolutionService.AddWorkBoard(Model);

                return RedirectToAction("WorkBoard", new { WorkSpaceID = Model.WorkSpacesId });
            }
          

        }
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmedWOrkbord(long id)
        {
            var isDeleted = await taskManagementEvolutionService.DeleteWorkBord(id);
            if (isDeleted)
            {
                // Data deletion successful
                return Json(isDeleted);
            }
            else
            {
                // Data deletion failed
                return Json(isDeleted);
            }
        }

        [HttpGet]
        public async Task<ActionResult> WorkSpaceMember(long workspaceId)
        {
            var obj = await taskManagementEvolutionService.GetWorkSpacemember(workspaceId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult WorkAssignForBoard(long[] selectedInstallments, long workspaceId)
        {
            var res = taskManagementEvolutionService.WorkAssignSerFoorBorad(selectedInstallments, workspaceId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteWorkspaceMember( long workspaceMemberId)
        {
            var res = taskManagementEvolutionService.DeleteWorkMember(workspaceMemberId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetSpaceWiseWorklebl(long workBoardId)
        { 
            var res = await taskManagementEvolutionService.GetWorkLebel(workBoardId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public async Task<ActionResult> SaveAttachmentFiles(IEnumerable<HttpPostedFileBase> files, long wrkid)
        //{
        //    var res= false;

        //    try
        //    {
        //        foreach (var file in files)
        //        {
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = Path.GetFileName(file.FileName);
        //                var path = Path.Combine(Server.MapPath("~/FileUpload/TaskAttatchment"), fileName);
        //                file.SaveAs(path);
        //               res=  await taskManagementEvolutionService.SaveAttachment(file, wrkid);
        //            }
        //        }
        //        return Json(res, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex) { 


        //    return Json(res, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public async Task<ActionResult> SaveAttachmentFiles(IEnumerable<HttpPostedFileBase> files, long wrkid)
        {
            var res = false;

            try
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var uploadFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileUpload", "TaskAttatchment");
                        var path = Path.Combine(uploadFolderPath, fileName);
                        //var path = Path.Combine(Server.MapPath("~/FileUpload/TaskAttatchment"), fileName);
                        file.SaveAs(path);
                        res = await taskManagementEvolutionService.SaveAttachment(file, wrkid);
                    }
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {


                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]

        public async Task<ActionResult> DeleteAttachment(long attachmentId)
        {
            var res=await taskManagementEvolutionService.Deleteattachment(attachmentId);
            return Json(res, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<ActionResult> DeleteWorkLabel(long labelId)
        {
            var res = await taskManagementEvolutionService.DeleteaWoklabel(labelId);
            return Json(res, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateEndDate(long workid, DateTime datePicker1)
        {


            var res =  taskManagementEvolutionService.UpdateEndDateee(workid, datePicker1);

            return Json(res, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult MakeAdmin(long WorkSpacesMemberId, bool isChecked, long workspaceid)
        {

            var res = taskManagementEvolutionService.AdminMake(WorkSpacesMemberId, isChecked, workspaceid); ;



            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
}