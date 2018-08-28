using BpmBot.DB.Repository;
using BpmBot.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ResultDB = BpmBot.DB.Model.Result;

namespace BpmBot.Service.Command.Impl
{
    class RunCommand : BaseApiService, ICommand
    {
        private readonly ResultRepository _resultRepository;
        private readonly ChatRepository _chatRepository;
        private readonly UserRepository _userRepository;
        private readonly CitationRepository _citationRepository;

        public RunCommand(
            ResultRepository resultRepository,
            ChatRepository chatRepository,
            UserRepository userRepository,
            CitationRepository citationRepository)
        {
            _resultRepository = resultRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _citationRepository = citationRepository;
        }
        public async Task Execute(Message message)
        {
            var todayResult = await _resultRepository.GetByUserIdForToday(message.chat.id);
            if (todayResult == null)
            {

                var userInChat = await _chatRepository.GetUsersCountInChat(message.chat.id);
                if (userInChat < 2)
                {
                    string textUser = "Так дело не пойдет, нужно больше человек для участвия";
                    _service.SendMessageAsync(message.chat.id, textUser);
                }
                else
                {
                    var candidateUser = await _userRepository.GetByChatIdRandom(message.chat.id);
                    var citation = await _citationRepository.GetRandomCitation();

                    foreach (var item in citation.SkipLast(1))
                    {
                        _service.SendMessageAsync(message.chat.id, citation.ElementAt(1).Text);
                        Thread.Sleep(1000);
                    }
                    
                    string textUser = String.Format(
                        citation.Last().Text, 
                        $"{candidateUser.FirstName} {candidateUser.LastName}",
                        new Random().NextDouble() % 10000);

                    var result = new ResultDB()
                    {
                        Date = DateTime.Now,
                        Chat = candidateUser.Chat,
                        User = candidateUser
                    };

                    await SaveResult(result);
                    _service.SendMessageAsync(message.chat.id, textUser);
                }
            }
            else
            {
                var winUser = todayResult.User;
                string text = $"Бонусы получил {winUser.FirstName} {winUser.LastName},но лавочка уже закрыта";
                if (new Random().Next() % 2 == 1)
                {
                    text = $"Поздравления от Олега - {winUser.FirstName} {winUser.LastName} хорошо идешь, курс SP в норме. Успехов!";
                }
                _service.SendMessageAsync(message.chat.id, text);
            }
        }

        private async Task SaveResult(ResultDB result)
        {
            await _resultRepository.AddAsync(result);
            await _resultRepository.SaveAsync();
        }
    }
}
