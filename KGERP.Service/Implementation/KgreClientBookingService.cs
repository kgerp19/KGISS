using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class KgreClientBookingService : IKgreBookingService
    {
        public void SaveClientFinal(PlotBooking PaymentInfo, List<ClientsModel> ClientArray, ClientsInfo ClientBasicInfo, string EditClientAutoId, ClientsInfo obj)
        {
            ERPEntities db = new ERPEntities();
            DateTime dateTime;
            string InvoiceNo = ClientBasicInfo.ClientsAutoId;
            if (EditClientAutoId.Equals("2"))
            {
                ClientsInfo extUser = new ClientsInfo();
                extUser = (from i in db.ClientsInfoes
                           where i.ClientsAutoId == ClientBasicInfo.ClientsAutoId
                           select i).FirstOrDefault();

                extUser.ClientsAutoId = InvoiceNo;
                extUser.Cli_ProjectName = ClientBasicInfo.Cli_ProjectName;
                extUser.Cli_BlockNo = ClientBasicInfo.Cli_BlockNo;
                extUser.Cli_PlotNo = ClientBasicInfo.Cli_PlotNo;
                extUser.Cli_PlotSize = ClientBasicInfo.Cli_PlotSize;
                extUser.Cli_Facing = ClientBasicInfo.Cli_Facing;
                extUser.Cli_Date = ClientBasicInfo.Cli_Date;
                dateTime = Convert.ToDateTime(ClientBasicInfo.Cli_Date);

                (from p in db.PloatInfoSetups
                 where p.projectId == obj.Cli_ProjectName && p.BlockNo == obj.Cli_BlockNo && p.PloatNo == obj.Cli_PlotNo && p.PlotSize == obj.Cli_PlotSize && p.PlotFace == obj.Cli_Facing
                 select p).ToList().ForEach(x => x.ProjectBooking = 0);
                (from p in db.PloatInfoSetups
                 where p.projectId == ClientBasicInfo.Cli_ProjectName && p.BlockNo == ClientBasicInfo.Cli_BlockNo && p.PloatNo == ClientBasicInfo.Cli_PlotNo && p.PlotSize == ClientBasicInfo.Cli_PlotSize && p.PlotFace == ClientBasicInfo.Cli_Facing
                 select p).ToList().ForEach(x => x.ProjectBooking = 1);

                db.SaveChanges();


            }
            else
            {
                ClientsInfo clientInfoTwoModel = new ClientsInfo();
                clientInfoTwoModel.ClientsAutoId = InvoiceNo;
                clientInfoTwoModel.Cli_ProjectName = ClientBasicInfo.Cli_ProjectName;
                clientInfoTwoModel.Cli_BlockNo = ClientBasicInfo.Cli_BlockNo;
                clientInfoTwoModel.Cli_PlotNo = ClientBasicInfo.Cli_PlotNo;
                clientInfoTwoModel.Cli_PlotSize = ClientBasicInfo.Cli_PlotSize;
                clientInfoTwoModel.Cli_Facing = ClientBasicInfo.Cli_Facing;
                clientInfoTwoModel.Cli_Date = ClientBasicInfo.Cli_Date;
                dateTime = Convert.ToDateTime(ClientBasicInfo.Cli_Date);

                (
                    from p in db.PloatInfoSetups
                    where p.projectId == ClientBasicInfo.Cli_ProjectName && p.BlockNo == ClientBasicInfo.Cli_BlockNo && p.PloatNo == ClientBasicInfo.Cli_PlotNo && p.PlotSize == ClientBasicInfo.Cli_PlotSize && p.PlotFace == ClientBasicInfo.Cli_Facing
                    select p).ToList().ForEach(x => x.ProjectBooking = 1);

                db.ClientsInfoes.Add(clientInfoTwoModel);
                db.SaveChanges();
            }

            SavePaymentInfo(PaymentInfo, InvoiceNo, dateTime, EditClientAutoId);
            ClientDeteils(ClientArray, InvoiceNo, EditClientAutoId);
        }

        public void SavePaymentInfo(PlotBooking PaymentInfo, string InvoiceNo, DateTime PayDate, string EditClientAutoId)
        {

            ERPEntities db = new ERPEntities();
            if (EditClientAutoId.Equals("1"))
            {
                PlotBooking clientInfoTwoModel = new PlotBooking();
                clientInfoTwoModel.Booking_Date = PayDate;
                clientInfoTwoModel.ClientAutoId = InvoiceNo;
                clientInfoTwoModel.PlotSize = PaymentInfo.PlotSize;
                clientInfoTwoModel.LandPricePerKatha = PaymentInfo.LandPricePerKatha;
                clientInfoTwoModel.LandValue = PaymentInfo.LandValue;
                clientInfoTwoModel.Discount = PaymentInfo.Discount;
                clientInfoTwoModel.LandValueAfterDiscount = PaymentInfo.LandValueAfterDiscount;
                clientInfoTwoModel.AdditionalCost = PaymentInfo.AdditionalCost;
                clientInfoTwoModel.OtharCostName = PaymentInfo.OtharCostName;
                clientInfoTwoModel.UtilityCost = PaymentInfo.UtilityCost;
                clientInfoTwoModel.GrandTotal = PaymentInfo.GrandTotal;
                clientInfoTwoModel.BokkingMoney = PaymentInfo.BokkingMoney;
                clientInfoTwoModel.RestOfAmount = PaymentInfo.RestOfAmount;
                clientInfoTwoModel.OneTime = PaymentInfo.OneTime;
                clientInfoTwoModel.InstallMent = PaymentInfo.InstallMent;
                clientInfoTwoModel.NoOfInstallment = PaymentInfo.NoOfInstallment;
                clientInfoTwoModel.InstallMentAmount = PaymentInfo.InstallMentAmount;
                db.PlotBookings.Add(clientInfoTwoModel);
                db.SaveChanges();


            }
            else
            {

                (from p in db.PlotBookings
                 where p.ClientAutoId == InvoiceNo
                 select p).ToList().ForEach(x => x.NoOfInstallment = PaymentInfo.NoOfInstallment);
                (from p in db.PlotBookings
                 where p.ClientAutoId == InvoiceNo
                 select p).ToList().ForEach(x => x.InstallMentAmount = PaymentInfo.InstallMentAmount);
                db.SaveChanges();
            }

        }

        public void ClientDeteils(List<ClientsModel> ClientArray, string InvoiceNo, string EditClientAutoId)
        {
            ERPEntities db = new ERPEntities();
            if (EditClientAutoId.Equals("2"))
            {
                var all = db.ClientsInfo_Del.Where(x => x.ClientAutoId == InvoiceNo);
                db.ClientsInfo_Del.RemoveRange(all);
                //db.SaveChanges();


                foreach (var cli in ClientArray)
                {

                    ClientsInfo_Del aDel = new ClientsInfo_Del();
                    aDel.ClientAutoId = InvoiceNo;
                    aDel.NameOfApplicant = cli.ApplicantName;
                    aDel.FathersName = cli.FathersName;
                    aDel.MothersName = cli.MothersName;
                    aDel.SpousesName = cli.SpousesName;
                    aDel.PresentAddress = cli.PresentAddress;
                    aDel.PermanentAddress = cli.PermanentAddress;
                    aDel.Nationality = cli.Nationality;
                    aDel.DateOfBirrh = cli.BirthDate;
                    aDel.TelOffice = cli.TelOffice;
                    aDel.TelephoneNoRes = cli.TelNoRes;
                    aDel.Fax = cli.Fax;
                    aDel.Profession = cli.Profession;
                    aDel.OfficialAddress = cli.OfficeAddress;
                    aDel.NationalIdNo = cli.NationalId;
                    aDel.TinNo = cli.TinNo;
                    aDel.MobileNo = cli.MobileNo;
                    aDel.EmailAdderss = cli.Email;
                    aDel.PassportNo = cli.PassportNo;
                    aDel.Designation = cli.Designation;
                    aDel.BankDraftPayOrdChq = cli.BankDrafPayOrdchq;
                    aDel.RepresentativeName = cli.Representative;
                    aDel.Nominee_FullName = cli.nomFullName;
                    aDel.Nominee_FathersName = cli.nomFathers;
                    aDel.Nominee_MothersName = cli.nomMothers;
                    aDel.Nominee_perAdderss = cli.nomAddress;
                    aDel.ReletionwithApplicant = cli.nomRelation;
                    aDel.Nationlaty = cli.nomNationality;
                    aDel.Natioal_IdNo = cli.nomNationalId;
                    aDel.TleOrMobileNo = cli.nomTleMobNo;
                    aDel.Email = cli.nomEmail;
                    //aDel.ClientImageLink = cli.CliImgName;
                    //aDel.NomineeImageLink = cli.nomImgName;
                    db.ClientsInfo_Del.Add(aDel);
                    db.SaveChanges();

                }
            }
            else
            {

                foreach (var cli in ClientArray)
                {

                    ClientsInfo_Del aDel = new ClientsInfo_Del();
                    aDel.ClientAutoId = InvoiceNo;
                    aDel.NameOfApplicant = cli.ApplicantName;
                    aDel.FathersName = cli.FathersName;
                    aDel.MothersName = cli.MothersName;
                    aDel.SpousesName = cli.SpousesName;
                    aDel.PresentAddress = cli.PresentAddress;
                    aDel.PermanentAddress = cli.PermanentAddress;
                    aDel.Nationality = cli.Nationality;
                    aDel.DateOfBirrh = cli.BirthDate;
                    aDel.TelOffice = cli.TelOffice;
                    aDel.TelephoneNoRes = cli.TelNoRes;
                    aDel.Fax = cli.Fax;
                    aDel.Profession = cli.Profession;
                    aDel.OfficialAddress = cli.OfficeAddress;
                    aDel.NationalIdNo = cli.NationalId;
                    aDel.TinNo = cli.TinNo;
                    aDel.MobileNo = cli.MobileNo;
                    aDel.EmailAdderss = cli.Email;
                    aDel.PassportNo = cli.PassportNo;
                    aDel.Designation = cli.Designation;
                    aDel.BankDraftPayOrdChq = cli.BankDrafPayOrdchq;
                    aDel.RepresentativeName = cli.Representative;
                    aDel.Nominee_FullName = cli.nomFullName;
                    aDel.Nominee_FathersName = cli.nomFathers;
                    aDel.Nominee_MothersName = cli.nomMothers;
                    aDel.Nominee_perAdderss = cli.nomAddress;
                    aDel.ReletionwithApplicant = cli.nomRelation;
                    aDel.Nationlaty = cli.nomNationality;
                    aDel.Natioal_IdNo = cli.nomNationalId;
                    aDel.TleOrMobileNo = cli.nomTleMobNo;
                    aDel.Email = cli.nomEmail;
                    //aDel.ClientImageLink = cli.CliImgName;
                    //aDel.NomineeImageLink = cli.nomImgName;
                    db.ClientsInfo_Del.Add(aDel);
                    db.SaveChanges();

                }
            }

        }

        public object LodeploatbyId(int id)
        {


            ERPEntities db = new ERPEntities();

            var projectName = db.KGRECustomers.Where(x => x.ClientId == id).Select(x => x.Project).FirstOrDefault();
            var projectId = db.KGREProjects.Where(x => x.ProjectName == projectName).Select(x => x.ProjectId).FirstOrDefault();

            object BasicInfo = null;
            BasicInfo = (from basic in db.PloatInfoSetups
                         where basic.projectId == projectId

                         select new
                         {
                             basic.BlockNo


                         }).Distinct();


            return BasicInfo;


        }

        public object LodeploatbyId1(int ProjectId, string BlockNo)
        {

            ERPEntities db = new ERPEntities();
            //var projectName = db.KGRECustomers.Where(x => x.ClientId == ProjectId).Select(x => x.Project).FirstOrDefault();
            //var prId = db.KGREProjects.Where(x => x.ProjectName == projectName).Select(x => x.ProjectId).FirstOrDefault();

            object BasicInfo = null;
            BasicInfo = (from basic in db.PloatInfoSetups
                         where basic.projectId == ProjectId && basic.BlockNo == BlockNo && basic.ProjectBooking != 1

                         select new
                         {
                             basic.PloatNo

                         }).Distinct();


            return BasicInfo;


        }

        public object LodeploatbyId2(int ProjectId, string BlockNo, string PloatNo)
        {

            ERPEntities db = new ERPEntities();
            //var projectName = db.KGRECustomers.Where(x => x.ClientId == ProjectId).Select(x => x.Project).FirstOrDefault();
            //var prId = db.KGREProjects.Where(x => x.ProjectName == projectName).Select(x => x.ProjectId).FirstOrDefault();

            object BasicInfo = null;
            BasicInfo = (from basic in db.PloatInfoSetups
                         where basic.projectId == ProjectId && basic.BlockNo == BlockNo && basic.PloatNo == PloatNo && basic.ProjectBooking != 1

                         select new
                         {
                             basic.PlotSize

                         });


            return BasicInfo;


        }

        public object LodeploatbyId3(int ProjectId, string BlockNo, string PloatNo, string PloatSize)
        {

            ERPEntities db = new ERPEntities();
            //var projectName = db.KGRECustomers.Where(x => x.ClientId == ProjectId).Select(x => x.Project).FirstOrDefault();
            //var prId = db.KGREProjects.Where(x => x.ProjectName == projectName).Select(x => x.ProjectId).FirstOrDefault();

            object BasicInfo = null;
            BasicInfo = (from basic in db.PloatInfoSetups
                         where basic.projectId == ProjectId && basic.BlockNo == BlockNo && basic.PloatNo == PloatNo && basic.PlotSize == PloatSize && basic.ProjectBooking != 1

                         select new
                         {
                             basic.PlotFace

                         });


            return BasicInfo;


        }

        public object LodeprojectByProidandCliAutoid(int id)
        {
            ERPEntities db = new ERPEntities();

            var projectId = db.KGRECustomers.Where(x => x.ClientId == id).Select(x => x.ProjectId).FirstOrDefault();
            //var projectId = db.KGREProjects.Where(x => x.ProjectName == projectName).Select(x => x.ProjectId).FirstOrDefault();



            object BasicInfo = null;
            BasicInfo = (from basic in db.PloatInfoSetups
                         where basic.projectId == projectId

                         select new
                         {
                             basic.BlockNo,
                             basic.PloatNo,
                             basic.PlotSize,
                             basic.PlotFace

                         });


            return BasicInfo;


        }

        public object LoadClientBasicInfoById(string id)
        {

            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            //BasicInfo = (from basic in db.KGRECustomers
            //             join plo in db.tbl_ProjectNameSetup on basic.ProjectName equals plo.pro_id
            //             join payinfo in db.PlotBookings on basic.ClientAutoId equals payinfo.ClientAutoId
            //             where basic.ClientAutoId == id

            //             select new
            //             {

            //                 payinfo.LandPricePerKatha,
            //                 payinfo.PlotSize,
            //                 payinfo.LandValue,
            //                 payinfo.Discount,
            //                 payinfo.LandValueAfterDiscount,
            //                 payinfo.AdditionalCost,
            //                 payinfo.UtilityCost,
            //                 payinfo.GrandTotal,
            //                 payinfo.BokkingMoney,
            //                 payinfo.RestOfAmount,
            //                 payinfo.InstallMentAmount,
            //                 payinfo.InstallMent,
            //                 payinfo.NoOfInstallment,
            //                 basic.Basic_Info_id,
            //                 basic.Name,
            //                 plo.pro_id,
            //                 plo.ProName,
            //                 basic.FathersName,
            //                 basic.PresentAddress,
            //                 basic.EntryDate,
            //                 basic.MobileNo,
            //                 basic.Email,
            //                 basic.Profession,
            //                 basic.ProjectName,
            //                 basic.ClientAutoId

            //             });


            return BasicInfo;


        }

        public object LoadTableDataByid(int id)
        {

            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.KGRECustomers
                         join pros in db.KGREProjects on basic.ProjectId equals pros.ProjectId
                         where basic.ClientId == id

                         select new
                         {
                             basic.ProjectId,
                             basic.ClientId,
                             basic.FullName,
                             basic.PresentAddress,
                             basic.PermanentAddress,
                             basic.Nationality,
                             basic.DateofBirth,
                             basic.Project,
                             basic.NID,
                             basic.MobileNo,
                             basic.Email,
                             basic.Designation

                         });


            return BasicInfo;


        }

        public void PaymentInfos(PlotBooking PaymentInfo)
        {
            ERPEntities db = new ERPEntities();
            PlotBooking clientInfoTwoModel = new PlotBooking();
            clientInfoTwoModel.ClientAutoId = PaymentInfo.ClientAutoId;
            clientInfoTwoModel.RestOfAmount = PaymentInfo.RestOfAmount;
            clientInfoTwoModel.PayType = PaymentInfo.PayType;
            clientInfoTwoModel.BankName = PaymentInfo.BankName;
            clientInfoTwoModel.ChaqueNo = PaymentInfo.ChaqueNo;
            clientInfoTwoModel.BokkingMoney = PaymentInfo.BokkingMoney;
            clientInfoTwoModel.Booking_Date = PaymentInfo.Booking_Date;
            clientInfoTwoModel.InstallMentAmount = PaymentInfo.InstallMentAmount;
            clientInfoTwoModel.NoOfInstallment = PaymentInfo.NoOfInstallment;
            db.PlotBookings.Add(clientInfoTwoModel);
            db.SaveChanges();
        }
    }
}
