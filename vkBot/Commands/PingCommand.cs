using System;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class PingCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"(?:\.пинг)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"👀 .пинг - Показывает задержку от ВК до бота. (бывает врет)";

        public void Init(IVkApi api)
        {
            Console.WriteLine("Ping initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            api.Messages.Edit(new MessageEditParams()
            {
                PeerId = message.PeerId.Value,
                MessageId = message.Id.Value,
                Message = $"[DLP Reborn Vстошесот.229]\nPONG\nОтвет через: {(DateTime.UtcNow - message.Date.Value).TotalSeconds}с."
            });
        }

    }
}
