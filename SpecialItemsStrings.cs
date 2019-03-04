using System.Collections.Generic;

internal static class SpecialItemsStrings
{
	public const string MEGASHIP = "Megaship";

	public const string NAME = "Name";

	public const string DESCRIPTION = "Description";

	public const string STAT1 = "Stat1Title";

	public const string STAT1TITLE = "Stat1";

	public const string STAT2 = "Stat2Title";

	public const string STAT2TITLE = "Stat2";

	public const string STAT3 = "Stat3Title";

	public const string STAT3TITLE = "Stat3";

	public const string STAT4 = "Stat4Title";

	public const string STAT4TITLE = "Stat4";

	public const string STAT5 = "Stat5Title";

	public const string STAT5TITLE = "Stat5";

	public const string STAT6 = "Stat6";

	public const string STAT6TITLE = "Stat6";

	private static Dictionary<string, string> _dictionary;

	public static Dictionary<string, string> dictionary => _dictionary;

	static SpecialItemsStrings()
	{
		_dictionary = new Dictionary<string, string>
		{
			{
				"MegashipName",
				"specialMegashipName"
			},
			{
				"MegashipDescription",
				"specialMegashipDesc"
			},
			{
				"MegashipStat1",
				"strSize"
			},
			{
				"MegashipStat1Title",
				"specialMegashipUnits"
			},
			{
				"MegashipStat2",
				"strCapacity"
			},
			{
				"MegashipStat2Title",
				"specialMegashipArmourCubes"
			},
			{
				"MegashipStat3",
				"strMass"
			},
			{
				"MegashipStat3Title",
				"specialMegashipTons"
			}
		};
	}
}
