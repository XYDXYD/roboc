using System.Text.RegularExpressions;

namespace Mothership
{
	internal static class SocialInputValidation
	{
		private static readonly string UsernameRegexPattern = "^[\\p{L}\\p{Nd}-_]?[\\p{L}\\p{Nd}-_\\.]*[\\p{L}\\p{Nd}-_]+$";

		private static readonly Regex _userNameRegex = new Regex(UsernameRegexPattern);

		private static readonly string[] INVALID_CHARACTERS = new string[2]
		{
			"|",
			"ㅤ"
		};

		public static bool ValidateUserName(ref string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
			{
				return false;
			}
			Match match = _userNameRegex.Match(playerName);
			if (!match.Success || playerName.StartsWith(".") || playerName.EndsWith("."))
			{
				return false;
			}
			playerName = playerName.Replace("\r", string.Empty);
			playerName = playerName.Replace("\n", string.Empty);
			return true;
		}

		public static bool DoesStringContainAtSymbol(ref string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			return str.Contains("@");
		}

		public static bool DoesStringContainInvalidCharacters(ref string str)
		{
			for (int i = 0; i < INVALID_CHARACTERS.Length; i++)
			{
				if (str.Contains(INVALID_CHARACTERS[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
