namespace Services.Analytics
{
	internal class LogPlayerRoboPassGradeUpDependency
	{
		public string season
		{
			get;
			private set;
		}

		public int grade
		{
			get;
			private set;
		}

		public LogPlayerRoboPassGradeUpDependency(string season_, int grade_)
		{
			season = season_;
			grade = grade_;
		}
	}
}
