using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services.Implementations
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmailAsync(string email, string subject, string confirmLink)
		{
			try
			{
				using (MailMessage message = new MailMessage())
				{
					using (SmtpClient smtpClient = new SmtpClient())
					{
						message.From = new MailAddress("omarshalabibot@gmail.com");
						message.To.Add(email);
						message.Subject = subject;
						message.IsBodyHtml = true;
						message.Body = confirmLink;

						smtpClient.Port = 587;
						smtpClient.Host = "smtp.gmail.com";
						smtpClient.EnableSsl = true;
						smtpClient.UseDefaultCredentials = false;
						smtpClient.Credentials = new NetworkCredential("omarshalabibot@gmail.com", "skvh zxra zbgt kcut");
						smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;


						await smtpClient.SendMailAsync(message);
					}
				}
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}
	}
}
