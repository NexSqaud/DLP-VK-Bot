using System;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class HelpCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:\.помощь)");
        public string CommandHelp => @"[📄] Помощь:
📖 .помощь - Показывает все команды.";

        public void Init(IVkApi api)
        {
            Console.WriteLine("Help initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            api.Messages.Edit(new MessageEditParams()
            {
                PeerId = message.PeerId.Value,
                MessageId = message.Id.Value,
                Message = $"[Команды бота]\n{Program.CommandsToString()}"
            });
        }
    }
}
