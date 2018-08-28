using BpmBot.DB.Repository;
using BpmBot.Model;
using System.Threading.Tasks;
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
        public async Task AddChatAsync(Message message)
        {
            var foundChat = await _chatRepository.GetByTelegramId(message.chat.id);

            if (foundChat != null)
            {
                ChatDb newChat = new ChatDb
                {
                    TelegramId = message.chat.id,
                    Title = message.chat.title,
                    Type = message.chat.type
                };

                await _chatRepository.AddAsync(newChat);
                await _chatRepository.SaveAsync();
            }
        }
    }
}
