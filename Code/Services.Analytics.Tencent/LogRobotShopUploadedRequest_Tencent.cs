using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogRobotShopUploadedRequest_Tencent : ILogRobotShopUploadedRequest, IServiceRequest<LogRobotShopUploadedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogRobotShopUploadedDependency _dependency;

		public void Inject(LogRobotShopUploadedDependency dependency)
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
