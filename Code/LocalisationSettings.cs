using System.Collections.Generic;

internal sealed class LocalisationSettings
{
	internal string GetCurrentLanguage()
	{
		return StringTableBase<StringTable>.Instance.currentLanguageString;
	}

	internal Dictionary<string, string> GetLanguages()
	{
		return StringTableBase<StringTable>.Instance.languages;
	}

	internal void SetLanguage(string language)
	{
		string languageKeyFromString = StringTableBase<StringTable>.Instance.GetLanguageKeyFromString(language);
		if (Localization.get_language() != languageKeyFromString)
		{
			Localization.set_language(languageKeyFromString);
		}
	}
}
