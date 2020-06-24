using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Valorant_BOT.Events
{
    class ConnectedEvent
    {
        public static Task Event()
        {
            Thread.Sleep(500);
            ValorantStatusEvent.CheckMaintenance(Startup._client);

            return Task.CompletedTask;
        }
    }
}
