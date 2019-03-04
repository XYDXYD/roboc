using System;

public class CCUExceededException : Exception
{
	public CCUExceededException(string message)
		: base(message)
	{
	}
}
