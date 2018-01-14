using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BpmBot.DB
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<BotContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new BotContext(builder.Options);
        }
    }
}
