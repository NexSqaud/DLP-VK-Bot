using System;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class BlackListCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:[+-]чс)\s?(\[id[0-9]+\|.*\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[📛] Черный список:
📵 +чс [упоминание или ответ] - Добавляет пользователя в черный список.
📱 -чс [упоминание или ответ] - Удаляет пользователя из черного списка.
";

        public void Init(IVkApi api)
        {
            Console.WriteLine("Black List initialized");
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
            if(userId == 0)
            {
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = $"⚠ ID пользователя не найден!"
                });
                return;
            }
            if (message.Text[0] == '+')
            {
                var user = api.Users.Get(new[] { userId }, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Sex)[0];
                var add = api.Account.BanUser(userId);
                var userName = $"[id{userId}|{user.FirstName} {user.LastName}]";
                if (add)
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"✅ {userName} добавлен{(user.Sex == Sex.Female ? "а" : "")} в черный список!"
                    });
                }
                else
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"⚠ Ошибка при добавлении {userName} в черный список!"
                    });
                }
            }
            else if (message.Text[0] == '-') 
            {
                var user = api.Users.Get(new[] { userId }, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Sex)[0];
                var delete = api.Account.UnbanUser(userId);
                var userName = $"[id{userId}|{user.FirstName} {user.LastName}]";
                if (delete)
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"✅ {userName} удален{(user.Sex == Sex.Female ? "а" : "")} из черного списока!"
                    });
                }
                else
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"⚠ Ошибка при удалении {userName} из черного списка!"
                    });
                }

            }
        }
    }
}
