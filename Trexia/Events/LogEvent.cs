using Discord;
using System;
using System.Threading.Tasks;

namespace Valorant_BOT.Events
{
    class LogEvent
    {
        public static Task Event(LogMessage msg)
        {
            if (msg.Message.Equals("A supplied token was invalid."))
            {
                Console.WriteLine("[ERROR] Your discord token was invalid.");
                Environment.Exit(1);
            } else
            {
                Console.WriteLine($"[LOG] {msg.Message}");
            }
            return Task.CompletedTask;
        }
    }
}
