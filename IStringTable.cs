using System.Collections.Generic;

public interface IStringTable
{
	Dictionary<string, string> languages
	{
		get;
	}

	string currentLanguageString
	{
		get;
	}

	string GetString(string key);

	string GetReplaceString(string key, string replaceKey, string replaceValue);

	void SetLanguage(string languageText);

	string GetLanguageKeyFromString(string text);
}
