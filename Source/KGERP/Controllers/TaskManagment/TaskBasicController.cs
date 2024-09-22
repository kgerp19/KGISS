using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace KGERP.Controllers.TaskManagment
{
    [CheckSession]
    public class TaskBasicController : Controller
    {
        private readonly TaskConfigurationService _service;
        public TaskBasicController(TaskConfigurationService service)
        {
            _service = service;
        }
        #region WorkState
        public async Task<ActionResult> WorkState(long? workboardid, long? WorkspaceId)
        {
            VMWorkState vMWorkState;
            vMWorkState = await Task.Run(() => _service.WorkStateGet(workboardid));
            vMWorkState.WokboardId = workboardid;
            vMWorkState.WorkspaceId = WorkspaceId;
            return View(vMWorkState);
        }
        [HttpPost]
        public async Task<ActionResult> WorkState(VMWorkState model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                await _service.WorkStateAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                await _service.WorkStateEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                await _service.WorkStateDelete(model.StateId);
            }
            else
            {
                return RedirectToAction("Error");
            }
            if (model.WokboardId > 0)
            {
                return RedirectToAction("WorkBoard", "TaskManagementEvolution", new { WorkSpaceID = model.WorkspaceId });

            }
            return RedirectToAction(nameof(WorkState));
        }
        #endregion
        #region WorkLabel
        public async Task<ActionResult> WorkLabel(long? workboardid,long?WorkspaceId)
        {
            VMWorkLabel vMWorkLabel;
            vMWorkLabel = await Task.Run(() => _service.WorkLabelGet(workboardid));
            vMWorkLabel.WokboardId = workboardid;
            vMWorkLabel.WorkspaceId= WorkspaceId;
            return View(vMWorkLabel);
        }
        [HttpPost]
        public async Task<ActionResult> WorkLabel(VMWorkLabel model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                await _service.WorkLabelAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                await _service.WorkLabelEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                await _service.WorkLabelDelete(model.WorkLabelId);
            }
            else
            {
                return RedirectToAction("Error");
            }
            if (model.WokboardId>0) {
                return RedirectToAction("WorkBoard", "TaskManagementEvolution", new { WorkSpaceID =model.WorkspaceId });

            }

            return RedirectToAction(nameof(WorkLabel));
        }
        #endregion

    }
}