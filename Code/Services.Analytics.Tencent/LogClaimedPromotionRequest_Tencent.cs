using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogClaimedPromotionRequest_Tencent : ILogClaimedPromotionRequest, IServiceRequest<LogClaimedPromotionDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogClaimedPromotionDependency _dependency;

		public void Inject(LogClaimedPromotionDependency dependency)
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
