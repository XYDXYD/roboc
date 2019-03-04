using System.Collections.Generic;

namespace RCNetwork.Events
{
	internal static class NetworkEventHints
	{
		private static HashSet<NetworkEvent> _clearTokens = new HashSet<NetworkEvent>
		{
			NetworkEvent.BeginSync
		};

		public static bool IsClearToken(NetworkEvent type)
		{
			return _clearTokens.Contains(type);
		}
	}
}
