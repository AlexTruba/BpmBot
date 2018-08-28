using BpmBot.Model;
using BpmBot.Service;
using BpmBot.TelegramApi;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace BpmBot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var timer = new Timer(2000))
            {
                Bot bot = new Bot();
                timer.Elapsed += async (sender, e) => await bot.Start();
                timer.Disposed += (sender, e) => bot.Dispose();
                timer.Start();
                Console.ReadKey();
            }
        }
    }
}
