using Svelto.WeakEvents;
using System;

internal interface IChatClient
{
	Action onUnexpectedDisconnection
	{
		get;
		set;
	}

	WeakEvent onConnected
	{
		get;
		set;
	}

	void SendMessage(string channelName, ChatChannelType channelType, OutGoingChatMessage message, WeakAction<OutGoingChatMessage> onSuccess, WeakAction<ChatReasonCode> onError, Action<Exception> onException);

	void SendPrivateMessage(string recipient, string message, string guid, Action<Exception> onException);

	void RegisterForMessageCallbacks(IChatMessageReceiver receiver);

	void DeregisterMessageCallbacks(IChatMessageReceiver receiver);

	bool IsConnected();

	bool IsConnecting();

	void Connect(Action onFail);
}
