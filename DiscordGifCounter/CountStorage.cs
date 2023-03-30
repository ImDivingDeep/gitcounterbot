using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGifCounter
{
    public class CountStorage
    {
        public List<UserStorage> Users { get; set; }
    }

    public class UserStorage
    {
        public ulong UserId { get; set; }
        public int Count { get; set; }
    }
}
