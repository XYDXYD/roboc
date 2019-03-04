using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogCollectedSeasonRewardRequest_Tencent : ILogCollectedSeasonRewardRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private int _robits;

		public void Inject(int robits)
		{
			_robits = robits;
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
