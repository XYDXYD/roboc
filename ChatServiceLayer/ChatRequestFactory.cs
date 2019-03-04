using ChatServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal class ChatRequestFactory : ServiceRequestFactory, IChatRequestFactory, IServiceRequestFactory
	{
		public ChatRequestFactory()
		{
			AddRelation<ISendPrivateMessageRequest, SendPrivateMessageRequest, PrivateMessageDependency>();
			AddRelation<IAddOrUpdateSanctionRequest, AddOrUpdateSanctionRequest, AddOrUpdateSanctionDependency>();
			AddRelation<IAcknowledgeSanctionRequest, AcknowledgeSanctionRequest, Sanction>();
			AddRelation<ICheckPendingSanctionRequest, CheckPendingSanctionsRequest>();
			AddRelation<IGetChatIgnoresRequest, GetChatIgnoresRequest>();
			AddRelation<IIgnoreUserRequest, IgnoreUserRequest, string>();
			AddRelation<IUnignoreUserRequest, UnignoreUserRequest, string>();
			AddRelation<IJoinChatChannelRequest, JoinChatChannelRequest, CreateOrJoinChatChannelDependency>();
			AddRelation<ILeaveChatChannelRequest, LeaveChatChannelRequest, ChannelNameAndType>();
			AddRelation<ISubscribeAllJoinedChannelsRequest, SubscribeAllJoinedChannelsRequest>();
			AddRelation<ICreateChatChannelRequest, CreateChatChannelRequest, CreateOrJoinChatChannelDependency>();
			AddRelation<IGetAllPublicChannelNamesRequest, GetAllPublicChannelNamesRequest>();
			AddRelation<IGetAllSubscribedChannelsRequest, GetAllSubscribedChannelsRequest>();
			AddRelation<IGetCanSendPrivateMessageRequest, GetCanSendPrivateMessageRequest, string>();
		}
	}
}
