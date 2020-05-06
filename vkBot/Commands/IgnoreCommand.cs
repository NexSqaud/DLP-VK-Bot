using System;
using System.Linq;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class IgnoreCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:[\.+-]игнор)\s?(\[id[0-9]+\|.*\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[🔕] Игнор:
📃 .игнор - Показывает список игнорируемых пользователей.
🔇 +игнор [упоминание или ответ] - Добавляет пользователя в список игнорируемых.
🔊 -игнор [упоминание или ответ] - Удаляет пользователя из списка игнорируемых.
";

        public void Init(IVkApi api)
        {
            Console.WriteLine("Ignore initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            long userId = 0;
            var match = CommandRegex.Match(message.Text);
            if (string.IsNullOrEmpty($"{match.Groups[1].Value}"))
            {
                if (message.ForwardedMessages.Count > 0)
                    userId = message.ForwardedMessages[0].FromId.Value;
                else if (message.ReplyMessage != null)
                    userId = message.ReplyMessage.FromId.Value;
            }
            else long.TryParse(match.Groups[1].Value.Split('|')[0].Substring(3), out userId);
            if(message.Text[0] == '-')
            {
                if (userId == 0) return;
                var name = Ignore.deleteIgnore(userId);
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = $"🔊 [id{userId}|{name.Name}] удален{(name.Female ? "а" : "")} из игнор-списка"
                });
            }
            else if(message.Text[0] == '+')
            {
                if (userId == 0) return;
                var user = api.Users.Get(new[] { userId }, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Sex)[0];
                var added = Ignore.addIgnore(userId, $"{user.FirstName} {user.LastName}", user.Sex == Sex.Female);
                if(added)
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"🔇 [id{userId}|{user.FirstName} {user.LastName}] добавлен{(user.Sex == Sex.Female ? "а" : "")} в игнор-список"
                    });
                else
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"⚠ [id{userId}|{user.FirstName} {user.LastName}] уже в игнор-списоке"
                    });
            }else if(message.Text[0] == '.')
            {
                var text = Ignore.ignoreList.Count > 0 ? Ignore.ignoreList
                    .Select((x, i) => $"{i + 1}. [id{x.Id}|{x.Name}]\n")
                    .Aggregate((first, second) => first + second) : null;
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = text != null ? $"🔇 Список игнорируемых:\n{text}" : "🔊 Нет игнорируемых!"
                });
            }
        }
    }
}
