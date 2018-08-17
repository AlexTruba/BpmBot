using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;

namespace BpmBot.DB
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserResult> UserResults { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Citation> Citations { get; set; }
        public DbSet<Global> Globals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*builder.Entity<Chat>()
                 .HasIndex(u => u.Number);*/
        }
    }
}