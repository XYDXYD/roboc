using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
internal class LocalizeUISprite : MonoBehaviour
{
	private UISprite _target;

	public string DefaultSpriteName;

	public List<LocalizedSprite> LocaleSprites = new List<LocalizedSprite>();

	public LocalizeUISprite()
		: this()
	{
	}

	private unsafe void Start()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		_target = this.GetComponent<UISprite>();
		Dictionary<string, string> languages = StringTableBase<StringTable>.Instance.languages;
		string[] array = new string[languages.Keys.Count];
		languages.Keys.CopyTo(array, 0);
		string[] array2 = array;
		foreach (string language in array2)
		{
			if (!LocaleSprites.Any((LocalizedSprite t) => t.locale == language))
			{
				LocaleSprites.Add(new LocalizedSprite(language, null));
			}
		}
		UpdateSprite();
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

	private void UpdateSprite()
	{
		LocalizedSprite localizedSprite = LocaleSprites.FirstOrDefault((LocalizedSprite t) => t.locale == Localization.get_language());
		if (localizedSprite != null)
		{
			_target.set_spriteName(localizedSprite.spriteNameToUse);
		}
		else
		{
			_target.set_spriteName(DefaultSpriteName);
		}
	}
}
