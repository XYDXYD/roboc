using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogReconnectedRequest_Tencent : ILogReconnectedRequest, IServiceRequest<float>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private float _timeTaken;

		public void Inject(float timeTaken)
		{
			_timeTaken = timeTaken;
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
