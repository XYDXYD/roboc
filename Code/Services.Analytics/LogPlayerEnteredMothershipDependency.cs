namespace Services.Analytics
{
	internal class LogPlayerEnteredMothershipDependency
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

		public uint robotCPU
		{
			get;
			private set;
		}

		public bool isCRFBot
		{
			get;
			private set;
		}

		public string controlType
		{
			get;
			private set;
		}

		public bool verticalStrafing
		{
			get;
			private set;
		}

		public int totalCosmetics
		{
			get;
			private set;
		}

		public bool sendShortVersion
		{
			get;
			private set;
		}

		public LogPlayerEnteredMothershipDependency(string levelName_, string gameModeType_, uint robotCPU_, bool isCRFBot_, string controlType_, bool verticalStrafing_, int totalCosmetics_)
		{
			levelName = levelName_;
			gameModeType = gameModeType_;
			robotCPU = robotCPU_;
			isCRFBot = isCRFBot_;
			controlType = controlType_;
			verticalStrafing = verticalStrafing_;
			totalCosmetics = totalCosmetics_;
			sendShortVersion = false;
		}

		public LogPlayerEnteredMothershipDependency(string levelName_, string gameModeType_)
		{
			levelName = levelName_;
			gameModeType = gameModeType_;
			sendShortVersion = true;
		}
	}
}
