using KGERP.Data.Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.LcInfoServices
{
    public class ShereMemberModel
    {
        public int MemberID { get; set; }
        public string MemberNo { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string HusbandName { get; set; }
        public string MotherName { get; set; }
        public string CurrentAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Religion { get; set; }
        public string Division { get; set; }
        public string Diistrict { get; set; }
        public string Village { get; set; }
        public string PostOffice { get; set; }
        public string PoliceStation { get; set; }
        public string District { get; set; }
        public string Recommended { get; set; }
        public string AcccountNo { get; set; }
        public string ParmPhone { get; set; }
        public string Nationality { get; set; }
        public string Education { get; set; }
        public string Occupation { get; set; }
        public DateTime DOB { get; set; }
        public string MarritalStatus { get; set; }
        public int Children { get; set; }
        public string HeirName { get; set; }
        public string HeirAddress { get; set; }
        public double Age { get; set; }
        public string Relationship { get; set; }
        public int MemberStatus { get; set; }
        public string Share { get; set; }
        public string Seaco_HeadID { get; set; }
        public double OpenBalane { get; set; }
        public double? SecendVershon { get; set; }

        public IEnumerable<ShereMemberModel> DataList { get; set; }

    }
}