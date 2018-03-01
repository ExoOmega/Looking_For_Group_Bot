using Discord;
using Discord.Commands;
using Discord.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looking_For_Group_Bot.Modules
{
    [Group("LFG-Admin")]
    [RequireBotPermission(GuildPermission.ManageRoles)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Name("List")]
        [Alias("list")]
        public async Task DefaultAdminList()
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

        [Command("create")]
        [Name("Create [Group Name]")]
        [Summary("Creates a Role group")]
        public async Task CreateRoleCommand([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            await Context.Channel.TriggerTypingAsync();
            if (msg == null)
            {
                await ReplyAsync("Group Name can not be empty. Please try again and include a Group Name to create.");
                return;
            }

            string UserName = Context.User.Username;
            string UserDescriminator = Context.User.Discriminator;

            var requestPermissions = new GuildPermissions();
            var requestOptions = new RequestOptions
            {
                AuditLogReason = $"{UserName}#{UserDescriminator} has created Role {GroupDiscriminator}{msg}"
            };
            var requestOptions2 = new RequestOptions
            {
                AuditLogReason = $"Setting {GroupDiscriminator}{msg} to mentionable."
            };
            RestRole createdRole = await Context.Guild.CreateRoleAsync(GroupDiscriminator + msg, requestPermissions);

            await createdRole.ModifyAsync(x =>
            {
                x.Mentionable = true;
            }, requestOptions2);

            await ReplyAsync($"Group {msg} has been created.");
        }

        [Command("delete")]
        [Name("Delete [Group Name]")]
        [Summary("Deletes a Role group")]
        public async Task DeleteRoleCommnand([Remainder] string msg = null)
        {
            var GroupDiscriminator = '~';

            await Context.Channel.TriggerTypingAsync();

            string UserName = Context.User.Username;
            string UserDescriminator = Context.User.Discriminator;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.ToUpper() == GroupDiscriminator + msg.ToUpper()
                select role;
            var requestOptions = new RequestOptions
            {
                AuditLogReason = $"{UserName}#{UserDescriminator} has deleted Role {GroupDiscriminator}{msg}"
            };

            try
            {
                var rolelinq = roleQuery.ElementAtOrDefault(0);
                await rolelinq.DeleteAsync(requestOptions);
            }
            catch
            {
                await ReplyAsync($"Group {msg} does not exist. Please verify spelling and try again.");
                return;
            }



            

            await ReplyAsync($"Group {msg} has been **DELETED.**");

        }

        [Command("clean")]
        [Name("Clean [Group Name]")]
        [Summary("Removes all users from the request group")]
        public async Task CleanRoleCommand([Remainder] string msg = null)
        {
            await DeleteRoleCommnand(msg);
            await CreateRoleCommand(msg);
        }
    }
}
