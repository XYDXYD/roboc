using System;

namespace WebServices
{
	internal class WebServerException : Exception
	{
		public WebServerException(string message)
			: base(message)
		{
		}
	}
}
