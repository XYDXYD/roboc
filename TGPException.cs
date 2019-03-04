using System;

internal class TGPException : Exception
{
	public static string CODE_ERROR = CODEERROR.TGP_EXCEPTION.ToString("d");

	public TGPException(string message)
		: base("Error code " + CODE_ERROR + ": " + message)
	{
	}
}
