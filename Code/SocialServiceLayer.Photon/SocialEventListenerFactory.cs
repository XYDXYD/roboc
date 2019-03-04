using Services;

namespace SocialServiceLayer.Photon
{
	internal class SocialEventListenerFactory : EventListenerFactory, ISocialEventListenerFactory, IEventListenerFactory
	{
		public SocialEventListenerFactory()
		{
			Bind<IAllFriendsOfflineEventListener, AllFriendsOfflineEventListener>();
			Bind<IFriendAcceptEventListener, FriendAcceptEventListener>();
			Bind<IFriendCancelledEventListener, FriendCancelledEventListener>();
			Bind<IFriendInviteEventListener, FriendInviteEventListener>();
			Bind<IFriendRemovedEventListener, FriendRemovedEventListener>();
			Bind<IFriendStatusEventListener, FriendStatusEventListener>();
			Bind<IFriendDeclineEventListener, FriendDeclineEventListener>();
			Bind<IKickedFromPlatoonEventListener, KickedFromPlatoonEventListener>();
			Bind<IPlatoonDisbandedEventListener, PlatoonDisbandedEventListener>();
			Bind<IPlatoonInviteCancelledEventListener, PlatoonInviteCancelledEventListener>();
			Bind<IPlatoonLeaderChangedEventListener, PlatoonLeaderChangedEventListener>();
			Bind<IPlatoonMemberAddedEventListener, PlatoonMemberAddedEventListener>();
			Bind<IPlatoonMemberLeftEventListener, PlatoonMemberLeftEventListener>();
			Bind<IPlatoonMemberStatusChangedEventListener, PlatoonMemberStatusChangedEventListener>();
			Bind<IPlatoonInviteEventListener, PlatoonInviteEventListener>();
			Bind<IPlatoonChangedEventListener, PlatoonChangedEventListener>();
			Bind<IPlatoonMemberAvatarChangedEventListener, PlatoonMemberAvatarChangedEventListener>();
			Bind<IPlatoonRobotTierChangedEventListener, PlatoonRobotTierChangedEventListener>();
			Bind<IClanInviteReceivedEventListener, ClanInviteReceivedEventListener>();
			Bind<IClanMemberJoinedEventListener, ClanMemberJoinedEventListener>();
			Bind<IClanMemberLeftEventListener, ClanMemberLeftEventListener>();
			Bind<IRemovedFromClanEventListener, RemovedFromClanEventListener>();
			Bind<IClanInviteCancelledEventListener, ClanInviteCancelledEventListener>();
			Bind<IClanMemberDataChangedEventListener, ClanMemberDataChangedEventListener>();
			Bind<IClanDataChangedEventListener, ClanDataChangedEventListener>();
			Bind<IClanRenamedEventListener, ClanRenamedEventListener>();
			Bind<IClanMemberXPChangedEventListener, ClanMemberXPChangedEventListener>();
			Bind<IFriendClanChangedEventListener, FriendClanChangedEventListener>();
			Bind<IAvatarUpdatedEventListener, AvatarUpdatedEventListener>();
		}
	}
}
