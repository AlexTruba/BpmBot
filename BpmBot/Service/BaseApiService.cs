using BpmBot.Infrastructure;
using BpmBot.TelegramApi;

namespace BpmBot.Service
{
    abstract class BaseApiService
    {
        protected readonly APIService _service;
        public BaseApiService()
        {
            var resolver = new ConfigurationResolver();
            _service = new APIService(resolver.GetValue("token"), resolver.GetValue("url"));
        }
    }
}
