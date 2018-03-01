using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looking_For_Group_Bot.Modules
{
    [Group("LFG")]
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task DefaultList()
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.StartsWith(GroupDiscriminator.ToString(), true, null)
                orderby role.Name
                select role;

            string roleListVar = "";
            StringBuilder roleListVarBuilder = new StringBuilder();
            foreach (IRole role in roleQuery)
            {
                roleListVarBuilder.Append(role.Name.TrimStart(GroupDiscriminator) + "\n");
            }
            roleListVar = roleListVarBuilder.ToString();
            if (roleListVar == "")
            {
                await ReplyAsync("There does not appear to be any Groups currently. Please try again late or have an Admin create one.");
                return;
            }

            await ReplyAsync($"__**Current Groups**__\n" +
                $"{roleListVar}");

        }

        [Command("join")]
        public async Task JoinGroup([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.ToUpper() == GroupDiscriminator + msg.ToUpper()
                select role;
            var rolelinq = roleQuery.ElementAtOrDefault(0);

            try
            {
                await Context.Guild.GetUser(Context.User.Id).AddRoleAsync(rolelinq);
            }
            catch
            {
                await ReplyAsync($"Group {msg} does not exist. Please Check the spelling or confirm that the group exists with an Admin.");
                return;
            }

            await Context.Channel.SendMessageAsync($"Added {playerName} to Group {rolelinq.Name.TrimStart(GroupDiscriminator)}.");
        }

        [Command("leave")]
        public async Task LeaveGroup([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.ToUpper() == GroupDiscriminator + msg.ToUpper()
                select role;

            var rolelinq = roleQuery.ElementAtOrDefault(0);

            try
            {
                await Context.Guild.GetUser(Context.User.Id).RemoveRoleAsync(rolelinq);
            }
            catch
            {
                await ReplyAsync($"Group {msg} could not be found. Please Check the spelling or contact an Admin to remove it.");
                return;
            }

            await Context.Channel.SendMessageAsync($"Removed {playerName} from Group {rolelinq.Name.TrimStart(GroupDiscriminator)}.");
        }

        [Command("join-all")]
        public async Task JoinAll([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.StartsWith(GroupDiscriminator.ToString(), true, null)
                select role;

            await Context.Guild.GetUser(Context.User.Id).AddRolesAsync(roleQuery);

            await Context.Channel.SendMessageAsync($"Added {playerName} to all Groups.");
        }
        [Command("leave-all")]
        public async Task LeaveAll([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.StartsWith(GroupDiscriminator.ToString(), true, null)
                select role;
            await Context.Guild.GetUser(Context.User.Id).RemoveRolesAsync(roleQuery);

            await Context.Channel.SendMessageAsync($"Removed {playerName} from all Group.");
        }
    }
}
