using Svelto.WeakEvents;

internal class PrivateChatChannel : IChatChannel
{
	private readonly ChatPresenter _chatPresenter;

	private readonly string _receiver;

	private readonly string _displayName;

	public string ChannelName => _receiver;

	public string VisibleName => _displayName;

	public bool Suspended
	{
		get;
		set;
	}

	public ChatChannelType ChatChannelType => ChatChannelType.Private;

	public PrivateChatChannel(ChatPresenter chatPresenter, string receiver, string displayName)
	{
		_chatPresenter = chatPresenter;
		_receiver = receiver;
		_displayName = displayName;
	}

	public void SendMessage(OutGoingChatMessage message, WeakAction<OutGoingChatMessage> onSentSuccesfully)
	{
		_chatPresenter.SendWhisper(_receiver, _displayName, message.RawText);
	}
}
