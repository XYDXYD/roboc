using System;

public class UnmanagedPhotonErrorException : Exception
{
	public static string CODE_ERROR = CODEERROR.UNKOWN_ERROR.ToString("d");

	public UnmanagedPhotonErrorException(string message)
		: base(message)
	{
	}
}
