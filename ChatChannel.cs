using ChatServiceLayer;
using Svelto.WeakEvents;
using System;
using System.Collections.Generic;
using Utility;

internal class ChatChannel : IChatChannel
{
	public List<ChatChannelMember> Members;

	private readonly string _channelName;

	private readonly ChatChannelType _chatChannelType;

	private readonly IChatClient _chatClient;

	private readonly ChatPresenter _chatPresenter;

	public string VisibleName => ChatUtil.GetVisibleChannelName(_channelName, _chatChannelType);

	public ChatChannelType ChatChannelType => _chatChannelType;

	public bool Suspended
	{
		get;
		set;
	}

	public string ChannelName => _channelName;

	public ChatChannel(ChatPresenter chatPresenter, IChatClient chatClient, string channelName, ChatChannelType channelType, IEnumerable<ChatChannelMember> members)
	{
		_chatPresenter = chatPresenter;
		_channelName = channelName;
		_chatChannelType = channelType;
		Members = ((members == null) ? new List<ChatChannelMember>() : new List<ChatChannelMember>(members));
		_chatClient = chatClient;
	}

	public virtual void SendMessage(OutGoingChatMessage message, WeakAction<OutGoingChatMessage> onSentSuccesfully)
	{
		_chatClient.SendMessage(_channelName, _chatChannelType, message, onSentSuccesfully, new WeakAction<ChatReasonCode>((Action<ChatReasonCode>)ErrorSendingMessage), ExceptionSendingMessage);
	}

	protected void ErrorJoiningChannel(Exception exception)
	{
		_chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrJoiningChannel"));
		Console.LogException(exception);
		RemoteLogger.Error(exception);
	}

	protected void ErrorLeavingChannel(Exception exception)
	{
		Console.LogException(exception);
		RemoteLogger.Error(exception);
	}

	private void ErrorSendingMessage(ChatReasonCode reason)
	{
		_chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString(reason.ToString()));
	}

	private void ExceptionSendingMessage(Exception exception)
	{
		_chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrSendingMessage"));
		Console.LogException(exception);
		RemoteLogger.Error(exception);
	}

	internal void SetMembers(ChatChannelMember[] members)
	{
		Members = new List<ChatChannelMember>(members);
	}
}
