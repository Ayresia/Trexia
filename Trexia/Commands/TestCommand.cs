using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valorant_BOT.Commands
{
    public class TestCommand : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestAsync()
        {
            await ReplyAsync("Hello World", true);
        }
    }
}
