using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogFriendAcceptInviteRequest_Tencent : ILogFriendAcceptInviteRequest, IServiceRequest, IAnswerOnComplete
	{
		private IServiceAnswer _answer;

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
