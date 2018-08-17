using BpmBot.DB;
using BpmBot.Factory;
using BpmBot.Model;
using BpmBot.TelegramApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entity = BpmBot.DB.Model;

namespace BpmBot.Service
{
    class Bot : IDisposable
    {
        private readonly APIService _service;
        private readonly BotContext _context;
        private readonly object _lockObject = new object();
        private int _lastUpdateId = 0;

        public Bot()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            _service = new APIService(configuration["token"], configuration["url"]);
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
            var global = _context.Globals.FirstOrDefault();
            if (global != null)
            {
                _lastUpdateId = global.UpdateMessageId;
            }
        }

        public async Task Start()
        {
            Console.WriteLine("Старт метода - ");
            var response = await _service.GetUpdatesAsync();

            var updates = response.result.Where(t => t.message != null && _lastUpdateId < t.update_id);
            foreach (var item in updates)
            {
                Console.WriteLine($"Обработка сообщения - {item.message.message_id}");
                Console.WriteLine($"Номер обновления - {item.update_id}");
                if (_lastUpdateId < item.update_id)
                {
                    lock (_lockObject)
                    {
                        _lastUpdateId = item.update_id;
                        ChooseMethod(item.message);
                    }
                }
            }
        }
        private void ChooseMethod(Message message)
        {
            var command = CommandFactory.GetCommand(message);
            command.Execute(message.chat);
            
            if (message.text == "/reg" || message.text == "/reg@BlackTicketBot")
            {
                CommandFactory.GetCommand(message).Execute(message.chat);
                AddRegisterInGame(message.chat, message.from);
            }
            if (message.text == "/run" || message.text == "/run@BlackTicketBot")
            {
                RunGame(message.chat);
            }
            if (message.text == "/result" || message.text == "/result@BlackTicketBot")
            {
                GetResult(message.chat);
            }
        }

        private void GetResult(Chat chat)
        {
            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("@ChatId", chat.id);
            var result = _context.UserResults.FromSql("GetStatistic @ChatId", param).ToList();
            StringBuilder text = new StringBuilder();
            int userCount = result.Count;
            text.Append($"Результаты выдачи бонусов:{Environment.NewLine}");
            for (int i = 0; i < userCount; i++)
            {
                text.Append($"{i + 1}) Cотрудник {result[i].UserName} получил бонусы {result[i].Count} раз");
                if (i != (userCount - 1))
                {
                    text.Append(Environment.NewLine);
                }
            }
            foreach (var p in result)
                Console.WriteLine("{0} - {1}{2}", p.UserName.Trim(), p.Count, Environment.NewLine);

            _service.SendMessageAsync(chat.id, text.ToString());
        }

        private void AddedInChat(Chat temp)
        {
            if (_context.Chats.Where(t => t.TelegramId == temp.id).ToList().Count == 0)
            {
                Entity.Chat chat = new Entity.Chat() { TelegramId = temp.id, Title = temp.title, Type = temp.type };
                _context.Chats.Add(chat);
                _context.SaveChanges();
            }
        }
        private void AddRegisterInGame(Chat temp, From userFrom)
        {
            var fullName = userFrom.first_name + " " + userFrom.last_name ?? "";
            var checkUser = _context.Users.Include(p => p.Chat).Where(t => t.TelegramId == userFrom.id && t.Chat.TelegramId == temp.id).ToList();
            if (!checkUser.Any())
            {
                var chat = _context.Chats.Where(t => t.TelegramId == temp.id).ToList().FirstOrDefault();
                Entity.User user = new Entity.User()
                {
                    TelegramId = userFrom.id,
                    LastName = userFrom.last_name,
                    FirstName = userFrom.first_name,
                    Chat = chat
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                string text = $"Поздравляю! Теперь {fullName} участвует в погоне за бонусами";
                _service.SendMessageAsync(temp.id, text);
            }
            else
            {
                string text = $"{fullName} узбагойся, дай другим отхватить кусочек бонусов!";
                _service.SendMessageAsync(temp.id, text);
            }

        }
        private void RunGame(Chat chat)
        {
            var currDate = DateTime.Now.Date;
            var checkResultForDay = _context.Results.Include(p => p.Chat).Include(p => p.User).Where(t => t.Date.Date == currDate && t.Chat.TelegramId == chat.id).ToList();
            if (checkResultForDay.Count == 0)
            {
                var checkUser = _context.Users.Include(p => p.Chat).Count(t => t.Chat.TelegramId == chat.id);
                if (checkUser < 2)
                {
                    string textUser = $"Так дело не пойдет, нужно больше человек для участвия";
                    _service.SendMessageAsync(chat.id, textUser);
                }
                else
                {
                    var candidateUser = _context.Users.Include(p => p.Chat).Where(t => t.Chat.TelegramId == chat.id && t.IsActive).ToList();
                    var citationNumber = _context.Citations.GroupBy(t => t.Group).Count();
                    Random random = new Random();
                    int num = random.Next() % citationNumber;
                    var citation = _context.Citations.Where(t => t.Group == num).OrderBy(t => t.Order).ToList();
                    for (int i = 0; i < citation.Count - 1; i++)
                    {
                        _service.SendMessageAsync(chat.id, citation[i].Text);
                        Thread.Sleep(1000);
                    }
                    var userWin = candidateUser[random.Next() % candidateUser.Count];

                    string textUser = String.Format(citation.Last().Text, $"{userWin.FirstName} {userWin.LastName}", random.NextDouble() % 10000);
                    Entity.Result result = new Entity.Result()
                    {
                        Date = DateTime.Now,
                        Chat = userWin.Chat,
                        User = userWin
                    };
                    _context.Results.Add(result);
                    _context.SaveChanges();
                    _service.SendMessageAsync(chat.id, textUser);
                }
            }
            else
            {
                var winUser = checkResultForDay.FirstOrDefault().User;
                string text = $"Бонусы получил {winUser.FirstName} {winUser.LastName},но лавочка уже закрыта";
                if (new Random().Next() % 2 == 1)
                {
                    text = $"Поздравления от Олега - {winUser.FirstName} {winUser.LastName} хорошо идешь, курс SP в норме. Успехов!";
                }
                _service.SendMessageAsync(chat.id, text);
            }
        }

        public void Dispose()
        {
            var global = _context.Globals.FirstOrDefault();
            if (global != null)
            {
                global.UpdateMessageId = _lastUpdateId;
                _context.Globals.Attach(global);
                _context.SaveChanges();
            }
            else
            {
                var newGlobal = new Entity.Global() { UpdateMessageId = _lastUpdateId };
                _context.Globals.Add(newGlobal);
                _context.SaveChanges();
            }
            _context.Dispose();
        }
    }
}
