using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
	public interface IEmailSender
	{
		public  Task SendEmailAsync(string email, string subject, string confirmLink);
		
		
	}
}
