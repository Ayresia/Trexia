using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace Trexia.Commands
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        public async Task InfoAsync()
        {
            var _client = Startup._client;

            var _embedAuthorBuilder = new EmbedAuthorBuilder
            {
                IconUrl = _client.CurrentUser.GetDefaultAvatarUrl(),
                Name = "Trexia"
            };

            var _embedBuilder = new EmbedBuilder
            {
                Author = _embedAuthorBuilder,
                Color = Color.Blue
            };

            _embedBuilder.AddField("Version", "A1.0", true);
            _embedBuilder.AddField("Creator", "Ayresia#2327", true);
            _embedBuilder.AddField("Total Users", Context.Guild.Users.Count, true);

            await ReplyAsync(String.Empty, false, _embedBuilder.Build());
        }
    }
}
