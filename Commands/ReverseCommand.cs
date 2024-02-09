using Discord.Commands;
using System.Linq;

namespace DiscordBot.Commands.EchoCommand
{
	public class ReverseCommand : ModuleBase<SocketCommandContext>
	{
		[Command("reverse")]
		[Summary("Reverses the input text")]
		public async Task ExecuteAsync([Remainder][Summary("A phrase")] string phrase)
		{
			if (string.IsNullOrEmpty(phrase))
			{
				await ReplyAsync("Usage: !reverse <phrase>");
				return;
			}

			string[] words = phrase.Split(' ');
			words = words.Reverse().ToArray();
			string reversedPhrase = string.Join(" ", words);
			await ReplyAsync(reversedPhrase);
		}
	}
}
