using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogRobotDownloadedRequest_Tencent : ILogRobotDownloadedRequest, IServiceRequest<LogRobotDownloadedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogRobotDownloadedDependency _dependency;

		public void Inject(LogRobotDownloadedDependency dependency)
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
