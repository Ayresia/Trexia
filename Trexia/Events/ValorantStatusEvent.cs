using Discord.WebSocket;
using System;
using System.Net;
using Discord;
using System.Collections.Generic;
using System.Linq;
using Discord.Rest;
using TimeZoneNames;
using System.Threading.Tasks;
using System.Net.Http;
using Polly;
using Discord.Net;

namespace Trexia.Events
{
    class ValorantStatusEvent
    {
        public static async Task RemoveAllMessages(DiscordSocketClient _client)
        {
            ulong guildID = ulong.Parse(Startup._configuration["guild_id"]);
            ulong channelID = ulong.Parse(Startup._configuration["channel_id"]);

            if (_client.GetGuild(guildID)?.GetTextChannel(channelID) is { } _socketTextChannel)
            {
                var messages = await _socketTextChannel.GetMessagesAsync().FlattenAsync();
                await _socketTextChannel.DeleteMessagesAsync(messages);
            }

        }

        public static async void SendEmbed(bool successful, List<ValorantEvent> jsonParsed, int counter, SocketTextChannel _socketTextChannel, DiscordSocketClient _client, List<RestUserMessage> messageIDs)
        {
            var currentTime = DateTime.Now.ToLongTimeString();
            var currentTimeZone = TZNames.GetAbbreviationsForTimeZone(TimeZoneInfo.Local.Id, "en-GB").Standard;

            var embedBuilder = new EmbedBuilder();
            var region = jsonParsed[0].Regions[counter];

            ulong guildID = ulong.Parse(Startup._configuration["guild_id"]);
            ulong channelID = ulong.Parse(Startup._configuration["channel_id"]);

            _socketTextChannel = _client.GetGuild(guildID).GetTextChannel(channelID);

            if (successful)
            {
                embedBuilder = new EmbedBuilder
                {
                    Title = $"{region.Name.ToUpper()} Maintenance Status:",
                    Color = Color.Green,
                    Description = "There is no scheduled Maintenance!"
                };
                embedBuilder.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                var unsuccessfulEmbed = await _socketTextChannel.SendMessageAsync(String.Empty, false, embedBuilder.Build());
                messageIDs.Add(unsuccessfulEmbed);
            }
            else
            {
                embedBuilder = new EmbedBuilder
                {
                    Title = $"{region.Name.ToUpper()} Maintenance Status:",
                    Color = Color.Red
                };

                if (region.Maintenances[0].MaintenanceStatus.Equals("scheduled"))
                {
                    embedBuilder.AddField("Status:", "Scheduled Maintenance", false);
                }

                embedBuilder.AddField("Description:", region.Maintenances[0].Updates[0].Description, false);
                embedBuilder.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                var unsuccessfulEmbed = await _socketTextChannel.SendMessageAsync(String.Empty, false, embedBuilder.Build());
                messageIDs.Add(unsuccessfulEmbed);
            }
        }

        public static async void EditEmbed(bool successful, List<ValorantEvent> jsonParsed, int counter, RestUserMessage message)
        {
            var currentTime = DateTime.Now.ToLongTimeString();
            var currentTimeZone = TZNames.GetAbbreviationsForTimeZone(TimeZoneInfo.Local.Id, "en-GB").Standard;
            var region = jsonParsed[0].Regions[counter];
            var embedBuilder = new EmbedBuilder();

            if (successful)
            {
                embedBuilder = new EmbedBuilder
                {
                    Title = $"{jsonParsed[0].Regions[counter].Name.ToUpper()} Maintenance Status:",
                    Color = Color.Green,
                    Description = "There is no scheduled Maintenance!"
                };
                embedBuilder.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                await message.ModifyAsync(m =>
                {
                    m.Embed = embedBuilder.Build();
                });
            }
            else
            {
                embedBuilder = new EmbedBuilder
                {
                    Title = $"{region.Name.ToUpper()} Maintenance Status:",
                    Color = Color.Red
                };

                if (region.Maintenances[0].MaintenanceStatus.Equals("scheduled"))
                {
                    embedBuilder.AddField("Status:", "Scheduled Maintenance", false);
                }

                embedBuilder.AddField("Description:", jsonParsed[0].Regions[counter].Maintenances[0].Updates[0].Description, false);
                embedBuilder.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                await message.ModifyAsync(m =>
                {
                    m.Embed = embedBuilder.Build();
                });
            }
        }

        public static async void CheckMaintenance(DiscordSocketClient _client)
        {
            SocketTextChannel _socketTextChannel = null;
            String currentJson;
            int indexCounter = 0;

            List<RestUserMessage> messageIDs = new List<RestUserMessage>();
            HttpClient httpClient = new HttpClient();

            while (true)
            {
                Policy
                    .Handle<HttpException>()
                    .Retry(3, onRetry: async (exception, retryCount) =>
                    {
                        currentJson = await httpClient.GetStringAsync("https://riotstatus.vercel.app/valorant");
                    });

                currentJson = await httpClient.GetStringAsync("https://riotstatus.vercel.app/valorant");
                var jsonParsed = ValorantEvent.FromJson(currentJson);
                var regions = jsonParsed[0].Regions;

                if (!messageIDs.Any())
                {
                    await RemoveAllMessages(_client);
                    await Task.Delay(650);

                    foreach (var region in regions)
                    {
                        await Task.Delay(1000);

                        if (region.Maintenances.Count != 0)
                        {
                            SendEmbed(false, jsonParsed, indexCounter, _socketTextChannel, _client, messageIDs);
                        }
                        else
                        {
                            SendEmbed(true, jsonParsed, indexCounter, _socketTextChannel, _client, messageIDs);
                        }

                        indexCounter++;
                    }
                }
                else
                {
                    foreach (var message in messageIDs)
                    {
                        await Task.Delay(1000);

                        if (regions[indexCounter].Maintenances.Count != 0)
                        {
                            EditEmbed(false, jsonParsed, indexCounter, message);
                            indexCounter++;
                        }
                        else
                        {
                            EditEmbed(true, jsonParsed, indexCounter, message);
                            indexCounter++;
                        }
                    }
                }

                Console.WriteLine("[LOG] Added/Updated All Maintenance Status Regions.");
                indexCounter = 0;
                await Task.Delay(900000);
            }
        }
    }
}