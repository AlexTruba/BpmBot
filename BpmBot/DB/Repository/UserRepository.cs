using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BpmBot.DB.Repository
{

    class UserRepository
    {
        private readonly BotContext _context;

        public UserRepository()
        {
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
        }

        public async Task<User> GetUserByIdAndChatIdAsync(int userId, int chatId)
        {
            return await _context
                .Set<User>()
                .AsNoTracking()
                .Include(_ => _.Chat)
                .SingleOrDefaultAsync(_ => _.TelegramId == userId && _.Chat.TelegramId == chatId);
        }

        public async Task<User> GetByChatIdRandom(int chatId)
        {
            return await _context
                .Set<User>()
                .Include(p => p.Chat)
                .Where(t => t.Chat.TelegramId == chatId && t.IsActive)
                .OrderBy(t => Guid.NewGuid())
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context
                .Set<User>()
                .AddAsync(user);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
