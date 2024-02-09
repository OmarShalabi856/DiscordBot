using Discord;
using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
	public class SendEmailCommand : ModuleBase<SocketCommandContext>
	{
		private readonly IEmailSender _emailSender;

		public SendEmailCommand(IEmailSender emailSender)
		{
			_emailSender = emailSender;
		}

		[Command("SendEmail")]
		[Summary("Sends an email with subject and message")]
		public async Task SendEmail([Remainder] string phrase)
		{
			try
			{
				string emailPattern = @"Email:\s*([^\s]+)";
				string subjectPattern = @"Subject:\s*([^\s]+)";
				string messagePattern = @"Message:\s*(.+)";

				Regex emailRegex = new Regex(emailPattern);
				Regex subjectRegex = new Regex(subjectPattern);
				Regex messageRegex = new Regex(messagePattern);

				Match emailMatch = emailRegex.Match(phrase);
				Match subjectMatch = subjectRegex.Match(phrase);
				Match messageMatch = messageRegex.Match(phrase);

				string email = emailMatch.Success ? emailMatch.Groups[1].Value : throw new Exception("Missing Email!");
				string subject = subjectMatch.Success ? subjectMatch.Groups[1].Value : throw new Exception("Missing Subject!");
				string message = messageMatch.Success ? messageMatch.Groups[1].Value : "";

				await _emailSender.SendEmailAsync(email, subject, message);

				await ReplyAsync("Email sent successfully!", false, null, null,null, new MessageReference(Context.Message.Id));
			}
			catch (Exception ex)
			{

				await ReplyAsync($"Error: {ex.Message}", false, null, null,null, new MessageReference(Context.Message.Id));
			}
		}
	}
}
