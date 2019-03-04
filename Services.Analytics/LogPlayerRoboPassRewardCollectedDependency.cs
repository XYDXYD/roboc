namespace Services.Analytics
{
	internal class LogPlayerRoboPassRewardCollectedDependency
	{
		public string prize
		{
			get;
			private set;
		}

		public int amount
		{
			get;
			private set;
		}

		public string season
		{
			get;
			private set;
		}

		public bool roboPassPlus
		{
			get;
			private set;
		}

		public int grade
		{
			get;
			private set;
		}

		public LogPlayerRoboPassRewardCollectedDependency(string prize_, int amount_, string season_, bool roboPassPlus_, int grade_)
		{
			prize = prize_;
			amount = amount_;
			season = season_;
			roboPassPlus = roboPassPlus_;
			grade = grade_;
		}
	}
}
