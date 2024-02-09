using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services.Implementations
{
	public class ScrapperSetup : IScrapperSetup
	{
		public async Task<Browser> LaunchBrowserAsync()
		{
			try
			{
				//await new BrowserFetcher().DownloadAsync();
				var launchOptions = new LaunchOptions { Headless = true ,ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe" };
				return (Browser)await Puppeteer.LaunchAsync(launchOptions);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error initializing Puppeteer: {ex.Message}");
				throw;
			}
		}
	}
}
