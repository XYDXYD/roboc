using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

internal sealed class SocialSettings : IInitialize
{
	private const string ACCEPTING_FRIEND_INVITES = "accepting_friend_invites";

	private const bool ACCEPTING_FRIEND_INVITES_DEFAULT_VALUE = true;

	private const string ACCEPT_PARTY_INVITES_FRIENDS_CLAN_ONLY = "friends_only_platoon_invites";

	private const bool ACCEPT_PARTY_INVITES_FRIENDS_CLAN_ONLY_DEFAULT = false;

	private bool _acceptPartyInvitesFromFriendsAndClanOnly;

	private bool _blockFriendInvites;

	private bool _settingsLoaded;

	[Inject]
	public ISocialRequestFactory socialRequestFactory
	{
		private get;
		set;
	}

	public bool SettingsLoaded => _settingsLoaded;

	public event Action OnSettingsLoaded;

	void IInitialize.OnDependenciesInjected()
	{
		IServiceRequest serviceRequest = socialRequestFactory.Create<IGetSocialSettingsRequest>().SetAnswer(new ServiceAnswer<Dictionary<string, object>>(OnSocialSettingsLoaded, OnGetSocialSettingsFail));
		serviceRequest.Execute();
	}

	public bool GetAcceptPartyInvitesFromFriendsAndClanOnlySetting()
	{
		return _acceptPartyInvitesFromFriendsAndClanOnly;
	}

	public bool IsBlockFriendInvites()
	{
		return _blockFriendInvites;
	}

	public void ChangeSettings(bool acceptPartyInvitesFromFriendsAndClanOnly, bool blockFriendInvites)
	{
		_acceptPartyInvitesFromFriendsAndClanOnly = acceptPartyInvitesFromFriendsAndClanOnly;
		_blockFriendInvites = blockFriendInvites;
	}

	public void SaveSettings()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("friends_only_platoon_invites", _acceptPartyInvitesFromFriendsAndClanOnly);
		dictionary.Add("accepting_friend_invites", !_blockFriendInvites);
		IServiceRequest serviceRequest = socialRequestFactory.Create<ISetSocialSettingsRequest, Dictionary<string, object>>(dictionary).SetAnswer(new ServiceAnswer(OnSaveSettingFail));
		serviceRequest.Execute();
	}

	private void OnSocialSettingsLoaded(Dictionary<string, object> settings)
	{
		_settingsLoaded = true;
		bool flag = true;
		if (settings.ContainsKey("accepting_friend_invites") && settings["accepting_friend_invites"] != null)
		{
			flag = (bool)settings["accepting_friend_invites"];
		}
		_blockFriendInvites = !flag;
		if (settings.ContainsKey("friends_only_platoon_invites") && settings["friends_only_platoon_invites"] != null)
		{
			_acceptPartyInvitesFromFriendsAndClanOnly = (bool)settings["friends_only_platoon_invites"];
		}
		else
		{
			_acceptPartyInvitesFromFriendsAndClanOnly = false;
		}
		if (this.OnSettingsLoaded != null)
		{
			this.OnSettingsLoaded();
		}
	}

	private void OnGetSocialSettingsFail(ServiceBehaviour behaviour)
	{
		RemoteLogger.Error(behaviour.errorBody, null, null);
		Console.LogError(behaviour.errorBody);
	}

	private void OnSaveSettingFail(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}
}
