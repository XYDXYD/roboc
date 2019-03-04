using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogFrameRateRequest_Tencent : ILogFrameRateRequest, IServiceRequest<LogFrameRateDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogFrameRateDependency _dependency;

		public void Inject(LogFrameRateDependency dependency)
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
