using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerRoboPassGradeUpRequest_Tencent : ILogPlayerRoboPassGradeUpRequest, IServiceRequest<LogPlayerRoboPassGradeUpDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerRoboPassGradeUpDependency _dependency;

		public void Inject(LogPlayerRoboPassGradeUpDependency dependency)
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
