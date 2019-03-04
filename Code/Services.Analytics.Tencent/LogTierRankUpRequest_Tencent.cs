using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogTierRankUpRequest_Tencent : ILogTierRankUpRequest, IServiceRequest<LogTierRankUpDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogTierRankUpDependency _dependency;

		public void Inject(LogTierRankUpDependency dependency)
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
