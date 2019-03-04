namespace Services.Analytics
{
	internal class LogOnLoggedInPlayerDataDependency
	{
		public int playerLevel
		{
			get;
			private set;
		}

		public uint playerXP
		{
			get;
			private set;
		}

		public long robitsBalance
		{
			get;
			private set;
		}

		public long ccBalance
		{
			get;
			private set;
		}

		public bool isDeveloper
		{
			get;
			private set;
		}

		public int totalFriends
		{
			get;
			private set;
		}

		public string clanName
		{
			get;
			private set;
		}

		public AnalyticsPremiumSubscriptionType premiumType
		{
			get;
			private set;
		}

		public int techsUnlocked
		{
			get;
			private set;
		}

		public int? roboPassXP
		{
			get;
			private set;
		}

		public bool? roboPassPlus
		{
			get;
			private set;
		}

		public string abTest
		{
			get;
			private set;
		}

		public string abTestGroupName
		{
			get;
			private set;
		}

		public LogOnLoggedInPlayerDataDependency(int playerLevel_, uint playerXP_, long robitsBalance_, long ccBalance_, bool isDeveloper_, int totalFriends_, string clanName_, AnalyticsPremiumSubscriptionType premiumType_, int techsUnlocked_, int? roboPassXP_, bool? roboPassPlus_, string abTest_, string abTestGroupName_)
		{
			playerLevel = playerLevel_;
			playerXP = playerXP_;
			robitsBalance = robitsBalance_;
			ccBalance = ccBalance_;
			isDeveloper = isDeveloper_;
			totalFriends = totalFriends_;
			clanName = clanName_;
			premiumType = premiumType_;
			techsUnlocked = techsUnlocked_;
			roboPassXP = roboPassXP_;
			roboPassPlus = roboPassPlus_;
			abTest = abTest_;
			abTestGroupName = abTestGroupName_;
		}
	}
}
