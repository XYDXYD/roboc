namespace Services.Analytics
{
	internal class LogFrameRateDependency
	{
		public string gameContext
		{
			get;
			private set;
		}

		public int avgFPS
		{
			get;
			private set;
		}

		public int sdFPS
		{
			get;
			private set;
		}

		public LogFrameRateDependency(string gameContext_, int avgFPS_, int sdFPS_)
		{
			gameContext = gameContext_;
			avgFPS = avgFPS_;
			sdFPS = sdFPS_;
		}
	}
}
