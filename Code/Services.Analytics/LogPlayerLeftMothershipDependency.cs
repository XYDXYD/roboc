namespace Services.Analytics
{
	internal class LogPlayerLeftMothershipDependency
	{
		public string levelName
		{
			get;
			private set;
		}

		public string gameModeType
		{
			get;
			private set;
		}

		public int duration
		{
			get;
			private set;
		}

		public LogPlayerLeftMothershipDependency(string levelName_, string gameModeType_, int duration_)
		{
			levelName = levelName_;
			gameModeType = gameModeType_;
			duration = duration_;
		}
	}
}
