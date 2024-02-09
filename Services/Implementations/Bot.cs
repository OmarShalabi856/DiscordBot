using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using DiscordBot.Commands;

namespace DiscordBot.Services.Implementations
{
	internal class Bot : IBot
	{
		private  ServiceProvider? _serviceProvider;
		
		private readonly ILogger<Bot> _logger;
		private readonly IConfiguration _configuration;
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;

		public Bot(
			ILogger<Bot> logger,
			IConfiguration configuration,
			DiscordSocketClient client,
			CommandService commands)
		{
			_logger = logger;
			_configuration = configuration;
			_client = client;
			_commands = commands;
		}
		public async Task StartAsync(ServiceProvider services)
		{
			string _discordToken = _configuration["DiscordToken"] ?? throw new Exception("Missing token");
			_serviceProvider = services;

			await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

			await _client.LoginAsync(TokenType.Bot, _discordToken);
			await _client.StartAsync();

			_client.MessageReceived += HandleCommandAsync;

			GetMatchScoresCommand matchCommands =
				new GetMatchScoresCommand(services.GetRequiredService<IScrapper>(),
												services.GetRequiredService<DiscordSocketClient>(),
												services.GetRequiredService<IConfiguration>()
											);

			while (true)
			{
				
				await Task.Run(async () =>
				{
					await matchCommands.GetMatchScores('1');
				});
				await Task.Delay(60000);
			}
		}

		public  async Task StopAsync()
		{
			if(_client != null)
			{
				await _client.LogoutAsync();
				await _client.StopAsync();	
			}
		}

		private async Task HandleCommandAsync(SocketMessage arg)
		{

			if (arg is not SocketUserMessage message || message.Author.IsBot)
			{
				return;
			}
			_logger.LogInformation($"{DateTime.Now.ToShortTimeString()} - {message.Author}: {message.Content}");

			int position = 0;
			bool messageIsCommand = message.HasCharPrefix('!', ref position);

			if (messageIsCommand)
			{

				await _commands.ExecuteAsync(
					new SocketCommandContext(_client, message),
					position,
					_serviceProvider);
			}
		}

		
	}
}
