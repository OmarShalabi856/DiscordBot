using Discord;
using DiscordBot.Services;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class Scrapper:IScrapper
	{
		public readonly IScrapperSetup _browserSetup;
		public static readonly string matchListElementSelector=".qa-match-block";
		public Scrapper(IScrapperSetup scrapperSetup)
		{
			_browserSetup = scrapperSetup;
		}

		public async Task<List<Game>> LaunchScrape(string url)
		{
			using(var browser = await _browserSetup.LaunchBrowserAsync())
			using(var page = await browser.NewPageAsync())
			{
				await page.GoToAsync(url);
				List<string> teams = [];
				var matchBlock=await page.QuerySelectorAsync(matchListElementSelector);

				IElementHandle[]? matchListElements = await matchBlock
					.QuerySelectorAsync("ul")?
					.GetAwaiter().GetResult()?
					.QuerySelectorAllAsync("li");

				List<Game> games = new List<Game>();
				int awayTeamScore, homeTeamScore = 0;
				string awayTeamName, homeTeamName = "";
				foreach (var match in matchListElements)
				{
					 awayTeamScore = 0; homeTeamScore = 0;
					 awayTeamName = ""; homeTeamName = "";

					MatchType matchType = await GetMatchType(match);
					IElementHandle? awayTeamData = await GetTeamElement(match,TeamType.Home, matchType);
					IElementHandle? homeTeamData = await GetTeamElement(match,TeamType.Away, matchType);
					
					if(awayTeamData==null || homeTeamData == null)
					{
						continue;
					}

					(awayTeamScore, awayTeamName) = await GetTeamData(awayTeamData,matchType);
					(homeTeamScore, homeTeamName) = await GetTeamData(homeTeamData, matchType);
					string matchStatus = await GetMatchStatus(match, matchType);
					Game game = new Game(homeTeamName, awayTeamName, homeTeamScore, awayTeamScore, matchStatus,matchType);
					games.Add(game);	
				}
				return games;
			}

			
		}

		private async Task<string> GetMatchStatus(IElementHandle match, MatchType matchType)
		{
			try
			{
				MatchElementSelectors matchElementSelectors = CreateMatchElementSelectos(matchType);
				var matchStatus = await match.QuerySelectorAsync(matchElementSelectors.Status)
					.EvaluateFunctionAsync<string>("node=>node.textContent");
				return matchStatus;


			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return "N/A";
			}
		}

		private async Task<MatchType> GetMatchType(IElementHandle match)
		{
			try
			{
				var liveMatchElement = await match.QuerySelectorAsync(TestMatchType.liveMatchElement);
				var finishedMatchElement = await match.QuerySelectorAsync(TestMatchType.finishedMatchElement);
				var upcomingMatchElement = await match.QuerySelectorAsync(TestMatchType.upcomingMatchElement);

				if (liveMatchElement != null)
				{
					return MatchType.Live;
				}
				else if (finishedMatchElement != null)
				{
					return MatchType.Finished;
				}
				else if (upcomingMatchElement != null)
				{
					return MatchType.Upcoming;
				}
				return MatchType.Unknown;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				return MatchType.Unknown;
			}
		}

		private async Task<IElementHandle?>  GetTeamElement(IElementHandle match, TeamType teamType,MatchType matchType)
		{
			try
			{
				MatchElementSelectors matchElementSelectors = CreateMatchElementSelectos(matchType);

				var teamElementField = TeamType.Away == teamType ? matchElementSelectors.AwayTeamElement : matchElementSelectors.HomeTeamElement;

				var teamElement = await match.QuerySelectorAsync(teamElementField);

				return teamElement;

			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
				return null;
			}
		}

		private MatchElementSelectors CreateMatchElementSelectos(MatchType matchType)
		{
			return matchType switch
			{
				MatchType.Live => new LiveMatchElementSelectors(),
				MatchType.Finished => new FinishedMatchElementSelectors(),
				MatchType.Upcoming => new UpcomingMatchElementSelectors(),
			};
		}

		private async Task<(int TeamScore, string TeamName)> GetTeamData(IElementHandle teamData, MatchType matchType)
		{
			try
			{
				MatchElementSelectors matchElementSelectors = CreateMatchElementSelectos(matchType);
				var teamName = await teamData.QuerySelectorAsync(matchElementSelectors.TeamName)
					.GetAwaiter().GetResult()?
					.EvaluateFunctionAsync<string>("node=>node.textContent");

				int score = 0;
				var teamScoreSpan = await teamData.QuerySelectorAsync(matchElementSelectors.TeamScoreSpan);
				if (teamScoreSpan != null)
				{
					score = await teamScoreSpan?
					.QuerySelectorAsync(matchElementSelectors.TeamNameScore)
					.GetAwaiter().GetResult()?
					.EvaluateFunctionAsync<int>("node=>node.textContent");
				}
				
				return (score, teamName);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (0, "");
			}
		}
	}
	public enum TeamType
	{
		Away,Home
	}
	public enum MatchType
	{
		Finished,Live,Upcoming,Unknown
	}
	public class Game
	{
		public string Id { get; set; }
		public string firstTeam { get; set; }
		public string SecondTeam { get; set; }

		public int firstTeamScore { get; set; }
		public int secondTeamScore { get; set; }

		public string status { get; set; }

		public MatchType matchType { get; set; }

		public DateTime startDateTime { get; set; }


		public Game(string firstTeam, string secondTeam, int firstTeamScore, int secondTeamScore, string status,MatchType matchType)
		{
			this.firstTeam = firstTeam;
			this.SecondTeam = secondTeam;
			this.firstTeamScore = firstTeamScore;
			this.secondTeamScore = secondTeamScore;
			this.status = status;
			this.matchType = matchType;
			Id = $"{firstTeam}{secondTeam}";
		}

		public override string ToString()
		{
			switch (matchType)
			{
				case MatchType.Finished:
					return $"{firstTeam} {firstTeamScore}-{secondTeamScore} {SecondTeam} ---- Match Status: {MatchType.Finished}\n";

				case MatchType.Live:
					return $"{firstTeam} {firstTeamScore}-{secondTeamScore} {SecondTeam}---- Time: {status} ---- Match Status: {MatchType.Live}\n";

				case MatchType.Upcoming:
					return $"{firstTeam} {status} {SecondTeam} ---- Match Status: {MatchType.Upcoming}\n";

				default:
					return "Invalid Match Type\n";
			}
		}

	}
}
