using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Discord;

namespace DiscordGifCounter
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider serviceProvider)
        {
            _commands = commands;
            _client = client;
            _services = serviceProvider;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _client.InteractionCreated += client_InteractionCreated;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            _services);
        }

        private async Task client_InteractionCreated(SocketInteraction arg)
        {
            var ctx = new SocketInteractionContext(_client, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message or if it is from the bot
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;

            if (message.Embeds.Any(a => a.Type == Discord.EmbedType.Gifv))
            {
                if (message.Channel.GetChannelType() == ChannelType.DM)
                {
                    await message.ReplyAsync(text: "Gifs in DMs do not count");
                    return;
                }

                Console.WriteLine($"User {message.Author} sent a gif");
                _services.GetService<CountService>().IncreaseGifCount(message.Author.Id);
            }
        }
    }
}
