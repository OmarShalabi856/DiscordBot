using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
	internal interface IBot
	{
		public Task StartAsync(ServiceProvider ServiceProvider);
		public Task StopAsync();

	}
}
