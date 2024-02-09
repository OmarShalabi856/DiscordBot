using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
	public interface IScrapper
	{
		public Task<List<Game>> LaunchScrape(string url);
	}
}
