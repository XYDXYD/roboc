using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerRoboPassRewardCollectedRequest_Tencent : ILogPlayerRoboPassRewardCollectedRequest, IServiceRequest<LogPlayerRoboPassRewardCollectedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerRoboPassRewardCollectedDependency _dependency;

		public void Inject(LogPlayerRoboPassRewardCollectedDependency dependency)
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
