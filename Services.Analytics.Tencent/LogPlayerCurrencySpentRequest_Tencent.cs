using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerCurrencySpentRequest_Tencent : ILogPlayerCurrencySpentRequest, IServiceRequest<LogPlayerCurrencySpentDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerCurrencySpentDependency _dependency;

		public void Inject(LogPlayerCurrencySpentDependency dependency)
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
