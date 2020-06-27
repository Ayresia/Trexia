using Discord.Commands;
using System.Threading.Tasks;

namespace Trexia.Commands
{
    public class Test : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestAsync()
        {
            await ReplyAsync("This is a test command.", false);
        }
    }
}
