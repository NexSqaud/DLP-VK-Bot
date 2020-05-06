using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VkNet.Abstractions;

namespace VKBot
{
    class Ignore
    {

        public static List<Ignorable> ignoreList = new List<Ignorable>();

        public static bool addIgnore(long userId, string name, bool female)
        {
            if (!ignoreList.Any(x => x.Id == userId))
            {
                ignoreList.Add(new Ignorable() { Id = userId, Name = name, Female = female });
                save();
                return true;
            }
            return false;
        }

        public static Ignorable deleteIgnore(long userId)
        {
            var toDelete = ignoreList.Find(x => x.Id == userId);
            ignoreList.Remove(toDelete);
            save();
            return toDelete;
        }

        public static bool doIgnore(long userId,ulong messageId, IVkApi api)
        {
            if (ignoreList.Count == 0) load();
            if(ignoreList.Any(x=>x.Id == userId))
            {
                return api.Messages.Delete(new[] { messageId }, deleteForAll: false)[messageId];
            }
            return false;
        }

        private static void save()
        {
            if (!File.Exists("ignore.json"))
                File.Create("ignore.json").Close();
            File.WriteAllText("ignore.json", JsonConvert.SerializeObject(ignoreList));
        }

        private static void load()
        {
            if (File.Exists("ignore.json"))
                ignoreList = JsonConvert.DeserializeObject<List<Ignorable>>(File.ReadAllText("ignore.json"));
            if (ignoreList == null) ignoreList = new List<Ignorable>();
        }

        public struct Ignorable
        {
            public long Id;
            public string Name;
            public bool Female;
        }

    }
}
