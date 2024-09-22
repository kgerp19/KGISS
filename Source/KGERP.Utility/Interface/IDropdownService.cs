using KGERP.Utility.SupportClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Utility.Interface
{
    public interface IDropdownService
    {
        List<SelectListItem> DefaultDDL();
        List<SelectModel> DefaultDDLSM();
        List<DropDownItem> DefaultCustomDDL();
        List<SelectListItem> DefaultDDL(string Disable);
        List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem = false);
        List<SelectModel> RenderDDLSM(List<SelectModel> listItems, bool? IsIncludeDefaultItem = false);
        List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem, string DisableItem);

        List<DropDownItem> RenderCustomDDL(List<DropDownItem> listItems, bool? IsIncludeDefaultItem);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listItems">List<SelectListItem></param>
        /// <param name="IsIncludeDefaultItem">Null,True Or False</param>
        /// <param name="DisableItem">Null or Specific Id (string Type)</param>
        /// <param name="DefaultSelected">Selected Id (String Type)</param>
        /// <returns></returns>
        List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem, string DisableItem, string DefaultSelected);
        List<SelectListItem> BooleanDDL(bool? IsIncludeDefaultItem = false, bool? defaultSelected = true);
        List<SelectListItem> YesOrNoDDL(bool? IsIncludeDefaultItem = false, bool? defaultSelected = true);
        List<SelectListItem> MatalFinishDDL(bool? IsIncludeDefaultItem = false);
        List<SelectListItem> BoleanYesOrNoDDL(bool? IsIncludeDefaultItem = false);

        List<SelectListItem> YesNoDDL(bool? IsIncludeDefaultItem = false, int? defaultSelected = 0, string Text0 = "", string Text1 = "");
        //List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0);
        //List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0, bool includeHalf = false);
        //List<SelectListItem> NumberDDL(int min, int max, string extraText, bool IsIncludeDefaultItem, int? defaultSelected = 0);
        List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0);
        List<SelectListItem> ChqNumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0);
        List<SelectListItem> NumberDDL(int min, int max, string extraText, bool IsIncludeDefaultItem, int? defaultSelected = 0);
        //List<SelectListItem> NumberDDL(int min, int max, string extraText, bool IsIncludeDefaultItem, int? defaultSelected = 0, bool includeHalf = false);
        List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, bool includeHalf, int? defaultSelected = 0);

        List<SelectListItem> DDLKG_CM(bool? IsIncludeDefaultItem = false, int? defaultSelected = 6);
        List<SelectListItem> DDLGetPaymentType();
        List<SelectListItem> DDLMonth(int Selected = 0, bool IsIncludeDefaultItem = true, int MonthFrom = 1, int MonthTo = 12);
        
        List<SelectListItem> DDLGender(bool IsIncludeDefaultItem = true, string Value1 = "M", string Value2 = "F");
    }
}
