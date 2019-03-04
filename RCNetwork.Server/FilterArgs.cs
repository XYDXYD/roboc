using RCNetwork.Events;

namespace RCNetwork.Server
{
	internal struct FilterArgs
	{
		public readonly NetworkEvent type;

		public readonly int player;

		public FilterArgs(NetworkEvent pType, int pPlayer)
		{
			type = pType;
			player = pPlayer;
		}
	}
}
