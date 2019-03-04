using System;

public class DuplicateLoginException : Exception
{
	public static string CODE_ERROR = CODEERROR.DUPLICATE_LOGIN.ToString("d");

	public DuplicateLoginException(string message)
		: base("Error code " + CODE_ERROR + ": " + message)
	{
	}
}
