using System.Text.RegularExpressions;
using VkNet.Abstractions;
using VkNet.Model;

namespace VKBot.Abstractions
{
    interface ILongPollCommand
    {
        Regex CommandRegex { get; }
        string CommandHelp { get; }

        void Init(IVkApi api);
        void Run(IVkApi api, Message message);
    }
}
