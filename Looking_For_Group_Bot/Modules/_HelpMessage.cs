using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;

namespace Looking_For_Group_Bot.Modules
{
    [Name("Help Module")]
    public class _HelpMessage : ModuleBase<SocketCommandContext>
    {
        public CommandService CommandService { get; set; }

        [Command("help")]
        [Name("help")]
        [Summary("Get bot help")]
        public async Task HelpAsync()
        {
            await Context.Channel.TriggerTypingAsync();

            var content = new StringBuilder();
            content.AppendLine("Example Usage: **>Lfg Join [Group Name]**");
            content.AppendLine("```");
            foreach (var module in CommandService.Modules)
            {
                if (module.Commands.Count == 0) continue;

                content.AppendLine(module.Name);

                foreach (var command in module.Commands)
                    content.AppendLine(command.Name + "  :  " + command.Summary);

                content.AppendLine();
            }
            content.AppendLine("Version 1.2.4");
            content.AppendLine("```");

            await ReplyAsync(content.ToString());
        }
    }
}
