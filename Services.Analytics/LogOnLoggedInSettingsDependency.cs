namespace Services.Analytics
{
	internal class LogOnLoggedInSettingsDependency
	{
		public string resolution
		{
			get;
			private set;
		}

		public string gfx
		{
			get;
			private set;
		}

		public bool fullScreen
		{
			get;
			private set;
		}

		public bool capFrameRateEnabled
		{
			get;
			private set;
		}

		public int capFrameRateAmount
		{
			get;
			private set;
		}

		public bool zoomMode
		{
			get;
			private set;
		}

		public bool invertY
		{
			get;
			private set;
		}

		public bool showCenterOfMass
		{
			get;
			private set;
		}

		public bool blockFriendClanInvites
		{
			get;
			private set;
		}

		public bool acceptFriendClanOnlyInvites
		{
			get;
			private set;
		}

		public string language
		{
			get;
			private set;
		}

		public string processorType
		{
			get;
			private set;
		}

		public int memorySize
		{
			get;
			private set;
		}

		public int shaderLevel
		{
			get;
			private set;
		}

		public LogOnLoggedInSettingsDependency(string resolution_, string gfx_, bool fullScreen_, bool capFrameRateEnabled_, int capFrameRateAmount_, bool zoomMode_, bool invertY_, bool showCenterOfMass_, bool blockFriendClanInvites_, bool acceptFriendClanOnlyInvites_, string language_, string processorType_, int memorySize_, int shaderLevel_)
		{
			resolution = resolution_;
			gfx = gfx_;
			fullScreen = fullScreen_;
			capFrameRateEnabled = capFrameRateEnabled_;
			capFrameRateAmount = capFrameRateAmount_;
			zoomMode = zoomMode_;
			invertY = invertY_;
			showCenterOfMass = showCenterOfMass_;
			blockFriendClanInvites = blockFriendClanInvites_;
			acceptFriendClanOnlyInvites = acceptFriendClanOnlyInvites_;
			language = language_;
			processorType = processorType_;
			memorySize = memorySize_;
			shaderLevel = shaderLevel_;
		}
	}
}
