using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class SubscribeAllJoinedChannelsRequest : ChatRequest<IEnumerable<ChatChannelInfo>>, ISubscribeAllJoinedChannelsRequest, IServiceRequest, IAnswerOnComplete<IEnumerable<ChatChannelInfo>>
	{
		protected override byte OperationCode => 11;

		public SubscribeAllJoinedChannelsRequest()
			: base("strErrorSubscribingToChatChannelsTitle", "strErrorSubscribingToChatChannelsBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override IEnumerable<ChatChannelInfo> ProcessResponse(OperationResponse response)
		{
			if (response.Parameters.ContainsKey(18))
			{
				Hashtable[] array = (Hashtable[])response.Parameters[18];
				List<ChatChannelInfo> list = new List<ChatChannelInfo>(array.Length);
				Hashtable[] array2 = array;
				foreach (Hashtable val in array2)
				{
					Hashtable channelInfo = val;
					list.Add(ChatChannelInfo.Deserialise(channelInfo));
				}
				return list;
			}
			return new ChatChannelInfo[0];
		}
	}
}
