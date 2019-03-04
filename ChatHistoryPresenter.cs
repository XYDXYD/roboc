using System.Collections.Generic;
using UnityEngine;

internal class ChatHistoryPresenter
{
	private class Message
	{
		public ChatMessage message;

		public bool visibleUnfocused;
	}

	private ChatHistoryView _view;

	private IChatChannel _channelFilter;

	private bool _focused;

	private bool _keepHistoryShown;

	private List<Message> _messages = new List<Message>();

	internal ChatHistoryView view
	{
		get
		{
			return _view;
		}
		set
		{
			_view = value;
			_view.textList.paragraphHistory = 200;
		}
	}

	internal void OnGUIEvent(ChatGUIEvent message)
	{
		switch (message.type)
		{
		case ChatGUIEvent.Type.Focus:
			OnFocusChange(focus: true);
			break;
		case ChatGUIEvent.Type.Unfocus:
			OnFocusChange(focus: false);
			break;
		case ChatGUIEvent.Type.MessageReceived:
			Add(message.chatMessage);
			break;
		case ChatGUIEvent.Type.SendMessage:
			ScrollDown();
			break;
		case ChatGUIEvent.Type.SetFilter:
			SetFilter(message.channel);
			break;
		case ChatGUIEvent.Type.ClearFilter:
			SetFilter(null);
			break;
		case ChatGUIEvent.Type.StyleChanged:
			_keepHistoryShown = (message.style == ChatStyle.CustomGame);
			UpdateMessagesVisibility();
			break;
		}
	}

	public void Tick()
	{
		if (!_focused)
		{
			FadeOldMessages();
		}
	}

	private void ScrollDown()
	{
		view.textList.set_scrollValue(1f);
	}

	private void Add(ChatMessage message)
	{
		Message message2;
		if (_messages.Count < _view.textList.paragraphHistory)
		{
			message2 = new Message();
		}
		else
		{
			message2 = _messages[0];
			_messages.RemoveAt(0);
		}
		message2.message = message;
		message2.visibleUnfocused = true;
		if (MessageShouldBeShown(message2))
		{
			_view.textList.Add(message.ToString());
		}
		_messages.Add(message2);
	}

	private void SetFilter(IChatChannel newFilter)
	{
		_channelFilter = newFilter;
		UpdateMessagesVisibility();
	}

	private bool MessageShouldBeShown(Message m)
	{
		if (!_focused && !m.visibleUnfocused && !_keepHistoryShown)
		{
			return false;
		}
		if (_channelFilter == null)
		{
			return true;
		}
		IChatMessageWithChannel chatMessageWithChannel = m.message as IChatMessageWithChannel;
		if (chatMessageWithChannel != null)
		{
			if (chatMessageWithChannel.ChannelName == _channelFilter.ChannelName && chatMessageWithChannel.ChatChannelType == _channelFilter.ChatChannelType)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	private void UpdateMessagesVisibility()
	{
		_view.textList.ClearWithoutRebuild();
		for (int i = 0; i < _messages.Count; i++)
		{
			Message message = _messages[i];
			if (MessageShouldBeShown(message))
			{
				_view.textList.AddWithoutRebuild(message.message.ToString());
			}
		}
		_view.textList.Rebuild();
	}

	private void FadeOldMessages()
	{
		float time = Time.get_time();
		bool flag = false;
		for (int i = 0; i < _messages.Count; i++)
		{
			Message message = _messages[i];
			if (message.visibleUnfocused != time - message.message.Created < _view.fadeTime)
			{
				message.visibleUnfocused = !message.visibleUnfocused;
				flag = true;
			}
		}
		if (flag)
		{
			UpdateMessagesVisibility();
		}
	}

	public void OnFocusChange(bool focus)
	{
		_focused = focus;
		UpdateMessagesVisibility();
		ScrollDown();
		Collider component = _view.GetComponent<Collider>();
		if (component != null)
		{
			component.set_enabled(focus);
		}
	}
}
