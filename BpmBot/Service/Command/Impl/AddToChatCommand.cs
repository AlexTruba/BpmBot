using System.Linq;
using BpmBot.DB.Repository;
using BpmBot.Model;

using System.Threading.Tasks;

namespace BpmBot.Service.Command.Impl
{
    public class AddToChatCommand : ICommand
    {
        private readonly ChatRepository _chatRepository;
        private readonly ChatService _chatService;
        public AddToChatCommand(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
            _chatService = new ChatService(_chatRepository);
        }
        public async Task Execute(Chat chat)
        {
            await _chatService.AddChatAsync(chat);
        }
    }
}
