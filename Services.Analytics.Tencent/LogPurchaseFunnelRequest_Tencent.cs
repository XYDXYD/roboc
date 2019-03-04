using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPurchaseFunnelRequest_Tencent : ILogPurchaseFunnelRequest, IServiceRequest<LogPurchaseFunnelDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPurchaseFunnelDependency _dependency;

		public void Inject(LogPurchaseFunnelDependency dependency)
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
