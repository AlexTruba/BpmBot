using System.Collections.Generic;

namespace BpmBot.Model
{
    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string language_code { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public bool? all_members_are_administrators { get; set; }
    }

    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }

    public class NewChatParticipant
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class NewChatMember
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public List<Entity> entities { get; set; }
        public NewChatParticipant new_chat_participant { get; set; }
        public NewChatMember new_chat_member { get; set; }
        public List<NewChatMember> new_chat_members { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public Message message { get; set; }
    }

    public class Response
    {
        public bool ok { get; set; }
        public List<Result> result { get; set; }
    }
}
