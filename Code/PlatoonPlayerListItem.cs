using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class PlatoonPlayerListItem : MonoBehaviour
{
	public UILabel _nameLabel;

	public UILabel _statusLabel;

	public UIPopupList _eventPopup;

	public Color blue = new Color(0.039f, 0.635f, 0.973f);

	public Color White = new Color(1f, 1f, 1f);

	private Action<string, string> _onSelectionChangeCallback;

	private Dictionary<string, string> _actions = new Dictionary<string, string>();

	private Dictionary<PlatoonMember.MemberStatus, string> _memberStatusStrings = new Dictionary<PlatoonMember.MemberStatus, string>
	{
		{
			PlatoonMember.MemberStatus.Ready,
			"strAccepted"
		},
		{
			PlatoonMember.MemberStatus.InQueue,
			"strAccepted"
		},
		{
			PlatoonMember.MemberStatus.Invited,
			"strPending"
		}
	};

	private string closeText = "strClose";

	public PlatoonPlayerListItem()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	//IL_002f: Unknown result type (might be due to invalid IL or missing references)


	private unsafe void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		EventDelegate.Add(_eventPopup.onChange, new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
	}

	private unsafe void OnDestroy()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		EventDelegate.Remove(_eventPopup.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	internal void SetData(string name, PlatoonMember.MemberStatus status, bool isLeader, Dictionary<string, string> actions, Action<string, string> onSelectionChangeCallback)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Color color = (!isLeader) ? White : blue;
		_nameLabel.set_text(name);
		string statusString = GetStatusString(status, isLeader);
		_statusLabel.set_text(statusString);
		_nameLabel.set_color(color);
		_statusLabel.set_color(color);
		_eventPopup.items.Clear();
		_actions.Clear();
		foreach (KeyValuePair<string, string> action in actions)
		{
			_actions[action.Key] = action.Value;
			_eventPopup.items.Add(action.Value);
		}
		_eventPopup.items.Add(StringTableBase<StringTable>.Instance.GetString(closeText));
		_eventPopup.set_value(closeText);
		_onSelectionChangeCallback = onSelectionChangeCallback;
	}

	private void OnSelectionChange()
	{
		if (UIPopupList.current != null)
		{
			string value = UIPopupList.current.get_value();
			if (value != closeText)
			{
				string arg = null;
				foreach (KeyValuePair<string, string> action in _actions)
				{
					if (action.Value == value)
					{
						arg = action.Key;
					}
				}
				_onSelectionChangeCallback(_nameLabel.get_text(), arg);
			}
		}
	}

	private string GetStatusString(PlatoonMember.MemberStatus status, bool isLeader)
	{
		if (isLeader)
		{
			return StringTableBase<StringTable>.Instance.GetString("strLeader");
		}
		return StringTableBase<StringTable>.Instance.GetString(_memberStatusStrings[status]);
	}
}
