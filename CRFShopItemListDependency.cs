using System.Collections.Generic;
using System.IO;

internal class CRFShopItemListDependency
{
	public const bool defaultPlayerFilter = false;

	public const bool defaultShowFeaturedRobots = false;

	public const bool defaultShowLockedRobots = true;

	public const bool defaultDevOnlyShowHiddenRobots = false;

	public const TextSearchField defaultTextSearchField = TextSearchField.ALL;

	public const ItemSortMode defaultItemSortMode = ItemSortMode.SUGGESTED;

	public uint page
	{
		get;
		set;
	}

	public uint pageSize
	{
		get;
		set;
	}

	public bool playerFilter
	{
		get;
		set;
	}

	public ItemSortMode sortMode
	{
		get;
		set;
	}

	public int minimumCpu
	{
		get;
		set;
	}

	public int maximumCpu
	{
		get;
		set;
	}

	public string textFilter
	{
		get;
		set;
	}

	public TextSearchField textSearchField
	{
		get;
		set;
	}

	public bool showFeaturedRobots
	{
		get;
		set;
	}

	public bool showLockedRobots
	{
		get;
		set;
	}

	public bool devOnlyShowHiddenRobots
	{
		get;
		set;
	}

	public bool noFiltersSelected
	{
		get;
		set;
	}

	public int minRobotRanking
	{
		get;
		set;
	}

	public int maxRobotRanking
	{
		get;
		set;
	}

	public int weaponFilter
	{
		get;
		set;
	}

	public int movementFilter
	{
		get;
		set;
	}

	public string movementCategoryGroups
	{
		get;
		set;
	}

	public string weaponCategoryGroups
	{
		get;
		set;
	}

	public Dictionary<uint, uint> cubeCounts
	{
		get;
		set;
	}

	public CRFShopItemListDependency()
	{
		page = 1u;
		pageSize = 1u;
		weaponFilter = 0;
		movementFilter = 0;
		weaponCategoryGroups = string.Empty;
		movementCategoryGroups = string.Empty;
		playerFilter = false;
		sortMode = ItemSortMode.SUGGESTED;
		minimumCpu = -1;
		maximumCpu = -1;
		textFilter = string.Empty;
		textSearchField = TextSearchField.ALL;
		showFeaturedRobots = false;
		showLockedRobots = true;
		devOnlyShowHiddenRobots = false;
		noFiltersSelected = true;
		minRobotRanking = -1;
		maxRobotRanking = -1;
	}

	public CRFShopItemListDependency(CRFShopItemListDependency source)
	{
		page = source.page;
		pageSize = source.pageSize;
		weaponCategoryGroups = source.weaponCategoryGroups;
		movementCategoryGroups = source.movementCategoryGroups;
		playerFilter = source.playerFilter;
		sortMode = source.sortMode;
		minimumCpu = source.minimumCpu;
		maximumCpu = source.maximumCpu;
		textFilter = source.textFilter;
		textSearchField = source.textSearchField;
		showFeaturedRobots = source.showFeaturedRobots;
		showLockedRobots = source.showLockedRobots;
		devOnlyShowHiddenRobots = source.devOnlyShowHiddenRobots;
		noFiltersSelected = source.noFiltersSelected;
		minRobotRanking = source.minRobotRanking;
		maxRobotRanking = source.maxRobotRanking;
	}

	public byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(page);
				binaryWriter.Write(pageSize);
				binaryWriter.Write(weaponFilter);
				binaryWriter.Write(movementFilter);
				binaryWriter.Write(weaponCategoryGroups);
				binaryWriter.Write(movementCategoryGroups);
				binaryWriter.Write(playerFilter);
				binaryWriter.Write((int)sortMode);
				binaryWriter.Write(minimumCpu);
				binaryWriter.Write(maximumCpu);
				binaryWriter.Write(minRobotRanking);
				binaryWriter.Write(maxRobotRanking);
				binaryWriter.Write(textFilter);
				binaryWriter.Write((int)textSearchField);
				binaryWriter.Write(showFeaturedRobots);
				binaryWriter.Write(devOnlyShowHiddenRobots);
				binaryWriter.Write(noFiltersSelected);
				return memoryStream.ToArray();
			}
		}
	}
}
