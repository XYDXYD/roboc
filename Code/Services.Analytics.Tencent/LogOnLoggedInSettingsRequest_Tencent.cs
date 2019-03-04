using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogOnLoggedInSettingsRequest_Tencent : ILogOnLoggedInSettingsRequest, IServiceRequest<LogOnLoggedInSettingsDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogOnLoggedInSettingsDependency _dependency;

		public void Inject(LogOnLoggedInSettingsDependency dependency)
		{
			_dependency = dependency;
		}

		public void Execute()
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed();
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer answer)
		{
			_answer = answer;
			return this;
		}
	}
}
