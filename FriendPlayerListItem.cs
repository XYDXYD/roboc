using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class FriendPlayerListItem : MonoBehaviour
{
	public UILabel _nameLabel;

	public UISprite _sprite;

	public UILabel _statusLabel;

	public UIPopupList _eventPopup;

	public Color blue = new Color(0f, 0.55f, 0.93f);

	public Color green = new Color(0.039f, 0.79f, 0.17f);

	public Color red = new Color(0.79f, 0.13f, 0f);

	public Color gray = new Color(0.96f, 0.96f, 0.96f);

	private Action<string, string> _callback;

	private Dictionary<string, string> _actions = new Dictionary<string, string>();

	private const string onlineText = "strOnline";

	private const string offlineText = "strOffline";

	private const string inviteSentText = "strSent";

	private const string invitePendingText = "strPending";

	private const string closeText = "strClose";

	public FriendPlayerListItem()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0044: Unknown result type (might be due to invalid IL or missing references)
	//IL_0049: Unknown result type (might be due to invalid IL or missing references)
	//IL_005e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0063: Unknown result type (might be due to invalid IL or missing references)


	private unsafe void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		EventDelegate val = new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		EventDelegate.Add(_eventPopup.onChange, val);
	}

	private unsafe void OnDestroy()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		EventDelegate.Remove(_eventPopup.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	internal void SetData(string name, bool online, FriendInviteStatus status, Dictionary<string, string> actions, Action<string, string> callback)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Color color = gray;
		string key = "UNKNOWN";
		switch (status)
		{
		case FriendInviteStatus.Accepted:
			if (online)
			{
				color = green;
				key = "strOnline";
			}
			else
			{
				color = red;
				key = "strOffline";
			}
			break;
		case FriendInviteStatus.InvitePending:
			color = blue;
			key = "strPending";
			break;
		case FriendInviteStatus.InviteSent:
			color = gray;
			key = "strSent";
			break;
		}
		_nameLabel.set_text(name);
		_statusLabel.set_text(StringTableBase<StringTable>.Instance.GetString(key));
		_nameLabel.set_color(color);
		_statusLabel.set_color(color);
		_sprite.set_color(color);
		_eventPopup.items.Clear();
		SetCommands(actions);
		_eventPopup.items.Add(StringTableBase<StringTable>.Instance.GetString("strClose"));
		_eventPopup.set_value("strClose");
		_callback = callback;
	}

	public void SetCommands(Dictionary<string, string> actions)
	{
		_eventPopup.Clear();
		_actions.Clear();
		foreach (KeyValuePair<string, string> action in actions)
		{
			_actions[action.Key] = action.Value;
			_eventPopup.items.Add(action.Value);
		}
	}

	private void OnSelectionChange()
	{
		if (UIPopupList.current != null)
		{
			string value = UIPopupList.current.get_value();
			if (value != "strClose")
			{
				string arg = null;
				foreach (KeyValuePair<string, string> action in _actions)
				{
					if (action.Value == value)
					{
						arg = action.Key;
					}
				}
				_callback(_nameLabel.get_text(), arg);
			}
		}
	}
}
