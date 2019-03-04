using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogClanCreatedRequest_Tencent : ILogClanCreatedRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private string _clanName;

		public void Inject(string clanName)
		{
			_clanName = clanName;
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
