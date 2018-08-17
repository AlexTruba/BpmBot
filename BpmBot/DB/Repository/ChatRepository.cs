using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BpmBot.DB.Repository
{
    public class ChatRepository
    {
        private readonly BotContext _context;
        public ChatRepository()
        {
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
        }

        public async Task<IEnumerable<Chat>> GetByTelegramId(int id)
        {
            return await _context
                .Set<Chat>()
                .Where(t => t.TelegramId == id)
                .ToListAsync();
        }
        public async Task AddAsync(Chat chat)
        {
            await _context
                .Set<Chat>()
                .AddAsync(chat);
        }

        public async Task SaveAsync()
        {
            await _context
                .SaveChangesAsync();
        }
    }
}
