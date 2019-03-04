namespace Services.Analytics
{
	internal class LogPlayerXpEarnedDependency
	{
		public int earned
		{
			get;
			private set;
		}

		public int userXP
		{
			get;
			private set;
		}

		public int? roboPassXP
		{
			get;
			private set;
		}

		public int userLevel
		{
			get;
			private set;
		}

		public int premiumBonus
		{
			get;
			private set;
		}

		public string source
		{
			get;
			private set;
		}

		public string sourceDetail
		{
			get;
			private set;
		}

		public LogPlayerXpEarnedDependency(int earned_, int userXP_, int? roboPassXP_, int userLevel_, int premiumBonus_, string source_, string sourceDetail_)
		{
			earned = earned_;
			userXP = userXP_;
			roboPassXP = roboPassXP_;
			userLevel = userLevel_;
			premiumBonus = premiumBonus_;
			source = source_;
			sourceDetail = sourceDetail_;
		}
	}
}
