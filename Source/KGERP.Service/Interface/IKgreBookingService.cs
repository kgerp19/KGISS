using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IKgreBookingService
    {
        void SaveClientFinal(PlotBooking PaymentInfo, List<ClientsModel> ClientArray, ClientsInfo ClientBasicInfo, string EditClientAutoId, ClientsInfo obj);
        void SavePaymentInfo(PlotBooking PaymentInfo, string InvoiceNo, DateTime PayDate, string EditClientAutoId);
        void ClientDeteils(List<ClientsModel> ClientArray, string InvoiceNo, string EditClientAutoId);
        object LodeploatbyId(int id);
        object LodeploatbyId1(int ProjectId, string BlockNo);
        object LodeploatbyId2(int ProjectId, string BlockNo, string PloatNo);
        object LodeploatbyId3(int ProjectId, string BlockNo, string PloatNo, string PloatSize);
        object LodeprojectByProidandCliAutoid(int id);
        object LoadClientBasicInfoById(string id);
        object LoadTableDataByid(int id);
        void PaymentInfos(PlotBooking PaymentInfo);

    }
}
