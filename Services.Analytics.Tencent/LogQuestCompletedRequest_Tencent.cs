using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogQuestCompletedRequest_Tencent : ILogQuestCompletedRequest, IServiceRequest<LogQuestCompletedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogQuestCompletedDependency _dependency;

		public void Inject(LogQuestCompletedDependency dependency)
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
