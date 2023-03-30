using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace DiscordGifCounter
{
    class Program
    {
        private DiscordSocketClient _client;
        private InteractionService _commands;
        private CommandHandler _commandHandler;
        private IServiceProvider _services;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });

            _commands = new InteractionService(_client);

            _client.Log += Log;
            _client.Ready += Client_Ready;

            _commands.Log += Log;

            _services = ConfigureServices();

            _commandHandler = new CommandHandler(_client, _commands, _services);

            // Token is read from the app.config file
            var token = ConfigurationManager.AppSettings.Get("Token");

            await _commandHandler.InstallCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {
            await _commands.RegisterCommandsGloballyAsync(true);
        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection().AddSingleton(new CountService());

            return map.BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
