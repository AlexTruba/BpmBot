using System.Threading.Tasks;
using BpmBot.Model;

namespace BpmBot.Service.Command
{
    public interface ICommand
    {
        Task Execute(Message message);
    }
}
