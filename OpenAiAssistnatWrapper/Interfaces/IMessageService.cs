using OpenAi_Assistant.Models;


namespace OpenAi_Assistant.Interfaces
{
    public interface IMessageService
    {

        Task<MessageModel> CreateMsg(string msg, string role, ThreadModel _currentThread);

    }
}
