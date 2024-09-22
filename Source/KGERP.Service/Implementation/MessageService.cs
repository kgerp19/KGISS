using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly ERPEntities context;
        public MessageService(ERPEntities context)
        {
            this.context = context;
        }

        public int SendMessage(MessageModel model)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KMCSLConnectionString"].ConnectionString;
            List<MessageModel> messageModels = new List<MessageModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetKMCSLCustomers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    MessageModel message = new MessageModel();
                    message.CompanyId = model.CompanyId;
                    message.sms_uid = rdr["memberID"].ToString();
                    message.ReceiverName = rdr["name"].ToString();
                    message.ReceiverMobile = rdr["phone"].ToString();
                    message.SendDate = DateTime.Now;
                    message.request_type = "N/A";
                    message.MessageBody = "Test KMCSL";

                    messageModels.Add(message);
                }
                con.Close();
            }
            return messageModels.Count();

        }
    }
}
