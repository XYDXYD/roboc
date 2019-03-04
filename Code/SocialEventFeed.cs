using Authentication;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

internal class SocialEventFeed : IInitialize, IWaitForFrameworkDestruction
{
	private ReportSocialEventCommand _reportSocialEventCommand;

	private IServiceEventContainer _socialEventContainer;

	private IChatClient _client;

	[Inject]
	internal ICommandFactory commandFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialEventContainerFactory socialEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialRequestFactory socialRequestFactory
	{
		get;
		private set;
	}

	[Inject]
	internal ChatClientProvider chatClientProvider
	{
		private get;
		set;
	}

	public void OnDependenciesInjected()
	{
		_reportSocialEventCommand = commandFactory.Build<ReportSocialEventCommand>();
		_client = chatClientProvider.GetClient();
		if (!_client.IsConnected())
		{
			IChatClient client = _client;
			client.onConnected += (Action)ShowOncePerLoginNotifications;
		}
		_socialEventContainer = socialEventContainerFactory.Create();
		_socialEventContainer.ReconnectedEvent += OnReconnectedToSocial;
		_socialEventContainer.DisconnectedEvent += OnDisconnectedFromSocial;
		_socialEventContainer.ListenTo<IPlatoonMemberStatusChangedEventListener, string, PlatoonStatusChangedData>(HandlePartyMemberHasChangedState);
		_socialEventContainer.ListenTo<IPlatoonMemberLeftEventListener, string, PlatoonMember.MemberStatus>(HandlePartyMemberLeft);
		_socialEventContainer.ListenTo<IPlatoonLeaderChangedEventListener, string>(HandlePartyLeaderChanged);
		_socialEventContainer.ListenTo<IKickedFromPlatoonEventListener>(HandleLocalPlayerKickedFromParty);
		_socialEventContainer.ListenTo<IPlatoonInviteEventListener, PlatoonInvite>(HandleOnInvitedToParty);
		_socialEventContainer.ListenTo<IPlatoonDisbandedEventListener>(OnPlatoonDisbanded);
		_socialEventContainer.ListenTo<IFriendInviteEventListener, FriendListUpdate>(OnFriendInviteReceived);
		_socialEventContainer.ListenTo<IFriendStatusEventListener, Friend, IList<Friend>>(OnFriendStatusChanged);
		_socialEventContainer.ListenTo<IFriendAcceptEventListener, FriendListUpdate>(OnFriendAcceptEvent);
		_socialEventContainer.ListenTo<IClanInviteReceivedEventListener, ClanInvite, ClanInvite[]>(OnClanInviteReceived);
		_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(OnClanMemberDataChanged);
		_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnPlayerJoined);
		_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnPlayerLeft);
	}

	public void OnFrameworkDestroyed()
	{
		if (_socialEventContainer != null)
		{
			_socialEventContainer.Dispose();
			_socialEventContainer = null;
		}
	}

	private void ShowMessage(string message)
	{
		_reportSocialEventCommand.Inject(message);
		_reportSocialEventCommand.Execute();
	}

	private void ShowOncePerLoginNotifications()
	{
		IChatClient client = _client;
		client.onConnected -= (Action)ShowOncePerLoginNotifications;
		socialRequestFactory.Create<IGetClanInvitesRequest>().SetAnswer(new ServiceAnswer<ClanInvite[]>(OnClanInvitesLoaded, OnClanInvitesLoadFailed)).Execute();
		IGetFriendListRequest getFriendListRequest = socialRequestFactory.Create<IGetFriendListRequest>();
		getFriendListRequest.ForceRefresh = false;
		getFriendListRequest.SetAnswer(new ServiceAnswer<GetFriendListResponse>(OnFriendListDataLoaded, OnFriendListLoadFailed));
		getFriendListRequest.Execute();
	}

	private void OnFriendListDataLoaded(GetFriendListResponse friendResponse)
	{
		for (int i = 0; i < friendResponse.friendsList.Count; i++)
		{
			Friend friend = friendResponse.friendsList[i];
			if (friend.InviteStatus == FriendInviteStatus.InvitePending)
			{
				ShowMessage(Localization.Get("strPendingFriendInvite", true).Replace("{INVITER}", friend.DisplayName));
			}
		}
	}

	private void OnFriendListLoadFailed(ServiceBehaviour serviceBehaviour)
	{
		Console.Log("LOAD FRIEND LIST FAILED.");
	}

	private void OnDisconnectedFromSocial()
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetString("strSocialDisconnect"));
	}

	private void OnReconnectedToSocial()
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetString("strSocialReconnect"));
	}

	private void HandleOnInvitedToParty(PlatoonInvite platoonInvite)
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetReplaceString("strPartyInviteReceivedFrom", "{PLAYER}", platoonInvite.DisplayName));
	}

	private void OnPlatoonDisbanded()
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetString("strPartyDisbanded"));
	}

	private void HandlePartyMemberLeft(string playerDisplayName, PlatoonMember.MemberStatus status)
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetReplaceString("strPlayerLeftParty", "{PLAYER}", playerDisplayName));
	}

	private void HandlePartyMemberHasChangedState(string playerDisplayName, PlatoonStatusChangedData statusChangeData)
	{
		if (statusChangeData.oldStatus == PlatoonMember.MemberStatus.Invited && statusChangeData.newStatus == PlatoonMember.MemberStatus.Ready)
		{
			ShowMessage(StringTableBase<StringTable>.Instance.GetReplaceString("strPlayerJoinedParty", "{PLAYER}", playerDisplayName));
		}
	}

	private void HandleLocalPlayerKickedFromParty()
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetString("strYouHaveBeenRemovedFromTheParty"));
	}

	private void HandlePartyLeaderChanged(string newLeaderName)
	{
		if (newLeaderName != User.DisplayName)
		{
			ShowMessage(StringTableBase<StringTable>.Instance.GetReplaceString("strPlayerLeaderChanged", "{PLAYER}", newLeaderName));
		}
		else
		{
			ShowMessage(StringTableBase<StringTable>.Instance.GetString("strYouAreThePartyLeader"));
		}
	}

	private void OnFriendAcceptEvent(FriendListUpdate update)
	{
		ShowMessage(StringTableBase<StringTable>.Instance.GetReplaceString("strPlayerAcceptedFriendRequest", "{PLAYER}", update.displayName));
	}

	private void OnFriendInviteReceived(FriendListUpdate update)
	{
		ShowMessage(Localization.Get("strFriendnviteReceived", true).Replace("{INVITER}", update.displayName));
	}

	private void OnFriendStatusChanged(Friend friend, IList<Friend> friendList)
	{
		if (friend.IsOnline)
		{
			ShowMessage(Localization.Get("strFriendOnline", true).Replace("{USERNAME}", friend.DisplayName));
		}
		else
		{
			ShowMessage(Localization.Get("strFriendOffline", true).Replace("{USERNAME}", friend.DisplayName));
		}
	}

	private void OnClanInviteReceived(ClanInvite clanInvite, ClanInvite[] _)
	{
		ShowMessage(Localization.Get("strClanInviteReceived", true).Replace("{INVITER}", clanInvite.InviterDisplayName).Replace("{CLANNAME}", clanInvite.ClanName));
	}

	private void HandleOnPlayerJoined(ClanMember[] clanMembers, ClanMember newMember)
	{
		ShowMessage(Localization.Get("strPlayerJoinedClan", true).Replace("{USERNAME}", newMember.DisplayName));
	}

	private void HandleOnPlayerLeft(ClanMember[] clanMembers, ClanMember exMember)
	{
		if (exMember != null)
		{
			ShowMessage(Localization.Get("strPlayerLeftClan", true).Replace("{USERNAME}", exMember.DisplayName));
		}
	}

	private void OnClanInvitesLoaded(ClanInvite[] invites)
	{
		foreach (ClanInvite clanInvite in invites)
		{
			ShowMessage(Localization.Get("strPendingClanInvite", true).Replace("{INVITER}", clanInvite.InviterDisplayName).Replace("{CLANNAME}", clanInvite.ClanName));
		}
	}

	private void OnClanInvitesLoadFailed(ServiceBehaviour serviceBehaviour)
	{
		Console.LogError("Error loading clan invites to display in chat");
	}

	private void OnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent eventContent)
	{
		if (!eventContent.UserName.Equals(User.DisplayName) && eventContent.IsOnline.HasValue)
		{
			if (eventContent.IsOnline == true)
			{
				ShowMessage(Localization.Get("strClanMemberOnline", true).Replace("{USERNAME}", eventContent.UserName));
			}
			else
			{
				ShowMessage(Localization.Get("strClanMemberOffline", true).Replace("{USERNAME}", eventContent.UserName));
			}
		}
	}
}
