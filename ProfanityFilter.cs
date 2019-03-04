using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal class ProfanityFilter
{
	public const string FOLDER = "ProfanityFilters/";

	public const string ALTS_PREFIX = "Alts_";

	public const string PADS_PREFIX = "Pads_";

	public const string PROF_PREFIX = "Prof_";

	public const char REPLACE_CHAR = '\0';

	private ProfanityNode _root;

	private string _textToFilter;

	private List<int> _startPoints = new List<int>();

	private List<int> _endPoints = new List<int>();

	private List<ProfanityAlternative> _alternatives = new List<ProfanityAlternative>();

	private List<char> _paddingChars = new List<char>();

	private bool _filesLoaded;

	private bool _enabled = true;

	private bool _isReady;

	public void LoadDataBase(string language)
	{
		if (language == "neutral")
		{
			language = "english";
		}
		string text = "ProfanityFilters/" + language + "/Alts_" + language;
		TextAsset val = Resources.Load<TextAsset>(text);
		text = "ProfanityFilters/" + language + "/Pads_" + language;
		TextAsset val2 = Resources.Load<TextAsset>(text);
		text = "ProfanityFilters/" + language + "/Prof_" + language;
		TextAsset val3 = Resources.Load<TextAsset>(text);
		_filesLoaded = (val3 != null && val != null && val2 != null);
		if (_filesLoaded)
		{
			Initialize(val3, val, val2);
		}
		else
		{
			Console.Log("Profanity Filter : files not correctly loaded");
		}
		_isReady = true;
	}

	public bool IsReady()
	{
		return _isReady;
	}

	private void Initialize(TextAsset profanityCSV, TextAsset altsCSV, TextAsset padsCSV)
	{
		ResetProfanityData();
		LoadProfanityCSV(profanityCSV);
		LoadAlternateCSV(altsCSV);
		LoadPaddingCharactersCSV(padsCSV);
	}

	public void OnFilterEnabledChanged(bool enabled)
	{
		SetEnabled(enabled);
	}

	private void SetEnabled(bool enabled)
	{
		_enabled = enabled;
	}

	private void ResetProfanityData()
	{
		_root = null;
		_root = new ProfanityNode('r');
		_textToFilter = string.Empty;
		_startPoints.Clear();
		_endPoints.Clear();
		_alternatives.Clear();
		_paddingChars.Clear();
	}

	private void LoadProfanityCSV(TextAsset profanityCSV)
	{
		char[] separator = new char[3]
		{
			'\n',
			',',
			'\r'
		};
		List<string> profanityPhrases = new List<string>(profanityCSV.get_text().Split(separator, StringSplitOptions.RemoveEmptyEntries));
		CreateProfanityDataTree(profanityPhrases);
	}

	private void LoadAlternateCSV(TextAsset altsCSV)
	{
		if (altsCSV != null)
		{
			char[] separator = new char[1]
			{
				'\n'
			};
			List<string> list = new List<string>(altsCSV.get_text().Split(separator, StringSplitOptions.RemoveEmptyEntries));
			foreach (string item2 in list)
			{
				if (item2.Length >= 3)
				{
					ProfanityAlternative item = new ProfanityAlternative(item2[0], item2[2]);
					_alternatives.Add(item);
				}
			}
		}
	}

	private void LoadPaddingCharactersCSV(TextAsset padsCSV)
	{
		if (padsCSV != null)
		{
			char[] separator = new char[1]
			{
				'\n'
			};
			List<string> list = new List<string>(padsCSV.get_text().Split(separator, StringSplitOptions.RemoveEmptyEntries));
			foreach (string item in list)
			{
				_paddingChars.Add(item[0]);
			}
		}
	}

	private void CreateProfanityDataTree(List<string> profanityPhrases)
	{
		foreach (string profanityPhrase in profanityPhrases)
		{
			ProfanityNode profanityNode = _root;
			for (int i = 0; i < profanityPhrase.Length; i++)
			{
				char letter = profanityPhrase[i];
				ProfanityNode profanityNode2 = profanityNode.children.Find((ProfanityNode x) => x.ch.Equals(letter));
				if (profanityNode2 == null)
				{
					if (i == profanityPhrase.Length - 1)
					{
						ProfanityNode profanityNode3 = new ProfanityNode(letter);
						profanityNode3.terminator = true;
						profanityNode2 = profanityNode3;
						profanityNode.children.Add(profanityNode2);
					}
					else
					{
						ProfanityNode profanityNode3 = new ProfanityNode(letter);
						profanityNode3.terminator = false;
						profanityNode2 = profanityNode3;
						profanityNode.children.Add(profanityNode2);
					}
				}
				else if (i == profanityPhrase.Length - 1)
				{
					profanityNode2.terminator = true;
				}
				profanityNode = profanityNode2;
			}
		}
	}

	public virtual string FilterString(string inputString)
	{
		if (!_enabled || !_filesLoaded)
		{
			return inputString;
		}
		_startPoints.Clear();
		_endPoints.Clear();
		_textToFilter = inputString;
		string text = inputString.ToLower();
		FindProfanity(ref text);
		return RemoveProfanity('\0');
	}

	private void FindProfanity(ref string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			FindProfanityPhrase(ref text, i, i, profaneLetterFound: false, _root, 0);
		}
	}

	private void FindProfanityPhrase(ref string text, int index, int phraseStart, bool profaneLetterFound, ProfanityNode node, int asterixCount)
	{
		if (index >= text.Length)
		{
			return;
		}
		char letter = text[index];
		List<ProfanityNode> list = new List<ProfanityNode>();
		if (index == 0 && !profaneLetterFound)
		{
			ProfanityNode profanityNode = _root.children.Find((ProfanityNode x) => x.ch.Equals(' '));
			if (profanityNode != null)
			{
				FindProfanityPhrase(ref text, 0, 0, profaneLetterFound: true, profanityNode, asterixCount);
			}
			List<ProfanityAlternative> list2 = _alternatives.FindAll((ProfanityAlternative x) => x.key.Equals(letter));
			foreach (ProfanityAlternative alt in list2)
			{
				ProfanityNode profanityNode2 = node.children.Find((ProfanityNode x) => x.ch.Equals(alt.alt));
				if (profanityNode2 != null)
				{
					list.Add(profanityNode2);
				}
			}
		}
		ProfanityNode profanityNode3 = node.children.Find((ProfanityNode x) => x.ch.Equals(letter));
		if (letter == '*' && asterixCount < 2)
		{
			asterixCount++;
			foreach (ProfanityNode child in node.children)
			{
				list.Add(child);
			}
		}
		if (_paddingChars.Contains(letter) && profaneLetterFound)
		{
			FindProfanityPhrase(ref text, index + 1, phraseStart, profaneLetterFound, node, asterixCount);
		}
		else if (profanityNode3 == null && profaneLetterFound)
		{
			if (index > 0)
			{
				if (letter == text[index - 1])
				{
					if (node.terminator)
					{
						_startPoints.Add(phraseStart);
						_endPoints.Add(index);
					}
					FindProfanityPhrase(ref text, index + 1, phraseStart, profaneLetterFound, node, asterixCount);
				}
				else
				{
					List<ProfanityAlternative> list3 = _alternatives.FindAll((ProfanityAlternative x) => x.key.Equals(letter));
					foreach (ProfanityAlternative alt2 in list3)
					{
						profanityNode3 = node.children.Find((ProfanityNode x) => x.ch.Equals(alt2.alt));
						if (profanityNode3 != null)
						{
							list.Add(profanityNode3);
						}
					}
				}
			}
		}
		else if (profanityNode3 != null)
		{
			list.Add(profanityNode3);
			if (index > 0 && letter == text[index - 1] && node.terminator)
			{
				_startPoints.Add(phraseStart);
				_endPoints.Add(index);
				FindProfanityPhrase(ref text, index + 1, phraseStart, profaneLetterFound, node, asterixCount);
			}
		}
		foreach (ProfanityNode item in list)
		{
			if (!profaneLetterFound)
			{
				profaneLetterFound = true;
				phraseStart = ((item.ch != ' ') ? index : (index + 1));
			}
			if (item.terminator)
			{
				_startPoints.Add(phraseStart);
				_endPoints.Add(index);
			}
			FindProfanityPhrase(ref text, index + 1, phraseStart, profaneLetterFound, item, asterixCount);
		}
	}

	private string RemoveProfanity(char replaceChar)
	{
		string text = _textToFilter;
		for (int i = 0; i < _startPoints.Count; i++)
		{
			int num = _endPoints[i] - _startPoints[i] + 1;
			if (num > _textToFilter.Length)
			{
				num = _textToFilter.Length;
			}
			if (num > 0)
			{
				text = text.Remove(_startPoints[i], num);
				text = text.Insert(value: (replaceChar != 0) ? new string(replaceChar, num) : new string('~', num), startIndex: _startPoints[i]);
			}
		}
		if (replaceChar == '\0')
		{
			text = text.Replace("~", string.Empty);
		}
		return text;
	}
}
