using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerLeftMothershipRequest_Tencent : ILogPlayerLeftMothershipRequest, IServiceRequest<LogPlayerLeftMothershipDependency>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private LogPlayerLeftMothershipDependency _dependency;

		public void Inject(LogPlayerLeftMothershipDependency dependency)
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
