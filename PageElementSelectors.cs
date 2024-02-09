using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class MatchElementSelectors
	{
		public string HomeTeamElement { get; set; }
		public string AwayTeamElement { get; set; }
		public string TeamName { get; set; }
		public string TeamScoreSpan { get; set; } = "empty";
		public string TeamNameScore { get; set; }="empty";
		public string Status { get; set; }
	}
	public class LiveMatchElementSelectors : MatchElementSelectors
	{
		public LiveMatchElementSelectors()
		{
			HomeTeamElement = ".sp-c-fixture__team--home";
			AwayTeamElement = ".sp-c-fixture__team--away";
			TeamName = ".qa-full-team-name";
			TeamScoreSpan = ".sp-c-fixture__block";
			TeamNameScore = ".sp-c-fixture__number";
			Status = ".sp-c-fixture__status--live-sport";
		}

	}

	public class FinishedMatchElementSelectors : MatchElementSelectors
	{
		public FinishedMatchElementSelectors()
		{
			HomeTeamElement = ".sp-c-fixture__team--home";
			AwayTeamElement = ".sp-c-fixture__team--away";
			TeamName = ".qa-full-team-name";
			TeamScoreSpan = ".sp-c-fixture__block";
			TeamNameScore = ".sp-c-fixture__number";
			Status = ".sp-c-fixture__status--ft";
		}
	}

	public class UpcomingMatchElementSelectors : MatchElementSelectors
	{

		public UpcomingMatchElementSelectors()
		{
			HomeTeamElement = ".sp-c-fixture__team--time-home";
			AwayTeamElement = ".sp-c-fixture__team--time-away";
			TeamName = ".qa-full-team-name";
			Status = ".sp-c-fixture__block--time";
		}

	}

	public static class TestMatchType
	{
		public static string upcomingMatchElement = ".sp-c-fixture__block--time";
		public static string liveMatchElement = ".sp-c-fixture__status--live-sport";
		public static string finishedMatchElement = ".sp-c-fixture__status--ft";
	}
}
