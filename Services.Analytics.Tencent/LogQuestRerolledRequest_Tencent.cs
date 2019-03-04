using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogQuestRerolledRequest_Tencent : ILogQuestRerolledRequest, IServiceRequest<LogQuestRerolledDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogQuestRerolledDependency _dependency;

		public void Inject(LogQuestRerolledDependency dependency)
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
