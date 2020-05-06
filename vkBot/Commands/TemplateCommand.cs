using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VKBot.Abstractions;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VKBot.Commands
{
    class TemplateCommand : ILongPollCommand
    {
        public Regex CommandRegex => new Regex(@"^(?:[\.+-]шаб)\s?(.*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string CommandHelp => @"[📃] Шаблоны:
📃 .шаб - Показывает список шаблонов.
✏ .шаб {название} - Заменяет сообщение шаблоном {название}.
📝 +шаб {название} {ENTER} {шаблон} - Добавляет шаблон {название}. Поддерживаются вложения.
🗑 -шаб {название} - Удаляет шаблон {название}.
";


        private List<Template> Templates = new List<Template>();

        public void Init(IVkApi api)
        {
            loadTemplates();
            Console.WriteLine("Template initialized");
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
                        Message = $"{(string.IsNullOrEmpty(text) ? "⚠ Список шаблонов пуст!" : "📃 Список шаблонов:\n")}{text}"
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
                        var attachment = getAttachments(template, api);
                        api.Messages.Edit(new MessageEditParams()
                        {
                            PeerId = message.PeerId.Value,
                            MessageId = message.Id.Value,
                            Message = template.Value,
                            Attachments = attachment
                        });
                    }
                }
            }else if(message.Text[0] == '+')
            {
                if(Templates.Any(x=>x.Name == templateName))
                {
                    api.Messages.Edit(new MessageEditParams()
                    {
                        PeerId = message.PeerId.Value,
                        MessageId = message.Id.Value,
                        Message = "⚠ Такой шаблон уже есть!"
                    });
                    return;
                }
                var addTemplate = message.Text.Split('\n').Length > 1 ? message.Text.Split('\n')
                    .Aggregate((first, second) => first != $"+дшаб {templateName}" ? first + "\n" + second : second)
                    : null;
                List<Attachment> attachments = new List<Attachment>();
                if(message.Attachments.Count > 0)
                {
                    attachments = message.Attachments.Select(x => new Attachment() { Type = getType(x.Type), OwnerId = x.Instance.OwnerId.Value, Id = x.Instance.Id.Value, AccessKey = x.Instance.AccessKey }).ToList();
                }
                Templates.Add(new Template() { Name = templateName, Value = addTemplate, Attachments = attachments });
                saveTemplates();
                api.Messages.Edit(new MessageEditParams()
                {
                    PeerId = message.PeerId.Value,
                    MessageId = message.Id.Value,
                    Message = $"✅ Шаблон {templateName} добавлен"
                });

            }
            else if(message.Text[0] == '-')
            {
                var deleteTemplate = Templates.Find(x => x.Name == templateName);
                if(deleteTemplate.Name == null)
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
                        Message = $"🗑 Шаблон {templateName} удален"
                    });
                }
            }
        }

        private Attachment.AttachmentType getType(Type type)
        {
            switch (type.Name)
            {
                case "Photo":
                    return Attachment.AttachmentType.Photo;
                case "Video":
                    return Attachment.AttachmentType.Video;
                case "Audio":
                    return Attachment.AttachmentType.Audio;
                case "Document":
                    return Attachment.AttachmentType.Doc;
                case "Wall":
                    return Attachment.AttachmentType.Wall;
                case "Market":
                    return Attachment.AttachmentType.Market;
                case "Poll":
                    return Attachment.AttachmentType.Poll;
                default:
                    return Attachment.AttachmentType.Unknown;
            }
            
        }

        private MediaAttachment[] getAttachments(Template template, IVkApi api)
        {
            List<MediaAttachment> attachments = new List<MediaAttachment>();
            var photos = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Photo);
            var videos = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Video);
            var audios = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Audio);
            var docs = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Doc);
            var walls = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Wall);
            var markets = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Market);
            var polls = template.Attachments.FindAll(x => x.Type == Attachment.AttachmentType.Poll);
            if(photos != null && photos.Count > 0)
                attachments.AddRange(photos.Select(x => new Photo() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (videos != null && videos.Count > 0)
                attachments.AddRange(videos.Select(x => new Video() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (audios != null && audios.Count > 0)
                attachments.AddRange(audios.Select(x => new Audio() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (docs != null && docs.Count > 0)
                attachments.AddRange(docs.Select(x => new Document() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (walls != null && walls.Count > 0)
                attachments.AddRange(walls.Select(x => new Wall() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (markets != null && markets.Count > 0)
                attachments.AddRange(markets.Select(x => new Market() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            if (polls != null && polls.Count > 0)
                attachments.AddRange(polls.Select(x => new Poll() { OwnerId = x.OwnerId, Id = x.Id, AccessKey = x.AccessKey }));
            return attachments.ToArray();
        }

        private void loadTemplates()
        {
            if (File.Exists("templates.json"))
                Templates = JsonConvert.DeserializeObject<List<Template>>(File.ReadAllText("templates.json"));
            if (Templates == null) Templates = new List<Template>();
        }

        private void saveTemplates()
        {
            if (!File.Exists("templates.json"))
                File.Create("templates.json").Close();
            File.WriteAllText("templates.json", JsonConvert.SerializeObject(Templates, Formatting.Indented));
        }

        struct Template
        {
            public string Name;
            public string Value;
            public List<Attachment> Attachments;
        }

        struct Attachment
        {

            public enum AttachmentType
            {
                Photo,
                Video,
                Audio,
                Doc,
                Wall,
                Market,
                Poll,
                Unknown
            }

            public long Id;
            public long OwnerId;
            public string AccessKey;
            public AttachmentType Type;
        }

    }
}
