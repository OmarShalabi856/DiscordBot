using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
	internal class Program
	{
		private static  void Main(string[] args)
		{
			 RunBot().GetAwaiter().GetResult();	
		}
			

		private static async Task RunBot()
		{
			

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(Assembly.GetExecutingAssembly())
				.AddJsonFile("appsettings.json")
				.Build();

		

			DiscordSocketConfig config = new()
			{
				GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
			};

			var serviceProvider = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration)
				.AddLogging(options =>
				{
					options.ClearProviders();
					options.AddConsole();
				})
				.AddScoped<IBot, Bot>()
				.AddSingleton<IScrapperSetup, ScrapperSetup>()
				.AddSingleton<IScrapper, Scrapper>()
				.AddSingleton(provider =>
				{
					return new DiscordSocketConfig
					{
						GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
						
					};
				})
				.AddSingleton(provider =>
				{
					var config = provider.GetRequiredService<DiscordSocketConfig>();
					return new DiscordSocketClient(config);
				})
				.AddSingleton<CommandService>()
				.AddScoped<IEmailSender, EmailSender>()
				.BuildServiceProvider();

			try
			{
				IBot bot = serviceProvider.GetRequiredService<IBot>();

				await bot.StartAsync(serviceProvider);

				do
				{
					var keyInfo = Console.ReadKey();

					if (keyInfo.Key == ConsoleKey.Q)
					{
						await bot.StopAsync();
						return;
					}
				} while (true);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				Environment.Exit(-1);
			}
		}
	}
}