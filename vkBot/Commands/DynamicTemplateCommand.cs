using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class DynamicTemplateCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:[\.+-]дшаб)\s?(.*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[📜] Динамические шаблоны:
📜 .дшаб - Показывает список динамических шаблонов.
▶ .дшаб {название} - Запускает динамический шаблон {название}.
📝 +дшаб {название} {ENTER} {кадры} - Добавляет динамический шаблон {название}. Кадры отделяются пустой строкой.
🗑 -дшаб {название} - Удаляет динамический шаблон {название}.
";

        private List<DynamicTemplate> Templates = new List<DynamicTemplate>();

        public void Init(IVkApi api)
        {
            loadTemplates();
            Console.WriteLine("Dynamic Templates initialized");
        }

        public void Run(IVkApi api, Message message)
        {
            var templateName = CommandRegex.Match(message.Text).Groups[1].Value;//.Split('\n')[0];
            if (message.Text[0] == '.')
            {
                if (string.IsNullOrEmpty(templateName))
                {
                    var text = Templates.Count > 0 ? Templates
                        .Select((template, i) => $"{i + 1}. {template.Name}\n")
                        .Aggregate((first, second) => first+second) : null;
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"{(string.IsNullOrEmpty(text) ? "⚠ Список динамических шаблонов пуст!" : "📜 Список  динамических шаблонов:\n")}{text}"
                    });
                }
                else
                {
                    var template = Templates.Find(x => x.Name == templateName);
                    if (string.IsNullOrEmpty(template.Name))
                        api.Messages.Edit(new MessageEditParams()
                        {
                            PeerId = message.PeerId.Value,
                            MessageId = message.Id.Value,
                            Message = "⚠ Шаблон не найден!"
                        });
                    else
                    {
                        var timer = new DynamicTemplateTimer();
                        timer.start(template, message, api);
                    }
                }
            }
            else if (message.Text[0] == '+')
            {
                if (Templates.Any(x => x.Name == templateName))
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = "⚠ Такой шаблон уже есть!"
                    });
                    return;
                }
                var addTemplate = new List<string>();
                var toaggregate = message.Text.Split('\n');
                int frameIterator = 0;
                for(int i = 1; i < toaggregate.Length; i++)
                {
                    if (toaggregate[i] != "")
                        if (addTemplate.Count > frameIterator)
                            addTemplate[frameIterator] += toaggregate[i];
                        else
                            addTemplate.Add(toaggregate[i] + "\n");
                    else frameIterator++;
                }
                Templates.Add(new DynamicTemplate() { Name = templateName, Value = addTemplate });
                saveTemplates();
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = $"✅ Динамический шаблон {templateName} добавлен"
                });

            }
            else if (message.Text[0] == '-')
            {
                var deleteTemplate = Templates.Find(x => x.Name == templateName);
                if (deleteTemplate.Name == null)
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = "⚠ Шаблон не найден!"
                    });
                else
                {
                    Templates.Remove(deleteTemplate);
                    saveTemplates();
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = $"🗑 Динамический шаблон {templateName} удален"
                    });
                }
            }
        }

        private void loadTemplates()
        {
            if (File.Exists("dynamicTemplates.json"))
                Templates = JsonConvert.DeserializeObject<List<DynamicTemplate>>(File.ReadAllText("dynamicTemplates.json"));
            if (Templates == null) Templates = new List<DynamicTemplate>();
        }

        private void saveTemplates()
        {
            if (!File.Exists("dynamicTemplates.json"))
                File.Create("dynamicTemplates.json").Close();
            File.WriteAllText("dynamicTemplates.json", JsonConvert.SerializeObject(Templates, Formatting.Indented));
        }

        struct DynamicTemplate
        {
            public string Name;
            public List<string> Value;
        }

        class DynamicTemplateTimer
        {
            int iterator = 0;
            Timer timer;
            DynamicTemplate template;
            IVkApi api;
            long peerId;
            long messageId;

            public void start(DynamicTemplate template, Message message, IVkApi api)
            {
                timer = new Timer(1000);
                timer.AutoReset = true;
                timer.Elapsed += OnElapsed;
                this.template = template;
                this.api = api;
                peerId = message.PeerId.Value;
                messageId = message.Id.Value;
                timer.Start();
            }

            void OnElapsed(object sender, ElapsedEventArgs args)
            {
                if (iterator >= template.Value.Count) 
                {
                    timer.Stop();
                    timer.Dispose();
                    return;
                }
                try
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = peerId,
                        MessageId = messageId,
                        Message = template.Value[iterator]
                    });
                }catch(CaptchaNeededException ex)
                {
                    Console.WriteLine($"DYNAMIC TEMPLATE {template.Name} NEED CAPTCHA");
                }
                iterator++;
            }

        }

    }
}
