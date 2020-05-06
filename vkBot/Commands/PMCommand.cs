using System;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;
using Message = VkNet.Model.Message;

namespace VKBot.Commands
{
    class PMCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:\.лс)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"✍️ .лс - Пишет сообщение себе в ЛС.
";

        public void Init(IVkApi api)
        {
            Console.WriteLine("PM command initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            api.Messages.Delete(new[] { (ulong)message.Id.Value }, deleteForAll: true);
            api.Messages.Send(new MessagesSendParams()
            {
                PeerId = api.UserId,
                RandomId = Environment.TickCount,
                Message = "я тут!"
            });
        }
    }
}
