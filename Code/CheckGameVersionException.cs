using System;

internal class CheckGameVersionException : Exception
{
	public static string CODE_ERROR = CODEERROR.CHECK_GAME_EXCEPTION.ToString("d");

	public CheckGameVersionException(string message)
		: base("Error code " + CODE_ERROR + ": Checking Game Version Failed -" + message)
	{
	}

	public CheckGameVersionException(string message, Exception inner)
		: base("Error code " + CODE_ERROR + ": Checking Game Version Failed -" + message, inner)
	{
	}
}
