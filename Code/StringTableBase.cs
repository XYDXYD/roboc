using Rewired;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public abstract class StringTableBase<T> : IStringTable where T : class, new()
{
	private static T _instance;

	private static StringBuilder _stringBuilder = new StringBuilder();

	private static Dictionary<string, LanguageData> _activeLanguages;

	private static Dictionary<string, string> _languageKeysToString;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				InitInstance();
			}
			return _instance;
		}
	}

	public string language => Localization.get_language();

	public Dictionary<string, LanguageData> activeLanguages => _activeLanguages;

	public Dictionary<string, string> languages
	{
		get
		{
			if (_languageKeysToString == null)
			{
				_languageKeysToString = new Dictionary<string, string>();
				using (Dictionary<string, LanguageData>.Enumerator enumerator = _activeLanguages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						_languageKeysToString.Add(enumerator.Current.Key, GetString(enumerator.Current.Value.languageStrKey));
					}
				}
			}
			return _languageKeysToString;
		}
	}

	public string currentLanguageSprite => _activeLanguages[Localization.get_language()].spriteName;

	public string currentLanguageString
	{
		get
		{
			if (languages.ContainsKey(Localization.get_language()))
			{
				return languages[Localization.get_language()];
			}
			return Localization.get_language();
		}
	}

	private static void InitInstance()
	{
		_instance = new T();
		Dictionary<string, LanguageData> dictionary = new Dictionary<string, LanguageData>();
		dictionary.Add("ChineseSimplified_Tencent", new LanguageData("strChineseSimplified", "Flags_75x50/Flags_China_75x50"));
		_activeLanguages = dictionary;
		if (!PlayerPrefs.HasKey("LanguageSelected") || !_activeLanguages.ContainsKey(Localization.get_language()))
		{
			Localization.set_language("ChineseSimplified_Tencent");
			PlayerPrefs.SetInt("LanguageSelected", 1);
		}
	}

	public void ApplyStringOverrides(string languageName, Dictionary<string, string> overrideStrings)
	{
		foreach (KeyValuePair<string, string> overrideString in overrideStrings)
		{
			Localization.Set(overrideString.Key, overrideString.Value);
		}
	}

	public string GetString(string key)
	{
		return Localization.Get(key, true);
	}

	public string GetKeyboardKeyString(KeyboardKeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		if (((int)key >= 97 && (int)key <= 122) || ((int)key >= 48 && (int)key <= 57) || ((int)key >= 282 && (int)key <= 296))
		{
			return ((object)key).ToString();
		}
		string text = $"strKeyboard{((object)key).ToString().ToUpper()}";
		string @string = GetString(text);
		if (@string == text)
		{
			return ((object)key).ToString();
		}
		return @string;
	}

	public string GetMouseJoystickString(string key)
	{
		string text = string.Format("str{0}", key.Replace(" ", string.Empty).ToUpper());
		string @string = GetString(text);
		if (@string == text)
		{
			return key;
		}
		return @string;
	}

	public string GetReplaceString(string key, string replaceKey, string replaceValue)
	{
		_stringBuilder.Length = 0;
		string @string = GetString(key);
		_stringBuilder.Append(@string);
		_stringBuilder.Replace(replaceKey, replaceValue);
		return _stringBuilder.ToString();
	}

	public string GetReplaceString(string key, string firstReplaceKey, string firstReplaceValue, string secondReplaceKey, string secondReplaceValue)
	{
		_stringBuilder.Length = 0;
		string @string = StringTableBase<StringTable>.Instance.GetString(key);
		_stringBuilder.Append(@string);
		_stringBuilder.Replace(firstReplaceKey, firstReplaceValue);
		_stringBuilder.Replace(secondReplaceKey, secondReplaceValue);
		return _stringBuilder.ToString();
	}

	public string GetReplaceStringWithInputActionKeyMap(string key)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		string text = GetString(key);
		Match match = Regex.Match(text, "{(.*?)}");
		while (match.Success)
		{
			string value = match.Groups[1].Value;
			if (match.Groups.Count < 2)
			{
			}
			KeyboardKeyCode inputActionKeyCode = InputRemapper.Instance.GetInputActionKeyCode(value);
			if ((int)inputActionKeyCode != 0)
			{
				text = text.Replace(match.Groups[0].Value, GetKeyboardKeyString(inputActionKeyCode));
			}
			else
			{
				string inputActionKeyMap = InputRemapper.Instance.GetInputActionKeyMap(match.Groups[1].Value);
				text = text.Replace(match.Groups[0].Value, (!(inputActionKeyMap == string.Empty)) ? GetMouseJoystickString(inputActionKeyMap) : GetString("strHotkeyNotBound"));
			}
			match = match.NextMatch();
		}
		return text;
	}

	public void SetLanguage(string languageText)
	{
		Localization.set_language(GetLanguageKeyFromString(languageText));
		if (!PlayerPrefs.HasKey("LanguageSelected"))
		{
			PlayerPrefs.SetInt("LanguageSelected", 1);
		}
	}

	public string GetLanguageKeyFromString(string text)
	{
		using (Dictionary<string, string>.Enumerator enumerator = languages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value == text)
				{
					return enumerator.Current.Key;
				}
			}
		}
		return "English";
	}
}
