namespace SocialServiceLayer
{
	internal class SearchClanDependency
	{
		public readonly string SearchString;

		public readonly int DaysSinceActive;

		public readonly int StartRange;

		public readonly int EndRange;

		public readonly ClanType[] ClanTypes;

		public SearchClanDependency(string searchString, int daysSinceActive, int startRange, int endRange, ClanType[] clanTypes)
		{
			SearchString = searchString;
			DaysSinceActive = daysSinceActive;
			StartRange = startRange;
			EndRange = endRange;
			ClanTypes = clanTypes;
		}
	}
}
