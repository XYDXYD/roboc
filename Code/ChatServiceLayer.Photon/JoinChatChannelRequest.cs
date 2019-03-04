using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using Utility;

namespace ChatServiceLayer.Photon
{
	internal class JoinChatChannelRequest : ChatRequest<ChatChannelInfo>, IJoinChatChannelRequest, IServiceRequest<CreateOrJoinChatChannelDependency>, IAnswerOnComplete<ChatChannelInfo>, IServiceRequest
	{
		private CreateOrJoinChatChannelDependency _dependency;

		public override bool isEncrypted => true;

		protected override byte OperationCode => 1;

		public JoinChatChannelRequest()
			: base("strErrorJoiningChatChannelTitle", "strErrorJoiningChatChannelBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[3] = _dependency.ChannelName;
			dictionary[1] = _dependency.ChannelType;
			if (_dependency.Password != null)
			{
				dictionary[16] = _dependency.Password;
			}
			OperationRequest val = new OperationRequest();
			val.Parameters = dictionary;
			return val;
		}

		public override void Execute()
		{
			Console.Log("Joining chat channel " + _dependency.ChannelName);
			base.Execute();
		}

		protected override ChatChannelInfo ProcessResponse(OperationResponse response)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			Console.Log("Received server response for joining chat channel " + _dependency.ChannelName);
			Hashtable channelInfo = response.Parameters[17];
			return ChatChannelInfo.Deserialise(channelInfo);
		}

		public void Inject(CreateOrJoinChatChannelDependency dependency)
		{
			_dependency = dependency;
		}
	}
}
