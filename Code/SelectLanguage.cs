using System;
using System.Collections.Generic;
using UnityEngine;

internal class SelectLanguage : MonoBehaviour
{
	public UIPopupList languagePopupList;

	public UILabel selectedLanguageLabel;

	public SelectLanguage()
		: this()
	{
	}

	private unsafe void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		PopulateLanguage();
		UpdateLabel();
	}

	private unsafe void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void OnLanguageChanged()
	{
		StringTableBase<StringTable>.Instance.SetLanguage(languagePopupList.get_value());
	}

	private void PopulateLanguage()
	{
		Dictionary<string, string> languages = StringTableBase<StringTable>.Instance.languages;
		Dictionary<string, string>.Enumerator enumerator = languages.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, string> current = enumerator.Current;
			languagePopupList.items.Add(current.Value);
		}
	}

	private void UpdateLabel()
	{
		if (Object.op_Implicit(selectedLanguageLabel))
		{
			languagePopupList.set_value(StringTableBase<StringTable>.Instance.currentLanguageString);
		}
	}
}
