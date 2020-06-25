using Discord;
using System;
using System.Threading.Tasks;

namespace Trexia.Events
{
    class LogEvent
    {
        public static Task Event(LogMessage msg)
        {
            if (msg.Message.Equals("A supplied token was invalid."))
            {
                Console.WriteLine("[ERROR] Your discord token is invalid, please change it from 'config.json'.");
                Environment.Exit(1);
            }
            
            Console.WriteLine($"[LOG] {msg.Message}");

            return Task.CompletedTask;
        }
    }
}
