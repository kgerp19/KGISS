using KGERP.Service.ServiceModel;

namespace KGERP.Service.Interface
{
    public interface IMessageService
    {
        int SendMessage(MessageModel model);
    }
}
