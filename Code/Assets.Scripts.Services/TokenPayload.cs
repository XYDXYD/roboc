using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services
{
	[Serializable]
	internal class TokenPayload
	{
		public string PublicId;

		public string DisplayName;

		public string RobocraftName;

		public string EmailAddress;

		public bool EmailVerified;

		public List<string> Flags;
	}
}
