using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogRobotShopDownloadedRequest_Tencent : ILogRobotShopDownloadedRequest, IServiceRequest<LogRobotShopDownloadedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogRobotShopDownloadedDependency _dependency;

		public void Inject(LogRobotShopDownloadedDependency dependency)
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
