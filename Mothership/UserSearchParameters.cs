namespace Mothership
{
	internal sealed class UserSearchParameters
	{
		public ItemSortMode sortByMode;

		public uint movementTypeIndex;

		public uint weaponTypeIndex;

		public uint robotCPUIndex;

		public uint partCategoryIndex;

		public uint robotTierIndex;

		public bool showMyRobots;

		public bool showFeaturedOnly;

		public bool showLockedBots;

		public bool devOnlyShowHiddenRobots;

		public string textFilter;

		public TextSearchField textSearchField;

		public bool NoFiltersSelected => movementTypeIndex == 0 && weaponTypeIndex == 0 && robotCPUIndex == 0 && partCategoryIndex == 0 && robotTierIndex == 0 && !showMyRobots && !showFeaturedOnly && showLockedBots && !devOnlyShowHiddenRobots && textFilter == string.Empty;

		public UserSearchParameters()
		{
			sortByMode = ItemSortMode.SUGGESTED;
			movementTypeIndex = 0u;
			weaponTypeIndex = 0u;
			robotCPUIndex = 0u;
			partCategoryIndex = 0u;
			robotTierIndex = 0u;
			showMyRobots = false;
			showFeaturedOnly = false;
			showLockedBots = true;
			devOnlyShowHiddenRobots = false;
			textSearchField = TextSearchField.ALL;
			textFilter = string.Empty;
		}
	}
}
