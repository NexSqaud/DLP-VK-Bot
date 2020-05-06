using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class DeleteCommand : ILongPollCommand
    {

        public Regex CommandRegex => new Regex(@"^(?:дд)\s([0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[✉] Удаление сообщений:
🚫 дд {число} - Удаляет последние {число} сообщений.
";

        public void Init(IVkApi api)
        {
            Console.WriteLine("Delete Initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            int count = int.Parse(CommandRegex.Match(message.Text).Groups[1].Value);
            count++;
            if (count > 200) count = 200;
            try
            {
                var history = api.Messages.GetHistory(new MessagesGetHistoryParams()
                {
                    Count = 200,
                    PeerId = message.PeerId
                });
                var messagesToDelete = history.Messages.ToList().FindAll(x => x.FromId == message.FromId);
                messagesToDelete.RemoveRange(count, messagesToDelete.Count - count);
                for(int i = 0; i < messagesToDelete.Count; i++)
                {
                    try
                    {
                        api.Messages.Edit(new MessageEditParams()
                        {
                            PeerId = message.PeerId.Value,
                            MessageId = messagesToDelete[i].Id.Value,
                            Message = "&#13;"
                        });
                    }catch(CaptchaNeededException ex)
                    {
                        Console.WriteLine("CAPTCHA NEEDED!");
                    }catch(Exception ex)
                    {
                        Console.WriteLine($"EXCEPTION:\n{ex.Message}");
                    }
                }

                List<ulong> ids = messagesToDelete.AsQueryable().Select(x => (ulong)x.Id).ToList();
                api.Messages.Delete(ids, deleteForAll: true);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Delete exception:\n{ex.Message}\n");
            }
        }
    }
}
