using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogRobotShopCollectedEarningsRequest_Tencent : ILogRobotShopCollectedEarningsRequest, IServiceRequest<LogRobotShopCollectedEarningsDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogRobotShopCollectedEarningsDependency _dependency;

		public void Inject(LogRobotShopCollectedEarningsDependency dependency)
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
