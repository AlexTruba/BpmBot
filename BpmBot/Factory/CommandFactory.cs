using BpmBot.DB.Repository;
using BpmBot.Model;
using BpmBot.Service.Command;
using BpmBot.Service.Command.Impl;

namespace BpmBot.Factory
{
    public static class CommandFactory
    {
        public static ICommand GetCommand(Message message)
        {
            if (message.new_chat_member != null && message.new_chat_member.is_bot)
            {
                return new AddToChatCommand(new ChatRepository());
            }

            return FindByName(message.text);
        }

        private static ICommand FindByName(string name)
        {
            switch (name)
            {
                case "/reg":
                case "/reg@BlackTicketBot":
                    return new RegCommand(new ChatRepository(), new UserRepository());
                case "/run":
                case "/run@BlackTicketBot":
                    return new RunCommand(
                        new ResultRepository(), 
                        new ChatRepository(),
                        new UserRepository(), 
                        new CitationRepository());
                case "/result":
                case "/result@BlackTicketBot":
                    //GetResult(message.chat);
                    break;
            }
            return new AddToChatCommand(new ChatRepository());
        }
    }
}
