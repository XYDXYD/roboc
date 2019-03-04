namespace Services.Analytics
{
	internal class LogSuccessfulLoginDependency
	{
		public AnalyticsLaunchMode launchMode
		{
			get;
			private set;
		}

		public AnalyticsLoginType loginType
		{
			get;
			private set;
		}

		public bool emailValidated
		{
			get;
			private set;
		}

		public LogSuccessfulLoginDependency(AnalyticsLaunchMode launchMode_, AnalyticsLoginType loginType_, bool emailValidated_)
		{
			launchMode = launchMode_;
			loginType = loginType_;
			loginType = loginType_;
			emailValidated = emailValidated_;
		}
	}
}
