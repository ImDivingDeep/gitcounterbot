using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordGifCounter
{
    public class CountService
    {
        private const string STORAGE_FILE = "storage.xml";
        private CountStorage countStorage;

        public CountService()
        {
            if (!File.Exists(STORAGE_FILE))
            {
                using var file = File.Create(STORAGE_FILE);
            }

            try
            {
                using var file = new StreamReader(STORAGE_FILE);
                XmlSerializer serializer = new(typeof(CountStorage));
                countStorage = (CountStorage)serializer.Deserialize(file);
            }
            catch (Exception)
            {
                countStorage = new CountStorage() { Users = new List<UserStorage>() };
            }
            
        }

        public int GetCountForUser(ulong userId)
        {
            var data = countStorage.Users.FirstOrDefault(u => u.UserId == userId);

            if (data == null)
                throw new ArgumentException();

            return data.Count;
        }

        public void IncreaseGifCount(ulong userId)
        {
            var data = countStorage.Users.FirstOrDefault(u => u.UserId == userId);

            if (data == null)
            {
                data = new UserStorage()
                {
                    UserId = userId,
                    Count = 1
                };

                countStorage.Users.Add(data);
            }
            else
            {
                data.Count++;
            }

            SerializeStorage();
        }

        public void SerializeStorage()
        {
            using var file = new StreamWriter(STORAGE_FILE);
            XmlSerializer serializer = new(typeof(CountStorage));
            serializer.Serialize(file, countStorage);
        }
    }
}
