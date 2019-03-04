using System.Collections.Generic;

internal class SpecialItemsStringTable
{
	private static SpecialItemsStringTable _instance;

	private Dictionary<string, string> _stringTable;

	public static SpecialItemsStringTable Instance
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

	public string GetString(string key)
	{
		string value = string.Empty;
		_stringTable.TryGetValue(key, out value);
		if (value != null)
		{
			return StringTableBase<StringTable>.Instance.GetString(value);
		}
		return value;
	}

	private static void InitInstance()
	{
		_instance = new SpecialItemsStringTable();
		_instance._stringTable = SpecialItemsStrings.dictionary;
	}
}
