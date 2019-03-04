using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogCubeUnlockedRequest_Tencent : ILogCubeUnlockedRequest, IServiceRequest<LogCubeUnlockedDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogCubeUnlockedDependency _dependency;

		public void Inject(LogCubeUnlockedDependency dependency)
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
