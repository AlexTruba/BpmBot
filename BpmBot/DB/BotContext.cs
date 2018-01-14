using BpmBot.DB.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmBot.DB
{
    public class BotContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Result> Results { get; set; }

        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*builder.Entity<Chat>()
                 .HasIndex(u => u.Number);*/
        }
    }
}
