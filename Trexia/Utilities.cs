using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Valorant_BOT;

namespace Trexia
{
    class Utilities
    {
        public static void createConfig()
        {
            var json = new
            {
                discord_token = "null",
                guild_id = "null",
                channel_id = "null",
            };

            using (StreamWriter file = File.CreateText(@".\config.json"))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(file, json);
            }
        }

        public static void checkConfig()
        {
            var configuration = Startup._configuration;

            if (configuration["discord_token"].Equals("null") || configuration["discord_token"].Length < 1)
            {
                Console.WriteLine("[LOG] Please put your discord token in `config.json`");
                Environment.Exit(1);
            } else if (configuration["guild_id"].Equals("null") || configuration["guild_id"].Length < 1 || checkStringAllDigit(configuration["guild_id"]))
            {
                Console.WriteLine("[LOG] Please put your guild id in `config.json`");
                Environment.Exit(1);
            } else if (configuration["channel_id"].Equals("null") || configuration["channel_id"].Length < 1 || checkStringAllDigit(configuration["channel_id"]))
            {
                Console.WriteLine("[LOG] Please put your channel id in `config.json`");
                Environment.Exit(1);
            }
        }

        public static bool checkStringAllDigit(String input)
        {
            foreach (char num in input)
            {
                if (num !< '0' || num !> '9')
                {
                    return true;
                }
            }

            return false;
        }
    }
}
