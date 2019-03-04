using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerEnteredMothershipRequest_Tencent : ILogPlayerEnteredMothershipRequest, IServiceRequest<LogPlayerEnteredMothershipDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerEnteredMothershipDependency _dependency;

		public void Inject(LogPlayerEnteredMothershipDependency dependency)
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
