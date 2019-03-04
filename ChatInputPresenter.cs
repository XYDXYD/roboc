using Svelto.Context;
using Svelto.IoC;
using System;
using UnityEngine;

internal class ChatInputPresenter : IInitialize, IWaitForFrameworkDestruction
{
	private bool _disableChat;

	private bool _inputHadFocus;

	private string _cachedText;

	private ShortCutMode _previousShortcutMode;

	private bool previousInputControllerEnabled;

	public ChatInputView view
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal InputController inputController
	{
		private get;
		set;
	}

	[Inject]
	internal DisableChatInputObserver disableChatInputObserver
	{
		private get;
		set;
	}

	public void OnDependenciesInjected()
	{
		disableChatInputObserver.AddAction((Action)DisableChat);
	}

	public void OnFrameworkDestroyed()
	{
		disableChatInputObserver.RemoveAction((Action)DisableChat);
	}

	public void OnGUIEvent(ChatGUIEvent message)
	{
		switch (message.type)
		{
		case ChatGUIEvent.Type.MessageReceived:
			break;
		case ChatGUIEvent.Type.BeginMessage:
			view.input.set_value(message.text);
			break;
		case ChatGUIEvent.Type.CancelMessage:
			view.input.set_value(string.Empty);
			break;
		case ChatGUIEvent.Type.Focus:
			view.input.set_isSelected(true);
			break;
		case ChatGUIEvent.Type.Unfocus:
			view.input.set_isSelected(false);
			break;
		case ChatGUIEvent.Type.SetChannel:
			OnSetChannel(message.channel);
			break;
		}
	}

	internal void OnSetChannel(IChatChannel channel)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		ChatChannelType channelType = channel?.ChatChannelType ?? ChatChannelType.None;
		Color val = Color32.op_Implicit(ChatColours.GetColour(channelType));
		if (view.placeholder != null)
		{
			UILabel placeholder = view.placeholder.placeholder;
			UILabel obj = placeholder;
			float r = val.r;
			float g = val.g;
			float b = val.b;
			Color color = placeholder.get_color();
			obj.set_color(new Color(r, g, b, color.a));
		}
		view.input.activeTextColor = val;
		view.input.set_defaultColor(val);
		view.input.caretColor = val;
		view.input.UpdateLabel();
		if (view.input.get_isSelected())
		{
			view.input.set_isSelected(false);
			view.input.set_isSelected(true);
		}
	}

	private bool CanBeFocusedWithShortcut()
	{
		if (_disableChat)
		{
			return false;
		}
		if (UICamera.get_inputHasFocus() || _inputHadFocus)
		{
			return false;
		}
		return true;
	}

	internal void Tick()
	{
		if (Input.GetKeyDown(9) && view.input.get_isSelected())
		{
			view.Bubble(new ChatGUIEvent(ChatGUIEvent.Type.GoToNextChannel));
		}
		if (CanBeFocusedWithShortcut())
		{
			bool flag = Input.GetKeyDown(47) || Input.GetKeyDown(267);
			if (InputRemapper.Instance.GetButtonDown("ToggleChatActive") || flag)
			{
				view.input.set_isSelected(true);
				if (flag)
				{
					view.input.set_value("/");
				}
			}
		}
		_inputHadFocus = UICamera.get_inputHasFocus();
	}

	internal void OnGetFocus()
	{
		view.Bubble(new ChatGUIEvent(ChatGUIEvent.Type.AskFocus));
		view.input.set_value(_cachedText);
		_previousShortcutMode = guiInputController.GetShortCutMode();
		guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
		previousInputControllerEnabled = inputController.Enabled;
		inputController.Enabled = false;
	}

	internal void OnLoseFocus()
	{
		_cachedText = view.input.get_value();
		view.input.set_value(string.Empty);
		guiInputController.SetShortCutMode(_previousShortcutMode);
		inputController.Enabled = previousInputControllerEnabled;
	}

	internal void OnSubmit()
	{
		string value = view.input.get_value();
		if (!string.IsNullOrEmpty(value))
		{
			value = NGUIText.StripSymbols(value);
			view.input.set_value(string.Empty);
			view.Bubble(new ChatGUIEvent(ChatGUIEvent.Type.SendMessage, value));
		}
		else
		{
			view.Bubble(new ChatGUIEvent(ChatGUIEvent.Type.AskUnfocus));
		}
	}

	private void DisableChat()
	{
		_disableChat = true;
	}
}
