using BpmBot.DB.Model;
using BpmBot.DB.Repository;
using BpmBot.Model;
using System.Threading.Tasks;

namespace BpmBot.Service.Command.Impl
{
    class RegCommand : BaseApiService, ICommand
    {
        private readonly ChatRepository _chatRepository;
        private readonly UserRepository _userRepository;
        private readonly ChatService _chatService;

        public RegCommand(ChatRepository chatRepository, UserRepository userRepository) 
            : base()
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _chatService = new ChatService(_chatRepository);
        }
        public async Task Execute(Message message)
        {
            await _chatService.AddChatAsync(message).ConfigureAwait(false);
        }

        private async Task RegisterInGame(Message message)
        {
            var fullName = $"{message.from.first_name} {message.from.last_name}";
            var checkUser = await _userRepository.GetUserByIdAndChatIdAsync(message.from.id, message.chat.id);
            if (checkUser != null)
            {

                var foundChat = await _chatRepository.GetByTelegramId(message.chat.id);
                
                User user = new User()
                {
                    TelegramId = message.from.id,
                    LastName = message.from.last_name,
                    FirstName = message.from.first_name,
                    Chat = foundChat
                };

                await _userRepository.AddAsync(user).ConfigureAwait(false);
                await _userRepository.SaveAsync().ConfigureAwait(false);

                string text = $"Поздравляю! Теперь {fullName} участвует в погоне за бонусами";
                _service.SendMessageAsync(message.chat.id, text);
            }
            else
            {
                string text = $"{fullName} узбагойся, дай другим отхватить кусочек бонусов!";
                _service.SendMessageAsync(message.chat.id, text);
            }
        }

    }
}
