using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogChatJoinedRequest_Tencent : ILogChatJoinedRequest, IServiceRequest<ChatChannelType>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private ChatChannelType _chatChannelType;

		public void Inject(ChatChannelType chatChannelType)
		{
			_chatChannelType = chatChannelType;
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
