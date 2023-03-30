using Discord;
using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGifCounter
{
    public class CountModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CountService _CountService;

        public CountModule(CountService testService)
        {
            _CountService = testService;
        }

        [SlashCommand("count", "Get the count for a user")]
        public async Task Count(IUser user)
        {
            try
            {
                int count = _CountService.GetCountForUser(user.Id);
                await RespondAsync($"User {user.Username} has sent {count} gifs");
            }
            catch (Exception)
            {
                await RespondAsync($"User {user.Username} was not found");
            }
        }
    }
}
