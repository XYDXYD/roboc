using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

[ExecuteInEditMode]
internal class LocalizeTexture : MonoBehaviour
{
	public UITexture texture;

	public List<LocaleTexture> localeTextures = new List<LocaleTexture>();

	public LocalizeTexture()
		: this()
	{
	}

	private unsafe void Start()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		Dictionary<string, string> languages = StringTableBase<StringTable>.Instance.languages;
		string[] array = new string[languages.Keys.Count];
		languages.Keys.CopyTo(array, 0);
		string[] array2 = array;
		foreach (string language in array2)
		{
			if (!localeTextures.Any((LocaleTexture t) => t.locale == language))
			{
				localeTextures.Add(new LocaleTexture(language, null));
			}
		}
		UpdateTexture();
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

	private void UpdateTexture()
	{
		LocaleTexture localeTexture = localeTextures.FirstOrDefault((LocaleTexture t) => t.locale == Localization.get_language());
		if (localeTexture != null)
		{
			texture.set_mainTexture(localeTexture.texture);
		}
		else
		{
			Console.LogWarning(Localization.get_language() + " texture not found!");
		}
	}
}
