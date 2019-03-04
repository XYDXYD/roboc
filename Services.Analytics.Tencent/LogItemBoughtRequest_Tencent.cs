using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogItemBoughtRequest_Tencent : ILogItemBoughtRequest, IServiceRequest<LogItemBoughtDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogItemBoughtDependency _dependency;

		public void Inject(LogItemBoughtDependency dependency)
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
