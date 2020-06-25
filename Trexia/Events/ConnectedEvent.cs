﻿using System.Threading;
using System.Threading.Tasks;

namespace Trexia.Events
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
