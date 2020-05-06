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
    class FriendCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:[+-]др)\s?(\[id[0-9]+\|.*\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[🧒👦] Друзья:
✔️ +др [упоминание или ответ] - Добавляет пользователя в друзья/отправляет заявку в друзья.
❌ -др [упоминание или ответ] - Удаляет пользователя из друзей/отменяет заявку в друзья.
";


        public void Init(IVkApi api)
        {
            Console.WriteLine("Friend initialized");
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
            if (userId == 0)
            {
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = $"⚠ ID пользователя не найден!"
                });
                return;
            }
            if (message.Text[0] == '-')
            {
                var user = api.Users.Get(new[] { userId }, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Sex)[0];
                var delete = api.Friends.Delete(userId);
                var userName = $"[id{userId}|{user.FirstName} {user.LastName}]";
                if(delete.Success.Value)
                {
                    string text = "";
                    if (delete.FriendDeleted.Value) text = $"✅ {userName} удален{(user.Sex == Sex.Female ? "а" : "")} из друзей!";
                    else if(delete.InRequestDeleted.Value) text = $"✅ Заявка от {userName} откланена!";
                    else if(delete.OutRequestDeleted.Value) text = $"✅ Заявка на добавление {userName} отменена!";
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = text
                    });
                }
                else
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"⚠ Заявка от {userName} не найдена!"
                    });
                }
            }
            else if (message.Text[0] == '+')
            {
                var user = api.Users.Get(new[] { userId }, ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.Sex)[0];
                var add = api.Friends.Add(userId);
                var userName = $"[id{userId}|{user.FirstName} {user.LastName}]";
                string text = "";
                switch (add)
                {
                    case AddFriendStatus.Accepted:
                        text = $"✅ {userName} добавлен{(user.Sex == Sex.Female ? "а" : "")} в друзья!";
                        break;
                    case AddFriendStatus.Resubmit:
                        text = $"✅ Заявка {userName} отправлена повторно!";
                        break;
                    case AddFriendStatus.Sended:
                        text = $"✅ Заявка {userName} отправлена!";
                        break;
                    case AddFriendStatus.Unknown:
                        text = $"⚠ При отправке заявки {userName} произошла ошибка!";
                        break;
                }
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = text
                });
            }
        }
    }
}
