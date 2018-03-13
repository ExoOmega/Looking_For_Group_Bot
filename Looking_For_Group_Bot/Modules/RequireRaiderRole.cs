using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Looking_For_Group_Bot.Modules
{
    class RequireRaiderRole : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext Context, CommandInfo command, IServiceProvider services)
        {
            ulong role = 422839300061790208;
            IGuildUser user = await Context.Guild.GetUserAsync(Context.User.Id);
            // If this command was executed by that user, return a success
            if (user.RoleIds.Contains(role))
                return PreconditionResult.FromSuccess();
            // Since it wasn't, fail
            else
                return PreconditionResult.FromError("You must be the owner of the bot to run this command.");
        }
    }
}
