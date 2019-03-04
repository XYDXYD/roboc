using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogItemShopVisitedRequest_Tencent : ILogItemShopVisitedRequest, IServiceRequest<LogItemShopVisitedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogItemShopVisitedDependency _dependency;

		public void Inject(LogItemShopVisitedDependency dependency)
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
