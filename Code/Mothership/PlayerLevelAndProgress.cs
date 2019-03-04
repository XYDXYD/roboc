namespace Mothership
{
	internal class PlayerLevelAndProgress
	{
		public uint playerLevel;

		public float progressToNextLevel;

		public PlayerLevelAndProgress(uint playerLevel_, float fractionalProgressToNextLevel_)
		{
			playerLevel = playerLevel_;
			progressToNextLevel = fractionalProgressToNextLevel_;
		}
	}
}
