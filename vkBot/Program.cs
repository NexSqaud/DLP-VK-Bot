using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VkNet;
using VkNet.Model;
using VkNetExtend.MessageLongPoll;
using VKBot.Abstractions;
using VKBot.Commands;
using System.Threading.Tasks;

namespace VKBot
{
    class Program
    {
        private static VkApi api;
        private static List<ILongPollCommand> commands = new List<ILongPollCommand>();

        static void Main(string[] args)
        {
            api = new VkApi();
            var token = "ТОКЕН";
            var userId = 0;
            api.Authorize(new ApiAuthParams() { AccessToken = token, UserId = userId });
            VkNetExtMessageLongPollWatcher watcher = new VkNetExtMessageLongPollWatcher(new MessageLongPollWatcherOptions() { Wait = 10, StepSleepTimeMsec = 10, MaxSleepSteps = 10 }, api);
            watcher.NewEvents += newEvents;
            initCommands();
            new Task(x => (x as VkNetExtMessageLongPollWatcher).StartWatchAsync(), watcher).Start();
            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void initCommands()
        {
            commands.Add(new HelpCommand());
            commands.Add(new PingCommand());
            commands.Add(new PMCommand());
            commands.Add(new DeleteCommand());
            commands.Add(new TemplateCommand());
            commands.Add(new DynamicTemplateCommand());
            commands.Add(new IgnoreCommand());
            commands.Add(new FriendCommand());
            commands.Add(new BlackListCommand());

            foreach (var command in commands)
                command.Init(api);
        }

        public static string CommandsToString()
        {
            return commands.Select(x => x.CommandHelp + "\n").Aggregate((first, second) => first + second);
        }

        private static void newEvents(IMessageLongPollWatcher watcher, LongPollHistoryResponse history)
        {
            List<long> ignoreList = new List<long>();
            List<Message> messages = new List<Message>();
            if (history.History.Count > 0)
            {
                foreach (var item in history.History)
                {
                    if (item[0] == 2 || item[0] == 5)
                    {
                        if (item[2] == 131200 || item[2] == 128
                            || item[0] == 5)
                        {
                            ignoreList.Add(item[1]);

                        }
                    }
                }
                foreach (var message in history.Messages)
                {
                    if (ignoreList.Contains(Convert.ToInt64(message.Id))) continue;
                    messages.Add(message);
                }
                newMessages(messages);
            }
        }

        private static void newMessages(IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                //Console.WriteLine($"New message: ({message.PeerId.Value}){message.FromId.Value} - {message.Text}");
                if (Ignore.doIgnore(message.FromId.Value, (ulong)message.Id.Value, api))
                    return;
                if (message.FromId != api.UserId)
                    return;
                foreach (var command in commands)
                    try
                    {
                        if (command.CommandRegex.IsMatch(message.Text))
                            command.Run(api, message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"EXCEPTION: {ex.Message}\nStack trace:\n{ex.StackTrace}");
                    }
            }
        }
    }
}
