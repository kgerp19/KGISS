using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.TaskManagment
{
    public class TaskManagementEvolutionVM
    {    
        public long ManagerId { get; set; }
        [Required(ErrorMessage = "Reporting Manager is Required")]
        public string Manager { get; set; }
        [DisplayName("Task Title")]
        [Required(ErrorMessage = "Task title is Required")]
        public string TaskTitle { get; set; }
        [DisplayName("Task Description")]
        [Required(ErrorMessage = "Task Description is Required")]
        public string TaskDescription { get; set; }
        [DisplayName("Expected End Date")]
        [Required(ErrorMessage = "Expected End Date is Required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExpectedEndDate { get; set; }
        //public DateTime ExpectedEndDate { get; set; }
        public int TaskStatusID { get; set; }
        public int OrderBy { get; set; }
        public string FeedbackMessage { get; set; }
        public bool isOK { get; set; }
        public IEnumerable<TaskManagementEvolutionVM> DataList { get; set; }
        public string StatusName { get; set; }
        public string WoorklabelColor { get; set; }
        public long? ManagerIdr { get; set; }
        public long? assignmemberId { get; set; }
        public int? workspaceid { get; set; }
        public int? WorkboardId { get; set; }
       public List<WorkState> WorkStates { get; set; }
    
       public List<VmWorkAttacthment> WorkSAttachmentList{ get; set; }
      
      

        #region attributesFromWork
        public long WorkId { get; set; }
        public long? worklebelId { get; set; }
        public string WorkNo { get; set; }
        public string WorkLebelName { get; set; }
        public string Title { get; set; }
        public string Seen { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public List<SelectWorkLebel> WorkLebel { get; set; }
        public List<WorkmemeberVm> WorkmeberVm { get; set;}
        public List<WorkComentLikVm> workComentLikVm { get; set;}
        public List<SelectWorkState> WoorkStateVms { get; set;}

        public List<WorkComentVm> workComentVm { get; set;}
        public List<long> EmpIDlList { get; set;}
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }

        public long EmployeeId { get; set; }

        #endregion
    }

    public partial class WorkBoardVm
    {
        public long WorkBoardId { get; set; }
        public string BoardName { get; set; }
        public string WorkSpaceName { get; set; }
        public long WorkSpacesId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public List<WorkBoardVm> DataList { get; set; }
    }
    public partial class WorkSpacesMemberVM
    {
        public long WorkSpacesMemberId { get; set; }
        public long WorkSpacesId { get; set; }
        public long EmployeeId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public List<WorkSpacesMemberVM> DataList { get; set; }
        public string Name { get; set; }
        public string KgId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class WorkmemeberVm
    {
        public int WorkMemberId { get; set; }
        public long EmployeeId { get; set; }
        public long WorkId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
      public string  EmployeeName { get; set; }
      public string  EmployeeKgId { get; set; }
    }
    public class VmWorkAttacthment
    {
        public long AttatchmentId { get; set; }
        public Nullable<long> WorkId { get; set; }
        public string Path { get; set; }
        public string AttatchmentText { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Extension { get; set; }
    }

    public class WorkComentLikVm
    {
        public long WorkLikeId { get; set; }
        public Nullable<long> WorkQAId { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeKgId { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveLike { get; set; }
        public string Username { get; set; }
    }

    
    public class WorkSpaceVm
    {
            public long WorkSpacesId { get; set; }
            public string WorkNo { get; set; }
            public string Title { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public string CreatedBy { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public string ModifiedBy { get; set; }
            public bool IsActive { get; set; }

        public List<WorkSpaceVm> DataList { get; set; }
    }

    public class WoorkStateVm
    {
        public int WorkStateId { get; set; }
        public string State { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Work> Works { get; set; }
    }

    public class WorkComentVm
    {

        public long WorkQAId { get; set; }
        public Nullable<long> WorkId { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public string Comment { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeKgId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string SessionName { get; set; }
        public string UserName { get; set; }
        public bool isActiveLike { get; set; }
        public int LikeCount { get;set; }
    }


  
}
