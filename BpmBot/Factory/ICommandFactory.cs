using BpmBot.Model;
using BpmBot.Service.Command;

namespace BpmBot.Factory
{
    public interface ICommandFactory
    {
       ICommand GetCommand(Message message);
    }
}
