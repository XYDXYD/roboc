using System;

internal struct ChatGUIEvent
{
	public enum Type
	{
		SendMessage,
		SetFilter,
		ToggleExpand,
		Focus,
		Unfocus,
		BeginMessage,
		CancelMessage,
		MessageReceived,
		SetChannel,
		GoToNextChannel,
		[Obsolete("Use AskFocus instead")]
		InputClicked,
		ClearFilter,
		UpdateChannelList,
		Disconnected,
		Connected,
		AskUnfocus,
		AskFocus,
		StyleChanged
	}

	private Type _type;

	private object _data;

	public Type type => _type;

	public string text => _data.ToString();

	public ChatMessage chatMessage => _data as ChatMessage;

	public IChatChannel channel => _data as IChatChannel;

	public ChatStyle style => (ChatStyle)_data;

	public ChatGUIEvent(Type aType, object aData = null)
	{
		_type = aType;
		_data = aData;
	}
}
