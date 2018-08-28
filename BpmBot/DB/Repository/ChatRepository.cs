using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Chat> GetByTelegramId(int id)
        {
            return await _context
                .Set<Chat>()
                .Where(t => t.TelegramId == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
        public async Task<int> GetUsersCountInChat(int chatId)
        {
            return await _context
                .Set<Chat>()
                .AsNoTracking()
                .Include(p => p.Users)
                .CountAsync(t => t.TelegramId == chatId)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(Chat chat)
        {
            await _context
                .Set<Chat>()
                .AddAsync(chat)
                .ConfigureAwait(false);
        }

        public async Task SaveAsync()
        {
            await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}
