using KGERP.Data.CustomModel;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KGERP.Controllers
{
    [CheckSession]
    public class WorkController : BaseController
    {
        private readonly IWorkService workService;
        private readonly IWorkStateService workStateService;
        private readonly IWorkAssignService workAssignService;
        public WorkController(IWorkService workService, IWorkStateService workStateService, IWorkAssignService workAssignService)
        {
            this.workService = workService;
            this.workStateService = workStateService;
            this.workAssignService = workAssignService;
        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<WorkModel> works = workService.GetWorks(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = Page_No ?? 1;
            return View(works.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpGet]
        
        public ActionResult CreateOrEdit(int id)
        {
            WorkViewModel vm = new WorkViewModel
            {
                Work = workService.GetWork(id),
                ManagerWorkStates = workStateService.GetManagerWorkStateSelectModels()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit(WorkViewModel vm)
        {
            bool result = false;
            if (vm.Work.WorkId <= 0)
            {
                result = workService.SaveWork(0, vm.Work);
            }
            else
            {
                result = workService.SaveWork(vm.Work.WorkId, vm.Work);
            }

            if (result)
            {
                TempData["successMessage"] = "Task Saved Successfully !";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        
        public ActionResult WorkAssignIndex(int workId)
        {
            WorkAssignViewModel vm = new WorkAssignViewModel
            {
                WorkAssign = workService.GetWorkAssign(workId),
                WorkAssigns = workService.GetWorkAssigns(workId),
                AssignMembers = workService.GetAssignMemberSelectModels(workId)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult WorkAssign(WorkAssignViewModel vm)
        {
            workService.SaveWorkAssign(0, vm.WorkAssign);
            return RedirectToAction("WorkAssignIndex", new { workId = vm.WorkAssign.WorkId });
        }




        
        [HttpGet]
        public ActionResult MemberWorkIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<WorkAssignModel> workAssigns = workService.GetEmployeeWorks();
            int Size_Of_Page = 10;
            int No_Of_Page = Page_No ?? 1;
            return View(workAssigns.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpGet]
        
        public ActionResult ChangeMemberState(int id)
        {
            WorkViewModel vm = new WorkViewModel();
            //vm.Work= workService.GetWork(id);
            vm.WorkAssign = workAssignService.GetWorkAssign(id);
            vm.MemberWorkStates = workStateService.GetMemberWorkStateSelectModels(id);
            return View(vm);
        }

        [HttpPost]
        
        public ActionResult ChangeMemberState(WorkViewModel vm)
        {
            bool result = false;
            List<WorkAssignFileModel> workAssignFileModels = new List<WorkAssignFileModel>();

            foreach (HttpPostedFileBase file in vm.files)
            {
                if (file != null)
                {
                    WorkAssignFileModel workAssignFileModel = new WorkAssignFileModel();
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = fileName + "_" + vm.WorkAssign.WorkAssignId.ToString() + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                    file.SaveAs(Path.Combine(Server.MapPath("~/TaskManagement/"), fileName));

                    //assigning file uploaded status to ViewBag for showing message to user.  
                    ViewBag.UploadStatus = vm.files.Count().ToString() + " files uploaded successfully.";

                    workAssignFileModel.FileName = fileName;
                    workAssignFileModel.WorkAssignId = vm.WorkAssign.WorkAssignId;
                    workAssignFileModel.FileStatus = "New";
                    workAssignFileModels.Add(workAssignFileModel);
                }

            }
            result = workAssignService.SaveWorkAssignFileList(workAssignFileModels);

            result = workService.ChangeMemberState(vm.WorkAssign);

            if (result)
            {
                TempData["successMessage"] = "Member Status Changed Successfully !";
                return RedirectToAction("MemberWorkIndex");
            }
            return View();
        }
        
        [HttpGet]
        public ActionResult WorkOverview(string searchText)
        {
            searchText = searchText ?? "";
            List<WorkCustomModel> managerWorks = workService.GetManagerWorks().Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.Designation.ToLower().Contains(searchText.ToLower()) || x.Department.ToLower().Contains(searchText.ToLower())).OrderBy(x => x.ManagerId).ThenByDescending(x => x.WorkNo).ThenBy(x => x.OrderNo).ThenBy(x => x.EntryDate).ToList();

            List<WorkCustomModel> newList = new List<WorkCustomModel>();
            foreach (var item in managerWorks)
            {
                WorkCustomModel m = new WorkCustomModel();
                m.ManagerId = item.ManagerId;
                m.Name = item.Name;
                if (item.ImageUrl == null)
                {
                    item.ImageUrl = "default.png";
                }
                m.ImageUrl = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Picture/" + item.ImageUrl;
                m.Department = item.Department;
                m.Designation = item.Designation;
                m.WorkNo = item.WorkNo;
                m.WorkTopic = item.WorkTopic;
                m.EntryDate = item.EntryDate;
                m.ExpectedEndDate = item.ExpectedEndDate;
                m.Remarks = item.Remarks;
                m.WorkState = item.WorkState;

                newList.Add(m);
            }
            var result = newList.GroupBy(x => new { x.ManagerId, x.Department, x.Designation });
            return View(result);
        }
        [HttpGet]
        
        public ActionResult DeleteWork(int id)
        {
            bool result = workService.DeleteWork(id);
            if (result)
            {
                TempData["successMessage"] = "Deleted Successfully !";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        
        public ActionResult DeleteMember(int workId, int workAssignId)
        {
            bool result = workService.DeleteMember(workAssignId);
            return RedirectToAction("WorkAssignIndex", new { workId });
        }

        
        [HttpGet]
        public ActionResult FileIndex(int workId, long workAssignId)
        {
            List<WorkAssignFileModel> files = workAssignService.GetFiles(workAssignId);
            return View(files);
        }

        public FileResult Download(long workAssingFileId)
        {
            string fileName = workAssignService.GetFileName(workAssingFileId);
            string path = Server.MapPath("~/TaskManagement/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        
        [HttpGet]
        public ActionResult WorkMemberIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<WorkMemberModel> models = workService.GetWorkMembers(searchText);
            List<WorkMemberModel> workMembersWithImage = models.Select(x => new WorkMemberModel
            {
                WorkMemberId = x.WorkMemberId,
                MemberId = x.MemberId,
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                Department = x.Department,
                Designation = x.Designation,
                ImageUrl = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Picture/" + x.ImageUrl,
            }).ToList();
            int Size_Of_Page = 10;
            int No_Of_Page = Page_No ?? 1;
            return View(workMembersWithImage.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpGet]
        
        public ActionResult WorkMemberCreate(int id)
        {
            WorkMemberModel model = workService.GetWorkMember(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult WorkMemberCreate(WorkMemberModel model)
        {
            bool result = false;
            result = workService.SaveWorkMember(0, model);
            if (result)
            {
                TempData["successMessage"] = "Member Saved Successfully !";
                return RedirectToAction("WorkMemberIndex");
            }
            return View();
        }

        [HttpGet]
        
        public ActionResult DeleteWorkMember(int id)
        {
            bool result = workService.DeleteWorkMember(id);
            return RedirectToAction("WorkMemberIndex");
        }
    }
}