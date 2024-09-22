using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
  

    public class ExitInterviewVM
    {
        public long Id { get; set; }

        public ActionEnum ActionType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }
        public string JoiningDateString 
        {
            get {
                return JoiningDate.ToString("dd-MMM-yyyy");                    
               }
        }

        public DateTime CreatedDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string ResignDateString 
        {

            get
            {
                string rdstring = "";
                if (ResignDate.HasValue)
                {
                    rdstring = ResignDate.Value.ToString("dd-MMM-yyyy");
                }
                return rdstring;
            }
        }
        //public string ServiceLength { get; set; }

        public ApprovalStatusEnum Status { get; set; }

        public string ServiceLength
        {
            get
            {
                string result = "";
                TimeSpan dateDifference = (ResignDate.HasValue ? ResignDate.Value : DateTime.Today)-JoiningDate.AddDays(-1);

                int years = (int)(dateDifference.TotalDays / 365.25); // Using 365.25 for a more accurate year calculation
                int months = (int)((dateDifference.TotalDays % 365.25) / 30.44); // Using 30.44 for an average month length
                int days = (int)(dateDifference.TotalDays % 30.44);


                result = $"{years:D2} Years {months:D2} Months {days:D2} Days";
                return result;
            }
        }

        public string ReasonForLeaving { get; set; }

        public bool IsAcceptedAnotherPosition { get; set; }
        public string WhatPromptedToSeekAnotherJOb { get; set; }
        public string WhenBeginSearchingAnotherJob { get; set; }
        public string WhatMakeNewJobMoreAttractive { get; set; }
        public string WhatMadeCareerGoalsBetter { get; set; }
        public string HaveUShereUrGoal { get; set; }
        public bool IsAdequateCareerOpportunitiesAvailableHere { get; set; }
        public CareerOpportunity CareerOpportunities { get; set; }


        public int RatingJobResponsibility { get; set; }
        public int RatingAchievingGoal { get; set; }
        public int RatingWorkEnvironment { get; set; }
        public int RatingDirectorOrManager { get; set; }
        public int RatingPay { get; set; }
        public int RatingBenefits { get; set; }

       // [DisplayName("")]
        public string WhatDidEnjoyMostAboutYourJob { get; set; }
        public string WhatDidEnjoyLeastAboutYourJob { get; set; }
        public string WhatMakesKrishibidGroupGoodPlaceToWork { get; set; }
        public string WhatMakesKrishibidGroupPoorPlaceToWork { get; set; }
        public string WhatRecommendationToMakingBetterToWork { get; set; }

        public bool WouldHaveUStatyedIfMoreStatisfactoryArrangementWorkedOUt { get; set; }



        public SignatoryStatusEnum SignatoryStatus { get; set; }


    }

    public class CareerOpportunity
    {
        public bool PromotionalOpportunities { get; set; }
        public bool PositionRotations { get; set; }
        public bool IncreasedResponsibilities { get; set; }
        public bool SpecialProjects { get; set; }
        public bool Overseas { get; set; }
        public bool NoProgression { get; set; }
        public bool Other { get; set; }
    }


    public class ExitInterviewModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string ReasonForLeaving { get; set; }
        public bool IsAcceptedAnotherPosition { get; set; }
        public string MotivationForJobChanges { get; set; }
        public string JobSearchStart { get; set; }
        public string AttractiveFactors { get; set; }
        public string CareerGoalsElseWhere { get; set; }
        public string CareerGoalDiscussion { get; set; }
        public bool IsCareerOpportunitiesSatisfaction { get; set; }
        public bool IsPromotionalOpportunities { get; set; }
        public bool IsPositionRotations { get; set; }
        public bool IsIncreasedResponsibilities { get; set; }
        public bool IsSpecialProjects { get; set; }
        public bool IsOverseas { get; set; }
        public bool IsNoProgression { get; set; }
        public bool IsOther { get; set; }
        public int RatingJobResponsibilities { get; set; }
        public int RatingGoalOpportunities { get; set; }
        public int RatingWorkEnvironment { get; set; }
        public int RatingManager { get; set; }
        public int RatingPay { get; set; }
        public int RatingBenefits { get; set; }
        public string JobEnjoyment { get; set; }
        public string JobChallenges { get; set; }
        public string PositiveAspectsOfWorkPlace { get; set; }
        public string NegativeAspectsOfWorkPlace { get; set; }
        public string WorkplaceImprovementSuggestions { get; set; }
        public bool IsConsideredStaying { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
