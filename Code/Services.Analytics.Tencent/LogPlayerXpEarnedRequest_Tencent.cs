using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerXpEarnedRequest_Tencent : ILogPlayerXpEarnedRequest, IServiceRequest<LogPlayerXpEarnedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerXpEarnedDependency _dependency;

		public void Inject(LogPlayerXpEarnedDependency dependency)
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
