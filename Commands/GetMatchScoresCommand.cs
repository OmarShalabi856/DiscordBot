using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
	public class GetMatchScoresCommand:ModuleBase<SocketCommandContext>
	{
		public readonly IScrapper _scrapper;
		public readonly DiscordSocketClient _client;
		public readonly IConfiguration _configuration;

		private List<Game> Games;
		public string LastFullScrapeDate;
		public bool ScrapeIntialGameSchedule;
		public GetMatchScoresCommand(IScrapper scrapper,DiscordSocketClient client,IConfiguration configuration)
		{
			_scrapper = scrapper;
			_configuration = configuration;
			_client = client;
			Games = [];
			LastFullScrapeDate = DateTime.Today.ToString();
			ScrapeIntialGameSchedule = true;
		}
		[Command("MatchScore")]
		[Summary("Gets Match Score")]
		public async Task GetMatchScores([Remainder]char index='0')
		{
			if(DateTime.Today> DateTime.Parse(LastFullScrapeDate) || Games.Count()==0)
			{
				ScrapeIntialGameSchedule = true;
			}
			string leagueName = await GetLeagueName(index);
			if (leagueName == "") return;

			string matchDataSource = $"https://www.bbc.com/sport/football/{leagueName}/scores-fixtures";
			var games=await _scrapper.LaunchScrape(matchDataSource);
			games = GetGamesWithUpdates(games);


			string results = "";
			foreach(Game game in games)
			{
				results += game.ToString();
			}

			try
			{
				await ReplyAsync(results);
			}
			catch
			{
				ulong channelId = _configuration.GetSection("Channel:ChannelId").Get<ulong>();
				var channel = _client.GetChannel(channelId) as IMessageChannel;
				if (results == "")
					return;
				_ = await channel.SendMessageAsync(results);
			}
			
		}

		private List<Game> GetGamesWithUpdates(List<Game> games)
		{
			List<Game> gamesWithUpdates = new List<Game>();

			if(ScrapeIntialGameSchedule)
			{
				games.ForEach(g => Games.Add(g));
				LastFullScrapeDate = DateTime.Today.ToString();
				ScrapeIntialGameSchedule = false;
				return gamesWithUpdates;
			}
			foreach (var game in games)
			{
				var existingGame = Games.FirstOrDefault(g => g.Id == game.Id);

				if (existingGame != null && HasGameChanged(existingGame, game))
				{
					gamesWithUpdates.Add(game);
					Games.Remove(existingGame);
					Games.Add(game);
				}
				else if (existingGame == null)
				{
					Games.Add(game);
				}
			}

			return gamesWithUpdates;
		}

		private bool HasGameChanged(Game existingGame, Game newGame)
		{
			return existingGame.firstTeamScore != newGame.firstTeamScore ||
				   existingGame.secondTeamScore != newGame.secondTeamScore ||
				   existingGame.matchType != newGame.matchType;

		}

		private async Task<string> GetLeagueName(char index)
		{
			string leagueName = LeagueNames.LaLiga;

			if (char.IsDigit(index) && index >= '1' && index <= '5')
			{
				switch (index)
				{
					case '1':
						leagueName = LeagueNames.LaLiga;
						break;
					case '2':
						leagueName = LeagueNames.PremierLeague;
						break;
					case '3':
						leagueName = LeagueNames.SerieA;
						break;
					case '4':
						leagueName = LeagueNames.Bundesliga;
						break;
					case '5':
						leagueName = LeagueNames.Ligue1;
						break;
					default:
						break;

				}
				return leagueName;
			}
			else
			{
				await ReplyAsync("Please Enter The Number Of The League:" +
				"\n1-Spanish La Liga" +
				"\n2-English Premier League" +
				"\n3-Italian Serie A" +
				"\n4-German Bundesliga" +
				"\n5-French Ligue 1");

				return "";
			}
		}
	}

	public static class LeagueNames
	{
		public const string LaLiga = "spanish-la-liga";
		public const string PremierLeague = "premier-league";
		public const string SerieA = "italian-serie-a";
		public const string Ligue1 = "french-ligue-one";
		public const string Bundesliga = "german-bundesliga";
	}
}
