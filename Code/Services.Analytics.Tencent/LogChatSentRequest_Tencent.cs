using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogChatSentRequest_Tencent : ILogChatSentRequest, IServiceRequest<LogChatSentDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogChatSentDependency _dependency;

		public void Inject(LogChatSentDependency dependency)
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
