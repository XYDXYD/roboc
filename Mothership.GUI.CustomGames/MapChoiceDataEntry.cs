namespace Mothership.GUI.CustomGames
{
	internal struct MapChoiceDataEntry
	{
		public readonly string mapNameKey;

		public readonly string mapNameForDisplay;

		public MapChoiceDataEntry(string mapNameKey_, string mapNameForDisplay_)
		{
			mapNameKey = mapNameKey_;
			mapNameForDisplay = mapNameForDisplay_;
		}
	}
}
