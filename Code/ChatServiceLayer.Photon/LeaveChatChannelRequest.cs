using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class LeaveChatChannelRequest : ChatRequest, ILeaveChatChannelRequest, IServiceRequest<ChannelNameAndType>, IAnswerOnComplete, IServiceRequest
	{
		private string _channelName;

		private ChatChannelType _channelType;

		protected override byte OperationCode => 5;

		public LeaveChatChannelRequest()
			: base("strErrorLeavingChatChannelTitle", "strErrorLeavingChatChannelBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[3] = _channelName;
			dictionary[1] = _channelType;
			OperationRequest val = new OperationRequest();
			val.Parameters = dictionary;
			return val;
		}

		public void Inject(ChannelNameAndType dependency)
		{
			_channelName = dependency.ChannelName;
			_channelType = dependency.ChatChannelType;
		}
	}
}
