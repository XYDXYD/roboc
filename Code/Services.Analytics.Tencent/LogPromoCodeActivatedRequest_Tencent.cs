using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPromoCodeActivatedRequest_Tencent : ILogPromoCodeActivatedRequest, IServiceRequest<LogPromoCodeActivatedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPromoCodeActivatedDependency _dependency;

		public void Inject(LogPromoCodeActivatedDependency dependency)
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
