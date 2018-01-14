using BpmBot.DB;
using BpmBot.Model;
using BpmBot.TelegramApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Entity = BpmBot.DB.Model;
namespace BpmBot.Service
{
    class Bot: IDisposable
    {
        private readonly APIService _service;
        private readonly IConfigurationRoot _configuration;
        private readonly BotContext _context;
        private readonly List<string> _citation = new List<string>
        {
            "И кто это здесь бонусов захотел?",
            "Ладно уже, придумываю новую систему начисления",
            "Хм, мне подсказали, что нужен ведущий разработчик",
            "Инициирую поиск разработчика дня...",
            "Бизнес-процесс запущен, завизированные навыки проверены",
            "Провожу опрос руководителя проекта..."
        };

        private int _lastUpdateId = 0;

        public Bot()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
            _service = new APIService(_configuration["token"], _configuration["url"]);
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
        }
        public void Start()
        {
            var response = _service.GetUpdatesAsync().Result.result.Where(t => t.message != null);
            foreach (var item in response)
            {
                if (_lastUpdateId < item.update_id)
                {
                    ChooseMethod(item.message);
                    _lastUpdateId = item.update_id;
                }
            }
        }
        private void ChooseMethod(Message message)
        {
            if (message.new_chat_member != null && message.new_chat_member.is_bot)
            {
                AddedInChat(message.chat);
            }
            if (message.text == "/reg" || message.text == "/reg@BlackTicketBot")
            {
                AddRegisterInGame(message.chat.id, message.from);
            }
            if (message.text == "/run" || message.text == "/run@BlackTicketBot")
            {
                RunGame(message.chat);
            }
        }
        private void AddedInChat(Chat temp)
        {
            if (_context.Chats.Where(t=>t.TelegramId == temp.id).ToList().Count == 0)
            {
                Entity.Chat chat = new Entity.Chat() { TelegramId = temp.id, Title = temp.title, Type = temp.type };
                _context.Chats.Add(chat);
                _context.SaveChanges();
            }
        }
        private void AddRegisterInGame(int chatId, From userFrom)
        {
            var fullName = userFrom.first_name + userFrom.last_name ?? "";
            var checkUser = _context.Users.Include(p=>p.Chat).Where(t => t.TelegramId == userFrom.id && t.Chat.TelegramId == chatId).ToList();
            if (checkUser.Count()==0)
            {
                Entity.User user = new Entity.User() {
                    TelegramId = userFrom.id,
                    LastName = userFrom.last_name,
                    FirstName = userFrom.first_name,
                    Chat = new Entity.Chat() { TelegramId = chatId }
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                string text = $"Поздравляю! Теперь {fullName} участвует в погоне за бонусами";
                _service.SendMessageAsync(chatId, text);
            }
            else
            {
                string text = $"{fullName} узбагойся, дай другим отхватить кусочек бонусов!";
                _service.SendMessageAsync(chatId, text);
            }
          
        }
        private void RunGame(Chat chat)
        {
            var currDate = DateTime.Now.Date;
            var checkResultForDay = _context.Results.Include(p => p.Chat).Include(p => p.User).Where(t => t.Date.Date  == currDate && t.Chat.TelegramId == chat.id).ToList();
            if (checkResultForDay.Count==0)
            {
                var checkUser = _context.Users.Include(p => p.Chat).Where(t => t.Chat.TelegramId == chat.id).Count();
                if (checkUser < 2)
                {
                    string textUser = $"Так дело не пойдет, нужно больше человек для участвия";
                    _service.SendMessageAsync(chat.id, textUser);
                }
                else
                {
                    var candidateUser = _context.Users.Include(p => p.Chat).Where(t => t.Chat.TelegramId == chat.id).ToList();
                    foreach (var item in _citation)
                    {
                        _service.SendMessageAsync(chat.id, item);
                        Thread.Sleep(200);
                    }
                    Random random = new Random();
                    var userWin = candidateUser[random.Next() % candidateUser.Count];
                    string textUser = $"Мои поздравления! Сегодня ведущим разраб становиться - {userWin.FirstName} {userWin.LastName}. Бонусы сможет получить после сдачи проекта.";
                    Entity.Result result = new Entity.Result()
                    {
                        Date = DateTime.Now,
                        Chat = new Entity.Chat() { TelegramId = chat.id },
                        User = new Entity.User() {TelegramId = userWin.TelegramId }
                    };
                    _context.Results.Add(result);
                    _context.SaveChanges();
                    _service.SendMessageAsync(chat.id, textUser);
                }
            }
            else
            {
                var winUser = checkResultForDay.FirstOrDefault().User;
                string text = $"Бонусы получил {winUser.FirstName} {winUser.LastName},на лавочка закрыта";
                _service.SendMessageAsync(chat.id, text);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
