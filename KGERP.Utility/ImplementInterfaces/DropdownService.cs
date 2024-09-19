using KGERP.Utility.Interface;
using KGERP.Utility.SupportClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Utility.ImplementInterfaces
{
    public class DropdownService: IDropdownService
    {
        public List<SelectListItem> DefaultDDL()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Please Select", Value = "" });
            return list;
        }

        public List<SelectModel> DefaultDDLSM()
        {
            var list = new List<SelectModel>
            {
                new SelectModel() { Text = "Please Select", Value = "" }
            };
            return list;
        }
        public List<DropDownItem> DefaultCustomDDL()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Text = "Please Select", Value = "" });
            return list;
        }
        public List<SelectListItem> DefaultDDL(string Disable)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Please Select", Value = "", Disabled = Disable == "" ? true : false });
            return list;
        }

        public List<SelectListItem> BooleanDDL(bool? IsIncludeDefaultItem = false, bool? defaultSelected = true)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.Add(new SelectListItem() { Text = "True", Value = "true", Selected = true == defaultSelected ? true : false });
            list.Add(new SelectListItem() { Text = "False", Value = "false", Selected = false == defaultSelected ? true : false });
            return list;
        }
        public List<SelectListItem> MatalFinishDDL(bool? IsIncludeDefaultItem = false)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.Add(new SelectListItem() { Text = "Metalic", Value = "true" });
            list.Add(new SelectListItem() { Text = "Paint ", Value = "false" });
            return list;
        }

        public List<SelectListItem> YesOrNoDDL(bool? IsIncludeDefaultItem = false, bool? defaultSelected = true)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.Add(new SelectListItem() { Text = "Yes", Value = "true", Selected = true == defaultSelected ? true : false });
            list.Add(new SelectListItem() { Text = "No", Value = "false", Selected = false == defaultSelected ? true : false });
            return list;
        }
        public List<SelectListItem> BoleanYesOrNoDDL(bool? IsIncludeDefaultItem = false)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.Add(new SelectListItem() { Text = "Yes", Value = "true" });
            list.Add(new SelectListItem() { Text = "No ", Value = "false" });
            return list;
        }
        public List<SelectListItem> YesNoDDL(bool? IsIncludeDefaultItem = false, int? defaultSelected = 0, string Text0 = "No", string Text1 = "Yes")
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.Add(new SelectListItem() { Text = Text1, Value = "1", Selected = 1 == defaultSelected.Value ? true : false });
            list.Add(new SelectListItem() { Text = Text0, Value = "0", Selected = 0 == defaultSelected.Value ? true : false });


            return list;
        }
        public List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            for (; min <= max; min++)
            {
                list.Add(new SelectListItem() { Text = min.ToString(), Value = min.ToString(), Selected = min == defaultSelected ? true : false });
            }

            return list;
        }


        public List<SelectListItem> ChqNumberDDL(int min, int max, bool IsIncludeDefaultItem, int? defaultSelected = 0)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            for (; min <= max; min += 10)
            {
                list.Add(new SelectListItem() { Text = min.ToString(), Value = min.ToString(), Selected = min == defaultSelected.Value ? true : false });
            }

            return list;
        }

        public List<SelectListItem> NumberDDL(int min, int max, string extraText, bool IsIncludeDefaultItem, int? defaultSelected = 0)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            for (; min <= max; min++)
            {
                list.Add(new SelectListItem() { Text = min.ToString() + extraText, Value = min.ToString(), Selected = min == defaultSelected ? true : false });
            }

            return list;
        }
        public List<SelectListItem> NumberDDL(int min, int max, bool IsIncludeDefaultItem, bool includeHalf, int? defaultSelected = 0)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            for (; min <= max; min++)
            {
                list.Add(new SelectListItem() { Text = min.ToString(), Value = min.ToString(), Selected = min == defaultSelected ? true : false });
                if (includeHalf)
                {
                    list.Add(new SelectListItem() { Text = (min + 0.5).ToString(), Value = (min + 0.5).ToString(), Selected = (min + 0.5) == defaultSelected ? true : false });
                }
            }

            return list;
        }

        public List<SelectListItem> RenderDDL(List<SelectListItem> listItems)
        {
            var list = DefaultDDL();
            list.AddRange(listItems);
            return list;
        }

        public List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem = false)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            list.AddRange(listItems);
            return list;
        }

        public List<DropDownItem> RenderCustomDDL(List<DropDownItem> listItems, bool? IsIncludeDefaultItem = false)
        {
            var list = new List<DropDownItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultCustomDDL();
            }
            list.AddRange(listItems);
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listItems"></param>
        /// <param name="IsIncludeDefaultItem"></param>
        /// <param name="DisableItem"> DisableItem=""</param>
        /// <returns></returns>
        public List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem, string DisableItem)
        {
            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL(DisableItem);
            }
            list.AddRange(listItems);
            return list;
        }

        public List<SelectListItem> DDLKG_CM(bool? IsIncludeDefaultItem = false, int? defaultSelected = 6)
        {

            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            //DB UNIT ID for KG
            list.Add(new SelectListItem() { Text = "KG", Value = "6", Selected = 6 == defaultSelected ? true : false });
            /// DB UNIT ID FOR CM
            list.Add(new SelectListItem() { Text = "Centimeter", Value = "2", Selected = 2 == defaultSelected ? true : false });
            return list;
        }


        public List<SelectListItem> RenderDDL(List<SelectListItem> listItems, bool? IsIncludeDefaultItem, string DisableItem, string DefaultSelected)
        {
            var list = this.RenderDDL(listItems, IsIncludeDefaultItem, DisableItem);
            foreach (var d in list)
            {
                if (d.Text == DefaultSelected)
                {
                    d.Selected = true;
                }
            }
            return list;
        }

        public List<SelectListItem> DDLMonth(int Selected = 0, bool IsIncludeDefaultItem = true, int MonthFrom = 1, int MonthTo = 12)
        {
            MonthFrom = (MonthFrom <= 0 || MonthFrom > MonthTo || MonthFrom > 12) ? 1 : MonthFrom;
            MonthTo = (MonthTo < MonthFrom || MonthTo > 12) ? 12 : MonthTo;

            var list = new List<SelectListItem>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDL();
            }
            if (Selected < 1 && IsIncludeDefaultItem)
            {
                list[0].Selected = true;
            }
            for (; MonthFrom <= MonthTo; MonthFrom++)
            {
                list.Add(new SelectListItem()
                {
                    Value = MonthFrom.ToString(),
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthFrom),
                    Selected = MonthFrom == Selected
                });
            }


            return list;
        }


        #region Finance
        public List<SelectListItem> DDLGetPaymentType()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Direct Payment", Value = "0" });
            list.Add(new SelectListItem() { Text = "Advance Payment Against Purchases", Value = "1" });
            list.Add(new SelectListItem() { Text = "Bill to Bill Payment", Value = "2" });
            list.Add(new SelectListItem() { Text = "Advance Payment Against Services", Value = "3" });
            return list;
        }

        public List<SelectListItem> DDLGender(bool IsIncludeDefaultItem = true, string Value1 = "M", string Value2 = "F")
        {
            var list = DefaultDDL();
            list.Add(new SelectListItem() { Text = "Male", Value = Value1 });
            list.Add(new SelectListItem() { Text = "Female", Value = Value2 });
            return list;

        }

        

        

        public List<SelectModel> RenderDDLSM(List<SelectModel> listItems, bool? IsIncludeDefaultItem = false)
        {
            var list = new List<SelectModel>();
            if (IsIncludeDefaultItem == true)
            {
                list = DefaultDDLSM();
            }
            list.AddRange(listItems);
            return list;
        }
        #endregion
    }
}
