using System.Linq;
using BpmBot.Model;
using System.Threading.Tasks;
using BpmBot.DB.Repository;
using ChatDb = BpmBot.DB.Model.Chat;

namespace BpmBot.Service
{
    class ChatService
    {
        private readonly ChatRepository _chatRepository;
        public ChatService(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
        public async Task AddChatAsync(Chat chat)
        {
            var list = await _chatRepository.GetByTelegramId(chat.id);

            if (list.Any())
            {
                ChatDb newChat = new ChatDb
                {
                    TelegramId = chat.id,
                    Title = chat.title,
                    Type = chat.type
                };

                await _chatRepository.AddAsync(newChat);
                await _chatRepository.SaveAsync();
            }
        }
    }
}
