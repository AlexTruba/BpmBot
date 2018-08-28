using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpmBot.DB.Repository
{
    class ResultRepository
    {
        private readonly BotContext _context;

        public ResultRepository()
        {
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
        }

        public async Task<Result> GetByUserIdForToday(int id)
        {
            return await _context
                 .Set<Result>()
                 .AsNoTracking()
                 .Include(p => p.Chat)
                 .Include(p => p.User)
                 .Where(t => t.Date.Date == DateTime.Now.Date
                     && t.Chat.TelegramId == id)
                 .FirstOrDefaultAsync()
                 .ConfigureAwait(false);
        }

        public async Task AddAsync(Result result)
        {
            await _context
                .Set<Result>()
                .AddAsync(result);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
