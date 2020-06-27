using Discord.Commands;
using Discord.WebSocket;
using Polly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Trexia.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        IDictionary<ulong, DateTime> currentCooldownUsers = new Dictionary<ulong, DateTime>();

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsASync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            int argPos = 0;
            var dateTime = DateTime.Now;
            var userID = message.Author.Id;
            var contains = currentCooldownUsers.ContainsKey(userID);

            if (message == null || !message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);


            if (contains && currentCooldownUsers[userID] > dateTime)
            {
                await context.Channel.SendMessageAsync($"You have a cooldown, you have {currentCooldownUsers[userID].Second - dateTime.Second}s left.");
                return;
            }
            else if (!contains)
            {
                currentCooldownUsers.Add(userID, dateTime.AddSeconds(5));
            }

            var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
            currentCooldownUsers[userID] = dateTime.AddSeconds(5);
        }
    }
}
