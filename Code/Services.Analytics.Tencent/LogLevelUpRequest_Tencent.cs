using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogLevelUpRequest_Tencent : ILogLevelUpRequest, IServiceRequest<LogLevelUpDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private int _level;

		public void Inject(LogLevelUpDependency level)
		{
			_level = level.level;
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
