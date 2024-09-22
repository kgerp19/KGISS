using System;

namespace KGERP.Data.CustomModel
{
    public class ClientsModel
    {
        public string ApplicantName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string SpousesName { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public DateTime BirthDate { get; set; }
        public string TinNo { get; set; }
        public string TelOffice { get; set; }
        public string MobileNo { get; set; }
        public string TelNoRes { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string PassportNo { get; set; }
        public string Profession { get; set; }
        public string Designation { get; set; }
        public string BankDrafPayOrdchq { get; set; }
        public string OfficeAddress { get; set; }
        public string Representative { get; set; }
        public string nomFullName { get; set; }
        public string nomFathers { get; set; }
        public string nomMothers { get; set; }
        public string nomAddress { get; set; }
        public string nomRelation { get; set; }
        public string nomNationality { get; set; }
        public string nomNationalId { get; set; }
        public string nomTleMobNo { get; set; }
        public string nomEmail { get; set; }

        public byte[] CliImg { get; set; }
        public string CliImgName { get; set; }

        public byte[] nomImg { get; set; }
        public string nomImgName { get; set; }
    }
}
