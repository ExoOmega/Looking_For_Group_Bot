using Discord;
using Discord.Commands;
using Discord.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looking_For_Group_Bot.Modules
{
    [Group("LFG")]
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        public char GroupDiscriminator = '~';

        [Command]
        [Name("List")]
        [Alias("list")]
        public async Task DefaultList()
        {
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
        [Name("Join [Group Name]")]
        [Summary("Joins a LFG Bot created Role group")]
        public async Task JoinGroup([Remainder] string msg = null)
        {
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
            
            //deletes the input command
            await Context.Message.DeleteAsync();
        }

        [Command("leave")]
        [Name("Leave [Group Name]")]
        [Summary("Leaves a LFG Bot created Role group")]
        public async Task LeaveGroup([Remainder] string msg = null)
        {
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

            //deletes the input command
            await Context.Message.DeleteAsync();
        }

        [Command("join-all")]
        [Name("Join-All")]
        [Summary("Joins all LFG Bot created Role groups")]
        public async Task JoinAll([Remainder] string msg = null)
        {
            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.StartsWith(GroupDiscriminator.ToString(), true, null)
                select role;

            await Context.Guild.GetUser(Context.User.Id).AddRolesAsync(roleQuery);

            await Context.Channel.SendMessageAsync($"Added {playerName} to all Groups.");

            //deletes the input command
            await Context.Message.DeleteAsync();
        }

        [Command("leave-all")]
        [Name("Leave-All")]
        [Summary("Leaves all LFG Bot created Role groups")]
        public async Task LeaveAll([Remainder] string msg = null)
        {
            await Context.Channel.TriggerTypingAsync();

            string playerName = Context.User.Mention;

            var playerId = Context.User.Id;

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.StartsWith(GroupDiscriminator.ToString(), true, null)
                select role;
            await Context.Guild.GetUser(Context.User.Id).RemoveRolesAsync(roleQuery);

            await Context.Channel.SendMessageAsync($"Removed {playerName} from all Group.");

            //deletes the input command
            await Context.Message.DeleteAsync();
        }

        [Command("create")]
        [Name("Create [Group Name]")]
        [Summary("Creates a Role group")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task CreateRoleCommand([Remainder] string msg = null)
        {
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
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task DeleteRoleCommnand([Remainder] string msg = null)
        {
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
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task CleanRoleCommand([Remainder] string msg = null)
        {
            await DeleteRoleCommnand(msg);
            await CreateRoleCommand(msg);
        }

        [Command("Ping")]
        [Name("Ping [Group Name]")]
        [Summary("Pings the specified Group")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "1")]
        [RequireRaiderRole(Group = "1")]
        public async Task PingRole([Remainder] string msg = null)
        {
            await Context.Channel.TriggerTypingAsync();
            if (msg == null)
            {
                await ReplyAsync("Group Name can not be empty. Please try again and include a Group Name to ping.");
                return;
            }

            string UserName = Context.User.Username;
            string UserDescriminator = Context.User.Discriminator;

            var requestOptions = new RequestOptions
            {
                AuditLogReason = $"Setting {GroupDiscriminator}{msg} to mentionable."
            };

            IEnumerable<IRole> roleQuery =
                from role in Context.Guild.Roles
                where role.Name.ToUpper() == GroupDiscriminator + msg.ToUpper()
                select role;
            try
            {
                var rolelinq = roleQuery.ElementAtOrDefault(0);
                await rolelinq.ModifyAsync(x =>
                {
                    x.Mentionable = true;
                });
                await ReplyAsync($"{Context.User.Mention} has Pinged Group {rolelinq.Mention}");
                await rolelinq.ModifyAsync(x =>
                {
                    x.Mentionable = false;
                });
            }
            catch
            {
                await ReplyAsync($"Group {msg} does not exist. Please verify spelling and try again.");
                return;
            }

            //deletes the input command
            await Context.Message.DeleteAsync();
        }
    }
}
