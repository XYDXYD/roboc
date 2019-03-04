namespace Services.Analytics
{
	internal class LogCubeUnlockedDependency
	{
		public uint techPointsCost
		{
			get;
			private set;
		}

		public string cubeNameKey
		{
			get;
			private set;
		}

		public int techsUnlocked
		{
			get;
			private set;
		}

		public LogCubeUnlockedDependency(uint techPointsCost_, string cubeNameKey_, int techsUnlocked_)
		{
			techPointsCost = techPointsCost_;
			cubeNameKey = cubeNameKey_;
			techsUnlocked = techsUnlocked_;
		}
	}
}
