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
            Bot bot = new Bot();
            Timer timer = new Timer(2000);
            timer.Elapsed += (sender, e) => bot.Start();
            timer.Start();
            Console.ReadKey();
        }
    }
}
