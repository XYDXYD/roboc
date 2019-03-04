using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogRobotControlsChangedRequest_Tencent : ILogRobotControlsChangedRequest, IServiceRequest<LogRobotControlsChangedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogRobotControlsChangedDependency _dependency;

		public void Inject(LogRobotControlsChangedDependency dependency)
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
