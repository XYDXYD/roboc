using System.Collections.Generic;

namespace Services.Web
{
	public class CustomGamesAllowedMapsData
	{
		public readonly Dictionary<GameModeType, List<string>> AllowedMaps;

		public readonly Dictionary<string, string> MapNameStrings;

		public CustomGamesAllowedMapsData(Dictionary<GameModeType, List<string>> allowedMaps_, Dictionary<string, string> mapNameStrings_)
		{
			AllowedMaps = new Dictionary<GameModeType, List<string>>(allowedMaps_);
			MapNameStrings = new Dictionary<string, string>(mapNameStrings_);
		}
	}
}
