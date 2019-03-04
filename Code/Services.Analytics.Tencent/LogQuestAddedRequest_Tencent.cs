using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogQuestAddedRequest_Tencent : ILogQuestAddedRequest, IServiceRequest<LogQuestAddedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogQuestAddedDependency _dependency;

		public void Inject(LogQuestAddedDependency dependency)
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
