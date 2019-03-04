namespace Services.Analytics
{
	internal class LogRobotControlsChangedDependency
	{
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

		public bool sidewaysDriving
		{
			get;
			private set;
		}

		public bool tracksTurnOnSpot
		{
			get;
			private set;
		}

		public LogRobotControlsChangedDependency(string controlType_, bool verticalStrafing_, bool sidewaysDriving_, bool tracksTurnOnSpot_)
		{
			controlType = controlType_;
			verticalStrafing = verticalStrafing_;
			sidewaysDriving = sidewaysDriving_;
			tracksTurnOnSpot = tracksTurnOnSpot_;
		}
	}
}
