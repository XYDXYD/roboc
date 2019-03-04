using System;

public class UserSuspendedException : Exception
{
	public static string CODE_ERROR = CODEERROR.USER_SUSPENDED.ToString("d");

	public UserSuspendedException(string message)
		: base(message)
	{
	}
}
