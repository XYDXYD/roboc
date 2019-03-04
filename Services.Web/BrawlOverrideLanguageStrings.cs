using System.Collections.Generic;

namespace Services.Web
{
	public class BrawlOverrideLanguageStrings
	{
		public readonly Dictionary<string, string> LanguageStrings;

		public BrawlOverrideLanguageStrings(Dictionary<string, string> inputLanguageStrings)
		{
			LanguageStrings = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> inputLanguageString in inputLanguageStrings)
			{
				LanguageStrings.Add(inputLanguageString.Key, inputLanguageString.Value);
			}
		}
	}
}
