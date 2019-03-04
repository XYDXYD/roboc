using ChatServiceLayer;
using Services.Analytics;
using Services.Requests.Interfaces;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Utility;

internal class ChatChannelContainer : IInitialize, IEnumerable<IChatChannel>, IEnumerable
{
	private readonly Dictionary<ChannelNameAndType, IChatChannel> _channels = new Dictionary<ChannelNameAndType, IChatChannel>();

	private IChatClient _chatClient;

	[CompilerGenerated]
	private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

	[Inject]
	internal ChatClientProvider chatClientProvider
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IChatRequestFactory chatRequestFactory
	{
		get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
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

	public int Count => _channels.Count;

	void IInitialize.OnDependenciesInjected()
	{
		_chatClient = chatClientProvider.GetClient();
	}

	public IEnumerator<IChatChannel> GetEnumerator()
	{
		return _channels.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public ChatChannel RecreateChannel(string name, ChatChannelType type)
	{
		Console.Log("Creating generic chat channel");
		ChatChannelMember[] members = new ChatChannelMember[0];
		ChatChannel chatChannel = new ChatChannel(chatPresenter, _chatClient, name, type, members);
		AddChannelToCollection(chatChannel);
		return chatChannel;
	}

	public void LeaveChannel(IChatChannel channel)
	{
		Console.Log("Leaving channel " + channel.VisibleName);
		ILeaveChatChannelRequest leaveChatChannelRequest = chatRequestFactory.Create<ILeaveChatChannelRequest>();
		leaveChatChannelRequest.Inject(new ChannelNameAndType(channel.ChannelName, channel.ChatChannelType));
		leaveChatChannelRequest.SetAnswer(new ServiceAnswer(delegate
		{
			OnLeftChatChannel(channel);
		}, ErrorWindow.ShowServiceErrorWindow));
		leaveChatChannelRequest.Execute();
	}

	private void OnLeftChatChannel(IChatChannel channel)
	{
		RemoveChannelFromCollection(new ChannelNameAndType(channel.ChannelName, channel.ChatChannelType));
	}

	internal void DestroyChannel(IChatChannel channel)
	{
		RemoveChannelFromCollection(new ChannelNameAndType(channel.ChannelName, channel.ChatChannelType));
	}

	internal IChatChannel GetNextChannel(IChatChannel current, bool excludeCurrent, bool forward)
	{
		if (excludeCurrent && _channels.Count == 1)
		{
			return null;
		}
		bool flag = false;
		List<IChatChannel> list = new List<IChatChannel>();
		foreach (IChatChannel value in _channels.Values)
		{
			list.Add(value);
		}
		for (int i = 0; i < list.Count; i++)
		{
			int index = (!forward) ? (list.Count - (i + 1)) : i;
			IChatChannel chatChannel = list[index];
			if (chatChannel == current)
			{
				flag = true;
			}
			else if (!chatChannel.Suspended && flag)
			{
				return chatChannel;
			}
		}
		return GetFirstChannel();
	}

	internal void SubscribeAllPersistentlyJoinedChannels(Action onComplete)
	{
		chatRequestFactory.Create<ISubscribeAllJoinedChannelsRequest>().SetAnswer(new ServiceAnswer<IEnumerable<ChatChannelInfo>>(delegate(IEnumerable<ChatChannelInfo> channels)
		{
			OnGotChannelSubscriptions(channels, onComplete);
		}, OnFailedToSubscribeToChannels)).Execute();
	}

	internal void ResubscribeAllChannels(Action onComplete)
	{
		chatRequestFactory.Create<IGetAllSubscribedChannelsRequest>().SetAnswer(new ServiceAnswer<IEnumerable<ChatChannelInfo>>(delegate(IEnumerable<ChatChannelInfo> channels)
		{
			OnGotChannelSubscriptions(channels, onComplete);
		}, OnFailedToSubscribeToChannels)).Execute();
	}

	internal void ReconnectedToServer(Action onComplete)
	{
		SubscribeAllPersistentlyJoinedChannels(delegate
		{
			ResubscribeAllChannels(onComplete);
		});
	}

	public IEnumerable<IChatChannel> GetAllChannels()
	{
		return _channels.Values;
	}

	internal void CreateNewChannel(string channelName, string password, Action<ChatChannel> onSuccess)
	{
		ICreateChatChannelRequest createChatChannelRequest = chatRequestFactory.Create<ICreateChatChannelRequest>();
		createChatChannelRequest.Inject(new CreateOrJoinChatChannelDependency(channelName, ChatChannelType.Custom, password));
		createChatChannelRequest.SetAnswer(new ServiceAnswer(delegate
		{
			OnChatChannelCreated(channelName, onSuccess);
		}, OnCreateChannelFailed));
		createChatChannelRequest.Execute();
	}

	private void OnCreateChannelFailed(ServiceBehaviour serviceBehaviour)
	{
		ChatReturnCode errorCode = (ChatReturnCode)serviceBehaviour.errorCode;
		if (errorCode == ChatReturnCode.ChannelExists)
		{
			chatPresenter.SystemMessage(Localization.Get("strChannelAlreadyExists", true));
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}
	}

	internal void JoinChannel(string channelName, ChatChannelType channelType, string password, Action<ChatChannel> onSuccess)
	{
		if ((channelType == ChatChannelType.Custom || channelType == ChatChannelType.Public) && _channels.ContainsKey(new ChannelNameAndType(channelName, channelType)))
		{
			chatPresenter.SystemMessage(Localization.Get("strChannelAlreadyJoined", true));
			return;
		}
		IJoinChatChannelRequest joinChatChannelRequest = chatRequestFactory.Create<IJoinChatChannelRequest>();
		joinChatChannelRequest.Inject(new CreateOrJoinChatChannelDependency(channelName, channelType, password));
		joinChatChannelRequest.SetAnswer(new ServiceAnswer<ChatChannelInfo>(delegate(ChatChannelInfo channelInfo)
		{
			OnChatChannelJoined(channelInfo, onSuccess);
		}, OnFailedToJoinChannel));
		joinChatChannelRequest.Execute();
	}

	private void OnFailedToJoinChannel(ServiceBehaviour serviceBehaviour)
	{
		switch (serviceBehaviour.errorCode)
		{
		case 14:
			chatPresenter.SystemMessage(Localization.Get("strIncorrectPassword", true));
			break;
		case 16:
			chatPresenter.SystemMessage(Localization.Get("strChannelRequiresPassword", true));
			break;
		case 15:
			chatPresenter.SystemMessage(Localization.Get("strErrorChannelNotExists", true));
			break;
		case 17:
			chatPresenter.SystemMessage(Localization.Get("strChannelExpired", true));
			break;
		default:
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			break;
		}
	}

	internal void GetDefaultChannel(Action<IChatChannel> onSuccess)
	{
		TaskRunner.get_Instance().Run(LoadPlatformConfigurationValues(onSuccess));
	}

	internal IChatChannel GetFirstChannel()
	{
		foreach (IChatChannel value in _channels.Values)
		{
			if (!value.Suspended)
			{
				return value;
			}
		}
		return null;
	}

	internal IChatChannel GetChannel(string name)
	{
		foreach (IChatChannel value in _channels.Values)
		{
			if (value.ChannelName == name)
			{
				return value;
			}
		}
		return null;
	}

	internal bool PrivateChannelExists(string receiver)
	{
		return _channels.ContainsKey(new ChannelNameAndType(receiver, ChatChannelType.Private));
	}

	internal void CreatePrivateChannel(string receiver, string displayName)
	{
		AddChannelToCollection(new PrivateChatChannel(chatPresenter, receiver, displayName));
	}

	private void ChannelSubscribed(ChatChannelInfo channelInfo, Action<ChatChannel> onSuccess)
	{
		ChannelNameAndType channelNameAndType = new ChannelNameAndType(channelInfo.ChannelName, channelInfo.ChannelType);
		if (_channels.ContainsKey(channelNameAndType))
		{
			RemoveChannelFromCollection(channelNameAndType);
		}
		ChatChannel chatChannel = new ChatChannel(chatPresenter, _chatClient, channelInfo.ChannelName, channelInfo.ChannelType, channelInfo.Members);
		AddChannelToCollection(chatChannel);
		onSuccess?.Invoke(chatChannel);
	}

	private void OnChatChannelCreated(string channelName, Action<ChatChannel> onSuccess)
	{
		ChannelSubscribed(new ChatChannelInfo(channelName, ChatChannelType.Custom, null), onSuccess);
		TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
	}

	private void OnChatChannelJoined(ChatChannelInfo channelInfo, Action<ChatChannel> onSuccess)
	{
		ChannelSubscribed(channelInfo, onSuccess);
	}

	private void OnGotChannelSubscriptions(IEnumerable<ChatChannelInfo> channels, Action onComplete)
	{
		foreach (ChatChannelInfo channel in channels)
		{
			string channelName = channel.ChannelName;
			ChatChannelType channelType = channel.ChannelType;
			ChatChannelMember[] members = channel.Members;
			ChannelNameAndType key = new ChannelNameAndType(channelName, channelType);
			if (_channels.ContainsKey(key))
			{
				((ChatChannel)_channels[key]).SetMembers(members);
			}
			else
			{
				AddChannelToCollection(new ChatChannel(chatPresenter, _chatClient, channelName, channelType, members));
			}
		}
		onComplete?.Invoke();
	}

	private void AddChannelToCollection(IChatChannel chatChannel)
	{
		_channels.Add(new ChannelNameAndType(chatChannel.ChannelName, chatChannel.ChatChannelType), chatChannel);
		chatPresenter.ChannelAddedToCollection(chatChannel);
	}

	private void RemoveChannelFromCollection(ChannelNameAndType channelNameAndType)
	{
		_channels.Remove(channelNameAndType);
		chatPresenter.ChannelRemovedFromCollection();
	}

	private void OnFailedToSubscribeToChannels(ServiceBehaviour serviceBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}

	internal bool HasJoinedChannelUnknownType(string channelName)
	{
		return GetChannelUnknownType(channelName) != null;
	}

	internal bool HasJoinedChannel(IChatChannel channel)
	{
		return HasJoinedChannel(new ChannelNameAndType(channel.ChannelName, channel.ChatChannelType));
	}

	internal bool HasJoinedChannel(ChannelNameAndType channelNameAndType)
	{
		return _channels.ContainsKey(channelNameAndType);
	}

	internal IChatChannel GetPlatoonChannel()
	{
		IChatChannel chatChannel = TryGetPlatoonChannel();
		if (chatChannel == null)
		{
			throw new Exception("Platoon channel not found");
		}
		return chatChannel;
	}

	internal IChatChannel GetClanChannel()
	{
		IChatChannel chatChannel = TryGetClanChannel();
		if (chatChannel == null)
		{
			throw new Exception("Clan channel not found");
		}
		return chatChannel;
	}

	internal IChatChannel TryGetClanChannel()
	{
		return TryGetChannelByType(ChatChannelType.Clan);
	}

	internal IChatChannel TryGetPlatoonChannel()
	{
		return TryGetChannelByType(ChatChannelType.Platoon);
	}

	internal IChatChannel TryGetCustomGameChannel()
	{
		return TryGetChannelByType(ChatChannelType.CustomGame);
	}

	internal IChatChannel TryGetChannelByType(ChatChannelType type)
	{
		foreach (IChatChannel value in _channels.Values)
		{
			if (value.ChatChannelType == type)
			{
				return value;
			}
		}
		return null;
	}

	internal IChatChannel GetChannelUnknownType(string channelName)
	{
		using (Dictionary<ChannelNameAndType, IChatChannel>.KeyCollection.Enumerator enumerator = _channels.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ChannelNameAndType current = enumerator.Current;
				if (current.ChatChannelType != ChatChannelType.Private)
				{
					ChannelNameAndType current2 = enumerator.Current;
					if (current2.ChannelName.Equals(channelName, StringComparison.CurrentCultureIgnoreCase))
					{
						return _channels[enumerator.Current];
					}
				}
			}
		}
		return null;
	}

	internal IChatChannel GetChannel(string channelName, ChatChannelType channelType)
	{
		ChannelNameAndType key = new ChannelNameAndType(channelName, channelType);
		if (_channels.ContainsKey(key))
		{
			return _channels[key];
		}
		Console.LogWarning("No channel found named " + channelName);
		return null;
	}

	private IEnumerator LoadPlatformConfigurationValues(Action<IChatChannel> onSuccess)
	{
		loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		}, delegate
		{
			loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		}).GetEnumerator();
		loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		if (task.succeeded)
		{
			string autoJoinPublicChatRoom = task.result.AutoJoinPublicChatRoom;
			if (!string.IsNullOrEmpty(autoJoinPublicChatRoom))
			{
				onSuccess(GetChannel(autoJoinPublicChatRoom));
			}
			else
			{
				onSuccess(GetFirstChannel());
			}
		}
		else
		{
			OnLoadingFailed(task.behaviour);
		}
	}

	private IEnumerator HandleAnalytics()
	{
		TaskService logChatCreatedRequest = analyticsRequestFactory.Create<ILogChatCreatedRequest>().AsTask();
		yield return logChatCreatedRequest;
		if (!logChatCreatedRequest.succeeded)
		{
			throw new Exception("Log Chat Created Request failed", logChatCreatedRequest.behaviour.exceptionThrown);
		}
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}
}
