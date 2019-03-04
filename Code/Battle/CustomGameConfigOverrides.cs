namespace Battle
{
	internal class CustomGameConfigOverrides
	{
		public readonly bool CaptureSegmentMemory;

		public readonly int GameTimeMinutesOverride;

		public readonly int EliminationCaptureTimeValue;

		public CustomGameConfigOverrides(bool segmentMemorySetting, int gameTimeMins, int eliminationCaptureTimeValueSeconds)
		{
			CaptureSegmentMemory = segmentMemorySetting;
			GameTimeMinutesOverride = gameTimeMins;
			EliminationCaptureTimeValue = eliminationCaptureTimeValueSeconds;
		}
	}
}
