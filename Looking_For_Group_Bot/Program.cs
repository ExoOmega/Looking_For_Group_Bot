using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.IO;

namespace Looking_For_Group_Bot
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private CommandHandler _handler;

        StreamReader botToken = new StreamReader(Path.Combine(Environment.CurrentDirectory, "token.txt"));

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient();

            await _client.LoginAsync(TokenType.Bot, botToken.ReadLine());

            await _client.StartAsync();

            _handler = new CommandHandler(_client);

            await Task.Delay(-1);
        }
    }
}