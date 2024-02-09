using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
	public interface IScrapperSetup
	{
		Task<Browser> LaunchBrowserAsync();
	}

}
