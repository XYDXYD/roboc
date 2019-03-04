using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerCurrencyEarnedRequest_Tencent : ILogPlayerCurrencyEarnedRequest, IServiceRequest<LogPlayerCurrencyEarnedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerCurrencyEarnedDependency _dependency;

		public void Inject(LogPlayerCurrencyEarnedDependency dependency)
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
