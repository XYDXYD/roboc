using System;

public class CCUCheckPassedException : Exception
{
	public CCUCheckPassedException(string message)
		: base(message)
	{
	}
}
