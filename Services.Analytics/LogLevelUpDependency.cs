namespace Services.Analytics
{
	public class LogLevelUpDependency
	{
		public int level;

		public string abTest;

		public string abTestGroup;

		public LogLevelUpDependency(int level_, string abTest_, string abTestGroup_)
		{
			level = level_;
			abTest = abTest_;
			abTestGroup = abTestGroup_;
		}
	}
}
