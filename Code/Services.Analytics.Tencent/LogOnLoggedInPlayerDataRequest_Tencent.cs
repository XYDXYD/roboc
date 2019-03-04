using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogOnLoggedInPlayerDataRequest_Tencent : ILogOnLoggedInPlayerDataRequest, IServiceRequest<LogOnLoggedInPlayerDataDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogOnLoggedInPlayerDataDependency _dependency;

		public void Inject(LogOnLoggedInPlayerDataDependency dependency)
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
