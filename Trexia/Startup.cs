using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Trexia.Events;
using Trexia.Services;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Trexia
{
    class Startup
    {
        public static DiscordSocketClient _client;
        public static IConfigurationRoot _configuration;
        private CommandService _commandService;
        private CommandHandler _commandHandler;


        static void Main(string[] args)
        {
            Console.Title = "Trexia 1.0A";
            try
            {
                IConfigurationBuilder _builder = new ConfigurationBuilder()
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

                _configuration = _builder.Build();

                Utilities.CheckConfig();
                new Startup().StartupAsync().GetAwaiter().GetResult();
            }
            catch (FileNotFoundException)
            {
                Utilities.CreateConfig();
                Console.WriteLine("[LOG] Created 'config.json', please relaunch Trexia.");
            }
        }

        public async Task StartupAsync()
        {
            var config = new DiscordSocketConfig { ExclusiveBulkDelete = true };

            _client = new DiscordSocketClient(config);
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService);

            _client.Log += LogEvent.Event;
            _client.Connected += ConnectedEvent.Event;

            await _commandHandler.InstallCommandsASync();
            await _client.LoginAsync(TokenType.Bot, _configuration["discord_token"]);
            await _client.StartAsync();
            await _client.SetGameAsync("Created by Ayresia#2327", null, ActivityType.Playing);

            await Task.Delay(-1);
        }
    }
}



