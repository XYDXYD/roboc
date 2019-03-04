using Svelto.ServiceLayer;

namespace Services.Web
{
	internal class SinglePlayer_SetRobotInGameRequest : ISetRobotInGameRequest, IServiceRequest<float>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private IServiceAnswer<bool> _answer;

		public void Inject(float dependency)
		{
		}

		public IServiceRequest SetAnswer(IServiceAnswer<bool> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj: true);
			}
		}
	}
}
