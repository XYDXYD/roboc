using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpListLocalize : MonoBehaviour
{
	[SerializeField]
	private List<string> keys;

	private UIPopupList _popUpList;

	public UIPopUpListLocalize()
		: this()
	{
	}

	private unsafe void Awake()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		_popUpList = this.GetComponent<UIPopupList>();
		UpdatePopUpList();
		Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private unsafe void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private void UpdatePopUpList()
	{
		_popUpList.Clear();
		for (int i = 0; i < keys.Count; i++)
		{
			_popUpList.AddItem(StringTableBase<StringTable>.Instance.GetString(keys[i]), (object)i);
		}
	}

	private void OnLanguageChanged()
	{
		UpdatePopUpList();
	}
}
