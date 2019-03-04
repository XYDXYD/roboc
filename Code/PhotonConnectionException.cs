using System;

public class PhotonConnectionException : Exception
{
	public static string CODE_ERROR = CODEERROR.CONNECTION_EXCEPTION.ToString("d");

	public PhotonConnectionException(string message)
		: base(message)
	{
	}

	public PhotonConnectionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
