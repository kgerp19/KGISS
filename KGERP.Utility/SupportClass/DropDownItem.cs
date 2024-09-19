using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Utility.SupportClass
{
    public class DropDownItem
    {
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public decimal Custom6 { get; set; }
        public SelectListGroup Group { get; set; }
    }
}
