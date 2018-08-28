using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BpmBot.DB.Repository
{
    class CitationRepository
    {
        private readonly BotContext _context;
        public CitationRepository()
        {
            var contectFactory = new DesignTimeDbContextFactory();
            _context = contectFactory.CreateDbContext(null);
        }
        public async Task<IEnumerable<Citation>> GetRandomCitation()
        {
            return await _context
                .Set<Citation>()
                .AsNoTracking()
                .GroupBy(t => t.Group)
                .OrderBy(t => Guid.NewGuid())
                .Select(x => x.First())
                .ToListAsync();
        }
    }
}
