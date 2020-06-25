using Discord.WebSocket;
using System;
using System.Threading;
using System.Net;
using Discord;
using System.Collections.Generic;
using System.Linq;
using Discord.Rest;
using TimeZoneNames;

namespace Trexia.Events
{
    class ValorantStatusEvent
    {
        public static async void CheckMaintenance(DiscordSocketClient _client)
        {
            SocketTextChannel client = null;
            String currentJson;
            int indexCounter = 0;
            ulong guildID = Convert.ToUInt64(Startup._configuration["guild_id"]);
            ulong channelID = Convert.ToUInt64(Startup._configuration["channel_id"]);

            List<RestUserMessage> messageIDs = new List<RestUserMessage>();
            try
            {
                client = _client.GetGuild(guildID).GetTextChannel(channelID);
            } catch (NullReferenceException)
            {
                Console.WriteLine("[ERROR] You have a invalid Guild/Channel ID in 'config.json'.");
                Environment.Exit(1);
            }

            var webClient = new WebClient();

            while (true)
            {
                currentJson = webClient.DownloadString("https://riotstatus.vercel.app/valorant");
                var jsonParsed = ValorantEvent.FromJson(currentJson);
                var embed = new EmbedBuilder();
                var currentTime = DateTime.Now.ToLongTimeString();
                var currentTimeZone = TZNames.GetAbbreviationsForTimeZone(TimeZoneInfo.Local.Id, "en-GB").Standard;

                if (!messageIDs.Any())
                {
                    try
                    {
                        var messages = await client.GetMessagesAsync().FlattenAsync();
                        await client.DeleteMessagesAsync(messages);
                    } catch (NullReferenceException)
                    {
                        Console.WriteLine("[ERROR] You have a invalid Guild/Channel ID in 'config.json'.");
                        break;
                    }

                    Thread.Sleep(500);

                    for (int cout = 0; cout < 6; cout++)
                    {
                        Thread.Sleep(850);

                        embed = new EmbedBuilder
                        {
                            Title = $"{jsonParsed[0].Regions[cout].Name.ToUpper()} Maintenance Status:",
                            Color = Color.Red
                        };

                        try
                        {
                            if (jsonParsed[0].Regions[cout].Maintenances[0].MaintenanceStatus.Equals("scheduled"))
                            {
                                embed.AddField("Status:", "Scheduled Maintenance", false);
                            }
                            embed.AddField("Description:", jsonParsed[0].Regions[cout].Maintenances[0].Updates[0].Description, false);
                            embed.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                            try
                            {
                                var unsuccessfulMessage = await client.SendMessageAsync(String.Empty, false, embed.Build());
                                messageIDs.Add(unsuccessfulMessage);
                            }
                            catch (NullReferenceException)
                            {
                                Console.WriteLine("[ERROR] You have a invalid Channel ID in 'config.json'");
                                break;
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            embed = new EmbedBuilder
                            {
                                Title = $"{jsonParsed[0].Regions[cout].Name.ToUpper()} Maintenance Status:",
                                Color = Color.Green,
                                Description = "There is no scheduled Maintenance!"
                            };
                            embed.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");
                            try
                            {
                                var successfulMessage = await client.SendMessageAsync(String.Empty, false, embed.Build());
                                messageIDs.Add(successfulMessage);
                            }
                            catch (NullReferenceException)
                            {
                                Console.WriteLine("[ERROR] You have a invalid Channel ID in 'config.json'");
                                break;
                            }
                            continue;
                        }
                    }
                }
                else
                {
                    foreach (var message in messageIDs)
                    {
                        embed = new EmbedBuilder
                        {
                            Title = $"{jsonParsed[0].Regions[indexCounter].Name.ToUpper()} Maintenance Status:",
                            Color = Color.Red,
                        };

                        try
                        {
                            if (jsonParsed[0].Regions[indexCounter].Maintenances[indexCounter].MaintenanceStatus.Equals("scheduled"))
                            {
                                embed.AddField("Status:", "Scheduled Maintenance", false);
                            }

                            embed.AddField("Description:", jsonParsed[0].Regions[indexCounter].Maintenances[0].Updates[0].Description, false);
                            embed.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                            await message.ModifyAsync(m =>
                            {
                                m.Embed = embed.Build();
                            });
                            indexCounter++;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            var embedS = new EmbedBuilder
                            {
                                Title = $"{jsonParsed[0].Regions[indexCounter].Name.ToUpper()} Maintenance Status:",
                                Color = Color.Green,
                                Description = "There is no scheduled Maintenance!",
                            };

                            embed.WithFooter($"Trexia A1.0 | Updated on {currentTime} {currentTimeZone}");

                            await message.ModifyAsync(m =>
                            {
                                m.Embed = embedS.Build();
                            });

                            indexCounter++;
                            continue;
                        }
                    }
                }

                indexCounter = 0;
                Console.WriteLine("[LOG] Added/Updated All Maintenance Status Regions.");
                Thread.Sleep(900000); // Equivalent to 15 minutes.
            }
        }
    }
}
