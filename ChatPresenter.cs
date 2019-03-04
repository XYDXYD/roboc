using ChatServiceLayer;
using Mothership;
using Robocraft.GUI;
using ServerStateServiceLayer;
using ServerStateServiceLayer.EventListeners.Photon;
using Services.Analytics;
using Services.Web.Photon;
using Simulation;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.WeakEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal abstract class ChatPresenter : IWaitForFrameworkDestruction, IChatMessageReceiver, IWaitForFrameworkInitialization, IFloatingWidget
{
	private const string PLAYER_PREF_EXPANDED = "ChatExpanded";

	private string CustomGameId;

	private readonly bool _shouldDisplayMemberEvents;

	private IDataSource channelDataSource;

	protected IChatChannel CurrentChannel;

	protected ChatView _view;

	private IChatClient _client;

	private Action _onReconnected;

	private AccountRights _accountRights;

	private IServiceEventContainer _chatEventContainer;

	private IServiceEventContainer _socialEventContainer;

	private IServiceEventContainer _serviceEventContainer;

	private bool _chatIsFocused;

	private bool _chatIsExpanded;

	private string _channelFilter;

	private bool _chatWasFocusedBeforeDisable;

	protected ChatStyle _chatStyle;

	private ShortCutMode _previousShortcutMode;

	private bool _isFirstConnection;

	internal string PlatoonId
	{
		get;
		private set;
	}

	internal string ClanName
	{
		get;
		private set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		get;
		set;
	}

	[Inject]
	internal ChatCommands chatCommands
	{
		get;
		set;
	}

	[Inject]
	internal IgnoreList ignoreList
	{
		private get;
		set;
	}

	[Inject]
	internal ChatChannelContainer chatChannelContainer
	{
		get;
		set;
	}

	[Inject]
	internal PrivateChat privateChat
	{
		get;
		set;
	}

	[Inject]
	internal ChatClientProvider chatClientProvider
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ChatSettings chatSettings
	{
		get;
		set;
	}

	[Inject]
	internal ProfanityFilter profanityFilter
	{
		get;
		set;
	}

	[Inject]
	internal IChatEventContainerFactory ChatEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ChatAudio audio
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialRequestFactory SocialRequestFactory
	{
		get;
		set;
	}

	[Inject]
	internal ISocialEventContainerFactory SocialEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IServerStateEventContainerFactory ServerStateEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IAnalyticsRequestFactory analyticsRequestFactory
	{
		private get;
		set;
	}

	public bool ChatInitialised
	{
		get;
		private set;
	}

	public static ChatColours ChatColours
	{
		get;
		private set;
	}

	protected ChatPresenter(bool shouldDisplayMemberEvents = true)
	{
		_shouldDisplayMemberEvents = shouldDisplayMemberEvents;
	}

	protected abstract void Initialize();

	protected abstract void TearDown();

	protected abstract string GetChatLocation();

	public void OnFrameworkInitialized()
	{
		_client = chatClientProvider.GetClient();
		_isFirstConnection = !_client.IsConnected();
	}

	public IEnumerator InitializeInFlow()
	{
		Console.Log("Chat Presenter Initialization");
		if (ChatColours == null)
		{
			ChatColours = (ChatColours)Resources.Load("ChatColours");
			ChatColours.Initialise();
		}
		if (!_client.IsConnected())
		{
			IChatClient client = _client;
			client.onConnected += (Action)InitialiseOnConnection;
		}
		else
		{
			if (_isFirstConnection)
			{
				InitialiseOnConnection();
			}
			else
			{
				InitialiseWithPreviousConnection();
			}
			_isFirstConnection = false;
		}
		GetAccountRights(delegate(AccountRights rights)
		{
			_accountRights = rights;
		});
		_chatEventContainer = ChatEventContainerFactory.Create();
		_chatEventContainer.ListenTo<IPlayerJoinedChatRoomEventListener, PlayerJoinedChatRoomData>(OnPlayerJoinedChatRoom);
		_chatEventContainer.ListenTo<IPlayerLeftChatRoomEventListener, PlayerLeftChatRoomData>(OnPlayerLeftChatRoom);
		profanityFilter.LoadDataBase("neutral");
		_socialEventContainer = SocialEventContainerFactory.Create();
		_socialEventContainer.ListenTo<IClanRenamedEventListener, ClanRenameDependency>(OnClanRenamed);
		_socialEventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(PlatoonChanged);
		_socialEventContainer.ListenTo<IRemovedFromClanEventListener>(OnRemovedFromClan);
		_serviceEventContainer = ServerStateEventContainerFactory.Create();
		_serviceEventContainer.ListenTo<ICustomGameDeclinedInvitationEventListener, DeclineInviteToSessionData>(OnDeclinedCustomGameInvitation);
		guiInputController.OnScreenStateChange += UpdateChatStyle;
		yield return chatSettings.Load(serviceRequestFactory);
		_view.SetVisible(chatSettings.IsChatEnabled());
		Initialize();
	}

	public void SetView(ChatView aView)
	{
		_view = aView;
		ChatBuffer.Instance.SetReceiver(this);
		if (ChatInitialised)
		{
			foreach (ChatMessage message in ChatBuffer.Instance.GetMessages())
			{
				MessageReceived(message, this);
			}
		}
	}

	public void InitializeView()
	{
		_chatIsFocused = false;
		OnFocusLost();
		_view.DeepBroadcast(ChatGUIEvent.Type.UpdateChannelList);
		_chatIsExpanded = GetInitialExpanding();
		UpdateVisibleExpanding();
	}

	public void SetChannelDataSource(IDataSource ds)
	{
		channelDataSource = ds;
	}

	internal void Tick()
	{
		if (InputRemapper.Instance.GetButtonDown("PlatoonChatShortcut"))
		{
			if (PlatoonId != null)
			{
				SelectPlatoonChannel();
			}
			else
			{
				SystemMessage(Localization.Get("strNotInPlatoon", true));
			}
		}
	}

	public void ClearView(ChatView aView)
	{
		if (_view == aView)
		{
			_view = null;
			ChatBuffer.Instance.RemoveReceiver(this);
		}
	}

	public void OnGUIEvent(object obj)
	{
		if (obj is ChatGUIEvent)
		{
			ChatGUIEvent chatGUIEvent = (ChatGUIEvent)obj;
			switch (chatGUIEvent.type)
			{
			case ChatGUIEvent.Type.SendMessage:
				ProcessInput(chatGUIEvent.text);
				if (_chatStyle == ChatStyle.Battle)
				{
					SetFocused(focused: false);
				}
				break;
			case ChatGUIEvent.Type.SetFilter:
				SetChannelFilter(chatGUIEvent.channel);
				break;
			case ChatGUIEvent.Type.ClearFilter:
				ClearChannelFilter();
				break;
			case ChatGUIEvent.Type.ToggleExpand:
				OnAskToggleExpand();
				break;
			case ChatGUIEvent.Type.GoToNextChannel:
				if (string.IsNullOrEmpty(_channelFilter))
				{
					SelectNextChannel(forward: true, excudeCurrent: false);
				}
				break;
			case ChatGUIEvent.Type.AskFocus:
				SetFocused(focused: true);
				break;
			case ChatGUIEvent.Type.AskUnfocus:
				SetFocused(focused: false);
				break;
			}
		}
		else
		{
			if (!(obj is SocialMessage))
			{
				return;
			}
			SocialMessage socialMessage = (SocialMessage)obj;
			switch (socialMessage.messageDispatched)
			{
			case SocialMessageType.SocialViewDisabled:
				_chatWasFocusedBeforeDisable = _chatIsFocused;
				SetFocused(focused: false);
				break;
			case SocialMessageType.SocialViewEnabled:
				if (chatSettings.IsChatEnabled())
				{
					_view.SetVisible(visible: true);
					SetFocused(_chatWasFocusedBeforeDisable);
				}
				else
				{
					_view.SetVisible(visible: false);
				}
				break;
			case SocialMessageType.ClickedOutsideSocial:
				SetFocused(focused: false);
				break;
			}
		}
	}

	public bool IsChatFocused()
	{
		return _chatIsFocused;
	}

	public void SetFocused(bool focused)
	{
		if (focused != _chatIsFocused)
		{
			_chatIsFocused = focused;
			OnFocusChange();
			if (_chatIsFocused)
			{
				audio.EnterChatMode();
			}
			else
			{
				audio.LeaveChatMode();
			}
		}
	}

	protected virtual bool GetInitialExpanding()
	{
		return PlayerPrefs.GetInt("ChatExpanded") != 0;
	}

	public void ApplyStyle()
	{
		switch (_chatStyle)
		{
		case ChatStyle.Default:
			if (_chatIsExpanded)
			{
				_view.ApplyGarageMaximised();
			}
			else
			{
				_view.ApplyGarageMinimised();
			}
			break;
		case ChatStyle.BuildMode:
			if (_chatIsExpanded)
			{
				_view.ApplyBuildModeMaximised();
			}
			else
			{
				_view.ApplyBuildModeMinimised();
			}
			break;
		case ChatStyle.BattleQueue:
			if (_chatIsExpanded)
			{
				_view.ApplyBattleQueueModeMaximised();
			}
			else
			{
				_view.ApplyBattleQueueModeMinimised();
			}
			break;
		}
	}

	private void UpdateChatStyle()
	{
		switch (guiInputController.GetActiveScreen())
		{
		case GuiScreens.Garage:
			_chatStyle = ChatStyle.Default;
			ApplyStyle();
			break;
		case GuiScreens.PrebuiltRobotScreen:
		case GuiScreens.BuildMode:
			_chatStyle = ChatStyle.BuildMode;
			ApplyStyle();
			break;
		case GuiScreens.BattleCountdown:
			_chatStyle = ChatStyle.BattleQueue;
			ApplyStyle();
			break;
		case GuiScreens.BattleStatsScreen:
		{
			HudStyle battleHudStyle = guiInputController.GetControllerForScreen(GuiScreens.BattleStatsScreen).battleHudStyle;
			if (battleHudStyle == HudStyle.HideAllButChat)
			{
				_chatStyle = ChatStyle.EndGame;
				_view.ApplyEndBattleStyle();
			}
			break;
		}
		case GuiScreens.CustomGameScreen:
			_chatStyle = ChatStyle.CustomGame;
			_view.ApplyCustomGameStyle();
			break;
		}
		UpdateFocusStyle();
		_view.DeepBroadcast(ChatGUIEvent.Type.StyleChanged, _chatStyle);
	}

	public virtual void OnFrameworkDestroyed()
	{
		if (_chatEventContainer != null)
		{
			_chatEventContainer.Dispose();
			_chatEventContainer = null;
		}
		if (_socialEventContainer != null)
		{
			_socialEventContainer.Dispose();
			_socialEventContainer = null;
		}
		if (_serviceEventContainer != null)
		{
			_serviceEventContainer.Dispose();
			_serviceEventContainer = null;
		}
		if (_client.IsConnected())
		{
			TearDown();
			foreach (IChatChannel item in new List<IChatChannel>(chatChannelContainer.GetAllChannels()))
			{
				chatChannelContainer.DestroyChannel(item);
			}
			IChatClient client = _client;
			client.onConnected -= (Action)InitialiseOnConnection;
			IChatClient client2 = _client;
			client2.onUnexpectedDisconnection = (Action)Delegate.Remove(client2.onUnexpectedDisconnection, new Action(DisconnectedFromServer));
			IChatClient client3 = _client;
			client3.onUnexpectedDisconnection = (Action)Delegate.Remove(client3.onUnexpectedDisconnection, new Action(ReconnectedToServer));
			ChatBuffer.Instance.RemoveReceiver(this);
		}
		else
		{
			Console.LogWarning("Chat already disconnected. Skipping TearDown");
		}
	}

	public void MessageReceived(ChatMessage message, object sender)
	{
		if (!ChatInitialised)
		{
			return;
		}
		InboundChatMessage inboundChatMessage = message as InboundChatMessage;
		if (inboundChatMessage != null)
		{
			InboundChatMessage inboundChatMessage2 = inboundChatMessage;
			string channelName = inboundChatMessage2.ChannelName;
			ChatChannelType chatChannelType = inboundChatMessage2.ChatChannelType;
			if (chatChannelContainer.GetChannel(channelName, chatChannelType).Suspended)
			{
				return;
			}
		}
		DisplayMessage(message);
	}

	private void DisplayMessage(ChatMessage message)
	{
		bool flag = false;
		if (!(message is IIgnorableChatMessage))
		{
			flag = true;
		}
		else if (!ignoreList.ShouldIgnore((message as IIgnorableChatMessage).SenderDisplayName))
		{
			flag = true;
		}
		if (flag)
		{
			if (message.ShouldFilterProfanity)
			{
				message.SetText(profanityFilter.FilterString(message.RawText));
			}
			_view.DeepBroadcast(ChatGUIEvent.Type.MessageReceived, message);
		}
		if (message is IncomingPrivateChatMessage)
		{
			IncomingPrivateChatMessage incomingPrivateChatMessage = message as IncomingPrivateChatMessage;
			if (!chatChannelContainer.PrivateChannelExists(incomingPrivateChatMessage.SenderName))
			{
				chatChannelContainer.CreatePrivateChannel(incomingPrivateChatMessage.SenderName, incomingPrivateChatMessage.SenderDisplayName);
			}
			privateChat.LastReceivedWhisperFrom = incomingPrivateChatMessage.SenderName;
			privateChat.LastReceivedWhisperFromDisplayName = incomingPrivateChatMessage.SenderDisplayName;
		}
	}

	public void SendOutgoingMessage(string messageToSend, string messageToShowLocal = null, IChatChannel channel = null)
	{
		if (messageToShowLocal == null)
		{
			messageToShowLocal = messageToSend;
		}
		if (IsConnected())
		{
			SendOutgoingMessageWithConnection(messageToSend, messageToShowLocal, channel);
			return;
		}
		_onReconnected = (Action)Delegate.Combine(_onReconnected, (Action)delegate
		{
			SendOutgoingMessageWithConnection(messageToSend, messageToShowLocal, channel);
		});
		_client.Connect(OnFailedToConnect);
	}

	public void SetChannelFilter(IChatChannel channel)
	{
		if (!chatChannelContainer.HasJoinedChannel(channel))
		{
			throw new Exception("That channel has not been joined");
		}
		_channelFilter = channel.VisibleName;
		SetActiveChannel(channel);
	}

	public void ClearChannelFilter()
	{
		_channelFilter = null;
	}

	public void SelectFirstChannel()
	{
		chatChannelContainer.GetDefaultChannel(SetActiveChannel);
	}

	public void LogLocalMessage(ChatMessage localMessage)
	{
		ChatBuffer.Instance.AddMessage(localMessage);
	}

	public void SendWhisper(string recipient, string displayName, string messageText)
	{
		OutgoingPrivateChatMessage message = new OutgoingPrivateChatMessage(recipient, displayName, messageText, GetChatLocation());
		privateChat.SendMessage(message);
		if (!chatChannelContainer.PrivateChannelExists(recipient))
		{
			chatChannelContainer.CreatePrivateChannel(recipient, displayName);
		}
		TaskRunner.get_Instance().Run(HandleChatSentAnalytics(ChatChannelType.Private, messageText.Length));
	}

	public void JoinChannel(string channelName, ChatChannelType channelType, string password = null)
	{
		chatChannelContainer.JoinChannel(channelName, channelType, password, OnChatChannelJoined);
	}

	public void LeaveChannel(string channelName, ChatChannelType channelType)
	{
		IChatChannel channel = chatChannelContainer.GetChannel(channelName, channelType);
		if (channel != null && channel.ChannelName == _channelFilter)
		{
			ClearChannelFilter();
			_view.DeepBroadcast(ChatGUIEvent.Type.ClearFilter);
		}
		chatChannelContainer.LeaveChannel(channel);
		if (CurrentChannel.VisibleName.Equals(channelName, StringComparison.OrdinalIgnoreCase) || (CurrentChannel.ChatChannelType == channelType && CurrentChannel.ChatChannelType == ChatChannelType.Clan))
		{
			SelectNextChannel(forward: true, excudeCurrent: true);
		}
		EventMessage(Localization.Get("strLeft", true), channel.VisibleName, channel.ChatChannelType);
	}

	public void CreateOrSelectWhisperChannel(string receiver, string displayName)
	{
		if (!chatChannelContainer.PrivateChannelExists(receiver))
		{
			chatChannelContainer.CreatePrivateChannel(receiver, displayName);
		}
		SetActiveChannel(chatChannelContainer.GetChannel(receiver, ChatChannelType.Private));
	}

	internal void SelectClanChannel()
	{
		SetActiveChannel(chatChannelContainer.GetClanChannel());
	}

	internal void SelectPlatoonChannel()
	{
		SetActiveChannel(chatChannelContainer.GetPlatoonChannel());
	}

	public void HandleQuitPressed()
	{
		SetFocused(focused: false);
	}

	protected virtual void OnFocusChange()
	{
		if (_chatIsFocused)
		{
			OnFocusGained();
			guiInputController.AddFloatingWidget(this);
		}
		else
		{
			OnFocusLost();
			guiInputController.RemoveFloatingWidget(this);
		}
	}

	private void OnFocusGained()
	{
		_view.BubbleTargetedDispatch(ChatGUIEvent.Type.Focus);
		UpdateFocusStyle();
		_view.DeepBroadcast(ChatGUIEvent.Type.Focus);
		UpdateVisibleExpanding();
	}

	private void OnFocusLost()
	{
		_view.DeepBroadcast(ChatGUIEvent.Type.Unfocus);
		UpdateFocusStyle();
		UpdateVisibleExpanding();
		_view.BubbleTargetedDispatch(ChatGUIEvent.Type.Unfocus);
	}

	protected virtual void ReconnectedToServer()
	{
		chatChannelContainer.ReconnectedToServer(OnResubscribed);
		if (PlatoonId != null)
		{
			JoinChannel(PlatoonId, ChatChannelType.Platoon);
		}
		if (ClanName != null)
		{
			JoinChannel(ClanName, ChatChannelType.Clan);
		}
		if (CustomGameId != null)
		{
			JoinChannel(CustomGameId, ChatChannelType.CustomGame);
		}
		IChatClient client = _client;
		client.onUnexpectedDisconnection = (Action)Delegate.Combine(client.onUnexpectedDisconnection, new Action(DisconnectedFromServer));
		IChatClient client2 = _client;
		client2.onConnected -= (Action)ReconnectedToServer;
		_view.DeepBroadcast(ChatGUIEvent.Type.Connected);
	}

	protected virtual void DisconnectedFromServer()
	{
		SetActiveChannel(null);
		SetDisconnected();
		IChatClient client = _client;
		client.onConnected += (Action)ReconnectedToServer;
		IChatClient client2 = _client;
		client2.onUnexpectedDisconnection = (Action)Delegate.Remove(client2.onUnexpectedDisconnection, new Action(DisconnectedFromServer));
		TaskRunner.get_Instance().Run(ConnectAfterDelay());
		SystemMessage(Localization.Get("strConnectionChatServerLost", true));
	}

	internal void ProcessInput(string input)
	{
		input = input.Trim();
		if (input != string.Empty)
		{
			if (input.StartsWith("/"))
			{
				chatCommands.ProcessCommand(input.Substring(1));
			}
			else
			{
				SendOutgoingMessage(input);
			}
		}
	}

	internal void StartSendPrivateMessage(string receiver, string displayName)
	{
		SetFocused(focused: true);
		CreateOrSelectWhisperChannel(receiver, displayName);
	}

	internal void SetActiveChannel(IChatChannel channel)
	{
		CurrentChannel = channel;
		_view.DeepBroadcast(ChatGUIEvent.Type.SetChannel, channel);
	}

	internal void SystemMessage(string text)
	{
		ChatBuffer.Instance.AddMessage(new HelpMessage(text));
	}

	public void SocialMessage(string text)
	{
		ChatBuffer.Instance.AddMessage(new SocialEventMessage(text));
	}

	internal void EventMessage(string text, string channelName, ChatChannelType channelType)
	{
		ChatBuffer.Instance.AddMessage(new ChannelEventMessage(text, channelName, channelType));
	}

	internal void UnsubscribeChannel(IChatChannel channel)
	{
		chatChannelContainer.LeaveChannel(channel);
		if (CurrentChannel == channel)
		{
			SelectNextChannel(forward: true, excudeCurrent: true);
		}
	}

	internal ChatMessage[] GetBuffer()
	{
		return ChatBuffer.Instance.GetMessages().ToArray();
	}

	protected void SetCustomGameSession(string sessionId)
	{
		IChatChannel chatChannel = chatChannelContainer.TryGetChannelByType(ChatChannelType.CustomGame);
		if (sessionId != null)
		{
			CustomGameId = sessionId;
			bool flag = true;
			if (chatChannel != null)
			{
				if (chatChannel.ChannelName != sessionId)
				{
					UnsubscribeChannel(chatChannel);
				}
				else
				{
					flag = false;
				}
			}
			if (flag && ChatInitialised)
			{
				JoinChannel(sessionId, ChatChannelType.CustomGame);
			}
		}
		else
		{
			if (chatChannel != null)
			{
				UnsubscribeChannel(chatChannel);
			}
			CustomGameId = null;
		}
	}

	protected void RefreshPlatoonData(Action<Platoon> success)
	{
		IGetPlatoonDataRequest getPlatoonDataRequest = SocialRequestFactory.Create<IGetPlatoonDataRequest>();
		getPlatoonDataRequest.ForceRefresh();
		getPlatoonDataRequest.SetAnswer(new ServiceAnswer<Platoon>(success, OnRefreshPlatoonDataFail)).Execute();
	}

	protected void PlatoonChanged(Platoon platoon)
	{
		if (!_client.IsConnected())
		{
			return;
		}
		IChatChannel chatChannel = chatChannelContainer.TryGetPlatoonChannel();
		if (platoon.isInPlatoon)
		{
			bool flag = false;
			if (chatChannel == null)
			{
				flag = (platoon.GetAcceptedMemberCount() >= 2);
			}
			else if (platoon.platoonId != PlatoonId)
			{
				UnsubscribeChannel(chatChannel);
				chatChannel = null;
				flag = (platoon.GetAcceptedMemberCount() >= 2);
			}
			PlatoonId = platoon.platoonId;
			if (flag && ChatInitialised)
			{
				chatChannelContainer.JoinChannel(platoon.platoonId, ChatChannelType.Platoon, null, OnChatChannelJoined);
			}
		}
		else if (PlatoonId != null)
		{
			PlatoonId = null;
			if (chatChannel != null)
			{
				UnsubscribeChannel(chatChannel);
			}
		}
	}

	internal bool PlatoonChannelExists()
	{
		return chatChannelContainer.TryGetPlatoonChannel() != null;
	}

	internal bool ClanChannelExists()
	{
		return chatChannelContainer.TryGetClanChannel() != null;
	}

	internal int GetChatRoomCount()
	{
		return chatChannelContainer.Count;
	}

	internal bool IsConnected()
	{
		return _client != null && _client.IsConnected();
	}

	internal bool IsConnecting()
	{
		return _client != null && _client.IsConnecting();
	}

	internal bool CheckForProfanity(string inputString)
	{
		return profanityFilter.FilterString(inputString) != inputString;
	}

	protected virtual void OnPersistentChannelsJoined()
	{
		if (_view != null && IsConnected())
		{
			SelectFirstChannel();
		}
		ChatInitialised = true;
		if (_view != null)
		{
			foreach (ChatMessage message in ChatBuffer.Instance.GetMessages())
			{
				DisplayMessage(message);
			}
		}
		RefreshPlatoonData(PlatoonChanged);
		SocialRequestFactory.Create<IGetMyClanInfoRequest>().SetAnswer(new ServiceAnswer<ClanInfo>(OnClanInfoLoaded, OnLoadClanInfoFailed)).Execute();
		serviceRequestFactory.Create<IRetrieveCustomGameSessionRequest>().SetAnswer(new ServiceAnswer<RetrieveCustomGameSessionRequestData>(delegate(RetrieveCustomGameSessionRequestData session)
		{
			if (session.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				SetCustomGameSession(session.Data.SessionGUID);
			}
			else
			{
				SetCustomGameSession(null);
			}
		}, OnLoadCustomGameDataFail)).Execute();
	}

	internal void SelectNextChannel(bool forward, bool excudeCurrent)
	{
		SetActiveChannel(chatChannelContainer.GetNextChannel(CurrentChannel, excudeCurrent, forward));
	}

	internal void ClanChanged(string clanName)
	{
		if (ClanName == null)
		{
			if (!ClanChannelExists())
			{
				JoinChannel(clanName, ChatChannelType.Clan);
			}
		}
		else if (ClanName != clanName)
		{
			if (ClanChannelExists())
			{
				LeaveChannel(chatChannelContainer.GetClanChannel().ChannelName, ChatChannelType.Clan);
			}
			if (clanName != null)
			{
				JoinChannel(clanName, ChatChannelType.Clan);
			}
		}
		ClanName = clanName;
	}

	internal virtual void ChannelAddedToCollection(IChatChannel chatChannel)
	{
		channelDataSource.RefreshData();
		if (_view != null)
		{
			_view.DeepBroadcast(ChatGUIEvent.Type.UpdateChannelList);
		}
	}

	internal virtual void ChannelRemovedFromCollection()
	{
		channelDataSource.RefreshData();
		if (_view != null)
		{
			_view.DeepBroadcast(ChatGUIEvent.Type.UpdateChannelList);
		}
	}

	private void OnAskToggleExpand()
	{
		_chatIsExpanded = !_chatIsExpanded;
		UpdateVisibleExpanding();
		PlayerPrefs.SetInt("ChatExpanded", _chatIsExpanded ? 1 : 0);
	}

	private void UpdateVisibleExpanding()
	{
		ApplyStyle();
	}

	private void UpdateFocusStyle()
	{
		GameObject[] enabledElementsOnFocus = _view.enabledElementsOnFocus;
		for (int i = 0; i < enabledElementsOnFocus.Length; i++)
		{
			enabledElementsOnFocus[i].SetActive(_chatIsFocused || _chatStyle == ChatStyle.CustomGame);
		}
		if (_chatStyle == ChatStyle.EndGame)
		{
			_view.ApplyEndBattleStyle();
		}
	}

	private void OnResubscribed()
	{
		SelectFirstChannel();
		if (_onReconnected != null)
		{
			_onReconnected();
			_onReconnected = null;
		}
		SystemMessage(Localization.Get("strConnectionChatServerReestablished", true));
	}

	private IEnumerator ConnectAfterDelay()
	{
		yield return null;
		_client.Connect(null);
	}

	private void SetDisconnected()
	{
		_view.DeepBroadcast(ChatGUIEvent.Type.Disconnected, Localization.Get("strConnectionError", true));
	}

	private void OnFailedToConnect()
	{
		_onReconnected = null;
		SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrNoChatConnection"));
	}

	private void SendOutgoingMessageWithConnection(string outgoingText, string localText, IChatChannel chatChannel = null)
	{
		if (chatChannel == null)
		{
			if (chatChannelContainer.Count == 0)
			{
				SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrNoChannelsJoined"));
				return;
			}
			if (CurrentChannel == null)
			{
				throw new Exception("Current channel is null");
			}
			chatChannel = CurrentChannel;
		}
		chatChannel.SendMessage(new OutGoingChatMessage(outgoingText, localText, chatChannel.ChannelName, chatChannel.ChatChannelType, GetChatLocation()), new WeakAction<OutGoingChatMessage>((Action<OutGoingChatMessage>)OnMessageSent));
		TaskRunner.get_Instance().Run(HandleChatSentAnalytics(chatChannel.ChatChannelType, outgoingText.Length));
	}

	private void OnMessageSent(OutGoingChatMessage outGoingChatMessage)
	{
		LocalChatMessage localMessage = new LocalChatMessage(ChatMessage.GetLocalUserName(), ChatMessage.GetLocalDisplayName(), _accountRights.Developer, _accountRights.Moderator, _accountRights.Admin, outGoingChatMessage.LocalText, isPrivate: false, outGoingChatMessage.ChannelName, outGoingChatMessage.ChatChannelType);
		LogLocalMessage(localMessage);
		audio.MessageSent();
	}

	private void InitialiseOnConnection()
	{
		IChatClient client = _client;
		client.onUnexpectedDisconnection = (Action)Delegate.Combine(client.onUnexpectedDisconnection, new Action(DisconnectedFromServer));
		IChatClient client2 = _client;
		client2.onConnected -= (Action)InitialiseOnConnection;
		chatChannelContainer.SubscribeAllPersistentlyJoinedChannels(delegate
		{
			OnPersistentChannelsJoined();
			SystemMessage(Localization.Get("strWelcomeToRobocraft", true));
		});
	}

	private void InitialiseWithPreviousConnection()
	{
		IChatClient client = _client;
		client.onUnexpectedDisconnection = (Action)Delegate.Combine(client.onUnexpectedDisconnection, new Action(DisconnectedFromServer));
		IChatClient client2 = _client;
		client2.onConnected -= (Action)InitialiseOnConnection;
		chatChannelContainer.ResubscribeAllChannels(OnPersistentChannelsJoined);
	}

	private void OnClanInfoLoaded(ClanInfo clanInfo)
	{
		if (clanInfo != null)
		{
			ClanChanged(clanInfo.ClanName);
		}
	}

	private void OnLoadClanInfoFailed(ServiceBehaviour serviceBehaviour)
	{
		SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
		Console.LogError("Unable to load clan info " + StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
	}

	private void OnRefreshPlatoonDataFail(ServiceBehaviour serviceBehaviour)
	{
		SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
		Console.LogError("Unable to load platoon info " + StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
	}

	private void OnLoadCustomGameDataFail(ServiceBehaviour serviceBehaviour)
	{
		WebServicesErrorCode errorCode = (WebServicesErrorCode)serviceBehaviour.errorCode;
		Console.LogError("Unable to load custom game info " + StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
	}

	private void GetAccountRights(Action<AccountRights> onComplete)
	{
		serviceRequestFactory.Create<IGetAccountRightsRequest>().SetAnswer(new ServiceAnswer<AccountRights>(onComplete, delegate(ServiceBehaviour serviceBehaviour)
		{
			onComplete(new AccountRights(moderator: false, developer: false, admin: false));
			OnGetAccountRightsFail(serviceBehaviour);
		})).Execute();
	}

	private void OnGetAccountRightsFail(ServiceBehaviour serviceBehaviour)
	{
		RemoteLogger.Error(serviceBehaviour.errorTitle, serviceBehaviour.errorBody, null);
	}

	private void OnChatChannelJoined(ChatChannel channel)
	{
		SetActiveChannel(channel);
		ClearChannelFilter();
		_view.DeepBroadcast(ChatGUIEvent.Type.ClearFilter);
		EventMessage(StringTableBase<StringTable>.Instance.GetString("strJoined"), channel.VisibleName, channel.ChatChannelType);
		TaskRunner.get_Instance().Run(HandleChatJoinedAnalytics(channel.ChatChannelType));
	}

	private void OnPlayerJoinedChatRoom(PlayerJoinedChatRoomData data)
	{
		string channelName = data.ChannelName;
		IChatChannel channelUnknownType = chatChannelContainer.GetChannelUnknownType(channelName);
		if (channelUnknownType != null && !channelUnknownType.Suspended)
		{
			string playerName = data.PlayerName;
			if (ShouldDisplayMemberEvents(channelUnknownType))
			{
				ChatBuffer.Instance.AddMessage(new ChannelEventMessage(string.Format("{0} {1}", playerName, StringTableBase<StringTable>.Instance.GetString("strJoinedChatRoom")), channelUnknownType.VisibleName, channelUnknownType.ChatChannelType));
			}
		}
	}

	private bool ShouldDisplayMemberEvents(IChatChannel chatChannel)
	{
		if (!_shouldDisplayMemberEvents)
		{
			return false;
		}
		switch (chatChannel.ChatChannelType)
		{
		case ChatChannelType.None:
		case ChatChannelType.Private:
			throw new Exception("Received channel member event for invalid channel type " + chatChannel.ChatChannelType);
		case ChatChannelType.Public:
		case ChatChannelType.Battle:
		case ChatChannelType.BattleTeam:
			return false;
		case ChatChannelType.Platoon:
		case ChatChannelType.Custom:
		case ChatChannelType.Clan:
		case ChatChannelType.CustomGame:
			return true;
		default:
			throw new ArgumentOutOfRangeException("Unsupported channel type " + chatChannel.ChatChannelType);
		}
	}

	private void OnPlayerLeftChatRoom(PlayerLeftChatRoomData data)
	{
		string channelName = data.ChannelName;
		IChatChannel channelUnknownType = chatChannelContainer.GetChannelUnknownType(channelName);
		if (channelUnknownType != null && !channelUnknownType.Suspended)
		{
			string playerName = data.PlayerName;
			if (ShouldDisplayMemberEvents(channelUnknownType))
			{
				ChatBuffer.Instance.AddMessage(new ChannelEventMessage(string.Format("{0} {1}", playerName, Localization.Get("strLeftChatRoom", true)), channelUnknownType.VisibleName, channelUnknownType.ChatChannelType));
			}
		}
	}

	private void OnDeclinedCustomGameInvitation(DeclineInviteToSessionData data)
	{
		IChatChannel chatChannel = chatChannelContainer.TryGetChannelByType(ChatChannelType.CustomGame);
		if (chatChannel != null)
		{
			ChatBuffer.Instance.AddMessage(new ChannelEventMessage(string.Format("{0} {1}", data.PlayerWhoDeclined, StringTableBase<StringTable>.Instance.GetString("strCustomGameChatMessageDeclinedInvitation")), chatChannel.ChannelName, chatChannel.ChatChannelType));
		}
	}

	private void OnClanRenamed(ClanRenameDependency dependency)
	{
		ClanChanged(dependency.NewClanName);
	}

	private void OnRemovedFromClan()
	{
		ClanChanged(null);
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}

	private IEnumerator HandleChatSentAnalytics(ChatChannelType channelType, int totalCharacters)
	{
		LogChatSentDependency chatSentDependency = new LogChatSentDependency(channelType, totalCharacters);
		TaskService logChatSentRequest = analyticsRequestFactory.Create<ILogChatSentRequest, LogChatSentDependency>(chatSentDependency).AsTask();
		yield return logChatSentRequest;
		if (!logChatSentRequest.succeeded)
		{
			throw new Exception("Log Chat Sent Request failed", logChatSentRequest.behaviour.exceptionThrown);
		}
	}

	private IEnumerator HandleChatJoinedAnalytics(ChatChannelType channelType)
	{
		TaskService logChatJoinedRequest = analyticsRequestFactory.Create<ILogChatJoinedRequest, ChatChannelType>(channelType).AsTask();
		yield return logChatJoinedRequest;
		if (!logChatJoinedRequest.succeeded)
		{
			throw new Exception("Log Chat Joined Request failed", logChatJoinedRequest.behaviour.exceptionThrown);
		}
	}
}
