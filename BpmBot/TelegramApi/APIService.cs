using BpmBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BpmBot.TelegramApi
{
    class APIService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client = new HttpClient();

        public APIService(string token, string url)
        {
            _baseUrl = $"{url}/bot{token}/";
        }

        public async Task<Response> GetUpdatesAsync()
        {
            var responseString = await GetResponseStringAsync("getUpdates");
            Response response = null;
            try
            {
                response = JsonConvert.DeserializeObject<Response>(responseString);
            }
            catch (Exception) { }

            return response;
        }
        public async Task<Response> GetChatAsync(int id)
        {
            var responseString = await PostResponseStringAsync("getChat",
                new Dictionary<string, string>()
                    {
                        { "chat_id", id.ToString() }
                    }
                ).ConfigureAwait(false);
            Response response = null;
            try
            {
                // change response to chat response
                response = JsonConvert.DeserializeObject<Response>(responseString);
            }
            catch (Exception) { }

            return response;
        }
        /*public async Task<Response> GetMemberAsync(int id)
        {
            var responseString = await PostResponseStringAsync("getMember",
                new Dictionary<string, string>()
                    {
                        { "chat_id", id.ToString() }
                    }
                );
            Response response = null;
            try
            {
                response = JsonConvert.DeserializeObject<Response>(responseString);
            }
            catch (Exception) { }

            return response;
        }*/

        public async void SendMessageAsync(int chatId, string text)
        {
            var responseString = await PostResponseStringAsync("sendMessage",
                new Dictionary<string, string>()
                    {
                        { "chat_id", chatId.ToString() },
                        { "text", text }
                    }
                ).ConfigureAwait(false);
        }

        #region Private Method
        private async Task<string> GetResponseStringAsync(string methodName)
        {
            var responseMessage = await _client.GetAsync(_baseUrl + methodName);
            return await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task<string> PostResponseStringAsync(string methodName, Dictionary<string, string> par)
        {
            var content = new FormUrlEncodedContent(par);
            var responseMessage = await _client.PostAsync(_baseUrl + methodName, content);
            return await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        #endregion Private Method
    }
}
