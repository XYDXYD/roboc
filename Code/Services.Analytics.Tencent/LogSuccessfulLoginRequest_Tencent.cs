using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogSuccessfulLoginRequest_Tencent : ILogSuccessfulLoginRequest, IServiceRequest<LogSuccessfulLoginDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogSuccessfulLoginDependency _dependency;

		public void Inject(LogSuccessfulLoginDependency dependency)
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
