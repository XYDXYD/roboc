using System.Collections.Generic;

namespace ChatServiceLayer
{
	internal struct IgnoreUserResponse
	{
		public bool Successful;

		public string Message;

		public HashSet<string> UpdatedIgnoreList;
	}
}
