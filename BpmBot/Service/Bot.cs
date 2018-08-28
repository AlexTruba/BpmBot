using BpmBot.DB;
using BpmBot.Factory;
using BpmBot.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity = BpmBot.DB.Model;
using System.Runtime.ConstrainedExecution;

namespace BpmBot.Service
{
    class Bot : BaseApiService, IDisposable
    {
        private readonly BotContext _context;
        private readonly object _lockObject = new object();
        private int _lastUpdateId = 0;

        public Bot()
        {
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
            command.Execute(message);

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
