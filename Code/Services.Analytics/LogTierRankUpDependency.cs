namespace Services.Analytics
{
	internal class LogTierRankUpDependency
	{
		public uint tier
		{
			get;
			private set;
		}

		public string rank
		{
			get;
			private set;
		}

		public LogTierRankUpDependency(uint tier_, string rank_)
		{
			tier = tier_;
			rank = rank_;
		}
	}
}
